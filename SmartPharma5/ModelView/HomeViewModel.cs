
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.Model;
using SmartPharma5.Services;
using MvvmHelpers;
Après :
using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;
*/
using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Linq;
using Plugin.Connectivity;
using MySqlConnector;
using System.ComponentModel;
using System.Collections.Generic;
Après :
using SmartPharma5.Model;
using SmartPharma5.Services;
using SmartPharma5.View;
using System;
using System.Collections.Generic;
using System.Connectivity;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
*/
using MySqlConnector;
using Plugin.Connectivity;
using SmartPharma5.Model;
using SmartPharma5.Models;
using SmartPharma5.Services;
using SmartPharma5.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Command = MvvmHelpers.Commands.Command;
//using SmartPharma5.View;
//using SmartPharma5.View;

namespace SmartPharma5.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        #region New_attribute_permmission
        #region Modules
        private bool module_achat = true;
        public bool Module_achat { get => module_achat; set => SetProperty(ref module_achat, value); }


        private bool module_dashboard = true;
        public bool Module_dashboard { get => module_dashboard; set => SetProperty(ref module_dashboard, value); }


        private bool module_comptabilité = true;
        public bool Module_comptabilité { get => module_comptabilité; set => SetProperty(ref module_comptabilité, value); }


        private bool module_hr = true;
        public bool Module_hr { get => module_hr; set => SetProperty(ref module_hr, value); }


        private bool module_marketing = true;
        public bool Module_marketing { get => module_marketing; set => SetProperty(ref module_marketing, value); }








        #endregion
        #region BtnVisbilite


        //-----------------------------Btns Opportunity----------------------------
        private bool btn_add_opp = true;
        public bool Btn_add_opp { get => btn_add_opp; set => SetProperty(ref btn_add_opp, value); }


        private bool btn_my_opp = true;
        public bool Btn_my_opp { get => btn_my_opp; set => SetProperty(ref btn_my_opp, value); }



        private bool btn_all_opp = true;
        public bool Btn_all_opp { get => btn_all_opp; set => SetProperty(ref btn_all_opp, value); }


        //-----------------------------Btns Dashboard----------------------------
        private bool btn_dashboard = true;
        public bool Btn_dashboard { get => btn_dashboard; set => SetProperty(ref btn_dashboard, value); }

        private bool btn_unpaid = true;
        public bool Btn_unpaid { get => btn_unpaid; set => SetProperty(ref btn_unpaid, value); }


        private bool btn_order = true;
        public bool Btn_order { get => btn_order; set => SetProperty(ref btn_order, value); }

        //-----------------------------Btns Payment----------------------------
        private bool btn_add_payment = true;
        public bool Btn_add_payment { get => btn_add_payment; set => SetProperty(ref btn_add_payment, value); }


        private bool btn_my_payment = true;
        public bool Btn_my_payment { get => btn_order; set => SetProperty(ref btn_my_payment, value); }


        private bool btn_all_payment = true;
        public bool Btn_all_payment { get => btn_all_payment; set => SetProperty(ref btn_all_payment, value); }


        private bool btn_add_transfer = true;
        public bool Btn_add_transfer { get => btn_add_transfer; set => SetProperty(ref btn_add_transfer, value); }


        private bool btn_my_cashdesk = true;
        public bool Btn_my_cashdesk { get => btn_my_cashdesk; set => SetProperty(ref btn_my_cashdesk, value); }


        private bool btn_all_cashdesk = true;
        public bool Btn_all_cashdesk { get => btn_all_cashdesk; set => SetProperty(ref btn_all_cashdesk, value); }


        private bool btn_my_waiting_tranfer = true;
        public bool Btn_my_waiting_tranfer { get => btn_my_waiting_tranfer; set => SetProperty(ref btn_my_waiting_tranfer, value); }


        private bool btn_my_transfer = true;
        public bool Btn_my_transfer { get => btn_my_transfer; set => SetProperty(ref btn_my_transfer, value); }


        private bool btn_all_transfer = true;
        public bool Btn_all_transfer { get => btn_all_transfer; set => SetProperty(ref btn_all_transfer, value); }

        //-----------------------------Btns Quiz----------------------------

        private bool btn_add_partner_form = true;
        public bool Btn_add_partner_form { get => btn_add_partner_form; set => SetProperty(ref btn_add_partner_form, value); }


        private bool btn_my_partner_form = true;
        public bool Btn_my_partner_form { get => btn_my_partner_form; set => SetProperty(ref btn_my_partner_form, value); }


        private bool btn_all_partner_form = true;
        public bool Btn_all_partner_form { get => btn_all_partner_form; set => SetProperty(ref btn_all_partner_form, value); }


        //-----------------------------Btns Hr----------------------------

        private bool btn_add_request_day_off = true;
        public bool Btn_add_request_day_off { get => btn_add_request_day_off; set => SetProperty(ref btn_add_request_day_off, value); }


        private bool btn_my_request_day_off = true;
        public bool Btn_my_request_day_off { get => btn_my_request_day_off; set => SetProperty(ref btn_my_request_day_off, value); }


        private bool btn_all_request_day_off = true;
        public bool Btn_all_request_day_off { get => btn_all_request_day_off; set => SetProperty(ref btn_all_request_day_off, value); }



        private bool btn_add_request_deposit = true;
        public bool Btn_add_request_deposit { get => btn_add_request_deposit; set => SetProperty(ref btn_add_request_deposit, value); }


        private bool btn_my_request_deposit = true;
        public bool Btn_my_request_deposit { get => btn_my_request_deposit; set => SetProperty(ref btn_my_request_deposit, value); }


        private bool btn_all_request_deposit = true;
        public bool Btn_all_request_deposit { get => btn_all_request_deposit; set => SetProperty(ref btn_all_request_deposit, value); }






        //-----------------------------Btns Profile----------------------------


        private bool btn_all_partner_update = true;
        public bool Btn_all_partner_update { get => btn_all_partner_update; set => SetProperty(ref btn_all_partner_update, value); }


        private bool btn_add_partner = true;
        public bool Btn_add_partner { get => btn_add_partner; set => SetProperty(ref btn_add_partner, value); }


        private bool btn_profile_validation = true;
        public bool Btn_profile_validation { get => btn_profile_validation; set => SetProperty(ref btn_profile_validation, value); }




        #endregion


        #endregion
        public Command ProfilePartner { get; }

        public AsyncCommand ChnageProfilePage { get; }
        /***********************************/
        public ObservableCollection<ServerSettings> SavedConnections { get; } = new ObservableCollection<ServerSettings>();
        public ICommand LoadConnectionsCommand { get; }
        public ICommand SelectConnectionCommand { get; }
        private ServerSettings _currentConnection;
        public ServerSettings CurrentConnection
        {
            get => _currentConnection;
            set => SetProperty(ref _currentConnection, value);
        }
        public User User { get; set; } = new User();
        public ICommand ManageConnectionsCommand { get; }
        public AsyncCommand AddCommand { get; set; }

        public ICommand DeleteConnectionCommand { get; }
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        /***********************************/
        public AsyncCommand Validation { get; }
        public AsyncCommand MyRequests { get; }

        public AsyncCommand UpdatePartner { get; }

        public AsyncCommand ListePartner { get; }

        public AsyncCommand AddPartnerTemp { get; }

        public AsyncCommand InsertCongé { get; }
        public AsyncCommand InsertAvance { get; }
        public AsyncCommand CommandMyWaitingTransfer { get; }
        public AsyncCommand CommandAddTransfer { get; }
        public AsyncCommand CommandMyForms { get; }
        public AsyncCommand CommandAddPartnerForms { get; }
        public AsyncCommand CommandAllForms { get; }
        public AsyncCommand CommandAllTransfer { get; }
        public AsyncCommand CommandMyTransfer { get; }
        public AsyncCommand CommandMyCashDesk { get; }
        public AsyncCommand CommandAllCashDesk { get; }
        public AsyncCommand CommandAddPayment { get; }
        public AsyncCommand CommandMyPayment { get; }
        public AsyncCommand CommandAllPayment { get; }
        public AsyncCommand CommandOrder { get; }
        public AsyncCommand CommandDashboard { get; }
        public AsyncCommand CommandStatment { get; }
        public AsyncCommand CommandAddOpp { get; }
        public AsyncCommand CommandMyOpp { get; }
        public AsyncCommand CommandAllOpp { get; }
        public AsyncCommand Contacts { get; }

        public AsyncCommand CommandTest { get; }

        public AsyncCommand AllPartnerCommand { get; }
        public AsyncCommand CommandNotificationTempValue { get; }
        public Command ExitCommand { get; set; }
        public AsyncCommand LogoutCommand { get; set; }

        private bool menuVisisble;
        public bool MenuVisisble { get => menuVisisble; set => SetProperty(ref menuVisisble, value); }

        private bool openPoopUp = true;
        public bool OpenPoopUp { get => openPoopUp; set => SetProperty(ref openPoopUp, value); }


        private bool allformsisvisible = true;
        public bool AllFormsIsVisible { get => allformsisvisible; set => SetProperty(ref allformsisvisible, value); }
        private bool myformsisvisible = true;
        public bool MyFormsIsVisible { get => myformsisvisible; set => SetProperty(ref myformsisvisible, value); }
        private bool addpartnerformsisvisible = true;
        public bool AddPartnerFormsIsVisible { get => addpartnerformsisvisible; set => SetProperty(ref addpartnerformsisvisible, value); }


        private bool waitingtransferisvisible = true;
        public bool MyWaitingTransferIsVisible { get => waitingtransferisvisible; set => SetProperty(ref waitingtransferisvisible, value); }
        private bool addtransferisvisible = true;
        public bool AddTransferIsVisible { get => addtransferisvisible; set => SetProperty(ref addtransferisvisible, value); }
        private bool mytransferisvisible = true;
        public bool MyTransferIsVisible { get => mytransferisvisible; set => SetProperty(ref mytransferisvisible, value); }
        private bool alltransferisvisible = true;
        public bool AllTransferIsVisible { get => alltransferisvisible; set => SetProperty(ref alltransferisvisible, value); }
        private bool allcashdeskisvisible = true;
        public bool AllCashDeskIsVisible { get => allcashdeskisvisible; set => SetProperty(ref allcashdeskisvisible, value); }
        private bool mycashdeskisvisible = true;
        public bool MyCashDeskIsVisible { get => mycashdeskisvisible; set => SetProperty(ref mycashdeskisvisible, value); }
        private bool addcashdeskisvisible = true;
        public bool AddCashDeskIsVisible { get => addcashdeskisvisible; set => SetProperty(ref addcashdeskisvisible, value); }
        private bool orderisvisible = true;
        public bool OrderIsVisible { get => orderisvisible; set => SetProperty(ref orderisvisible, value); }
        private bool dashboardisvisible = true;
        public bool DashboardIsVisible { get => dashboardisvisible; set => SetProperty(ref dashboardisvisible, value); }
        private bool statmentisvisible = true;
        public bool StatmentIsVisible { get => statmentisvisible; set => SetProperty(ref statmentisvisible, value); }
        private bool alloppisvisible = true;
        public bool AllOppIsVisible { get => alloppisvisible; set => SetProperty(ref alloppisvisible, value); }
        private bool myoppisvisible = true;
        public bool MyOppIsVisible { get => myoppisvisible; set => SetProperty(ref myoppisvisible, value); }
        private bool addoppisvisible = true;
        public bool AddOppIsVisible { get => addoppisvisible; set => SetProperty(ref addoppisvisible, value); }

        private bool addpaymentisvisible = true;
        public bool AddPaymentIsVisible { get => addpaymentisvisible; set => SetProperty(ref addpaymentisvisible, value); }
        private bool mypaymentisvisible = true;
        public bool MyPaymentIsVisible { get => mypaymentisvisible; set => SetProperty(ref mypaymentisvisible, value); }
        private bool paymentisvisible = true;
        public bool PaymentIsVisible { get => paymentisvisible; set => SetProperty(ref paymentisvisible, value); }
        private bool allpaymentisvisible = true;
        public bool AllPaymentIsVisible { get => allpaymentisvisible; set => SetProperty(ref allpaymentisvisible, value); }

        private bool IsCustomerClicked = true;

        private bool isOpen = false;
        public bool IsOpen { get => isOpen; set => SetProperty(ref isOpen, value); }





        public AsyncCommand CommandTtest { get; }

        public AsyncCommand CommandHolidayRequestAll { get; }

        public AsyncCommand CommandHolidayRequestMy { get; }

        public AsyncCommand CommandAvanceRequestAll { get; }

        public AsyncCommand CommandAvanceRequestMy { get; }

        public HomeViewModel()
        {
            MenuVisisble = false;
            user_contrat.getBtnIInvisibles();
            user_contrat.getModules();





            ProfilePartner = new Command(async () =>
            {


                //UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(1000);

                //await App.Current.MainPage.Navigation.PushAsync(new AllPartnerProfile());
                // UserDialogs.Instance.HideLoading();





            });
            CommandTtest = new AsyncCommand(test);
            AllPartnerCommand = new AsyncCommand(AllPartnerCommandFonction);
            Validation = new AsyncCommand(Validate);
            MyRequests = new AsyncCommand(Requests);
            ChnageProfilePage = new AsyncCommand(changeProfilePage);
            ListePartner = new AsyncCommand(AllPartner);
            AddPartnerTemp = new AsyncCommand(addPartnerTemp);
            CommandTest = new AsyncCommand(TestCommand);
            UpdatePartner = new AsyncCommand(update_partner);
            CommandMyWaitingTransfer = new AsyncCommand(MyWaitingTransfer);
            CommandMyForms = new AsyncCommand(MyForms);
            CommandAddPartnerForms = new AsyncCommand(TestCommand);
            CommandAllForms = new AsyncCommand(AllForms);
            CommandAddTransfer = new AsyncCommand(AddTransfer);
            CommandAllTransfer = new AsyncCommand(Alltransfer);
            CommandMyTransfer = new AsyncCommand(Mytransfer);
            CommandMyCashDesk = new AsyncCommand(MyCashDesk);
            CommandAllCashDesk = new AsyncCommand(AllCashDesk);
            CommandOrder = new AsyncCommand(Order);
            CommandDashboard = new AsyncCommand(Dashboard);
            CommandStatment = new AsyncCommand(Statment);
            CommandAddOpp = new AsyncCommand(AddOpp);
            CommandMyOpp = new AsyncCommand(MyOpp);
            CommandAllOpp = new AsyncCommand(AllOpp);
            CommandAddPayment = new AsyncCommand(AddPayement);
            CommandMyPayment = new AsyncCommand(MyPayment);
            CommandAllPayment = new AsyncCommand(AllPayment);
            InsertCongé = new AsyncCommand(changeInsert);
            InsertAvance = new AsyncCommand(Insert);
            Contacts = new AsyncCommand(contactsFun);

            ExitCommand = new Command(Exit);
            LogoutCommand = new AsyncCommand(Logout);
            CommandHolidayRequestAll = new AsyncCommand(HolidayAll);
            CommandHolidayRequestMy = new AsyncCommand(HolidayMy);
            CommandAvanceRequestMy = new AsyncCommand(avanceMy);
            CommandAvanceRequestAll = new AsyncCommand(avanceAll);
            CommandNotificationTempValue = new AsyncCommand(changeToNotifications);

            UserCheckModule();
            DbConnection.Deconnecter();
            bool test_connection = IsNetworkLikelyAvailable();
            /*******************************/
            LoadConnectionsCommand = new Command(async () => await LoadConnections());
            // SelectConnectionCommand = new Microsoft.Maui.Controls.Command<ServerSettings>(SelectConnection);
            SelectConnectionCommand = new Microsoft.Maui.Controls.Command<ServerSettings>(async (conn) => await SelectConnectionAsync(conn));

            Task.Run(async () => await LoadConnections());
            ManageConnectionsCommand = new Command(OnManageConnections);
            AddCommand = new AsyncCommand(AddConnection);
            DeleteConnectionCommand = new Microsoft.Maui.Controls.Command<ServerSettings>(async (connection) => await DeleteConnectionAsync(connection));

            /**********************/
        }

        /*****************************************/
        public async Task DeleteConnectionAsync(ServerSettings connection)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirmation",
                $"Voulez-vous vraiment supprimer la connexion '{connection.CustomName}' ?",
                "Oui", "Non");

            if (confirm)
            {
                var configService = new JsonConfigService();
                bool success = await configService.RemoveConnectionAsync(connection.ConnectionId);

                if (success)
                {
                    // Supprime de la collection observable (rafraîchit automatiquement l'UI)
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        SavedConnections.Remove(connection);
                    });
                    UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                    Task.Delay(1000).GetAwaiter();
                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
                    UserDialogs.Instance.HideLoading();

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Erreur",
                        "La suppression a échoué",
                        "OK");
                }
            }
        }
        public async Task LoadConnectionsmanage()
        {
            IsBusy = true;

            var configService = new JsonConfigService();

            if (CurrentConnection == null)
            {
                CurrentConnection = await configService.LoadSettingsAsync();
            }

            var history = await configService.LoadHistoryAsync();

            Device.BeginInvokeOnMainThread(() =>
            {
                SavedConnections.Clear();

                if (history?.Connections != null)
                {
                    foreach (var conn in history.Connections
                        .Where(c => CurrentConnection == null || c.ConnectionId != CurrentConnection.ConnectionId)
                        .OrderByDescending(c => c.ConnectionDate))
                    {
                        SavedConnections.Add(conn);
                    }
                }

                IsBusy = false;
            });
        }


        private async Task AddConnection()
        {
            var r = await App.Current.MainPage.DisplayAlert("Avertissement", "Êtes-vous sûr de vouloir changer de connexion", "Oui", "Non");
            if (r)
            {
                Preferences.Set("idagent", Convert.ToUInt32(null));
                await User_Module_Groupe_Services.DeleteAll();
                UserDialogs.Instance.ShowLoading("Chargement en cours Veuillez patienter ...");
                await Task.Delay(1500);
                await App.Current.MainPage.Navigation.PushAsync(new LoginView());
                UserDialogs.Instance.HideLoading();


            }
        }
        private async void OnManageConnections()
        {
            await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new ManageConnectionsView()));

        }
        public async Task LoadConnections()
        {
            var configService = new JsonConfigService();
            if (CurrentConnection == null)
            {
                CurrentConnection = await configService.LoadSettingsAsync();
            }
            var history = await configService.LoadHistoryAsync();

            Device.BeginInvokeOnMainThread(() =>
            {
                SavedConnections.Clear();

                if (history?.Connections != null)
                {
                    // Exclure la connexion actuelle
                    foreach (var conn in history.Connections
                        .Where(c => CurrentConnection == null || c.ConnectionId != CurrentConnection.ConnectionId)
                        .OrderByDescending(c => c.ConnectionDate))
                    {
                        SavedConnections.Add(conn);
                    }
                }
            });
        }
        private async Task<int> UserCheckModulelogin()
        {
            int CrmGroupe = 0;
            int iduser = Preferences.Get("iduser", 0);
            var UMG = await User_Module_Groupe_Services.GetGroupeCRM(iduser);

            if (UMG != null)
            {
                CrmGroupe = UMG.IdGroup;
            }
            return CrmGroupe;
        }
        private void UpdateUIForGroup(int CrmGroupe)
        {
            // Mettez ici la logique pour mettre à jour l'interface en fonction du groupe
            switch (CrmGroupe)
            {
                case 27:
                    AllOppIsVisible = MyOppIsVisible = AddOppIsVisible = AddPaymentIsVisible = MyFormsIsVisible = AllFormsIsVisible = false;
                    break;
                case 28:
                    // Configuration pour le groupe 28
                    break;
                case 32:
                    AllOppIsVisible = AllFormsIsVisible = false;
                    break;
                case 37:
                    AllOppIsVisible = MyOppIsVisible = AddOppIsVisible = true;
                    break;
                default:
                    AllOppIsVisible = MyOppIsVisible = AddOppIsVisible = AddPaymentIsVisible = MyFormsIsVisible = AllFormsIsVisible = false;
                    break;
            }
        }

        public async Task SelectConnectionAsync(ServerSettings connection)
        {
            if (connection == null) return;

            // Créer une instance de LoginViewModel pour réutiliser sa logique
            var loginVM = new LoginViewModel
            {
                Server = new Server
                {
                    Address = connection.Address,
                    Name = connection.Name,
                    Password = connection.Password,
                    Data_base = connection.Database,
                    Port = connection.Port
                },
                User = new User
                {
                    Login = connection.Login,
                    Password = connection.PasswordLogin
                }
            };

            // Appeler la méthode de connexion
            var success = await loginVM.TryLoginAsync(connection);

            if (success)
            {
                // Rediriger vers HomeView après une connexion réussie
                var CrmGroupe = await loginVM.UserCheckModule();

                // Mettre à jour l'UI en fonction du groupe (comme dans LoginViewModel)
                UpdateUIForGroup(CrmGroupe);

                UserDialogs.Instance.ShowLoading("Loading...");
                await Task.Delay(1000);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
                UserDialogs.Instance.HideLoading();
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", loginVM.Message, "OK");
            }
        }
        //    public async Task SelectConnectionAsync(ServerSettings connection)
        //{
        //    if (connection == null)
        //        return;

        //    var configService = new JsonConfigService();

        //    // Sauvegarder les préférences de connexion
        //    Preferences.Set("address", connection.Address);
        //    Preferences.Set("name", connection.Name);
        //    Preferences.Set("password", connection.Password);
        //    Preferences.Set("database", connection.Database);
        //    Preferences.Set("port", connection.Port);
        //    Preferences.Set("UserName", connection.Login); 
        //    Preferences.Set("Password", connection.PasswordLogin);

        //    CurrentConnection = connection;
        //    connection.ConnectionDate = DateTime.Now;

        //    // Mise à jour de l'historique
        //    await configService.AddToHistoryAsync(connection);
        //    await configService.SaveSettingsAsync(connection);
        //    Preferences.Set("current_connection_id", connection.ConnectionId);

        //    // Mise à jour de la base locale
        //    DbConnection.Update();

        //    var connectivity = DbConnection.ConnectionIsTrue();
        //    if (connectivity)
        //    {
        //        User.Login = connection.Login;
        //        User.Password = connection.PasswordLogin;

        //        var loginSuccess = User.LoginTrue();
        //        if (loginSuccess)
        //        {
        //            if (User != null)
        //            {
        //                Preferences.Set("UserName", User.Login);
        //                Preferences.Set("Password", User.Password);
        //            }

        //            var CrmGroupe = Task.Run(async () => await UserCheckModulelogin()).Result;
        //            UpdateUIForGroup(CrmGroupe);
        //            Preferences.Set("UserName", User.Login);
        //            Preferences.Set("Password", User.Password);

        //            switch (CrmGroupe)
        //            {
        //                case 27:
        //                    UserDialogs.Instance.ShowLoading("Loading, please wait...");
        //                    Task.Delay(1000).GetAwaiter();
        //                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
        //                    UserDialogs.Instance.HideLoading();
        //                    break;
        //                case 28:
        //                    UserDialogs.Instance.ShowLoading("Loading, please wait...");
        //                    Task.Delay(1000).GetAwaiter();
        //                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
        //                    UserDialogs.Instance.HideLoading();
        //                    break;
        //                case 32:
        //                    UserDialogs.Instance.ShowLoading("Loading, please wait...");
        //                    Task.Delay(1000).GetAwaiter();
        //                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
        //                    UserDialogs.Instance.HideLoading();
        //                    break;
        //                case 37:
        //                    UserDialogs.Instance.ShowLoading("Loading, please wait...");
        //                    Task.Delay(1000).GetAwaiter();
        //                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
        //                    UserDialogs.Instance.HideLoading();
        //                    break;
        //                default:
        //                    UserDialogs.Instance.ShowLoading("Loading, please wait...");
        //                    Task.Delay(1000).GetAwaiter();
        //                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
        //                    UserDialogs.Instance.HideLoading();
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            await App.Current.MainPage.DisplayAlert("Erreur", "Échec de l’authentification", "OK");
        //        }

        //    }
        //    else
        //    {
        //        await App.Current.MainPage.DisplayAlert("Erreur", "Problème de connexion avec la base", "OK");
        //    }
        //}

        //public void SelectConnection(ServerSettings connection)
        //{
        //    if (connection != null)
        //    {
        //        // Créer un message détaillé
        //        string connectionDetails =
        //            $"ID: {connection.ConnectionId}\n" +
        //            $"Adresse: {connection.Address}\n" +
        //            $"Port: {connection.Port}\n" +
        //            $"Utilisateur: {connection.Name}\n" +
        //            $"Base de données: {connection.Database}\n" +
        //            $"Date: {connection.ConnectionDate:dd/MM/yyyy HH:mm}";

        //        // Afficher toutes les informations
        //        Application.Current.MainPage.DisplayAlert(
        //            $"Détails Connexion {connection.ConnectionId}",
        //            connectionDetails,
        //            "OK");

        //        // Mettre à jour les préférences si nécessaire
        //        Preferences.Set("address", connection.Address);
        //        Preferences.Set("name", connection.Name);
        //        Preferences.Set("password", connection.Password);
        //        Preferences.Set("database", connection.Database);
        //        Preferences.Set("port", connection.Port);
        //    }
        //}

        /*****************************************/




        private async Task contactsFun()
        {

            try
            {
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                //await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new opportunity_formView()));
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }

        }
        
        private async Task Requests()
        {

            try
            {
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new MyRequestsTap()));
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }

        }
        private async Task Validate()
        {

            try
            {
                 UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new TabPageValidationUpdates()));
                 UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }

        }


        //----------------------------------------------------------
        private async Task test()
        {

            try
            {
                  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                // await App.Current.MainPage.Navigation.PushAsync(new TestEtat());
                 UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }

        }

        private async Task VisibilteAdd()
        {
            List<string> modules = user_contrat.ListModules;
            List<string> btnInvisibles = user_contrat.ListInVisibleBtn;

            foreach (string btn in btnInvisibles)
            {


                if (btn == "achat_add_opportunity") { Btn_add_opp = false; }
                if (btn == "achat_my_opportunies") { Btn_my_opp = false; }
                if (btn == "achat_all_opportunities") { Btn_all_opp = false; }
                if (btn == "marketing_update_profile") { Btn_all_partner_update = false; }
                if (btn == "marketing_add_partner") { Btn_add_partner = false; }
                if (btn == "marketing_validation_profile") { Btn_profile_validation = false; }
                if (btn == "marketing_add_partner_form") { Btn_add_partner_form = false; }
                if (btn == "marketing_my_partner_form") { Btn_my_partner_form = false; }
                if (btn == "marketing_all_partner_form") { Btn_all_partner_form = false; }
                if (btn == "dashboard_dashboard") { Btn_dashboard = false; }
                if (btn == "dashboard_unpaid") { Btn_unpaid = false; }
                if (btn == "dashboard_order") { Btn_order = false; }
                if (btn == "hr_add_request_day_off") { Btn_add_request_day_off = false; }
                if (btn == "hr_my_request_day_off") { Btn_my_request_day_off = false; }
                if (btn == "hr_all_request_day_off") { Btn_all_request_deposit = false; }
                if (btn == "hr_add_deposit_request") { Btn_add_request_deposit = false; }
                if (btn == "hr_my_deposit_request") { Btn_my_request_deposit = false; }
                if (btn == "hr_all_deposit_request") { Btn_all_request_deposit = false; }
                if (btn == "comptabilite_add_payment") { Btn_add_payment = false; }
                if (btn == "comptabilite_my_payment") { Btn_my_payment = false; }
                if (btn == "comptabilite_all_payment") { Btn_all_payment = false; }
                if (btn == "comptabilite_add_transfer") { Btn_add_transfer = false; }
                if (btn == "comptabilite_my_cashdesk") { Btn_my_cashdesk = false; }
                if (btn == "comptabilite_all_cashdesk") { Btn_all_cashdesk = false; }
                if (btn == "comptabilite_my_waiting_transfer") { Btn_my_waiting_tranfer = false; }
                if (btn == "comptabilite_my_transfer") { Btn_my_transfer = false; }
                if (btn == "comptabilite_all_transfer") { Btn_all_cashdesk = false; }






            }


        }






        private async Task changeProfilePage()
        {

            try
            {
                 UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                // await App.Current.MainPage.Navigation.PushAsync(new ValidateUpdateOfProfile());
                  UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }

        }


        private async Task AllPartnerCommandFonction()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await user_contrat.getInfo();

            try
            {
               

                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new ShowTest()));

            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }

            UserDialogs.Instance.HideLoading();













        }

        public async Task<int> LoadNumbers()
        {

            string sqlCmd = "SELECT  \"1\" as id, count(marketing_profile_attribut_value_temp.id) as count1\r\nfrom marketing_profile_attribut_value_temp\r\nleft join atooerp_user on   atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\nleft join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\nleft join marketing_profile_attribut on marketing_profile_attribut_value.attribut= marketing_profile_attribut.Id\r\nleft join marketing_profile_instances on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\nleft join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\nleft join commercial_partner on marketing_profile_instances.partner=commercial_partner.Id\r\nleft join commercial_partner as com2 on marketing_profile_attribut_value_temp.partner=com2.Id\r\nleft join commercial_partner_category as current_category on com2.category=current_category.id\r\nleft join atooerp_type_element as element1 on marketing_profile_attribut_value_temp.type = element1.id\r\nleft join atooerp_type_element as element2 on marketing_profile_attribut_value.type = element2.id\r\nleft join atooerp_person as person on person.id=33\r\n\r\nleft join commercial_partner_category as temp_category on marketing_profile_attribut_value_temp.int_value=temp_category.id\r\nwhere marketing_profile_attribut_value_temp.profile_instance_temp is null and  marketing_profile_attribut_value_temp.partner_temp is null\r\nand marketing_profile_attribut_value_temp.state = 1\r\nand (marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance  group by instance.partner) or marketing_profile_attribut_value_temp.attribut_name is not null)\r\nunion\r\nselect \"2\" as id ,count(m.id) as count1\r\nFROM marketing_profile_instance_temp m\r\nleft join commercial_partner partner on partner.id = m.partner\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join marketing_profile_instances on marketing_profile_instances.partner=m.partner and marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance group by instance.partner)\r\nleft join marketing_profile profile on profile.id=marketing_profile_instances.profil\r\nleft join hr_employe employe on employe.user=m.user\r\nleft join atooerp_person on atooerp_person.id=employe.id\r\nwhere m.partner_temp is null and m.instance is null and m.state=1\r\n\r\n\r\nunion\r\nSELECT \"3\" as id , count(c.id) as count1\r\nFROM commercial_partner_temp c\r\nleft join atooerp_person on  atooerp_person.id = c.employe\r\nleft join  marketing_profile_attribut_value_temp on marketing_profile_attribut_value_temp.partner_temp=c.id and marketing_profile_attribut_value_temp.attribut_name=\"name\"\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=c.id\r\nWHERE c.partner is null and c.state=1\r\n\r\n;";
            int result = 0;



            if (await DbConnection.Connecter3())
            {

                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    try
                    {


                        result += Convert.ToInt32(reader["count1"]);




                    }
                    catch (Exception ex)
                    {
                        reader.Close();


                    }

                }

                reader.Close();
                DbConnection.Deconnecter();
            }
            else
            {
                return result;

            }
            return result;



        }
        private async Task AllPartner()
        {
            await user_contrat.getInfo();

            try
            {

                // await App.Current.MainPage.Navigation.PushAsync(new PartnerListForUpdate());
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }

        }

        private async Task addPartnerTemp()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await user_contrat.getInfo();
            if (await DbConnection.Connecter3())
            {
                
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new AddPartnerTemp()));
                UserDialogs.Instance.HideLoading();

            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "OK");
                UserDialogs.Instance.HideLoading();
            }
        }
        private async Task update_partner()
        {


            try
            {
                // await App.Current.MainPage.Navigation.PushAsync(new ProfileUpdate(1));
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }











        }
        private async Task TestCommand()
        {

            UserDialogs.Instance.Toast("All Partners ...");
            await Task.Delay(200);

            if (await DbConnection.Connecter3())
            {

                try
                {
                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new AllPartnerForForms()));
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
        private async Task changeToNotifications()
        {
            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                //  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");

                //    await App.Current.MainPage.Navigation.PushAsync(new NotificationsProfilePartner());
                //  UserDialogs.Instance.HideLoading();
            }
            else
                await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");
        }
        private async Task HolidayAll()
        {

              UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new Day_Off_requestList(1)));
             UserDialogs.Instance.HideLoading();




        }
        public async Task Insert()
        {

            await user_contrat.getInfo();
            if (await DbConnection.Connecter3())
            {
                  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new AjouterAvance()));
                  UserDialogs.Instance.HideLoading();
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "OK");
            }




        }

        private async Task HolidayMy()
        {


            await user_contrat.getInfo();
             UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new Day_Off_requestList(0)));

              UserDialogs.Instance.HideLoading();






        }
        private async Task changeToProfilePartner4()
        {
            //  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            DbConnection.Deconnecter();
            // await App.Current.MainPage.Navigation.PushAsync(new AllPartnerProfile());

            // UserDialogs.Instance.HideLoading();

        }
        private async Task changeToProfilePartner2()
        {
            DbConnection.Deconnecter();
            //  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            //   await App.Current.MainPage.Navigation.PushAsync(new AllPartnerProfile());



        }
        private async Task changeToProfilePartner()
        {
            //  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            DbConnection.Deconnecter();

            if (DbConnection.Connecter())
            {



                //      await App.Current.MainPage.Navigation.PushAsync(new AllPartnerProfile());





            }
            else
                await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");

            //   UserDialogs.Instance.HideLoading();

        }

        private async Task changeInsert()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);



            if (await DbConnection.Connecter3())
            {
                // UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                //await Task.Delay(300);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new AjouterCongé()));
               

            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "OK");
            }


             UserDialogs.Instance.HideLoading();



        }
        public bool IsNetworkLikelyAvailable()
        {
            return CrossConnectivity.Current.IsConnected;
        }



        public async Task avanceMy()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(1000);
            await user_contrat.getInfo();

            //  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new Avance_list(0)));
             UserDialogs.Instance.HideLoading();





        }

        public async Task avanceAll()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(1000);






            //    UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new Avance_list(1)));
               UserDialogs.Instance.HideLoading();


        }

        private async Task MyWaitingTransfer()
        {
             UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await App.Current.MainPage.Navigation.PushAsync(new MyTransferListView(1));
              UserDialogs.Instance.HideLoading();

        }

        private async Task AddTransfer()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");

               await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new MyCash_deskView()));
              UserDialogs.Instance.HideLoading();

        }

        private async Task MyForms()
        {
            UserDialogs.Instance.Toast("My Forms ...");
            await Task.Delay(200);
            if (await DbConnection.Connecter3())
                try
                {
                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new PartnerFormView("My_Forms")));

                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.Navigation.PopAsync();
                }

            else
                await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");







        }

        private async Task AllForms()
        {
            UserDialogs.Instance.Toast("All Forms ...");
            await Task.Delay(200);
            if (await DbConnection.Connecter3())
                try
                {
                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new PartnerFormView("All_Forms")));

                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.Navigation.PopAsync();
                }

            else
                await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");




        }

        private async Task AddPartnerForms()
        {

            //   UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            if (await DbConnection.Connecter3())
                try
                {
                    //         await App.Current.MainPage.Navigation.PushAsync(new PartnerListView());

                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.Navigation.PopAsync();
                }

            else
                await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");
            //  UserDialogs.Instance.HideLoading();
        }

       

        private async Task AllCashDesk()
        {

               UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
              await App.Current.MainPage.Navigation.PushAsync(new MyCash_deskView("this is All Cash desk"));
               UserDialogs.Instance.HideLoading();

        }

        private async Task MyCashDesk()
        {
               UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await App.Current.MainPage.Navigation.PushAsync(new MyCash_deskView());
              UserDialogs.Instance.HideLoading();

        }

        private async Task Mytransfer()
        {
             UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await App.Current.MainPage.Navigation.PushAsync(new MyTransferListView());
              UserDialogs.Instance.HideLoading();

        }
        private async Task Alltransfer()
        {

              UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await App.Current.MainPage.Navigation.PushAsync(new MyTransferListView("All transfer"));
              UserDialogs.Instance.HideLoading();

        }
        private async Task AllPayment()
        {


            UserDialogs.Instance.Toast("All Payment ...");
            await Task.Delay(200);

            if (await DbConnection.Connecter3())
            {

                try
                {

                    await App.Current.MainPage.Navigation.PushAsync(new PaymentListView());
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

        private async Task MyPayment()
        {


            UserDialogs.Instance.Toast("My Payment ...");
            await Task.Delay(200);

            if (await DbConnection.Connecter3())
            {

                try
                {

                    uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));

                    await App.Current.MainPage.Navigation.PushAsync(new PaymentListView((int)idagent));
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

        private async Task AddPayement()
        {
            UserDialogs.Instance.ShowLoading("Please wait ...");
            UserDialogs.Instance.Toast("Add New Payment ...");
            await Task.Delay(200);

            if (await DbConnection.Connecter3())
            {

                try
                {
                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new PaymentCustomers()));
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

        private async Task Order()
        {
            if (await DbConnection.Connecter3())
            {

            }
            //  await App.Current.MainPage.Navigation.PushAsync(new OrderListView());
            else
                await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");
        }

        private async void UserCheckModule()
        {
            int ComGroup = 0;
            int CrmGroupe = 0;
            int iduser = Preferences.Get("iduser", 0);
            try
            {
                var UMG = await User_Module_Groupe_Services.GetGroupeCRM(iduser);
                var UMG_Comp = await User_Module_Groupe_Services.GetGroupeComp(iduser);

                if (UMG != null)
                {
                    CrmGroupe = UMG.IdGroup;
                }

                if (UMG_Comp != null)
                {
                    ComGroup = UMG_Comp.IdGroup;
                }

                switch (CrmGroupe)
                {
                    case 27:
                        AllOppIsVisible = MyOppIsVisible = AddOppIsVisible = AddPaymentIsVisible = MyFormsIsVisible = AllFormsIsVisible = false;
                        break;
                    case 28:
                        break;
                    case 32:
                        AllOppIsVisible = AllFormsIsVisible = false;
                        break;
                    case 37:
                        AllOppIsVisible = MyOppIsVisible = AddOppIsVisible = true;
                        break;
                    default:
                        AllOppIsVisible = MyOppIsVisible = AddOppIsVisible = AddPaymentIsVisible = MyFormsIsVisible = AllFormsIsVisible = false;
                        break;
                }

                switch (ComGroup)
                {
                    case 18:
                        break;
                    case 33:
                        AllPaymentIsVisible = AllCashDeskIsVisible = AllTransferIsVisible = false;

                        break;
                    default:
                        AddPaymentIsVisible = MyPaymentIsVisible = AllPaymentIsVisible = MyCashDeskIsVisible = AllCashDeskIsVisible = MyTransferIsVisible = AllTransferIsVisible = MyWaitingTransferIsVisible = AddTransferIsVisible = PaymentIsVisible = false;

                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task AllOpp()
        {
            UserDialogs.Instance.Toast("All Opportunities ...");
            await Task.Delay(200);

            if (await DbConnection.Connecter3())
            {
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new OpportunityListView(0)));
            }
            //  await App.Current.MainPage.Navigation.PushAsync(new OrderListView());
            else
                await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");
   
            


        }

        private async Task MyOpp()
        {
            UserDialogs.Instance.Toast("My Opportunities ...");
            await Task.Delay(200);
            if (await DbConnection.Connecter3())
            {
                uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new OpportunityListView(idagent)));
                //CurrentData.CurrentNoteModule = "Opportunities";
            }
            //  await App.Current.MainPage.Navigation.PushAsync(new OrderListView());
            else
                await UserDialogs.Instance.AlertAsync("Connection failed");


            
         






        }

        private async Task AddOpp()
        {
            UserDialogs.Instance.Toast("Add New Opportunities ...");
            await Task.Delay(200);
            if (await DbConnection.Connecter3())
            {
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new CustomerView2()));
            }
            else
                await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");
        }

        private async Task Dashboard()
        {
            
               await App.Current.MainPage.Navigation.PushAsync(new DashboardingView());


        }
        private async Task Statment()
        {
            if (await DbConnection.Connecter3())
            {
                //    await App.Current.MainPage.Navigation.PushAsync(new StatmentListView());
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
                await User_Module_Groupe_Services.DeleteAll();
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(1500);
                await App.Current.MainPage.Navigation.PushAsync(new LoginView());
                UserDialogs.Instance.HideLoading();


            }
        }

        private async void Exit()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to exit!", "Yes", "No");
            if (r)
            {
                await App.Current.MainPage.Navigation.PopToRootAsync();
            }
        }
    }
}
