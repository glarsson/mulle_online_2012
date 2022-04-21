using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ComponentModel;
using System.Windows.Threading;
using System.IO;
using Mulle.Client.NetworkServiceReference;
using Mulle.Lib.Cards;

namespace Mulle.Client
{
    /// <summary>
    /// Interaction logic for LoungeWindow.xaml
    /// </summary>

    public partial class GameWindow : Window, INotifyPropertyChanged
    {

        // old code imported from previous projects
        public int z_index = 1000;
        public int ClickCounter = 0;
        public int[] RowCount = new int[3];
        public int[] ColCount = new int[3];
        public int[] OtherColCount = new int[3];
        public Deck deck = new Deck();
        public IList<Card> Player1Hand = new List<Card>();
        public Grid MainTableGrid = new Grid();
        public Grid PlayerCardsGrid = new Grid();
        public Grid OtherPlayerCardsGrid = new Grid();
        public List<Card> tempCardCompare = new List<Card>();
        #region INITIALIZE GRIDS
        ColumnDefinition MainTableGridCol1 = new ColumnDefinition();
        ColumnDefinition MainTableGridCol2 = new ColumnDefinition();
        ColumnDefinition MainTableGridCol3 = new ColumnDefinition();
        ColumnDefinition MainTableGridCol4 = new ColumnDefinition();
        RowDefinition MainTableGridRow1 = new RowDefinition();
        RowDefinition MainTableGridRow2 = new RowDefinition();

        ColumnDefinition PlayerCardsGridCol1 = new ColumnDefinition();
        ColumnDefinition PlayerCardsGridCol2 = new ColumnDefinition();
        ColumnDefinition PlayerCardsGridCol3 = new ColumnDefinition();
        ColumnDefinition PlayerCardsGridCol4 = new ColumnDefinition();
        ColumnDefinition PlayerCardsGridCol5 = new ColumnDefinition();
        ColumnDefinition PlayerCardsGridCol6 = new ColumnDefinition();
        ColumnDefinition PlayerCardsGridCol7 = new ColumnDefinition();
        ColumnDefinition PlayerCardsGridCol8 = new ColumnDefinition();
        RowDefinition PlayerCardsGridRow1 = new RowDefinition();

        ColumnDefinition OtherPlayerCardsGridCol1 = new ColumnDefinition();
        ColumnDefinition OtherPlayerCardsGridCol2 = new ColumnDefinition();
        ColumnDefinition OtherPlayerCardsGridCol3 = new ColumnDefinition();
        ColumnDefinition OtherPlayerCardsGridCol4 = new ColumnDefinition();
        ColumnDefinition OtherPlayerCardsGridCol5 = new ColumnDefinition();
        ColumnDefinition OtherPlayerCardsGridCol6 = new ColumnDefinition();
        ColumnDefinition OtherPlayerCardsGridCol7 = new ColumnDefinition();
        ColumnDefinition OtherPlayerCardsGridCol8 = new ColumnDefinition();
        RowDefinition OtherPlayerCardsGridRow1 = new RowDefinition();
        #endregion        
        // end of old code
        
        
        private StringBuilder gameChatOutputLog = new StringBuilder();
        private PlayerContract opponentContract = new PlayerContract();
        private PlayerContract localPlayerContract = new PlayerContract();


        private BitmapImage _opponentProfilePicture = new BitmapImage();
        public BitmapImage _OpponentProfilePicture
        {
            get { return _opponentProfilePicture; }
            set
            {
                if (_opponentProfilePicture != value)
                {
                    _opponentProfilePicture = value;
                    OnPropertyChanged("_OpponentProfilePicture");
                }
            }
        }

        private BitmapImage _localPlayerProfilePicture = new BitmapImage();
        public BitmapImage _LocalPlayerProfilePicture
        {
            get { return _localPlayerProfilePicture; }
            set
            {
                if (_localPlayerProfilePicture != value)
                {
                    _localPlayerProfilePicture = value;
                    OnPropertyChanged("_LocalPlayerProfilePicture");
                }
            }
        }        


