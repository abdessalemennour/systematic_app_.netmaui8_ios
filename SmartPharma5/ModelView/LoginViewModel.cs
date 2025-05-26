
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.Models;
using SmartPharma5.Services;
//using SmartPharma5.View;
Après :
//using SmartPharma5.View;
using MvvmHelpers;
using SmartPharma5.View;
*/
//using MvvmHelpers.Commands;
//using Xamarin.Essentials;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
Après :
using Xamarin.Forms;


using SmartPharma5.Model;
using SmartPharma5.Models;
using SmartPharma5.Services;
*/
//using MvvmHelpers.Commands;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.View;
using System;
using System.Collections.Generic;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Après :
using SmartPharma5.Services;
using SmartPharma5.View;
using SmartPharma5.View;
using System;
using System.Collections.Generic;
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
*/
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.View;
using MvvmHelpers.Commands;
Après :
using SmartPharma5.Text;
using System.Threading.Tasks;
*/
using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using SmartPharma5.Model;
using SmartPharma5.Models;
using
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.Services;
using SmartPharma5.View;
using Command = MvvmHelpers.Commands.Command;
Après :
using Command = MvvmHelpers.Commands.Command;
*/
SmartPharma5.Services;
using SmartPharma5.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Command = MvvmHelpers.Commands.Command;
//using SmartPharma5.View;
//using SmartPharma53.View;

//using Acr.UserDialogs;

