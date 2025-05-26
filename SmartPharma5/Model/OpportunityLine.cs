using MvvmHelpers;
using MySqlConnector;
using SQLite;
using System.ComponentModel;

namespace SmartPharma5.Model
{
    public class OpportunityLine : ObservableObject
    {
        #region attribus
        [PrimaryKey, AutoIncrement, Column("Id")]
        public int Id { get; set; }
        [Column("range")]
        public int range { get; set; }
        [Column("description")]
        public string description { get; set; }
        [Column("uniteName")]
        public string uniteName { get; set; }
        [Column("price")]
        public decimal price
        {
            get
            {
                _PUTTC = _price_tax_included * (1 - discount);
                return _price;
            }
            set
            {
                _price = value;
                _PUTTC = _price_tax_included * (1 - discount);
            }
        }
        decimal _price;

        private decimal Discount;
        [Column("discount")]
        public decimal discount { get => Discount; set => SetProperty(ref Discount, value); }
        [Column("IdPiece")]
        public int IdPiece { get; set; }
        [Column("purchase")]
        public bool purchase { get; set; }
        [Column("IdProduct")]
        public int IdProduct { get; set; }
        private decimal Quantity;
        [Column("quantity")]
        public decimal quantity { get => Quantity; set => SetProperty(ref Quantity, value); }//public decimal quantity { get; set; }
        [Column("cpublic_price")]
        public decimal? public_price { get; set; }
        [Column("characteristic")]
        public string characteristic { get; set; }
        [Column("picture")]
        public byte[] picture { get; set; }
        [Column("shippingCode")]
        public string shippingCode { get; set; }
        [Column("shippingDate")]
        public DateTime? shippingDate { get; set; }

        #endregion

        #region TAX

