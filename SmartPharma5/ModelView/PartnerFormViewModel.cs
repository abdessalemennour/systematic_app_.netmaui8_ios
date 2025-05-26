/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.Model;
using SmartPharma5.View;
Après :
using DevExpress.Maui.View;
*/
//using Xamarin.Essentials;
//using Acr.UserDialogs;
using Acr.UserDialogs;
//using Android.Content;
using DevExpress.Maui.Scheduler;
//using DevExpress.XamarinForms.DataGrid;
using MvvmHelpers;
using MvvmHelpers.Commands;
using SmartPharma5.View;
using System.Globalization;
using
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
//using SmartPharma.Model;
Après :
using SmartPharma5.Model;
using SmartPharma5.ModelView;
using SmartPharma5.View;
//using SmartPharma.Model;
*/
SmartPharma5.Model;
using SQLite;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System.Data;
using System.Text;
using System.Threading.Tasks;
Après :
using System.Collections.ObjectModel;
using System.Data;
*/
//using 
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System.Collections.ObjectModel;
using SmartPharma5.ModelView;
Après :
using System.Text;
using System.Threading.Tasks;
*/

using System.Data;
using System.Windows.Input;
using Microsoft.Maui.Graphics;

namespace SmartPharma5.ViewModel
{
    public class PartnerFormViewModel : BaseViewModel
    {
        public int opp = 0;
        public int partnerId = 0;
        public string names = "";
        public MvvmHelpers.Commands.Command<Partner_Form> TapCommand { get; }
        public MvvmHelpers.Commands.Command<Partner_Form.Collection> ConfigBtnCommand { get; }
        public MvvmHelpers.Commands.Command LocalisationCommand { get; }

        public AsyncCommand goLocalisation { get; set; }
        private bool _isGpsAvailable;
        public bool IsGpsAvailable
        {
            get => _isGpsAvailable;
            set => SetProperty(ref _isGpsAvailable, value);
        }

    
    private ObservableRangeCollection<Partner_Form.Collection> partnerFormList;

        public ObservableRangeCollection<Partner_Form.Collection> PartnerFormList { get => partnerFormList; set => SetProperty(ref partnerFormList, value); }

        private ObservableRangeCollection<string> statelist;
        public ObservableRangeCollection<string> StateList { get => statelist; set => SetProperty(ref statelist, value); }


        private ObservableRangeCollection<Cycle> cyclelist;
        public ObservableRangeCollection<Cycle> CycleList { get => cyclelist; set => SetProperty(ref cyclelist, value); }


        private ObservableRangeCollection<Partner> partnerlist;
        public ObservableRangeCollection<Partner> PartnerList { get => partnerlist; set => SetProperty(ref partnerlist, value); }

        private ObservableRangeCollection<string> listOfForm;
        public ObservableRangeCollection<string> ListOfForm { get => listOfForm; set => SetProperty(ref listOfForm, value); }

        private ObservableRangeCollection<Empolye> empolyelist;
        public ObservableRangeCollection<Empolye> EmpolyeList { get => empolyelist; set => SetProperty(ref empolyelist, value); }


        public bool ispulltorefreshenabled = false;
        public bool IsPullToRefreshEnabled { get => ispulltorefreshenabled; set => SetProperty(ref ispulltorefreshenabled, value); }


        public AsyncCommand RefreshCommand { get; }


        public bool actpopup = true;
        public bool ActPopup { get => actpopup; set => SetProperty(ref actpopup, value); }

        private bool testLoad;
        public bool TestLoad { get => testLoad; set => SetProperty(ref testLoad, value); }

        public string StrTest = "";

        private bool loading;
        public bool Loading { get => loading; set => SetProperty(ref loading, value); }


        private bool isFiltred;
        public bool IsFiltred { get => isFiltred; set => SetProperty(ref isFiltred, value); }


        private bool isCreated;
        public bool IsCreated { get => isCreated; set => SetProperty(ref isCreated, value); }

        private Color _buttonColor = Color.FromHex("#4CAF50"); // Couleur par défaut
        public Color ButtonColor
        {
            get => _buttonColor;
            set => SetProperty(ref _buttonColor, value);
        }

        public PartnerFormViewModel()
        {

        }



