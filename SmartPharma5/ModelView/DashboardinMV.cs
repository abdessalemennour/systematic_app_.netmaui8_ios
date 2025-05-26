using Acr.UserDialogs;
using DevExpress.Maui.Charts;
using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5;
using SmartPharma5.Model;
using SmartPharma5.Services;
using SmartPharma5.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ChartView = SmartPharma5.View.ChartView;

namespace SmartPharma5.ModelView
{
    public class DashboardinMV : BaseViewModel
    {





        private bool dashboardisvisible = true;
        public bool DashboardIsVisible { get => dashboardisvisible; set => SetProperty(ref dashboardisvisible, value); }
        private bool mydashboardisvisible = true;
        public bool MyDashboardIsVisible { get => mydashboardisvisible; set => SetProperty(ref mydashboardisvisible, value); }




        private List<DashBoardingModel> list_Dashboard;
        public List<DashBoardingModel> List_Dashboard { get => list_Dashboard; set => SetProperty(ref list_Dashboard, value); }



        public AsyncCommand GoToGrid { get; }
        public AsyncCommand MyDashBoardCommand { get; }
        public AsyncCommand AllDashBoardCommand { get; }

        public DashboardinMV()
        {


            List_Dashboard = GetListDashboard().Result;

            GoToGrid = new AsyncCommand(goToGrid);
            MyDashBoardCommand = new AsyncCommand(MyDash);
            AllDashBoardCommand = new AsyncCommand(AllDash);
            UserCheckModule();

        }

        public async Task MyDash()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new SammaryView((uint)Preferences.Get("idagent", Convert.ToUInt32(null)))));
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Warning", "VERIFY YOUR XML FILE IS SUCCEFULY INTEGRATED!", "OK");
            }

        }
        public async Task AllDash()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new SammaryView()));
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }

        }

        public async Task goToGrid()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
               // await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new ChartView()));
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                await DbConnection.ErrorConnection();
            }
            UserDialogs.Instance.HideLoading();


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


            switch (CrmGroupe)
            {
                case 27:
                    MyDashboardIsVisible = DashboardIsVisible = false;
                    break;
                case 28:

                    break;
                case 32:
                    DashboardIsVisible = false;
                    break;
                case 37:

                    break;
                default:

                    break;
            }
        }
        public async  Task<List<DashBoardingModel>> GetListDashboard()
        {
            List<DashBoardingModel> list = new List<DashBoardingModel>();
           
            try
            {
                string sqlCmd1 = "SELECT id,name FROM atooerp_app_dashboard";

                DbConnection.Deconnecter();
                DbConnection.Connecter();

                MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {

                        list.Add(new DashBoardingModel(Convert.ToInt32(reader["id"]), reader["name"].ToString())) ;
                        



                       



                    }
                    catch (Exception ex)
                    {
                        return null;

                    }

                }
                reader.Close();
                DbConnection.Deconnecter();




            }
            catch (Exception e)
            {
                return null;

            }



            return list;




        }
    }

}
public class DashBoardingModel{
    public int Id { get; set; }
    public string Name { get; set; }
    public DashBoardingModel(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public async Task GoTOMyDashboar()
    {
        try
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new DashBoardCustomazedView(this.Id)));
            UserDialogs.Instance.HideLoading();
        }
        catch (Exception ex)
        {
            UserDialogs.Instance.HideLoading();
            await DbConnection.ErrorConnection();
        }
        UserDialogs.Instance.HideLoading();

    }
}
