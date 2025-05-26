using SmartPharma5.Model;
using MvvmHelpers;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static SQLite.SQLite3;
using System.Threading.Tasks;
using MvvmHelpers.Commands;
//using Xamarin.Forms;
using SmartPharma5.View;
//using Xamarin.Essentials;

namespace SmartPharma5.ViewModel
{
    internal class Cash_deskLogViewModel: BaseViewModel
    {
        public bool actpopup = true;
        public bool ActPopup { get => actpopup; set => SetProperty(ref actpopup, value); }
        private bool successpopup = false;
        public bool SuccessPopup { get => successpopup; set => SetProperty(ref successpopup, value); }
        private string successpopupmessage;
        public string SuccessPopupMessage { get => successpopupmessage; set => SetProperty(ref successpopupmessage, value); }
        private DataTable logCash_deskList;
        public DataTable LogCash_deskList { get => logCash_deskList; set => SetProperty(ref logCash_deskList, value); }
        private DateTime startdate = DateTime.Now - new TimeSpan(30, 0, 0, 0);
        public DateTime StartDate { get => startdate; set => SetProperty(ref startdate, value); }
        private DateTime enddate = DateTime.Now;
        public DateTime EndDate { get => enddate; set => SetProperty(ref enddate, value); }
        public MvvmHelpers.Commands.Command RefreshCommand { get; set; }
        public AsyncCommand ExitCommand { get; set; }
        public AsyncCommand LogoutCommand { get; set; }
        private CashDesk cash_desk;
        public CashDesk CashDesk { get => cash_desk; set => SetProperty(ref cash_desk, value); }


        private bool testCon = false;
        public bool TestCon { get => testCon; set => SetProperty(ref testCon, value); }
        public Cash_deskLogViewModel() 
        {
        }
        public Cash_deskLogViewModel(CashDesk cd)
        {
            CashDesk = cd;
            Title = "Log[ " + CashDesk.Name + " ]";
            LogCash_deskList = new DataTable();
            LogCash_deskList = GetLogByCash_deskAndDates();
            RefreshCommand = new MvvmHelpers.Commands.Command(Refresh);
            ExitCommand = new AsyncCommand(Exit);
            LogoutCommand = new AsyncCommand(Logout);
        }
        public async void Refresh()
        {
            SuccessPopupMessage = "Refresh Data...";
            SuccessPopup = true;
            await Task.Delay(1000);
            LogCash_deskList.Clear();
            ActPopup = true;
            await Task.Delay(1000);
            LogCash_deskList = GetLogByCash_deskAndDates();
            SuccessPopup = false;
        }
        public DataTable GetLogByCash_deskAndDates()
        {
            DataTable dt = new DataTable();
            string sqlCmd = "SELECT accounting_cash_register.Id, accounting_cash_register.`date`, accounting_cash_register_motif.name AS Motif," +
                " accounting_cash_register.register, CONCAT(atooerp_person.first_name, ' ', atooerp_person.last_name) AS Agent," +
                " accounting_cash_register.amount AS Montant, accounting_cash_register.solde, accounting_cash_register.cash_desk," +
                " accounting_cash_register.payment, accounting_cash_register.payment_transfert, accounting_cash_register.pos_session_inventory," +
                " accounting_cash_register.bank_deposit, accounting_cash_register.bank_deposit_line, accounting_cash_register.cash_in_out " +
                "FROM accounting_cash_register " +
                "LEFT OUTER JOIN accounting_cash_register_motif ON accounting_cash_register.motif = accounting_cash_register_motif.Id " +
                "LEFT OUTER JOIN hr_employe ON accounting_cash_register.agent = hr_employe.Id " +
                "LEFT OUTER JOIN atooerp_person ON atooerp_person.Id = hr_employe.Id " +
                "WHERE  (accounting_cash_register.`date` >= '"+ StartDate.ToString("yyyy-MM-dd") + "' ) AND (accounting_cash_register.`date` <= '"+ EndDate.ToString("yyyy-MM-dd") +" 23:59:59') AND (accounting_cash_register.cash_desk = " + cash_desk.Id + ")";

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            adapter.Fill(dt);
            DbConnection.Deconnecter();
            return dt;
        }
        private async Task Logout()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to lougout!", "Yes", "No");
            if (r)
            {
                Preferences.Set("idagent", Convert.ToUInt32(null));
                await App.Current.MainPage.Navigation.PushAsync(new LoginView());
            }
        }

        private async Task Exit()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to exit!", "Yes", "No");
            if (r)
            {
                await App.Current.MainPage.Navigation.PopToRootAsync();
            }
        }
    }
}
