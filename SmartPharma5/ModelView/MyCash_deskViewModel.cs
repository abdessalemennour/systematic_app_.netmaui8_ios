using SmartPharma5.Model;
using SmartPharma5.View;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
//using Xamarin.Essentials;

namespace SmartPharma5.ViewModel
{
    internal class MyCash_deskViewModel :BaseViewModel
    {
        public AsyncCommand ExitCommand { get; set; }
        public AsyncCommand LogoutCommand { get; set; }
        public List<CashDesk> MyCash_deskList { get; set; }
        private decimal total;
        public AsyncCommand<CashDesk> CommandAddTransferCashDesk { get; }
        public AsyncCommand<CashDesk> CommandTransferCashDesk { get; }
        public AsyncCommand<CashDesk> CommandStateCashDesk { get; }
        public AsyncCommand<CashDesk> CommandLogCashDesk { get; }
        public decimal Total { get => total; set => SetProperty(ref total, value); }

        private bool testCon = false;
        public bool TestCon { get => testCon; set => SetProperty(ref testCon, value); }

        private bool loading = false;
        public bool Loading { get => loading; set => SetProperty(ref loading, value); }

        public MyCash_deskViewModel()
        {
            Title = "My Cash Desk";
            CommandAddTransferCashDesk = new AsyncCommand<CashDesk>(AddTransferCashDesk);
            CommandTransferCashDesk = new AsyncCommand<CashDesk>(TransferCashDesk);
            CommandStateCashDesk = new AsyncCommand<CashDesk>(StateCashDesk);
            CommandLogCashDesk = new AsyncCommand<CashDesk>(LogCashDesk);
           

            try
            {

                MyCash_deskList = new List<CashDesk>(CashDesk.GetMyCash_desk().Result);


            }
            catch (Exception ex)
            {
                MyCash_deskList = new List<CashDesk>();
                TestCon = true;
                return;

            }
            
            ExitCommand = new AsyncCommand(Exit);
            LogoutCommand = new AsyncCommand(Logout);
            total = Get_TotalAmount(MyCash_deskList);
        }
        public MyCash_deskViewModel(string c)
        {
            Title = "All Cash Desk";
            CommandAddTransferCashDesk = new AsyncCommand<CashDesk>(AddTransferCashDesk);
            CommandTransferCashDesk = new AsyncCommand<CashDesk>(TransferCashDesk);
            CommandStateCashDesk = new AsyncCommand<CashDesk>(StateCashDesk);
            CommandLogCashDesk = new AsyncCommand<CashDesk>(LogCashDesk);
            //MyCash_deskList = new ObservableCollection<CashDesk>(CashDesk.GetMyCash_desk());
            ExitCommand = new AsyncCommand(Exit);
            LogoutCommand = new AsyncCommand(Logout);
            try
            {
                
                MyCash_deskList = new List<CashDesk>(CashDesk.GetAllCash_desk().Result);

            }
            catch(Exception ex)
            {
                MyCash_deskList= new List<CashDesk>();
                TestCon = true;
                return;

            }
           
            total = Get_TotalAmount(MyCash_deskList);
        }
        public static Decimal Get_TotalAmount(List<CashDesk> list)
        {
            decimal sum = 0;
            foreach (CashDesk c in list)
            { sum += c.Amount; }
            return sum;
        }
        private async Task TransferCashDesk(CashDesk cd)
        {
            if (await DbConnection.Connecter3()) {
                UserDialogs.Instance.ShowLoading("Loading please wait ...");
                await Task.Delay(1000);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new MyTransferListView(cd)));
                UserDialogs.Instance.HideLoading();
            }
                
            else
                await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");
        }
        private async Task AddTransferCashDesk(CashDesk cd)
        {
            if (await DbConnection.Connecter3()) {
                UserDialogs.Instance.ShowLoading("Loading please wait ...");
                await Task.Delay(1000);

                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new TransferView(cd)));
                UserDialogs.Instance.HideLoading();
            }
                
            else
                await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");
        }
        private async Task StateCashDesk(CashDesk cd)
        {
            if (await DbConnection.Connecter3()) {
                UserDialogs.Instance.ShowLoading("Loading please wait ...");
                await Task.Delay(1000);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new Cash_deskStateView(cd)));
                UserDialogs.Instance.HideLoading();
            }
               
            
            else
                await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");
        }
        private async Task LogCashDesk(CashDesk cd)
        {
            if (await DbConnection.Connecter3()) {
                UserDialogs.Instance.ShowLoading("Loading please wait ...");
                await Task.Delay(1000);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new Cash_deskLogView(cd)));
                UserDialogs.Instance.HideLoading();
            }
               
           
            else
                await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");
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
