using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;


/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.View;
using MvvmHelpers;
using MvvmHelpers.Commands;
Après :
using SmartPharma5;
using SmartPharma5.Model;
using SmartPharma5.Commands;
*/
using SmartPharma5.Model;
using SmartPharma5.View;
//using Xamarin.Essentials;

namespace SmartPharma5.ViewModel
{
    internal class PaymentCustomerViewModel : BaseViewModel
    {
        public AsyncCommand ExitCommand { get; set; }
        public AsyncCommand LogoutCommand { get; set; }
        public bool actpopup = false;
        public bool ActPopup { get => actpopup; set => SetProperty(ref actpopup, value); }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand<Partner> TapCommand { get; }
        public Payment Payment { get; set; }
        private ObservableRangeCollection<Partner> clientList;

        public ObservableRangeCollection<Partner> ClientList { get => clientList; set => SetProperty(ref clientList, value); }


        private bool testCon = false;
        public bool TestCon { get => testCon; set => SetProperty(ref testCon, value); }

        private bool loading = false;
        public bool Loading { get => loading; set => SetProperty(ref loading, value); }
        public PaymentCustomerViewModel()
        {
        }
        public PaymentCustomerViewModel(Payment Payment)
        {
            RefreshCommand = new AsyncCommand(Refresh);
            TapCommand = new AsyncCommand<Partner>(TapCommandAsync);
            ExitCommand = new AsyncCommand(Exit);
            LogoutCommand = new AsyncCommand(Logout);
            ClientList = new ObservableRangeCollection<Partner>();
            Task.Run(() => LoadCustomer());
        }

        private async Task TapCommandAsync(object partner)
        {
            Loading = true;
            await Task.Delay(500);
            uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
            Payment = new Payment((int)idagent, partner as Partner);
            await App.Current.MainPage.Navigation.PushAsync(new PaymentView(Payment));
            Loading = false;
        }


        static async Task<bool> checkPermissionPayment(int id_user)
        {
            bool permissoion = false;
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "SELECT max(payment_partner) as payment_partner \r\nFROM atooerp_app_permission_temp inner join\r\natooerp_user_module_group usg on usg.group = atooerp_app_permission_temp.group\r\nwhere user =" + id_user + ";";

                try
                {

                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader["quiz_partner"]) == 1)
                        {
                            reader.Close();
                            return true;
                        }
                        reader.Close();

                    }

                }

                catch (Exception ex)
                {
                    return false;
                }

            }
            return false;

        }

        private async Task LoadCustomer()
        {
            ActPopup = true;
            //await Task.Delay(2000);
            ClientList.Clear();
            int iduser = Preferences.Get("iduser", 0);

            try
            {
                //if (!await checkPermissionPayment(iduser))
                //{
                //    uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                //    var C = Task.Run(() => Partner.GetPartnerListByAgent(idagent));
                //    ClientList = new ObservableRangeCollection<Partner>(await C);
                //}
                //else
                //{
                    var C = Task.Run(() => Partner.GetPartnaire());
                    ClientList = new ObservableRangeCollection<Partner>(await C);
                //}



                
            }
            catch (Exception ex)
            {
                ClientList = new ObservableRangeCollection<Partner>();
                TestCon = true;

                ClientList = new ObservableRangeCollection<Partner>();

            }

            ActPopup = false;
        }
        private async Task Refresh()
        {
            await Task.Delay(2000);
            ActPopup = true;
            ClientList.Clear();
            int iduser = Preferences.Get("iduser", 0);
            try
            {

                if (!await checkPermissionPayment(iduser))
                {
                    uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                    var C = Task.Run(() => Partner.GetPartnerListByAgent(idagent));
                    ClientList = new ObservableRangeCollection<Partner>(await C);
                }
                else
                {
                    var C = Task.Run(() => Partner.GetPartnaire());
                    ClientList = new ObservableRangeCollection<Partner>(await C);
                }

            }
            catch (Exception ex)
            {
                ClientList = new ObservableRangeCollection<Partner>();
                TestCon = true;

                ClientList = new ObservableRangeCollection<Partner>();

            }
            ActPopup = false;

        }
        private async Task Logout()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to lougout!", "Yes", "No");
            if (r)
            {
                Preferences.Set("idagent", Convert.ToUInt32(null));
                //await App.Current.MainPage.Navigation.PushAsync(new LoginView());
            }
        }

        private async Task Exit()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to exit!", "Yes", "No");
            if (r)
            {
                await App.Current.MainPage.Navigation.PopToRootAsync();
            }
        }
    }
}
