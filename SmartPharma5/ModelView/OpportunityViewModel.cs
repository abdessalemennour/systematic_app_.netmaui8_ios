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
using Acr.UserDialogs;
using DevExpress.Data.Svg;
using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;


/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.View;
using SmartPharma5.Model;
Après :
using SmartPharma5.Model;
using SmartPharma5.Services;
*/
using SmartPharma5.Model;
using SmartPharma5.ModelView;
using SmartPharma5.Services;
using SmartPharma5.View;
using System.Collections.ObjectModel;

//using Xamarin.Essentials;
//using Xamarin.Forms;
using Command = MvvmHelpers.Commands.Command;

namespace SmartPharma5.ViewModel
{
    public partial class OpportunityViewModel : BaseViewModel
    {
        /*  public ObservableCollection<Currency> Currencies { get; set; }
          public Currency SelectedCurrency { get; set; }
          /**************************************/
        private int userId = Preferences.Get("iduser", 0);

        private Tuple<uint, string> _selectedCurrency;
        public Tuple<uint, string> SelectedCurrency
        {
            get => _selectedCurrency;
            set
            {
                _selectedCurrency = value;
                OnPropertyChanged();
                OnCurrencyChanged();
            }
        }
        private int _selectedOpportunityId;
        public int SelectedOpportunityId
        {
            get => _selectedOpportunityId;
            set
            {
                _selectedOpportunityId = value;
                OnPropertyChanged(nameof(SelectedOpportunityId));
            }
        }
        private bool _isVisibleSocietyComboBox;
        public bool IsVisibleSocietyComboBox
        {
            get => _isVisibleSocietyComboBox;
            set => SetProperty(ref _isVisibleSocietyComboBox, value);
        }
        public List<Tuple<uint, string>> Currencies { get; set; }

        private void OnCurrencyChanged()
        {
            if (SelectedCurrency != null)
            {
                uint selectedCurrencyId = SelectedCurrency.Item1; // Récupère l'ID de la devise sélectionnée
                Opportunity.Currency = selectedCurrencyId; // Met à jour la propriété Currency dans le modèle
            }
        }


        private Document _temporaryDocument;
        public Document TemporaryDocument
        {
            get => _temporaryDocument;
            set
            {
                _temporaryDocument = value;
                OnPropertyChanged(nameof(TemporaryDocument));
            }
        }
        public ObservableCollection<Document> TemporaryDocuments { get; set; } = new ObservableCollection<Document>();
        // Méthode pour annuler le document temporaire
        public void CancelTemporaryDocument()
        {
            TemporaryDocument = null;
        }
        private ObservableRangeCollection<Society> societyList;
        public ObservableRangeCollection<Society> SocietyList { get => societyList; set => SetProperty(ref societyList, value); }

        private Society societySelectedItem;
        public Society SocietySelectedItem { get => societySelectedItem; set => SetProperty(ref societySelectedItem, value); }

        public AsyncCommand SocietyChangedCommand { get; }
        /************************************/


        public Command EnAttenteCommand { get; }
        public Command BcCommand { get; }
        public Command QuotationCommand { get; }

        public Command GangneCommand { get; }
        public Command PerduCommand { get; }
        public Command Tryagain { get; }

        public AsyncCommand ClosePopupWholesalerCommand { get; }
        public AsyncCommand RefreshCommand { get; }

        public AsyncCommand<OpportunityLine> RemoveCommand { get; }
        public AsyncCommand<decimal> DiscountChangeCommand { get; }
        public AsyncCommand CancelMoreDetailCommand { get; set; }
        public AsyncCommand GetQuotation { get; }


