//using Acr.UserDialogs;
using Acr.UserDialogs;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.View;
using SQLite;
using System.ComponentModel;
using System.Data;
using System.Globalization;
//using Xamarin.Forms;
//using static Xamarin.Essentials.Permissions;

namespace SmartPharma5.Model
{



    public class Partner
    {
        [PrimaryKey, Column("Id")]
        public uint Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Reference")]
        public string Reference { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }
        [Column("ChecSocity")]
        public bool ChecSocity { get; set; }
        [Column("Website")]
        public string Website { get; set; }
        [Column("Phone")]
        public string Phone { get; set; }
        public string FullAdress { get; }
        [Column("Mobile")]
        public string Mobile { get; set; }
        [Column("Fax")]
        public string Fax { get; set; }
        [Column("Email")]
        public string Email { get; set; }
        public Image Image { get; set; }
        public ImageSource Photo { get; }
        [Column("Note")]
        public string Note { get; set; }
        [Column("Customer")]
        public bool Customer { get; set; }
        [Column("Supplier")]
        public bool Supplier { get; set; }
        [Column("PaymentMethodSupplier")]
        public int PaymentMethodSupplier { get; set; }
        [Column("PaymentConditionSupplier")]
        public int PaymentConditionSupplier { get; set; }
        [Column("PaymentConditionCustomer")]
        public int PaymentConditionCustomer { get; set; }
        [Column("Socity")]
        public int Socity { get; set; }
        [Column("Number")]
        public string Number { get; set; }
        [Column("Street")]
        public string Street { get; set; }
        [Column("City")]
        public string City { get; set; }
        [Column("State")]
        public string State { get; set; }
        [Column("Country")]
        public string Country { get; set; }
        [Column("PostalCode")]
        public string PostalCode { get; set; }
        [Column("DeliveryNumber")]
        public string DeliveryNumber { get; set; }
        [Column("DeliveryStreet")]
        public string Gps { get; set; }
        [Column("Gps")]
        public string DeliveryStreet { get; set; }
        [Column("DeliveryCity")]
        public string DeliveryCity { get; set; }
        [Column("DeliveryState")]
        public string DeliveryState { get; set; }
        [Column("DeliveryCountry")]
        public string DeliveryCountry { get; set; }
        [Column("DeliveryPostalCode")]
        public string DeliveryPostalCode { get; set; }
        [Column("CreditLimit")]
        public decimal CreditLimit { get; set; }
        [Column("Currency")]
        public uint Currency { get; set; }
        public string CurrencyName { get; set; }

        [Column("JobPosition")]
        public string JobPosition { get; set; }
        [Column("CustomsCode")]
        public string CustomsCode { get; set; }
        [Column("VatCode")]
        public string VatCode { get; set; }
        [Column("TradeRegister")]
        public string TradeRegister { get; set; }
        [Column("Picture")]
        public byte[] Picture { get; set; }
        [Column("PaymentMethodCustomer")]
        public uint PaymentMethodCustomer { get; set; }
        [Column("RestAmount")]
        public decimal RestAmount { get; set; }
        [Column("DueDate")]
        public DateTime DueDate { get; set; }
        [Column("Actif")]
        public bool Actif { get; set; }
        [Column("CustomerDiscount")]
        public decimal CustomerDiscount { get; set; }
        [Column("SupplierDiscount")]
        public decimal SupplierDiscount { get; set; }
        [Column("VatExemption")]
        public bool VatExemption { get; set; }
        [Column("CustumerWithholdingTax")]
        public uint CustumerWithholdingTax { get; set; }
        [Column("SupplierWithholdingTax")]
        public uint SupplierWithholdingTax { get; set; }
        [Column("Activity")]
        public string Activity { get; set; }
        [Column("Category")]
        public uint Category { get; set; }
        public string Category_Name { get; set; }
        public decimal? Rest { get; set; }
        public DateTime? Due_date { get; set; }

        public decimal? Unpaied_invoice { get; set; }

        public decimal? Unpaied_invoice_due { get; set; }

        public AsyncCommand ShowProfile { get; set; }
        public AsyncCommand ShowContacts { get; set; }

        public AsyncCommand ShowForms { get; set; }
        public static List<(uint Id, string Name)> AllCurrencies { get; set; } = new List<(uint, string)>();
        private async Task Button_Clicked()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await App.Current.MainPage.Navigation.PushAsync(new ListContactsPartner(Convert.ToInt32(this.Id)));
            UserDialogs.Instance.HideLoading();

        }
        public Partner()
        {
            ShowContacts = new AsyncCommand(Button_Clicked);
            ShowProfile = new AsyncCommand(showProfileFonc);
        }
        public Partner(uint id, string name)
        {
            Id = id;
            Name = name;
            ShowProfile = new AsyncCommand(showProfileFonc);
            ShowContacts = new AsyncCommand(Button_Clicked);
            ShowForms = new AsyncCommand(showFormFonc);
        }
        public Partner(uint id, string name, string phone, string country, string email, string reference, int pcc, uint pmc, decimal? rest, DateTime? due_date, decimal? unpaied_invoice, decimal? unpaied_invoice_due, uint currency)
        {
            Id = id;
            Name = name;
            Phone = phone;
            Country = country;
            Email = email;
            Reference = reference;
            PaymentConditionCustomer = pcc;
            PaymentMethodCustomer = pmc;
            Rest = rest;
            Due_date = due_date;
            Unpaied_invoice = unpaied_invoice;
            Unpaied_invoice_due = unpaied_invoice_due;
            Currency = currency;
            ShowProfile = new AsyncCommand(showProfileFonc);
            ShowForms = new AsyncCommand(showFormFonc);
            ShowContacts = new AsyncCommand(Button_Clicked);

        }
        //currency


        public Partner(uint id, string name, string phone, string country, string email, string reference, int pcc, uint pmc, decimal? rest, DateTime? due_date, uint currency)
        {
            Id = id;
            Name = name;
            Phone = phone;
            Country = country;
            Email = email;
            Reference = reference;
            PaymentConditionCustomer = pcc;
            PaymentMethodCustomer = pmc;
            Rest = rest;
            Due_date = due_date;
            Currency = currency;
            ShowProfile = new AsyncCommand(showProfileFonc);
            ShowForms = new AsyncCommand(showFormFonc);
            ShowContacts = new AsyncCommand(Button_Clicked);


        }