        public PartnerFormViewModel(int opp)
        {

            this.IsFiltred = false;
            IsCreated = true;
            this.opp = opp;

            ConfigBtnCommand = new MvvmHelpers.Commands.Command<Partner_Form.Collection>(Config);


            RefreshCommand = new AsyncCommand(Refresh);

            Task.Run(async () => LoadPartnerFormByOpp(opp).GetAwaiter());



        }
        public PartnerFormViewModel(string type)
        {

            this.IsFiltred = true;
            IsCreated = false;



            ConfigBtnCommand = new MvvmHelpers.Commands.Command<Partner_Form.Collection>(Config);
            this.StrTest = type;

            RefreshCommand = new AsyncCommand(Refresh);
            if (this.StrTest == "My_Forms")
            {
                Task.Run(async () => LoadMyPartnerForm().GetAwaiter());

            }
            else
            {
                Task.Run(async () => LoadAllPartnerForm().GetAwaiter());

            }
        }


        public async Task localisationFunction(int id_partner_form, uint id_partner)
        {
            UserDialogs.Instance.ShowLoading("Loading Please wait ...");
            await Task.Delay(500);

            try
            {
                if (await DbConnection.Connecter3())
                {
                    string gps1 = await Partner_Form.GetGpsCoordinatesFromDatabase(id_partner_form);
                    string gps2 = await Partner.GetGpsCoordinates(id_partner);

                    // Mettre à jour la couleur du bouton
                    if (!string.IsNullOrEmpty(gps1) && !string.IsNullOrEmpty(gps2))
                    {
                        ButtonColor = Color.FromHex("#0000FF"); // Bleu
                    }
                    else if (string.IsNullOrEmpty(gps1) && string.IsNullOrEmpty(gps2))
                    {
                        ButtonColor = Color.FromHex("#000000"); // Noir
                    }
                    else if (!string.IsNullOrEmpty(gps1))
                    {
                        ButtonColor = Color.FromHex("#FF0000"); // Rouge
                    }
                    else if (!string.IsNullOrEmpty(gps2))
                    {
                        ButtonColor = Color.FromHex("#00FF00"); // Vert
                    }

                    // Si une des coordonnées est disponible
                    if (!string.IsNullOrEmpty(gps1) || !string.IsNullOrEmpty(gps2))
                    {
                        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new LocationView(gps1, gps2)));
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Erreur", "Impossible de récupérer les coordonnées", "OK");
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "OK");
                }
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }


