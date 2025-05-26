using Acr.UserDialogs;
//using SmartPharma;
//using SmartPharma.Model;
//using SmartPharma.View;
using MvvmHelpers;
using MvvmHelpers.Commands;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
//using SmartPharma.View;
Après :
using SmartPharma5.Model;
using SmartPharma5.View;
//using SmartPharma.View;
*/
using SmartPharma5.Model;
using SmartPharma5.View;
//using SmartPharma.View;
using System.Collections.ObjectModel;
using
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System.Threading.Tasks;

using SmartPharma5.Model;
using SmartPharma5.View;
using Command = MvvmHelpers.Commands.Command;
Après :
using System.Threading.Tasks;
using Command = MvvmHelpers.Commands.Command;
*/
Command = MvvmHelpers.Commands.Command;

namespace SmartPharma5.ModelView
{
    public class AvanceMV : BaseViewModel
    {

        public int affiche_type { get; set; }

        private List<ShearchItemAvance> list_shearsh_avance;
        public List<ShearchItemAvance> List_shearsh_avance { get => list_shearsh_avance; set => SetProperty(ref list_shearsh_avance, value); }

        private List<avance_request> list_avance_request;
        public List<avance_request> List_avance_request { get => list_avance_request; set => SetProperty(ref list_avance_request, value); }

        private ShearchItemAvance selectItem = ShearchItemAvance.getAllSheachItem()[0];
        public ShearchItemAvance SelectItem { get => selectItem; set => SetProperty(ref selectItem, value); }

        private DateTime startShearshDate;
        public DateTime StartShearshDate { get => startShearshDate; set => SetProperty(ref startShearshDate, value); }

        private DateTime endShearshDate;
        public DateTime EndShearshDate { get => endShearshDate; set => SetProperty(ref endShearshDate, value); }

        public AsyncCommand RefreshCommand { get; }

        private bool isDateShearsh;
        public bool IsDateShearsh { get => isDateShearsh; set => SetProperty(ref isDateShearsh, value); }



        private bool isDateStartShearsh;
        public bool IsDateStartShearsh { get => isDateStartShearsh; set => SetProperty(ref isDateStartShearsh, value); }


        private bool isDateEndShearsh;
        public bool IsDateEndShearsh { get => isDateEndShearsh; set => SetProperty(ref isDateEndShearsh, value); }



        public DateTime MinShearshStartDate { get; set; } = DateTime.Now.AddYears(-1);



        private bool isStringShearsh;
        public bool IsStringShearsh { get => isStringShearsh; set => SetProperty(ref isStringShearsh, value); }

        private bool actPopup = false;
        public bool ActPopup { get => actPopup; set => SetProperty(ref actPopup, value); }


        public bool insertAvance { get; set; }


        public string Title { get; set; }


        public AsyncCommand InsertAvance { get; }

        public AsyncCommand changeToHome { get; }

        public Command changeSearsh { get; }

        public AsyncCommand ShearshCommand { get; }

        public AsyncCommand ShaershWithDate { get; }


        private bool testCon = false;
        public bool TestCon { get => testCon; set => SetProperty(ref testCon, value); }




        public Command CommandSearch
        {
            get;

        }
        public AvanceMV() { }