        public AsyncCommand WholeSalerRemoveCommand { get; set; }
        public AsyncCommand<Partner> WholesalerTapCommand { get; set; }
        public AsyncCommand AddCommand { get; set; }
        public AsyncCommand EditCommand { get; set; }
        public AsyncCommand ExitCommand { get; set; }
        public AsyncCommand LogoutCommand { get; set; }
        public AsyncCommand ChangeClientCommand { get; set; }
        public AsyncCommand GoBackCommand { get; set; }
        public AsyncCommand<object> QuantityChangeCommand { get; set; }
        public AsyncCommand SaveMoreDetailCommand { get; }
        public AsyncCommand MoreDetailCommand { get; }
        public Command ValidateWithWholeSalerCommand { get; }
        public AsyncCommand CancelCommand { get; }
        public AsyncCommand GratuiteCommand { get; }
        public AsyncCommand ValidateCommand { get; }
        public AsyncCommand GetForms { get; }
        private bool successpopup = false;
        public bool SuccessPopup { get => successpopup; set => SetProperty(ref successpopup, value); }
        private bool fieldpopup = false;
        private string successpopupmessage;
        public string SuccessPopupMessage { get => successpopupmessage; set => SetProperty(ref successpopupmessage, value); }

        private string message;
        public string Message { get => message; set => SetProperty(ref message, value); }

        public bool FieldPopup { get => fieldpopup; set => SetProperty(ref fieldpopup, value); }
        public bool savingpopup = false;
        public bool SavingPopup { get => savingpopup; set => SetProperty(ref savingpopup, value); }

        private bool toinvoiceedit = true;
        public bool ToinvoiceEdit { get => toinvoiceedit; set => SetProperty(ref toinvoiceedit, value); }
        public bool moredetailpopup = false;
        public bool MoreDetailPopup { get => moredetailpopup; set => SetProperty(ref moredetailpopup, value); }
        public bool wholesalerpopup = false;
        public bool WholesalerPopup { get => wholesalerpopup; set => SetProperty(ref wholesalerpopup, value); }
        public bool actpopup = false;
        public bool ActPopup { get => actpopup; set => SetProperty(ref actpopup, value); }
        private bool wholeSalerremoveisvisible;
        public bool WholeSalerRemoveIsvisible { get => wholeSalerremoveisvisible; set => SetProperty(ref wholeSalerremoveisvisible, value); }
        private bool wholesalertitlevisible = false;
        public bool WholesalerTitleVisible { get => wholesalertitlevisible; set => SetProperty(ref wholesalertitlevisible, value); }
        private bool changeclientactive = true;
        public bool ChangeClientActive { get => changeclientactive; set => SetProperty(ref changeclientactive, value); }
        private bool discountedit = false;
        public bool DiscountEdit { get => discountedit; set => SetProperty(ref discountedit, value); }
        private bool quantityedit = false;
        public bool QuantityEdit { get => quantityedit; set => SetProperty(ref quantityedit, value); }
        private bool removevisible = true;

        public bool IsNotLoaded { get => isNotLoaded; set => SetProperty(ref isNotLoaded, value); }
        private bool isNotLoaded;


        public bool RemoveVisible { get => removevisible; set => SetProperty(ref removevisible, value); }
        private bool addactive = false;
        public bool AddActive { get => addactive; set => SetProperty(ref addactive, value); }
        private bool aditactive = false;
        public bool EditActive { get => aditactive; set => SetProperty(ref aditactive, value); }
        private bool moredetailactive = true;
        public bool MoreDetailActive { get => moredetailactive; set => SetProperty(ref moredetailactive, value); }
        private bool wholesaleractive = true;
        public bool WholesalerActive { get => wholesaleractive; set => SetProperty(ref wholesaleractive, value); }
        private bool gratuiteactive = false;
        public bool GratuiteActive { get => gratuiteactive; set => SetProperty(ref gratuiteactive, value); }
        private bool validateactive = true;
        public bool ValidateActive { get => validateactive; set => SetProperty(ref validateactive, value); }
        private decimal discountvalue;
        public decimal DiscountValue { get => discountvalue; set => SetProperty(ref discountvalue, value); }
        private bool successsavingpopup;
        public bool SuccessSavingPopup { get => successsavingpopup; set => SetProperty(ref successsavingpopup, value); }
        private bool buttonstateisvisible = true;
        public bool ButtonStateIsVisible { get => buttonstateisvisible; set => SetProperty(ref buttonstateisvisible, value); }
        private bool bcisenabled = true;
        public bool BcIsEnabled { get => bcisenabled; set => SetProperty(ref bcisenabled, value); }
        private bool gangneisenabled = true;
        public bool GangneIsEnabled { get => gangneisenabled; set => SetProperty(ref gangneisenabled, value); }
        private bool perduisenabled = true;
        public bool PerduIsEnabled { get => perduisenabled; set => SetProperty(ref perduisenabled, value); }
        private bool enattenteisenabled = true;
        public bool EnAttenteIsEnabled { get => enattenteisenabled; set => SetProperty(ref enattenteisenabled, value); }
        public NotificationViewModel NotificationVM => App.NotificationVM;