        private async Task Refresh()
        {
            bool Testcon = false;
            ActPopup = true;
            var P = Task.Run(() => DbConnection.Connecter3());
            Testcon = await P;
            if (Testcon == false)
            {


                TestLoad = true;
                PartnerFormList = new ObservableRangeCollection<Partner_Form.Collection>();
                IsBusy = false;

                return;
            }
            PartnerFormList = new ObservableRangeCollection<Partner_Form.Collection>();
            TestLoad = false;
            try
            {
                if (this.opp == 0)
                {
                    if (this.StrTest == "My_Forms")
                    {
                        var C = Task.Run(() => Partner_Form.GetMyPartnerForm());
                        var c = await C;
                        PartnerFormList = new ObservableRangeCollection<Partner_Form.Collection>(c.OrderBy(o => Math.Abs(o.dateDiff.TotalDays)));
                        var A = Task.Run(() => Cycle.GetAllCycle());
                        CycleList = new ObservableRangeCollection<Cycle>(await A);
                        uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                        PartnerList = new ObservableRangeCollection<Partner>(PartnerFormList.GroupBy(p => p.Partner_id).Select(g => new Partner((uint)g.Key, g.Max(x => x.Partner_name))));
                        StateList = new ObservableRangeCollection<String>(PartnerFormList.Select(x => x.state).Distinct());
                        ListOfForm = new ObservableRangeCollection<String>(PartnerFormList.Select(x => x.Form_name).Distinct());
                    }
                    else
                    {

                        var C = Task.Run(() => Partner_Form.GetAllPartnerForm());
                        var c = await C;
                        PartnerFormList = new ObservableRangeCollection<Partner_Form.Collection>(c.OrderBy(o => Math.Abs(o.dateDiff.TotalDays)));
                        var A = Task.Run(() => Cycle.GetAllCycle());
                        CycleList = new ObservableRangeCollection<Cycle>(await A);
                        //var D = Task.Run(() => Partner.GetPartnaire());
                        PartnerList = new ObservableRangeCollection<Partner>(PartnerFormList.GroupBy(p => p.Partner_id).Select(g => new Partner((uint)g.Key, g.Max(x => x.Partner_name))));
                        EmpolyeList = new ObservableRangeCollection<Empolye>(PartnerFormList.GroupBy(p => p.Agent_id).Select(g => new Empolye((int)(g.Key), g.Max(x => x.Agent_name))));
                        StateList = new ObservableRangeCollection<String>(PartnerFormList.Select(x => x.state).Distinct());
                        ListOfForm = new ObservableRangeCollection<String>(PartnerFormList.Select(x => x.Form_name).Distinct());

                    }
                }
                else
                {
                    var C = Task.Run(() => Partner_Form.GetPartnerFormByOpp(opp));
                    var c = await C;
                    PartnerFormList = new ObservableRangeCollection<Partner_Form.Collection>(c.OrderBy(o => Math.Abs(o.dateDiff.TotalDays)));
                    var A = Task.Run(() => Cycle.GetAllCycle());
                    CycleList = new ObservableRangeCollection<Cycle>(await A);
                    uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                    PartnerList = new ObservableRangeCollection<Partner>(PartnerFormList.GroupBy(p => p.Partner_id).Select(g => new Partner((uint)g.Key, g.Max(x => x.Partner_name))));
                    StateList = new ObservableRangeCollection<String>(PartnerFormList.Select(x => x.state).Distinct());
                    ListOfForm = new ObservableRangeCollection<String>(PartnerFormList.Select(x => x.Form_name).Distinct());

                }

            }
            catch (Exception ex)
            {
                TestLoad = true;
            }
            ActPopup = false;
            IsBusy = false;

        }



        public PartnerFormViewModel(Partner partner)
        {
            if (partner != null)
                partnerId = Convert.ToInt32(partner.Id);
            //Task.Run(() => LoadPartnerForm());



        }