        public Partner(uint id, string name, string phone, string country, string postale_code, string street, string state, string category_name, string vat_code, string fax, string number, uint currency)
        {
            Id = id;
            Name = name;
            Phone = phone;
            Country = country;
            State = state;
            PostalCode = postale_code;
            State = state;
            Street = street;
            Category_Name = category_name;
            VatCode = vat_code;
            Fax = fax;
            Number = number;
            Currency = currency;
            ShowProfile = new AsyncCommand(showProfileFonc);
            ShowForms = new AsyncCommand(showFormFonc);
            ShowContacts = new AsyncCommand(Button_Clicked);


        }
        public Partner(uint id, string name, string phone, string country, string postale_code, string street, string state, string category_name, string vat_code, string fax, string number, uint currency, string gps)
        {
            Id = id;
            Name = name;
            Phone = phone;
            Country = country;
            State = state;
            PostalCode = postale_code;
            State = state;
            Street = street;
            Category_Name = category_name;
            VatCode = vat_code;
            Fax = fax;
            Number = number;
            Currency = currency;
            Gps = gps;
            ShowProfile = new AsyncCommand(showProfileFonc);
            ShowForms = new AsyncCommand(showFormFonc);
            ShowContacts = new AsyncCommand(Button_Clicked);

        }

            public Partner(int id, string name, uint category, string phone, string street, string city, string postal_code, string state, string email, uint currency, ImageSource img, string category_name)
        {
            Id = (uint)id;
            Category = category;
            Name = name;
            Phone = phone;
            FullAdress = street + " " + city + " " + postal_code;
            State = state;
            Email = email;
            Currency = currency;
            Photo = img;
            Category_Name = category_name;              
            ShowProfile = new AsyncCommand(showProfileFonc);
            ShowContacts = new AsyncCommand(Button_Clicked);
            ShowForms = new AsyncCommand(showFormFonc);
        }

