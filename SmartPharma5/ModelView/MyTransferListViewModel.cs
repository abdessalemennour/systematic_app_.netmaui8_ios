using SmartPharma5.Model;
using SmartPharma5.View;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
//using Xamarin.Essentials;
//using Xamarin.Forms;
//using static SmartPharma.Model.Transfer;

namespace SmartPharma5.ViewModel
{
    internal class MyTransferListViewModel:BaseViewModel
    {
        public int iduser;
        public Transfer Transfer;
        public CashDesk Cd_in=new CashDesk();
        public bool ispulltorefreshenabled = false;
        public bool IsPullToRefreshEnabled { get => ispulltorefreshenabled; set => SetProperty(ref ispulltorefreshenabled, value); }
        public bool actpopup = true;
        public bool ActPopup { get => actpopup; set => SetProperty(ref actpopup, value); }

        public AsyncCommand<SmartPharma5.Model.Transfer.Collection> TapCommand { get; set; }
        public AsyncCommand RefreshCommand { get; }

        public AsyncCommand ExitCommand { get; set; }
        public AsyncCommand LogoutCommand { get; set; }
        private uint itemselected = 0;
        public uint ItemSelected { get => itemselected; set => SetProperty(ref itemselected, value); }
        private ObservableRangeCollection<Transfer.Collection> transferlist;
        public ObservableRangeCollection<Transfer.Collection> TransferList { get => transferlist; set => SetProperty(ref transferlist, value); }
        public int agentId = 0;

        private bool testCon = false;
        public bool TestCon { get => testCon; set => SetProperty(ref testCon, value); }


        private bool loading = false;
        public bool Loading { get => loading; set => SetProperty(ref loading, value); }

