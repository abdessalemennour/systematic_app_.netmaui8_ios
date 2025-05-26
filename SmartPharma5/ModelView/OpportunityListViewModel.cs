
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.Model;

using SmartPharma5.View;
using MvvmHelpers;
Après :
using MvvmHelpers;
using MvvmHelpers.Commands;
using SmartPharma5.Model;
*/
using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.View;
using SmartPharma5.Model;
Après :
using SmartPharma5.Model;
using SmartPharma5.View;
*/
using SmartPharma5.Model;
using SmartPharma5.View;
//using Xamarin.Essentials;
//using static SmartPharma.Model.Opportunity;
//using Xamarin.Forms;

namespace SmartPharma5.ViewModel
{
    public class OpportunityListViewModel : BaseViewModel
    {
        //public Color ColorBackground { get; set; } = Color.FromArgb(204, 0, 1);
        public bool ispulltorefreshenabled = false;
        public bool IsPullToRefreshEnabled { get => ispulltorefreshenabled; set => SetProperty(ref ispulltorefreshenabled, value); }
        public bool actpopup = true;
        public bool ActPopup { get => actpopup; set => SetProperty(ref actpopup, value); }

        private bool testLoad;
        public bool TestLoad { get => testLoad; set => SetProperty(ref testLoad, value); }


        private bool loading;
        public bool Loading { get => loading; set => SetProperty(ref loading, value); }

        

        public AsyncCommand<SmartPharma5.Model.Opportunity.Collection> TapCommand { get; set; }
        public AsyncCommand RefreshCommand { get; }

        //private ObservableCollection<Opportunity> opportunityList;
        public AsyncCommand ExitCommand { get; set; }
        public AsyncCommand LogoutCommand { get; set; }
        public Opportunity Opportunity;
        private uint itemselected = 0;
        public uint ItemSelected { get => itemselected; set => SetProperty(ref itemselected, value); }

        private Color stateColor;
        public Color StateColor { get => stateColor; set => SetProperty(ref stateColor, value); }

        private string stateString;
        public string StateString { get => stateString; set => SetProperty(ref stateString, value); }
        private ObservableRangeCollection<SmartPharma5.Model.Opportunity.Collection> opportunitylist;
        public ObservableRangeCollection<SmartPharma5.Model.Opportunity.Collection> OpportunityList { get => opportunitylist; set => SetProperty(ref opportunitylist, value); }
        public int agentId = 0;


        private bool testConnection;
        public bool TestConnection { get => testConnection; set => SetProperty(ref testConnection, value); }
        public OpportunityListViewModel()
        {
            TestLoad = true;
        }
        public OpportunityListViewModel(uint agentid)
        {
            // TestLoad = true;
            ExitCommand = new AsyncCommand(Exit);
            LogoutCommand = new AsyncCommand(Logout);
            TapCommand = new AsyncCommand<SmartPharma5.Model.Opportunity.Collection>(TapCommandAsync);
            RefreshCommand = new AsyncCommand(Refresh);
            //OpportunityList = new ObservableRangeCollection<Collection>();

            /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
            Avant :
                        agentId =(int) agentid;

                        //Task.Run( ()=>  LoadOpportunities(agentId));
            Après :
                        agentId =(int) agentid;

                        //Task.Run( ()=>  LoadOpportunities(agentId));
            */
            agentId = (int)agentid;

            //Task.Run( ()=>  LoadOpportunities(agentId));
        }

        public async void LoadOpportunities(uint agentid)

