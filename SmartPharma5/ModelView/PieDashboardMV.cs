using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.Model;
using SmartPharma5.Services;
using SmartPharma5.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Command = MvvmHelpers.Commands.Command;

namespace SmartPharma5.ModelView
{
    class PieDashboardMV : BaseViewModel
    {
        public Command RefreshCommand { get; set; }

        private List<LandAreaItem> landareas;
        public List<LandAreaItem> LandAreas { get => landareas; set => SetProperty(ref landareas, value); }

        private List<LandAreaItem> landareasdirectandindirect;
        public List<LandAreaItem> LandAreasDirectAndIndirect { get => landareasdirectandindirect; set => SetProperty(ref landareasdirectandindirect, value); }

        private IReadOnlyList<LandAreaItem> landareasnbdirectandindirect;
        public IReadOnlyList<LandAreaItem> LandAreasNbDirectAndIndirect { get => landareasnbdirectandindirect; set => SetProperty(ref landareasnbdirectandindirect, value); }

        private IReadOnlyList<LandAreaItem> landareasavrgdirectandindirect;
        public IReadOnlyList<LandAreaItem> LandAreasAvrgDirectAndIndirect { get => landareasavrgdirectandindirect; set => SetProperty(ref landareasavrgdirectandindirect, value); }
        private IList<LandAreaItem> landareastobbyagent;
        public IList<LandAreaItem> LandAreasTOBbyAgent { get => landareastobbyagent; set => SetProperty(ref landareastobbyagent, value); }

        private IList<LandAreaItem> landareaskpibyagent;
        public IList<LandAreaItem> LandAreasKPIByAgent { get => landareaskpibyagent; set => SetProperty(ref landareaskpibyagent, value); }

        private IList<LandAreaItem> landareasjobposition;
        public IList<LandAreaItem> LandAreasJobPosition { get => landareasjobposition; set => SetProperty(ref landareasjobposition, value); }

        public AsyncCommand LogoutCommand { get; set; }

        private bool load_finish = false;
        public bool Load_finish { get => load_finish; set => SetProperty(ref load_finish, value); }


        private bool loading = true;
        public bool Loading { get => loading; set => SetProperty(ref loading, value); }

        private string arg;
        public string Arg { get => arg; set => SetProperty(ref arg, value); }

        private int gaugevalue;
        public int GaugeValue { get => gaugevalue; set => SetProperty(ref gaugevalue, value); }
        private int gaugemaxvalue;
        public Int32 GaugeMaxValue { get => gaugemaxvalue; set => SetProperty(ref gaugemaxvalue, value); }
        private int togoal;
        public int ToGoal { get => togoal; set => SetProperty(ref togoal, value); }
        private DateTime startdate = DateTime.Now - new TimeSpan(30, 0, 0, 0);
        public DateTime StartDate { get => startdate; set => SetProperty(ref startdate, value); }
        private DateTime enddate = DateTime.Now;
        public DateTime EndDate { get => enddate; set => SetProperty(ref enddate, value); }
        private int totalunpaid;
        public int TotalUnpaid { get => totalunpaid; set => SetProperty(ref totalunpaid, value); }
        private int totalgagnee;
        public int TotalGagnee { get => totalgagnee; set => SetProperty(ref totalgagnee, value); }
        private int totalperdu;
        public int TotalPerdu { get => totalperdu; set => SetProperty(ref totalperdu, value); }
        private int totalenattente;
        public int TotalEnAttente { get => totalenattente; set => SetProperty(ref totalenattente, value); }
        private int totalproceccing;
        public int TotalProceccing { get => totalproceccing; set => SetProperty(ref totalproceccing, value); }
        private int nbunpaid;
        public int NbUnpaid { get => nbunpaid; set => SetProperty(ref nbunpaid, value); }
        private DateTime? maxduedate;
        public DateTime? MaxDueDate { get => maxduedate; set => SetProperty(ref maxduedate, value); }
        private string agentname = "All Agent";
        public string agentName { get => agentname; set => SetProperty(ref agentname, value); }
        private bool landareastobbyagentisvisible = true;
        public bool LandAreasTOBbyAgentIsVisible { get => landareastobbyagentisvisible; set => SetProperty(ref landareastobbyagentisvisible, value); }


        private DataTable generalquery;
        public DataTable GeneralQuery { get => generalquery; set => SetProperty(ref generalquery, value); }
        public Task<DataTable> ResultGeneral { get; set; }


        public DateTime DateNow = DateTime.Now.Date;



        public int MoyenDirect { get; set; }
        public int MoyenGross { get; set; }
        public int ToDirect { get; set; }
        public int ToGross { get; set; }

