using MySqlConnector;
using SmartPharma5.Model;
using SmartPharma5.ModelView;
using SmartPharma5.Services;
using SmartPharma5.View;
using System.ComponentModel;
using System.Data;
using SmartPharma5.View;
using System.Diagnostics;

namespace SmartPharma5
{
    public partial class App : Application
    {
        public static BindingList<Tax> taxList;
        public static NotificationViewModel NotificationVM { get; } = new NotificationViewModel();

        public static BindingList<Tax.Type> taxTypeList;
        public uint IdEmploye { get; set; }
        public int IdUser { get; set; }
        public User User { get; set; }



        public App()
        {
            try
            {
                //user_contrat.GetLogoFromDatabase().GetAwaiter() ;
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzAyMzE5M0AzMjM0MmUzMDJlMzBtY2lxOXBDZmJZOUVlRlhzVEd5QVBVV2VDeStPZDI1L1BUTStOU0VtUXBBPQ==");
                InitializeComponent();

                user_contrat.getInfo().GetAwaiter();
                user_contrat.getModules().GetAwaiter();
                //user_contrat.getResponsabilities();
                // MainPage = new NavigationPage(new NavigationPage(new ShowMenuItemPages()));

                //MainPage = new NavigationPage(new BarPieGaugeViews());

                try
                {

                    string login = Preferences.Get("UserName", "");
                    string password = Preferences.Get("Password", "");
                    User = new User(login, password);
                    IdEmploye = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                    IdUser = Preferences.Get("iduser", 0);
                    var LoginSuccess = false;

                    if (login == "" && password == "")
                    {



                        LoginSuccess = false;

                    }
                    else
                    {
                        LoginSuccess = true;
                        // LoginSuccess = User.LoginTrue(login,password).Result;
                    }


                    //Task.Run(async()=>await User_Module_Groupe_Services.DeleteAll()) ;

                    // Task.Run(async () => await updateLocalDataBase(IdUser));





                    var CrmGroupe = Task.Run(async () => await UserCheckModule()).Result;

                    try
                    {
                        bool? IsActif = User.UserIsActif(Convert.ToUInt32(IdEmploye));
                        if (IsActif == null)
                        {


                            MainPage = new NavigationPage(new NavigationPage(new TestInternet()));
                        }
                        else
                        {


                            if (IsActif == false)
                            {
                                MainPage = new NavigationPage(new NavigationPage(new LoginView()));
                                return;

                            }




                            if (IdEmploye == 0)
                            {
                                MainPage = new NavigationPage(new NavigationPage(new LoginView()));
                                return;


                            }


                            else
                            {


                                switch (CrmGroupe)
                                {
                                    case 27:

                                        MainPage = new NavigationPage(new NavigationPage(new HomeView()));
                                        break;
                                    case 28:

                                        MainPage = new NavigationPage(new NavigationPage(new SammaryView()));
                                        break;
                                    case 32:

                                        MainPage = new NavigationPage(new NavigationPage(new SammaryView(IdEmploye)));
                                        break;
                                    case 37:

                                        MainPage = new NavigationPage(new NavigationPage(new SammaryView()));
                                        break;
                                    default:

                                        MainPage = new NavigationPage(new NavigationPage(new HomeView()));
                                        break;
                                }
                            }

                            taxList = Tax.getList();
                            taxTypeList = Tax.Type.getList();
                        }

                    }
                    catch (Exception ex)
                    {

                        MainPage = new LoginView();
                    }

                }
                catch (Exception ex)
                {
                }

            }
            catch (Exception ex)
            {

            }


        }
        //public App()
        //{
        //    try
        //    {
        //        // Initialisation Syncfusion
        //        try
        //        {
        //            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzAyMzE5M0AzMjM0MmUzMDJlMzBtY2lxOXBDZmJZOUVlRlhzVEd5QVBVV2VDeStPZDI1L1BUTStOU0VtUXBBPQ==");
        //            Console.WriteLine("License Syncfusion initialisée avec succès");
        //        }
        //        catch (Exception syncEx)
        //        {
        //            Console.WriteLine($"ERREUR Syncfusion: {syncEx.Message}");
        //            throw;
        //        }

        //        InitializeComponent();
        //        Console.WriteLine("Application initialisée");

        //        // Chargement des données utilisateur
        //        try
        //        {
        //            user_contrat.getInfo().GetAwaiter();
        //            user_contrat.getModules().GetAwaiter();
        //            Console.WriteLine("Données utilisateur chargées avec succès");
        //        }
        //        catch (Exception userDataEx)
        //        {
        //            Console.WriteLine($"ERREUR Chargement données utilisateur: {userDataEx.Message}");
        //        }

        //        try
        //        {
        //            string login = Preferences.Get("UserName", "");
        //            string password = Preferences.Get("Password", "");
        //            Console.WriteLine($"Récupération des préférences - Login: {login}");

        //            User = new User(login, password);
        //            IdEmploye = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
        //            IdUser = Preferences.Get("iduser", 0);
        //            var LoginSuccess = !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password);

        //            Console.WriteLine($"ID Employé: {IdEmploye}, ID User: {IdUser}, LoginSuccess: {LoginSuccess}");

