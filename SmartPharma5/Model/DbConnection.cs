using MySqlConnector;
using Plugin.Connectivity;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
//using Xamarin.Essentials;
#if ANDROID
using Android.OS;
#endif
namespace SmartPharma5.Model
{
    public static class DbConnection
    {
        public static string Database = Preferences.Get("database", "");
        public static string Address = Preferences.Get("address", "");
        public static string Name = Preferences.Get("name", "");
        public static string Password = Preferences.Get("password", "");
        public static int Port = Preferences.Get("port", 0);
        //public readonly static string Database = "atooerp_test_app_2";
        //public readonly static string Addresse = "141.94.240.89";
        //public readonly static int Port = 3306;
        public static string ConnectionString = "Server=" + Address + ";Port=" + Port + ";database=" + Database + ";Uid=" + Name + "; Pwd=" + Password + ";Allow User Variables=true;Connect Timeout = 3;";
        public static MySqlConnection con = new MySqlConnection(ConnectionString);

        public static async Task ErrorConnection()
        {
            await App.Current.MainPage.DisplayAlert("Warning", "Failed connection", "OK");
        }


        public static void Update()
        {
            Database = Preferences.Get("database", "");
            Address = Preferences.Get("address", "");
            Name = Preferences.Get("name", "");
            Password = Preferences.Get("password", "");
            Port = Preferences.Get("port", 0);
            ConnectionString = "Server=" + Address + ";Port=" + Port + ";database=" + Database + ";Uid=" + Name + "; Pwd=" + Password + ";Allow User Variables=true;Connect Timeout = 3;";
            con = new MySqlConnection(ConnectionString);
        }
        public static bool CheckConnectivity()
        {
            bool IsConnected = CrossConnectivity.Current.IsConnected;

            return IsConnected;
        }
        public static bool ConnectionIsTrue()
        {
            try
            {
                con.Close();
                con.Open();
                con.Close();
                return true;
            }
            catch (Exception ex)
            { return false; }
        }

        public static bool ConnectionIsTrue1()
        {
            try
            {
                con.Close();
                con.Open();
                con.Close();
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        public static bool Connecter()
        {
            try
            {
                Deconnecter();
                if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
                {
                    con.Open();
                    return true;
                }
            }
            catch (Exception EX)
            {
                //ErrorConnection();
            }

            return false;
        }
        public static void Deconnecter()
        {
            if (con.State == ConnectionState.Open || con.State == ConnectionState.Connecting)
            {

                con.Close();
            }
        }
        public static bool checkContectivityToTheServeur()
        {
            if (con.State == ConnectionState.Open)
            {

                return true;
            }
            else
            {
                return false;
            }


        }


        public async static Task<bool> Connecter3()
        {

            try
            {
                Deconnecter();
                if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
                {
                    con.Open();
                    return true;
                }
            }
            catch (Exception EX)
            {
                //ErrorConnection();
            }

            return false;
        }
        public static string macAdresse()
        {

            var ni = NetworkInterface.GetAllNetworkInterfaces()
                .OrderBy(intf => intf.NetworkInterfaceType)
                .FirstOrDefault(intf => intf.OperationalStatus == OperationalStatus.Up
                      && (intf.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                          || intf.NetworkInterfaceType == NetworkInterfaceType.Ethernet));
            if (ni != null)
            {
                var hw = ni.GetPhysicalAddress();
                return string.Join(":", (from ma in hw.GetAddressBytes() select ma.ToString("X2")).ToArray());
            }
            return "";
        }


        public static string MachineName()
        {
#if ANDROID
            return $"{Build.Manufacturer} {Build.Model}";
#elif IOS
                    return UIKit.UIDevice.CurrentDevice.Name;
#else
            return Environment.MachineName;
#endif
        }

        public static string IpAddress()
        {
            var IpAddress = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault();

            if (IpAddress != null)
            {
                return IpAddress.ToString();
            }
            return "";
        }


        public static bool Connecter2()
        {
            try
            {
                Deconnecter();
                if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
                {
                    con.Open();
                    return true;
                }
            }
            catch (Exception EX)
            {
                //ErrorConnection();
            }

            return false;
        }
    }
}