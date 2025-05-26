using MySqlConnector;
using System.ComponentModel;
using System.Data;

namespace SmartPharma5.Model
{
    public class Tax
    {
        #region attribus
        public uint? Id { get; set; }
        public string name { get; set; }
        public decimal value { get; set; }
        public DateTime create_date
        {
            get;
            set;
        }
        public bool percent { get; set; }
        public decimal? quantity_max { get; set; }
        public uint? tax_type { get; set; }
        public bool activated { get; set; }
        #endregion
        #region Constructor
        public Tax()
        {
            this.create_date = DateTime.Now;
            percent = activated = true;

        }
        public Tax(uint Id, string name, decimal value, DateTime create_date, bool percent,
            decimal? quantity_max, uint? tax_type, bool activated)
        {
            this.Id = Id;
            this.name = name;
            this.value = value;
            this.create_date = create_date;
            this.percent = percent;
            this.quantity_max = quantity_max;
            this.tax_type = tax_type;
            this.activated = activated;
        }
        #endregion
        public static BindingList<Tax> getList()
        {
            BindingList<Tax> List = new BindingList<Tax>();
            DataTable TaxList = commercial_taxTable();
            try
            {
                foreach (DataRow Row in TaxList.Rows)
                    List.Add(new Tax(Convert.ToUInt32(Row["Id"]),
                        Row["name"].ToString(), Convert.ToDecimal(Row["value"]),
                        Convert.ToDateTime(Row["create_date"]),
                            Convert.ToBoolean(Row["percent"]),
                            Row["quantity_max"] is decimal ? Convert.ToDecimal(Row["quantity_max"]) : (decimal?)null,
                            Row["tax_type"] is uint ? Convert.ToUInt32(Row["tax_type"]) : (uint?)null,
                            Convert.ToBoolean(Row["activated"])));
            }
            catch (Exception exception)
            {
                //await App.Current.MainPage.DisplayAlert("Warning", exception.ToString() , "Ok");
            }


            return List;
        }
        //public static void setList()
        //{
        //    DbConnection.taxList = getList();
        //    DbConnection.taxTypeList = Type.getList();
        //}
        public decimal getTaxAmount(decimal amountBase)
        {
            if (percent)
                return amountBase * value;
            else
                return amountBase + value;
        }
        public decimal getBaseTax(decimal total_amount)
        {
            if (percent)
                return total_amount / (1 + value);
            else
                return total_amount - value;
        }
        public decimal getTaxAmount(decimal amountBase, decimal quantity)
        {
            if (percent)
                if (quantity_max.HasValue)
                    return amountBase * value * Math.Min(quantity_max.Value, quantity);
                else
                    return amountBase * value * quantity;
            else
                if (quantity_max.HasValue)
                return amountBase + value * Math.Min(quantity_max.Value, quantity);
            else
                return amountBase + value * quantity;
        }
        public static DataTable commercial_tax_typeTable()
        {
            string sqlCmd = "SELECT commercial_tax_type.* FROM commercial_tax_type;";

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.ConnectionString);
            adapter.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        public static DataTable commercial_taxTable()
        {

            string sqlCmd = "SELECT commercial_tax.* FROM commercial_tax;";
            DataTable dt = new DataTable();
            try
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.ConnectionString);
                adapter.SelectCommand.CommandType = CommandType.Text;

                adapter.Fill(dt);
            }
            catch (Exception exception)
            {
                //await App.Current.MainPage.DisplayAlert("Warning", exception.ToString(), "Ok");
            }

            return dt;
        }
        public class Type
        {
            #region attribus
            public uint Id { get; set; }
            public string name { get; set; }
            public string full_name { get; set; }
            public int level { get; set; }
            public DateTime create_date
            {
                get;
                set;
            }
            public string memo { get; set; }
            public bool activated { get; set; }
            #endregion
            #region Constructor
            public Type()
            {
                this.create_date = DateTime.Now;
                activated = true;

            }
            public Type(uint Id, DateTime create_date, string name, string full_name, int level,
                string memo, bool activated)
            {
                this.Id = Id;
                this.name = name;
                this.full_name = full_name;
                this.level = level;
                this.create_date = create_date;
                this.activated = activated;
            }
            #endregion
            public static BindingList<Type> getList()
            {
                BindingList<Type> List = new BindingList<Type>();
                try
                {
                    DataTable Tax_typeList = commercial_tax_typeTable();
                    foreach (DataRow Row in Tax_typeList.Rows)
                        List.Add(new Type(Convert.ToUInt32(Row["Id"]),
                            Convert.ToDateTime(Row["create_date"]),
                            Row["name"].ToString(),
                            Row["full_name"].ToString(),
                            Convert.ToInt32(Row["level"]),
                            Row["memo"].ToString(),
                                Convert.ToBoolean(Row["activated"])));
                }
                catch (Exception exception)
                {
                    //await App.Current.MainPage.DisplayAlert("Warning", exception.ToString() , "Ok");
                }

                return List;
            }
            public BindingList<Tax> getTaxList(BindingList<Tax> TaxListBase)
            {
                Tax nullTax = new Tax();
                //nullTax.Id = null;
                BindingList<Tax> TaxList = new BindingList<Tax>();
                TaxList.Add(nullTax);
                foreach (Tax Tax in TaxListBase)
                    if (Tax.tax_type == this.Id)
                        TaxList.Add(Tax);
                return TaxList;
            }
        }
    }
}