        public AvanceMV(int id)
        {

            changeToHome = new AsyncCommand(changeCongé);
            RefreshCommand = new AsyncCommand(RefreshFonction);




            this.affiche_type = id;



            if (affiche_type == 0)
            {

                insertAvance = true;
                Title = "MY";
                try
                {
                    List_avance_request = new List<avance_request>(avance_request.getDepositRequestByUserId().Result);

                }
                catch (Exception ex)
                {
                    TestCon = true;
                    return;

                }


                CommandSearch = new Command((flag) =>
                {
                    Console.WriteLine(SelectItem.Name);

                    Shearch(flag.ToString());

                });

                changeSearsh = new Command(async () =>
                {

                    List_avance_request.Clear();


                    if (affiche_type == 0)
                    {
                        try
                        {
                            List_avance_request = new List<avance_request>(avance_request.getDepositRequestByUserId().Result);
                        }
                        catch (Exception ex)
                        {
                            TestCon = true;
                            return;

                        }

                    }
                    else
                    {
                        try
                        {
                            List_avance_request = new List<avance_request>(avance_request.GetRequestAvance().Result);
                        }
                        catch (Exception ex)
                        {
                            TestCon = true;
                            return;

                        }


                    }


                    StartShearshDate = DateTime.Now.AddMonths(-6);



                    if (selectItem.Name.ToLower() == "create date")
                    {

                        IsDateShearsh = true;
                        IsStringShearsh = false;

                        if (selectItem.Name.ToLower() == "create date") { IsDateEndShearsh = true; IsDateStartShearsh = true; }


                    }
                    else if (selectItem.Name.ToLower() == "description" || selectItem.Name.ToLower() == "name employe")
                    {
                        IsDateShearsh = false;
                        IsStringShearsh = true;

                    }
                    else
                    {
                        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                        await Task.Delay(1000);
                        IsDateShearsh = false;
                        IsStringShearsh = false;
                        if (selectItem.Name.ToLower() == "stat : accepter")
                        {
                            try
                            {
                                List_avance_request = new List<avance_request>();
                                List_avance_request = avance_request.getDepositRequestByStateAndUser(2).Result;

                            }
                            catch (Exception ex)
                            {
                                TestCon = true;
                                UserDialogs.Instance.HideLoading(); return;
                            }


                        }
                        if (selectItem.Name.ToLower() == "stat : refuser")
                        {
                            try
                            {
                                List_avance_request = new List<avance_request>();
                                List_avance_request = avance_request.getDepositRequestByStateAndUser(3).Result;
                            }
                            catch (Exception ex)
                            {
                                TestCon = true;
                                UserDialogs.Instance.HideLoading(); return;
                            }

                        }
                        if (selectItem.Name.ToLower() == "stat : en attent")
                        {
                            try
                            {
                                List_avance_request = new List<avance_request>();
                                List_avance_request = avance_request.getDepositRequestByStateAndUser(1).Result;
                            }
                            catch (Exception ex)
                            {
                                TestCon = true;
                                UserDialogs.Instance.HideLoading(); return;
                            }

                        }
                        UserDialogs.Instance.HideLoading();
                    }



                });
                ShaershWithDate = new AsyncCommand(getDataByDate);
            }
            else
            {

                insertAvance = false;
                Title = "All";
                try
                {

                    List_avance_request = new List<avance_request>(avance_request.GetRequestAvance().Result);
                    TestCon = false;
                }
                catch (Exception ex)
                {
                    TestCon = true;
                    List_avance_request = new List<avance_request>();
                    return;
                }


                CommandSearch = new Command((flag) =>
                {
                    Console.WriteLine(SelectItem.Name);

                    Shearch(flag.ToString());

                });
                changeSearsh = new Command(async () =>
                {


                    List_avance_request.Clear();
                    if (affiche_type == 0)
                    {
                        try
                        {

                            List_avance_request = new List<avance_request>(avance_request.getDepositRequestByUserId().Result);
                            TestCon = false;
                        }
                        catch (Exception ex)
                        {
                            TestCon = true;
                            List_avance_request = new List<avance_request>();
                            return;

                        }
                    }
                    else
                    {
                        try
                        {

                            List_avance_request = new List<avance_request>(avance_request.GetRequestAvance().Result);
                            TestCon = false;
                        }
                        catch (Exception ex)
                        {
                            TestCon = true;
                            List_avance_request = new List<avance_request>();
                            return;
                        }


                    }




                    StartShearshDate = DateTime.Now.AddMonths(-6);



                    if (selectItem.Name.ToLower() == "create date")
                    {
                        IsDateShearsh = true;
                        IsStringShearsh = false;
                        IsDateEndShearsh = true;
                        IsDateStartShearsh = true;

                    }
                    else if (selectItem.Name.ToLower() == "description" || selectItem.Name.ToLower() == "name employe")
                    {
                        IsDateShearsh = false;
                        IsStringShearsh = true;

                    }
                    else
                    {
                        
                        IsDateShearsh = false;
                        IsStringShearsh = false;
                        if (selectItem.Name.ToLower() == "stat : accepter")
                        {
                            try
                            {

                                List_avance_request = new List<avance_request>(avance_request.getDepositRequestByState(2).Result);
                                TestCon = false;

                            }
                            catch (Exception ex)
                            {
                                TestCon = true;
                                List_avance_request = new List<avance_request>();
                               
                                return;
                            }
                        }
                        if (selectItem.Name.ToLower() == "stat : refuser")
                        {
                            try
                            {

                                List_avance_request = new List<avance_request>(avance_request.getDepositRequestByState(3).Result);
                                TestCon = false;
                            }
                            catch (Exception ex)
                            {
                                TestCon = true;
                                List_avance_request = new List<avance_request>();
                                return;
                            }

                        }
                        if (selectItem.Name.ToLower() == "stat : en attent")
                        {
                            try
                            {
                                TestCon = false;
                                List_avance_request = new List<avance_request>(avance_request.getDepositRequestByState(1).Result);

                            }
                            catch (Exception ex)
                            {
                                TestCon = true;
                                List_avance_request = new List<avance_request>();
                                
                                return;
                            }

                        }
                        UserDialogs.Instance.HideLoading();
                    }

                });
                ShaershWithDate = new AsyncCommand(getDataByDate);

            }



            List_shearsh_avance = ShearchItemAvance.getAllSheachItem();

            changeToHome = new AsyncCommand(changeCongé);
            InsertAvance = new AsyncCommand(Insert);


        }

