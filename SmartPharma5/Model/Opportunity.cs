using Microsoft.Maui.Graphics;
using MvvmHelpers;
using MySqlConnector;
using SQLite;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using Color = System.Drawing.Color;

namespace SmartPharma5.Model
{
    public class Opportunity : BaseViewModel
    {
        #region attribus
        [Column("Id")]
        public int Id { get; set; }
        [Column("Code")]
        public string code { get; set; }
        [Column("date de création")]
        public DateTime? create_date
        {
            get
            {
                return this._create_date.HasValue
                   ? this._create_date.Value
                   : DateTime.Now;
            }

            set { this._create_date = value; }
        }
        private DateTime? _create_date = null;
        [Column("Référence")]
        public string reference { get; set; }
        [Column("Conditions générales")]
        public string general_condition { get; set; }
        [Column("Notes")]
        public string memo { get; set; }

        [Column("Date")]
        public DateTime? date
        {
            get
            {
                return this._date.HasValue
                   ? this._date.Value
                   : DateTime.Now;
            }

            set { this._date = value; }
        }
        private DateTime? _date = null;
        public bool withholding_tax_chec { get; set; }
        public int IdWithholding_tax { get; set; }
        public int IdRevenue_stamp { get; set; }
        public int IdPartner { get; set; }
        public int IdPayment_method { get; set; }
        public int IdPayment_condition { get; set; }

        [Column("Mode de payement")]
        public int Payment_method { get; set; }
        [Column("Condition de payement")]
        public int Payment_condition { get; set; }
        private decimal amount;
        [Column("Montant Total")]
        public decimal totalAmount { get => amount; set => SetProperty(ref amount, value); }
        public bool toinvoice { get; set; }
        public bool validated { get; set; }
        public bool delivred { get; set; }
        public DateTime? delivred_date
        {
            get
            {
                return this._delivred_date.HasValue
                   ? this._delivred_date.Value
                   : DateTime.Now;
            }

            set { this._delivred_date = value; }
        }
        private DateTime? _delivred_date = null;
        public int IdWarehouse { get; set; }

        [Column("Partenaire")]
        public string partnerName { get; set; }
        //public List<Opportunity_Line> piece_lineList { get; set; }
        public DateTime? due_date
        {
            get
            {
                switch (IdPayment_condition)
                {
                    case 2: return date.Value.AddDays(15);
                    case 3:
                        return new DateTime(date.Value.AddDays(30).Year, date.Value.AddDays(30).Month,
                    DateTime.DaysInMonth(date.Value.AddDays(30).Year, date.Value.AddDays(30).Month));
                    case 4: return date.Value.AddDays(30);
                    case 5: return new DateTime(date.Value.Year, date.Value.Month, DateTime.DaysInMonth(date.Value.Year, date.Value.Month));
                    case 6: return null;
                    case 7: return date.Value.AddDays(60);
                    case 8: return date.Value.AddDays(90);
                    case 9: return date.Value.AddDays(120);
                    case 10: return date.Value.AddDays(150);
                    case 11: return date.Value.AddDays(180);
                    default: return date;
                }
            }
            set { }
        }
        // Propriété
        public uint? Currency { get; set; }
        public int? IdAgent { get; set; }
        public uint parent { get; set; }
        private int dealer;
        public int Dealer { get => dealer; set => SetProperty(ref dealer, value); }
        private string dealername;
        public string dealerName { get => dealername; set => SetProperty(ref dealername, value); }
        public string stateName { get; set; }
        public int state { get; set; }
        // public decimal purchase_probability { get; set; }
        public int purchase_probability { get; set; }
        private BindingList<OpportunityLine> opportunity_lines;
        public BindingList<OpportunityLine> Opportunity_lines { get => opportunity_lines; set => SetProperty(ref opportunity_lines, value); }
        //public BindingList<OpportunityLine> OpportunityLines { get => opportunity_lines; set => SetProperty(ref opportunity_lines, value); }
        public BindingList<State> StateLines { get; set; }
         