namespace SmartPharma5.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        public User User { get; set; }
        private Server server;
        public Server Server { get => server; set => SetProperty(ref server, value); }
        public AsyncCommand LoginCommand { get; }
        public Command Tryagain { get; }
        /***************************/
        public ObservableCollection<ServerSettings> SavedConnections { get; } = new ObservableCollection<ServerSettings>();
        public ICommand UseSavedConnectionCommand { get; }
        private ServerSettings _currentConnection;
        public ServerSettings CurrentConnection
        {
            get => _currentConnection;
            set => SetProperty(ref _currentConnection, value);
        }
        /**************************/
        private bool actpopup = false;
        private bool successpopup = false;
        private bool fieldpopup = false;
        private string message;
        public string Message { get => message; set => SetProperty(ref message, value); }
        public bool SuccessPopup { get => successpopup; set => SetProperty(ref successpopup, value); }
        public bool FieldPopup { get => fieldpopup; set => SetProperty(ref fieldpopup, value); }
        public bool ActPopup { get => actpopup; set => SetProperty(ref actpopup, value); }

        public LoginViewModel()
        {
            LoginCommand = new AsyncCommand(Login);
            Tryagain = new Command(() => FieldPopup = false);
            User = new User();
            //instance de la classe Server
            //Server = new Server(Preferences.Get("name", "root"), Preferences.Get("password", "1261986"), Preferences.Get("address", "192.168.1.100"), Preferences.Get("database", "atooerp"), Preferences.Get("port", 3306));
            Server = new Server(Preferences.Get("name", "maui"), Preferences.Get("password", "0000"), Preferences.Get("address", "141.94.195.211"), Preferences.Get("database", "maui"), Preferences.Get("port", 3306));

            /******************************/
            UseSavedConnectionCommand = new Microsoft.Maui.Controls.Command<ServerSettings>(UseSavedConnection);
            // Charger les connexions sauvegardées au démarrage
            Task.Run(async () => await LoadSavedConnections());
            /******************************/
        }

        /******************************************/
        private async Task LoadSavedConnections()
        {
            try
            {
                SavedConnections.Clear();
                var configService = new JsonConfigService();
                var history = await configService.LoadHistoryAsync();

                if (history?.Connections != null)
                {
                    // Trier par date récente et prendre les 5 dernières
                    foreach (var conn in history.Connections
                        .OrderByDescending(c => c.ConnectionDate)
                        .Take(5))
                    {
                        SavedConnections.Add(conn);
                    }
                }
            }
            catch (Exception ex)
            {
                //Debug.WriteLine($"Erreur chargement connexions: {ex.Message}");
            }
        }
        private void UseSavedConnection(ServerSettings connection)
        {
            if (connection == null) return;

            // Mettre à jour les propriétés du serveur
            Server.Address = connection.Address;
            Server.Port = connection.Port;
            Server.Name = connection.Name;
            Server.Password = connection.Password;
            Server.Data_base = connection.Database;

            // Optionnel: déclencher automatiquement la connexion
            // LoginCommand.Execute(null);
            OnPropertyChanged(nameof(Server));

        }

        /*********************************************/
        private async Task Login()

        /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
        Avant :
                {

                    ActPopup = true;
        Après :
                {

                    ActPopup = true;
        */
        {

            ActPopup = true;
            await Task.Delay(1000);

            var Connectivity = DbConnection.CheckConnectivity();

            if (await Server.ServerConnectionIsTrue())
            {
                Preferences.Set("address", Server.Address);
                Preferences.Set("name", Server.Name);
                Preferences.Set("password", Server.Password);
                Preferences.Set("database", Server.Data_base);
                Preferences.Set("port", Server.Port);

                /**************************************************/
                var configService = new JsonConfigService();

                // Charger l'historique pour trouver la connexion correspondante
                var history = await configService.LoadHistoryAsync();
                var existingConnection = history?.Connections?.FirstOrDefault(c =>
                    c.Address == Server.Address &&
                    c.Name == Server.Name &&
                    c.Password == Server.Password &&
                    c.Database == Server.Data_base &&
                    c.Port == Server.Port);

                if (existingConnection != null)
                {
                    // Utiliser la connexion existante avec son ConnectionId
                    CurrentConnection = existingConnection;
                    existingConnection.ConnectionDate = DateTime.Now; // Mettre à jour la date
                }
                else
                {
                    // Créer une nouvelle connexion
                    CurrentConnection = new ServerSettings
                    {
                        Login = User.Login,
                        PasswordLogin = User.Password,
                        Address = Server.Address,
                        Name = Server.Name,
                        Password = Server.Password,
                        Database = Server.Data_base,
                        Port = Server.Port,
                        ConnectionDate = DateTime.Now
                    };
                }

                // Sauvegarder dans l'historique (cela générera/mettra à jour le ConnectionId)
                await configService.AddToHistoryAsync(CurrentConnection);
                CurrentConnection.CustomName = $"connexion{CurrentConnection.ConnectionId}";
                // Sauvegarder comme connexion actuelle
                await configService.SaveSettingsAsync(CurrentConnection);
                Preferences.Set("current_connection_id", CurrentConnection.ConnectionId);

                /***************************************************/

                DbConnection.Update();


                if (Connectivity)
                {
                    var DbConnectivity = DbConnection.ConnectionIsTrue();
                    if (DbConnectivity)
                    {
                        var LoginSuccess = User.LoginTrue();
                        if (LoginSuccess)

                        /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                        Avant :
                                                {


                                                    uint IdAgent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                        Après :
                                                {


                                                    uint IdAgent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                        */
                        {


                            uint IdAgent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));

                            ActPopup = false;
                            await Task.Delay(1000);
                            SuccessPopup = true;
                            await Task.Delay(2000);

                            /*************************/
                            // Sauvegarde finale après succès complet
                            await configService.SaveSettingsAsync(CurrentConnection);
                            Preferences.Set("current_connection_id", CurrentConnection.ConnectionId);

                            // Préparer la HomeView avec la connexion actuelle
                            var homeView = new HomeView();
                            if (homeView.BindingContext is HomeViewModel homeViewModel)
                            {
                                homeViewModel.CurrentConnection = this.CurrentConnection;
                                await homeViewModel.LoadConnections();
                            }

                            //string historyContent = await configService.GetFormattedHistoryAsync();
                            //await App.Current.MainPage.DisplayAlert("Historique des connexions", historyContent, "OK");

                            /***********************/
                            //await App.Current.MainPage.DisplayAlert("Success", "Login success! Redirecting...", "OK");
                            var CrmGroupe = Task.Run(async () => await UserCheckModule()).Result;
                            Preferences.Set("UserName", User.Login);
                            Preferences.Set("Password", User.Password);
                            switch (CrmGroupe)
                            {

                                case 27:
                                    //  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                                    //  Task.Delay(1000).GetAwaiter();

                                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
                                    //   UserDialogs.Instance.HideLoading();

                                    break;
                                case 28:
                                    UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                                    Task.Delay(1000).GetAwaiter();
                                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
                                    //await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
                                    // await Shell.Current.GoToAsync("///HomeView");


                                    UserDialogs.Instance.HideLoading();

                                    break;
                                case 32:
                                    //  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                                    // Task.Delay(1000).GetAwaiter();
                                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
                                    //  UserDialogs.Instance.HideLoading();

                                    break;
                                case 37:
                                    // UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                                    // Task.Delay(1000).GetAwaiter();
                                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
                                    // UserDialogs.Instance.HideLoading();

                                    break;
                                default:
                                    //UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                                    //  Task.Delay(1000).GetAwaiter();
                                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
                                    // UserDialogs.Instance.HideLoading();
                                    break;
                            }
                            SuccessPopup = false;

                        }
                        else
                        {
                            //await App.Current.MainPage.DisplayAlert("Warning", "Please enter a correct username and password", "OK");
                            Message = "Please enter a correct username and password";
                            ActPopup = false;
                            await Task.Delay(1000);
                            FieldPopup = true;
                        }
                    }
                    else
                    {
                        //await App.Current.MainPage.DisplayAlert("Warning", "There is a problem in connecting to the server. \n Please contact our support for further assistance.", "OK");
                        Message = "There is a problem in connecting to the server. \n Please contact our support for further assistance.";
                        ActPopup = false;
                        await Task.Delay(1000);
                        FieldPopup = true;
                    }
                }
                else
                {
                    Message = "No network connectivity.";
                    //ActPopup = false;
                    //await Task.Delay(1000);
                    //FieldPopup = true;
                }
            }
            else
            {
                Message = "Please enter a correct Server Parameter";
                ActPopup = false;
                await Task.Delay(1000);
                FieldPopup = true;
            }

        }
        public async Task<bool> TryLoginAsync(ServerSettings connection, User user = null)
        {
            ActPopup = true;
            await Task.Delay(1000);

            // Mettre à jour les préférences avec la nouvelle connexion
            Preferences.Set("address", connection.Address);
            Preferences.Set("name", connection.Name);
            Preferences.Set("password", connection.Password);
            Preferences.Set("database", connection.Database);
            Preferences.Set("port", connection.Port);

            // Mettre à jour l'utilisateur si fourni
            if (user != null)
            {
                User = user;
                Preferences.Set("UserName", user.Login);
                Preferences.Set("Password", user.Password);
            }

            // Vérifier la connectivité
            if (!await Server.ServerConnectionIsTrue())
            {
                Message = "Server connection failed.";
                ActPopup = false;
                return false;
            }

            // Mettre à jour la connexion dans la base locale
            DbConnection.Update();

            if (!DbConnection.ConnectionIsTrue())
            {
                Message = "Database connection failed.";
                ActPopup = false;
                return false;
            }

            // Authentifier l'utilisateur
            if (!User.LoginTrue())
            {
                Message = "Invalid credentials.";
                ActPopup = false;
                return false;
            }

            // Sauvegarder la connexion dans l'historique
            var configService = new JsonConfigService();
            await configService.AddToHistoryAsync(connection);
            await configService.SaveSettingsAsync(connection);
            Preferences.Set("current_connection_id", connection.ConnectionId);

            ActPopup = false;
            return true;
        }
        public async Task<int> UserCheckModule()
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
    }
}
