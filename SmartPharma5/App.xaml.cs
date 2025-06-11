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
                    //  Charger la liste des sociétés
                    Task.Run(async () =>
                    {
                        var societies = await Society.GetAllSocietiesAsync();
                    }).GetAwaiter().GetResult();

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