        #endregion
        #region constructeur
        public Opportunity() { this.opportunity_lines = new BindingList<OpportunityLine>(); }
        public Opportunity(Collection collection)
        {
            Id = collection.Id;
            this.opportunity_lines = new BindingList<OpportunityLine>();
        }
            public Opportunity(int id)
        {
            string sqlCmd = "select crm_opportunity.purchase_probability, crm_opportunity.Id, crm_opportunity.code, crm_opportunity.create_date, crm_opportunity.date, " +
                "crm_opportunity.reference, crm_opportunity.memo, crm_opportunity.general_condition, crm_opportunity.partner, " +
                "crm_opportunity.payment_condition, crm_opportunity.payment_method, crm_opportunity.validated, " +
                "crm_opportunity.agent, crm_opportunity.revenue_stamp, crm_opportunity.to_invoice, " +
                "crm_opportunity.state, crm_opportunity.order, crm_opportunity.dealer, crm_opportunity.currency, " + // crm_opportunity.currency
                "p1.name as partnerName, p2.name as dealerName, crm_opportunity.parent, " +
                "crm_state.name, crm_opportunity_state.create_date as date_State, crm_opportunity_state.state as opp_State, " +
                "crm_opportunity.order, sale_order.create_date as orderDate, sale_order.delivred, " +
                "max(sale_order.delivred_date) as delivredDate, sale_order.invoice, sale_invoice.create_date as invoiceDate " +
                "from crm_opportunity " +
                "left join commercial_partner p1 on p1.Id = crm_opportunity.partner " +
                "left join commercial_partner p2 on p2.Id = crm_opportunity.dealer " +
                "left join crm_opportunity_state on crm_opportunity.Id = crm_opportunity_state.opportunity " +
                "left join sale_order on sale_order.Id = crm_opportunity.order " +
                "left join sale_shipping on sale_order.Id = sale_shipping.order " +
                "left join sale_invoice on sale_invoice.Id = sale_order.invoice " +
                "left join crm_state on crm_state.Id = crm_opportunity_state.state " +
                "where crm_opportunity_state.opportunity = " + id +
                " group by crm_opportunity_state.Id " +
                "order by crm_opportunity_state.date;";
            DbConnection.Connecter();
            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            try
            {
                this.Id = id;
                this.code = dt.Rows[0]["code"].ToString();
                this.create_date = Convert.ToDateTime(dt.Rows[0]["create_date"]);
                this.partnerName = dt.Rows[0]["partnerName"].ToString();
                this.dealerName = dt.Rows[0]["dealerName"].ToString();
                this.date = Convert.ToDateTime(dt.Rows[0]["date"]);
                this.reference = dt.Rows[0]["reference"].ToString();
                this.memo = dt.Rows[0]["memo"].ToString();
                this.general_condition = dt.Rows[0]["general_condition"].ToString();

                this.IdPartner = Convert.ToInt32(dt.Rows[0]["partner"].ToString());
                this.IdPayment_condition = Convert.ToInt32(dt.Rows[0]["payment_condition"].ToString()); ;
                this.IdPayment_method = Convert.ToInt32(dt.Rows[0]["payment_method"].ToString());
                this.validated = Convert.ToBoolean(dt.Rows[0]["validated"]);
                // this.purchase_probability = Convert.ToDecimal(dt.Rows[0]["purchase_probability"]);
                this.purchase_probability = Convert.ToInt32(dt.Rows[0]["purchase_probability"]);
                this.Currency = Convert.ToUInt32(dt.Rows[0]["currency"].ToString());

                try
                {
                    this.delivred = Convert.ToBoolean(dt.Rows[0]["delivred"]);
                }
                catch { }
                try { this.delivred_date = Convert.ToDateTime(dt.Rows[0]["delivredDate"]); }
                catch { }
                this.IdAgent = dt.Rows[0]["agent"] is uint ? Convert.ToInt32(dt.Rows[0]["agent"]) : (int?)null; ;


                this.IdRevenue_stamp = Convert.ToInt32(dt.Rows[0]["revenue_stamp"]);
                this.toinvoice = Convert.ToBoolean(dt.Rows[0]["to_invoice"]);
                this.state = Convert.ToInt32(dt.Rows[0]["state"].ToString());
                //this.order = Convert.ToUInt32(reader["order"]);
                this.Dealer = Convert.ToInt32(dt.Rows[0]["dealer"]);
                this.parent = Convert.ToUInt32(dt.Rows[0]["parent"]);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            opportunity_lines = new BindingList<OpportunityLine>(OpportunityLine.GetOpportunityLine(this.Id));
            totalAmount = getTotalAmount(opportunity_lines);
            this.StateLines = State.getStateList(dt);

        }

       /*  public Opportunity(int id, int IdPartner, string partnerName, int IdPayment_method, int IdPayment_condition, int? IdAgent, string memo, bool toinvoice, int parent, int dealer, BindingList<OpportunityLine> lines)
          {
              this.Id = id;
              this.IdPartner = IdPartner;
              this.partnerName = partnerName;
              this.IdPayment_condition = IdPayment_condition;
              this.IdPayment_method = IdPayment_method;
              this.IdAgent = IdAgent;
              this.memo = memo;
              this.toinvoice = toinvoice;
              this.parent = (uint)parent;
              this.Dealer = dealer;
              this.opportunity_lines = new BindingList<OpportunityLine>(lines);

          }*/
       public Opportunity(int id, int IdPartner, string partnerName, int IdPayment_method, int IdPayment_condition, int? IdAgent, string memo, uint? currency, bool toinvoice, int parent, int dealer, BindingList<OpportunityLine> lines)
        {
            this.Id = id;
            this.IdPartner = IdPartner;
            this.partnerName = partnerName;
            this.IdPayment_condition = IdPayment_condition;
            this.IdPayment_method = IdPayment_method;
            this.IdAgent = IdAgent;
            this.memo = memo;
            this.Currency = currency;  // Ajouté ici
            this.toinvoice = toinvoice;
            this.parent = (uint)parent;
            this.Dealer = dealer;
            this.opportunity_lines = new BindingList<OpportunityLine>(lines);
        }

        /*   public Opportunity(int id, int IdPartner, string partnerName, int IdPayment_method, int IdPayment_condition, int? IdAgent, string memo, string currency, bool toinvoice, int parent, int dealer, BindingList<OpportunityLine> lines)
          {
              this.Id = id;
              this.IdPartner = IdPartner;
              this.partnerName = partnerName;
              this.IdPayment_condition = IdPayment_condition;
              this.IdPayment_method = IdPayment_method;
              this.IdAgent = IdAgent;
              this.memo = memo;
              this.currency = currency;
              this.toinvoice = toinvoice;
              this.parent = (uint)parent;
              this.Dealer = dealer;
              this.opportunity_lines = new BindingList<OpportunityLine>(lines);
          }*/

        public Opportunity(int idagent, Partner partner)
        {
            IdAgent = idagent;
            IdPartner = (int)partner.Id;
            partnerName = partner.Name;
            IdPayment_condition = partner.PaymentConditionCustomer;
            IdPayment_method = (int)partner.PaymentMethodCustomer;
            toinvoice = true;
            Currency = partner.Currency;
            this.opportunity_lines = new BindingList<OpportunityLine>();
           // LoadCurrencyFromPartner((int)partner.Id);  
        }
     /*   private void LoadCurrencyFromPartner(int partnerId)
        {
            string sqlCmd = "SELECT currency FROM commercial_partner WHERE Id = @PartnerId";
            DbConnection.Connecter();

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.Parameters.AddWithValue("@PartnerId", partnerId);
            adapter.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                string currencyName = dt.Rows[0]["currency"].ToString();
                if (!string.IsNullOrEmpty(currencyName))
                {
                    this.Currency = currencyName; 
                }
            }
        }*/
        public Opportunity(Opportunity opportunity)
        {
            Dealer = opportunity.Dealer;
            dealerName = opportunity.dealerName;
        }


        public void TransferToQuotation()
        {
            int IDBC = 0;
            string Code = CreatCodeQuotation();
            string totalAmount = this.totalAmount.ToString().Replace(',', '.');
            string sqlCmd = "INSERT INTO sale_quotation SET code ='" + Code + "', create_date= NOW(), date= NOW(), " +
                "tva_chec=" + true + ", memo='" + memo + "', currency='" + Currency + "', " +
                "partner=" + (int)IdPartner + ", payment_method=" + (int)IdPayment_method +
                ", payment_condition=" + (int)IdPayment_condition + ", validated=false, total_amount=" + totalAmount +
                ", agent=" + (int)IdAgent + ", tax1=true, tax2=true, tax3=true, revenue_stamp=0, crm_opportunity=" + Id +
                "; SELECT MAX(Id) FROM " + DbConnection.Database + ".sale_quotation;";

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {

                IDBC = int.Parse(cmd.ExecuteScalar().ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine("err");
            }
            DbConnection.Deconnecter();
            sqlCmd = "UPDATE crm_opportunity SET crm_opportunity.quotation = " + IDBC + " WHERE Id = " + Id + ";";
            cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {

                cmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();
            foreach (var line in opportunity_lines)
            {
                string price = line.price.ToString().Replace(',', '.');
                string discount = line.discount.ToString().Replace(',', '.');
                string quantity = line.quantity.ToString().Replace(',', '.');
                string tax1_base = line.tax1_base.ToString().Replace(',', '.');
                string tax1_amount = line.tax1_amount.ToString().Replace(',', '.');
                string tax2_base = line.tax2_base.ToString().Replace(',', '.');
                string tax2_amount = line.tax2_amount.ToString().Replace(',', '.');
                string tax3_base = line.tax3_base.ToString().Replace(',', '.');
                string tax3_amount = line.tax3_amount.ToString().Replace(',', '.');
                string tax4_base = line.tax4_base.ToString().Replace(',', '.');
                string tax4_amount = line.tax4_amount.ToString().Replace(',', '.');
                string tax5_base = line.tax5_base.ToString().Replace(',', '.');
                string tax5_amount = line.tax5_amount.ToString().Replace(',', '.');

                sqlCmd = "INSERT INTO sale_quotation_line SET description='" + line.description + "', price=" + price + ", discount=" + discount + ", piece=" + IDBC + ",product=" + line.IdProduct + ",quantity=" + quantity + ",tax1=" + (line.tax1 is uint ? (string)line.tax1.Value.ToString() : "NULL") + ",tax1_base=" + tax1_base + ",tax1_amount=" + tax1_amount + ",tax2=" + (line.tax2 is uint ? (string)line.tax2.Value.ToString() : "NULL") + ",tax2_base=" + tax2_base + ",tax2_amount=" + tax2_amount + ",tax3=" + (line.tax3 is uint ? (string)line.tax3.Value.ToString() : "NULL") + ",tax3_base=" + tax3_base + ",tax3_amount=" + tax3_amount + ",tax4=" + (line.tax4 is uint ? (string)line.tax4.Value.ToString() : "NULL") + ",tax4_base=" + tax4_base + ",tax4_amount=" + tax4_amount + ",tax5=" + (line.tax5 is uint ? (string)line.tax5.Value.ToString() : "NULL") + ",tax5_base=" + tax5_base + ",tax5_amount=" + tax5_amount + ";";
                cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                DbConnection.Connecter();
                try
                {

                    cmd.ExecuteScalar();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                DbConnection.Deconnecter();
            }
            //ModifyState(2);
        }
        public void TransferToBc()
        {
            int IDBC = 0;
            string Code = CreatCodeBc();
            string totalAmount = this.totalAmount.ToString().Replace(',', '.');

            string sqlCmd = "INSERT INTO sale_order SET code ='" + Code + "',create_date= NOW(), date= NOW(),tva_chec=" + true + ",memo='" + memo + "', currency='" + Currency + "',partner=" + (int)IdPartner + ",payment_method=" + (int)IdPayment_method + ",payment_condition=" + (int)IdPayment_condition + ",validated=false,total_amount=" + totalAmount + ",paied_amount=0,delivred=0,due_date=Now(),delivred_date=now(),agent=" + (int)IdAgent + ",tax1=true,tax2=true,tax3=true,revenue_stamp=0;SELECT MAX(Id) FROM " + DbConnection.Database + ".sale_order;";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {

                IDBC = int.Parse(cmd.ExecuteScalar().ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine("err");
            }
            DbConnection.Deconnecter();
            sqlCmd = "UPDATE crm_opportunity SET crm_opportunity.order = " + IDBC + " WHERE Id = " + Id + ";";
            cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {

                cmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();
            foreach (var line in opportunity_lines)
            {
                string price = line.price.ToString().Replace(',', '.');
                string discount = line.discount.ToString().Replace(',', '.');
                string quantity = line.quantity.ToString().Replace(',', '.');
                string tax1_base = line.tax1_base.ToString().Replace(',', '.');
                string tax1_amount = line.tax1_amount.ToString().Replace(',', '.');
                string tax2_base = line.tax2_base.ToString().Replace(',', '.');
                string tax2_amount = line.tax2_amount.ToString().Replace(',', '.');
                string tax3_base = line.tax3_base.ToString().Replace(',', '.');
                string tax3_amount = line.tax3_amount.ToString().Replace(',', '.');
                string tax4_base = line.tax4_base.ToString().Replace(',', '.');
                string tax4_amount = line.tax4_amount.ToString().Replace(',', '.');
                string tax5_base = line.tax5_base.ToString().Replace(',', '.');
                string tax5_amount = line.tax5_amount.ToString().Replace(',', '.');

                sqlCmd = "INSERT INTO sale_order_line SET description='" + line.description + "', price=" + price + ", discount=" + discount + ", piece=" + IDBC + ",product=" + line.IdProduct + ",quantity=" + quantity + ",tax1=" + (line.tax1 is uint ? (string)line.tax1.Value.ToString() : "NULL") + ",tax1_base=" + tax1_base + ",tax1_amount=" + tax1_amount + ",tax2=" + (line.tax2 is uint ? (string)line.tax2.Value.ToString() : "NULL") + ",tax2_base=" + tax2_base + ",tax2_amount=" + tax2_amount + ",tax3=" + (line.tax3 is uint ? (string)line.tax3.Value.ToString() : "NULL") + ",tax3_base=" + tax3_base + ",tax3_amount=" + tax3_amount + ",tax4=" + (line.tax4 is uint ? (string)line.tax4.Value.ToString() : "NULL") + ",tax4_base=" + tax4_base + ",tax4_amount=" + tax4_amount + ",tax5=" + (line.tax5 is uint ? (string)line.tax5.Value.ToString() : "NULL") + ",tax5_base=" + tax5_base + ",tax5_amount=" + tax5_amount + ";";
                cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                DbConnection.Connecter();
                try
                {

                    cmd.ExecuteScalar();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                DbConnection.Deconnecter();
            }
            ModifyState(2);
        }
        public Opportunity opportunityLineGratuite(Opportunity opportunity)
        {
            //BindingList<OpportunityLine> List = new BindingList<OpportunityLine>();
            foreach (OpportunityLine line in opportunity.opportunity_lines)
            {
                line.quantity = 0;
                line.discount = 1;
                line.PTARTTC = line.PUARTTC * line.quantity;
            }
            totalAmount = opportunity.getTotalAmount(opportunity_lines);
            return opportunity;
        }
        private string CreatCodeQuotation()
        {
            string year = string.Empty;
            string sqlCmd = "SELECT prefix,separator1,year,separator2,final_number,separator3,suffixe FROM commercial_dialing where piece_type like 'Sale.Quotation%';";

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adapter.Fill(dt);


            if (int.Parse(dt.Rows[0]["year"].ToString()) == 1)
                year = DateTime.Now.Year.ToString();

            int BCID = int.Parse(dt.Rows[0]["final_number"].ToString()) + 1;

            DbConnection.Deconnecter();
            DbConnection.Connecter();
            sqlCmd = "Update commercial_dialing Set final_number = final_number + 1 where piece_type like '%Sale.Quotation%';";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            try
            {
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            DbConnection.Deconnecter();

            return _ = dt.Rows[0]["prefix"].ToString() + dt.Rows[0]["separator1"].ToString() + year + dt.Rows[0]["separator2"].ToString() + (int.Parse(dt.Rows[0]["final_number"].ToString())).ToString() + dt.Rows[0]["separator3"].ToString() + dt.Rows[0]["suffixe"].ToString();
        }
        private string CreatCodeBc()
        {
            string year = string.Empty;
            string sqlCmd = "SELECT prefix,separator1,year,separator2,final_number,separator3,suffixe FROM commercial_dialing where piece_type like 'Sale.Order%';";

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adapter.Fill(dt);


            if (int.Parse(dt.Rows[0]["year"].ToString()) == 1)
                year = DateTime.Now.Year.ToString();

            int BCID = int.Parse(dt.Rows[0]["final_number"].ToString()) + 1;

            DbConnection.Deconnecter();
            DbConnection.Connecter();
            sqlCmd = "Update commercial_dialing Set final_number = final_number + 1 where piece_type like '%Sale.Order%';";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            try
            {
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            DbConnection.Deconnecter();

            return _ = dt.Rows[0]["prefix"].ToString() + dt.Rows[0]["separator1"].ToString() + year + dt.Rows[0]["separator2"].ToString() + (int.Parse(dt.Rows[0]["final_number"].ToString())).ToString() + dt.Rows[0]["separator3"].ToString() + dt.Rows[0]["suffixe"].ToString();
        }
        #endregion
        #region parametre
        public virtual bool referenceExiste()
        {
            return false;
        }
        public virtual bool getTva_chec() { return true; }
        public virtual string getCode()
        {
            return null;
        }
        public virtual int getLastId()
        {
            return 0;
        }

        #endregion
        #region price
        public virtual decimal getTotalAmount()
        {
            decimal total = 0;
            foreach (OpportunityLine line in opportunity_lines)
                total = total + line.PTARTTC;
            return Math.Round(total, 3);
        }

        #endregion
        #region transfer
        public virtual BindingList<Opportunity> getPieceTransferred()
        {
            return new BindingList<Opportunity>();
        }

        public virtual string type { get; set; }
        #endregion
        #region Taxe
        public class Taxe
        {
            public uint IdTaxe { get; set; }
            public decimal amount { get; set; }
            public decimal baseAmount { get; set; }
            public string taxeName { get; set; }
            public string taxeType { get; set; }
            public int level { get; set; }
            public Taxe() { }
            public Taxe(uint IdTaxe, decimal amount, decimal baseAmount, string taxeName, int level)
            {
                this.IdTaxe = IdTaxe;
                this.amount = amount;
                this.baseAmount = baseAmount;
                this.taxeName = taxeName;
                this.level = level;
                this.taxeType = new Tax().GetType().AssemblyQualifiedName;
            }

            public void addTax(decimal baseAmount, decimal amount)
            {
                this.baseAmount += baseAmount;
                this.amount += amount;
            }
        }
        public BindingList<Taxe> getTaxGroup(BindingList<OpportunityLine> Piece_lineList)
        {
            BindingList<Taxe> TaxList = new BindingList<Taxe>();
            foreach (OpportunityLine line in Piece_lineList)
            {
                if (line.tax1.HasValue)
                {
                    bool check = false;
                    foreach (Taxe Tax in TaxList)
                        if (Tax.IdTaxe == line.tax1)
                        {
                            Tax.addTax(line.tax1_base, line.tax1_amount);
                            check = true;
                            break;
                        }
                    if (!check)
                        TaxList.Add(new Taxe(line.tax1.Value, line.tax1_amount, line.tax1_base, line.tax1_name, 1));
                }
                if (line.tax2.HasValue)
                {
                    bool check = false;
                    foreach (Taxe Tax in TaxList)
                        if (Tax.IdTaxe == line.tax2)
                        {
                            Tax.addTax(line.tax2_base, line.tax2_amount);
                            check = true;
                            break;
                        }
                    if (!check)
                        TaxList.Add(new Taxe(line.tax2.Value, line.tax2_amount, line.tax2_base, line.tax2_name, 2));
                }
                if (line.tax3.HasValue)
                {
                    bool check = false;
                    foreach (Taxe Tax in TaxList)
                        if (Tax.IdTaxe == line.tax3)
                        {
                            Tax.addTax(line.tax3_base, line.tax3_amount);
                            check = true;
                            break;
                        }
                    if (!check)
                        TaxList.Add(new Taxe(line.tax3.Value, line.tax3_amount, line.tax3_base, line.tax3_name, 3));
                }
                if (line.tax4.HasValue)
                {
                    bool check = false;
                    foreach (Taxe Tax in TaxList)
                        if (Tax.IdTaxe == line.tax4)
                        {
                            Tax.addTax(line.tax4_base, line.tax4_amount);
                            check = true;
                            break;
                        }
                    if (!check)
                        TaxList.Add(new Taxe(line.tax4.Value, line.tax4_amount, line.tax4_base, line.tax4_name, 4));
                }
                if (line.tax5.HasValue)
                {
                    bool check = false;
                    foreach (Taxe Tax in TaxList)
                        if (Tax.IdTaxe == line.tax5)
                        {
                            Tax.addTax(line.tax5_base, line.tax5_amount);
                            check = true;
                            break;
                        }
                    if (!check)
                        TaxList.Add(new Taxe(line.tax5.Value, line.tax5_amount, line.tax5_base, line.tax5_name, 5));
                }
            }

            return TaxList;
        }

        #endregion

        public class Withholding_tax_line
        {
            public int Id { get; set; }
            public int IdWithholding_tax { get; set; }
            public string piece_type { get; set; }
            public int IdPiece { get; set; }
            public decimal amount { get; set; }
            public string Withholding_taxName { get; set; }
            public decimal value { get; set; }
            public decimal pieceAmount { get; set; }
            public decimal netAmount { get; set; }
            public string pieceCode { get; set; }


            public Withholding_tax_line(int IdWithholding_tax, string piece_type, int IdPiece, decimal amount,
                string Withholding_taxName, decimal value, decimal pieceAmount, decimal netAmount, string code, string reference)
            {
                this.IdWithholding_tax = IdWithholding_tax;
                this.piece_type = piece_type;
                this.IdPiece = IdPiece;
                this.amount = amount;
                this.Withholding_taxName = Withholding_taxName;
                this.value = value;
                this.pieceAmount = pieceAmount;
                this.netAmount = netAmount;
                if (piece_type.Contains("Purchase"))
                    this.pieceCode = reference;
                else
                    this.pieceCode = code;

            }

        }
        public decimal getTotalAmount(IEnumerable<OpportunityLine> piece_ligne)
        {
            decimal total = 0;
            foreach (var piece in piece_ligne)
                total = total + piece.PTARTTC;
            return Math.Round(total, 3);
        }
        public async Task<bool?> Validate()
          {
              try
              {
                  if (!this.validated)
                      this.code = CreatCode();
                  this.validated = true;
                  if (Id == 0)
                    await insert();
                  else
                      update();
                  insertlinesAsync();
                  return true;
              }
              catch (Exception ex)
              {
                  return null;
              }
          }
        /*  public async Task<bool?> Validate()
          {
              try
              {
                  if (!this.validated)
                      this.code = CreatCode();
                  this.validated = true;
                  return true;
              }
              catch (Exception ex)
              {
                  return null;
              }
          }*/
        /*****************log**************************/
        public async Task Log(string actionType)
        {
            string macaddress = DbConnection.macAdresse();
            string ipaddress = DbConnection.IpAddress();
            string machinename = DbConnection.MachineName();
            string gpsCoordinates = null;

            try
            {
                // Tentative de récupération de la localisation avec timeout
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                Location location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    gpsCoordinates = $"{location.Latitude.ToString("F15", CultureInfo.InvariantCulture)},{location.Longitude.ToString("F15", CultureInfo.InvariantCulture)}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur de géolocalisation (non bloquante) : " + ex.Message);
                // On continue sans les coordonnées GPS
            }

            try
            {
                string sqlCmd = @"INSERT INTO atooerp_log 
                     SET user = " + Preferences.Get("iduser", 0) +
                                     ", object_type = '" + CurrentData.CurrentNoteModule.Replace("'", "''") +
                                     "', object = " + CurrentData.CurrentModuleId +
                                     ", type = '" + actionType.Replace("'", "''") +
                                     "', name = '" + ("opportunity").Replace("'", "''") + "'" +
                                     ", date = NOW()" +
                                     ", ip = '" + ipaddress.Replace("'", "''") + "'" +
                                     (gpsCoordinates != null ? ", gps = '" + gpsCoordinates.Replace("'", "''") + "'" : "") +
                                     ", machine_name = '" + machinename.Replace("'", "''") + "'" +
                                     ", mac_adress = '" + macaddress.Replace("'", "''") + "'";

                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                DbConnection.Connecter();

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur SQL lors de l'enregistrement du log : " + ex.Message);
                    Console.WriteLine("Requête SQL: " + sqlCmd); // Pour débogage
                }
                finally
                {
                    DbConnection.Deconnecter();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur générale lors de l'enregistrement du log : " + ex.Message);
            }
        }
        /**********************************************/
        public async Task insert()
        {
            string totalAmount = this.totalAmount.ToString().Replace(',', '.');
            string sqlCmd;

            // Récupérer la position GPS actuelle
            try
            {
                var location = await Geolocation.GetLocationAsync();

                if (location != null)
                {
                    // Si la localisation est disponible, inclure le champ GPS dans la requête
                    string gpsCoordinates = $"{location.Latitude.ToString("F15", CultureInfo.InvariantCulture)},{location.Longitude.ToString("F15", CultureInfo.InvariantCulture)}";

                    sqlCmd = @"INSERT INTO crm_opportunity 
                     SET purchase_probability=" + (int)purchase_probability +
                                     ",code ='" + code +
                                     "',create_date= NOW(), date= NOW(),tva_chec=" + true +
                                     ",memo='" + memo +
                                     "',partner=" + (int)IdPartner +
                                     ",payment_method=" + (int)IdPayment_method +
                                     ",payment_condition=" + (int)IdPayment_condition +
                                     ",validated=" + validated +
                                     ",total_amount=" + totalAmount +
                                     ",due_date=Now(),delivred_date=now(),agent=" + (int)IdAgent +
                                     ",tax1=true,tax2=true,tax3=true,revenue_stamp=0" +
                                     ",closing_date=Now(),to_invoice=" + toinvoice +
                                     ",dealer=" + Dealer +
                                     ", parent=" + parent +
                                     ",currency=" + Currency +
                                     ",gps='" + gpsCoordinates + "'" +
                                     ";SELECT LAST_INSERT_ID();";
                }
                else
                {
                    // Si la localisation n'est pas disponible, ne pas inclure le champ GPS
                    sqlCmd = @"INSERT INTO crm_opportunity 
                     SET purchase_probability=" + (int)purchase_probability +
                                     ",code ='" + code +
                                     "',create_date= NOW(), date= NOW(),tva_chec=" + true +
                                     ",memo='" + memo +
                                     "',partner=" + (int)IdPartner +
                                     ",payment_method=" + (int)IdPayment_method +
                                     ",payment_condition=" + (int)IdPayment_condition +
                                     ",validated=" + validated +
                                     ",total_amount=" + totalAmount +
                                     ",due_date=Now(),delivred_date=now(),agent=" + (int)IdAgent +
                                     ",tax1=true,tax2=true,tax3=true,revenue_stamp=0" +
                                     ",closing_date=Now(),to_invoice=" + toinvoice +
                                     ",dealer=" + Dealer +
                                     ", parent=" + parent +
                                     ",currency=" + Currency +
                                     ";SELECT LAST_INSERT_ID();";
                }
            }
            catch (Exception ex)
            {
                // En cas d'erreur lors de la récupération de la localisation, utiliser la requête sans GPS
                Console.WriteLine("Erreur de géolocalisation : " + ex.Message);
                sqlCmd = @"INSERT INTO crm_opportunity 
                 SET purchase_probability=" + (int)purchase_probability +
                                 ",code ='" + code +
                                 "',create_date= NOW(), date= NOW(),tva_chec=" + true +
                                 ",memo='" + memo +
                                 "',partner=" + (int)IdPartner +
                                 ",payment_method=" + (int)IdPayment_method +
                                 ",payment_condition=" + (int)IdPayment_condition +
                                 ",validated=" + validated +
                                 ",total_amount=" + totalAmount +
                                 ",due_date=Now(),delivred_date=now(),agent=" + (int)IdAgent +
                                 ",tax1=true,tax2=true,tax3=true,revenue_stamp=0" +
                                 ",closing_date=Now(),to_invoice=" + toinvoice +
                                 ",dealer=" + Dealer +
                                 ", parent=" + parent +
                                 ",currency=" + Currency +
                                 ";SELECT LAST_INSERT_ID();";
            }

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {
                Id = int.Parse(cmd.ExecuteScalar().ToString());
                CurrentData.CurrentModuleId = Id;
                await Log("insert");

            }
            catch
            {
                Console.WriteLine("err");
            }
            DbConnection.Deconnecter();
            sqlCmd = " INSERT INTO crm_opportunity_state SET create_date= NOW(), date= NOW(), opportunity=" + Id + ", state=1;SELECT MAX(Id) FROM crm_opportunity_state; ";
            cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {
                state = int.Parse(cmd.ExecuteScalar().ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();
            sqlCmd = " UPDATE crm_opportunity SET state = " + state + " WHERE Id = " + Id + ";";
            cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();
        }
        public void update()
        {
            string totalAmount = this.totalAmount.ToString().Replace(',', '.');
            string sqlCmd = "Update crm_opportunity SET purchase_probability=" + (int)purchase_probability + ", tva_chec=" + true + ",code= '" + code + "' ,memo='" + memo + "',partner=" + (int)IdPartner + ",payment_method=" + (int)IdPayment_method + ",payment_condition=" + (int)IdPayment_condition + ",validated=" + validated + ",total_amount=" + totalAmount + ",due_date=Now(),delivred_date=now(),agent=" + (int)IdAgent + ",revenue_stamp=0,closing_date=Now(),to_invoice=" + toinvoice + ",dealer=" + Dealer + ", parent=" + parent + ",currency=" + Currency +" where Id = " + Id + ";";
            
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {

                cmd.ExecuteScalar();
                int rowsAffected = cmd.ExecuteNonQuery();

                // Si la mise à jour a réussi (au moins 1 ligne affectée)
                if (rowsAffected > 0)
                {
                    // Enregistrer le log
                    Log("update");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();

        }

        /**********************************/
        /*  public static async Task<List<Currency>> GetCurrenciesFromDatabaseAsync()
          {
              var currencies = new List<Currency>();

              string sqlCmd = "SELECT name FROM atooerp_currency;";
              DbConnection.Connecter();

              try
              {
                  using (var cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                  {
                      using (var reader = await cmd.ExecuteReaderAsync())
                      {
                          while (await reader.ReadAsync())
                          {
                              currencies.Add(new Currency
                              {
                                  Id = reader.GetInt32("id"),
                                  name = reader.GetString("name")
                              });
                          }
                      }
                  }
              }
              catch (Exception ex)
              {
                  Console.WriteLine($"Error: {ex.Message}");
              }
              finally
              {
                  DbConnection.Deconnecter();
              }

              return currencies;
          }*/
        /**********************************/
        public void insertlinesAsync()
        {
            DbConnection.Connecter();
            string sqlCmd = "Delete from crm_opportunity_line where piece = " + this.Id + ";";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            try
            {
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            DbConnection.Deconnecter();
            foreach (var line in opportunity_lines)
            {
                string price = line.price.ToString().Replace(',', '.');
                string discount = line.discount.ToString().Replace(',', '.');
                string quantity = line.quantity.ToString().Replace(',', '.');
                string tax1_base = line.tax1_base.ToString().Replace(',', '.');
                string tax1_amount = line.tax1_amount.ToString().Replace(',', '.');
                string tax2_base = line.tax2_base.ToString().Replace(',', '.');
                string tax2_amount = line.tax2_amount.ToString().Replace(',', '.');
                string tax3_base = line.tax3_base.ToString().Replace(',', '.');
                string tax3_amount = line.tax3_amount.ToString().Replace(',', '.');
                string tax4_base = line.tax4_base.ToString().Replace(',', '.');
                string tax4_amount = line.tax4_amount.ToString().Replace(',', '.');
                string tax5_base = line.tax5_base.ToString().Replace(',', '.');
                string tax5_amount = line.tax5_amount.ToString().Replace(',', '.');

                sqlCmd = "INSERT INTO crm_opportunity_line SET description='" + line.description + "', price=" + price + ", discount=" + discount + ", piece=" + this.Id + ",product=" + line.IdProduct + ",quantity=" + quantity + ",tax1=" + (line.tax1 is uint ? (string)line.tax1.Value.ToString() : "NULL") + ",tax1_base=" + tax1_base + ",tax1_amount=" + tax1_amount + ",tax2=" + (line.tax2 is uint ? (string)line.tax2.Value.ToString() : "NULL") + ",tax2_base=" + tax2_base + ",tax2_amount=" + tax2_amount + ",tax3=" + (line.tax3 is uint ? (string)line.tax3.Value.ToString() : "NULL") + ",tax3_base=" + tax3_base + ",tax3_amount=" + tax3_amount + ",tax4=" + (line.tax4 is uint ? (string)line.tax4.Value.ToString() : "NULL") + ",tax4_base=" + tax4_base + ",tax4_amount=" + tax4_amount + ",tax5=" + (line.tax5 is uint ? (string)line.tax5.Value.ToString() : "NULL") + ",tax5_base=" + tax5_base + ",tax5_amount=" + tax5_amount + ";";
                cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                DbConnection.Connecter();
                try
                {

                    cmd.ExecuteScalar();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                DbConnection.Deconnecter();

            }
        }
        public string CreatCode()
        {
            string year = string.Empty;
            string sqlCmd = "SELECT prefix,separator1,year,separator2,final_number,separator3,suffixe FROM commercial_dialing where piece_type like 'CRM.Opportunity%';";

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adapter.Fill(dt);


            if (int.Parse(dt.Rows[0]["year"].ToString()) == 1)
                year = DateTime.Now.Year.ToString();

            int OPID = int.Parse(dt.Rows[0]["final_number"].ToString()) + 1;

            DbConnection.Deconnecter();
            DbConnection.Connecter();
            sqlCmd = "Update commercial_dialing Set final_number = final_number + 1 where piece_type like '%CRM.Opportunity%';";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            try
            {
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            DbConnection.Deconnecter();

            return _ = dt.Rows[0]["prefix"].ToString() + dt.Rows[0]["separator1"].ToString() + year + dt.Rows[0]["separator2"].ToString() + (int.Parse(dt.Rows[0]["final_number"].ToString())).ToString() + dt.Rows[0]["separator3"].ToString() + dt.Rows[0]["suffixe"].ToString();
        }
        public int Getstate()
        {
            int state = 0;
            string sqlCmd = "SELECT state FROM crm_opportunity_state " +
                "WHERE(opportunity = " + Id + ") AND(`date` = (SELECT MAX(`date`) AS Expr1 " +
                "FROM crm_opportunity_state crm_opportunity_state_1 " +
                "WHERE(opportunity = " + Id + ")))";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Deconnecter();
            DbConnection.Connecter();
            try
            {

                state = int.Parse(cmd.ExecuteScalar().ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();


            return state;

        }

        public void ModifyState(int newstate)
        {
            int NewState = 0;
            string sqlCmd = " INSERT INTO crm_opportunity_state SET create_date= NOW(), date= NOW(), opportunity=" + Id + ", state=" + newstate + ";SELECT MAX(Id) FROM crm_opportunity_state; ";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {

                NewState = int.Parse(cmd.ExecuteScalar().ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();
            sqlCmd = " UPDATE crm_opportunity SET state = " + NewState + " WHERE Id = " + Id + ";";
            cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {

                cmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();
        }
        public class Collection
        {
            public int Id { get; set; }
            public string code { get; set; }
            public DateTime create_date { get; set; }
            public string partnaireName { get; set; }
            public decimal total_amount { get; set; }
            public string ordreCode { get; set; }

            public  string GratiutéColor { get; set; }

            public string WholsealerColor { get; set; }

            public DateTime? ordreDate { get; set; }
            public bool? delivred { get; set; }
            public string delivredState
            {
                get
                {
                    if (delivred.HasValue)
                        if (delivred.Value)
                            return "Delivered";
                        else
                            return "Processing";
                    else
                        return "";
                }
            }
            public DateTime? delivred_date { get; set; }
            public string wholesalerName { get; set; }
            public string agentName { get; set; }
            public string parentCode { get; set; }

            public bool HasWholesaler { get; set; }

            public bool HasParent { get; set; }
            public string stateName { get; set; }
            public Microsoft.Maui.Graphics.Color stateColor
            {
                get
                {
                    switch (stateName)
                    {
                        case ("Gagnée"):
                            return Colors.Green;
                        case ("Perdue"):
                            return Colors.Red;
                        default: return Colors.Gray;
                    }
                }
            }
            public Microsoft.Maui.Graphics.Color wholesalerColor
            {
                get
                {
                    if (string.IsNullOrEmpty(wholesalerName))
                        return Colors.Transparent;
                    else
                        return Colors.Orange;

                }
            }
            public Microsoft.Maui.Graphics.Color gratuiteColor
            {
                get
                {
                    if (string.IsNullOrEmpty(parentCode))
                        return Colors.Transparent;
                    else
                        return Colors.GreenYellow;

                }
            }
            public Microsoft.Maui.Graphics.Color delivredColor
            {
                get
                {
                    if (delivredState == "Delivered")
                        return Colors.BlueViolet;
                    else
                        return Colors.Orange;

                }
            }


            public Collection() { }
            public Collection(int id, string code, DateTime create_date, string partnaireName, string agentName, decimal total_amount, string ordreCode, DateTime? ordreDate, bool? delivred, DateTime? delivred_date, string wholesalerName, string parentCode, string stateName)
            {
                Id = id;
                this.code = code;
                this.create_date = create_date;
                this.partnaireName = partnaireName;
                this.agentName = agentName;
                this.total_amount = total_amount;
                this.ordreCode = ordreCode;
                this.ordreDate = ordreDate;
                this.delivred = delivred;
                this.delivred_date = delivred_date;
                this.wholesalerName = wholesalerName;
                this.parentCode = parentCode;
                this.stateName = stateName;
                if(parentCode!= "" )
                {
                    this.HasParent= true;
                }
                if(wholesalerName != "")
                {
                    this.HasWholesaler = true;                }
            }

            public static MySqlCommand cmd;

            public async static Task<BindingList<Collection>> GetOpportunityByAgent(uint agentId)
            {
                BindingList<Collection> list = new BindingList<Collection>();
                string sqlCmd = "select crm_opportunity.Id, CONCAT(atooerp_person.first_name,' ',atooerp_person.last_name) as agentName,crm_opportunity.code ,crm_opportunity.create_date, commercial_partner.name as partnaireName, crm_opportunity.total_amount as total_amount, sale_order.code as ordreCode, sale_order.date as ordreDate, sale_order.delivred , sale_order.delivred_date, " +
                    "wholesaler.name as wholesalerName, parent.code as parentCode, crm_state.name stateName " +
                    "from crm_opportunity left join " +
                    "commercial_partner on commercial_partner.Id = crm_opportunity.partner left join " +
                    "atooerp_person on atooerp_person.Id = crm_opportunity.agent left join " +
                    "sale_order on sale_order.Id = crm_opportunity.`order` left join " +
                    "commercial_partner wholesaler on wholesaler.Id = crm_opportunity.dealer left join " +
                    "crm_opportunity parent on parent.Id = crm_opportunity.parent left join " +
                    "crm_opportunity_state on crm_opportunity.state = crm_opportunity_state.Id left join " +
                    "crm_state on crm_state.Id = crm_opportunity_state.state " +
                    "where( (crm_opportunity.agent = " + agentId + ") or (commercial_partner.sale_agent = " + agentId + "))" +
                    " order by crm_opportunity.Id desc ";

                MySqlDataReader reader = null;


                if (await DbConnection.Connecter3())
                {
                    try
                    {
                        cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {

                            list.Add(new Collection(
                                reader.GetInt32("Id"),
                                reader["code"].ToString(),
                                reader.GetDateTime("create_date"),
                                reader["partnaireName"].ToString(),
                                reader["agentName"].ToString(),
                                reader.GetDecimal("total_amount"),
                                reader["ordreCode"].ToString(),
                                reader["ordreDate"] is DateTime ? reader.GetDateTime("ordreDate") : (DateTime?)null,
                                reader["delivred"] is bool ? reader.GetBoolean("delivred") : (bool?)null,
                                reader["delivred_date"] is DateTime ? reader.GetDateTime("delivred_date") : (DateTime?)null,
                                reader["wholesalerName"].ToString(),
                                reader["parentCode"].ToString(),
                                reader["stateName"].ToString()));



                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        reader.Close();
                        return null;
                    }

                    reader.Close();

                }
                else
                {
                    return null;
                }

                return list;
            }
            public async static Task<BindingList<Collection>> GetOpportunities()
{
                BindingList<Collection> list = new BindingList<Collection>();

                if (await DbConnection.Connecter3())
                {
                    string sqlCmd = "select crm_opportunity.Id , CONCAT(atooerp_person.first_name,' ',atooerp_person.last_name) as agentName,crm_opportunity.code ,crm_opportunity.create_date, commercial_partner.name as partnaireName, crm_opportunity.total_amount as total_amount, sale_order.code as ordreCode, sale_order.date as ordreDate, sale_order.delivred , sale_order.delivred_date, " +
                    "wholesaler.name as wholesalerName, parent.code as parentCode, crm_state.name stateName " +
                    "from crm_opportunity left join " +
                    "commercial_partner on commercial_partner.Id = crm_opportunity.partner left join " +
                    "atooerp_person on atooerp_person.Id = crm_opportunity.agent left join " +
                    "sale_order on sale_order.Id = crm_opportunity.`order` left join " +
                    "commercial_partner wholesaler on wholesaler.Id = crm_opportunity.dealer left join " +
                    "crm_opportunity parent on parent.Id = crm_opportunity.parent left join " +
                    "crm_opportunity_state on crm_opportunity.state = crm_opportunity_state.Id left join " +
                    "crm_state on crm_state.Id = crm_opportunity_state.state " +
                    "order by crm_opportunity.Id desc ";
                    MySqlDataReader reader = null;
                    try
                    {
                        cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {

                            list.Add(new Collection(
                                reader.GetInt32("Id"),
                                reader["code"].ToString(),
                                reader.GetDateTime("create_date"),
                                reader["partnaireName"].ToString(),
                                reader["agentName"].ToString(),
                                reader.GetDecimal("total_amount"),
                                reader["ordreCode"].ToString(),
                                reader["ordreDate"] is DateTime ? reader.GetDateTime("ordreDate") : (DateTime?)null,
                                reader["delivred"] is bool ? reader.GetBoolean("delivred") : (bool?)null,
                                reader["delivred_date"] is DateTime ? reader.GetDateTime("delivred_date") : (DateTime?)null,
                                reader["wholesalerName"].ToString(),
                                reader["parentCode"].ToString(),
                                reader["stateName"].ToString()));
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        //await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "Ok");
                        reader.Close();
                        return null;
                    }


                    DbConnection.Deconnecter();
                    reader.Close();
                }
                else
                {
                    return null;
                }

                return list;

                //return new BindingList<Collection>();


            }

        }
        public class State
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public int opportunity { get; set; }
            public int state { get; set; }
            public string name
            {
                get
                {
                    switch (state)
                    {
                        case 1:
                            return "En Attente";
                        case 2:
                            return "Gangné";
                        case 3:
                            return "Perdu";
                        case 4:
                            return "BC";
                        case 5:
                            return "Livré";
                        case 6:
                            return "Facturé";
                        case 7:
                            return "Payeé";
                        default: return "None";
                    }
                }
            }
            public bool _default { get; set; }
            public State() { }

            public State(DateTime date, int State)
            {
                Date = date;
                state = State;
            }
            public static BindingList<State> getStateList(DataTable DT)
            {
                BindingList<State> list = new BindingList<State>();
                foreach (DataRow dr in DT.Rows)

                    list.Add(new State(
                        Convert.ToDateTime(dr["date_State"]),
                        Convert.ToInt32(dr["opp_State"].ToString()))
                        );
                try
                {
                    if (Convert.ToInt32(DT.Rows[0]["order"].ToString()) != 0)
                    {
                        list.Add(new State(
                            Convert.ToDateTime(DT.Rows[0]["orderDate"]), 4)
                            );

                        if (DT.Rows[0]["delivred"] != null && Convert.ToBoolean(DT.Rows[0]["delivred"]))
                        {
                            list.Add(new State(
                                Convert.ToDateTime(DT.Rows[0]["delivredDate"]), 5)
                                );

                            if (DT.Rows[0]["invoice"].ToString() != null && Convert.ToInt32(DT.Rows[0]["invoice"].ToString()) != 0)
                                list.Add(new State(
                                    Convert.ToDateTime(DT.Rows[0]["invoiceDate"]), 6)
                                    );
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


                return list;
            }

        }

    }
}

