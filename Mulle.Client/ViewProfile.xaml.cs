using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;
using Mulle.Client.NetworkServiceReference;

namespace Mulle.Client
{
    /// <summary>
    /// Interaction logic for EditProfile.xaml
    /// </summary>
    public partial class ViewProfile : Window, INotifyPropertyChanged
    {
        public byte[] playerProfilePicByteArray;

        private BitmapImage _profilePicture = new BitmapImage();
        public BitmapImage _ProfilePicture
        {
            get { return _profilePicture; }
            set
            {
                if (_profilePicture != value)
                {
                    _profilePicture = value;
                    OnPropertyChanged("_ProfilePicture");
                }
            }
        }        
        
        
        public ViewProfile(string alias)
        {
            InitializeComponent();
            this.DataContext = this;

            PlayerContract localPlayerEntity = GlobalProperties._client.GetPlayerDetails(alias);
            aliasLabel.Content = localPlayerEntity.Alias;
            emailLabel.Content = localPlayerEntity.Email;
            rankLabel.Content = localPlayerEntity.Rank;
            winlossLabel.Content = localPlayerEntity.Win.ToString() + " / " + localPlayerEntity.Loss.ToString();

            byte[] tempByteArray = null;
            if (GlobalProperties.clientImageCache.Count != 0)
            {
                foreach (KeyValuePair<string, byte[]> item in GlobalProperties.clientImageCache)
                {
                    if (item.Key == alias)
                    {
                        tempByteArray = item.Value;
                    }
                }
            }
            if (tempByteArray == null)
            {
                tempByteArray = GlobalProperties._client.DownloadProfilePicture(alias);
                if (tempByteArray != null)
                {
                    GlobalProperties.clientImageCache.Add(alias, tempByteArray);
                }
            }
            if (tempByteArray != null)
            {
                var bi = new BitmapImage();
                _ProfilePicture = ImageFromBuffer(tempByteArray);
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
            {
                var handler = PropertyChanged;
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public BitmapImage ImageFromBuffer(Byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            stream.Position = 0;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        private void RequestGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RequestFriend_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Ignore_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