        public MyTransferListViewModel()
        {
            Title = "My Transfer";
            iduser = Preferences.Get("iduser", 0);
            ExitCommand = new AsyncCommand(Exit);
            //LogoutCommand = new AsyncCommand(Logout);
            TapCommand = new AsyncCommand<Transfer.Collection>(TapCommandAsync);
            RefreshCommand = new AsyncCommand(Refresh);
            TransferList = new ObservableRangeCollection<Transfer.Collection>();
            // agentId = (int)agentid;
            ExitCommand = new AsyncCommand(Exit);
            LogoutCommand = new AsyncCommand(Logout);

            Task.Run(() => LoadTransfer());
        }
        public MyTransferListViewModel(string ch)
        {
            Title = "All Transfer";
            iduser = Preferences.Get("iduser", 0);
            ExitCommand = new AsyncCommand(Exit);
            //LogoutCommand = new AsyncCommand(Logout);
            TapCommand = new AsyncCommand<Transfer.Collection>(TapCommandAsync);
            RefreshCommand = new AsyncCommand(Refresh);
            ExitCommand = new AsyncCommand(Exit);
            LogoutCommand = new AsyncCommand(Logout);
            TransferList = new ObservableRangeCollection<Transfer.Collection>();
            // agentId = (int)agentid;

            Task.Run(() => LoadAllTransfer());
        }
        public MyTransferListViewModel(int ch)
        {
            Title = "Waiting Transfer";
            iduser = Preferences.Get("iduser", 0);
            ExitCommand = new AsyncCommand(Exit);
            
            TapCommand = new AsyncCommand<Transfer.Collection>(TapCommandAsync);
            RefreshCommand = new AsyncCommand(Refresh);
            ExitCommand = new AsyncCommand(Exit);
            LogoutCommand = new AsyncCommand(Logout);
            TransferList = new ObservableRangeCollection<Transfer.Collection>();
           

            Task.Run(() => LoadAllTransferByWaiting());
        }
        public MyTransferListViewModel(CashDesk cd)
        {
            Title = "Transfer[ "+cd.Name+" ]";
            Cd_in = cd;
            iduser = Preferences.Get("iduser", 0);
            ExitCommand = new AsyncCommand(Exit);
            //LogoutCommand = new AsyncCommand(Logout);
            TapCommand = new AsyncCommand<Transfer.Collection>(TapCommandAsync);
            RefreshCommand = new AsyncCommand(Refresh);
            TransferList = new ObservableRangeCollection<Transfer.Collection>();
            // agentId = (int)agentid;

            Task.Run(() => LoadTransferByCash_desk_in());
        }
        public async void LoadAllTransferByWaiting()
        {
            ActPopup = true;
            try
            {
                var O = Task.Run(() => Transfer.GetCollectionListByWaiting());
                TransferList = new ObservableRangeCollection<Transfer.Collection>(await O);
            }
            catch(Exception ex)
            {
                TransferList = new ObservableRangeCollection<Transfer.Collection>();
                TestCon = true;
                ActPopup = false;
                return;

            }
          
            ActPopup = false;
        }
        public async void LoadAllTransfer()
        {
            ActPopup = true;
            try
            {
                var O = Task.Run(() => Transfer.GetCollectionList());
                TransferList = new ObservableRangeCollection<Transfer.Collection>(await O);
                ActPopup = false;

            }
            catch(Exception ex)
            {
                Console.Write(ex);
                TransferList = new ObservableRangeCollection<Transfer.Collection>();
                TestCon = true;
                
                ActPopup = false;
                

            }
            
            
        }
        public async void LoadTransfer()
        {
            ActPopup = true;
            try
            {
                var O = Task.Run(() => Transfer.GetCollectionList(iduser));
                TransferList = new ObservableRangeCollection<Transfer.Collection>(await O);
                ActPopup = false;
            }
            catch(Exception ex)
            {
                TransferList = new ObservableRangeCollection<Transfer.Collection>();
                TestCon = true;
                ActPopup = false;
                return;

            }
            
            
        }
        public async void LoadTransferByCash_desk_in()
        {
            ActPopup = true;
            try
            {
                var O = Task.Run(() => Transfer.GetCollectionListByCash_desk_in(iduser, Cd_in.Id));
                TransferList = new ObservableRangeCollection<Transfer.Collection>(await O);
                ActPopup = false;

            }
            catch(Exception ex)
            {
                TransferList = new ObservableRangeCollection<Transfer.Collection>();
                TestCon = true;
                ActPopup = false;
                return;
            }
            
            
        }
        public async Task Refresh()
        {
            TransferList.Clear();
            await Task.Delay(500);

            try
            {
                if (Cd_in.Id > 0)
                {
                    var O = Model.Transfer.GetCollectionListByCash_desk_in(iduser, Cd_in.Id);
                    TransferList = new ObservableRangeCollection<Transfer.Collection>(await O);
                }
                else
                {
                    var S = Model.Transfer.GetCollectionList(iduser);
                    TransferList = new ObservableRangeCollection<Transfer.Collection>(await S);
                }
            }
            catch (Exception ex)
            {
                TransferList = new ObservableRangeCollection<Transfer.Collection>();
                TestCon = true;
                IsBusy = false;
                return;
            }
            ActPopup = false;
            IsBusy = false;

        }
        public async Task RefreshOnApp()
        {
            //IsBusy = true;
            IsPullToRefreshEnabled = false;
            ActPopup = true;
            TransferList.Clear();

            await Task.Delay(500);
            //ClientList.Clear();
            //if (agentId != 0)
            /*{
                var O = Task.Run(() => Collection.GetOpportunityByAgent((uint)agentId));
                TransferList = new ObservableRangeCollection<Collection>(await O);

            }
            else
            {*/
            try
            {
                if (Cd_in.Id > 0)
                {
                    var O = Model.Transfer.GetCollectionListByCash_desk_in(iduser, Cd_in.Id);
                    TransferList = new ObservableRangeCollection<Transfer.Collection>(await O);
                }
                else
                {
                    var S = Model.Transfer.GetCollectionList(iduser);
                    TransferList = new ObservableRangeCollection<Transfer.Collection>(await S);
                }

            }
            catch(Exception ex)
            {
                TransferList = new ObservableRangeCollection<Transfer.Collection>();
                TestCon = true;
                ActPopup = false;
                return;
            }
          

            //}
            ActPopup = false;
            //IsBusy = false;
            IsPullToRefreshEnabled = true;
        }
        public async Task TapCommandAsync(object obj)
        {
            Loading = true;
            await Task.Delay(500);
            
            if (obj != null)
            {
                if (DbConnection.Connecter())
                {
                    var Tr = obj as Transfer.Collection;

                    Transfer transfer = new Transfer(Tr.Id);
                    //-------------------------
                    //await App.Current.MainPage.Navigation.PushAsync(new TransferStateView(transfer));
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "Ok");
                }
            }
            Loading = false;
        }

        private async Task Logout()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to lougout!", "Yes", "No");
            if (r)
            {
                Preferences.Set("idagent", Convert.ToUInt32(null));
                await Society.ClearSocietiesCache();
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
