using Acr.UserDialogs;
using MvvmHelpers;
using MySqlConnector;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using Color = System.Drawing.Color;

namespace SmartPharma5.Model
{
    public class Payment : ObservableObject
    {
        #region Attribut
        public int Id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public DateTime? create_date { get; set; }
        public DateTime? date { get; set; }
        private string Reference;
        public string reference { get => Reference; set => SetProperty(ref Reference, value); }
        private decimal Amount;
        public decimal amount { get => Amount; set => SetProperty(ref Amount, value); }
        public string memo { get; set; }
        public int IdPayment_method { get; set; }
        public DateTime? due_date { get; set; }
        public int IdPartner { get; set; }
        public int IdCurrency { get; set; }
        public uint? Currency { get; set; }
        public uint? GetCurrencyFromCashDesk()
        {
            return this.CashDesk?.Currency;
        }

        public int? DecimalNumber
        {
            get => _decimalNumber;
            set
            {
                SetProperty(ref _decimalNumber, value);
                OnPropertyChanged(nameof(AmountFormat)); // Met à jour dynamiquement l'affichage
            }
        }
        private int? _decimalNumber;
        public Cash_desk CashDesk { get; set; }
  /*      public async Task<Cash_desk> GetCashDeskAsync()
        {
            if (IdCashDesk > 0)
            {
                return await Cash_desk.getCash_deskById(IdCashDesk);
            }
            return null;
        }*/
        public int IdPayment_type { get; set; } //Affectation auto/manuelle ou par pieces
        public uint? IdCash_desk { get; set; }
        public bool ended { get; set; } //(0)
        public string PurchaseSale { get; set; }    //Purchase||Sale||HR
        public bool validated { get; set; }
        public string type { get { return "Payement"; } }
        public int bank_account { get; set; }  //(0)
        public int sale_bank { get; set; }
        public bool sign { get; set; } // (1)
        public uint? agent { get; set; }
        public BindingList<Payment_piece> Payment_pieceList { get; set; }
        #endregion
        #region Constructeur

        public Payment()
        {
            create_date = date = DateTime.Now;
            ended = false;
            sign = true;
            Payment_pieceList = new BindingList<Payment_piece>();
        }
        public Payment(int idagent, Partner partner)
        {
            IdPartner = (int)partner.Id;
            if (partner.PaymentMethodCustomer > 0)
                IdPayment_method = (int)partner.PaymentMethodCustomer;
            agent = (uint)idagent;
            create_date = date = due_date = DateTime.Now;
            ended = false;
            sign = true;
            Payment_pieceList = new BindingList<Payment_piece>();
        }
        public Payment(int Id)
        {
            DbConnection.ConnectionIsTrue();
            string sqlCmd = "SELECT * FROM commercial_payment WHERE Id=" + Id + ";";

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            try
            {
                this.Id = Id;
                this.code = dt.Rows[0]["code"].ToString();
                this.create_date = Convert.ToDateTime(dt.Rows[0]["create_date"]);
                this.date = Convert.ToDateTime(dt.Rows[0]["date"]);
                this.reference = dt.Rows[0]["reference"].ToString();
                this.Currency = dt.Rows[0]["currency"] != DBNull.Value ? Convert.ToUInt32(dt.Rows[0]["currency"]) : (uint?)null;

                // Déterminer le nombre de décimales en fonction de Currency
                if (Currency.HasValue)
                {
                    this.DecimalNumber = GetDecimalPlacesByCurrencyId(Currency.Value);
                }

                decimal rawAmount = Convert.ToDecimal(dt.Rows[0]["amount"]);
                this.amount = DecimalNumber.HasValue ? Math.Round(rawAmount, DecimalNumber.Value) : rawAmount;

                this.memo = dt.Rows[0]["memo"].ToString();
                this.IdPayment_method = Convert.ToInt32(dt.Rows[0]["payment_method"]);
                this.IdCash_desk = dt.Rows[0]["cash_desk"] is uint ? (uint)dt.Rows[0]["cash_desk"] : (uint?)null;
                this.IdPartner = Convert.ToInt32(dt.Rows[0]["partner"].ToString());
                this.PurchaseSale = dt.Rows[0]["piece_type"].ToString();
                this.due_date = string.IsNullOrEmpty(dt.Rows[0]["due_date"].ToString()) ?
                           (DateTime?)null : Convert.ToDateTime(dt.Rows[0]["due_date"].ToString());
                this.IdPayment_type = Convert.ToInt32(dt.Rows[0]["payment_type"]);
                this.ended = Convert.ToBoolean(dt.Rows[0]["ended"]);
                this.validated = Convert.ToBoolean(dt.Rows[0]["validated"]);
                this.bank_account = Convert.ToInt32(dt.Rows[0]["bank_account"]);
                this.sale_bank = Convert.ToInt32(dt.Rows[0]["sale_bank"]);
                this.sign = Convert.ToBoolean(dt.Rows[0]["sign"]);
                this.agent = dt.Rows[0]["agent"] is uint ? Convert.ToUInt32(dt.Rows[0]["agent"]) : (uint?)null;
                this.Payment_pieceList = new BindingList<Payment_piece>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            DbConnection.Deconnecter();
            GetPaymentPieceByPayment();
        }

        private int? GetDecimalPlacesByCurrencyId(uint currencyId)
        {
            int? decimalNumber = null;

            try
            {
                DbConnection.ConnectionIsTrue();

                string sqlCmd = "SELECT decimal_numbre FROM atooerp_currency WHERE Id = @CurrencyId;";

                using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@CurrencyId", currencyId);

                        if (DbConnection.con.State != ConnectionState.Open)
                        {
                            DbConnection.con.Open();
                        }

                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            decimalNumber = Convert.ToInt32(result);
                        }
                        else
                        {
                            throw new Exception($"Aucune valeur trouvée pour decimal_numbre avec Id = {currencyId}");
                        }
                    }
                    catch (MySqlException sqlEx)
                    {
                        throw new Exception("Erreur SQL lors de l'exécution de la requête SELECT sur atooerp_currency", sqlEx);
                    }
                    catch (InvalidCastException castEx)
                    {
                        throw new Exception("Erreur de conversion de decimal_numbre en int", castEx);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Erreur inconnue lors de l'exécution de la requête", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération de decimal_number", ex);
            }

            return decimalNumber;
        }

        public string AmountFormat
        {
            get
            {
                int decimalPlaces = DecimalNumber ?? 3;
                return $"F{decimalPlaces}";
            }
        }
        private int? GetDecimalNumberByCurrencyId(uint currencyId)
        {
            int? decimalNumber = null;

            try
            {
                DbConnection.ConnectionIsTrue();

                string sqlCmd = "SELECT decimal_numbre FROM atooerp_currency WHERE Id = @CurrencyId;";

                using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@CurrencyId", currencyId);

                        if (DbConnection.con.State != ConnectionState.Open)
                        {
                            DbConnection.con.Open();
                        }

                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            decimalNumber = Convert.ToInt32(result);
                        }
                        else
                        {
                            throw new Exception($"Aucune valeur trouvée pour decimal_numbre avec Id = {currencyId}");
                        }
                    }
                    catch (MySqlException sqlEx)
                    {
                        throw new Exception("Erreur SQL lors de l'exécution de la requête SELECT sur atooerp_currency", sqlEx);
                    }
                    catch (InvalidCastException castEx)
                    {
                        throw new Exception("Erreur de conversion de decimal_numbre en int", castEx);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Erreur inconnue lors de l'exécution de la requête", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération de decimal_number", ex);
            }