        //public uint Currency { get; set; }
        private bool testCon = true;
        public bool TestCon { get => testCon; set => SetProperty(ref testCon, value); }
        public string Note { get; set; } = string.Empty;
        public bool IsCorrection;

        private ObservableRangeCollection<Partner> wholesalerlist;
        public ObservableRangeCollection<Partner> WholesalerList { get => wholesalerlist; set => SetProperty(ref wholesalerlist, value); }
        public Opportunity oppo;
        public Opportunity Opportunity { get => oppo; set => SetProperty(ref oppo, value); }
        public ObservableRangeCollection<Product> ProductList { get; set; }
        /*****************/
        private bool _isSaveDocumentButtonVisible;
        public bool IsSaveDocumentButtonVisible
        {
            get => _isSaveDocumentButtonVisible;
            set => SetProperty(ref _isSaveDocumentButtonVisible, value);
        }

        private bool _isMainButtonVisible;
        public bool IsMainButtonVisible
        {
            get => _isMainButtonVisible;
            set => SetProperty(ref _isMainButtonVisible, value);
        }
        private bool _isFormsButtonVisible;
        public bool IsFormsButtonVisible
        {
            get => _isFormsButtonVisible;
            set => SetProperty(ref _isFormsButtonVisible, value);
        }

        /*****************/
        /*public void LoadCurrencies()
        {
            Currencies = Partner.AllCurrencies.Select(c => c.Name).ToList();
        }*/