        public BindingList<Tax> TaxList = App.taxList;
        public bool tax1_check = true;
        public bool tax2_check = true;
        public bool tax3_check = true;
        public bool tax4_check = true;
        public bool tax5_check = true;
        public uint? tax1 { get; set; }
        public decimal tax1_base { get { return PUARHT; } }
        public Tax Tax1
        {
            get
            {
                if (tax1.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == tax1.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public string tax1_name { get { return Tax1.name; } }
        public decimal getRound(decimal value) { return value; }

        public decimal tax1_amount { get { return getRound(Tax1.getTaxAmount(tax1_base)) * Convert.ToInt16(tax1_check); } }
        public decimal tax1_total_amount { get { return getRound(Tax1.getTaxAmount(tax1_base, quantity)) * Convert.ToInt16(tax1_check); } }
        public uint? tax2 { get; set; }
        public decimal tax2_base { get { return getRound(PUARHT + tax1_amount); } }
        public Tax Tax2
        {
            get
            {
                if (tax2.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == tax2.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public string tax2_name { get { return Tax2.name; } }
        public decimal tax2_amount { get { return getRound(Tax2.getTaxAmount(tax2_base)) * Convert.ToInt16(tax2_check); } }
        public decimal tax2_total_amount { get { return getRound(Tax2.getTaxAmount(tax2_base, quantity)) * Convert.ToInt16(tax2_check); } }
        public uint? tax3 { get; set; }
        public decimal tax3_base { get { return getRound(tax2_base + tax2_amount); } }
        public Tax Tax3
        {
            get
            {
                if (tax3.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == tax3.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public string tax3_name { get { return Tax3.name; } }
        public decimal tax3_amount { get { return getRound(Tax3.getTaxAmount(tax3_base)) * Convert.ToInt16(tax3_check); } }
        public decimal tax3_total_amount { get { return getRound(Tax3.getTaxAmount(tax3_base, quantity)) * Convert.ToInt16(tax3_check); } }
        public uint? tax4 { get; set; }
        public decimal tax4_base { get { return getRound(tax3_base + tax3_amount); } }
        public Tax Tax4
        {
            get
            {
                if (tax4.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == tax4.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public string tax4_name { get { return Tax4.name; } }
        public decimal tax4_amount { get { return getRound(Tax4.getTaxAmount(tax4_base)) * Convert.ToInt16(tax4_check); } }
        public decimal tax4_total_amount { get { return getRound(Tax4.getTaxAmount(tax4_base, quantity)) * Convert.ToInt16(tax4_check); } }
        public uint? tax5 { get; set; }
        public decimal tax5_base { get { return getRound(tax4_base + tax4_amount); } }
        public Tax Tax5
        {
            get
            {
                if (tax5.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == tax5.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public string tax5_name { get { return Tax5.name; } }
        public decimal tax5_amount { get { return getRound(Tax5.getTaxAmount(tax5_base)) * Convert.ToInt16(tax5_check); } }
        public decimal tax5_total_amount { get { return getRound(Tax5.getTaxAmount(tax5_base, quantity)) * Convert.ToInt16(tax5_check); } }
        public decimal tax_amount
        {
            get
            {
                return getRound(tax5_amount + tax4_amount +
                    tax3_amount + tax2_amount + tax1_amount);
            }
        }
        public decimal price_tax_included
        {
            get { return getRound(PUARHT + tax_amount); }
            set { _price_tax_included = value; }
        }
        decimal _price_tax_included;
        public decimal tax_total_amount
        {
            get
            {
                return getRound(tax5_total_amount + tax4_total_amount +
                    tax3_total_amount + tax2_total_amount + tax1_total_amount);
            }
        }
        #endregion

        #region constructeur
        public OpportunityLine() { }
        public OpportunityLine(int Range, string description, decimal price, int product, decimal quantity, uint? tax1, uint? tax2, uint? tax3, uint? tax4, uint? tax5, decimal discount)
        {
            this.range = Range;
            this.description = description;
            this.price = price;
            this.IdProduct = product;
            this.quantity = quantity;
            this.tax1 = tax1;
            this.tax2 = tax2;
            this.tax3 = tax3;
            this.tax4 = tax4;
            this.tax5 = tax5;
            this.discount = discount;
            this.PTARTTC = getRound((PUARTTC * quantity));
        }
        public OpportunityLine(int Id, int range, string description, decimal price, decimal discount, int IdPiece, int IdProduct, decimal quantity, bool purchase, string reference, string uniteName, uint? tax1, uint? tax2, uint? tax3, uint? tax4, uint? tax5)
        {
            this.Id = Id;
            this.range = range;
            this.description = description;
            this.price = price;
            this.discount = discount;
            this.IdPiece = IdPiece;
            this.IdProduct = IdProduct;
            this.quantity = quantity;

            this.purchase = purchase;
            this.productReference = reference;
            this.uniteName = uniteName;
            this.tax1 = tax1;
            this.tax2 = tax2;
            this.tax3 = tax3;
            this.tax4 = tax4;
            this.tax5 = tax5;
        }


        #endregion

        #region getter
        public decimal PTHT
        {
            get
            {
                return getRound((price * quantity));
            }
        }
        private decimal MtDiscountUnit
        {
            get { return getRound((price * discount)); }
        }
        public decimal MtDiscountTotal
        {
            get { return getRound((PTHT * discount)); }
        }
        public decimal PUARHT
        {
            get { return getRound((price - MtDiscountUnit)); }
        }
        public decimal PTARHT
        {
            get { return getRound((quantity * PUARHT)); }
        }
        public decimal PUTTC_HR
        {
            get
            {
                decimal t1 = getRound(Tax1.getTaxAmount(price)) * Convert.ToInt16(tax1_check);
                decimal t2 = getRound(Tax2.getTaxAmount(price + t1)) * Convert.ToInt16(tax2_check);
                decimal t3 = getRound(Tax3.getTaxAmount(price + t1 + t2)) * Convert.ToInt16(tax3_check);
                decimal t4 = getRound(Tax4.getTaxAmount(price + t1 + t2 + t3)) * Convert.ToInt16(tax4_check);
                decimal t5 = getRound(Tax4.getTaxAmount(price + t1 + t2 + t3 + t4)) * Convert.ToInt16(tax5_check);
                return price + t1 + t2 + t3 + t4 + t5;

            }
        }
        public decimal PUTTC
        {
            get
            {
                if ((1 - discount) != 0)
                    _PUTTC = getRound(price_tax_included / (1 - discount));
                else
                    _PUTTC = PUTTC_HR;
                return _PUTTC;
            }
            set
            {
                _PUTTC = value;
                _price = getRound(Tax1.getBaseTax(getRound(Tax2.getBaseTax(getRound(Tax3.getBaseTax(getRound(Tax4.getBaseTax(getRound(Tax5.getBaseTax(_PUTTC))))))))));
            }
        }
        decimal _PUTTC;
        public decimal PTTTC
        {
            get { return getRound((PUTTC * quantity)); }
            set { }
        }
        public decimal PUARTTC
        {
            get
            {
                // return (PUARHT * (Fodec.value * Convert.ToInt32(checFODEC) + 1) * (1 + VAT.value));
                return price_tax_included;
            }
            set { }
        }
        private decimal ptarttc;
        public decimal PTARTTC { get => ptarttc; set => SetProperty(ref ptarttc, value); }
        public string productReference { get; set; }
        #endregion

        #region getliste


        public static BindingList<OpportunityLine> GetOpportunityLine(int idopp)
        {
            BindingList<OpportunityLine> LineList = new BindingList<OpportunityLine>();
            string sqlCmd = "SELECT *"
                            + " FROM crm_opportunity_line"
                            + " where crm_opportunity_line.piece = " + idopp + ";";
            DbConnection.Deconnecter();
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    LineList.Add(new OpportunityLine(
                         0, reader.GetString("description"),
                         reader.GetDecimal("price"),
                         reader.GetInt32("product"),
                         reader.GetDecimal("quantity"),
                         reader["tax1"] is uint ? Convert.ToUInt32(reader["tax1"]) : (uint?)null,
                         reader["tax2"] is uint ? Convert.ToUInt32(reader["tax2"]) : (uint?)null,
                         reader["tax3"] is uint ? Convert.ToUInt32(reader["tax3"]) : (uint?)null,
                         reader["tax4"] is uint ? Convert.ToUInt32(reader["tax4"]) : (uint?)null,
                         reader["tax5"] is uint ? Convert.ToUInt32(reader["tax5"]) : (uint?)null,
                         reader.GetDecimal("discount")));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            DbConnection.Deconnecter();
            reader.Close();

            return LineList;
        }



        #endregion

    }
}

