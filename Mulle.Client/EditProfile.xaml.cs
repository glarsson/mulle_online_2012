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
    public partial class EditProfile : Window, INotifyPropertyChanged
    {
        public byte[] editProfileByteArray;

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
        
        
        public EditProfile()
        {
            InitializeComponent();
            this.DataContext = this;
            PlayerContract localPlayerEntity = GlobalProperties._client.GetPlayerDetails(GlobalProperties.localAlias);
            aliasLabel.Content = localPlayerEntity.Alias;
            emailLabel.Content = localPlayerEntity.Email;
            rankLabel.Content = localPlayerEntity.Rank;
            winlossLabel.Content = localPlayerEntity.Win.ToString() + " / " + localPlayerEntity.Loss.ToString();

            // check Image Cache before requesting from server
            byte[] LoadProfileByteArray = null;
            if (GlobalProperties.clientImageCache.Count != 0)
            {
                foreach (KeyValuePair<string, byte[]> item in GlobalProperties.clientImageCache)
                {
                    if (item.Key == localPlayerEntity.Alias)
                    {
                        LoadProfileByteArray = item.Value;
                    }
                }
            }
            if (LoadProfileByteArray == null)
            {
                LoadProfileByteArray = GlobalProperties._client.DownloadProfilePicture(GlobalProperties.localAlias);
                if (LoadProfileByteArray != null)
                {
                    GlobalProperties.clientImageCache.Add(GlobalProperties.localAlias, LoadProfileByteArray);
                }
            }
            if (LoadProfileByteArray != null)
            {
                var bi = new BitmapImage();
                _ProfilePicture = ImageFromBuffer(LoadProfileByteArray);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (editProfileByteArray != null)
            {
                GlobalProperties._client.UploadProfilePicture(GlobalProperties.localAlias, editProfileByteArray);
                string tempSelection = null;
                if (GlobalProperties.clientImageCache.Count != 0)
                {
                    foreach (KeyValuePair<string, byte[]> item in GlobalProperties.clientImageCache)
                    {
                        if (item.Key == GlobalProperties.localAlias)
                        {
                            tempSelection = item.Key;
                        }
                    }
                    if (tempSelection != null)
                    {
                        GlobalProperties.clientImageCache.Remove(tempSelection);
                        GlobalProperties.clientImageCache.Add(GlobalProperties.localAlias, editProfileByteArray);
                        this.Close();
                    }
                }
                this.Close();
            }
        }

        private void EditPicture_Click(object sender, RoutedEventArgs e)
        {
            Stream checkStream = null;
            OpenFileDialog op = new OpenFileDialog();
            op.Multiselect = false;
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";
            BitmapImage bi = new BitmapImage();
            if ((bool)op.ShowDialog(Window.GetWindow(this)))
            {
              if ((checkStream = op.OpenFile()) != null)
                { 
                    bi = new BitmapImage(new Uri(op.FileName, UriKind.Absolute));
                    imgPhoto.Source = bi;
                    editProfileByteArray = ResizeImage(BufferFromImage(bi));
                }
            }
            
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
        public Byte[] BufferFromImage(BitmapImage imageSource)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageSource));
            // encoder.QualityLevel = 100;
            MemoryStream buffer = new MemoryStream();
            encoder.Save(buffer);
            return buffer.ToArray();
        }
    


    // TESTING FROM HERE WITH RESIZING

        private byte[] ResizeImage(byte[] inputBytes)
        {

            byte[] imageBytes = inputBytes;
            // decode the image such that its width is 120 and its
            // height is scaled proportionally
            ImageSource imageSource = CreateImage(imageBytes, 256, 256);
            // OTHER OPTIONS
            // the following will decode the image to its natural size
            // imageSource = CreateImage(imageBytes, 0, 0);
            // the following will decode the image such that its height
            // is 160 and its width is scaled proportionally
            // imageSource = CreateImage(imageBytes, 0, 160);
            // the following will decode the image to exactly 120 x 160
            // imageSource = CreateImage(imageBytes, 120, 160);
            imageBytes = GetEncodedImageData(imageSource, ".png");
            return imageBytes;
        }

        private static byte[] LoadImageData(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open,
                FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] imageBytes = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close();
            return imageBytes;
        }

        private static void SaveImageData(byte[] imageData,
            string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create,
                FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(imageData);
            bw.Close();
            fs.Close();
        }
 
        private static ImageSource CreateImage(byte[] imageData,
                int decodePixelWidth, int decodePixelHeight)
        {
            if (imageData == null) return null;
            BitmapImage result = new BitmapImage();
            result.BeginInit();
            if (decodePixelWidth > 0)
            {
                result.DecodePixelWidth = decodePixelWidth;
            }
            if (decodePixelHeight > 0)
            {
                result.DecodePixelHeight = decodePixelHeight;
            }
            result.StreamSource = new MemoryStream(imageData);
            result.CreateOptions = BitmapCreateOptions.None;
            result.CacheOption = BitmapCacheOption.Default;
            result.EndInit();
            return result;
        }

        internal byte[] GetEncodedImageData(ImageSource image,
                string preferredFormat)
        {
            byte[] result = null;
            BitmapEncoder encoder = null;
            switch (preferredFormat.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;
 
                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;
 
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
 
                case ".tif":
                case ".tiff":
                    encoder = new TiffBitmapEncoder();
                    break;
 
                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;
 
                case ".wmp":
                    encoder = new WmpBitmapEncoder();
                    break;
            }
 
            if (image is BitmapSource)
            {
                MemoryStream stream = new MemoryStream();
                encoder.Frames.Add(
                    BitmapFrame.Create(image as BitmapSource));
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                result = new byte[stream.Length];
                BinaryReader br = new BinaryReader(stream);
                br.Read(result, 0, (int)stream.Length);
                br.Close();
                stream.Close();
            }
            return result;
        }
    }


}