        private int nowvisited;
        public int NowVisited { get => nowvisited; set => SetProperty(ref nowvisited, value); }
        private int nowvisit;
        public int NowVisit { get => nowvisit; set => SetProperty(ref nowvisit, value); }
        private int nowtovisit;
        public int NowToVisit { get => nowtovisit; set => SetProperty(ref nowtovisit, value); }
        private decimal nowkpi;
        public decimal NowKPI { get => nowkpi; set => SetProperty(ref nowkpi, value); }
        private int periodvisited;
        public int PeriodVisited { get => periodvisited; set => SetProperty(ref periodvisited, value); }
        private int periodvisit;
        public int PeriodVisit { get => periodvisit; set => SetProperty(ref periodvisit, value); }
        private int periodtovisit;
        public int PeriodToVisit { get => periodtovisit; set => SetProperty(ref periodtovisit, value); }
        private decimal periodkpi;
        public decimal PeriodKPI { get => periodkpi; set => SetProperty(ref periodkpi, value); }
        private int cyclevisited;
        public int CycleVisited { get => cyclevisited; set => SetProperty(ref cyclevisited, value); }
        private int cyclevisit;
        public int CycleVisit { get => cyclevisit; set => SetProperty(ref cyclevisit, value); }
        private int cycletovisit;
        public int CycleToVisit { get => cycletovisit; set => SetProperty(ref cycletovisit, value); }
        private decimal cyclekpi;
        public decimal CycleKPI { get => cyclekpi; set => SetProperty(ref cyclekpi, value); }

        IEnumerable<DataRow> querydirect { get; set; }
        IEnumerable<DataRow> queryindirect { get; set; }
        DataTable querydirectandindirect { get; set; }
        DataTable queryunpaid { get; set; }
        DataTable queryOpp { get; set; }
        IEnumerable<DataRow> querynow { get; set; }

        public int IdJobPosition { get; set; } = 0;
        public uint IdAgent { get; set; } = 0;
        int UserCount { get; set; } = 0;


        public PieDashboardMV()
        {
            try
            {


                RefreshCommand = new Command(Load);
                Task.Run(() => Load());
                //Init();
                LogoutCommand = new AsyncCommand(Logout);
            }
            catch (Exception ex)
            { }


        }
        public PieDashboardMV(uint idAgent)
        {

            LandAreasTOBbyAgentIsVisible = false;
            agentName = "";
            IdAgent = idAgent;
            RefreshCommand = new Command(LoadByAgent);
            Task.Run(() => LoadByAgent());
            Init();



            LogoutCommand = new AsyncCommand(Logout);

        }



        public async void Load()
        {
            Load_finish = false;
            Loading = true;


            Init();
            IdAgent = 0;
            IdJobPosition = 0;
            agentName = "All Agent";
            try
            {
                StartDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 00, 00, 00);
                EndDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 23, 59, 59);
                //EndDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 00, 00, );

                var Opp = Task.Run(() => GetDataOpp());
                queryOpp = await Opp;

                var Result = Task.Run(() => GetDataDirectAndIndirect());
                querydirectandindirect = (await Result);
                querydirect = GetRowDirect(querydirectandindirect);
                queryindirect = GetRowInDirect(querydirectandindirect);

                var Unpaid = Task.Run(() => GetDataUnpaid());
                queryunpaid = await Unpaid;

                var User = Task.Run(() => GetUserCount());
                UserCount = await User;


                ResultGeneral = Task.Run(() => GetGeneralPartnerFormInformation());
                GeneralQuery = await ResultGeneral;

                //querynow = GetRowInDirectNow(GeneralQuery);


                FillInterface();

                Load_finish = true;
                Loading = false;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }
        public async void LoadByAgent()
        {
            Load_finish = false;
            Loading = true;
            Init();

            try
            {
                StartDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 00, 00, 00);
                EndDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 00, 00, 00);

                var Opp = Task.Run(() => GetDataOppByAgent());
                queryOpp = await Opp;

                var Result = Task.Run(() => GetDataDirectAndIndirectByAgent());
                querydirect = GetRowDirect(await Result);
                queryindirect = GetRowInDirect(await Result);
                querydirectandindirect = (await Result);

                var Unpaid = Task.Run(() => GetDataUnpaidByAgent());
                queryunpaid = await Unpaid;

                ResultGeneral = Task.Run(() => GetGeneralPartnerFormInformation());
                GeneralQuery = await ResultGeneral;
                if (GeneralQuery != null)
                    try
                    {
                        GeneralQuery = GeneralQuery.AsEnumerable().Where(x => x.Field<uint>("employe_id") == IdAgent).CopyToDataTable();
                    }
                    catch (Exception ex)
                    {
                        GeneralQuery = null;
                    }

                FillInterface();


                Load_finish = true;
                Loading = false;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public async void LoadByJobPosition(int jobposition)
        {
            Load_finish = false;
            Loading = true;



            Init();

            try
            {
                StartDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 00, 00, 00);
                EndDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 00, 00, 00);



                var Result = Task.Run(() => GetDataDirectAndIndirect());

                var dtdirectandindirect = await Result;
                if (dtdirectandindirect != null)
                    try
                    {
                        querydirectandindirect = dtdirectandindirect.AsEnumerable().Where(x => x.Field<uint?>("job_poition_id") == jobposition).CopyToDataTable();
                    }
                    catch (Exception ex) { }

                querydirect = GetRowDirect(querydirectandindirect);
                queryindirect = GetRowInDirect(querydirectandindirect);
                //querydirectandindirect = (await Result);