        //            if (!LoginSuccess)
        //            {
        //                Console.WriteLine("Aucun login/password trouvé, redirection vers LoginView");
        //                MainPage = new NavigationPage(new LoginView());
        //                return;
        //            }

        //            // Vérification du module CRM
        //            try
        //            {
        //                var CrmGroupe = Task.Run(async () => await UserCheckModule()).Result;
        //                Console.WriteLine($"Module CRM récupéré: {CrmGroupe}");
        //            }
        //            catch (Exception crmEx)
        //            {
        //                Console.WriteLine($"ERREUR Module CRM: {crmEx.Message}");
        //                throw;
        //            }

        //            // Vérification statut utilisateur
        //            try
        //            {
        //                bool? IsActif = User.UserIsActif(Convert.ToUInt32(IdEmploye));
        //                Console.WriteLine($"Statut utilisateur: {IsActif}");

        //                if (IsActif == null)
        //                {
        //                    Console.WriteLine("Statut utilisateur null, redirection vers TestInternet");
        //                    MainPage = new NavigationPage(new TestInternet());
        //                    return;
        //                }

        //                if (IsActif == false)
        //                {
        //                    Console.WriteLine("Utilisateur inactif, redirection vers LoginView");
        //                    MainPage = new NavigationPage(new LoginView());
        //                    return;
        //                }

        //                if (IdEmploye == 0)
        //                {
        //                    Console.WriteLine("ID Employé = 0, redirection vers LoginView");
        //                    MainPage = new NavigationPage(new LoginView());
        //                    return;
        //                }

        //                // Gestion de la navigation en fonction du groupe CRM

        //                try
        //                {
        //                    var CrmGroupe = Task.Run(async () => await UserCheckModule()).Result;
        //                    Console.WriteLine($"Navigation basée sur le groupe CRM: {CrmGroupe}");

        //                    switch (CrmGroupe)
        //                    {
        //                        case 27:
        //                            Console.WriteLine("Redirection vers HomeView");
        //                            MainPage = new NavigationPage(new HomeView());
        //                            break;
        //                        case 28:
        //                            MainPage = new NavigationPage(new NavigationPage(new SammaryView()));
        //                            break;
        //                        case 32:
        //                            MainPage = new NavigationPage(new NavigationPage(new SammaryView(IdEmploye)));
        //                            break;
        //                        case 37:
        //                            Console.WriteLine($"Redirection vers SammaryView avec ID Employé: {IdEmploye}");
        //                            MainPage = new NavigationPage(new SammaryView(IdEmploye));
        //                            break;
        //                        default:
        //                            Console.WriteLine("Groupe CRM non reconnu, redirection vers HomeView par défaut");
        //                            MainPage = new NavigationPage(new HomeView());
        //                            break;
        //                    }

        //                    // Chargement des taxes
        //                    try
        //                    {
        //                        taxList = Tax.getList();
        //                        taxTypeList = Tax.Type.getList();
        //                        Console.WriteLine($"Taxes chargées - Count: {taxList?.Count}");
        //                    }
        //                    catch (Exception taxEx)
        //                    {
        //                        Console.WriteLine($"ERREUR Chargement taxes: {taxEx.Message}");
        //                    }
        //                }
        //                catch (Exception navEx)
        //                {
        //                    Console.WriteLine($"ERREUR Navigation: {navEx.Message}");
        //                    MainPage = new LoginView();
        //                }
        //            }
        //            catch (Exception statusEx)
        //            {
        //                Console.WriteLine($"ERREUR Vérification statut: {statusEx.Message}");
        //                MainPage = new LoginView();
        //            }
        //        }
        //        catch (Exception mainEx)
        //        {
        //            Console.WriteLine($"ERREUR PRINCIPALE: {mainEx.Message}");
        //            Console.WriteLine($"STACK TRACE: {mainEx.StackTrace}");
        //            MainPage = new LoginView();
        //        }
        //    }
        //    catch (Exception globalEx)
        //    {
        //        Console.WriteLine($"ERREUR GLOBALE DANS LE CONSTRUCTEUR APP: {globalEx.Message}");
        //        Console.WriteLine($"STACK TRACE COMPLET: {globalEx.StackTrace}");

        //        // Solution pour l'erreur "Either set MainPage or override CreateWindow"
        //        MainPage = new NavigationPage(new LoginView());
        //    }
        //}


        protected override void OnStart()
        {
            // Initialiser le Shell (si ce n'est pas déjà fait)
            /* Shell shell = (Shell)MainPage;
             if (shell == null)
             {
                 MainPage = new AppShell();
                 shell = (Shell)MainPage;
             }

             // Naviguer vers la page souhaitée
             shell.GoToAsync("//appshell/LoginView");*/
        }
        public async Task updateLocalDataBase(int iduser)
        {
            string sqlCmd = "SELECT Id ,atooerp_user_module_group.user, module,atooerp_user_module_group.group FROM atooerp_user_module_group WHERE(atooerp_user_module_group.user = " + iduser + ");";
            try
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                adapter.SelectCommand.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                try
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        await User_Module_Groupe_Services.




                                Adddb(new User_module_groupe(
                            Convert.ToInt32(row["Id"]),
                            Convert.ToInt32(row["user"]),
                            Convert.ToInt32(row["module"]),
                            Convert.ToInt32(row["group"])));
                    }
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }
        }
        public static async Task<int> UserCheckModule()
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