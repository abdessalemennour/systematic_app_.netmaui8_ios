//using DevExpress.Utils.Commands;
//using Acr.UserDialogs;
//using SmartPharma;
//using SmartPharma.Model;
//using SmartPharma.View;
using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System;
Après :
using SmartPharma5.Model;
using SmartPharma5.View;
using System;
*/
using SmartPharma5.Model;
using SmartPharma5.View;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System.Collections.ObjectModel;

using System.Linq;


using System.Threading.Tasks;
Après :
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
*/
using System.Collections.ObjectModel;
using Command = MvvmHelpers.Commands.Command;
//using SmartPharma5.view;

namespace SmartPharma5.ModelView
{
    public class CongéMV : BaseViewModel
    {


        public int type_affichage { get; set; }

        private List<day_off_request> list_day_off;
        public List<day_off_request> List_day_off { get => list_day_off; set => SetProperty(ref list_day_off, value); }

        public AsyncCommand RefreshCommand { get; }

        private bool actPopup = false;
        public bool ActPopup { get => actPopup; set => SetProperty(ref actPopup, value); }

        public AsyncCommand InsertCongé { get; }

        public AsyncCommand changeToHome { get; }

        public AsyncCommand ShearshCommand { get; }

        public AsyncCommand ShaershWithDate { get; }


        private List<ShearchItem> listItemsShearch;
        public List<ShearchItem> ListItemsShearch { get => listItemsShearch; set => SetProperty(ref listItemsShearch, value); }



        private ShearchItem selectItem = ShearchItem.getAllSheachItem()[0];
        public ShearchItem SelectItem { get => selectItem; set => SetProperty(ref selectItem, value); }


        public string name_user1 { get; set; }

        public string name_exercice { get; set; }
        public decimal day_off_number { get; set; }
        public decimal day_off_use { get; set; }
        public decimal pay_slip_day_off_number { get; set; }

        public decimal day_off_rest { get; set; }

        private DateTime startShearshDate;
        public DateTime StartShearshDate { get => startShearshDate; set => SetProperty(ref startShearshDate, value); }

        private DateTime endShearshDate;
        public DateTime EndShearshDate { get => endShearshDate; set => SetProperty(ref endShearshDate, value); }

        public bool insertCongé { get; set; }


        public string Title { get; set; }

        public bool HasContract
        {
            get
            {
                return user_contrat.HasNoContract;
            }
        }




        private bool isDateShearsh;
        public bool IsDateShearsh { get => isDateShearsh; set => SetProperty(ref isDateShearsh, value); }



        private bool isDateStartShearsh;
        public bool IsDateStartShearsh { get => isDateStartShearsh; set => SetProperty(ref isDateStartShearsh, value); }


        private bool isDateEndShearsh;
        public bool IsDateEndShearsh { get => isDateEndShearsh; set => SetProperty(ref isDateEndShearsh, value); }



        public DateTime MinShearshStartDate { get; set; } = DateTime.Now.AddYears(-1);



        private bool isStringShearsh;
        public bool IsStringShearsh { get => isStringShearsh; set => SetProperty(ref isStringShearsh, value); }

        public Command changeSearsh { get; }

        private bool testCon = false;
        public bool TestCon { get => testCon; set => SetProperty(ref testCon, value); }






        public Command CommandSearch
        {
            get;

        }

        public CongéMV() { }



