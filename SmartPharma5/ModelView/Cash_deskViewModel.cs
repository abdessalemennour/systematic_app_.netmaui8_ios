using SmartPharma5.Model;
using MvvmHelpers;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartPharma5.ViewModel
{
    internal class Cash_deskViewModel : BaseViewModel
    {
        public DataTable StateCash_deskList { get; set; }

        private bool testCon = false;
        public bool TestCon { get => testCon; set => SetProperty(ref testCon, value); }
        public Cash_deskViewModel()
        {
            StateCash_deskList = new DataTable();
            StateCash_deskList = GetStateByCash_desk(3);
        }
        public Cash_deskViewModel(CashDesk cd)
        {
            Title = "State[ " + cd.Name + " ]";
            StateCash_deskList = new DataTable();
            StateCash_deskList = GetStateByCash_desk(cd.Id);
        }
        public DataTable GetStateByCash_desk(int idCash_desk)
        {
            DataTable dt = new DataTable();
            string sqlCmd = "SELECT commercial_payment.Id, commercial_payment.code, commercial_payment.`date`, commercial_partner.name AS Partner, commercial_payment_method.name AS MethodeDePayement, commercial_payment.reference,  commercial_payment.amount AS Montant, commercial_payment.due_date AS DateEcheance, commercial_payment.ended AS SOLDE, commercial_payment.validated AS VALIDE, accounting_bank.name AS Bank FROM     commercial_payment LEFT OUTER JOIN  accounting_bank ON commercial_payment.sale_bank = accounting_bank.Id LEFT OUTER JOIN  commercial_payment_type ON commercial_payment.payment_type = commercial_payment_type.Id LEFT OUTER JOIN  commercial_partner ON commercial_payment.partner = commercial_partner.Id LEFT OUTER JOIN  commercial_payment_method ON commercial_payment.payment_method = commercial_payment_method.Id WHERE  (commercial_payment.cash_desk = "+idCash_desk+")";

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            adapter.Fill(dt);
            DbConnection.Deconnecter();
            return dt;
        }
    }
}