            return decimalNumber;
        }


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
                                     "', name = '" + ("payment").Replace("'", "''") + "'" +
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
        #endregion
        #region Insert
        public async void Insert()
        {
            this.code = CreatCode();
            string _Amount = this.amount.ToString().Replace(',', '.');
            string _dueDate = "null";
            if (this.due_date != null)
                _dueDate = "'" + Convert.ToDateTime(due_date).ToString("yyyy-MM-dd") + "'";
            string sqlCmd = "INSERT INTO commercial_payment SET " +
                "code = '" + code + "', " +
                "create_date = NOW(), " +
                "date = NOW(), " +
                "amount = " + _Amount + ", " +
                "memo = '" + memo + "', " +
                "payment_method = " + (int)IdPayment_method + ", " +
                "cash_desk = " + (int)IdCash_desk + ", " +
                "partner = " + (int)IdPartner + ", " +
                "name = (SELECT name FROM commercial_partner WHERE id = " + (int)IdPartner + "), " + // Récupération directe du nom
                "piece_type = 'Sale', " +
                "payment_type = " + (int)IdPayment_type + ", " +
                "ended = 0, " +
                "validated = " + false + ", " +
                "sale_bank = " + sale_bank + ", " +
                "sign = 1, " +
                "pos_session = 0, " +
                "reference = '" + reference + "', " +
                "due_date = " + _dueDate + ", " +
                "agent = " + agent + ", " +
                "currency = " + Currency + "; " +
                "SELECT MAX(Id) FROM " + DbConnection.Database + ".commercial_payment;"; MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {

                Id = int.Parse(cmd.ExecuteScalar().ToString());
                CurrentData.CurrentModuleId = Id;
                await Log("insert");
                foreach (Payment_piece piece in Payment_pieceList)
                {
                    piece.payment = Id;
                    piece.Insert();
                }
                DbConnection.Connecter();
                sqlCmd = "UPDATE commercial_payment SET validated = 1 where Id=" + Id + " ;";
                cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();
        }
        public bool Validate()
        {
            try
            {
                this.validated = true;
                if (Id == 0)
                    SetAmount();
                Insert();

                //else
                //update();
                //insertlines();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        #region Payment_piece
        public decimal getAssignedAmount()
        {
            decimal mount = 0;
            for (int i = 0; i < Payment_pieceList.Count; i++)
                mount = mount + Payment_pieceList[i].amount;
            return mount;
        }
        #endregion
        #region Functions
        public void GetPaymentPieceByPayment()
        {
            //DbConnection.Connecter();
            try
            {
                //Payment_pieceList.Clear();
                string sqlCmd = "SELECT pp.*, " +
                    "COALESCE(sale_order.code, sale_shipping.code, sale_invoice.code, sale_credit_invoice.code) AS code, " +
                    "commercial_dialing.name AS piece_typeName, " +
                    "COALESCE(sale_order.total_amount, sale_shipping.total_amount, sale_invoice.total_amount, sale_credit_invoice.total_amount) AS total_amount, " +
                    "COALESCE(sale_order.paied_amount, sale_shipping.paied_amount, sale_invoice.paied_amount, sale_credit_invoice.paied_amount) AS paied_amount, " +
                    "(COALESCE(sale_order.total_amount, sale_shipping.total_amount, sale_invoice.total_amount, sale_credit_invoice.total_amount) - " +
                    "COALESCE(sale_order.paied_amount, sale_shipping.paied_amount, sale_invoice.paied_amount, sale_credit_invoice.paied_amount)) AS restAmount, " +
                    "sale_invoice.currency AS Currency, " +  // Récupération de la devise
                    "atooerp_currency.symbol AS CurrencySymbol, " + // Récupération du symbole de la devise
                    "atooerp_currency.decimal_numbre AS DecimalNumber " + // Récupération de decimal_number
                    "FROM commercial_payment_piece pp " +
                    "LEFT JOIN commercial_dialing ON commercial_dialing.piece_type = pp.piece_type " +
                    "LEFT JOIN sale_order ON sale_order.Id = pp.piece AND pp.piece_type = 'Sale.Order, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' " +
                    "LEFT JOIN sale_shipping ON sale_shipping.Id = pp.piece AND pp.piece_type = 'Sale.Shipping, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' " +
                    "LEFT JOIN sale_invoice ON sale_invoice.Id = pp.piece AND pp.piece_type = 'Sale.Invoice, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' " +
                    "LEFT JOIN sale_credit_invoice ON sale_credit_invoice.Id = pp.piece AND pp.piece_type = 'Sale.Credit_invoice, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' " +
                    "LEFT JOIN atooerp_currency ON sale_invoice.currency = atooerp_currency.Id " + // Jointure pour récupérer le symbole
                    "WHERE pp.payment = " + this.Id + ";";


                DbConnection.Deconnecter();
                DbConnection.Connecter();

                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        Payment_pieceList.Add(new Payment_piece(
                            Convert.ToInt32(reader["Id"]),
                            Convert.ToInt32(reader["piece"]),
                            reader["piece_type"].ToString(),
                            reader["piece_typeName"].ToString(),
                            reader["code"].ToString(),
                            this.Id,
                            Convert.ToDecimal(reader["amount"]),
                            Convert.ToDecimal(reader["total_amount"]),
                            Convert.ToDecimal(reader["paied_amount"]),
                            Convert.ToDecimal(reader["restAmount"]),
                            reader["CurrencySymbol"] is DBNull ? "" : reader["CurrencySymbol"].ToString(),
                            reader["DecimalNumber"] is DBNull ? 0 : Convert.ToDecimal(reader["DecimalNumber"])
                                                                                                                           
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
        /*
         public void GetPaymentPieceByPayment()
   {
       //DbConnection.Connecter();
       try
       {
           //Payment_pieceList.Clear();
           string sqlCmd = "SELECT pp.*,coalesce(sale_order.code, sale_shipping.code, sale_invoice.code, sale_credit_invoice.code) as code ,commercial_dialing.name as piece_typeName ,coalesce(sale_order.total_amount, sale_shipping.total_amount, sale_invoice.total_amount, sale_credit_invoice.total_amount) as total_amount,coalesce(sale_order.paied_amount, sale_shipping.paied_amount, sale_invoice.paied_amount, sale_credit_invoice.paied_amount) as paied_amount,(coalesce(sale_order.total_amount, sale_shipping.total_amount, sale_invoice.total_amount, sale_credit_invoice.total_amount)-coalesce(sale_order.paied_amount, sale_shipping.paied_amount, sale_invoice.paied_amount, sale_credit_invoice.paied_amount)) as restAmount  " +
               "FROM commercial_payment_piece pp left join " +
               "commercial_dialing on commercial_dialing.piece_type=pp.piece_type left join  " +
               "sale_order on sale_order.Id = piece and pp.piece_type = 'Sale.Order, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' left join  " +
               "sale_shipping on sale_shipping.Id = piece and pp.piece_type = 'Sale.Shipping, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' left join  " +
               "sale_invoice on sale_invoice.Id = piece and pp.piece_type = 'Sale.Invoice, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' left join  " +
               "sale_credit_invoice on sale_credit_invoice.Id = piece and pp.piece_type = 'Sale.Credit_invoice, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' " +
               " Where payment=" + this.Id + ";";
           DbConnection.Deconnecter();
           DbConnection.Connecter();

           MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
           MySqlDataReader reader = cmd.ExecuteReader();

           while (reader.Read())
           {
               try
               {
                   Payment_pieceList.Add(new Payment_piece(
                       Convert.ToInt32(reader["Id"]),
                       Convert.ToInt32(reader["piece"]),
                       reader["piece_type"].ToString(),
                       reader["piece_typeName"].ToString(),
                       reader["code"].ToString(),
                       this.Id,
                       Convert.ToDecimal(reader["amount"]),
                       Convert.ToDecimal(reader["total_amount"]),
                       Convert.ToDecimal(reader["paied_amount"]),
                       Convert.ToDecimal(reader["restAmount"])
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
   }*/
        public static async Task<BindingList<Piece>> GetUnpaiedPiece(int partner)
          {
              string sqlCmd = "SELECT sale_balance.piece_type, sale_balance.piece_typeName, sale_balance.Id, sale_balance.code, sale_balance.reference, sale_balance.`Date`,    .IdPartner, sale_balance.partnerName, sale_balance.payment_conditionName, " +
                  "sale_balance.payment_methodName, sale_balance.total_amount, sale_balance.paied_amount, sale_balance.restAmount, sale_balance.due_date, commercial_partner.reference AS PartnerRef, commercial_partner.email AS Email,  " +
                  "commercial_partner_category.name AS PartnerCategory, sale_balance.pieceAgent, sale_balance.partnerAgent,if(sale_balance.piece_type like '%invoice%', 1, if(sale_balance.piece_type like '%shipping%',2,3)) as rang " +
                  "FROM     sale_balance LEFT OUTER JOIN " +
                  "commercial_partner ON sale_balance.IdPartner = commercial_partner.Id LEFT OUTER JOIN " +
                  "commercial_partner_category ON commercial_partner.category = commercial_partner_category.Id " +
                  "WHERE(FORMAT(sale_balance.restAmount, 3) <> 0) AND(sale_balance.IdPartner =" + partner + ") " +
                  "ORDER BY rang, due_date";
              BindingList<Piece> list = new BindingList<Piece>();

              DbConnection.Deconnecter();
              DbConnection.Connecter();

              MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
              MySqlDataReader reader = cmd.ExecuteReader();

              while (reader.Read())
              {
                  try
                  {
                      list.Add(new Piece(
                          Convert.ToInt32(reader["Id"]),
                          reader["code"].ToString(),
                          reader["reference"].ToString(),
                          Convert.ToDateTime(reader["Date"]),
                          Convert.ToInt32(reader["IdPartner"]),
                          reader["partnerName"].ToString(),
                          reader["payment_conditionName"].ToString(),
                          reader["payment_methodName"].ToString(),
                          Convert.ToDecimal(reader["total_amount"]),
                          Convert.ToDecimal(reader["paied_amount"]),
                          Convert.ToDecimal(reader["restAmount"]),
                          Convert.ToDateTime(reader["due_date"]),
                          reader["piece_type"].ToString(),
                          reader["piece_typeName"].ToString(),
                          reader["PartnerRef"].ToString(),
                          reader["Email"].ToString(),
                          reader["PartnerCategory"].ToString()));
                  }
                  catch (Exception ex)
                  {
                      await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                      await App.Current.MainPage.Navigation.PopAsync();
                  }

              }
              reader.Close();
              DbConnection.Deconnecter();


              return list;
          }

        /*   public static async Task<BindingList<Piece>> GetUnpaiedPiece(int partner, uint? currencyId)
           {
               string sqlCmd = "SELECT sale_balance.piece_type, sale_balance.piece_typeName, sale_balance.Id, sale_balance.code, sale_balance.reference, sale_balance.`Date`, sale_balance.IdPartner, sale_balance.partnerName, sale_balance.payment_conditionName, " +
                   "sale_balance.payment_methodName, sale_balance.total_amount, sale_balance.paied_amount, sale_balance.restAmount, sale_balance.due_date, commercial_partner.reference AS PartnerRef, commercial_partner.email AS Email,  " +
                   "commercial_partner_category.name AS PartnerCategory, sale_balance.pieceAgent, sale_balance.partnerAgent,if(sale_balance.piece_type like '%invoice%', 1, if(sale_balance.piece_type like '%shipping%',2,3)) as rang " +
                   "FROM sale_balance LEFT OUTER JOIN " +
                   "commercial_partner ON sale_balance.IdPartner = commercial_partner.Id LEFT OUTER JOIN " +
                   "commercial_partner_category ON commercial_partner.category = commercial_partner_category.Id " +
                   "WHERE (FORMAT(sale_balance.restAmount, 3) <> 0) AND (sale_balance.IdPartner = " + partner + ") " +
                   (currencyId.HasValue ? "AND (sale_balance.IdCurrency = " + currencyId.Value + ") " : "") +
                   "ORDER BY rang, due_date";

               BindingList<Piece> list = new BindingList<Piece>();

               DbConnection.Deconnecter();
               DbConnection.Connecter();

               MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
               MySqlDataReader reader = cmd.ExecuteReader();

               while (reader.Read())
               {
                   try
                   {
                       list.Add(new Piece(
                           Convert.ToInt32(reader["Id"]),
                           reader["code"].ToString(),
                           reader["reference"].ToString(),
                           Convert.ToDateTime(reader["Date"]),
                           Convert.ToInt32(reader["IdPartner"]),
                           reader["partnerName"].ToString(),
                           reader["payment_conditionName"].ToString(),
                           reader["payment_methodName"].ToString(),
                           Convert.ToDecimal(reader["total_amount"]),
                           Convert.ToDecimal(reader["paied_amount"]),
                           Convert.ToDecimal(reader["restAmount"]),
                           Convert.ToDateTime(reader["due_date"]),
                           reader["piece_type"].ToString(),
                           reader["piece_typeName"].ToString(),
                           reader["PartnerRef"].ToString(),
                           reader["Email"].ToString(),
                           reader["PartnerCategory"].ToString()));
                   }
                   catch (Exception ex)
                   {
                       await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                       await App.Current.MainPage.Navigation.PopAsync();
                   }
               }
               reader.Close();
               DbConnection.Deconnecter();

               return list;
           }*/
        public static async Task<BindingList<Piece>> GettotalUnpaiedPiece(int partner)
        {
            string sqlCmd = "SELECT sale_balance.piece_type, sale_balance.piece_typeName, sale_balance.Id, sale_balance.code, sale_balance.reference, sale_balance.`Date`, sale_balance.IdPartner, sale_balance.partnerName, sale_balance.payment_conditionName, " +
                "sale_balance.payment_methodName, sale_balance.total_amount, sale_balance.paied_amount, sale_balance.restAmount, sale_balance.due_date, commercial_partner.reference AS PartnerRef, commercial_partner.email AS Email,  " +
                "commercial_partner_category.name AS PartnerCategory, sale_balance.pieceAgent, sale_balance.partnerAgent,if(sale_balance.piece_type like '%invoice%', 1, if(sale_balance.piece_type like '%shipping%',2,3)) as rang " +
                "FROM     sale_balance LEFT OUTER JOIN " +
                "commercial_partner ON sale_balance.IdPartner = commercial_partner.Id LEFT OUTER JOIN " +
                "commercial_partner_category ON commercial_partner.category = commercial_partner_category.Id " +
                "WHERE(FORMAT(sale_balance.restAmount, 3) <> 0) AND(sale_balance.IdPartner =" + partner + ") " +
                "ORDER BY rang, due_date";
            BindingList<Piece> list = new BindingList<Piece>();

            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    list.Add(new Piece(
                        Convert.ToInt32(reader["Id"]),
                        reader["code"].ToString(),
                        reader["reference"].ToString(),
                        Convert.ToDateTime(reader["Date"]),
                        Convert.ToInt32(reader["IdPartner"]),
                        reader["partnerName"].ToString(),
                        reader["payment_conditionName"].ToString(),
                        reader["payment_methodName"].ToString(),
                        Convert.ToDecimal(reader["total_amount"]),
                        Convert.ToDecimal(reader["paied_amount"]),
                        Convert.ToDecimal(reader["restAmount"]),
                        Convert.ToDateTime(reader["due_date"]),
                        reader["piece_type"].ToString(),
                        reader["piece_typeName"].ToString(),
                        reader["PartnerRef"].ToString(),
                        reader["Email"].ToString(),
                        reader["PartnerCategory"].ToString()));
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                    await App.Current.MainPage.Navigation.PopAsync();
                }

            }
            reader.Close();
            DbConnection.Deconnecter();


            return list;
        }
        /*   public static async Task<BindingList<Piece>> GetUnpaiedPiece(int partner, uint? currencyId)
           {
               string sqlCmd = @"
       SELECT 
           sale_balance.piece_type, 
           sale_balance.piece_typeName, 
           sale_balance.Id, 
           sale_balance.code, 
           sale_balance.reference, 
           sale_balance.`Date`, 
           sale_balance.IdPartner, 
           sale_balance.partnerName, 
           sale_balance.payment_conditionName, 
           sale_balance.payment_methodName, 
           FORMAT(sale_balance.total_amount, sale_balance.decimal_numbre) AS total_amount, 
           sale_balance.paied_amount, 
           FORMAT(sale_balance.restAmount, sale_balance.decimal_numbre) AS restAmount,
           sale_balance.due_date, 
           commercial_partner.reference AS PartnerRef, 
           commercial_partner.email AS Email,  
           commercial_partner_category.name AS PartnerCategory, 
           sale_balance.pieceAgent, 
           sale_balance.partnerAgent,
           IF(sale_balance.piece_type LIKE '%invoice%', 1, IF(sale_balance.piece_type LIKE '%shipping%', 2, 3)) AS rang,
           sale_balance.decimal_numbre
       FROM 
           sale_balance 
           LEFT OUTER JOIN commercial_partner ON sale_balance.IdPartner = commercial_partner.Id 
           LEFT OUTER JOIN commercial_partner_category ON commercial_partner.category = commercial_partner_category.Id 
       WHERE 
           FORMAT(sale_balance.restAmount, 3) <> 0 
           AND sale_balance.IdPartner = @partner 
           " + (currencyId.HasValue ? "AND sale_balance.IdCurrency = @currencyId " : "") + @"
       ORDER BY 
           rang, due_date";

               BindingList<Piece> list = new BindingList<Piece>();

               DbConnection.Deconnecter();
               DbConnection.Connecter();

               using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
               {
                   cmd.Parameters.AddWithValue("@partner", partner);
                   if (currencyId.HasValue)
                   {
                       cmd.Parameters.AddWithValue("@currencyId", currencyId.Value);
                   }

                   using (MySqlDataReader reader = cmd.ExecuteReader())
                   {
                       while (reader.Read())
                       {
                           try
                           {
                               list.Add(new Piece(
                                   Convert.ToInt32(reader["Id"]),
                                   reader["code"].ToString(),
                                   reader["reference"].ToString(),
                                   Convert.ToDateTime(reader["Date"]),
                                   Convert.ToInt32(reader["IdPartner"]),
                                   reader["partnerName"].ToString(),
                                   reader["payment_conditionName"].ToString(),
                                   reader["payment_methodName"].ToString(),
                                   Convert.ToDecimal(reader["total_amount"]), // total_amount est déjà formaté
                                   Convert.ToDecimal(reader["paied_amount"]),
                                   Convert.ToDecimal(reader["restAmount"]),
                                   Convert.ToDateTime(reader["due_date"]),
                                   reader["piece_type"].ToString(),
                                   reader["piece_typeName"].ToString(),
                                   reader["PartnerRef"].ToString(),
                                   reader["Email"].ToString(),
                                   reader["PartnerCategory"].ToString(),
                                   Convert.ToInt32(reader["decimal_numbre"]) // Ajouter decimal_number
                               ));
                           }
                           catch (Exception ex)
                           {
                               await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                               await App.Current.MainPage.Navigation.PopAsync();
                           }
                       }
                   }
               }

               DbConnection.Deconnecter();

               return list;
           }*/
        /*  public static async Task<BindingList<Piece>> GetUnpaiedPiece(int partner, uint? currencyId)
          {
              string sqlCmd = @"
  SELECT 
      sale_balance.piece_type, 
      sale_balance.piece_typeName, 
      sale_balance.Id, 
      sale_balance.code, 
      sale_balance.reference, 
      sale_balance.`Date`, 
      sale_balance.IdPartner, 
      sale_balance.partnerName, 
      sale_balance.payment_conditionName, 
      sale_balance.payment_methodName, 
      FORMAT(sale_balance.total_amount, sale_balance.decimal_numbre) AS total_amount, 
      sale_balance.paied_amount, 
      FORMAT(sale_balance.restAmount, sale_balance.decimal_numbre) AS restAmount,
      sale_balance.due_date, 
      commercial_partner.reference AS PartnerRef, 
      commercial_partner.email AS Email,  
      commercial_partner_category.name AS PartnerCategory, 
      sale_balance.pieceAgent, 
      sale_balance.partnerAgent,
      IF(sale_balance.piece_type LIKE '%invoice%', 1, IF(sale_balance.piece_type LIKE '%shipping%', 2, 3)) AS rang,
      sale_balance.decimal_numbre
  FROM 
      sale_balance 
      LEFT OUTER JOIN commercial_partner ON sale_balance.IdPartner = commercial_partner.Id 
      LEFT OUTER JOIN commercial_partner_category ON commercial_partner.category = commercial_partner_category.Id 
  WHERE 
      FORMAT(sale_balance.restAmount, 3) <> 0 
      AND sale_balance.IdPartner = @partner 
  ORDER BY 
      rang, due_date";

              BindingList<Piece> list = new BindingList<Piece>();

              DbConnection.Deconnecter();
              DbConnection.Connecter();

              using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
              {
                  cmd.Parameters.AddWithValue("@partner", partner);

                  using (MySqlDataReader reader = cmd.ExecuteReader())
                  {
                      while (reader.Read())
                      {
                          try
                          {
                              list.Add(new Piece(
                                  Convert.ToInt32(reader["Id"]),
                                  reader["code"].ToString(),
                                  reader["reference"].ToString(),
                                  Convert.ToDateTime(reader["Date"]),
                                  Convert.ToInt32(reader["IdPartner"]),
                                  reader["partnerName"].ToString(),
                                  reader["payment_conditionName"].ToString(),
                                  reader["payment_methodName"].ToString(),
                                  Convert.ToDecimal(reader["total_amount"]), // total_amount est déjà formaté
                                  Convert.ToDecimal(reader["paied_amount"]),
                                  Convert.ToDecimal(reader["restAmount"]),
                                  Convert.ToDateTime(reader["due_date"]),
                                  reader["piece_type"].ToString(),
                                  reader["piece_typeName"].ToString(),
                                  reader["PartnerRef"].ToString(),
                                  reader["Email"].ToString(),
                                  reader["PartnerCategory"].ToString(),
                                  Convert.ToInt32(reader["decimal_numbre"]) // Ajouter decimal_number
                              ));
                          }
                          catch (Exception ex)
                          {
                              await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                              await App.Current.MainPage.Navigation.PopAsync();
                          }
                      }
                  }
              }

              DbConnection.Deconnecter();

              return list;
          }
        */
        public static async Task<BindingList<Piece>> GetUnpaiedPiece(int partner, uint? currencyId)
        {
            string sqlCmd = @"
SELECT 
    sale_balance.piece_type, 
    sale_balance.piece_typeName, 
    sale_balance.Id, 
    sale_balance.code, 
    sale_balance.reference, 
    sale_balance.`Date`, 
    sale_balance.IdPartner, 
    sale_balance.partnerName, 
    sale_balance.payment_conditionName, 
    sale_balance.payment_methodName, 
    FORMAT(sale_balance.total_amount, sale_balance.decimal_numbre) AS total_amount, 
    sale_balance.paied_amount, 
    FORMAT(sale_balance.restAmount, sale_balance.decimal_numbre) AS restAmount,
    sale_balance.due_date, 
    commercial_partner.reference AS PartnerRef, 
    commercial_partner.email AS Email,  
    commercial_partner_category.name AS PartnerCategory, 
    sale_balance.pieceAgent, 
    sale_balance.partnerAgent,
    IF(sale_balance.piece_type LIKE '%invoice%', 1, IF(sale_balance.piece_type LIKE '%shipping%', 2, 3)) AS rang,
    sale_balance.decimal_numbre,
    sale_balance.IdCurrency,
    sale_balance.currencySymbol
FROM 
    sale_balance 
    LEFT OUTER JOIN commercial_partner ON sale_balance.IdPartner = commercial_partner.Id 
    LEFT OUTER JOIN commercial_partner_category ON commercial_partner.category = commercial_partner_category.Id 
WHERE 
    FORMAT(sale_balance.restAmount, 3) <> 0 
    AND sale_balance.IdPartner = @partner 
ORDER BY 
    rang, due_date";

            BindingList<Piece> list = new BindingList<Piece>();

            DbConnection.Deconnecter();
            DbConnection.Connecter();

            using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
            {
                cmd.Parameters.AddWithValue("@partner", partner);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            // Nettoyer les chaînes total_amount et restAmount avant la conversion
                            string totalAmountString = reader["total_amount"].ToString();
                            totalAmountString = totalAmountString.Replace(",", ""); // Supprime les séparateurs de milliers

                            string restAmountString = reader["restAmount"].ToString();
                            restAmountString = restAmountString.Replace(",", ""); // Supprime les séparateurs de milliers

                            list.Add(new Piece(
                                Convert.ToInt32(reader["Id"]),
                                reader["code"].ToString(),
                                reader["reference"].ToString(),
                                Convert.ToDateTime(reader["Date"]),
                                Convert.ToInt32(reader["IdPartner"]),
                                reader["partnerName"].ToString(),
                                reader["payment_conditionName"].ToString(),
                                reader["payment_methodName"].ToString(),
                                decimal.Parse(totalAmountString, CultureInfo.InvariantCulture), // Convertir en decimal
                                Convert.ToDecimal(reader["paied_amount"]),
                                decimal.Parse(restAmountString, CultureInfo.InvariantCulture), // Convertir en decimal
                                Convert.ToDateTime(reader["due_date"]),
                                reader["piece_type"].ToString(),
                                reader["piece_typeName"].ToString(),
                                reader["PartnerRef"].ToString(),
                                reader["Email"].ToString(),
                                reader["PartnerCategory"].ToString(),
                                Convert.ToInt32(reader["decimal_numbre"]),
                                Convert.ToInt32(reader["IdCurrency"]),
                                reader["currencySymbol"].ToString() // Ajoutez IdCurrency ici
                            ));
                        }
                        catch (Exception ex)
                        {
                            await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                            await App.Current.MainPage.Navigation.PopAsync();
                        }
                    }
                }
            }

            DbConnection.Deconnecter();

            return list;
        }
        public static async Task<BindingList<Payment_method>> getPaymentMethodByUser(int iduser)
        {
            string sqlCmd = "SELECT distinct commercial_payment_method.Id, commercial_payment_method.name, commercial_payment_method.bank, commercial_payment_method.picture, commercial_payment_method.active, commercial_payment_method.payment_method_type FROM commercial_payment_method inner join accounting_cash_desk on accounting_cash_desk.payment_method= commercial_payment_method.Id left join accounting_user_cash_desk on accounting_user_cash_desk.cash_desk=accounting_cash_desk.Id where accounting_user_cash_desk.`user`=" + iduser + ";";
            BindingList<Payment_method> list = new BindingList<Payment_method>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    list.Add(new Payment_method(
                        Convert.ToInt32(reader["Id"]),
                        reader["name"].ToString()));
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                    await App.Current.MainPage.Navigation.PopAsync();
                }

            }
            reader.Close();
            DbConnection.Deconnecter();


            return list;
        }
        public static async Task<BindingList<Payment_method>> getPaymentMethod()
        {
            string sqlCmd = "SELECT Id, name, bank, picture, active, payment_method_type FROM commercial_payment_method;";
            BindingList<Payment_method> list = new BindingList<Payment_method>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    list.Add(new Payment_method(
                        Convert.ToInt32(reader["Id"]),
                        reader["name"].ToString()));
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                    await App.Current.MainPage.Navigation.PopAsync();
                }

            }
            reader.Close();
            DbConnection.Deconnecter();


            return list;
        }
        public static async Task<BindingList<Bank>> getBankList()
        {
            string sqlCmd = "SELECT Id, name FROM accounting_bank;";
            BindingList<Bank> Banklist = new BindingList<Bank>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    Banklist.Add(new Bank(
                        Convert.ToInt32(reader["Id"]),
                        reader["name"].ToString()));
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                    await App.Current.MainPage.Navigation.PopAsync();
                }

            }
            reader.Close();
            DbConnection.Deconnecter();


            return Banklist;
        }

        public static async Task<BindingList<Type>> getTypeList()
        {
            string sqlCmd = "SELECT Id, name FROM commercial_payment_type;";
            BindingList<Type> Typelist = new BindingList<Type>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    Typelist.Add(new Type(
                        Convert.ToInt32(reader["Id"]),
                        reader["name"].ToString()));
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                    await App.Current.MainPage.Navigation.PopAsync();
                }

            }
            reader.Close();
            DbConnection.Deconnecter();
            return Typelist;
        }
        public static async Task<BindingList<Cash_desk>> getCash_deskList()
        {
            string sqlCmd = "SELECT Id, name,principal FROM accounting_cash_desk;";
            BindingList<Cash_desk> Cash_desklist = new BindingList<Cash_desk>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    Cash_desklist.Add(new Cash_desk(
                        Convert.ToInt32(reader["Id"]),
                        reader["name"].ToString(),
                        Convert.ToBoolean(reader["principal"])));
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                    await App.Current.MainPage.Navigation.PopAsync();
                }

            }
            reader.Close();
            DbConnection.Deconnecter();
            return Cash_desklist;
        }
         public static BindingList<Cash_desk> getCash_deskListByUserAndPayment_methodAndPayment_type(int idUser, int IdPayment_method)
          {
              string sqlCmd = "SELECT accounting_cash_desk.Id, accounting_cash_desk.name, " +
                  " accounting_cash_desk.amount AS Montant, commercial_payment_method.name AS MethodePaiement, " +
                  " accounting_cash_desk.principal, accounting_cash_desk.sale AS Vente,  " +
                  "accounting_cash_desk.purchase AS Achat, accounting_cash_desk.hr AS Hr, accounting_cash_desk.bank AS Bank,  " +
                  "accounting_cash_desk.cash AS Cash, accounting_cash_desk.pos AS POS, accounting_cash_desk.activated AS Active,  " +
                  "atooerp_user.login AS Admin, accounting_cash_desk.currency AS Currency " + // Ajout de la colonne currency
                  "FROM accounting_cash_desk LEFT OUTER JOIN  " +
                  "accounting_user_cash_desk ON accounting_cash_desk.Id = accounting_user_cash_desk.cash_desk LEFT OUTER JOIN  " +
                  "accounting_user_cash_desk_type ON accounting_user_cash_desk_type.Id = accounting_user_cash_desk.`type` LEFT OUTER JOIN  " +
                  "atooerp_user ON accounting_user_cash_desk.`user` = atooerp_user.Id LEFT OUTER JOIN  " +
                  "commercial_payment_method ON accounting_cash_desk.payment_method = commercial_payment_method.Id  " +
                  "WHERE  (accounting_user_cash_desk.`user` = " + idUser + ") AND (accounting_cash_desk.activated = 1)  " +
                  "AND (accounting_user_cash_desk_type.payment=1)  " +
                  "AND (accounting_cash_desk.payment_method = " + IdPayment_method + ") AND (accounting_cash_desk.sale = 1)  " +
                  "GROUP BY accounting_cash_desk.Id ORDER BY accounting_cash_desk.Id;";
              BindingList<Cash_desk> Cash_desklist = new BindingList<Cash_desk>();
              DbConnection.Deconnecter();
              DbConnection.Connecter();

              MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
              MySqlDataReader reader = cmd.ExecuteReader();

              while (reader.Read())
              {
                  try
                  {
                      // Conversion de la colonne "Currency" en uint?
                      uint? currency = reader["Currency"] == DBNull.Value ? (uint?)null : Convert.ToUInt32(reader["Currency"]);

                      Cash_desklist.Add(new Cash_desk(
                          Convert.ToInt32(reader["Id"]),
                          reader["name"].ToString(),
                          Convert.ToBoolean(reader["principal"]),
                          currency)); // Ajout de la valeur de currency
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
        
        public static BindingList<Cash_desk> getCash_deskListByUserAndPaymentAndCurrency_methodAndPayment_type(int idUser, int IdPayment_method, uint? currencyId)
        {
            string sqlCmd = "SELECT accounting_cash_desk.Id, accounting_cash_desk.name, " +
                " accounting_cash_desk.amount AS Montant, commercial_payment_method.name AS MethodePaiement, " +
                " accounting_cash_desk.principal, accounting_cash_desk.sale AS Vente,  " +
                "accounting_cash_desk.purchase AS Achat, accounting_cash_desk.hr AS Hr, accounting_cash_desk.bank AS Bank,  " +
                "accounting_cash_desk.cash AS Cash, accounting_cash_desk.pos AS POS, accounting_cash_desk.activated AS Active,  " +
                "atooerp_user.login AS Admin, accounting_cash_desk.currency AS Currency " +
                "FROM accounting_cash_desk LEFT OUTER JOIN  " +
                "accounting_user_cash_desk ON accounting_cash_desk.Id = accounting_user_cash_desk.cash_desk LEFT OUTER JOIN  " +
                "accounting_user_cash_desk_type ON accounting_user_cash_desk_type.Id = accounting_user_cash_desk.`type` LEFT OUTER JOIN  " +
                "atooerp_user ON accounting_user_cash_desk.`user` = atooerp_user.Id LEFT OUTER JOIN  " +
                "commercial_payment_method ON accounting_cash_desk.payment_method = commercial_payment_method.Id  " +
                "WHERE  (accounting_user_cash_desk.`user` = " + idUser + ") AND (accounting_cash_desk.activated = 1)  " +
                "AND (accounting_user_cash_desk_type.payment=1)  " +
                "AND (accounting_cash_desk.payment_method = " + IdPayment_method + ") AND (accounting_cash_desk.sale = 1)  " +
                (currencyId.HasValue ? "AND (accounting_cash_desk.currency = " + currencyId.Value + ") " : "") +
                "GROUP BY accounting_cash_desk.Id ORDER BY accounting_cash_desk.Id;";

            BindingList<Cash_desk> Cash_desklist = new BindingList<Cash_desk>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    uint? currency = reader["Currency"] == DBNull.Value ? (uint?)null : Convert.ToUInt32(reader["Currency"]);

                    Cash_desklist.Add(new Cash_desk(
                        Convert.ToInt32(reader["Id"]),
                        reader["name"].ToString(),
                        Convert.ToBoolean(reader["principal"]),
                        currency));
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

        public static async Task<BindingList<Cash_desk>> getCash_deskListByUser(int idUser)
        {
            string sqlCmd = "SELECT accounting_cash_desk.Id, accounting_cash_desk.name, " +
                " accounting_cash_desk.amount AS Montant, commercial_payment_method.name AS MethodePaiement, " +
                " accounting_cash_desk.principal, accounting_cash_desk.sale AS Vente,  " +
                "accounting_cash_desk.purchase AS Achat, accounting_cash_desk.hr AS Hr, accounting_cash_desk.bank AS Bank,  " +
                "accounting_cash_desk.cash AS Cash, accounting_cash_desk.pos AS POS, accounting_cash_desk.activated AS Active,  " +
                "atooerp_user.login AS Admin  " +
                "FROM accounting_cash_desk LEFT OUTER JOIN  " +
                "accounting_user_cash_desk ON accounting_cash_desk.Id = accounting_user_cash_desk.cash_desk LEFT OUTER JOIN  " +
                "atooerp_user ON accounting_user_cash_desk.`user` = atooerp_user.Id LEFT OUTER JOIN  " +
                "commercial_payment_method ON accounting_cash_desk.payment_method = commercial_payment_method.Id  " +
                "WHERE  (accounting_user_cash_desk.`user` = " + idUser + ") AND (accounting_cash_desk.activated = 1)  " +
                "AND (accounting_cash_desk.sale = 1)  " +
                "GROUP BY accounting_cash_desk.Id ORDER BY accounting_cash_desk.Id;";
            BindingList<Cash_desk> Cash_desklist = new BindingList<Cash_desk>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    Cash_desklist.Add(new Cash_desk(
                         Convert.ToInt32(reader["Id"]),
                         reader["name"].ToString(),
                         Convert.ToBoolean(reader["principal"])));
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                    await App.Current.MainPage.Navigation.PopAsync();
                }

            }
            reader.Close();
            DbConnection.Deconnecter();
            return Cash_desklist;
        }
        public static BindingList<Cash_desk> getCash_deskListByPayment_method(int IdPayment_method)
        {
            string sqlCmd = "SELECT accounting_cash_desk.Id, accounting_cash_desk.name, " +
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
                "AND (accounting_cash_desk.payment_method = " + IdPayment_method + ")   " +
                "GROUP BY accounting_cash_desk.Id ORDER BY accounting_cash_desk.Id;";
            BindingList<Cash_desk> Cash_desklist = new BindingList<Cash_desk>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    Cash_desklist.Add(new Cash_desk(
                        Convert.ToInt32(reader["Id"]),
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
        public int CountReference()
        {
            int res = 0;
            string sqlCmd = "SELECT count(Id) as result FROM commercial_payment where reference = '" + reference + "' And sale_bank = " + sale_bank + " AND partner = " + this.IdPartner + " AND payment_method = " + this.IdPayment_method + "; ";

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            try
            {
                res = Convert.ToInt32(dt.Rows[0]["result"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return res;
        }
        public string CreatCode()
        {
            string year = string.Empty;
            string sqlCmd = "SELECT prefix,separator1,year,separator2,final_number,separator3,suffixe FROM commercial_dialing where piece_type like 'Commercial.Payment%';";

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adapter.Fill(dt);


            if (int.Parse(dt.Rows[0]["year"].ToString()) == 1)
                year = DateTime.Now.Year.ToString();

            int OPID = int.Parse(dt.Rows[0]["final_number"].ToString()) + 1;

            DbConnection.Deconnecter();
            DbConnection.Connecter();
            sqlCmd = "Update commercial_dialing Set final_number = final_number + 1 where piece_type like '%Commercial.Payment%';";
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
        public void SetAmount()
        {
            amount = 0;
            foreach (Payment_piece P in Payment_pieceList)
                amount += P.amount;
        }
        /*     public static async Task<BindingList<Collection>> GetCollectionList()
             {
                 BindingList<Collection> list = new BindingList<Collection>();
                 string sqlCmd = "SELECT commercial_payment.Id, commercial_payment.code, commercial_payment.create_date, commercial_partner.name AS Partner, " +
                     "commercial_payment_method.name AS PayementMethod, commercial_payment.reference, ROUND(commercial_payment.amount, " +
                     "(SELECT atooerp_currency.decimal_numbre " +
                     "FROM atooerp_currency " +
                     "WHERE(atooerp_currency.principal = 1))) as Amount, commercial_payment.due_date as Due_date, commercial_payment.ended as Ended, commercial_payment.validated as Validated,commercial_payment.agent as IdAgent,concat(atooerp_person.first_name,' ',atooerp_person.last_name) as NameAgent " +
                     "FROM commercial_payment LEFT OUTER JOIN " +
                     "commercial_payment_type ON commercial_payment.payment_type = commercial_payment_type.Id LEFT OUTER JOIN " +
                     "commercial_partner ON commercial_payment.partner = commercial_partner.Id LEFT OUTER JOIN " +
                     "commercial_payment_method ON commercial_payment.payment_method = commercial_payment_method.Id left join  " +
                     "atooerp_person on commercial_payment.agent=atooerp_person.Id " +
                     "WHERE(commercial_payment.piece_type = 'Sale') and((commercial_payment.validated = 0) or(commercial_payment.`date`>= (NOW() - INTERVAL 40 DAY)) or(commercial_payment.`date`>= (NOW() - INTERVAL 500 DAY))) " +
                     "ORDER BY commercial_payment.`date` DESC";
                 MySqlDataReader reader = null;

                 // Display loading indicator
                 using (var loading = UserDialogs.Instance.Loading("Loading please wait...", null, null, true, MaskType.Black))
                 {
                     if (DbConnection.Connecter())
                     {
                         try
                         {
                             MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                             reader = cmd.ExecuteReader();
                             while (reader.Read())
                             {
                                 list.Add(new Collection(
                                     reader.GetInt32("Id"),
                                     reader["code"].ToString(),
                                     reader.GetDateTime("create_date"),
                                     reader["Partner"].ToString(),
                                     reader["PayementMethod"].ToString(),
                                     reader["reference"].ToString(),
                                     reader.GetDecimal("Amount"),
                                     reader["Due_date"] is DateTime ? reader.GetDateTime("Due_date") : (DateTime?)null,
                                     Convert.ToBoolean(reader["Ended"]),
                                     Convert.ToBoolean(reader["Validated"]),
                                     reader["IdAgent"] is uint ? reader.GetUInt32("IdAgent") : (uint?)null,
                                     reader["NameAgent"].ToString()));
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
                 }

                 return list;
             }*/
        public static async Task<BindingList<Collection>> GetCollectionList()
        {
            BindingList<Collection> list = new BindingList<Collection>();
            string sqlCmd = "SELECT commercial_payment.Id, commercial_payment.code, commercial_payment.create_date, commercial_partner.name AS Partner, " +
                "commercial_payment_method.name AS PayementMethod, commercial_payment.reference, " +
                "FORMAT(commercial_payment.amount, atooerp_currency.decimal_numbre) as Amount, " + // Formater Amount selon decimal_numbre
                "commercial_payment.due_date as Due_date, commercial_payment.ended as Ended, commercial_payment.validated as Validated, " +
                "commercial_payment.agent as IdAgent, concat(atooerp_person.first_name,' ',atooerp_person.last_name) as NameAgent, " +
                "atooerp_currency.symbol AS CurrencySymbol, " + // Ajouter le symbole de la devise
                "atooerp_currency.decimal_numbre AS Numbre_decimal " + // Ajouter decimal_numbre pour référence
                "FROM commercial_payment " +
                "LEFT OUTER JOIN commercial_payment_type ON commercial_payment.payment_type = commercial_payment_type.Id " +
                "LEFT OUTER JOIN commercial_partner ON commercial_payment.partner = commercial_partner.Id " +
                "LEFT OUTER JOIN commercial_payment_method ON commercial_payment.payment_method = commercial_payment_method.Id " +
                "LEFT JOIN atooerp_person ON commercial_payment.agent = atooerp_person.Id " +
                "LEFT JOIN atooerp_currency ON commercial_payment.currency = atooerp_currency.Id " + // Jointure pour récupérer la devise
                "WHERE(commercial_payment.piece_type = 'Sale') " +
                "AND ((commercial_payment.validated = 0) OR (commercial_payment.`date` >= (NOW() - INTERVAL 40 DAY)) OR " +
                "(commercial_payment.`date` >= (NOW() - INTERVAL 500 DAY))) " +
                "ORDER BY commercial_payment.`date` DESC";

            MySqlDataReader reader = null;

            // Display loading indicator
            using (var loading = UserDialogs.Instance.Loading("Loading please wait...", null, null, true, MaskType.Black))
            {
                if (DbConnection.Connecter())
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            // Nettoyer la chaîne Amount avant la conversion
                            string amountString = reader["Amount"].ToString();
                            amountString = amountString.Replace(",", ""); // Supprime les séparateurs de milliers

                            list.Add(new Collection(
                                reader.GetInt32("Id"),
                                reader["code"].ToString(),
                                reader.GetDateTime("create_date"),
                                reader["Partner"].ToString(),
                                reader["PayementMethod"].ToString(),
                                reader["reference"].ToString(),
                                decimal.Parse(amountString, CultureInfo.InvariantCulture), // Convertir en decimal
                                reader["Due_date"] is DateTime ? reader.GetDateTime("Due_date") : (DateTime?)null,
                                Convert.ToBoolean(reader["Ended"]),
                                Convert.ToBoolean(reader["Validated"]),
                                reader["IdAgent"] is uint ? reader.GetUInt32("IdAgent") : (uint?)null,
                                reader["NameAgent"].ToString(),
                                // reader.GetUInt32("currency"), // Décommenter si nécessaire
                                reader["CurrencySymbol"].ToString() // Récupère le symbole de la devise
                            ));
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        reader?.Close();
                        UserDialogs.Instance.Alert($"Erreur: {ex.Message}", "Erreur", "OK");
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            return list;
        }

        /*  public static async Task<BindingList<Collection>> GetCollectionListByAgent(int agent)
          {
              BindingList<Collection> list = new BindingList<Collection>();
              string sqlCmd = "SELECT commercial_payment.Id, commercial_payment.code, commercial_payment.create_date, commercial_partner.name AS Partner, " +
                  "commercial_payment_method.name AS PayementMethod, commercial_payment.reference, ROUND(commercial_payment.amount, " +
                  "(SELECT atooerp_currency.decimal_numbre " +
                  "FROM atooerp_currency " +
                  "WHERE(atooerp_currency.principal = 1))) as Amount, commercial_payment.due_date as Due_date, commercial_payment.ended as Ended, commercial_payment.validated as Validated,commercial_payment.agent as IdAgent,concat(atooerp_person.first_name,' ',atooerp_person.last_name) as NameAgent " +
                  "FROM commercial_payment LEFT OUTER JOIN " +
                  "commercial_payment_type ON commercial_payment.payment_type = commercial_payment_type.Id LEFT OUTER JOIN " +
                  "commercial_partner ON commercial_payment.partner = commercial_partner.Id LEFT OUTER JOIN " +
                  "commercial_payment_method ON commercial_payment.payment_method = commercial_payment_method.Id left join  " +
                  "atooerp_person on commercial_payment.agent=atooerp_person.Id " +
                  "WHERE (commercial_payment.agent=" + agent + ") and (commercial_payment.piece_type = 'Sale') and((commercial_payment.validated = 0) or(commercial_payment.`date`>= (NOW() - INTERVAL 40 DAY)) or(commercial_payment.`date`>= (NOW() - INTERVAL 1000 DAY))) " +
                  "ORDER BY commercial_payment.`date` DESC";
              MySqlDataReader reader = null;

              // Display loading indicator
              using (var loading = UserDialogs.Instance.Loading("Loading please wait...", null, null, true, MaskType.Black))
              {
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
                                  reader["code"].ToString(),
                                  reader.GetDateTime("create_date"),
                                  reader["Partner"].ToString(),
                                  reader["PayementMethod"].ToString(),
                                  reader["reference"].ToString(),
                                  reader.GetDecimal("Amount"),
                                  reader["Due_date"] is DateTime ? reader.GetDateTime("Due_date") : (DateTime?)null,
                                  Convert.ToBoolean(reader["Ended"]),
                                  Convert.ToBoolean(reader["Validated"]),
                                  reader["IdAgent"] is uint ? reader.GetUInt32("IdAgent") : (uint?)null,
                                  reader["NameAgent"].ToString()
                                 // reader["symbol"].ToString()

                                  ));
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
              }

              return list;
          }*/
        /* public static async Task<BindingList<Collection>> GetCollectionListByAgent(int agent)
         {
             BindingList<Collection> list = new BindingList<Collection>();
             string sqlCmd = @"
 SELECT 
     commercial_payment.Id, 
     commercial_payment.currency,
     commercial_payment.code, 
     commercial_payment.create_date, 
     commercial_partner.name AS Partner, 
     commercial_payment_method.name AS PayementMethod, 
     commercial_payment.reference, 
     ROUND(commercial_payment.amount, 
         (SELECT atooerp_currency.decimal_numbre 
          FROM atooerp_currency 
          WHERE atooerp_currency.principal = 1)) as Amount, 
     commercial_payment.due_date as Due_date, 
     commercial_payment.ended as Ended, 
     commercial_payment.validated as Validated,
     commercial_payment.agent as IdAgent,
     CONCAT(atooerp_person.first_name, ' ', atooerp_person.last_name) as NameAgent,
     atooerp_currency.symbol as CurrencySymbol  -- Récupérer le symbole de la devise
 FROM 
     commercial_payment 
     LEFT OUTER JOIN commercial_payment_type ON commercial_payment.payment_type = commercial_payment_type.Id 
     LEFT OUTER JOIN commercial_partner ON commercial_payment.partner = commercial_partner.Id 
     LEFT OUTER JOIN commercial_payment_method ON commercial_payment.payment_method = commercial_payment_method.Id 
     LEFT JOIN atooerp_person ON commercial_payment.agent = atooerp_person.Id 
     LEFT JOIN atooerp_currency ON commercial_payment.currency = atooerp_currency.Id  -- Jointure avec atooerp_currency
 WHERE 
     commercial_payment.agent = @agent 
     AND commercial_payment.piece_type = 'Sale' 
     AND (commercial_payment.validated = 0 
          OR commercial_payment.`date` >= (NOW() - INTERVAL 40 DAY) 
          OR commercial_payment.`date` >= (NOW() - INTERVAL 1000 DAY))
 ORDER BY 
     commercial_payment.`date` DESC";
             MySqlDataReader reader = null;

             // Display loading indicator
             using (var loading = UserDialogs.Instance.Loading("Loading please wait...", null, null, true, MaskType.Black))
             {
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
                                 reader["code"].ToString(),
                                 reader.GetDateTime("create_date"),
                                 reader["Partner"].ToString(),
                                 reader["PayementMethod"].ToString(),
                                 reader["reference"].ToString(),
                                 reader.GetDecimal("Amount"),
                                 reader["Due_date"] is DateTime ? reader.GetDateTime("Due_date") : (DateTime?)null,
                                 Convert.ToBoolean(reader["Ended"]),
                                 Convert.ToBoolean(reader["Validated"]),
                                 reader["IdAgent"] is uint ? reader.GetUInt32("IdAgent") : (uint?)null,
                                 reader["NameAgent"].ToString(),
                                 reader.GetUInt32("currency")
                                 //reader["CurrencySymbol"].ToString()
                                 // reader["symbol"].ToString()

                                 ));
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
             }

             return list;
         }*/
        public static async Task<BindingList<Collection>> GetCollectionListByAgent(int agent)
        {
            BindingList<Collection> list = new BindingList<Collection>();
            string sqlCmd = "SELECT commercial_payment.Id, commercial_payment.currency, commercial_payment.code, commercial_payment.create_date, " +
                "commercial_partner.name AS Partner, commercial_payment_method.name AS PayementMethod, commercial_payment.reference, " +
                "FORMAT(commercial_payment.amount, atooerp_currency.decimal_numbre) as Amount, " + // Formatage selon decimal_numbre
                "commercial_payment.due_date as Due_date, commercial_payment.ended as Ended, commercial_payment.validated as Validated, " +
                "commercial_payment.agent as IdAgent, concat(atooerp_person.first_name,' ',atooerp_person.last_name) as NameAgent, " +
                "atooerp_currency.symbol AS CurrencySymbol, " + // Ajout du symbole de la devise
                "atooerp_currency.decimal_numbre AS Numbre_decimal " + // Ajout de decimal_numbre pour référence
                "FROM commercial_payment " +
                "LEFT OUTER JOIN commercial_payment_type ON commercial_payment.payment_type = commercial_payment_type.Id " +
                "LEFT OUTER JOIN commercial_partner ON commercial_payment.partner = commercial_partner.Id " +
                "LEFT OUTER JOIN commercial_payment_method ON commercial_payment.payment_method = commercial_payment_method.Id " +
                "LEFT JOIN atooerp_person ON commercial_payment.agent = atooerp_person.Id " +
                "LEFT JOIN atooerp_currency ON commercial_payment.currency = atooerp_currency.Id " + // Jointure pour récupérer la devise
                "WHERE (commercial_payment.agent = " + agent + ") AND (commercial_payment.piece_type = 'Sale') " +
                "AND ((commercial_payment.validated = 0) OR (commercial_payment.`date` >= (NOW() - INTERVAL 40 DAY)) OR " +
                "(commercial_payment.`date` >= (NOW() - INTERVAL 1000 DAY))) " +
                "ORDER BY commercial_payment.`date` DESC";

            MySqlDataReader reader = null;

            // Display loading indicator
            using (var loading = UserDialogs.Instance.Loading("Loading please wait...", null, null, true, MaskType.Black))
            {
                if (await DbConnection.Connecter3())
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            // Nettoyer la chaîne Amount avant la conversion
                            string amountString = reader["Amount"].ToString();
                            amountString = amountString.Replace(",", ""); // Supprime les séparateurs de milliers

                            list.Add(new Collection(
                                reader.GetInt32("Id"),
                                reader["code"].ToString(),
                                reader.GetDateTime("create_date"),
                                reader["Partner"].ToString(),
                                reader["PayementMethod"].ToString(),
                                reader["reference"].ToString(),
                                decimal.Parse(amountString, CultureInfo.InvariantCulture), // Convertir en decimal
                                reader["Due_date"] is DateTime ? reader.GetDateTime("Due_date") : (DateTime?)null,
                                Convert.ToBoolean(reader["Ended"]),
                                Convert.ToBoolean(reader["Validated"]),
                                reader["IdAgent"] is uint ? reader.GetUInt32("IdAgent") : (uint?)null,
                                reader["NameAgent"].ToString(),
                                reader.GetUInt32("currency"),
                                reader["CurrencySymbol"].ToString() // Récupère le symbole de la devise
                            ));
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        reader?.Close();
                        UserDialogs.Instance.Alert($"Erreur: {ex.Message}", "Erreur", "OK");
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            return list;
        }

        /*    public static async Task<BindingList<Collection>> GetCollectionListByAgent(int agent)
            {
                BindingList<Collection> list = new BindingList<Collection>();
                string sqlCmd = "SELECT commercial_payment.Id, commercial_payment.code, commercial_payment.create_date, commercial_partner.name AS Partner, " +
                    "commercial_payment_method.name AS PayementMethod, commercial_payment.reference, ROUND(commercial_payment.amount, " +
                    "(SELECT atooerp_currency.decimal_numbre " +
                    "FROM atooerp_currency " +
                    "WHERE(atooerp_currency.principal = 1))) as Amount, commercial_payment.due_date as Due_date, commercial_payment.ended as Ended, commercial_payment.validated as Validated, commercial_payment.agent as IdAgent, concat(atooerp_person.first_name,' ',atooerp_person.last_name) as NameAgent, commercial_payment.currency as Currency, atooerp_currency.symbol as CurrencySymbol " + // Ajout de CurrencySymbol
                    "FROM commercial_payment LEFT OUTER JOIN " +
                    "commercial_payment_type ON commercial_payment.payment_type = commercial_payment_type.Id LEFT OUTER JOIN " +
                    "commercial_partner ON commercial_payment.partner = commercial_partner.Id LEFT OUTER JOIN " +
                    "commercial_payment_method ON commercial_payment.payment_method = commercial_payment_method.Id LEFT JOIN " +
                    "atooerp_person ON commercial_payment.agent = atooerp_person.Id LEFT JOIN " +
                    "atooerp_currency ON commercial_payment.currency = atooerp_currency.code " + // Jointure avec atooerp_currency
                    "WHERE (commercial_payment.agent=" + agent + ") and (commercial_payment.piece_type = 'Sale') and((commercial_payment.validated = 0) or(commercial_payment.`date`>= (NOW() - INTERVAL 40 DAY)) or(commercial_payment.`date`>= (NOW() - INTERVAL 1000 DAY))) " +
                    "ORDER BY commercial_payment.`date` DESC";
                MySqlDataReader reader = null;

                // Display loading indicator
                using (var loading = UserDialogs.Instance.Loading("Loading please wait...", null, null, true, MaskType.Black))
                {
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
                                    reader["code"].ToString(),
                                    reader.GetDateTime("create_date"),
                                    reader["Partner"].ToString(),
                                    reader["PayementMethod"].ToString(),
                                    reader["reference"].ToString(),
                                    reader.GetDecimal("Amount"),
                                    reader["Due_date"] is DateTime ? reader.GetDateTime("Due_date") : (DateTime?)null,
                                    Convert.ToBoolean(reader["Ended"]),
                                    Convert.ToBoolean(reader["Validated"]),
                                    reader["IdAgent"] is uint ? reader.GetUInt32("IdAgent") : (uint?)null,
                                    reader["NameAgent"].ToString(),
                                    reader["Currency"].ToString(),
                                    reader["CurrencySymbol"].ToString()));
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
                }

                return list;
            }*/
        #endregion
        public class Piece : ObservableObject
        {
            #region Attribut
            public int Id { get; set; }
            public string code { get; set; }
            public string reference { get; set; }
            public DateTime date { get; set; }
            public int IdPartner { get; set; }
            public string partnerName { get; set; }
            public string paymentConditionName { get; set; }
            public string paymentMethodName { get; set; }
            public decimal total_amount { get; set; }
            public decimal paied_amount { get; set; }
            public decimal rest_amount { get; set; }
            public DateTime due_date { get; set; }
            public string piece_type { get; set; }
            public string piece_type_name { get; set; }
            public string partnerRef { get; set; }
            public string email { get; set; }
            public string partnerCategory { get; set; }
            public int DecimalNumbre { get; set; }
            public int IdCurrency { get; set; } // Ajoutez cette ligne
            public string currencySymbol { get; set; }

            private bool is_checked;
            public bool Is_checked { get => is_checked; set => SetProperty(ref is_checked, value); }
            public Color stateColor
            {
                get
                {
                    switch (piece_type)
                    {
                        case ("Sale.Invoice, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"):
                            return Color.FromArgb(255, 0, 0);
                        case ("Sale.Shipping, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"):
                            return Color.FromArgb(255, 127, 80);
                        case ("Sale.Credit_invoice, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"):
                            return Color.FromArgb(105, 105, 105);
                        case ("Sale.Order, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"):
                            return Color.FromArgb(255, 160, 122);
                        default: return Color.FromArgb(0, 0, 0);
                    }
                }
            }
            public Color due_dateColor
            {
                get
                {
                    if (due_date < DateTime.Now)
                        return Color.FromArgb(255, 0, 0);
                    else
                        return Color.FromArgb(0, 0, 0);
                }
            }
            #endregion
            #region Constructeur
            public Piece() { }
            public Piece(int id, string code, string reference, DateTime date, int IdPartner,
                string partnerName, string paymentConditionName, string paymentMethodName,
                decimal total_amount, decimal paied_amount, decimal rest_amount, DateTime due_date,
                string piece_type, string peece_type_name, string partnerRef, string email, string partnerCategory)
            {
                Id = id;
                this.code = code;
                this.reference = reference;
                this.date = date;
                this.IdPartner = IdPartner;
                this.partnerName = partnerName;
                this.paymentConditionName = paymentConditionName;
                this.paymentMethodName = paymentMethodName;
                this.total_amount = total_amount;
                this.paied_amount = paied_amount;
                this.rest_amount = rest_amount;
                this.due_date = due_date;
                this.piece_type = piece_type;
                this.piece_type_name = peece_type_name;
                this.partnerRef = partnerRef;
                this.email = email;
                this.partnerCategory = partnerCategory;
                this.Is_checked = false;
            }

            public Piece(int id, string code, string reference, DateTime date, int IdPartner,
       string partnerName, string paymentConditionName, string paymentMethodName,
       decimal total_amount, decimal paied_amount, decimal rest_amount, DateTime due_date,
       string piece_type, string peece_type_name, string partnerRef, string email, string partnerCategory, int decimalNumbre, int idCurrency, string currencySymbol) // Ajoutez idCurrency ici
            {
                Id = id;
                this.code = code;
                this.reference = reference;
                this.date = date;
                this.IdPartner = IdPartner;
                this.partnerName = partnerName;
                this.paymentConditionName = paymentConditionName;
                this.paymentMethodName = paymentMethodName;
                this.total_amount = total_amount;
                this.paied_amount = paied_amount;
                this.rest_amount = rest_amount;
                this.due_date = due_date;
                this.piece_type = piece_type;
                this.piece_type_name = peece_type_name;
                this.partnerRef = partnerRef;
                this.email = email;
                this.partnerCategory = partnerCategory;
                this.Is_checked = false;
                DecimalNumbre = decimalNumbre;
                IdCurrency = idCurrency; // Initialisez IdCurrency
                this.currencySymbol = currencySymbol;
            }
            /*   public Piece(int id, string code, string reference, DateTime date, int IdPartner,
               string partnerName, string paymentConditionName, string paymentMethodName,
               decimal total_amount, decimal paied_amount, decimal rest_amount, DateTime due_date,
               string piece_type, string peece_type_name, string partnerRef, string email, string partnerCategory, int decimalNumbre)
               {
                   Id = id;
                   this.code = code;
                   this.reference = reference;
                   this.date = date;
                   this.IdPartner = IdPartner;
                   this.partnerName = partnerName;
                   this.paymentConditionName = paymentConditionName;
                   this.paymentMethodName = paymentMethodName;
                   this.total_amount = total_amount;
                   this.paied_amount = paied_amount;
                   this.rest_amount = rest_amount;
                   this.due_date = due_date;
                   this.piece_type = piece_type;
                   this.piece_type_name = peece_type_name;
                   this.partnerRef = partnerRef;
                   this.email = email;
                   this.partnerCategory = partnerCategory;
                   this.Is_checked = false;
                   DecimalNumbre = decimalNumbre;
               }*/

            #endregion
        }
        public class Payment_piece : ObservableObject
        {
            #region Attribus
            public int Id { get; set; }
            public DateTime create_date { get; set; }
            //public uint Currency { get; set; }

            public int piece { get; set; }
            public string PieceCode { get; set; }
            public string piece_typeName { get; set; }
            public string piece_type { get; set; }
            public int payment { get; set; }
            public decimal amount { get; set; }
            public decimal Piece_totalAmont { get; set; }
            public decimal Piece_paiedAmount { get; set; }
            private decimal piece_restAmount;
            public uint? Currency { get; set; }
            public string CurrencySymbol { get; set; }
            public decimal DecimalNumber { get; set; }  

            public decimal Piece_restAmount
            {
                get => piece_restAmount; set => SetProperty(ref piece_restAmount, value);
            }
            #endregion
            #region Constructeur
            public Payment_piece() { }

           /* public Payment_piece(int id, int piece, string piece_type, string piece_typeName, string piece_code, int payment, decimal amount, decimal piece_totalAmont, decimal piece_paiedAmount, decimal piece_restAmount, string currencySymbol)
            {
                Id = id;
                this.create_date = DateTime.Now;
                this.piece = piece;
                this.piece_type = piece_type;
                this.piece_typeName = piece_typeName;
                this.PieceCode = piece_code;
                this.payment = payment;
                this.amount = amount;
               // Currency = currency;
                CurrencySymbol = currencySymbol;
                this.Piece_totalAmont = piece_totalAmont;
                if (Id > 0)
                    this.Piece_paiedAmount = piece_paiedAmount;
                else
                    this.Piece_paiedAmount = piece_paiedAmount + this.amount;
                if (Id > 0)
                    Piece_restAmount = piece_restAmount;
                else
                    Piece_restAmount = piece_restAmount - this.amount;
            }*/
            public Payment_piece(int id, int piece, string piece_type, string piece_typeName, string piece_code, int payment, decimal amount, decimal piece_totalAmont, decimal piece_paiedAmount, decimal piece_restAmount, string currencySymbol, decimal decimalNumber)
            {
                Id = id;
                this.create_date = DateTime.Now;
                this.piece = piece;
                this.piece_type = piece_type;
                this.piece_typeName = piece_typeName;
                this.PieceCode = piece_code;
                this.payment = payment;
                this.amount = amount;
                CurrencySymbol = currencySymbol;
                this.Piece_totalAmont = Math.Round(piece_totalAmont, (int)decimalNumber);  
                // Ajoute le nouveau champ decimalNumber au constructeur
                this.DecimalNumber = decimalNumber;

                if (Id > 0)
                    this.Piece_paiedAmount = piece_paiedAmount;
                else
                    this.Piece_paiedAmount = piece_paiedAmount + this.amount;

                if (Id > 0)
                    Piece_restAmount = Math.Round(piece_restAmount, (int)decimalNumber);
                else
                    Piece_restAmount = Math.Round(piece_restAmount - this.amount, (int)decimalNumber);

            }

            #endregion
            public void Insert()
            {
                string _Amount = this.amount.ToString().Replace(',', '.');
                string sqlCmd = "INSERT INTO commercial_payment_piece SET create_date= NOW(), amount=" + _Amount + ",payment=" + (int)payment + ",piece=" + (int)piece + ",piece_type='" + piece_type + "';SELECT MAX(Id) FROM " + DbConnection.Database + ".commercial_payment_piece;";
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                DbConnection.Connecter();
                try
                {

                    Id = int.Parse(cmd.ExecuteScalar().ToString());

                }
                catch
                {
                    Console.WriteLine("err");
                }
                DbConnection.Deconnecter();
            }
        }
        /*  public class currency
          {
              public int Id { get; set; }
              public string Name { get; set; }

              public currency() { }

              public currency(int id, string name)
              {
                  Id = id;
                  Name = name;
              }

              public static async Task<List<currency>> GetAllCurrencies()
              {
                  List<currency> currencies = new List<currency>();
                  string sqlCmd = "SELECT * FROM commercial_currency;";

                  MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                  adapter.SelectCommand.CommandType = CommandType.Text;
                  DataTable dt = new DataTable();
                  adapter.Fill(dt);

                  try
                  {
                      foreach (DataRow row in dt.Rows)
                      {
                          currencies.Add(new currency(
                              Convert.ToInt32(row["Id"]),
                              row["Name"].ToString()
                          ));
                      }
                  }
                  catch (Exception ex)
                  {
                      await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                  }

                  return currencies;
              }
          }*/
        public class Payment_method
        {
            public int Id { get; set; }
            public string name { get; set; }
            public Payment_method(int id, string name)
            {
                Id = id;
                this.name = name;
            }
            public Payment_method(int Id)
            {
                if (DbConnection.Connecter())
                {
                    string sqlCmd = "SELECT * FROM commercial_payment_method where Id=" + Id + ";";
                    try
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                        adapter.SelectCommand.CommandType = CommandType.Text;
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        this.Id = Id;
                        this.name = dt.Rows[0]["name"].ToString();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                DbConnection.Deconnecter();
            }
            public Payment_method()
            {
            }
            public static async Task<Payment_method> GetPayment_MethodById(int Id)
            {
                Payment_method payment_method = new Payment_method();
                string sqlCmd = "SELECT * FROM commercial_payment_method where Id=" + Id + ";";
                try
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    payment_method.Id = Id;
                    payment_method.name = dt.Rows[0]["name"].ToString();
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                }
                return payment_method;
            }
        }
        /*       public class Currency
               {
                   public int Id { get; set; }
                   public string name { get; set; }
                   public Currency(int id, string name)
                   {
                       Id = id;
                       this.name = name;
                   }
                   public Currency() { }

                   public Currency(int Id)
                   {

                       Currency currency = new Currency();
                       string sqlCmd = "SELECT * FROM commercial_currency where Id=" + Id + ";";
                       MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                       adapter.SelectCommand.CommandType = CommandType.Text;
                       DataTable dt = new DataTable();
                       adapter.Fill(dt);

                       try
                       {
                           this.Id = Id;
                           this.name = dt.Rows[0]["name"].ToString();
                       }
                       catch (Exception ex)
                       {
                           Console.WriteLine(ex.Message);
                       }


                   }

               }*/
        public class Bank
        {
            public int Id { get; set; }
            public string name { get; set; }
            public Bank(int id, string name)
            {
                Id = id;
                this.name = name;
            }
            public Bank() { }
            public Bank(int Id)
            {
                string sqlCmd = "SELECT * FROM accounting_bank where Id=" + Id + ";";

                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                adapter.SelectCommand.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                try
                {
                    this.Id = Id;
                    this.name = dt.Rows[0]["name"].ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            public static async Task<Bank> getBankById(int Id)
            {
                Bank bank = new Bank();
                string sqlCmd = "SELECT * FROM accounting_bank where Id=" + Id + ";";

                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                adapter.SelectCommand.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                try
                {
                    bank.Id = Id;
                    bank.name = dt.Rows[0]["name"].ToString();
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                }
                return bank;
            }
        }
        public class Type
        {
            public int Id { get; set; }
            public string name { get; set; }
            public Type(int id, string name)
            {
                Id = id;
                this.name = name;
            }
            public Type(int Id)
            {
                string sqlCmd = "SELECT * FROM commercial_payment_type where Id=" + Id + ";";

                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                adapter.SelectCommand.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                try
                {
                    this.Id = Id;
                    this.name = dt.Rows[0]["name"].ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            public Type() { }
            public static async Task<Type> GetTypeById(int Id)
            {
                Type type = new Type();
                string sqlCmd = "SELECT * FROM commercial_payment_type where Id=" + Id + ";";

                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                adapter.SelectCommand.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                try
                {
                    type.Id = Id;
                    type.name = dt.Rows[0]["name"].ToString();
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                }
                return type;
            }
        }

       /* public static async Task<List<Cash_desk>> GetCashDesksByCurrency(uint currencyId)
        {
            List<Cash_desk> cashDesks = new List<Cash_desk>();
            string sqlCmd = "SELECT * FROM accounting_cash_desk WHERE Currency = " + currencyId + ";";

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    Cash_desk cd = new Cash_desk
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        name = row["name"].ToString(),
                        principal = Convert.ToBoolean(row["principal"]),
                        Currency = row["Currency"] as uint?
                    };
                    cashDesks.Add(cd);
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
            }

            return cashDesks;
        }

        */
        public class Cash_desk
        {
            public int Id { get; set; }
            public string name { get; set; }
            public bool principal { get; set; }
            public uint? Currency { get; set; }
            public static uint? CurrencySelected { get; set; }
            public Cash_desk() { }
            public Cash_desk(int id, string name, bool principal)
            {
                Id = id;
                this.name = name;
                this.principal = principal;
            }

            public Cash_desk(int id, string name, bool principal, uint? currency)
            {
                Id = id;
                this.name = name;
                this.principal = principal;
                Currency = currency;
                CurrencySelected = currency;
            }
            public Cash_desk(int Id)
            {
                string sqlCmd = "SELECT * FROM accounting_cash_desk where Id=" + Id + ";";

                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                adapter.SelectCommand.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                try
                {
                    this.Id = Id;
                    this.name = dt.Rows[0]["name"].ToString();
                    this.principal = Convert.ToBoolean(dt.Rows[0]["principal"]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            public static async Task<Cash_desk> getCash_deskById(int Id)
            {
                Cash_desk cd = new Cash_desk();
                string sqlCmd = "SELECT * FROM accounting_cash_desk where Id=" + Id + ";";

                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                adapter.SelectCommand.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                try
                {
                    cd.Id = Id;
                    cd.name = dt.Rows[0]["name"].ToString();
                    cd.principal = Convert.ToBoolean(dt.Rows[0]["principal"]);
                    cd.Currency = dt.Rows[0]["Currency"] as uint?;
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                }
                return cd;
            }
            public static async Task<List<Cash_desk>> GetCashDesksByCurrency(uint? currencyId)
            {
                List<Cash_desk> cashDesks = new List<Cash_desk>();
                string sqlCmd = "SELECT * FROM accounting_cash_desk WHERE Currency = @Currency;";

                // Utilisez la chaîne de connexion pour créer une nouvelle instance de MySqlConnection
                using (MySqlConnection conn = new MySqlConnection(DbConnection.ConnectionString)) // Assurez-vous que DbConnection.ConnectionString est une chaîne de connexion valide
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@Currency", currencyId);
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                cashDesks.Add(new Cash_desk
                                {
                                    Id = reader.GetInt32("Id"),
                                    name = reader.GetString("name"),
                                    principal = reader.GetBoolean("principal"),
                                    Currency = reader.IsDBNull("Currency") ? (uint?)null : reader.GetUInt32("Currency")
                                });
                            }
                        }
                    }
                }

                return cashDesks;
            }

        }

        public class Collection
        {
            public int Id { get; set; }
            public string code { get; set; }
            public DateTime date { get; set; }
            public string Partner { get; set; }
            public string PayementMethod { get; set; }
            public string reference { get; set; }
            public decimal Amount { get; set; }
            public DateTime? Due_date { get; set; }
            public bool Ended { get; set; }
            public bool Validated { get; set; }
            public uint? IdAgent { get; set; }
            public string NameAgent { get; set; }
            public uint? CurrencyId { get; set; }
            public uint Currency { get; set; }
            public string CurrencySymbol { get; set; }
            //public string symbol { get; set; }// Symbole de la devise ($, €, etc.)
            public int DecimalNumbre { get; set; }
            private int? decimalPlaces;
            public string AmountFormat
            {
                get
                {
                    if (!decimalPlaces.HasValue)
                    {
                        decimalPlaces = GetDecimalPlacesByCurrencyIdlist(CurrencyId);
                    }
                    return Amount.ToString($"N{decimalPlaces.Value}");
                }
            }
            private int GetDecimalPlacesByCurrencyIdlist(uint? currencyId)
            {
                if (!currencyId.HasValue) return 3; // Valeur par défaut

                try
                {
                    DbConnection.ConnectionIsTrue();
                    string sqlCmd = "SELECT decimal_numbre FROM atooerp_currency WHERE Id = @CurrencyId;";

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        cmd.Parameters.AddWithValue("@CurrencyId", currencyId.Value);

                        if (DbConnection.con.State != ConnectionState.Open)
                        {
                            DbConnection.con.Open();
                        }

                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            throw new Exception($"Aucune valeur trouvée pour decimal_numbre avec Id = {currencyId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de la récupération du nombre de décimales : " + ex.Message);
                    return 2; // Valeur par défaut en cas d'erreur
                }
                finally
                {
                    DbConnection.Deconnecter();
                }
            }
            public Collection()
            {
            }
            /*public Collection(int id, string code, DateTime date, string partner, string payementMethod, string reference, decimal amount, DateTime? due_date, bool ended, bool validated, uint? idAgent, string nameAgent)
            {
                Id = id;
                this.code = code;
                this.date = date;
                Partner = partner;
                PayementMethod = payementMethod;
                this.reference = reference;
                Amount = amount;
                Due_date = due_date;
                Ended = ended;
                Validated = validated;
                IdAgent = idAgent;
                NameAgent = nameAgent;

            }*/
            public Collection(int id, string code, DateTime date, string partner, string payementMethod, string reference, decimal amount,
             DateTime? due_date, bool ended, bool validated, uint? idAgent, string nameAgent, string currencySymbol)
            {
                Id = id;
                this.code = code;
                this.date = date;
                Partner = partner;
                PayementMethod = payementMethod;
                this.reference = reference;
                Amount = amount;
                Due_date = due_date;
                Ended = ended;
                Validated = validated;
                IdAgent = idAgent;
                NameAgent = nameAgent;
                CurrencySymbol = currencySymbol;  // Stocke le symbole de la devise
            }
            public Collection(int id, string code, DateTime date, string partner, string payementMethod, string reference, decimal amount,
                DateTime? due_date, bool ended, bool validated, uint? idAgent, string nameAgent, uint currency, string currencySymbol)
            {
                Id = id;
                this.code = code;
                this.date = date;
                Partner = partner;
                PayementMethod = payementMethod;
                this.reference = reference;
                Amount = amount;
                Due_date = due_date;
                Ended = ended;
                Validated = validated;
                IdAgent = idAgent;
                NameAgent = nameAgent;
                Currency = currency;
                CurrencySymbol = currencySymbol;  // Stocke le symbole de la devise
            }

            /*  public Collection(int id, string code, DateTime date, string partner, string payementMethod, string reference, decimal amount, DateTime? due_date, bool ended, bool validated, uint? idAgent, string nameAgent, string currency, string currencySymbol)
              {
                  Id = id;
                  this.code = code;
                  this.date = date;
                  Partner = partner;
                  PayementMethod = payementMethod;
                  this.reference = reference;
                  Amount = amount;
                  Due_date = due_date;
                  Ended = ended;
                  Validated = validated;
                  IdAgent = idAgent;
                  NameAgent = nameAgent;
                  Currency = currency;
                  CurrencySymbol = currencySymbol; 
              }*/

        }
        public class currency
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public currency(int Id, string Name) {
                this.Id = Id;
                this.Name = Name;

            }
            public static List<currency> GetAllCurrencies()
            {
                List<currency> currencies = new List<currency>();

                try
                {
                    DbConnection.ConnectionIsTrue();
                    string sqlCmd = "SELECT Id, Name FROM atooerp_currency;";

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        if (DbConnection.con.State != ConnectionState.Open)
                        {
                            DbConnection.con.Open();
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                currencies.Add(new currency
                                    (reader.GetInt32("Id"),
                                    reader.GetString("Name")
                                    ));

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de la récupération des devises : " + ex.Message);
                }
                finally
                {
                    DbConnection.Deconnecter();
                }

                return currencies;
            }
        }
    }
}