using MvvmHelpers;
using MySqlConnector;
using System.ComponentModel;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System.Text;
using static SmartPharma5.Model.Payment;
using System.Threading.Tasks;
using static SmartPharma5.Model.Transfer;
using System.Data.SqlTypes;
using System.Data;
using System.Drawing;
//using Xamarin.Essentials;
Après :
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
//using Xamarin.Essentials;
//using SkiaSharp;
using Xamarin.Essentials;
*/
using System.Data;
//using static SmartPharma5.Model.Opportunity;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using static SmartPharma5.Model.Opportunity;
Après :
using static SmartPharma5.Model.Transfer;
*/

//using Xamarin.Forms.Shapes;

namespace SmartPharma5.Model
{
    public class Transfer : ObservableObject
    {
        #region Attribut
        public int Id { get; set; }
        private decimal Amount;
        public decimal amount { get => Amount; set => SetProperty(ref Amount, value); }
        public DateTime create_date { get; set; }
        public int cash_desk_in { get; set; }
        public int cash_desk_out { get; set; }
        private string Memo;
        public string memo { get => Memo; set => SetProperty(ref Memo, value); }
        public bool validated { get; set; }
        public int? state { get; set; }
        public string last_state { get; set; }
        public Color stateColor
        {
            get
            {
                if (last_state != null)
                {
                    if (last_state.Contains("Accepté"))
                        return Colors.GreenYellow;
                    else if (last_state.Contains("Refusé"))
                        return Colors.Red;
                    else
                        return Colors.Gray;
                }
                else
                    return Colors.Gray;

            }
        }
        public BindingList<Payment_transfer> PaymentList
        {
            get;
            set;
        }
        #endregion
        #region Constructeur
        public Transfer()
        {
            create_date = DateTime.Now;
            PaymentList = new BindingList<Payment_transfer>();
        }
        public Transfer(int Id)
        {
            DbConnection.ConnectionIsTrue();
            string sqlCmd = "SELECT accounting_transfer.*,accounting_transfer_state.name as last_state,accounting_transfer_state_log.create_date as date_state  " +
                "FROM accounting_transfer left join  " +
                "accounting_transfer_state_log on accounting_transfer_state_log.Id = accounting_transfer.state left join  " +
                "accounting_transfer_state on accounting_transfer_state_log.state = accounting_transfer_state.Id  " +
                "where accounting_transfer.Id = " + Id + "";

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            try
            {
                this.Id = Id;
                this.amount = Convert.ToDecimal(dt.Rows[0]["amount"]);
                this.create_date = Convert.ToDateTime(dt.Rows[0]["create_date"]);
                this.cash_desk_in = Convert.ToInt32(dt.Rows[0]["cash_desk_in"]);
                this.cash_desk_out = Convert.ToInt32(dt.Rows[0]["cash_desk_out"]);
                this.memo = dt.Rows[0]["memo"].ToString();
                this.validated = Convert.ToBoolean(dt.Rows[0]["validated"]);
                this.state = Convert.ToInt32(dt.Rows[0]["state"]);
                this.last_state = dt.Rows[0]["last_state"].ToString() + " : " + Convert.ToDateTime(dt.Rows[0]["date_state"]).ToString("dd/MM/yyyy  h:mm tt");
                this.PaymentList = new BindingList<Payment_transfer>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();
            GetPaymentTransferByTransfer();
        }
        #endregion
        #region Functions
        public void Insert()
        {
            string _Amount = this.amount.ToString().Replace(',', '.');
            this.validated = true;
            string sqlCmd = "INSERT INTO accounting_transfer SET amount =" + _Amount + ",create_date= NOW(),cash_desk_in=" + cash_desk_in + ",cash_desk_out=" + cash_desk_out + ",memo='" + memo + "',validated=" + validated + ",state=null;SELECT MAX(Id) FROM " + DbConnection.Database + ".accounting_transfer;";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {

                this.Id = int.Parse(cmd.ExecuteScalar().ToString());
                SetState(1);
                foreach (Payment_transfer payment in PaymentList)
                {
                    payment.transfer = Id;
                    payment.Insert();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();
        }
        public void SetTransferAmount()
        {
            amount = 0;
            foreach (Payment_transfer payment in PaymentList)
                amount += payment.amount;
        }
        public void SetState(int state)
        {
            State State = new State(this.Id, state);
            State.Insert();
            this.state = State.Id;
        }
        public static BindingList<Model.CashDesk> getCash_deskList()
        {
            string sqlCmd = "SELECT accounting_cash_desk.Id, accounting_cash_desk.name,accounting_cash_desk.payment_method, " +
                " accounting_cash_desk.amount AS Montant, commercial_payment_method.name AS MethodePaiement, " +
                " accounting_cash_desk.principal, accounting_cash_desk.sale AS Vente,  " +
                "accounting_cash_desk.purchase AS Achat, accounting_cash_desk.hr AS Hr, accounting_cash_desk.bank AS Bank,  " +
                "accounting_cash_desk.cash AS Cash, accounting_cash_desk.pos AS POS, accounting_cash_desk.activated AS Active,  " +
                "atooerp_user.login AS Admin  " +
                "FROM accounting_cash_desk LEFT OUTER JOIN  " +
                "accounting_user_cash_desk ON accounting_cash_desk.Id = accounting_user_cash_desk.cash_desk LEFT OUTER JOIN  " +
                "accounting_user_cash_desk_type ON accounting_user_cash_desk_type.Id = accounting_user_cash_desk.`type` LEFT OUTER JOIN  " +
                "atooerp_user ON accounting_user_cash_desk.`user` = atooerp_user.Id LEFT OUTER JOIN  " +
                "commercial_payment_method ON accounting_cash_desk.payment_method = commercial_payment_method.Id  " +
                "WHERE  (accounting_cash_desk.activated = 1)  " +
                "GROUP BY accounting_cash_desk.Id ORDER BY accounting_cash_desk.Id;";
            BindingList<CashDesk> Cash_desklist = new BindingList<CashDesk>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    Cash_desklist.Add(new CashDesk(
                        Convert.ToInt32(reader["Id"]),
                        Convert.ToInt32(reader["payment_method"]),
                        reader["name"].ToString(),
                        Convert.ToBoolean(reader["principal"])));
                }
                catch (Exception ex)
                {
                    App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                    App.Current.MainPage.Navigation.PopAsync();
                }

            }
            reader.Close();
            DbConnection.Deconnecter();
            return Cash_desklist;
        }
        public static BindingList<Model.CashDesk> getCash_deskListByPayment_method(CashDesk CashDesk)
        {
            string sqlCmd = "SELECT accounting_cash_desk.Id, accounting_cash_desk.name,accounting_cash_desk.payment_method, " +
                " accounting_cash_desk.amount AS Montant, commercial_payment_method.name AS MethodePaiement, " +
                " accounting_cash_desk.principal, accounting_cash_desk.sale AS Vente,  " +
                "accounting_cash_desk.purchase AS Achat, accounting_cash_desk.hr AS Hr, accounting_cash_desk.bank AS Bank,  " +
                "accounting_cash_desk.cash AS Cash, accounting_cash_desk.pos AS POS, accounting_cash_desk.activated AS Active,  " +
                "atooerp_user.login AS Admin  " +
                "FROM accounting_cash_desk LEFT OUTER JOIN  " +
                "accounting_user_cash_desk ON accounting_cash_desk.Id = accounting_user_cash_desk.cash_desk LEFT OUTER JOIN  " +
                "accounting_user_cash_desk_type ON accounting_user_cash_desk_type.Id = accounting_user_cash_desk.`type` LEFT OUTER JOIN  " +
                "atooerp_user ON accounting_user_cash_desk.`user` = atooerp_user.Id LEFT OUTER JOIN  " +
                "commercial_payment_method ON accounting_cash_desk.payment_method = commercial_payment_method.Id  " +
                "WHERE  (accounting_cash_desk.activated = 1)  " +
                "AND (accounting_cash_desk.payment_method = " + CashDesk.Payment_method + ") AND (accounting_cash_desk.cash = " + CashDesk.Cash + ") AND (accounting_cash_desk.Id <> " + CashDesk.Id + ")   " +
                "GROUP BY accounting_cash_desk.Id ORDER BY accounting_cash_desk.Id;";
            BindingList<CashDesk> Cash_desklist = new BindingList<CashDesk>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    Cash_desklist.Add(new CashDesk(
                        Convert.ToInt32(reader["Id"]),
                        Convert.ToInt32(reader["payment_method"]),
                        reader["name"].ToString(),
                        Convert.ToBoolean(reader["principal"])));
                }
                catch (Exception ex)
                {
                    App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                    App.Current.MainPage.Navigation.PopAsync();
                }

            }
            reader.Close();
            DbConnection.Deconnecter();
            return Cash_desklist;
        }
        public static BindingList<Payment> GetPaymentByCash_desk(int Cash_desk)
        {
            string sqlCmd = "SELECT commercial_payment.Id, commercial_payment.code, commercial_payment.`date`, commercial_partner.name AS Partner, commercial_payment_method.name AS MethodeDePayement, commercial_payment.reference, commercial_payment.amount AS Montant, commercial_payment.due_date AS DateEcheance, commercial_payment.ended AS SOLDE, commercial_payment.validated AS VALIDE FROM   commercial_payment LEFT OUTER JOIN commercial_payment_type ON commercial_payment.payment_type = commercial_payment_type.Id LEFT OUTER JOIN commercial_partner ON commercial_payment.partner = commercial_partner.Id LEFT OUTER JOIN commercial_payment_method ON commercial_payment.payment_method = commercial_payment_method.Id WHERE (commercial_payment.cash_desk = " + Cash_desk + ");";
            BindingList<Payment> list = new BindingList<Payment>();

            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    list.Add(new Payment(
                        Convert.ToInt32(reader["Id"]),
                        reader["code"].ToString(),
                        Convert.ToDateTime(reader["date"]),
                        reader["Partner"].ToString(),
                        reader["MethodeDePayement"].ToString(),
                        reader["reference"].ToString(),
                        Convert.ToDecimal(reader["Montant"]),
                        reader["DateEcheance"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["DateEcheance"]) : null,
                        Convert.ToBoolean(reader["SOLDE"]),
                        Convert.ToBoolean(reader["VALIDE"]), false));
                }
                catch (Exception ex)
                {
                    App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                    App.Current.MainPage.Navigation.PopAsync();
                }

            }
            reader.Close();
            DbConnection.Deconnecter();


            return list;
        }
        public bool IsUserStateEnabled(int iduser)
        {
            bool res = false;
            if (DbConnection.Connecter())
            {
                string sqlCmd = "SELECT accounting_user_cash_desk_type.transfer_state FROM accounting_user_cash_desk left join accounting_user_cash_desk_type on accounting_user_cash_desk.`type`= accounting_user_cash_desk_type.Id where accounting_user_cash_desk.cash_desk =" + this.cash_desk_in + " and accounting_user_cash_desk.`user`=" + iduser + "; ";
                try
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                        res = Convert.ToBoolean(dt.Rows[0]["transfer_state"]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            DbConnection.Deconnecter();
            return res;
        }
        public int GetState()
        {
            int res = 0;
            if (DbConnection.Connecter())
            {
                string sqlCmd = "SELECT accounting_transfer_state.Id FROM accounting_transfer left join accounting_transfer_state_log on accounting_transfer_state_log.Id =accounting_transfer.state left join accounting_transfer_state on accounting_transfer_state_log.state=accounting_transfer_state.Id Where accounting_transfer.Id=" + this.Id + ";";
                try
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                        res = Convert.ToInt32(dt.Rows[0]["Id"]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            DbConnection.Deconnecter();
            return res;
        }
        public static async Task<BindingList<Collection>> GetCollectionListByCash_desk_in(int iduser, int cash_desk_in)
        {
            BindingList<Collection> list = new BindingList<Collection>();
            string sqlCmd = "SELECT distinct accounting_transfer.Id, accounting_transfer.amount, accounting_transfer.create_date, ucOut.name as cash_desk_out, ucIn.name as cash_desk_in,accounting_transfer_state.name as state, accounting_transfer.memo, accounting_transfer.validated  " +
                    "FROM accounting_transfer left join  " +
                    "accounting_cash_desk ucIn on accounting_transfer.cash_desk_in = ucIn.Id left join  " +
                    "accounting_cash_desk ucOut on accounting_transfer.cash_desk_out = ucOut.Id left join  " +
                    "accounting_user_cash_desk on (accounting_user_cash_desk.cash_desk=accounting_transfer.cash_desk_in or accounting_user_cash_desk.cash_desk=accounting_transfer.cash_desk_out) and accounting_user_cash_desk.`user`=" + iduser + " left join  " +
                    "accounting_transfer_state_log on accounting_transfer_state_log.Id = accounting_transfer.state left join  " +
                    "accounting_transfer_state on accounting_transfer_state_log.state = accounting_transfer_state.Id  " +
                    "where accounting_transfer.cash_desk_in=" + cash_desk_in + " OR accounting_transfer.cash_desk_out=" + cash_desk_in + "  " +
                    "order by  accounting_transfer.Id desc";

            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {

                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        list.Add(new Collection(
                            reader.GetInt32("Id"),
                            reader.GetDateTime("create_date"),
                            reader["cash_desk_out"].ToString(),
                            reader["cash_desk_in"].ToString(),
                            reader.GetDecimal("amount"),
                            reader["state"].ToString(),
                            reader["memo"].ToString()));
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
        public static async Task<BindingList<Collection>> GetCollectionListByWaiting()
        {
            BindingList<Collection> list = new BindingList<Collection>();
            int iduser = Preferences.Get("iduser", 0);
            string sqlCmd = "SELECT distinct accounting_transfer.Id, accounting_transfer.amount, accounting_transfer.create_date, ucOut.name as cash_desk_out, ucIn.name as cash_desk_in,accounting_transfer_state.name as state, accounting_transfer.memo, accounting_transfer.validated  " +
                "FROM accounting_transfer left join  " +
                "accounting_cash_desk ucIn on accounting_transfer.cash_desk_in = ucIn.Id left join  " +
                "accounting_cash_desk ucOut on accounting_transfer.cash_desk_out = ucOut.Id inner join  " +
                 "accounting_user_cash_desk on (accounting_user_cash_desk.cash_desk=accounting_transfer.cash_desk_in or accounting_user_cash_desk.cash_desk=accounting_transfer.cash_desk_out) and accounting_user_cash_desk.`user`=" + iduser + " left join  " +
                "accounting_transfer_state_log on accounting_transfer_state_log.Id = accounting_transfer.state left join  " +
                "accounting_transfer_state on accounting_transfer_state_log.state = accounting_transfer_state.Id  " +
                "WHERE accounting_transfer_state_log.state=1  " +
                "order by accounting_transfer.Id desc";
            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {


                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        list.Add(new Collection(
                            reader.GetInt32("Id"),
                            reader.GetDateTime("create_date"),
                            reader["cash_desk_out"].ToString(),
                            reader["cash_desk_in"].ToString(),
                            reader.GetDecimal("amount"),
                            reader["state"].ToString(),
                            reader["memo"].ToString()));
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
        public static async Task<BindingList<Collection>> GetCollectionList()
        {
            BindingList<Collection> list = new BindingList<Collection>();
            string sqlCmd = "SELECT distinct accounting_transfer.Id, accounting_transfer.amount, accounting_transfer.create_date, ucOut.name as cash_desk_out, ucIn.name as cash_desk_in,accounting_transfer_state.name as state, accounting_transfer.memo, accounting_transfer.validated  " +
                    "FROM accounting_transfer left join  " +
                    "accounting_cash_desk ucIn on accounting_transfer.cash_desk_in = ucIn.Id left join  " +
                    "accounting_cash_desk ucOut on accounting_transfer.cash_desk_out = ucOut.Id left join  " +
                    "accounting_transfer_state_log on accounting_transfer_state_log.Id = accounting_transfer.state left join  " +
                    "accounting_transfer_state on accounting_transfer_state_log.state = accounting_transfer_state.Id  " +
                    "order by  accounting_transfer.Id desc";
            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {

                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        list.Add(new Collection(
                            reader.GetInt32("Id"),
                            reader.GetDateTime("create_date"),
                            reader["cash_desk_out"].ToString(),
                            reader["cash_desk_in"].ToString(),
                            reader.GetDecimal("amount"),
                            reader["state"].ToString(),
                            reader["memo"].ToString()));
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
        public static async Task<BindingList<Collection>> GetCollectionList(int iduser)
        {
            BindingList<Collection> list = new BindingList<Collection>();
            string sqlCmd = "SELECT distinct accounting_transfer.Id, accounting_transfer.amount, accounting_transfer.create_date, ucOut.name as cash_desk_out, ucIn.name as cash_desk_in,accounting_transfer_state.name as state, accounting_transfer.memo, accounting_transfer.validated  " +
                    "FROM accounting_transfer left join  " +
                    "accounting_cash_desk ucIn on accounting_transfer.cash_desk_in = ucIn.Id left join  " +
                    "accounting_cash_desk ucOut on accounting_transfer.cash_desk_out = ucOut.Id inner join  " +
                    "accounting_user_cash_desk on (accounting_user_cash_desk.cash_desk=accounting_transfer.cash_desk_in or accounting_user_cash_desk.cash_desk=accounting_transfer.cash_desk_out) and accounting_user_cash_desk.`user`=" + iduser + " left join  " +
                    "accounting_transfer_state_log on accounting_transfer_state_log.Id = accounting_transfer.state left join  " +
                    "accounting_transfer_state on accounting_transfer_state_log.state = accounting_transfer_state.Id  " +
                    "order by  accounting_transfer.Id desc";
            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {

                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        list.Add(new Collection(
                            reader.GetInt32("Id"),
                            reader.GetDateTime("create_date"),
                            reader["cash_desk_out"].ToString(),
                            reader["cash_desk_in"].ToString(),
                            reader.GetDecimal("amount"),
                            reader["state"].ToString(),
                            reader["memo"].ToString()));
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
        public void GetPaymentTransferByTransfer()
        {
            try
            {
                //Payment_pieceList.Clear();
                string sqlCmd = "SELECT accounting_payment_transfer.Id,commercial_payment.amount, accounting_payment_transfer.create_date, accounting_payment_transfer.payment, accounting_payment_transfer.transfer, accounting_payment_transfer.memo,commercial_payment.code as PaymentCode,commercial_partner.name as PartnerName FROM accounting_payment_transfer left join commercial_payment on accounting_payment_transfer.payment=commercial_payment.Id left join commercial_partner on commercial_payment.partner= commercial_partner.Id where accounting_payment_transfer.transfer=" + this.Id + ";";
                DbConnection.Deconnecter();
                DbConnection.Connecter();

                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        this.PaymentList.Add(new Payment_transfer(
                            Convert.ToInt32(reader["Id"]),
                            Convert.ToInt32(reader["payment"]),
                            Convert.ToInt32(reader["transfer"]),
                            Convert.ToDecimal(reader["amount"]),
                            reader["PaymentCode"].ToString(),
                            reader["PartnerName"].ToString()
                            ));
                    }
                    catch (Exception ex)
                    {
                        App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                    }
                }

                reader.Close();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            DbConnection.Deconnecter();
        }
        #endregion
        public class Payment_transfer
        {
            #region Attribut
            public int Id { get; set; }
            public DateTime create_date { get; set; }
            public int payment { get; set; }
            public int transfer { get; set; }
            public decimal amount { get; set; }
            public string memo { get; set; }
            public string PaymentCode { get; set; }
            public string PartnerName { get; set; }
            #endregion
            #region Constructeur
            public Payment_transfer()
            {
            }
            public Payment_transfer(int Id, int payment, int transfer, decimal amount, string PaymentCode, string PartnerName)
            {
                this.Id = Id;
                this.create_date = create_date;
                this.payment = payment;
                this.transfer = transfer;
                this.memo = memo;
                this.amount = amount;
                this.PaymentCode = PaymentCode;
                this.PartnerName = PartnerName;
            }
            #endregion
            public void Insert()
            {
                string sqlCmd = "INSERT INTO accounting_payment_transfer SET create_date= NOW(),payment=" + payment + ",transfer=" + transfer + ",memo='" + memo + "';SELECT MAX(Id) FROM " + DbConnection.Database + ".accounting_transfer;";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    DbConnection.Connecter();
                    cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                DbConnection.Deconnecter();
            }
        }
        public class Payment : ObservableObject
        {
            public int Id { get; set; }
            public string code { get; set; }
            public DateTime date { get; set; }
            public string Partner { get; set; }
            public string MethodeDePayement { get; set; }
            public string reference { get; set; }
            public decimal Montant { get; set; }
            public DateTime? DateEcheance { get; set; }
            public bool SOLDE { get; set; }
            public bool VALIDE { get; set; }
            private bool is_checked;
            public bool Is_checked { get => is_checked; set => SetProperty(ref is_checked, value); }
            public Payment() { }
            public Payment(int id, string code, DateTime date, string partner, string methodeDePayement, string reference, decimal montant, DateTime? dateEcheance, bool sOLDE, bool vALIDE, bool is_checked)
            {
                Id = id;
                this.code = code;
                this.date = date;
                Partner = partner;
                MethodeDePayement = methodeDePayement;
                this.reference = reference;
                Montant = montant;
                DateEcheance = dateEcheance;
                SOLDE = sOLDE;
                VALIDE = vALIDE;
                Is_checked = is_checked;
            }
            public static bool IsPaymentAvailable(int idPayment)
            {
                bool res = true;
                string sqlCmd = "SELECT accounting_transfer.Id, accounting_transfer.amount, accounting_transfer.create_date, accounting_transfer.cash_desk_in, accounting_transfer.cash_desk_out, accounting_transfer.memo, accounting_transfer.validated, accounting_transfer_state.Id AS state " +
                    "FROM accounting_transfer LEFT OUTER JOIN " +
                    "accounting_transfer_state_log ON accounting_transfer.state = accounting_transfer_state_log.Id LEFT OUTER JOIN " +
                    "accounting_transfer_state ON accounting_transfer_state_log.state = accounting_transfer_state.Id " +
                    "WHERE(accounting_transfer.Id = " +
                    "(SELECT transfer AS exp1 " +
                    "FROM accounting_payment_transfer " +
                    "WHERE(payment = " + idPayment + ") AND(create_date = " +
                    "(SELECT MAX(create_date) AS Expr1 " +
                    "FROM accounting_payment_transfer accounting_payment_transfer_1 " +
                    "WHERE(payment = " + idPayment + ")))))";
                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                adapter.SelectCommand.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                try
                {
                    if (dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0]["state"]) == 1)
                        res = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                DbConnection.Deconnecter();

                return res;
            }
            public static bool IsListPaymentAvailable(BindingList<Payment_transfer> PTList)
            {
                bool res = true;
                foreach (Payment_transfer pt in PTList)
                    if (!IsPaymentAvailable(pt.payment))
                    {
                        res = false;
                        break;
                    }
                return res;
            }
        }
        public class State
        {
            public int Id { get; set; }
            public DateTime create_date { get; set; }
            public int transfer { get; set; }
            public int state { get; set; }
            public State(int transfer, int state)
            {
                this.transfer = transfer;
                this.state = state;
            }
            public void Insert()
            {
                string sqlCmd = "INSERT INTO accounting_transfer_state_log SET create_date= NOW(),transfer=" + transfer + ",state=" + state + ";SELECT MAX(Id) FROM " + DbConnection.Database + ".accounting_transfer_state_log;";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    DbConnection.Connecter();
                    this.Id = int.Parse(cmd.ExecuteScalar().ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                DbConnection.Deconnecter();
            }
        }
        public class Collection
        {
            public int Id { get; set; }
            public DateTime create_date { get; set; }
            public string cash_desk_out { get; set; }
            public string cash_desk_in { get; set; }
            public decimal Amount { get; set; }
            public string state { get; set; }
            public string memo { get; set; }
            public Color stateColor
            {
                get
                {
                    switch (state)
                    {
                        case ("Accepté"):
                            return Colors.GreenYellow;
                        case ("Refusé"):
                            return Colors.Red;
                        default: return Colors.Gray;
                    }
                }
            }

            public Collection(int id, DateTime create_date, string cash_desk_out, string cash_desk_in, decimal amount, string state, string memo)
            {
                Id = id;
                this.create_date = create_date;
                this.cash_desk_out = cash_desk_out;
                this.cash_desk_in = cash_desk_in;
                Amount = amount;
                this.state = state;
                this.memo = memo;
            }
        }
    }
}