        public async Task RefreshFonction()
        {
            List_avance_request = new List<avance_request>();
            if (affiche_type== 0)
            {
                try
                {
                    List_avance_request = new List<avance_request>(avance_request.getDepositRequestByUserId().Result);

                }
                catch (Exception ex)
                {
                    TestCon = true;
                    return;

                }

               
            }
            else
            {
                try
                {

                    List_avance_request = new List<avance_request>(avance_request.GetRequestAvance().Result);
                    TestCon = false;
                }
                catch (Exception ex)
                {
                    TestCon = true;
                    List_avance_request = new List<avance_request>();
                    return;
                }

            }
            IsBusy= false;

        }


        public async Task changeCongé()
        {
            await App.Current.MainPage.Navigation.PushAsync(new HomeView());
        }

        public async Task Insert()
        {
            await App.Current.MainPage.Navigation.PushAsync(new AjouterAvance());
        }

        public async Task getDataByDate()
        {

            if (affiche_type == 0)
            {
                List_avance_request.Clear();
                if (selectItem.Name.ToLower() == "create date")
                {
                    try
                    {
                        TestCon = false;

                        List_avance_request = new List<avance_request>(avance_request.getDepositRequestByCreateDateAndUser(StartShearshDate.ToString("yyyy-MM-dd HH:mm:ss"), EndShearshDate.ToString("yyyy-MM-dd HH:mm:ss")).Result);
                    }
                    catch (Exception ex)
                    {
                        TestCon = true;
                        List_avance_request = new List<avance_request>(); return;
                    }
                }


            }
            else
            {
                List_avance_request.Clear();
                if (selectItem.Name.ToLower() == "create date")
                {

                    try
                    {
                        TestCon = false;
                        List_avance_request = new List<avance_request>(avance_request.getDepositRequestByCreateDate(StartShearshDate.ToString("yyyy-MM-dd HH:mm:ss"), EndShearshDate.ToString("yyyy-MM-dd HH:mm:ss")).Result);
                    }
                    catch (Exception ex) { TestCon = true; return; }
                }


            }


        }








        private async Task Shearch(string flag)
        {

            if (affiche_type == 1)
            {
                try
                {
                    TestCon = false;
                    List_avance_request = new List<avance_request>(avance_request.GetRequestAvance().Result);
                }
                catch (Exception ex)
                {
                    List_avance_request = new List<avance_request>();
                    TestCon = true;
                    return;
                }


                ObservableCollection<avance_request> list1 = new ObservableCollection<avance_request>(List_avance_request);

                if (SelectItem.Name.ToLower() == "description")
                {
                    this.List_avance_request = list1.Where(s => s.description.ToUpper().Contains(flag.ToUpper())).ToList();
                }
                else
                {
                    this.List_avance_request = list1.Where(s => s.nom_employe.ToUpper().Contains(flag.ToUpper())).ToList();

                }

            }
            else
            {

                try
                {
                    TestCon = false;
                    List_avance_request = new List<avance_request>(avance_request.getDepositRequestByUserId().Result);
                }
                catch (Exception ex)
                {
                    List_avance_request = new List<avance_request>();
                    TestCon = true;
                    return;

                }

                ObservableCollection<avance_request> list1 = new ObservableCollection<avance_request>(List_avance_request);

                if (SelectItem.Name.ToLower() == "description")
                {
                    this.List_avance_request = list1.Where(s => s.description.ToUpper().Contains(flag.ToUpper())).ToList();
                }
                else
                {
                    this.List_avance_request = list1.Where(s => s.nom_employe.ToUpper().Contains(flag.ToUpper())).ToList();

                }

            }







        }
    }
}