        private async void Config(Partner_Form.Collection obj)
        {


            UserDialogs.Instance.Toast("Configuration Quiz ...");
            await Task.Delay(200);

            if (await DbConnection.Connecter3())
            {

                try
                {
                    Loading = true;
                    await Task.Delay(1000);
                    SchedulerDataStorage dataStorage = new SchedulerDataStorage();
                    AppointmentItem appointment = new AppointmentItem
                    {
                        Id = Convert.ToInt32(obj.Id),
                        End = obj.Estimated_date,
                        Start = obj.Estimated_date,
                        LabelId = obj.Id_Calender,
                        Subject = obj.Partner_name
                    };
                    //------------------Return-----------------------------------------

                    View.AppointmentEditPage appEditPage = new View.AppointmentEditPage(appointment, dataStorage);

                    await App.Current.MainPage.Navigation.PushAsync(appEditPage);
                    Loading = false;
                }
                catch (Exception ex)
                {
                    await DbConnection.ErrorConnection();
                    UserDialogs.Instance.HideLoading();
                }

            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "OK");

            }


        }

        private async Task LoadPartnerForm()
        {
            var C = Task.Run(() => Partner_Form.GetPartnerFormByIdPartner(partnerId));

            var c = await C;
            PartnerFormList = new ObservableRangeCollection<Partner_Form.Collection>(c.OrderBy(o => Math.Abs(o.dateDiff.TotalDays)));
            ListOfForm = new ObservableRangeCollection<String>(PartnerFormList.Select(x => x.Form_name).Distinct());

        }
        public async Task LoadMyPartnerForm()
        {
            PartnerFormList = new ObservableRangeCollection<Partner_Form.Collection>();
            IsPullToRefreshEnabled = false;
            ActPopup = true;
            PartnerFormList.Clear();

            await Task.Delay(500);
            try
            {
                var C = Task.Run(() => Partner_Form.GetMyPartnerForm());
                var c = await C;
                PartnerFormList = new ObservableRangeCollection<Partner_Form.Collection>(c.OrderBy(o => Math.Abs(o.dateDiff.TotalDays)));
                var A = Task.Run(() => Cycle.GetAllCycle());
                CycleList = new ObservableRangeCollection<Cycle>(await A);
                uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                PartnerList = new ObservableRangeCollection<Partner>(PartnerFormList.GroupBy(p => p.Partner_id).Select(g => new Partner((uint)g.Key, g.Max(x => x.Partner_name))));
                StateList = new ObservableRangeCollection<String>(PartnerFormList.Select(x => x.state).Distinct());
                ListOfForm = new ObservableRangeCollection<String>(PartnerFormList.Select(x => x.Form_name).Distinct());
            }
            catch (Exception ex)
            {
                TestLoad = true;

            }



            ActPopup = false;
            //IsBusy = false;
            IsPullToRefreshEnabled = true;

            //FillState();
        }
        public async Task LoadPartnerFormByOpp(int opp)
        {
            PartnerFormList = new ObservableRangeCollection<Partner_Form.Collection>();
            IsPullToRefreshEnabled = false;
            ActPopup = true;
            PartnerFormList.Clear();

            await Task.Delay(500);
            try
            {
                var C = Task.Run(() => Partner_Form.GetPartnerFormByOpp(opp));
                var c = await C;
                PartnerFormList = new ObservableRangeCollection<Partner_Form.Collection>(c.OrderBy(o => Math.Abs(o.dateDiff.TotalDays)));
                var A = Task.Run(() => Cycle.GetAllCycle());
                CycleList = new ObservableRangeCollection<Cycle>(await A);
                uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                PartnerList = new ObservableRangeCollection<Partner>(PartnerFormList.GroupBy(p => p.Partner_id).Select(g => new Partner((uint)g.Key, g.Max(x => x.Partner_name))));
                StateList = new ObservableRangeCollection<String>(PartnerFormList.Select(x => x.state).Distinct());
                ListOfForm = new ObservableRangeCollection<String>(PartnerFormList.Select(x => x.Form_name).Distinct());
            }
            catch (Exception ex)
            {
                TestLoad = true;

            }



            ActPopup = false;
            //IsBusy = false;
            IsPullToRefreshEnabled = true;

            //FillState();
        }
        public async Task LoadAllPartnerForm()
        {

            PartnerFormList = new ObservableRangeCollection<Partner_Form.Collection>();
            IsPullToRefreshEnabled = false;
            ActPopup = true;
            PartnerFormList.Clear();

            await Task.Delay(500);

            try
            {
                var C = Task.Run(() => Partner_Form.GetAllPartnerForm());
                var c = await C;
                PartnerFormList = new ObservableRangeCollection<Partner_Form.Collection>(c.OrderBy(o => Math.Abs(o.dateDiff.TotalDays)));
                var A = Task.Run(() => Cycle.GetAllCycle());
                CycleList = new ObservableRangeCollection<Cycle>(await A);
                //var D = Task.Run(() => Partner.GetPartnaire());
                PartnerList = new ObservableRangeCollection<Partner>(PartnerFormList.GroupBy(p => p.Partner_id).Select(g => new Partner((uint)g.Key, g.Max(x => x.Partner_name))));
                EmpolyeList = new ObservableRangeCollection<Empolye>(PartnerFormList.GroupBy(p => p.Agent_id).Select(g => new Empolye((int)(g.Key), g.Max(x => x.Agent_name))));
                StateList = new ObservableRangeCollection<String>(PartnerFormList.Select(x => x.state).Distinct());
                ListOfForm = new ObservableRangeCollection<String>(PartnerFormList.Select(x => x.Form_name).Distinct());

            }
            catch (Exception ex)
            {
                TestLoad = true;
            }


            ActPopup = false;
            //IsBusy = false;
            IsPullToRefreshEnabled = true;
            // FillState();
        }
        private void FillState()
        {





            //StateList = new ObservableRangeCollection<Partner_Form.State>
            //{
            //    new Partner_Form.State(){ Id=1,Name="Validated" },
            //    new Partner_Form.State(){ Id=2,Name="Closed" },
            //    new Partner_Form.State(){ Id=3,Name="Waiting" },
            //    new Partner_Form.State(){ Id=4,Name="Opned" }
            //};
        }










    }
}