        public CongéMV(int id)
        {
            this.type_affichage = id;

            if (this.type_affichage == 0)
            {
                this.insertCongé = true;
                this.Title = "MY";


            }
            else
            {
                this.insertCongé = false;
                this.Title = "All";

            }
            if (id == 1)
            {

                try
                {

                    StartShearshDate = DateTime.Now;



                    user_contrat.getInfo();


                    name_exercice = user_contrat.name_exercice;
                    name_user1 = user_contrat.nameuser;
                    day_off_number = user_contrat.day_off_number;
                    day_off_use = user_contrat.day_off_use;
                    pay_slip_day_off_number = user_contrat.pay_slip_day_off_number;
                    day_off_rest = user_contrat.day_off_rest;




                    listItemsShearch = ShearchItem.getAllSheachItem();
                    List_day_off = new List<day_off_request>();

                    try
                    {

                        List_day_off = new List<day_off_request>(day_off_request.GetRequestDayOff().Result);
                    }
                    catch (Exception ex)
                    {
                        List_day_off = new List<day_off_request>();
                        TestCon = true;
                        return;
                    }

                    RefreshCommand = new AsyncCommand(Refresh);
                    InsertCongé = new AsyncCommand(changeInsert);

                    CommandSearch = new Command((flag) =>
                    {
                        Console.WriteLine(SelectItem.Name);

                        Shearch(flag.ToString());

                    });

                    changeSearsh = new Command(async () =>
                    {

                        List_day_off.Clear();
                        try
                        {

                            List_day_off = new List<day_off_request>(day_off_request.GetRequestDayOff().Result);
                        }
                        catch (Exception ex)
                        {
                            List_day_off = new List<day_off_request>();
                            TestCon = true;
                            return;
                        }
                        StartShearshDate = DateTime.Now.AddMonths(-6);
                        Console.WriteLine("ggggg");


                        if (selectItem.Name.ToLower() == "create date" || selectItem.Name.ToLower() == "start date" || selectItem.Name.ToLower() == "end date")
                        {

                            IsDateShearsh = true;
                            IsStringShearsh = false;

                            if (selectItem.Name.ToLower() == "create date") { IsDateEndShearsh = true; IsDateStartShearsh = true; }
                            if (selectItem.Name.ToLower() == "start date") { IsDateEndShearsh = false; IsDateStartShearsh = true; }
                            if (selectItem.Name.ToLower() == "end date") { IsDateEndShearsh = true; IsDateStartShearsh = false; }
                            //UserDialogs.Instance.HideLoading();

                        }
                        else if (selectItem.Name.ToLower() == "description" || selectItem.Name.ToLower() == "name employe")
                        {
                            IsDateShearsh = false;
                            IsStringShearsh = true;
                            //UserDialogs.Instance.HideLoading();

                        }
                        else
                        {
                            // UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                            await Task.Delay(1000);
                            IsDateShearsh = false;
                            IsStringShearsh = false;
                            if (selectItem.Name.ToLower() == "stat : accepter")
                            {
                                try
                                {
                                    List_day_off = new List<day_off_request>(day_off_request.getDataByState(2).Result);
                                    // UserDialogs.Instance.HideLoading();

                                }
                                catch (Exception ex)
                                {
                                    TestCon = true;
                                    List_day_off = new List<day_off_request>();
                                    //  UserDialogs.Instance.HideLoading();
                                    return;

                                }

                            }
                            if (selectItem.Name.ToLower() == "stat : refuser")
                            {
                                try
                                {
                                    List_day_off = new List<day_off_request>(day_off_request.getDataByState(2).Result);
                                    // UserDialogs.Instance.HideLoading();

                                }
                                catch (Exception ex)
                                {
                                    TestCon = true;
                                    List_day_off = new List<day_off_request>();
                                    // UserDialogs.Instance.HideLoading();
                                    return;

                                }
                            }
                            if (selectItem.Name.ToLower() == "stat : en attent")
                            {
                                try
                                {
                                    List_day_off = new List<day_off_request>(day_off_request.getDataByState(2).Result);
                                    // UserDialogs.Instance.HideLoading();

                                }
                                catch (Exception ex)
                                {
                                    TestCon = true;
                                    List_day_off = new List<day_off_request>();
                                    //  UserDialogs.Instance.HideLoading();
                                    return;

                                }
                            }
                            // UserDialogs.Instance.HideLoading();
                        }

                    });



                    changeToHome = new AsyncCommand(changeAvance);

                    ShaershWithDate = new AsyncCommand(getDataByDate);


                }
                catch (Exception ex)
                {
                    TestCon = true;
                }

            }
            else
            {
                try
                {
                    StartShearshDate = DateTime.Now;



                    user_contrat.getInfo();


                    name_exercice = user_contrat.name_exercice;
                    name_user1 = user_contrat.nameuser;
                    day_off_number = user_contrat.day_off_number;
                    day_off_use = user_contrat.day_off_use;
                    pay_slip_day_off_number = user_contrat.pay_slip_day_off_number;
                    day_off_rest = user_contrat.day_off_rest;




                    listItemsShearch = ShearchItem.getAllSheachItem();
                    list_day_off = new List<day_off_request>();
                    try
                    {
                        List_day_off = new List<day_off_request>(day_off_request.getRequestByUserId().Result);

                    }
                    catch (Exception ex)
                    {
                        List_day_off = new List<day_off_request>();

                        TestCon = true;
                        return;


                    }

                    RefreshCommand = new AsyncCommand(Refresh);
                    InsertCongé = new AsyncCommand(changeInsert);

                    CommandSearch = new Command((flag) =>
                    {
                        Console.WriteLine(SelectItem.Name);

                        Shearch(flag.ToString());

                    });

                    changeSearsh = new Command(async () =>
                    {

                        // UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                        List_day_off.Clear();
                        try
                        {
                            List_day_off = new List<day_off_request>(day_off_request.getRequestByUserId().Result);

                        }
                        catch (Exception ex)
                        {
                            List_day_off = new List<day_off_request>();
                            // UserDialogs.Instance.HideLoading();
                            TestCon = true;
                            return;


                        }
                        StartShearshDate = DateTime.Now.AddMonths(-6);
                        Console.WriteLine("ggggg");


                        if (selectItem.Name.ToLower() == "create date" || selectItem.Name.ToLower() == "start date" || selectItem.Name.ToLower() == "end date")
                        {

                            IsDateShearsh = true;
                            IsStringShearsh = false;

                            if (selectItem.Name.ToLower() == "create date") { IsDateEndShearsh = true; IsDateStartShearsh = true; }
                            if (selectItem.Name.ToLower() == "start date") { IsDateEndShearsh = false; IsDateStartShearsh = true; }
                            if (selectItem.Name.ToLower() == "end date") { IsDateEndShearsh = true; IsDateStartShearsh = false; }
                            // UserDialogs.Instance.HideLoading();

                        }
                        else if (selectItem.Name.ToLower() == "description" || selectItem.Name.ToLower() == "name employe")
                        {
                            IsDateShearsh = false;
                            IsStringShearsh = true;
                            // UserDialogs.Instance.HideLoading();
                        }
                        else
                        {
                            //  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                            await Task.Delay(1000);

                            IsDateShearsh = false;
                            IsStringShearsh = false;
                            if (selectItem.Name.ToLower() == "stat : accepter")
                            {
                                try
                                {
                                    List_day_off = new List<day_off_request>(day_off_request.getDataByStateAndUser(2).Result);
                                    //   UserDialogs.Instance.HideLoading();

                                }
                                catch (Exception ex)
                                {
                                    List_day_off = new List<day_off_request>();
                                    TestCon = true;
                                    //  UserDialogs.Instance.HideLoading();
                                    return;


                                }

                            }
                            if (selectItem.Name.ToLower() == "stat : refuser")
                            {

                                try
                                {
                                    List_day_off = new List<day_off_request>(day_off_request.getDataByStateAndUser(3).Result);
                                    //   UserDialogs.Instance.HideLoading();

                                }
                                catch (Exception ex)
                                {
                                    List_day_off = new List<day_off_request>();
                                    TestCon = true;
                                    //  UserDialogs.Instance.HideLoading();
                                    return;
                                }

                            }
                            if (selectItem.Name.ToLower() == "stat : en attent")
                            {
                                try
                                {
                                    List_day_off = new List<day_off_request>(day_off_request.getDataByStateAndUser(1).Result);
                                    //  UserDialogs.Instance.HideLoading();

                                }
                                catch (Exception ex)
                                {
                                    List_day_off = new List<day_off_request>();
                                    TestCon = true;
                                    //  UserDialogs.Instance.HideLoading();
                                    return;
                                }
                            }
                        }

                    });



                    changeToHome = new AsyncCommand(changeAvance);

                    ShaershWithDate = new AsyncCommand(getDataByDate);

                }
                catch (Exception ex)
                {
                    TestCon = true;
                }



            }





















        }