        public OpportunityViewModel(Opportunity opportunity)
        {
            // Initialisation des commandes
            EnAttenteCommand = new Command(EnAttente);
            BcCommand = new Command(Bc);
            QuotationCommand = new Command(quotationFun);
            GangneCommand = new Command(Gangne);
            PerduCommand = new Command(Perdu);
            CancelMoreDetailCommand = new AsyncCommand(CancelMoreDetail);
            AddCommand = new AsyncCommand(AddItems);
            WholeSalerRemoveCommand = new AsyncCommand(WholeSalerRemove);
            WholesalerTapCommand = new AsyncCommand<Partner>(WholesalerTap);
            GoBackCommand = new AsyncCommand(back);
            EditCommand = new AsyncCommand(Edit);
            ExitCommand = new AsyncCommand(Exit);
            LogoutCommand = new AsyncCommand(Logout);
            SaveMoreDetailCommand = new AsyncCommand(SaveMoreDetail);
            MoreDetailCommand = new AsyncCommand(MoreDetailAsync);
            DiscountChangeCommand = new AsyncCommand<decimal>(DiscountChange);
            QuantityChangeCommand = new AsyncCommand<object>(QuantityChange);
            RemoveCommand = new AsyncCommand<OpportunityLine>(Remove);
            ValidateCommand = new AsyncCommand(Validate);
            GratuiteCommand = new AsyncCommand(Gratuite);
            ValidateWithWholeSalerCommand = new Command(ValidateWithWholeSaler);
            CancelCommand = new AsyncCommand(Cancel);
            GetForms = new AsyncCommand(getForms);
            ClosePopupWholesalerCommand = new AsyncCommand(ClosePopupWholesaler);
            Tryagain = new Command(() => FieldPopup = false);
            Opportunity = opportunity;
            /*******************************/
            // Simule une liste de devises (id, nom)
            Currencies = new List<Tuple<uint, string>>
                {
                    new Tuple<uint, string>(1, "TND"),
                    new Tuple<uint, string>(2, "USD"),
                    new Tuple<uint, string>(3, "EURO"),
                };

            // Sélectionne la devise correspondant à Opportunity.Currency
            SelectedCurrency = Currencies.FirstOrDefault(c => c.Item1 == Opportunity.Currency);

            /*******************************/
            RefreshCommand = new AsyncCommand(Refresh);
            GetQuotation = new AsyncCommand(GetQuotationFun);
            Title = "Opportunity";

            if (Opportunity.Id == 0)
            {
                // Mode ajout (nouvelle opportunité)
                IsSaveDocumentButtonVisible = false;
                IsMainButtonVisible = false;
                IsFormsButtonVisible = false;
            }
            else
            {
                // Mode affichage (opportunité existante)
                IsSaveDocumentButtonVisible = true;
                IsMainButtonVisible = true;
                IsFormsButtonVisible = true;
            }

            Task.Run(async () => await LoadProductAndWholesaler());

            // Contrôler la visibilité du titre du grossiste
            if (Opportunity.Dealer != 0)
            {
                WholesalerTitleVisible = true;
            }
            else
            {
                WholesalerTitleVisible = false;
            }

            ValidatedControl(Opportunity.validated);
            ActPopup = false;


            // Initialize Society
            SocietyList = new ObservableRangeCollection<Society>();
            SocietyChangedCommand = new AsyncCommand(SocietyChanged);
            Task.Run(() => LoadSocieties());
        }