        /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
        Avant :
                {

                    ActPopup = true;
        Après :
                {

                    ActPopup = true;
        */
        {

            ActPopup = true;

            if (OpportunityList != null)
                opportunitylist.Clear();
            else

                /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                Avant :
                                OpportunityList = new ObservableRangeCollection<SmartPharma5.Model.Opportunity.Collection>();




                            if (agentid != 0)
                Après :
                                OpportunityList = new ObservableRangeCollection<SmartPharma5.Model.Opportunity.Collection>();




                            if (agentid != 0)
                */
                OpportunityList = new ObservableRangeCollection<SmartPharma5.Model.Opportunity.Collection>();




            if (agentid != 0)
            {


                var O = Task.Run(() => SmartPharma5.Model.Opportunity.Collection.GetOpportunityByAgent(agentid));
                OpportunityList = new ObservableRangeCollection<SmartPharma5.Model.Opportunity.Collection>(await O);
            }
            else
            {
                var O = Task.Run(() => SmartPharma5.Model.Opportunity.Collection.GetOpportunities());
                OpportunityList = new ObservableRangeCollection<SmartPharma5.Model.Opportunity.Collection>(await O);
            }

            ActPopup = false;
        }
        public async Task Refresh()
        {
            bool Testcon = false;


            var P = Task.Run(() => DbConnection.Connecter3());
            Testcon = await P;
            if (Testcon == false)
            {


                TestLoad = true;
                OpportunityList = new ObservableRangeCollection<SmartPharma5.Model.Opportunity.Collection>();
                IsBusy = false;

                return;
            }
            OpportunityList = new ObservableRangeCollection<SmartPharma5.Model.Opportunity.Collection>();
            TestLoad = false;
            try
            {
                if (agentId != 0)
                {
                    var O = Task.Run(() => SmartPharma5.Model.Opportunity.Collection.GetOpportunityByAgent((uint)agentId));
                    OpportunityList = new ObservableRangeCollection<SmartPharma5.Model.Opportunity.Collection>(await O);

                }
                else
                {

                    var O = Task.Run(() => SmartPharma5.Model.Opportunity.Collection.GetOpportunities());

                    OpportunityList = new ObservableRangeCollection<SmartPharma5.Model.Opportunity.Collection>(await O);

                }

            }
            catch (Exception ex)
            {
                TestLoad = true;
            }


            ActPopup = false;
            IsBusy = false;

        }
        public async Task RefreshOnApp()
        {
            using (UserDialogs.Instance.Loading("Refreshing, please wait..."))
            {
                OpportunityList = new ObservableRangeCollection<SmartPharma5.Model.Opportunity.Collection>();
                IsPullToRefreshEnabled = false;
                ActPopup = true;

                OpportunityList.Clear();

                await Task.Delay(500);

                try
                {
                    if (agentId != 0)
                    {
                        var O = Task.Run(() => SmartPharma5.Model.Opportunity.Collection.GetOpportunityByAgent((uint)agentId));
                        OpportunityList = new ObservableRangeCollection<SmartPharma5.Model.Opportunity.Collection>(await O);
                    }
                    else
                    {
                        var O = Task.Run(() => SmartPharma5.Model.Opportunity.Collection.GetOpportunities());
                        OpportunityList = new ObservableRangeCollection<SmartPharma5.Model.Opportunity.Collection>(await O);
                    }
                }
                catch (Exception ex)
                {
                    TestLoad = true;
                }

                ActPopup = false;
                IsPullToRefreshEnabled = true;
            }
        }
        public async Task ChangeConnexionState()
        {


            if (!DbConnection.Connecter())
            {
                StateColor = Colors.Red;
                StateString = "Déconnecté";


            }
            else
            {
                StateColor = Colors.GreenYellow;
                StateString = "Connecté";

            }
            DbConnection.Deconnecter();
            await Task.Delay(5000);





        }
        public async Task TapCommandAsync(object obj)
        {
            TestLoad = false;
            Loading = true;
            await Task.Delay(500);
            if (obj != null)
            {
                if (await DbConnection.Connecter3())
                {
                    var opportunity = obj as SmartPharma5.Model.Opportunity.Collection;

                    Opportunity = new Opportunity(opportunity);
                    //Opportunity = new Opportunity();
                    //CurrentData.CurrentModule = "Opportunities";

                    //await Opportunity.GetOpportunityLine((int)o.Id); ;

                    await App.Current.MainPage.Navigation.PushAsync(new OpportunityView(Opportunity));
                }
                else
                {
                    TestConnection = true;
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
                await App.Current.MainPage.Navigation.PushAsync(new LoginView());
            }
        }

        private async Task Exit()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to exit!", "Yes", "No");
            if (r)
            {
                await App.Current.MainPage.Navigation.PopAsync();
            }
        }
    }
}