        public async Task changeAvance()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await App.Current.MainPage.Navigation.PushAsync(new HomeView());
            UserDialogs.Instance.HideLoading();
        }

        public async Task getDataByDate()
        {


            if (type_affichage == 0)
            {
                // UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(1000);
                List_day_off.Clear();

                if (selectItem.Name.ToLower() == "create date")
                {
                    try
                    {
                        List_day_off = new List<day_off_request>(day_off_request.getDataByDateAndUser(StartShearshDate.ToString("yyyy-MM-dd HH:mm:ss"), EndShearshDate.ToString("yyyy-MM-dd HH:mm:ss")).Result);
                        // UserDialogs.Instance.HideLoading();
                    }
                    catch (Exception ex)
                    {
                        List_day_off = new List<day_off_request>();
                        TestCon = true;
                        //  UserDialogs.Instance.HideLoading();
                        return;
                    }
                }
                if (selectItem.Name.ToLower() == "start date")
                {
                    try
                    {
                        List_day_off = new List<day_off_request>(day_off_request.getDataByStartDateAndUser(startShearshDate).Result);
                        // UserDialogs.Instance.HideLoading();

                    }
                    catch (Exception ex)
                    {
                        List_day_off = new List<day_off_request>();
                        TestCon = true;
                        // UserDialogs.Instance.HideLoading();
                        return;
                    }


                }
                if (selectItem.Name.ToLower() == "end date")
                {
                    try
                    {
                        List_day_off = new List<day_off_request>(day_off_request.getDataByEndDateAndUser(EndShearshDate).Result);
                        // UserDialogs.Instance.HideLoading();
                    }
                    catch (Exception ex)
                    {
                        List_day_off = new List<day_off_request>();
                        TestCon = true;
                        // UserDialogs.Instance.HideLoading();
                        return;

                    }

                }

            }
            else
            {
                //  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(1000);
                List_day_off.Clear();
                if (selectItem.Name.ToLower() == "create date")
                {
                    try
                    {
                        List_day_off = new List<day_off_request>(day_off_request.getDataByDate(StartShearshDate.ToString("yyyy-MM-dd HH:mm:ss"), EndShearshDate.ToString("yyyy-MM-dd HH:mm:ss")).Result);
                        //  UserDialogs.Instance.HideLoading();
                    }
                    catch (Exception ex)
                    {
                        List_day_off = new List<day_off_request>();
                        //  UserDialogs.Instance.HideLoading();
                        TestCon = true;
                        return;

                    }
                }
                if (selectItem.Name.ToLower() == "start date")
                {
                    try
                    {
                        List_day_off = new List<day_off_request>(day_off_request.getDataByStartDate(startShearshDate).Result);
                        // UserDialogs.Instance.HideLoading();
                    }
                    catch (Exception ex)
                    {
                        TestCon = true;
                        // UserDialogs.Instance.HideLoading();
                        List_day_off = new List<day_off_request>();
                        return;

                    }
                }
                if (selectItem.Name.ToLower() == "end date")
                {
                    try
                    {
                        List_day_off = new List<day_off_request>(day_off_request.getDataByEndDate(EndShearshDate).Result);
                        // UserDialogs.Instance.HideLoading();

                    }
                    catch (Exception ex)
                    {
                        TestCon = true;
                        // UserDialogs.Instance.HideLoading();
                        List_day_off = new List<day_off_request>();
                        return;
                    }

                }

            }



            //UserDialogs.Instance.HideLoading();

        }








