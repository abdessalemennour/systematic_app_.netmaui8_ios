
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.Model;
using SmartPharma5.Services;
using SmartPharma5.View;
using MvvmHelpers;
Après :
using MvvmHelpers;
using MvvmHelpers.Commands;
using SmartPharma5.Model;
using SmartPharma5.Services;
*/
using MvvmHelpers;
using MvvmHelpers.Commands;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.View;
using SmartPharma5.Model;
Après :
using SmartPharma5.Model;
using SmartPharma5.Services;
*/
using SmartPharma5.Model;
using SmartPharma5.Services;
using SmartPharma5.View;
//using Xamarin.Essentials;

namespace SmartPharma5.ViewModel
{
    public class CustomerViewModel : BaseViewModel
    {
        public AsyncCommand ExitCommand { get; set; }
        public AsyncCommand LogoutCommand { get; set; }
        public bool actpopup = false;
        public bool ActPopup { get => actpopup; set => SetProperty(ref actpopup, value); }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand<Partner> TapCommand { get; }
        public bool ispulltorefreshenabled = false;
        public bool IsPullToRefreshEnabled { get => ispulltorefreshenabled; set => SetProperty(ref ispulltorefreshenabled, value); }
        public Opportunity Opportunity { get; set; }
        private ObservableRangeCollection<Partner> clientList;

        public ObservableRangeCollection<Partner> ClientList { get => clientList; set => SetProperty(ref clientList, value); }

        private bool testLoad;
        public bool TestLoad { get => testLoad; set => SetProperty(ref testLoad, value); }

        public CustomerViewModel()
        {
            RefreshCommand = new AsyncCommand(Refresh);
            TapCommand = new AsyncCommand<Partner>(TapCommandAsync);
            ExitCommand = new AsyncCommand(Exit);
            LogoutCommand = new AsyncCommand(Logout);
            //ClientList = new ObservableRangeCollection<Partner>();
             Task.Run(async () => await LoadCustomer());

        }

        private async Task TapCommandAsync(object partner)
        {
            if (partner != null)
            {
                if (DbConnection.Connecter())
                {
                    uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                    Opportunity = new Opportunity((int)idagent, partner as Partner);
                    //await App.Current.MainPage.Navigation.PushAsync(new OpportunityView(Opportunity));
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "Ok");
                }
            }
        }


        private async Task LoadCustomer()
        {
            ClientList = new ObservableRangeCollection<Partner>();
            IsPullToRefreshEnabled = false;
            ActPopup = true;
            ClientList.Clear();
            await Task.Delay(500);
            int CrmGroupe = 0;
            int iduser = Preferences.Get("iduser", 0);
            var UMG = await User_Module_Groupe_Services.GetGroupeCRM(iduser);

            if (UMG != null)
            {
                CrmGroupe = UMG.IdGroup;
            }
            //await Task.Delay(2000);
            try
            {
                if (CrmGroupe == 32)
                {
                    uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                    var C = Task.Run(() => Partner.GetPartnaireByIdAgent(idagent));
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
                TestLoad = true;
            }

            ActPopup = false;
            IsPullToRefreshEnabled = true;
        }
        private async Task Refresh()
        {
            ClientList = new ObservableRangeCollection<Partner>();
            IsPullToRefreshEnabled = false;
            ActPopup= true;
            ClientList.Clear();
            int CrmGroupe = 0;
            await Task.Delay(500);
            int iduser = Preferences.Get("iduser", 0);
            var UMG = await User_Module_Groupe_Services.GetGroupeCRM(iduser);

            if (UMG != null)
            {
                CrmGroupe = UMG.IdGroup;
            }
            //await Task.Delay(2000);

            try
            {
                if (CrmGroupe == 32)
                {
                    uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                    var C = Task.Run(() => Partner.GetPartnaireByIdAgent(idagent));
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
                TestLoad = true;
            }
            ActPopup = false;
            //IsBusy = false;
            IsPullToRefreshEnabled = true;

        }
        private async Task Logout()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to lougout!", "Yes", "No");
            if (r)
            {
                Preferences.Set("idagent", Convert.ToUInt32(null));
                await App.Current.MainPage.Navigation.PushAsync(new LoginView());
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
