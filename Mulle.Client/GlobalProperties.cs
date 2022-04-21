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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mulle.Client.NetworkServiceReference;

namespace Mulle.Client
{
    public static class GlobalProperties
    {
        public static NetworkServiceCallbackHandler _netWorkServiceModel { get; set; }
        public static NetworkServiceClient _client { get; set; }
        public static string localAlias { get; set; }
        public static int gameId { get; set; }
        //private Dictionary<string, byte[]>  clientImageCache;    public Dictionary<string, byte[]> ClientImageCache{ get {  if(clientImageCache == null) clientImageCache = new Dictionary<string, byte[]>() return clientImageCache;  }
        public static Dictionary<string, byte[]> clientImageCache { get; set; }
    }
}