        public GameWindow(string opponent)
        {
            InitializeComponent();
            this.DataContext = this;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            GlobalProperties._netWorkServiceModel.PropertyChanged += new PropertyChangedEventHandler(_netWorkServiceModel_PropertyChanged);
            opponentContract = GlobalProperties._client.GetPlayerDetails(opponent);
            localPlayerContract = GlobalProperties._client.GetPlayerDetails(GlobalProperties.localAlias);

            opponentNameLabel.Content = opponentContract.Alias;
            localPlayerNameLabel.Content = localPlayerContract.Alias;
            ShowProfilePictures();

            PutCardOnTable(GlobalProperties._client.RequestMainTableCards(GlobalProperties.localAlias), "main_table");
            PutCardOnTable(GlobalProperties._client.RequestPlayerHand(GlobalProperties.localAlias), "player_hand");
            PutCardOnTable(GlobalProperties._client.RequestPlayerHand(GlobalProperties.localAlias), "other_player_hand");
        }

        // ********************
        // ******TESTING*******
        // ********************
        // ********************
        private void PutCardOnTable(List<Card> cardlist, string action)
        {

            foreach (Card card in cardlist)
            {
                BitmapImage src = new BitmapImage();
                ImageBrush ib = new ImageBrush();
                Rectangle rec = new Rectangle();
                var button = new Button();
                TranslateTransform myTranslate = new TranslateTransform();

                // specify bitmap
                src.BeginInit();
                src.UriSource = new Uri(card.ImagePath, UriKind.Relative);
                src.EndInit();
                //import bitmap
                ib.ImageSource = src;
                //setup rectangle to hold card image
                rec.Width = 74;
                rec.Height = 98;
                //setup button
                button.Width = 76;
                button.Height = 100;
                //fill rectangle with image
                rec.Fill = ib;
                //fill button with rectangle/image
                button.Content = rec;

                //button.Click += CardClicked(sender, e, card);

                button.Click += (sender, eventArgs) =>
                {
                    CardClicked(card);
                };


                if (action == "main_table")
                {
                    PlayingTableStackPanel.Children.Add(button);
                    //ColCount[0]++;
                    //if (ColCount[0] == 4)
                    //{
                    //    RowCount[0]++;
                    //    ColCount[0] = 0;
                    //}
                }
                if (action == "player_hand")
                {
                    PlayerCardsStackPanel.Children.Add(button);
                }
                if (action == "other_player_hand")
                {
                    OtherPlayerCardsStackPanel.Children.Add(button);
                }
            }
        }

        public void CardClicked(Card card)
        {

            UpdateGameChatWindow((card.Suit.ToString() + card.Value.ToString()) + "\n");
            //DebugOutputTextbox.Text = (message_string + "\n" + DebugOutputTextbox.Text);

            tempCardCompare.Add(card);
            if (tempCardCompare.Count == 2)
                CompareTwoCards(tempCardCompare[0], tempCardCompare[1]);

        }

        public void CompareTwoCards(Card card1, Card card2)
        {
            UpdateGameChatWindow((ValueToInt(card1) + ValueToInt(card2)).ToString());

        }

        public int ValueToInt(Card card)
        {
            int tempInt = -1;
            string tempString;
            tempString = string.Format("{3}", card.Value);
            //tempInt = Convert.ToInt32(tempString);
            return tempInt;
        }
        // ********************
        // ******TESTING*******
        // ********END*********
        // ********************


       private void ChatEnterKeyHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GlobalProperties._client.PlayerToServerGameChat(GlobalProperties.localAlias, gameChatInput.Text);
                gameChatInput.Text = String.Empty;
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
        

        void _netWorkServiceModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
            var gameChatOutput = ((NetworkServiceCallbackHandler)sender).GameChatOutput;
            
