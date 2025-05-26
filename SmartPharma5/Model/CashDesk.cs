//using DevExpress.XamarinForms.Core.Themes;
using MvvmHelpers;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Data;
//using Xamarin.Essentials;
//using Xamarin.Forms.PlatformConfiguration;

namespace SmartPharma5.Model
{
    public class CashDesk : ObservableObject
    {
        #region attributs
        public int Id { get; set; }
        public DateTime Create_date { get; set; }
        public string Name { get; set; }
        public bool Principal { get; set; }
        public bool Hr { get; set; }
        public bool Sale { get; set; }
        public bool Purchase { get; set; }
        public bool Bank { get; set; }
        public bool Cash { get; set; }
        public bool Pos { get; set; }
        public decimal Amount { get; set; }
        public bool Activated { get; set; }
        public int Payment_method { get; set; }
        public string Payment_method_name { get; set; }
        public string Memo { get; set; }
        public int IdUser { get; set; }
        public int IdAgent { get; set; }
        public string NameAgent { get; set; }
        public decimal Amount_By_Agent { get; set; }
        public String Header_agent { get; set; }
        public decimal Amount_By_Method { get; set; }
        public String Header_method { get; set; }
        public bool isTransfer { get; set; }
        public bool isTransfer_state { get; set; }
        public bool isNotCash { get; set; }
        #endregion
        #region constructers
        public CashDesk() { }
        public CashDesk(int Id)
        {
            DbConnection.ConnectionIsTrue();
            string sqlCmd = "SELECT * FROM accounting_cash_desk where Id=" + Id + ";";

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            try
            {
                this.Id = Id;
                this.Amount = Convert.ToDecimal(dt.Rows[0]["amount"]);
                this.Create_date = Convert.ToDateTime(dt.Rows[0]["create_date"]);
                this.Name = (dt.Rows[0]["name"]).ToString();
                this.Principal = Convert.ToBoolean(dt.Rows[0]["principal"]);
                this.Sale = Convert.ToBoolean(dt.Rows[0]["sale"]);
                this.Purchase = Convert.ToBoolean(dt.Rows[0]["purchase"]);
                this.Hr = Convert.ToBoolean(dt.Rows[0]["hr"]);
                this.Bank = Convert.ToBoolean(dt.Rows[0]["bank"]);
                this.Cash = Convert.ToBoolean(dt.Rows[0]["cash"]);
                this.Pos = Convert.ToBoolean(dt.Rows[0]["pos"]);
                this.Activated = Convert.ToBoolean(dt.Rows[0]["activated"]);
                this.Payment_method = Convert.ToInt32(dt.Rows[0]["payment_method"]);
                this.Memo = dt.Rows[0]["memo"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();
        }
        public CashDesk(int idcashdesk, decimal amount, string paymentname, int payment_method, string name, bool isTransfer, bool cash, bool isTransfer_state)
        {
            Id = idcashdesk;
            Amount = amount;
            Payment_method_name = paymentname;
            Name = name;
            this.Payment_method = payment_method;
            this.isTransfer = isTransfer;
            Cash = cash;
            isNotCash = !cash;
            this.isTransfer_state = isTransfer_state;
        }

        public CashDesk(int id, int payment_method, string name, bool principal)
        {
            this.Id = id;
            this.Payment_method = payment_method;
            this.Name = name;
            this.Principal = principal;
        }
        #endregion
        #region methods
        public static async Task<CashDesk> GetCash_deskById(int Id)
        {
            CashDesk cd = new CashDesk();
            DbConnection.ConnectionIsTrue();
            string sqlCmd = "SELECT * FROM accounting_cash_desk where Id=" + Id + ";";

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            try
            {
                cd.Id = Id;
                cd.Amount = Convert.ToDecimal(dt.Rows[0]["amount"]);
                cd.Create_date = Convert.ToDateTime(dt.Rows[0]["create_date"]);
                cd.Name = (dt.Rows[0]["name"]).ToString();
                cd.Principal = Convert.ToBoolean(dt.Rows[0]["principal"]);
                cd.Sale = Convert.ToBoolean(dt.Rows[0]["sale"]);
                cd.Purchase = Convert.ToBoolean(dt.Rows[0]["purchase"]);
                cd.Hr = Convert.ToBoolean(dt.Rows[0]["hr"]);
                cd.Bank = Convert.ToBoolean(dt.Rows[0]["bank"]);
                cd.Cash = Convert.ToBoolean(dt.Rows[0]["cash"]);
                cd.Pos = Convert.ToBoolean(dt.Rows[0]["pos"]);
                cd.Activated = Convert.ToBoolean(dt.Rows[0]["activated"]);
                cd.Payment_method = Convert.ToInt32(dt.Rows[0]["payment_method"]);
                cd.Memo = dt.Rows[0]["memo"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();
            return cd;
        }
        public async static Task<ObservableCollection<CashDesk>> GetMyCash_desk()
        {

            ObservableCollection<CashDesk> list = new ObservableCollection<CashDesk>();
            string sqlCmd = "SELECT distinct accounting_cash_desk.Id, accounting_cash_desk.name,accounting_cash_desk.payment_method,  " +
                "accounting_cash_desk.amount, commercial_payment_method.name AS Payment_method_name,  " +
                "accounting_cash_desk.principal AS Principale,  accounting_cash_desk.sale AS Vente,  " +
                "accounting_cash_desk.purchase AS Achat, accounting_cash_desk.hr AS Hr, accounting_cash_desk.bank AS Bank,  " +
                "accounting_cash_desk.cash AS Cash, accounting_cash_desk.pos AS POS,  accounting_cash_desk.activated AS Active,  " +
                "atooerp_user.login AS Admin, accounting_cash_desk.payment_method AS idpayment_method, " +
                "accounting_user_cash_desk_type.transfer as transfer,accounting_user_cash_desk_type.transfer_state as transfer_state  " +
                "FROM accounting_cash_desk INNER JOIN   " +
                "accounting_user_cash_desk ON accounting_cash_desk.Id = accounting_user_cash_desk.cash_desk LEFT OUTER JOIN  " +
                "accounting_user_cash_desk_type on accounting_user_cash_desk.type=accounting_user_cash_desk_type.Id LEFT OUTER JOIN  " +
                "atooerp_user ON accounting_user_cash_desk.`user` = atooerp_user.Id  LEFT OUTER JOIN   " +
                "commercial_payment_method ON accounting_cash_desk.payment_method = commercial_payment_method.Id  " +
                "WHERE  (accounting_user_cash_desk.`user` = " + Preferences.Get("iduser", 0) + ")  " +
                "AND (accounting_cash_desk.activated = 1)  " +
                "GROUP BY accounting_cash_desk.Id, accounting_cash_desk.payment_method  " +
                "ORDER BY accounting_cash_desk.Id";

            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new CashDesk(Convert.ToInt32(reader["Id"]), Convert.ToDecimal(reader["amount"]), reader["Payment_method_name"].ToString(), Convert.ToInt32(reader["payment_method"]), reader["name"].ToString(), Convert.ToBoolean(reader["transfer"]), Convert.ToBoolean(reader["Cash"]), Convert.ToBoolean(reader["transfer_state"])));
                    }
                    reader.Close();

                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;

                }

            }
            else
            {
                return null;
            }


            return list;
        }
        public async static Task<ObservableCollection<CashDesk>> GetAllCash_desk()
        {

            ObservableCollection<CashDesk> list = new ObservableCollection<CashDesk>();
            string sqlCmd = "SELECT distinct accounting_cash_desk.Id, accounting_cash_desk.name,accounting_cash_desk.payment_method,  " +
                "accounting_cash_desk.amount, commercial_payment_method.name AS Payment_method_name,  " +
                "accounting_cash_desk.principal AS Principale,  accounting_cash_desk.sale AS Vente,  " +
                "accounting_cash_desk.purchase AS Achat, accounting_cash_desk.hr AS Hr, accounting_cash_desk.bank AS Bank,  " +
                "accounting_cash_desk.cash AS Cash, accounting_cash_desk.pos AS POS,  accounting_cash_desk.activated AS Active,  " +
                "accounting_cash_desk.payment_method AS idpayment_method  " +
                "FROM accounting_cash_desk Left OUTER JOIN  " +
                "commercial_payment_method ON accounting_cash_desk.payment_method = commercial_payment_method.Id  " +
                "WHERE accounting_cash_desk.activated = 1  " +
                "GROUP BY accounting_cash_desk.Id, accounting_cash_desk.payment_method  " +
                "ORDER BY accounting_cash_desk.Id";
            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {
                try
                {

                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new CashDesk(Convert.ToInt32(reader["Id"]), Convert.ToDecimal(reader["amount"]), reader["Payment_method_name"].ToString(), Convert.ToInt32(reader["payment_method"]), reader["name"].ToString(), true, Convert.ToBoolean(reader["Cash"]), true));
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;

                }
            }
            else
            {
                return null;
            }
            return list;
        }
        #endregion
    }
}