        public Partner(uint id, string name, string phone, string country, string email, string reference, int pcc, uint pmc, uint currency)
        {
            Id = id;
            Name = name;
            Phone = phone;
            Country = country;
            Email = email;
            Reference = reference;
            PaymentConditionCustomer = pcc;
            PaymentMethodCustomer = pmc;
            Currency = currency;
            ShowProfile = new AsyncCommand(showProfileFonc);
            ShowForms = new AsyncCommand(showFormFonc);
            ShowContacts = new AsyncCommand(Button_Clicked);
        }
      
      
        private async Task showFormFonc()
        {
            await App.Current.MainPage.Navigation.PushAsync(new FormListView(this));
        }
        private async Task showProfileFonc()
        {

            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await App.Current.MainPage.Navigation.PushAsync(new ProfileUpdate(Id));
            UserDialogs.Instance.HideLoading();

        }
        public static async Task<BindingList<Partner>> GetPartnaire()
        {
            string sqlCmd = "SELECT cp.Id,cp.name,cp.mobile,cp.country,cp.email,cp.reference,cp.payment_condition_customer," +
                    "cp.payment_method_customer,cp.currency,yy.due_date,yy.rest " +
                    "FROM commercial_partner cp Left Join " +
                    "(SELECT partner, sum(restAmount) as rest, min(due_date) as due_date " +
                    "from(SELECT  sale_balance.restAmount, sale_balance.due_date, commercial_partner.Id as partner " +
                    "FROM     commercial_partner LEFT OUTER JOIN sale_balance " +
                    "ON sale_balance.IdPartner = commercial_partner.Id LEFT OUTER JOIN " +
                    "commercial_partner_category ON commercial_partner.category = commercial_partner_category.Id " +
                    "WHERE(FORMAT(sale_balance.restAmount, 3) <> 0)) xx " +
                    "group by partner " +
                    "order by due_date ) yy on cp.Id = yy.partner " +
                    "WHERE(cp.customer = 1) AND(cp.chec_socity = 1) AND(cp.actif = 1)" +
                    " order by yy.due_date is null, yy.due_date asc;";

            BindingList<Partner> list = new BindingList<Partner>();

            // Display loading indicator
            using (var loading = UserDialogs.Instance.Loading("Loading please wait ...", null, null, true, MaskType.Black))
            {
                try
                {
                    if (await DbConnection.Connecter3())
                    {
                        MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    list.Add(new Partner(
                                        Convert.ToUInt32(reader["Id"]),
                                        reader["name"].ToString(),
                                        reader["mobile"].ToString(),
                                        reader["country"].ToString(),
                                        reader["email"].ToString(),
                                        reader["reference"].ToString(),
                                        int.Parse(reader["payment_condition_customer"].ToString()),
                                        Convert.ToUInt32(reader["payment_method_customer"]),
                                        reader["rest"] is decimal ? Convert.ToDecimal(reader["rest"]) : (decimal?)null,
                                        reader["due_date"] is DateTime ? Convert.ToDateTime(reader["due_date"].ToString()) : (DateTime?)null,
                                        reader["currency"] is null ? 0 : Convert.ToUInt32(reader["currency"])

                                    ));
                                }
                                catch (Exception ex)
                                {
                                    reader.Close();
                                    return null;
                                }
                            }
                        }

                        DbConnection.Deconnecter();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions as needed
                    return null;
                }
            }

            return list;
        }

        //--------------------------------------------------------------Get All Partner By Med Sahli --------------------------------------------------- 
        public async static Task<List<Partner>> GetPartnerListByAgent(uint id_agent)
        {
            List<Partner> list = new List<Partner>();
            ImageSource img = ImageSource.FromResource("@drawable/userregular.png");
            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "select commercial_partner.Id,commercial_partner.name,category,commercial_partner.phone,commercial_partner.street,commercial_partner.city,commercial_partner.postal_code,commercial_partner.state,commercial_partner.email,commercial_partner.currency,commercial_partner_category.name as category_name from commercial_partner\r\nleft join commercial_partner_category on commercial_partner_category.Id =  commercial_partner.category " +
                    "where not(customer=0 and supplier=1) and sale_agent = " + id_agent + ";";

                try
                {

                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        try
                        {
                            list.Add(new Partner(
                  Convert.ToInt32(reader["Id"]),
                  reader["name"] is null ? "" : reader["name"].ToString(),
                  reader["category"] is null ? 0 : Convert.ToUInt32(reader["category"]),
                  reader["phone"] is null ? "" : reader["phone"].ToString(),
                  reader["street"] is null ? "" : reader["street"].ToString(),
                  reader["city"] is null ? "" : reader["city"].ToString(),
                  reader["postal_code"] is null ? "" : reader["postal_code"].ToString(),
                  reader["state"] is null ? "" : reader["state"].ToString(),
                  reader["email"] is null ? "" : reader["email"].ToString(),
                  reader["currency"] is null ? 0 : Convert.ToUInt32(reader["currency"]),
                  img,
                   reader["category_name"] is null ? "" : reader["category_name"].ToString()));
                        }
                        catch (Exception ex)
                        {
                        }

                    }
                    reader.Close();
                }
                catch (Exception ex)
                {

                    reader.Close();
                    return null;
                    //App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "Ok");
                    //App.Current.MainPage.Navigation.PopAsync();
                }




            }
            else
            {

                reader.Close();
                return null;
                //App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "Ok");
                //App.Current.MainPage.Navigation.PopAsync();





            }

            return list;

        }
        public async static Task<List<Partner>> GetPartnerList()
        {
            List<Partner> list = new List<Partner>();
            ImageSource img = ImageSource.FromResource("@drawable/userregular.png");

            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "SELECT commercial_partner.Id, commercial_partner.name, " +
                                "category, commercial_partner.phone, commercial_partner.street, " +
                                "commercial_partner.city, commercial_partner.postal_code, " +
                                "commercial_partner.state, commercial_partner.email, " +
                                "commercial_partner.currency, " +  // Ajout de l'attribut currency
                                "commercial_partner_category.name AS category_name " +
                                "FROM commercial_partner " +
                                "LEFT JOIN commercial_partner_category ON " +
                                "commercial_partner_category.Id = commercial_partner.category " +
                                "WHERE NOT(customer=0 AND supplier=1);";


                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            try
                            {
                                int id = Convert.ToInt32(reader["Id"]);

                                list.Add(new Partner(
                                    id,
                                    reader["name"] is null ? "" : reader["name"].ToString(),
                                    reader["category"] is null ? 0 : Convert.ToUInt32(reader["category"]),
                                    reader["phone"] is null ? "" : reader["phone"].ToString(),
                                    reader["street"] is null ? "" : reader["street"].ToString(),
                                    reader["city"] is null ? "" : reader["city"].ToString(),
                                    reader["postal_code"] is null ? "" : reader["postal_code"].ToString(),
                                    reader["state"] is null ? "" : reader["state"].ToString(),
                                    reader["email"] is null ? "" : reader["email"].ToString(),
                                    reader["currency"] is null ? 0 : Convert.ToUInt32(reader["currency"]),
                                    img,
                                    reader["category_name"] is null ? "" : reader["category_name"].ToString()


                                ));
                            }
                            catch (Exception ex)
                            {

                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions here
                    return null;
                }
                finally
                {
                    // Ensure the reader is closed in case of an exception
                    DbConnection.con.Close();
                }
            }
            else
            {
                // Handle the case when the database connection fails
                return null;
            }

            return list;
        }
        /* public async static Task<List<Partner>> GetPartnerList()
         {
             List<Partner> list = new List<Partner>();
             ImageSource img = ImageSource.FromResource("@drawable/userregular.png");
             MySqlDataReader reader = null;
             if (await DbConnection.Connecter3())
             {
                 string sqlCmd = "select commercial_partner.Id,commercial_partner.name,category,commercial_partner.phone,commercial_partner.street,commercial_partner.city,commercial_partner.postal_code,commercial_partner.state,commercial_partner.email,commercial_partner_category.name as category_name from commercial_partner\r\nleft join commercial_partner_category on commercial_partner_category.Id =  commercial_partner.category where not(customer=0 and supplier=1);";

                 try
                 {

                     MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                     reader = cmd.ExecuteReader();

                     while (reader.Read())
                     {
                         int id = Convert.ToInt32(reader["Id"]);
                         try
                         {
                             list.Add(new Partner(
                   Convert.ToInt32(reader["Id"]),
                   reader["name"] is null ? "" : reader["name"].ToString(),
                   reader["category"] is null ? 0 : Convert.ToUInt32(reader["category"]),
                   reader["phone"] is null ? "" : reader["phone"].ToString(),
                   reader["street"] is null ? "" : reader["street"].ToString(),
                   reader["city"] is null ? "" : reader["city"].ToString(),
                   reader["postal_code"] is null ? "" : reader["postal_code"].ToString(),
                   reader["state"] is null ? "" : reader["state"].ToString(),
                   reader["email"] is null ? "" : reader["email"].ToString(),
                   img,
                    reader["category_name"] is null ? "" : reader["category_name"].ToString()));
                         }
                         catch (Exception ex)
                         {
                             reader.Close();
                             return null;

                         }

                     }
                     reader.Close();
                 }
                 catch (Exception ex)
                 {

                     reader.Close();
                    return null;
                     //App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "Ok");
                     //App.Current.MainPage.Navigation.PopAsync();
                 }




             }
             else
             {

                 reader.Close();
                 return null;
                 //App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "Ok");
                 //App.Current.MainPage.Navigation.PopAsync();





             }
             reader.Close();
             return list;

         }*/
        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public async static Task<List<Partner>> GetPartnaireForFormByIdAgent(uint idagent)
        {

            string sqlCmd = "select commercial_partner.Id,commercial_partner.name,category,commercial_partner.phone,commercial_partner.street,commercial_partner.city,commercial_partner.postal_code,commercial_partner.state,commercial_partner.currency,commercial_partner.email,commercial_partner_category.name as category_name from commercial_partner\r\nleft join commercial_partner_category on commercial_partner_category.Id =  commercial_partner.category ;";
            //where not(customer=0 and supplier=1)
            List<Partner> list = new List<Partner>();

            ImageSource img = ImageSource.FromResource("@drawable/userregular.png");

            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        try
                        {

                            list.Add(new Partner(
                           Convert.ToInt32(reader["Id"]),
                           reader["name"] is null ? "" : reader["name"].ToString(),
                           reader["category"] is null ? 0 : Convert.ToUInt32(reader["category"]),
                           reader["phone"] is null ? "" : reader["phone"].ToString(),
                           reader["street"] is null ? "" : reader["street"].ToString(),
                           reader["city"] is null ? "" : reader["city"].ToString(),
                           reader["postal_code"] is null ? "" : reader["postal_code"].ToString(),
                           reader["state"] is null ? "" : reader["state"].ToString(),                          
                           reader["email"] is null ? "" : reader["email"].ToString(),
                           reader["currency"] is null ? 0 : Convert.ToUInt32(reader["currency"]),
                           img,
                            reader["category_name"] is null ? "" : reader["category_name"].ToString()
                           ));
                        }
                        catch (Exception ex)
                        {

                        }

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
        // ajouter la récupération de la colonne currency
        public static async Task<BindingList<Partner>> GetPartnaireForPayment()
        {
            string sqlCmd = "SELECT cp.Id, cp.name, cp.mobile, cp.country, cp.email, cp.reference, cp.payment_condition_customer, cp.payment_method_customer, yy.due_date, " +
                            "yy.rest, yy.unpaied_invoice, yy.unpaied_invoice_due, cp.currency " + // Ajout de cp.currency 
                            "FROM commercial_partner cp " +
                            "LEFT JOIN (SELECT partner, sum(restAmount) as rest, sum(unpaied_invoice) as unpaied_invoice, sum(unpaied_invoice_due) as unpaied_invoice_due, min(due_date) as due_date " +
                            "           FROM (SELECT sale_balance.restAmount, " +
                            "                        if(piece_type = 'Sale.Invoice, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', restAmount, 0) as unpaied_invoice, " +
                            "                        if(piece_type = 'Sale.Invoice, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' AND date(sale_balance.due_date) < now(), restAmount, 0) as unpaied_invoice_due, " +
                            "                        sale_balance.due_date, commercial_partner.Id as partner " +
                            "                 FROM commercial_partner " +
                            "                 LEFT OUTER JOIN sale_balance ON sale_balance.IdPartner = commercial_partner.Id " +
                            "                 LEFT OUTER JOIN commercial_partner_category ON commercial_partner.category = commercial_partner_category.Id " +
                            "                 WHERE FORMAT(sale_balance.restAmount, 3) <> 0) xx " +
                            "           GROUP BY partner ORDER BY due_date) yy ON cp.Id = yy.partner " +
                            "WHERE cp.customer = 1 AND cp.chec_socity = 1 AND cp.actif = 1 " +
                            "ORDER BY yy.due_date IS NULL, yy.due_date ASC;";
            BindingList<Partner> list = new BindingList<Partner>();

            DbConnection.Deconnecter();
            if (await DbConnection.Connecter3())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        list.Add(new Partner(
                            Convert.ToUInt32(reader["Id"]),
                            reader["name"]?.ToString(),
                            reader["mobile"]?.ToString(),
                            reader["country"]?.ToString(),
                            reader["email"]?.ToString(),
                            reader["reference"]?.ToString(),
                            int.Parse(reader["payment_condition_customer"]?.ToString() ?? "0"),
                            Convert.ToUInt32(reader["payment_method_customer"]?.ToString() ?? "0"),
                            reader["rest"] != DBNull.Value ? Convert.ToDecimal(reader["rest"]) : (decimal?)null,
                            reader["due_date"] != DBNull.Value ? Convert.ToDateTime(reader["due_date"].ToString()) : (DateTime?)null,
                            reader["unpaied_invoice"] != DBNull.Value ? Convert.ToDecimal(reader["unpaied_invoice"]) : (decimal?)null,
                            reader["unpaied_invoice_due"] != DBNull.Value ? Convert.ToDecimal(reader["unpaied_invoice_due"]) : (decimal?)null,
                            reader["currency"] != DBNull.Value ? Convert.ToUInt32(reader["currency"]) : 0 // Conversion en uint
                        ));
                    }
                    catch (Exception ex)
                    {
                        // Gérer l'exception
                        Console.WriteLine("Erreur lors de la création d'un objet Partner : " + ex.Message);
                    }

                }
                reader.Close();
                DbConnection.Deconnecter();
            }
            else
            {

                return null;
            }

            return list;
        }
        public static async Task<BindingList<Partner>> GetPartnaireByIdAgent(uint idagent)
        {
            string sqlCmd = "SELECT cp.Id,cp.name,cp.mobile,cp.country,cp.email,cp.reference,cp.payment_condition_customer,cp.currency,cp.payment_method_customer,yy.due_date,\r\n\r\n       yy.rest, unpaied_invoice, unpaied_invoice_due\r\n\r\nFROM commercial_partner cp\r\n\r\nLeft Join (SELECT partner, sum(restAmount) as rest, sum(unpaied_invoice) as unpaied_invoice, sum(unpaied_invoice_due) as unpaied_invoice_due, min(due_date) as due_date\r\n\r\n           from(SELECT  sale_balance.restAmount,\r\n\r\n                        if(piece_type = 'Sale.Invoice, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' , restAmount, 0) as unpaied_invoice,\r\n\r\n                        if(piece_type = 'Sale.Invoice, Sale, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'  and date(sale_balance.due_date) < now(), restAmount, 0) as unpaied_invoice_due,\r\n\r\n                        sale_balance.due_date, commercial_partner.Id as partner\r\n\r\n                 FROM     commercial_partner\r\n\r\n                 LEFT OUTER JOIN sale_balance ON sale_balance.IdPartner = commercial_partner.Id\r\n\r\n                 LEFT OUTER JOIN commercial_partner_category ON commercial_partner.category = commercial_partner_category.Id\r\n\r\n                 WHERE(FORMAT(sale_balance.restAmount, 3) <> 0))\r\n\r\nxx group by partner order by due_date)  yy on cp.Id = yy.partner WHERE(cp.customer = 1) AND(cp.chec_socity = 1) AND(cp.actif = 1) AND (sale_agent = " + idagent + ") \r\n\r\norder by yy.due_date is null, yy.due_date asc;";

            BindingList<Partner> list = new BindingList<Partner>();

            DbConnection.Deconnecter();
            if (await DbConnection.Connecter3())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        list.Add(new Partner(
      Convert.ToUInt32(reader["Id"]),
      reader["name"]?.ToString(),
      reader["mobile"]?.ToString(),
      reader["country"]?.ToString(),
      reader["email"]?.ToString(),
      reader["reference"]?.ToString(),
      int.Parse(reader["payment_condition_customer"]?.ToString() ?? "0"),
      Convert.ToUInt32(reader["payment_method_customer"]?.ToString() ?? "0"),
      reader["rest"] != DBNull.Value ? Convert.ToDecimal(reader["rest"]) : (decimal?)null,
      reader["due_date"] != DBNull.Value ? Convert.ToDateTime(reader["due_date"].ToString()) : (DateTime?)null,
      reader["unpaied_invoice"] != DBNull.Value ? Convert.ToDecimal(reader["unpaied_invoice"]) : (decimal?)null,
      reader["unpaied_invoice_due"] != DBNull.Value ? Convert.ToDecimal(reader["unpaied_invoice_due"]) : (decimal?)null,
      reader["currency"] != DBNull.Value ? Convert.ToUInt32(reader["currency"]) : 0
  ));
                    }
                    catch (Exception ex)
                    {
                        reader.Close();
                        return null;
                        //await App.Current.MainPage.DisplayAlert("Warning", "Connetion Time out", "Ok");
                        //await App.Current.MainPage.Navigation.PopAsync();
                    }

                }
                reader.Close();
                DbConnection.Deconnecter();
            }
            else
            {

                return null;
            }

            return list;
        }
        public async static Task<Partner> GetCommercialPartnerById(int idpartner)
        {
            string sqlCmd = "SELECT partner.Id,partner.vat_code,partner.fax ,partner.category, partner.country , partner.mobile,partner.name, partner.email, partner.number, partner.phone, partner.postal_code , partner.street ,partner.currency, partner.state ,category.name category_name,category.id category FROM commercial_partner partner left join commercial_partner_category category on category.Id = partner.category  WHERE(partner.Id = " + (uint)idpartner + ");";
            Partner partner = new Partner();

            if (await DbConnection.Connecter3())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {
                            var name = reader["name"].ToString();
                            var country = reader["country"];
                            partner = new Partner(Convert.ToUInt32(reader["Id"]),
                            reader["name"].ToString(),
                            reader["mobile"].ToString(),
                            reader["country"].ToString(),
                            reader["postal_code"].ToString(),
                            reader["street"].ToString(),
                            reader["state"].ToString(),
                            reader["category_name"].ToString(),
                            reader["vat_code"].ToString(),
                            reader["fax"].ToString(),
                            reader["number"].ToString(),
                            reader["currency"] != DBNull.Value ? Convert.ToUInt32(reader["currency"]) : 0

                            );
                        partner.Category = Convert.ToUInt32(reader["category"]);

                    }
                    catch (Exception ex)
                    {


                    }
                }
                reader.Close();


            }
            return partner;

        }
        /* public async static Task<Partner> GetCommercialPartnerById(int idpartner)
         {
             string sqlCmd = "SELECT partner.Id, partner.vat_code, partner.fax, partner.category, partner.country, partner.mobile, partner.name, partner.email, partner.number, partner.phone, partner.postal_code, partner.street, partner.state, category.name category_name, category.id category FROM commercial_partner partner LEFT JOIN commercial_partner_category category ON category.Id = partner.category WHERE partner.Id = " + (uint)idpartner + ";";
             Partner partner = new Partner();

             if (await DbConnection.Connecter3())
             {
                 MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                 MySqlDataReader reader = cmd.ExecuteReader();

                 while (reader.Read())
                 {
                     try
                     {
                         partner = new Partner(
                             Convert.ToUInt32(reader["Id"]),
                             reader["name"].ToString(),
                             reader["mobile"].ToString(),
                             reader["country"].ToString(),
                             reader["postal_code"].ToString(),
                             reader["street"].ToString(),
                             reader["state"].ToString(),
                             reader["category_name"].ToString(),
                             reader["vat_code"].ToString(),
                             reader["fax"].ToString(),
                             reader["number"].ToString()
                         );

                         partner.Category = Convert.ToUInt32(reader["category"]);
                     }
                     catch (Exception ex)
                     {
                         Console.WriteLine($"Erreur lors de la récupération du partenaire : {ex.Message}");
                     }
                 }
                 reader.Close();
             }

             // Étape supplémentaire : mise à jour de la colonne DeliveryNumber avec les coordonnées GPS
             if (partner != null)
             {
                 try
                 {
                     var location = await Geolocation.GetLocationAsync();

                     if (location != null)
                     {
                         string formattedLatitude = location.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);
                         string formattedLongitude = location.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);

                         string deliveryNumber = $"{formattedLatitude},{formattedLongitude}";

                         // Mise à jour de la colonne DeliveryNumber
                         string updateCmd = "UPDATE commercial_partner SET DeliveryNumber = @deliveryNumber WHERE Id = @id";
                         MySqlCommand updateCommand = new MySqlCommand(updateCmd, DbConnection.con);
                         updateCommand.Parameters.AddWithValue("@deliveryNumber", deliveryNumber);
                         updateCommand.Parameters.AddWithValue("@id", partner.Id);

                         await updateCommand.ExecuteNonQueryAsync();

                         Console.WriteLine("Coordonnées GPS mises à jour avec succès.");
                     }
                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine($"Erreur lors de la mise à jour de la colonne DeliveryNumber : {ex.Message}");
                 }
             }

             return partner;
         }*/

        public static async Task<BindingList<Partner>> GetWholesalerList()
        {
            BindingList<Partner> list = new BindingList<Partner>();

            string sqlCmd = "SELECT Id,chec_socity,city,country,create_date,credit_limit,currency,customer,"
                         + " customs_code,delivery_city,delivery_country,delivery_number,delivery_postal_code,delivery_state,"
                         + " delivery_street,email,fax,job_position,mobile,name,note,number,"
                         + " payment_condition_customer,payment_condition_supplier,payment_method_customer,payment_method_supplier,"
                         + " phone,picture,postal_code,reference,socity,state,street,supplier,"
                         + " trade_register,vat_code,website,rest_amount,due_date,customer_discount,vat_exemption,"
                         + " custumer_withholding_tax,category"
                         + " FROM commercial_partner "
                         + " WHERE(chec_socity = 1) AND (actif = 1) AND (category = 2) OR (category = 3) OR (category = 4);";



            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.ConnectionString);
            adapter.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();

            adapter.Fill(dt);


            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new Partner(
                        uint.Parse(dr["Id"].ToString()),
                        dr["name"].ToString(),
                        dr["mobile"].ToString(),
                        dr["country"].ToString(),
                        dr["email"].ToString(),
                        dr["reference"].ToString(),
                        Convert.ToInt32(dr["payment_condition_customer"]),
                        Convert.ToUInt32(dr["payment_method_customer"].ToString()),
                        dr["currency"] is DBNull ? 0 : Convert.ToUInt32(dr["currency"])));
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Warning", ex.ToString(), "Ok");
                await App.Current.MainPage.Navigation.PopAsync();
            }
            return list;
        }

        /*   public static Task<Partner> GetCommercialPartnerByIdForPayment(int idpartner)
           {
               string sqlCmd = "SELECT Id,country,mobile,name, email, number, payment_condition_customer, payment_condition_supplier, payment_method_customer, payment_method_supplier, " +
                   "phone, reference FROM commercial_partner WHERE(Id = " + (uint)idpartner + ");";
               Partner partner = new Partner();
               DbConnection.Deconnecter();
               DbConnection.Connecter();

               MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

               MySqlDataReader reader = cmd.ExecuteReader();
               while (reader.Read())
               {
                   try
                   {
                       partner = new Partner(Convert.ToUInt32(reader["Id"]),
                           reader["name"].ToString(),
                           reader["mobile"].ToString(),
                           reader["country"].ToString(),
                           reader["email"].ToString(),
                           reader["reference"].ToString(),
                           int.Parse(reader["payment_condition_customer"].ToString()),
                           Convert.ToUInt32(reader["payment_method_customer"]));
                   }
                   catch (Exception ex)
                   {
                       Console.WriteLine(ex.Message);
                   }
               }

               DbConnection.Deconnecter();

               return Task.FromResult(partner);

           }*/
        public static Task<Partner> GetCommercialPartnerByIdForPayment(int idpartner)
        {
            string sqlCmd = "SELECT Id, country, mobile, name, email, number, payment_condition_customer, " +
                            "payment_condition_supplier, payment_method_customer, payment_method_supplier, " +
                            "phone, reference, currency FROM commercial_partner WHERE Id = " + (uint)idpartner + ";";

            Partner partner = new Partner();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    partner = new Partner(
                        Convert.ToUInt32(reader["Id"]),
                        reader["name"].ToString(),
                        reader["mobile"].ToString(),
                        reader["country"].ToString(),
                        reader["email"].ToString(),
                        reader["reference"].ToString(),
                        int.Parse(reader["payment_condition_customer"].ToString()),
                        Convert.ToUInt32(reader["payment_method_customer"]),
                        Convert.ToUInt32(reader["currency"]) // Devise récupérée
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            DbConnection.Deconnecter();
            return Task.FromResult(partner);
        }


        /*************Mettre à jour la colonne gps dans ta table commercial_partner(gps)*************/

        public static async Task UpdateGpsCoordinates(uint id_partner)
        {
            // Connexion à la base de données
            if (await DbConnection.Connecter3())
            {
                // Récupérer la position actuelle
                var location = await Geolocation.GetLocationAsync();

                if (location != null)
                {
                    // Formater les coordonnées GPS
                    string gpsCoordinates = $"{location.Latitude.ToString("F15", CultureInfo.InvariantCulture)},{location.Longitude.ToString("F15", CultureInfo.InvariantCulture)}";

                    string sqlCmd = "UPDATE commercial_partner SET gps = @Gps WHERE Id = @IdPartner;";

                    try
                    {

                        MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                        cmd.Parameters.AddWithValue("@Gps", gpsCoordinates);
                        cmd.Parameters.AddWithValue("@IdPartner", id_partner);

                        // Exécuter la commande
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("La position GPS a été mise à jour avec succès.");
                        }
                        else
                        {
                            Console.WriteLine("Aucune ligne mise à jour dans la base de données.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur lors de la mise à jour de Gps: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("La récupération de la position GPS a échoué.");
                }
            }
            else
            {
                Console.WriteLine("Échec de la connexion à la base de données.");
            }
        }
        public static async Task UpdateManualGpsCoordinates(uint id_partner, double latitude, double longitude)
        {
            // Connexion à la base de données
            if (await DbConnection.Connecter3())
            {
                // Formater les coordonnées GPS
                string gpsCoordinates = $"{latitude.ToString("F15", CultureInfo.InvariantCulture)},{longitude.ToString("F15", CultureInfo.InvariantCulture)}";

                string sqlCmd = "UPDATE commercial_partner SET gps = @Gps WHERE Id = @IdPartner;";

                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    cmd.Parameters.AddWithValue("@Gps", gpsCoordinates);
                    cmd.Parameters.AddWithValue("@IdPartner", id_partner);

                    // Exécuter la commande
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("La position GPS manuelle a été mise à jour avec succès.");
                    }
                    else
                    {
                        Console.WriteLine("Aucune ligne mise à jour dans la base de données.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la mise à jour de Gps: {ex.Message}");
                    throw; // Propager l'exception pour la gérer dans l'UI
                }
            }
            else
            {
                Console.WriteLine("Échec de la connexion à la base de données.");
                throw new Exception("Échec de la connexion à la base de données");
            }
        }

        /*************Récupérer la colonne gps dans ta table commercial_partner(gps)*************/
        public static async Task<string> GetExistingGpsCoordinates(uint id_partner)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "SELECT gps FROM commercial_partner WHERE Id = @IdPartner;";

                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    cmd.Parameters.AddWithValue("@IdPartner", id_partner);

                    var result = await cmd.ExecuteScalarAsync();
                    return result?.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des coordonnées GPS: {ex.Message}");
                    return null;
                }
            }
            return null;
        }
        /**************************************************************************************/
        public static async Task<string> GetGpsCoordinates(uint id_partner)
        {
            try
            {
                string gpsCoordinates = null;
                string sqlCmd = "SELECT gps FROM commercial_partner WHERE Id = @IdPartner;";

                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                cmd.Parameters.AddWithValue("@IdPartner", id_partner);
                DbConnection.Connecter();
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    gpsCoordinates = reader.GetString("gps");
                }

                reader.Close();
                return gpsCoordinates;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des coordonnées GPS: {ex.Message}");
                return null;
            }
            finally
            {
                DbConnection.Deconnecter();
            }
        }


        public async static Task<List<tempValueName>> GetCommercialPartnerTempName(int idpartner)
        {
            string sqlCmd = "SELECT *  FROM marketing_profile_attribut_value_temp WHERE partner = " + idpartner + " and attribut_name='name' and state=1;";
            List<tempValueName> partner = new List<tempValueName>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    partner.Add(new tempValueName(reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"])));


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            DbConnection.Deconnecter();

            return partner;

        }


        public async static Task<List<tempValueName>> GetCommercialPartnerTempState(int idpartner)
        {
            string sqlCmd = "SELECT *  FROM marketing_profile_attribut_value_temp WHERE partner = " + idpartner + " and attribut_name='state' and state=1;";
            List<tempValueName> partner = new List<tempValueName>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    partner.Add(new tempValueName(reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"])));


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            DbConnection.Deconnecter();

            return partner;

        }

        public async static Task<List<tempValueName>> GetCommercialPartnerTempStreet(int idpartner)
        {
            string sqlCmd = "SELECT *  FROM marketing_profile_attribut_value_temp WHERE partner = " + idpartner + " and attribut_name='street' and state=1;";
            List<tempValueName> partner = new List<tempValueName>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    partner.Add(new tempValueName(reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"])));


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            DbConnection.Deconnecter();

            return partner;

        }




        public async static Task<List<tempValueName>> GetCommercialPartnerTempCategory(int idpartner)
        {

            string sqlCmd = "SELECT  partner.create_date ,category.name  category_name FROM marketing_profile_attribut_value_temp  partner " +
                "left join commercial_partner_category category on category.Id = partner.int_value WHERE attribut_name='category' and partner=" + idpartner + " and state=1;";


            List<tempValueName> partner = new List<tempValueName>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    partner.Add(new tempValueName(reader["category_name"].ToString(), Convert.ToDateTime(reader["create_date"])));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            DbConnection.Deconnecter();

            return partner;

        }


        public static async Task updateNamePartner(int id, string name)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "insert into marketing_profile_attribut_value_temp(partner,attribut_name,string_value,user,create_date,state,employe) values (" + id + ",'name','" + name + "'," + user_contrat.iduser + " , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 1 , " + user_contrat.id_employe + ") ;";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    cmd.ExecuteScalar();
                    //UserDialogs.Instance.Alert("ADDED TO TEMP VALUES");


                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
        }

        public static async Task updateCategoryPartner(int id, int category)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "insert into marketing_profile_attribut_value_temp(partner,attribut_name,int_value,user,create_date,state,employe) values (" + id + ",'category'," + (category) + "," + user_contrat.iduser + " ,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 1 , " + user_contrat.id_employe + ") ;";
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                try
                {
                    cmd.ExecuteScalar();
                    //UserDialogs.Instance.Alert("ADDED TO TEMP VALUES");
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
        }


        public static async Task updateStreetPartner(int id, string street)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "insert into marketing_profile_attribut_value_temp(partner,attribut_name,string_value,user,create_date,state,employe) values (" + id + ",'street','" + street + "'," + user_contrat.iduser + " , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 1 , " + user_contrat.id_employe + ") ;";
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                try
                {
                    cmd.ExecuteScalar();
                    // UserDialogs.Instance.Alert("ADDED TO TEMP VALUES");
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
        }


        public static async Task updateCodePostalePartner(int id, string code_postale)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "insert into marketing_profile_attribut_value_temp(partner,attribut_name,string_value,user,create_date,state,employe) values (" + id + ",'postal_code','" + code_postale + "'," + user_contrat.iduser + " , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 1 , " + user_contrat.id_employe + ") ;";
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                try
                {
                    cmd.ExecuteScalar();
                    //UserDialogs.Instance.Alert("ADDED TO TEMP VALUES");
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
        }


        public static async Task updateStatePartner(int id, string state)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "insert into marketing_profile_attribut_value_temp(partner,attribut_name,string_value,user,create_date,state,employe) values (" + id + ",'state','" + state + "'," + user_contrat.iduser + " , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 1 , " + user_contrat.id_employe + ") ;";
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                try
                {
                    cmd.ExecuteScalar();
                    // UserDialogs.Instance.Alert("ADDED TO TEMP VALUES");
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
        }


        public static async Task updateCountryPartner(int id, string country)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "insert into marketing_profile_attribut_value_temp(partner,attribut_name,string_value,user,create_date,state,employe) values (" + id + ",'country','" + country + "'," + user_contrat.iduser + " , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 1 , " + user_contrat.id_employe + ") ;";
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                try
                {
                    cmd.ExecuteScalar();
                    //UserDialogs.Instance.Alert("ADDED TO TEMP VALUES");
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
        }


        public static async Task updateNumberePartner(int id, string number)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "insert into marketing_profile_attribut_value_temp(partner,attribut_name,string_value,user,create_date,state,employe) values (" + id + ",'number','" + number + "'," + user_contrat.iduser + " , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 1 , " + user_contrat.id_employe + ") ;";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    cmd.ExecuteScalar();
                    //                      UserDialogs.Instance.Alert("ADDED TO TEMP VALUES");


                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
        }
        public static async Task updateFaxPartner(int id, string fax)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "insert into marketing_profile_attribut_value_temp(partner,attribut_name,string_value,user,create_date,state,employe) values (" + id + ",'fax','" + fax + "'," + user_contrat.iduser + " , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 1 , " + user_contrat.id_employe + ") ;";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    cmd.ExecuteScalar();
                    //UserDialogs.Instance.Alert("ADDED TO TEMP VALUES");


                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
        }

        public static async Task updateMobilePartner(int id, string mobile)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "insert into marketing_profile_attribut_value_temp(partner,attribut_name,string_value,user,create_date,state,employe) values (" + id + ",'mobile','" + mobile + "'," + user_contrat.iduser + " , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 1 , " + user_contrat.id_employe + ") ;";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    cmd.ExecuteScalar();
                    //UserDialogs.Instance.Alert("ADDED TO TEMP VALUES");


                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
        }


        public static async Task updateVatCodePartner(int id, string vat_code)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "insert into marketing_profile_attribut_value_temp(partner,attribut_name,string_value,user,create_date,state,employe) values (" + id + ",'vat_code','" + vat_code + "'," + user_contrat.iduser + " , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 1 , " + user_contrat.id_employe + ") ;";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    cmd.ExecuteScalar();
                    // UserDialogs.Instance.Alert("ADDED TO TEMP VALUES");


                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
        }


        /*      public static async Task<int?> InsertNewPartner(string name, string street, string city, string state, string postal_code, string country, string email, string fax, bool? customer, bool? supplier, int? category, string vat_code, int id_employe)
              {
                  if (await DbConnection.Connecter3())
                  {
                      string sqlCmd = "insert into commercial_partner(create_date,name,street,city,state,postal_code,country,email,fax,customer,supplier,category,vat_code,customer_discount,supplier_discount,Custumer_withholding_tax,Supplier_withholding_tax,sale_agent,currency) " +
                                      "values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + name + "','" + street + "','" + city + "','" + state + "','" + postal_code + "','" + country + "','" + email + "','" + fax + "'," + customer + "," + supplier + "," + category + ",'" + vat_code + "'," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + id_employe + ", 1); " + // Ajout de la devise par défaut (1)
                                      "select max(id) from commercial_partner;";

                      MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                      try
                      {
                          return int.Parse(cmd.ExecuteScalar().ToString());
                      }
                      catch (Exception ex)
                      {
                          // Log l'erreur si nécessaire
                          return null;
                      }
                  }
                  else
                  {
                      return null;
                  }
              }*/

        /*****************log**************************/
        public static async Task Log(string actionType)
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
                                     "', name = '" + ("partner").Replace("'", "''") + "'" +
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


        public static async Task<int?> InsertNewPartner(string name, string street, string city, string state, string postal_code, string country, string email, string fax, bool? customer, bool? supplier, int? category, string vat_code, string gps, int id_employe)
        {
            try
            {
                if (!await DbConnection.Connecter3())
                {
                    return null;
                }

                // Étape 1 : Récupérer l'ID de la devise principale
                int currencyId = 1; // Valeur par défaut
                string getCurrencySql = "SELECT id FROM atooerp_currency WHERE principal = 1 LIMIT 1";

                using (var getCurrencyCmd = new MySqlCommand(getCurrencySql, DbConnection.con))
                {
                    var result = await getCurrencyCmd.ExecuteScalarAsync();
                    if (result != null)
                    {
                        currencyId = Convert.ToInt32(result);
                    }
                }

                // Étape 2 : Insérer le nouveau partenaire
                string sqlCmd = @"INSERT INTO commercial_partner(create_date, name, street, city, state, postal_code, 
                         country, email, fax, customer, supplier, category, vat_code, 
                         customer_discount, supplier_discount, Custumer_withholding_tax, 
                         Supplier_withholding_tax, sale_agent, currency, gps) 
                         VALUES(@create_date, @name, @street, @city, @state, @postal_code, 
                         @country, @email, @fax, @customer, @supplier, @category, @vat_code, 
                         0, 0, 0, 0, @id_employe, @currencyId, @gps);
                         SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                {
                    // Paramètres pour éviter les injections SQL
                    cmd.Parameters.AddWithValue("@create_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@street", street);
                    cmd.Parameters.AddWithValue("@city", city);
                    cmd.Parameters.AddWithValue("@state", state);
                    cmd.Parameters.AddWithValue("@postal_code", postal_code);
                    cmd.Parameters.AddWithValue("@country", country);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@fax", fax);
                    cmd.Parameters.AddWithValue("@customer", customer);
                    cmd.Parameters.AddWithValue("@supplier", supplier);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@vat_code", vat_code);
                    cmd.Parameters.AddWithValue("@id_employe", id_employe);
                    cmd.Parameters.AddWithValue("@currencyId", currencyId);
                    cmd.Parameters.AddWithValue("@gps", gps);

                    // Exécution de la commande
                    var id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    CurrentData.CurrentNoteModule = "Commercial.Partner";
                    CurrentData.CurrentActivityModule = "Commercial.Partner, Commercial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    CurrentData.CurrentFormModule = "AtooERP_Standard.Partner.Partner_update, AtooERP_Standard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    CurrentData.CurrentModuleId = id;
                    await Log("insert");

                    return id;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'insertion du partenaire: {ex.Message}");
                return null;
            }
            finally
            {
                // Assurez-vous que la connexion est fermée
                if (DbConnection.con?.State == System.Data.ConnectionState.Open)
                {
                    await DbConnection.con.CloseAsync();
                }
            }
        }
        //public static async Task<int?> InsertNewPartner(string name, string street, string city, string state, string postal_code, string country, string email, string fax, bool? customer, bool? supplier, int? category, string vat_code, string gps, int id_employe)
        //{
        //    if (await DbConnection.Connecter3())
        //    {
        //        // Étape 1 : Récupérer l'ID de la devise principale à partir de la table atooerp_currency
        //        string getCurrencySql = "SELECT id FROM atooerp_currency WHERE principal = 1 LIMIT 1";
        //        MySqlCommand getCurrencyCmd = new MySqlCommand(getCurrencySql, DbConnection.con);
        //        object result = getCurrencyCmd.ExecuteScalar();

        //        int currencyId = result != null ? Convert.ToInt32(result) : 1; // Si aucune devise principale n'est trouvée, on utilise 1 par défaut

        //        // Étape 2 : Insérer le nouveau partenaire avec la devise récupérée
        //        string sqlCmd = "insert into commercial_partner(create_date,name,street,city,state,postal_code,country,email,fax,customer,supplier,category,vat_code,customer_discount,supplier_discount,Custumer_withholding_tax,Supplier_withholding_tax,sale_agent,currency,gps) " +
        //                        "values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + name + "','" + street + "','" + city + "','" + state + "','" + postal_code + "','" + country + "','" + email + "','" + fax + "'," + customer + "," + supplier + "," + category + ",'" + vat_code + "'," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + id_employe + "," + currencyId + ",'" + gps + "'); " +
        //                        "select max(id) from commercial_partner;";

        //        MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
        //        try
        //        {

        //            int Id = int.Parse(cmd.ExecuteScalar().ToString());
        //            //CurrentData.CurrentNoteModule = "Commercial.Partner";
        //            //CurrentData.CurrentActivityModule = "Commercial.Partner, Commercial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        //            //CurrentData.CurrentFormModule = "AtooERP_Standard.Partner.Partner_update, AtooERP_Standard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        //            CurrentData.CurrentModuleId = Id;
        //            await Log("insert");
        //            return int.Parse(cmd.ExecuteScalar().ToString());

        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Erreur SQL lors de l'enregistrement du log : " + ex.Message);

        //            // Log l'erreur si nécessaire
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}



    }

    public class tempValueName
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime create_date { get; set; }
        public tempValueName(string name, DateTime create_date)
        {

            this.name = name;
            this.create_date = create_date;
        }
    }

}
