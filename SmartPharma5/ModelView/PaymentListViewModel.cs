
using SmartPharma5.Model;
using SmartPharma5.View;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Threading.Tasks;
using static SmartPharma5.Model.Payment;
using System;

namespace SmartPharma5.ViewModel
{
    public class PaymentListViewModel : BaseViewModel
    {
        public Payment Payment;
        public bool ispulltorefreshenabled = false;
        public bool IsPullToRefreshEnabled { get => ispulltorefreshenabled; set => SetProperty(ref ispulltorefreshenabled, value); }
        public bool actpopup = true;
        public bool ActPopup { get => actpopup; set => SetProperty(ref actpopup, value); }

        public AsyncCommand<Collection> TapCommand { get; set; }
        public AsyncCommand RefreshCommand { get; }

        public AsyncCommand ExitCommand { get; set; }
        public AsyncCommand LogoutCommand { get; set; }
        private uint itemselected = 0;
        public uint ItemSelected { get => itemselected; set => SetProperty(ref itemselected, value); }
        private ObservableRangeCollection<Collection> paymentlist;
        public ObservableRangeCollection<Collection> PaymentList { get => paymentlist; set => SetProperty(ref paymentlist, value); }
        public int agentId = 0;

        private bool testCon = false;
        public bool TestCon { get => testCon; set => SetProperty(ref testCon, value); }


        private bool loading = false;
        public bool Loading { get => loading; set => SetProperty(ref loading, value); }



        public PaymentListViewModel()
        {
            ExitCommand = new AsyncCommand(Exit);
            //LogoutCommand = new AsyncCommand(Logout);
            TapCommand = new AsyncCommand<Collection>(TapCommandAsync);
            RefreshCommand = new AsyncCommand(Refresh);
            PaymentList = new ObservableRangeCollection<Collection>();
            // agentId = (int)agentid;

            Task.Run(() => LoadPayment());
        }
        public PaymentListViewModel(int agentid)
        {
            ExitCommand = new AsyncCommand(Exit);
            //LogoutCommand = new AsyncCommand(Logout);
            TapCommand = new AsyncCommand<Collection>(TapCommandAsync);
            RefreshCommand = new AsyncCommand(Refresh);
            PaymentList = new ObservableRangeCollection<Collection>();
            agentId = (int)agentid;

            Task.Run(() => LoadPayment());
        }
        public async void LoadPayment()
        {

            ActPopup = true;

            /*if (PaymentList != null)
                paymentlist.Clear();*/

            try
            {
                if (agentId != 0)
                {

                    var O = Task.Run(() => Payment.GetCollectionListByAgent(agentId));
                    PaymentList = new ObservableRangeCollection<Collection>(await O);
                }
                else
                {
                    var O = Task.Run(() => Model.Payment.GetCollectionList());
                    PaymentList = new ObservableRangeCollection<Collection>(await O);
                }

            }
            catch (Exception ex)
            {
                PaymentList = new ObservableRangeCollection<Collection>();
                TestCon = true;



            }

            ActPopup = false;
        }

     /*   public async Task Refresh()
        {
            PaymentList.Clear();
            await Task.Delay(500);
            //ClientList.Clear();
            /*if (agentId != 0)
            {
                var O = Task.Run(() => Collection.GetOpportunityByAgent((uint)agentId));
                PaymentList = new ObservableRangeCollection<Collection>(await O);

            }
            else
            {*/

           /* var O = Model.Payment.GetCollectionList();

            PaymentList = new ObservableRangeCollection<Collection>(await O);

            //}
            ActPopup = false;
            IsBusy = false;

        }*/

                 public async Task Refresh()
               {
                   PaymentList.Clear();
                   await Task.Delay(500);
                   //ClientList.Clear();
                  if (agentId != 0)
                   {
                       var O = Task.Run(() => Payment.GetCollectionListByAgent(agentId));
                       PaymentList = new ObservableRangeCollection<Collection>(await O);

                   }
                   else
                   {

                   var O = Model.Payment.GetCollectionList();

                   PaymentList = new ObservableRangeCollection<Collection>(await O);

                   }
                   ActPopup = false;
                   IsBusy = false;

               }
        public async Task RefreshOnApp()
        {
            //IsBusy = true;
            IsPullToRefreshEnabled = false;
            ActPopup = true;
            PaymentList.Clear();

            await Task.Delay(500);
            //ClientList.Clear();
            //if (agentId != 0)
            /*{
                var O = Task.Run(() => Collection.GetOpportunityByAgent((uint)agentId));
                PaymentList = new ObservableRangeCollection<Collection>(await O);

            }
            else
            {*/

            var O = Task.Run(() => Model.Payment.GetCollectionList());

            PaymentList = new ObservableRangeCollection<Collection>(await O);

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
                    var payment = obj as Collection;

                    Payment = new Payment(payment.Id);

                    await App.Current.MainPage.Navigation.PushAsync(new PayView(Payment));
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "Ok");
                }
            }
            Loading = false;
        }

        /*private async Task Logout()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to lougout!", "Yes", "No");
            if (r)
            {
                Preferences.Set("idagent", Convert.ToUInt32(null));
                await App.Current.MainPage.Navigation.PushAsync(new LoginView());
            }
        }*/

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