        private async Task Refresh()
        {
            if (type_affichage == 1)
            {
                //await Task.Delay(2000);
                this.List_day_off.Clear();
                try
                {

                    List_day_off = new List<day_off_request>(day_off_request.GetRequestDayOff().Result);
                    // UserDialogs.Instance.HideLoading();
                }
                catch (Exception ex)
                {
                    List_day_off = new List<day_off_request>();

                    TestCon = true;
                    return;
                }


                IsBusy = false;
            }
            else
            {
                //await Task.Delay(2000);
                this.List_day_off.Clear();
                try
                {
                    List_day_off = new List<day_off_request>(day_off_request.getRequestByUserId().Result);
                    //  UserDialogs.Instance.HideLoading();

                }
                catch (Exception ex)
                {
                    List_day_off = new List<day_off_request>();

                    TestCon = true;
                    return;


                }


                IsBusy = false;


            }










        }

        private void Shearch(string flag)
        {
            //  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");

            if (type_affichage == 1)
            {
                try
                {

                    List_day_off = new List<day_off_request>(day_off_request.GetRequestDayOff().Result);
                    //  UserDialogs.Instance.HideLoading();
                }
                catch (Exception ex)
                {
                    List_day_off = new List<day_off_request>();
                    // UserDialogs.Instance.HideLoading();
                    TestCon = true;
                    return;
                }

                ObservableCollection<day_off_request> list1 = new ObservableCollection<day_off_request>(List_day_off);

                if (SelectItem.Name.ToLower() == "description")
                {
                    this.List_day_off = list1.Where(s => s.description.ToUpper().Contains(flag.ToUpper())).ToList();
                }
                else
                {
                    this.List_day_off = list1.Where(s => s.nom_employe.ToUpper().Contains(flag.ToUpper())).ToList();

                }

            }
            else
            {

                try
                {
                    List_day_off = new List<day_off_request>(day_off_request.getRequestByUserId().Result);
                    //  UserDialogs.Instance.HideLoading();

                }
                catch (Exception ex)
                {
                    List_day_off = new List<day_off_request>();
                    //  UserDialogs.Instance.HideLoading();
                    TestCon = true;
                    return;


                }

                ObservableCollection<day_off_request> list1 = new ObservableCollection<day_off_request>(List_day_off);

                if (SelectItem.Name.ToLower() == "description")
                {
                    this.List_day_off = list1.Where(s => s.description.ToUpper().Contains(flag.ToUpper())).ToList();
                }
                else
                {
                    this.List_day_off = list1.Where(s => s.nom_employe.ToUpper().Contains(flag.ToUpper())).ToList();

                }

            }




            // UserDialogs.Instance.HideLoading();


        }
        private async Task changeInsert()
        {
            await App.Current.MainPage.Navigation.PushAsync(new AjouterCongé());
        }



    }
}