        public async Task GetQuotationFun()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new SaleQuotationView(Opportunity)));
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }

        }
        public async Task getForms()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new PartnerFormView(Opportunity.Id, Opportunity.IdPartner)));
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }

        }

        private async Task Refresh()
        {
            await Task.Delay(200);
            ActPopup = false;

        }
        static async Task<bool> checkPermissionState(int id_user)
        {
            bool permissoion = false;
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "SELECT max(opportunity_state) as opportunity_state \r\nFROM atooerp_app_permission_temp inner join\r\natooerp_user_module_group usg on usg.group = atooerp_app_permission_temp.group\r\nwhere user =" + id_user + ";";

                try
                {

                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader["opportunity_state"]) == 1)
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
        private async void UserCheckModule()
        {
            int CrmGroupe = 0;
            int iduser = Preferences.Get("iduser", 0);
            var UMG = await User_Module_Groupe_Services.GetGroupeCRM(iduser);

            if (UMG != null)
            {
                CrmGroupe = UMG.IdGroup;
            }


            if (await checkPermissionState(iduser))
            {


                ButtonStateIsVisible = true;
                StateButtonEnable();

            }
            else
            {
                ButtonStateIsVisible = false;
                StateButtonEnable();

            }

            ActPopup = false;

        }
        private void StateButtonEnable()
        {
            switch (Opportunity.StateLines.LastOrDefault().state)
            {
                case 1:
                    EnAttenteIsEnabled = false;
                    break;
                case 2:
                    GangneIsEnabled = false;
                    IsNotLoaded = false;
                    break;
                case 3:
                    PerduIsEnabled = false;
                    IsNotLoaded = false;
                    break;
                default:
                    EnAttenteIsEnabled = BcIsEnabled = GangneIsEnabled = PerduIsEnabled = false;
                    break;
            }
            if (Opportunity.Dealer != 0)
                BcIsEnabled = false;
        }
        private void StateButtonControl()
        {
            if (!
                Opportunity.validated)
            {
                ButtonStateIsVisible = false;
            }
            else
            {
                UserCheckModule();

            }
        }
        private async void EnAttente(object obj)
        {
            SavingPopup = true;
            await Task.Delay(1000);
            if (DbConnection.Connecter())
            {
                Opportunity.ModifyState(1);
                savingpopup = false;
                SuccessPopupMessage = "Opportunity State has been changed to 'En Attente' ";
                SuccessPopup = true;

                await Task.Delay(1000);
                SuccessPopup = false;
                await App.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                SavingPopup = false;
                await Task.Delay(1000);
                Message = "Connection Failed";
                FieldPopup = true;
            }

        }

        private async void Perdu()
        {
            SavingPopup = true;
            await Task.Delay(1000);
            if (DbConnection.Connecter())
            {
                Opportunity.ModifyState(3);
                SavingPopup = false;
                SuccessPopupMessage = "Opportunity State has been changed to 'Perdue' ";
                SuccessPopup = true;
                await Task.Delay(1000);
                await App.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                SavingPopup = false;
                await Task.Delay(1000);
                Message = "Connection Failed";
                FieldPopup = true;
            }

        }

        private async void Gangne()
        {
            SavingPopup = true;
            await Task.Delay(1000);
            if (DbConnection.Connecter())
            {
                Opportunity.ModifyState(2);
                savingpopup = false;
                SuccessPopupMessage = "Opportunity State has been changed to 'Gagnée' ";
                SuccessPopup = true;
                await Task.Delay(1000);

                await App.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                SavingPopup = false;
                await Task.Delay(1000);
                Message = "Connection Failed";
                FieldPopup = true;
            }

        }
        private async void quotationFun()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to generate New Quotation for this Opportunity!", "Yes", "No");
            if (r)
            {
                SavingPopup = true;
                await Task.Delay(1000);
                if (DbConnection.Connecter())
                {
                    Opportunity.TransferToQuotation();
                    SavingPopup = false;
                    SuccessPopupMessage = "'Quotation' has been successfully generated ";
                    SuccessPopup = true;
                    await Task.Delay(1000);
                    SuccessPopup = false;
                    await App.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    SavingPopup = false;
                    await Task.Delay(1000);
                    Message = "Connection Failed";
                    FieldPopup = true;
                }
            }
        }
        private async void Bc()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to generate BC for this Opportunity!", "Yes", "No");
            if (r)
            {
                SavingPopup = true;
                await Task.Delay(1000);
                if (DbConnection.Connecter())
                {
                    Opportunity.TransferToBc();
                    SavingPopup = false;
                    SuccessPopupMessage = "'Bon Command' has been successfully generated ";
                    SuccessPopup = true;
                    await Task.Delay(1000);
                    SuccessPopup = false;
                    await App.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    SavingPopup = false;
                    await Task.Delay(1000);
                    Message = "Connection Failed";
                    FieldPopup = true;
                }
            }
        }
        private Task ClosePopupWholesaler()
        {
            WholesalerPopup = false;
            return Task.CompletedTask;
        }

        public OpportunityViewModel(ObservableRangeCollection<Partner> wholesalerList)
        {
            WholesalerList = wholesalerList;
        }
        private Task WholesalerTap(Partner arg)
        {
            WholesalerPopup = false;

            Opportunity.Dealer = (int)arg.Id;
            Opportunity.dealerName = (string)arg.Name;

            WholeSalerRemoveIsvisible = true;
            WholesalerTitleVisible = true;
            return Task.CompletedTask;

        }
        private async Task<bool> CheckInternetConnection()
        {
            try
            {
                var current = Connectivity.NetworkAccess;
                return current == NetworkAccess.Internet;
            }
            catch
            {
                return false;
            }
        }
        private async Task LoadProductAndWholesaler()
        {
            try
            {
                // Display the loading dialog
                IsNotLoaded = true;
                ActPopup = true;
                AddActive = false; // Désactive le bouton au début du chargement

                // Vérifie si la connexion internet est disponible avant de continuer
                bool isConnected = await CheckInternetConnection(); // Vous devrez implémenter cette méthode

                if (!isConnected)
                {
                    // Si pas de connexion, afficher un message et sortir
                    await UserDialogs.Instance.AlertAsync("Aucune connexion internet disponible", "Erreur", "OK");
                    return;
                }

                var C = Task.Run(() => Product.GetProduct(Opportunity.IdPartner));
                var P = Task.Run(() => Partner.GetWholesalerList());

                WholesalerList = new ObservableRangeCollection<Partner>(await P);
                ProductList = new ObservableRangeCollection<Product>(await C);

                // Si l'opportunité est nouvelle (Id = 0), activer le bouton
                if (Opportunity.Id == 0)
                {
                    AddActive = true;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, show a message, etc.)
                Console.WriteLine(ex.Message);
                await UserDialogs.Instance.AlertAsync("Échec de la connexion: ", "Erreur", "OK");
                // var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to exit!", "Yes", "No");

                // Important: on vérifie quand même si on doit activer le bouton
                //if (Opportunity.Id == 0)
                //{
                //    AddActive = true;
                //    IsNotLoaded = false;

                //}
                AddActive = false;

            }
            finally
            {
                // Dismiss the loading dialog
                ActPopup = false;
                IsNotLoaded = false;
            }
        }
        //private async Task LoadProductAndWholesaler()
        //{
        //    try
        //    {
        //        // Display the loading dialog

        //        IsNotLoaded = true;
        //        ActPopup = true;
        //        AddActive = false;


        //        var C = Task.Run(() => Product.GetProduct(Opportunity.IdPartner));
        //        var P = Task.Run(() => Partner.GetWholesalerList());

        //        WholesalerList = new ObservableRangeCollection<Partner>(await P);
        //        ProductList = new ObservableRangeCollection<Product>(await C);

        //        if (Opportunity.Id == 0)
        //        {
        //            AddActive = true;
        //            IsNotLoaded = false;
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle the exception (log it, show a message, etc.)
        //        Console.WriteLine(ex.Message);
        //    }
        //    finally
        //    {
        //        // Dismiss the loading dialog

        //        ActPopup = false;
        //        IsNotLoaded = false;
        //    }
        //}


        private async Task Cancel()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to exit!", "Yes", "No");
            if (r)
            {
                await App.Current.MainPage.Navigation.PopAsync();
                //await  Shell.Current.GoToAsync("..");
            }
        }
        private void ValidateWithWholeSaler()
        {
            WholesalerPopup = true;
            //return Task.CompletedTask;
        }
        private async Task Gratuite()
        {

            Opportunity = new Opportunity(0, Opportunity.IdPartner, Opportunity.partnerName, Opportunity.IdPayment_method, Opportunity.IdPayment_condition, Opportunity.IdAgent, "", Opportunity.Currency, false, Opportunity.Id, 0, Opportunity.Opportunity_lines);
            Opportunity = Opportunity.opportunityLineGratuite(Opportunity);
            ActPopup = true;
            await Task.Delay(1000);
            //DiscountValue = Convert.ToDecimal(100);
            WholesalerTitleVisible = false;
            ActPopup = false;
            Checkbutton();
            //await App.Current.MainPage.Navigation.PushAsync(new ProductListView(Opportunity, ProductList));
        }
        private async Task Validate()
        {
            try
            {
                SavingPopup = true;
                await Task.Delay(1000);

                if (Opportunity.Opportunity_lines.Count != 0)
                {
                    var Connectivity = DbConnection.CheckConnectivity();
                    if (Connectivity)
                    {
                        if (await DbConnection.Connecter3())
                        {
                            try
                            {
                                if (Opportunity.Validate() == null)
                                {
                                    TestCon = true;
                                    return;
                                }

                                // Enregistrer l'opportunité et récupérer l'Id généré
                                //int opportunityId = Opportunity.insert();

                                if (Opportunity.Id == -1)
                                {
                                    await App.Current.MainPage.DisplayAlert("Error", "Failed to t opportunity.", "OK");
                                    return;
                                }

                                // Enregistrer tous les documents temporaires
                                foreach (var document in TemporaryDocuments)
                                {
                                    bool isSaved = await Document.SaveToDatabase(document, Opportunity.Id);
                                    if (!isSaved)
                                    {
                                        await App.Current.MainPage.DisplayAlert("Error", "Failed to save the document.", "OK");
                                        return;
                                    }
                                }

                                SavingPopup = false;
                                SuccessPopupMessage = "Your opportunity has been successfully sent!";
                                SuccessPopup = true;
                                await Task.Delay(1000);
                                SuccessPopup = false;

                                if (Opportunity.Dealer == 0)
                                {
                                    await App.Current.MainPage.DisplayAlert("Done", "Opportunity Created Successfully", "OK");
                                    await App.Current.MainPage.Navigation.PopAsync();
                                }
                                else
                                {
                                    var r = await App.Current.MainPage.DisplayAlert("MESSAGE", "Do you want to add GRATUITE!", "Yes", "No");
                                    if (r)
                                    {
                                        await Gratuite();
                                        ValidatedControl(Opportunity.validated);
                                        Title = "Gratuité";
                                    }
                                    else
                                    {
                                        await App.Current.MainPage.DisplayAlert("Done", "Opportunity Created Successfully", "OK");
                                        await App.Current.MainPage.Navigation.PopAsync();
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                TestCon = true;
                            }
                        }
                        else
                        {
                            SavingPopup = false;
                            Message = "There is a problem in connecting to the server. \n Please contact our support for further assistance.";
                            TestCon = true;
                            FieldPopup = true;
                        }
                    }
                    else
                    {
                        Message = "No network connectivity.";
                        TestCon = true;
                        SavingPopup = false;
                        await Task.Delay(1000);
                        FieldPopup = true;
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Add products before validate", "OK");
                    SavingPopup = false;
                }
            }
            catch (Exception ex)
            {
                // Gérer les exceptions
            }
        }
        public void ValidatedControl(bool validated)
        {
            MoreDetailActive = true;

            AddActive = ToinvoiceEdit = WholeSalerRemoveIsvisible = RemoveVisible = ValidateActive = GratuiteActive = EditActive = WholesalerActive = !validated;
            QuantityEdit = DiscountEdit = validated;
            if (Opportunity.parent != 0)
                GratuiteActive = WholesalerActive = false;

            StateButtonControl();
            ControleState();
        }
        void ControleState()
        {
            if (Opportunity.Id > 0)
            {
                if (Opportunity.StateLines.Last().state > 1)
                {
                    EditActive = GratuiteActive = false;
                }
                else
                    EditActive = GratuiteActive = true;
            }
            else
                EditActive = GratuiteActive = false;

        }

        private async Task Remove(OpportunityLine arg)
        {
            Opportunity.Opportunity_lines.Remove(arg);
            Opportunity.totalAmount = Opportunity.getTotalAmount();

        }
        void Checkbutton()
        {
            if (!ValidateActive)
            {
                ValidatedControl(false);
            }
            else
            {
                ValidatedControl(true);
            }
        }


        private Task QuantityChange(object arg)
        {

            var opp_line = (OpportunityLine)arg;
            opp_line.PTARTTC = opp_line.PUARTTC * opp_line.quantity;
            Opportunity.totalAmount = Opportunity.getTotalAmount();
            return Task.CompletedTask;

        }

        private Task DiscountChange(decimal arg)
        {
            foreach (OpportunityLine o in Opportunity.Opportunity_lines)
            {
                o.discount = o.discount + arg / 100;
                if (o.discount > 1)
                    o.discount = 1;
                if (o.discount < 0)
                    o.discount = 0;
                o.PTARTTC = o.PUARTTC * o.quantity;
            }
            Opportunity.totalAmount = Opportunity.getTotalAmount();
            return Task.CompletedTask;
        }

        private Task MoreDetailAsync()
        {
            MoreDetailPopup = true;
            return Task.CompletedTask;
        }

        private Task SaveMoreDetail()
        {
            MoreDetailPopup = false;
            return Task.CompletedTask;
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
                Opportunity = new Opportunity();

            }
        }

        private Task Edit()
        {
            Checkbutton();
            GratuiteActive = false;
            return Task.CompletedTask;
        }

        private Task WholeSalerRemove()
        {
            WholesalerTitleVisible = false;
            WholeSalerRemoveIsvisible = false;
            Opportunity.Dealer = 0;
            Opportunity.dealerName = "";
            return Task.CompletedTask;
        }

        private async Task AddItems()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(1000);
                //await Shell.Current.GoToAsync("..");
                await App.Current.MainPage.Navigation.PushAsync(new ProductListView(Opportunity, ProductList));
                UserDialogs.Instance.HideLoading();

            }
            catch (Exception ex)
            {

            }

        }

        private Task CancelMoreDetail()
        {
            MoreDetailPopup = false;
            return Task.CompletedTask;
        }
        private async Task back()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to exit this opportunity!", "Yes", "No");
            if (r)
            {

                await App.Current.MainPage.Navigation.PopAsync();
            }
        }
        private async Task SocietyChanged()
        {
            if (SocietySelectedItem != null)
            {
                // Met à jour l'ID de la société dans l'opportunité
                Opportunity.Socity = SocietySelectedItem.Id;
                Console.WriteLine($"Selected society ID: {Opportunity.Socity}");
            }
        }
        //private async Task LoadSocieties()
        //{
        //    // 1. Vider le cache avant de recharger
        //    await Society.ClearSocietiesCache();

        //    // 2. Charger les sociétés pour le nouvel utilisateur
        //    var societies = await Society.GetAllSocietiesAsync();

        //    // 3. Mettre à jour la liste
        //    SocietyList.Clear();
        //    foreach (var society in societies)
        //    {
        //        SocietyList.Add(society);
        //    }

        //    // 4. Sélectionner la première société comme valeur par défaut
        //    if (SocietyList.Count > 0)
        //    {
        //        SocietySelectedItem = SocietyList[0];
        //    }

        //    // 5. Notifier les changements
        //    OnPropertyChanged(nameof(SocietyList));
        //    OnPropertyChanged(nameof(SocietySelectedItem));
        //}
        private async Task LoadSocieties()
        {
            try
            {
                // 1. Vider le cache avant de recharger
                await Society.ClearSocietiesCache();

                // 2. Charger les sociétés
                var societies = await Society.GetAllSocietiesAsync();

                // 3. Mettre à jour la liste
                SocietyList.ReplaceRange(societies);
                IsVisibleSocietyComboBox = SocietyList.Count > 1;

                // 4. Conserver la sélection existante ou sélectionner la société correspondant à Opportunity.Socity
                if (SocietyList.Count > 0)
                {
                    if (Opportunity.Socity.HasValue)
                    {
                        // Recherche de la société correspondant à l'ID stocké dans Opportunity
                        var matchingSociety = SocietyList.FirstOrDefault(s => s.Id == Opportunity.Socity.Value);
                        SocietySelectedItem = matchingSociety ?? SocietyList[0];
                    }
                    else
                    {
                        // Sélection par défaut seulement si aucune société n'est déjà sélectionnée
                        SocietySelectedItem = SocietyList[0];
                    }
                }

                // 5. Notifier les changements
                OnPropertyChanged(nameof(SocietyList));
                OnPropertyChanged(nameof(SocietySelectedItem));
                OnPropertyChanged(nameof(IsVisibleSocietyComboBox));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading societies: {ex.Message}");
            }
        }
    }
    }