                var Unpaid = Task.Run(() => GetDataUnpaid());
                queryunpaid = await Unpaid;

                ResultGeneral = Task.Run(() => GetGeneralPartnerFormInformation());

                GeneralQuery = await ResultGeneral;
                int index0 = GeneralQuery.Rows.Count;
                if (GeneralQuery != null)
                    try
                    {
                        GeneralQuery = GeneralQuery.AsEnumerable().Where(x => x.Field<uint>("job_poition_id") == jobposition).CopyToDataTable();
                        int index1 = GeneralQuery.Rows.Count;
                    }
                    catch (Exception ex) { }

                FillInterface();


                Load_finish = true;
                Loading = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void Init()
        {
            GeneralQuery = new DataTable();

            NowVisit = 0;
            NowVisited = 0;
            NowToVisit = 0;
            NowKPI = 0;
            PeriodVisit = 0;
            PeriodVisited = 0;
            PeriodToVisit = 0;
            PeriodKPI = 0;
            CycleVisit = 0;
            CycleVisited = 0;
            CycleToVisit = 0;
            CycleKPI = 0;

            ToGoal = 0;
            GaugeMaxValue = 1;
            GaugeValue = 0;
            MoyenDirect = 0;
            MoyenGross = 0;
            ToDirect = 0;
            ToGross = 0;
            TotalUnpaid = 0;
            TotalGagnee = 0;
            TotalPerdu = 0;
            TotalEnAttente = 0;
            TotalProceccing = 0;
            MaxDueDate = DateTime.Now;
            LandAreas = new List<LandAreaItem>()
                {
                    new LandAreaItem("En Attente", 1),
                    new LandAreaItem("Perdu", 1),
                    new LandAreaItem("Gagneé", 1)

                };
            LandAreasDirectAndIndirect = new List<LandAreaItem>()
                {
                    new LandAreaItem("T/O DIRECT", 1),
                    new LandAreaItem("T/O INDIRECT", 1),
                };
            LandAreasAvrgDirectAndIndirect = new List<LandAreaItem>()
                {
                    new LandAreaItem("AVRAGE DIRECT", 0),
                    new LandAreaItem("AVRAGE INDIRECT", 0),
                    new LandAreaItem("AVRAGE DIRECT/INDIRECT", 0),
                };
            LandAreasNbDirectAndIndirect = new List<LandAreaItem>()
                {
                    new LandAreaItem("NB DIRECT", 1),
                    new LandAreaItem("NB INDIRECT", 1),
                };
            //var list = new List<LandAreaItem>(querydirectandindirect.AsEnumerable().GroupBy(p => p.Field<uint?>("agent"), p => p.Field<decimal>("total_amount"), (key, g) => new LandAreaItem(key.ToString(), 1)));
            if (IdAgent == 0)
            {
                LandAreasTOBbyAgent = new List<LandAreaItem>();

                if (IdJobPosition == 0)
                {
                    LandAreasKPIByAgent = new List<LandAreaItem>();
                    LandAreasJobPosition = new List<LandAreaItem>();

                }
            }


        }
        private async Task Logout()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to lougout!", "Yes", "No");
            if (r)
            {
                Preferences.Set("idagent", Convert.ToUInt32(null));
                await User_Module_Groupe_Services.DeleteAll();
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new LoginView()));
            }
        }
        public void FillInterface()

        {
            Load_finish = false;
            try
            {
                TimeSpan periode = EndDate - StartDate;

                if (IdAgent == 0)
                    ToGoal = (int)(Convert.ToInt32((periode.Days + 1) * 2000) * UserCount);
                else
                    ToGoal = (int)Convert.ToInt32((periode.Days + 1) * 2000);

                if (queryOpp.AsEnumerable().Count() > 0)
                {
                    TotalGagnee = queryOpp.AsEnumerable().Count(x => x.Field<uint?>("oppState") == 2);
                    TotalPerdu = queryOpp.AsEnumerable().Count(x => x.Field<uint?>("oppState") == 3);

                }
                if (querydirectandindirect.AsEnumerable().Count() > 0)
                {
                    TotalEnAttente = queryOpp.AsEnumerable().Count(x => x.Field<uint?>("oppState") == 1);
                    TotalProceccing = queryOpp.AsEnumerable().Count(x => x.Field<bool?>("delivred") == false);

                    ToDirect = (int)querydirect.Sum(x => x.Field<decimal>("TotalHT"));
                    ToGross = (int)queryindirect.Select(x => x.Field<decimal>("TotalHT")).Sum();

                    if (ToDirect + ToGross >= ToGoal)
                    {
                        GaugeMaxValue = ToDirect + ToGross;
                        GaugeValue = ToDirect + ToGross;
                    }
                    else if (ToDirect + ToGross > 0)
                    {
                        GaugeMaxValue = ToGoal;
                        GaugeValue = ToDirect + ToGross;
                    }



                    LandAreasDirectAndIndirect = new List<LandAreaItem>()
                    {
                    new LandAreaItem("T/O DIRECT", ToDirect),
                    new LandAreaItem("T/O INDIRECT", ToGross),
                    };





                    if (querydirect.Count() != 0)
                        MoyenDirect = (int)(querydirect.Select(x => x.Field<decimal>("TotalHT")).Sum() / querydirect.Count());
                    if (queryindirect.Count() != 0)
                        MoyenGross = (int)(queryindirect.Select(x => x.Field<decimal>("TotalHT")).Sum() / queryindirect.Count());

                    LandAreasAvrgDirectAndIndirect = new List<LandAreaItem>()
                    {
                        new LandAreaItem("AVRAGE DIRECT", MoyenDirect),
                        new LandAreaItem("AVRAGE INDIRECT", MoyenGross),
                        new LandAreaItem("AVRAGE DIRECT/INDIRECT", (ToGross+ToDirect)/((querydirect.Count()+queryindirect.Count()))),
                    };

                    LandAreasNbDirectAndIndirect = new List<LandAreaItem>()
                    {
                    new LandAreaItem("NB DIRECT", querydirect.Count()),
                    new LandAreaItem("NB INDIRECT", queryindirect.Count()),
                    };

                    if (IdAgent == 0)
                    {
                        IList<LandAreaItem> items = querydirectandindirect.AsEnumerable().Select(row => new LandAreaItem((int)(row.Field<uint?>("agent") is null ? 0 : Convert.ToInt32(row.Field<uint?>("agent"))), row.Field<string>("fullNamAgent"), (int)row.Field<decimal>("TotalHT"))).ToList();
                        var list = items.GroupBy(p => p.Name).Select(g => new LandAreaItem(g.Max(q => q.Id), g.Key, g.Sum(x => x.Number))).ToList();
                        list = list.Where(x => x.Number > 0 && x.Name != "" && x.Id != 0).ToList().OrderByDescending(x => x.Number).ToList();
                        LandAreasTOBbyAgent = new List<LandAreaItem>(list);



                    }
                }

                if (queryunpaid.AsEnumerable().Count() > 0)
                {
                    TotalUnpaid = (int)queryunpaid.AsEnumerable().Select(x => x.Field<decimal>("restAmount")).Sum();
                    try
                    {
                        MaxDueDate = (DateTime?)queryunpaid.AsEnumerable().Min(x => x.Field<DateTime>("due_date"));
                    }
                    catch
                    {
                    }
                }

                if (GeneralQuery.AsEnumerable().Count() != 0)
                {
                    NowVisited = GeneralQuery.AsEnumerable().Count(x => Convert.ToBoolean(x.Field<Object>("validated")) == true && x.Field<string>("type_view") == "1");
                    NowVisit = GeneralQuery.AsEnumerable().Count(x => x.Field<string>("type_view") == "1");
                    NowToVisit = NowVisit - NowVisited;
                    if (NowVisit > 0)
                        NowKPI = ((decimal)NowVisited / (decimal)NowVisit);

                    PeriodVisited = GeneralQuery.AsEnumerable().Count(x => Convert.ToBoolean(x.Field<Object>("validated")) == true && x.Field<string>("type_view") == "2");
                    PeriodVisit = GeneralQuery.AsEnumerable().Count(x => x.Field<string>("type_view") == "2");
                    PeriodToVisit = PeriodVisit - PeriodVisited;
                    if (PeriodVisit > 0)
                        PeriodKPI = ((decimal)PeriodVisited / (decimal)PeriodVisit);

                    CycleVisited = GeneralQuery.AsEnumerable().Count(x => Convert.ToBoolean(x.Field<Object>("validated")) == true && x.Field<string>("type_view") == "3");
                    CycleVisit = GeneralQuery.AsEnumerable().Count(x => x.Field<string>("type_view") == "3");
                    CycleToVisit = CycleVisit - CycleVisited;
                    if (CycleVisit > 0)
                        CycleKPI = ((decimal)CycleVisited / (decimal)CycleVisit);



                    var itemvisit = GeneralQuery.AsEnumerable().Where
                                                                (x => x.Field<string>("type_view") == "2").GroupBy
                                                                (x => x.Field<uint>("employe_id")).Select
                                                                (g => new LandAreaItem((int)g.Key, g.Max(q => q.Field<string>("fullname")), 100 * (g.Sum(x => Convert.ToDouble(x.Field<object>("validated"))) / Convert.ToDouble(g.Count() is 0 ? 1 : g.Count())))).ToList();
                    LandAreasKPIByAgent = new List<LandAreaItem>(itemvisit);
                    if (IdJobPosition == 0 && IdAgent == 0)
                    {

                        /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                        Avant :
                                                var itemvisitByJobPosition = GeneralQuery.AsEnumerable().Where(x => x.Field<string>("type_view") == "2").GroupBy(x => x.Field<uint>("job_poition_id")).Select(g => new LandAreaItem((int)g.Key, g.Max(q => q.Field<string>("job_position_name")), 100*( g.Sum(x => Convert.ToDouble(x.Field<object>("validated"))) / Convert.ToDouble(g.Count() is 0 ? 1 : g.Count())))).ToList();
                        Après :
                                                var itemvisitByJobPosition = GeneralQuery.AsEnumerable().Where(x => x.Field<string>("type_view") == "2").GroupBy(x => x.Field<uint>("job_poition_id")).Select(g => new LandAreaItem((int)g.Key, g.Max(q => q.Field<string>("job_position_name")), 100* (g.Sum(x => Convert.ToDouble(x.Field<object>("validated"))) / Convert.ToDouble(g.Count() is 0 ? 1 : g.Count())))).ToList();
                        */
                        var itemvisitByJobPosition = GeneralQuery.AsEnumerable().Where(x => x.Field<string>("type_view") == "2").GroupBy(x => x.Field<uint>("job_poition_id")).Select(g => new LandAreaItem((int)g.Key, g.Max(q => q.Field<string>("job_position_name")), 100 * (g.Sum(x => Convert.ToDouble(x.Field<object>("validated"))) / Convert.ToDouble(g.Count() is 0 ? 1 : g.Count())))).ToList();
                        LandAreasJobPosition = new List<LandAreaItem>(itemvisitByJobPosition);
                    }





                }




            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



        }



        private IEnumerable<DataRow> GetRowDirect(DataTable result)
        {
            IEnumerable<DataRow> r = from opp in result.AsEnumerable()
                                     where opp.Field<string>("Type") == "VDIRECT"
                                     select opp;

            return r;
        }
        private IEnumerable<DataRow> GetRowInDirect(DataTable result)
        {
            IEnumerable<DataRow> r = from opp in result.AsEnumerable()
                                     where opp.Field<string>("Type") == "VGRO"
                                     select opp;

            return r;
        }
        private IOrderedEnumerable<IGrouping<string, DataRow>> GetRowTotalAmountByAgent(DataTable result)
        {
            var r = from opp in result.AsEnumerable()
                    group opp by opp.Field<string>("agentName") into g
                    orderby g.Key
                    select g;


            return r;
        }

        private IEnumerable<DataRow> GetRowInDirectNow(DataTable result)
        {
            IEnumerable<DataRow> r = from form in result.AsEnumerable()
                                     where form.Field<string>("type_view") == "1"
                                     select form;

            return r;
        }


        //private int GetTotalUnpaid(DataTable unpaidlist)
        //{
        //    int r = from unpaid in unpaidlist.AsEnumerable()
        //                             where unpaid.Field<string>("saleType") == "Grossite"
        //                             select unpaid;

        //    return r;
        //}

        //private List<LandAreaItem> GroupingByAgent( DataTable myTable)
        //{
        //    var myLinqQuery = myTable.AsEnumerable()
        //                     .GroupBy(r1 => r1.Field<int>("ID1"))
        //                     .Select(g =>
        //                     {
        //                         var row = myTable.NewRow();

        //                         row["ID1"] = g.Select(r1 => r1.Field<int>("ID1"));
        //                         row["ID2"] = g.Select(r => r.Field<int>("ID2"));
        //                         row["Value1"] = g.Select(r => r.Field<string>("Value1"));

        //                         return row;
        //                     }).CopyToDataTable();
        //    return myTable.AsEnumerable().ToList();
        //}

        public DataTable GetDataDirectAndIndirect()
        {

            DataTable dt = new DataTable();
            if (DbConnection.Connecter())
            {

                string sqlCmd = "select 'VDIRECT' as Type, sale_shipping.Id as ID,  " +
                "`order` as commande, sum(price*quantity*(1-discount)) as TotalHT, sale_shipping.agent as agent, " +
                "concat(first_name,' ', last_name) as fullNamAgent, code, sale_shipping.`date` as Date, partner as customer,hr_job_position.Id as job_poition_id , hr_job_position.name as job_position_name " +
                "from sale_shipping_line " +
                "left join sale_shipping on sale_shipping.Id = sale_shipping_line.piece " +
                "LEFT JOIN atooerp_person on atooerp_person.Id = sale_shipping.agent left join " +
                "hr_employe on atooerp_person.Id=hr_employe.Id left join " +
                "hr_job_position on hr_employe.job_position = hr_job_position.Id left join " +

                "commercial_partner on commercial_partner.Id = partner  " +
                "where (sale_shipping.`date`>= '" + StartDate.ToString("yyyy-MM-dd ") + "') and (sale_shipping.`date`< DATE_ADD('" + EndDate.ToString("yyyy-MM-dd") + "', INTERVAL 1 DAY)) and (commercial_partner.category not in (2,3,4)) " +
                "group by piece " +
                "UNION SELECT 'VGRO' as Type, c.Id as ID, c.`order` as commande, c.total_amount as TotalHT, c.agent as agent, " +
                "concat(first_name,' ', last_name) as fullNamAgent, code, c.`date` as Date, partner as customer,hr_job_position.Id as job_poition_id , hr_job_position.name as job_position_name " +
                "FROM crm_opportunity_line l " +
                "left join crm_opportunity c on l.piece=c.Id " +
                "left join crm_opportunity_state s on s.Id=c.state " +
                "left join atooerp_person on atooerp_person.Id = c.agent left join " +
                "hr_employe on atooerp_person.Id=hr_employe.Id left join " +
                "hr_job_position on hr_employe.job_position = hr_job_position.Id left join " +
                "commercial_partner on commercial_partner.Id = partner" +

                " where (c.`date`>= '" + StartDate.ToString("yyyy-MM-dd ") + "') and (c.`date`<  DATE_ADD('" + EndDate.ToString("yyyy-MM-dd") + "', INTERVAL 1 DAY)) " +
                "and  (commercial_partner.category not in (2,3,4)) and  (s.state = 2) and (c.dealer > 0) " +
                "group by piece ";

                try
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                }

            }
            DbConnection.Deconnecter();

            return dt;
        }
        public DataTable GetDataUnpaid()
        {

            DataTable dt = new DataTable();
            if (DbConnection.Connecter())
            {

                string sqlCmd = "SELECT sale_balance.piece_type, sale_balance.piece_typeName, sale_balance.Id, sale_balance.code, sale_balance.reference, sale_balance.`Date`, sale_balance.IdPartner, sale_balance.partnerName, sale_balance.payment_conditionName, " +
                "sale_balance.payment_methodName, sale_balance.total_amount, sale_balance.paied_amount, sale_balance.restAmount, sale_balance.due_date, commercial_partner.reference AS PartnerRef, commercial_partner.email AS Email, " +
                "commercial_partner_category.name AS PartnerCategory, sale_balance.pieceAgent, sale_balance.partnerAgent,if(sale_balance.piece_type like '%invoice%', 1, if(sale_balance.piece_type like '%shipping%',2,3)) as rang " +
                "FROM sale_balance LEFT OUTER JOIN " +
                "commercial_partner ON sale_balance.IdPartner = commercial_partner.Id LEFT OUTER JOIN " +
                "commercial_partner_category ON commercial_partner.category = commercial_partner_category.Id " +
                "WHERE(FORMAT(sale_balance.restAmount, 3) <> 0) " +
                "ORDER BY rang, due_date;";

                try
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                DbConnection.Deconnecter();
            }


            return dt;
        }
        public int GetUserCount()
        {
            int count = 0;
            if (DbConnection.Connecter())
            {
                string sqlCmd = "SELECT count(*) FROM atooerp_user LEFT join hr_employe on hr_employe.user= atooerp_user.Id where actif = 1 and hr_employe.job_position = 1 or hr_employe.job_position = 2;";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    cmd.CommandType = CommandType.Text;

                    count = int.Parse(cmd.ExecuteScalar().ToString());

                }
                catch (Exception ex)
                {
                    App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                }
                DbConnection.Deconnecter();
            }
            return count;
        }
        public DataTable GetDataOpp()
        {

            DataTable dt = new DataTable();
            if (DbConnection.Connecter())
            {

                string sqlCmd = "select crm_opportunity.Id , CONCAT(atooerp_person.first_name,' ', " +
                    "atooerp_person.last_name) as agentName,crm_opportunity.code , " +
                    "crm_opportunity.create_date, commercial_partner.name as partnaireName, " +
                    "crm_opportunity.total_amount as total_amount, sale_order.code as ordreCode, " +
                    "sale_order.date as ordreDate, sale_order.delivred , sale_order.delivred_date, " +
                    "wholesaler.name as wholesalerName, parent.code as parentCode, " +
                    "crm_opportunity_state.state as oppState ,hr_job_position.Id as job_poition_id , hr_job_position.name as job_position_name " +
                    "from crm_opportunity " +
                    "left join commercial_partner on commercial_partner.Id = crm_opportunity.partner " +
                    "left join atooerp_person on atooerp_person.Id = crm_opportunity.agent " +
                    "left join hr_employe on atooerp_person.Id=hr_employe.Id  " +
                    "left join hr_job_position on hr_employe.job_position = hr_job_position.Id " +
                    "left join sale_order on sale_order.Id = crm_opportunity.`order` " +
                    "left join commercial_partner wholesaler on wholesaler.Id = crm_opportunity.dealer " +
                    "left join crm_opportunity parent on parent.Id = crm_opportunity.parent " +
                    "left join crm_opportunity_state on crm_opportunity.state = crm_opportunity_state.Id " +
                    "left join crm_state on crm_state.Id = crm_opportunity_state.state " +
                    "where crm_opportunity.date >= '" + StartDate.ToString("yyyy-MM-dd ") + "' and crm_opportunity.date < DATE_ADD(  '" + EndDate.ToString("yyyy-MM-dd") + "', INTERVAL 1 DAY) " +
                    "order by crm_opportunity.Id desc";
                try
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                }

            }

            DbConnection.Deconnecter();
            return dt;
        }

        public DataTable GetDataDirectAndIndirectByAgent()
        {

            DataTable dt = new DataTable();
            if (DbConnection.Connecter())
            {

                string sqlCmd = "select 'VDIRECT' as Type, sale_shipping.Id as ID,  " +
                   "`order` as commande, sum(price*quantity*(1-discount)) as TotalHT, sale_shipping.agent as agent, " +
                   "concat(first_name,' ', last_name) as fullNamAgent, code, sale_shipping.`date` as Date, partner as customer " +
                   "from sale_shipping_line " +
                   "left join sale_shipping on sale_shipping.Id = sale_shipping_line.piece " +
                   "LEFT JOIN atooerp_person on atooerp_person.Id = sale_shipping.agent " +
                   "left join commercial_partner on commercial_partner.Id = partner  " +
                   "where (sale_shipping.`date`>= '" + StartDate.ToString("yyyy-MM-dd ") + "') and (sale_shipping.`date`< DATE_ADD('" + EndDate.ToString("yyyy-MM-dd") + "', INTERVAL 1 DAY)) and (commercial_partner.category not in (2,3,4)) and (agent = " + IdAgent + ") " +
                   "group by piece " +
                   "UNION SELECT 'VGRO' as Type, c.Id as ID, c.`order` as commande, c.total_amount as TotalHT, c.agent as agent, " +
                   "concat(first_name,' ', last_name) as fullNamAgent, code, c.`date` as Date, partner as customer " +
                   "FROM crm_opportunity_line l " +
                   "left join crm_opportunity c on l.piece=c.Id " +
                   "left join crm_opportunity_state s on s.Id=c.state " +
                   "left join atooerp_person on atooerp_person.Id = c.agent " +
                   "left join commercial_partner on commercial_partner.Id = partner " +
                   "where (c.`date`>= '" + StartDate.ToString("yyyy-MM-dd ") + "') and (c.`date`<  DATE_ADD('" + EndDate.ToString("yyyy-MM-dd") + "', INTERVAL 1 DAY)) and (agent = " + IdAgent + ") " +
                   "and commercial_partner.category not in (2,3,4) and  s.state = 2 and c.dealer > 0 " +
                   "group by piece ";

                try
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                }

            }

            DbConnection.Deconnecter();
            return dt;
        }
        public DataTable GetDataUnpaidByAgent()
        {

            DataTable dt = new DataTable();
            if (DbConnection.Connecter())
            {

                string sqlCmd = "SELECT sale_balance.piece_type, sale_balance.piece_typeName, sale_balance.Id, sale_balance.code, sale_balance.reference, sale_balance.`Date`, sale_balance.IdPartner, sale_balance.partnerName, sale_balance.payment_conditionName, " +
                "sale_balance.payment_methodName, sale_balance.total_amount, sale_balance.paied_amount, sale_balance.restAmount, sale_balance.due_date, commercial_partner.reference AS PartnerRef, commercial_partner.email AS Email, " +
                "commercial_partner_category.name AS PartnerCategory, sale_balance.pieceAgent, sale_balance.partnerAgent,if(sale_balance.piece_type like '%invoice%', 1, if(sale_balance.piece_type like '%shipping%',2,3)) as rang " +
                "FROM sale_balance LEFT OUTER JOIN " +
                "commercial_partner ON sale_balance.IdPartner = commercial_partner.Id LEFT OUTER JOIN " +
                "commercial_partner_category ON commercial_partner.category = commercial_partner_category.Id " +
                "WHERE(FORMAT(sale_balance.restAmount, 3) <> 0) and (commercial_partner.sale_agent=" + IdAgent + ") " +
                "ORDER BY rang, due_date;";

                try
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                }

            }

            DbConnection.Deconnecter();
            return dt;
        }
        public DataTable GetDataOppByAgent()
        {

            DataTable dt = new DataTable();
            if (DbConnection.Connecter())
            {

                string sqlCmd = "select crm_opportunity.Id , CONCAT(atooerp_person.first_name,' ', " +
                    "atooerp_person.last_name) as agentName,crm_opportunity.code , " +
                    "crm_opportunity.create_date, commercial_partner.name as partnaireName, " +
                    "crm_opportunity.total_amount as total_amount, sale_order.code as ordreCode, " +
                    "sale_order.date as ordreDate, sale_order.delivred , sale_order.delivred_date, " +
                    "wholesaler.name as wholesalerName, parent.code as parentCode, " +
                    "crm_opportunity_state.state as oppState " +
                    "from crm_opportunity " +
                    "left join commercial_partner on commercial_partner.Id = crm_opportunity.partner " +
                    "left join atooerp_person on atooerp_person.Id = crm_opportunity.agent " +
                    "left join sale_order on sale_order.Id = crm_opportunity.`order` " +
                    "left join commercial_partner wholesaler on wholesaler.Id = crm_opportunity.dealer " +
                    "left join crm_opportunity parent on parent.Id = crm_opportunity.parent " +
                    "left join crm_opportunity_state on crm_opportunity.state = crm_opportunity_state.Id " +
                    "left join crm_state on crm_state.Id = crm_opportunity_state.state " +
                    "where crm_opportunity.date >= '" + startdate.ToString("yyyy-MM-dd") + "' and crm_opportunity.date < DATE_ADD(  '" + enddate.ToString("yyyy-MM-dd") + "', INTERVAL 1 DAY) and (crm_opportunity.agent = " + IdAgent + ") " +
                    "order by crm_opportunity.Id desc";
                try
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                }

            }

            DbConnection.Deconnecter();
            return dt;
        }

        public DataTable GetGeneralPartnerFormInformation()
        {
            DataTable dt = new DataTable();
            if (DbConnection.Connecter())
            {
                string sqlCmd = "select  marketing_quiz_partner_form.Id , '1' as type_view, marketing_quiz_partner_form.user_id , marketing_quiz_partner_form.employe_id, marketing_quiz_partner_form.begin_date as partner_form_begin_date " +
                ", marketing_quiz_partner_form.estimated_date, marketing_quiz_partner_form.end_date as partner_form_end_date , marketing_quiz_partner_form.validated, marketing_quiz_partner_form.cycle_id , " +
                "cycle.name as cycle_name , cycle.begin_date as cycle_begin_date , cycle.end_date as cycle_end_date , " +
                "concat(atooerp_person.first_name, ' ', atooerp_person.last_name) as fullname , hr_job_position.Id as job_poition_id , hr_job_position.name as job_position_name from marketing_quiz_partner_form " +
                "left join hr_employe on marketing_quiz_partner_form.employe_id = hr_employe.Id left join " +
                "hr_job_position on hr_employe.job_position = hr_job_position.Id left join " +
                "atooerp_person on hr_employe.Id = atooerp_person.Id left join " +
                "marketing_quiz_cycle cycle on marketing_quiz_partner_form.cycle_id = cycle.Id left join " +
                "atooerp_user on marketing_quiz_partner_form.user_id = atooerp_user.Id " +
                "where '" + DateNow.ToString("yyyy-MM-dd") + " 00:00:00' <= marketing_quiz_partner_form.estimated_date and '" + DateNow.AddDays(1).ToString("yyyy-MM-dd") + " 00:00:00' >= marketing_quiz_partner_form.estimated_date " +
                "union " +
                "select marketing_quiz_partner_form.Id, '2' as type_view, marketing_quiz_partner_form.user_id , marketing_quiz_partner_form.employe_id, marketing_quiz_partner_form.begin_date as partner_form_begin_date," +
                " marketing_quiz_partner_form.end_date as partner_form_end_date ,marketing_quiz_partner_form.estimated_date, marketing_quiz_partner_form.validated, marketing_quiz_partner_form.cycle_id , " +
                "cycle.name as cycle_name , cycle.begin_date as cycle_begin_date , cycle.end_date as cycle_end_date , concat(atooerp_person.first_name, ' ', atooerp_person.last_name) as fullname , " +
                "hr_job_position.Id as job_poition_id , hr_job_position.name as job_position_name from marketing_quiz_partner_form left join " +
                "hr_employe on marketing_quiz_partner_form.employe_id = hr_employe.Id left join " +
                "hr_job_position on hr_employe.job_position = hr_job_position.Id left join " +
                "atooerp_person on hr_employe.Id = atooerp_person.Id left join " +
                "marketing_quiz_cycle cycle on marketing_quiz_partner_form.cycle_id = cycle.Id left join " +
                "atooerp_user on marketing_quiz_partner_form.user_id = atooerp_user.Id " +
                "where '" + StartDate.ToString("yyyy-MM-dd") + " 00:00:00" + "' <= marketing_quiz_partner_form.estimated_date  and '" + EndDate.ToString("yyyy-MM-dd") + " 00:00:00" + "' >= marketing_quiz_partner_form.estimated_date " +
                "union " +
                "select marketing_quiz_partner_form.Id, '3' as type_view, marketing_quiz_partner_form.user_id , marketing_quiz_partner_form.employe_id, marketing_quiz_partner_form.begin_date as partner_form_begin_date, " +
                "marketing_quiz_partner_form.end_date as partner_form_end_date ,marketing_quiz_partner_form.estimated_date, marketing_quiz_partner_form.validated, marketing_quiz_partner_form.cycle_id , cycle.name as cycle_name , " +
                "cycle.begin_date as cycle_begin_date , cycle.end_date as cycle_end_date , concat(atooerp_person.first_name, ' ', atooerp_person.last_name) as fullname , " +
                "hr_job_position.Id as job_poition_id , hr_job_position.name as job_position_name from marketing_quiz_partner_form left join " +
                "hr_employe on marketing_quiz_partner_form.employe_id = hr_employe.Id left join " +
                "hr_job_position on hr_employe.job_position = hr_job_position.Id left join " +
                "atooerp_person on hr_employe.Id = atooerp_person.Id left join " +
                "marketing_quiz_cycle cycle on marketing_quiz_partner_form.cycle_id = cycle.Id left join " +
                "atooerp_user on marketing_quiz_partner_form.user_id = atooerp_user.Id " +
                "where now() between cycle.begin_date and cycle.end_date; ";
                try
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.Fill(dt);
                    int i = dt.Rows.Count;

                }
                catch (Exception ex)
                {
                    App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                }

            }
            DbConnection.Deconnecter();
            return dt;
        }



    }

    public class LandAreaItem
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public double Number { get; set; } = 0;

        public LandAreaItem(int id, string name, double number)
        {
            Id = id;
            Name = name;
            Number = number;
        }

        public LandAreaItem(string name, double number)
        {
            this.Name = name;
            this.Number = number;
        }
    }
}