            if (e.PropertyName == "GameChatOutput")
            {
                Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() => { UpdateGameChatWindow(gameChatOutput); })
                );
            }
        }

        void UpdateGameChatWindow(string gameChatUpdateString)
        {
            StringBuilder timestamp = new StringBuilder();
            timestamp.Append("[");
            timestamp.Append(DateTime.Now.ToString("HH:mm:ss"));
            timestamp.Append("] ");
            gameChatOutputLog.Append(timestamp);
            gameChatOutputLog.Append(gameChatUpdateString);
            gameChatOutput.Text = String.Empty;
            gameChatOutput.Text = gameChatOutputLog.ToString();
            //chatOutput.Focus();
            gameChatOutput.CaretIndex = gameChatOutput.Text.Length;
            gameChatOutput.ScrollToEnd();
            System.Windows.Threading.Dispatcher.Run();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Add logic to save and suspend game state at server side when client closes window
        }

        public void ShowProfilePictures()
        {
            // OPPONENT START (MUST REFACTOR THIS)
            byte[] tempByteArray = null;
            if (GlobalProperties.clientImageCache.Count != 0)
            {
                foreach (KeyValuePair<string, byte[]> item in GlobalProperties.clientImageCache)
                {
                    if (item.Key == opponentContract.Alias)
                    {
                        tempByteArray = item.Value;
                    }
                }
            }
            if (tempByteArray == null)
            {
                tempByteArray = GlobalProperties._client.DownloadProfilePicture(opponentContract.Alias);
                if (tempByteArray != null)
                {
                    GlobalProperties.clientImageCache.Add(opponentContract.Alias, tempByteArray);
                }
            }
            if (tempByteArray != null)
            {
                _OpponentProfilePicture = ImageFromBuffer(tempByteArray);
            }
            // OPPONENT END, START OF LOCAL PLAYER (MUST REFACTOR THIS)
            byte[] tempByteArray2 = null;
            if (GlobalProperties.clientImageCache.Count != 0)
            {
                foreach (KeyValuePair<string, byte[]> item in GlobalProperties.clientImageCache)
                {
                    if (item.Key == localPlayerContract.Alias)
                    {
                        tempByteArray2 = item.Value;
                    }
                }
            }
            if (tempByteArray2 == null)
            {
                tempByteArray2 = GlobalProperties._client.DownloadProfilePicture(localPlayerContract.Alias);
                if (tempByteArray2 != null)
                {
                    GlobalProperties.clientImageCache.Add(localPlayerContract.Alias, tempByteArray2);
                }
            }
            if (tempByteArray2 != null)
            {
                _LocalPlayerProfilePicture = ImageFromBuffer(tempByteArray2);
            }

        }

        public bool? ShowNotificationWindowYesNo(string header, string message)
        {
            Point locationFromScreen = this.Grid.PointToScreen(new Point(0, 0));
            PresentationSource source = PresentationSource.FromVisual(this);
            System.Windows.Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);
            NotificationWindowYesNo nw = new NotificationWindowYesNo(header, message);
            nw.Owner = Window.GetWindow(this);
            nw.Top = locationFromScreen.Y + this.Height - 180;
            nw.Left = locationFromScreen.X + this.Width - 202;
            nw.ShowDialog();
            return (nw.DialogResult);
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

        private void PlayerMenu_PutToTable_Click(object sender, RoutedEventArgs e)
        {
            //if (
        }

        private void PlayerMenu_Select_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuitem = e.Source as MenuItem;
            if (menuitem != null)
            {
                //MessageBox.Show(menuitem.Header.ToString(), "Name");
            }
            //menuitem.
        }

        private void TableMenu_Build_Click(object sender, RoutedEventArgs e)
        {

        }
        private void TableMenu_Take_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void SelectedCardsMenu_PutToTable_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void SelectedCards_TakeFromTable_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
