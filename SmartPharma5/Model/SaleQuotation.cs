using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.Model
{
    public class SaleQuotation
    {
      
        public int Id { get; set; }

  
        public string Code { get; set; }

        public DateTime? CreateDate { get; set; }


        public string Reference { get; set; }

        
        public string GeneralCondition { get; set; }

        public DateTime? Date { get; set; }

        public bool TvaChec { get; set; } = false;

        public bool FodecChec { get; set; } = false;

        public uint Fodec { get; set; } = 0;

        public string Memo { get; set; }

        public uint Partner { get; set; } = 0;

        public uint PaymentMethod { get; set; } = 0;

        public string PaymentMethodName { get; set; } 

        public uint PaymentCondition { get; set; } = 0;

        public string PaymentConditionName { get; set; }

        public bool Validated { get; set; } = false;

        
        public decimal TotalAmount { get; set; } = 0.00000m;

        public uint Agent { get; set; } = 0;

        public bool Tax1 { get; set; } = false;

        public bool Tax2 { get; set; } = false;

        public bool Tax3 { get; set; } = false;

        public bool Tax4 { get; set; } = false;

        public bool Tax5 { get; set; } = false;

        public uint RevenueStamp { get; set; } = 0;

        public uint? CrmOpportunity { get; set; }

        public AsyncCommand edit_quotation { get; set; }

        public SaleQuotation()
        {
            edit_quotation = new AsyncCommand(edit_func);
            
        }

        private async Task edit_func()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);


            await App.Current.MainPage.Navigation.PushAsync(new edit_quotationView(this.Id, (int)this.Partner));

            UserDialogs.Instance.HideLoading();

        }

        public static async Task<List<SaleQuotation>> getSaleQuoatationByOpportunity(int opportunity)
        {
            string sqlCmd = "SELECT s.*,commercial_payment_method.name payment_method_name,commercial_payment_condition.name as payment_condition_name " +
                "FROM sale_quotation s\r\n" +
                "left join commercial_payment_method on commercial_payment_method.id=s.payment_method\r\n" +
                "LEFT JOIN commercial_payment_condition on commercial_payment_condition.id=s.payment_condition " +
                "where crm_opportunity=" + opportunity + ";" +
                "order by s.create_date asc";
            List<SaleQuotation> Quotations = new List<SaleQuotation>();

            if (await DbConnection.Connecter3())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {
                        SaleQuotation quotation = new SaleQuotation();
                        quotation.Id = Convert.ToInt32(reader["Id"]);
                        quotation.Partner = (uint)Convert.ToInt32(reader["Partner"]);
                        quotation.Code = Convert.ToString(reader["code"]); 
                        quotation.TotalAmount = Convert.ToDecimal(reader["total_amount"]);
                        quotation.CreateDate = Convert.ToDateTime(reader["create_date"]);
                        quotation.Reference = Convert.ToString(reader["reference"]);
                        quotation.PaymentMethodName = Convert.ToString(reader["payment_method_name"]);
                        quotation.PaymentConditionName = Convert.ToString(reader["payment_condition_name"]);
                        Quotations.Add(quotation);


                    }
                    catch (Exception ex)
                    {


                    }
                }
                reader.Close();


            }
            return Quotations;
        }

        public static async Task<List<SaleQuotationLine>> getSaleQuoatationLineById(int Quotation)
        {
            string sqlCmd = "SELECT s.*,commercial_product.name product_name FROM sale_quotation_line s\r\n" +
                "LEFT JOIN commercial_product ON commercial_product.Id=s.product" +
                "\r\nwhere piece="+Quotation+";";
            List<SaleQuotationLine> Quotations = new List<SaleQuotationLine>();

            if (await DbConnection.Connecter3())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {
                        SaleQuotationLine quotation = new SaleQuotationLine();
                        quotation.Id = Convert.ToInt32(reader["Id"]);
                        quotation.NameProduct = Convert.ToString(reader["product_name"]);
                        quotation.Quantity = Convert.ToInt32(reader["quantity"]);
                        quotation.Discount = Convert.ToDecimal(reader["discount"]);
                        quotation.Price = Convert.ToDecimal(reader["price"]);
                        quotation.TotalAmount = quotation.Quantity * quotation.Price;
                        quotation.Price_with_discount = quotation.Price - (quotation.Price* quotation.Discount);
                        Quotations.Add(quotation);
                    }
                    catch (Exception ex)
                    {


                    }
                }
                reader.Close();
            }
            return Quotations;
        }

        public static async Task<SaleQuotation> getSaleQuoatationById(int opportunity)
        {
            string sqlCmd = "SELECT s.*,commercial_payment_method.name payment_method_name,commercial_payment_condition.name as payment_condition_name " +
                "FROM sale_quotation s\r\n" +
                "left join commercial_payment_method on commercial_payment_method.id=s.payment_method\r\n" +
                "LEFT JOIN commercial_payment_condition on commercial_payment_condition.id=s.payment_condition " +
                "where s.Id=" + opportunity + ";" +
                "order by s.create_date asc";
            List<SaleQuotation> Quotations = new List<SaleQuotation>();

            if (await DbConnection.Connecter3())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {
                        SaleQuotation quotation = new SaleQuotation();
                        quotation.Id = Convert.ToInt32(reader["Id"]);
                        quotation.Partner = (uint)Convert.ToInt32(reader["Partner"]);
                        quotation.Code = Convert.ToString(reader["code"]);
                        quotation.TotalAmount = Convert.ToDecimal(reader["total_amount"]);
                        quotation.CreateDate = Convert.ToDateTime(reader["create_date"]);
                        quotation.Reference = Convert.ToString(reader["reference"]);
                        quotation.PaymentMethodName = Convert.ToString(reader["payment_method_name"]);
                        quotation.PaymentConditionName = Convert.ToString(reader["payment_condition_name"]);
                        Quotations.Add(quotation);


                    }
                    catch (Exception ex)
                    {


                    }
                }
                reader.Close();


            }
            return Quotations[0];
        }
    }

    public class SaleQuotationLine : BaseViewModel
    {


        private int id;
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string nameProduct;
        public string NameProduct { get => nameProduct; set => SetProperty(ref nameProduct, value); }


        private int quantity;
        public int Quantity { get => quantity; set => SetProperty(ref quantity, value); }


        private decimal price;
        public decimal Price { get => price; set => SetProperty(ref price, value); }



        private decimal price_with_discount;
        public decimal Price_with_discount { get => price_with_discount; set => SetProperty(ref price_with_discount, value); }

        private decimal discount;
        public decimal Discount { get => discount; set => SetProperty(ref discount, value); }

        private decimal totalAmount;
        public decimal TotalAmount { get => totalAmount; set => SetProperty(ref totalAmount, value); }

        private bool isUpdated;
        public bool IsUpdated { get => isUpdated; set => SetProperty(ref isUpdated, value); }

        private bool isNew;
        public bool IsNew { get => isNew; set => SetProperty(ref isNew, value); }

        private  bool enabledChange;
        public  bool EnabledChange { get => enabledChange; set => SetProperty(ref enabledChange, value); }

      

        public AsyncCommand changeQuantity { get; set; }
        public SaleQuotationLine()
        {
            EnabledChange = false;
            IsUpdated = false;
            changeQuantity = new AsyncCommand(change);
        }

        public async Task change()
        {
            this.TotalAmount = this.Price_with_discount * this.Quantity;
            this.IsUpdated = true;
        }

    

    }
}
