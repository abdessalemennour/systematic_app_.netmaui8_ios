using MySqlConnector;
using SQLite;
using System.ComponentModel;
using System.Data;

namespace SmartPharma5.Model
{
    public class Product
    {
        #region attributs
        public int Id { get; set; }
        [Column("Nom")]
        public string name { get; set; }
        [Column("Réference")]
        public string reference { get; set; }
        [Column("Code à  barres")]
        public string barcode { get; set; }
        [Column("Date de création")]
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
        [Column("Actif")]
        public bool actif { get; set; }
        [Column("Carractéristiques")]
        public string characteristic { get; set; }
        [Column("Déscription du fournisseur")]
        public string supplier_description { get; set; }
        [Column("Temp de production")]
        public int time_production { get; set; }
        [Column("Stock")]
        public decimal stock { get; set; }
        public decimal actual_stock { get; set; }
        public decimal theoretical_stock { get; set; }
        [Column("Rayon")]
        public string line { get; set; }
        [Column("Rongé")]
        public string row { get; set; }
        [Column("Case")]
        public string square { get; set; }
        [Column("Description du client")]
        public string customer_description { get; set; }
        [Column("Temps de livraison")]
        public int time_delivery { get; set; }
        [Column("Garantie")]
        public int guarantee { get; set; }
        [Column("A peser")]
        public bool to_weigh { get; set; }

        [Column("Nombre d'unité par lot")]
        public int batch_units { get; set; }
        [Column("NGP")]
        public string ngp { get; set; }

        [Column("Stock maximum")]
        public decimal max_stock { get; set; }

        [Column("Stock minimum")]
        public decimal min_stock { get; set; }
        [Column("Catégorie")]
        public int IdCategory { get; set; }
        [Column("Methode d'approvisionnement")]
        public int IdProviding_method { get; set; }
        [Column("Type")]
        public int IdType { get; set; }
        [Column("Methode de fourniture")]
        public int IdSupply_method { get; set; }

        [Column("Unité")]
        public int IdUnite { get; set; }
        [Column("Catégorie Point de vente")]
        public int IdCategory_point_sale { get; set; }
        [Column("Image")]
        public byte[] picture { get; set; }

        [Column("Unité")]
        public int Unite { get; set; }
        public int IdPrices_calculating_method { get; set; }
        [Column("Prix d'achat")]
        public decimal price_purchase
        {
            get { return _price_purchase; }
            set { _price_purchase = value; }
        }
        decimal _price_purchase;
        [Column("Prix de vente")]
        public decimal price_sale
        {
            get
            {
                return _price_sale;
            }
            set
            {
                this._price_sale = value;
            }
        }

        decimal _price_sale;

        [Column("Type")]
        public string type { get { return "Produit"; } }
        public bool is_composite { get; set; }
        public decimal public_price { get; set; }
        public bool purchase { get; set; }
        public bool sale { get; set; }
        public bool pos_synchronous { get; set; }
        public decimal Discount { get; set; }
        public decimal PUTTC
        {
            get
            {
                if ((1 - Discount) != 0)
                    _PUTTC = getRound(sale_price_tax_included * (1 - Discount));
                else
                    _PUTTC = 0;
                return _PUTTC;
            }
        }
        decimal _PUTTC;
        #region TAX
        public BindingList<Tax> TaxList { get; set; }
        #region Purchase
        public uint? purchase_tax1 { get; set; }
        decimal purchase_tax1Base { get { return _price_purchase; } }
        public Tax Purchase_tax1
        {
            get
            {
                if (purchase_tax1.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == purchase_tax1.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public decimal purchase_tax1Amount { get { return Purchase_tax1.getTaxAmount(purchase_tax1Base); } }
        public uint? purchase_tax2 { get; set; }
        decimal purchase_tax2Base { get { return price_purchase + purchase_tax1Amount; } }
        public Tax Purchase_tax2
        {
            get
            {
                if (purchase_tax2.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == purchase_tax2.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public decimal purchase_tax2Amount { get { return Purchase_tax2.getTaxAmount(purchase_tax2Base); } }
        public uint? purchase_tax3 { get; set; }
        decimal purchase_tax3Base { get { return purchase_tax2Base + purchase_tax2Amount; } }
        public Tax Purchase_tax3
        {
            get
            {
                if (purchase_tax3.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == purchase_tax3.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public decimal purchase_tax3Amount { get { return Purchase_tax3.getTaxAmount(purchase_tax3Base); } }
        public uint? purchase_tax4 { get; set; }
        decimal purchase_tax4Base { get { return purchase_tax3Base + purchase_tax3Amount; } }
        public Tax Purchase_tax4
        {
            get
            {
                if (purchase_tax4.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == purchase_tax4.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public decimal purchase_tax4Amount { get { return Purchase_tax4.getTaxAmount(purchase_tax4Base); } }
        public uint? purchase_tax5 { get; set; }
        decimal purchase_tax5Base { get { return purchase_tax4Base + purchase_tax4Amount; } }
        public Tax Purchase_tax5
        {
            get
            {
                if (purchase_tax5.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == purchase_tax5.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public decimal purchase_tax5Amount { get { return Purchase_tax5.getTaxAmount(purchase_tax5Base); } }
        public decimal purchase_taxAmount
        {
            get
            {
                return purchase_tax5Amount + purchase_tax4Amount +
                    purchase_tax3Amount + purchase_tax2Amount + purchase_tax1Amount;
            }
        }
        public decimal purchase_price_tax_included
        {
            get
            {
                _purchase_price_tax_included = _price_purchase + purchase_taxAmount;
                return _purchase_price_tax_included;
            }
            set
            {
                _purchase_price_tax_included = value;
                _price_purchase = Purchase_tax1.getBaseTax(Purchase_tax2.getBaseTax(Purchase_tax3.getBaseTax(Purchase_tax4.getBaseTax(Purchase_tax5.getBaseTax(_purchase_price_tax_included)))));
            }
        }
        decimal _purchase_price_tax_included;
        #endregion
        #region Sale
        public uint? sale_tax1 { get; set; }
        decimal sale_tax1Base { get { return _price_sale; } }
        public Tax Sale_tax1
        {
            get
            {
                if (sale_tax1.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == sale_tax1.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public decimal sale_tax1Amount { get { return Sale_tax1.getTaxAmount(sale_tax1Base); } }
        public uint? sale_tax2 { get; set; }
        decimal sale_tax2Base { get { return price_sale + sale_tax1Amount; } }
        public Tax Sale_tax2
        {
            get
            {
                if (sale_tax2.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == sale_tax2.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public decimal sale_tax2Amount { get { return Sale_tax2.getTaxAmount(sale_tax2Base); } }
        public uint? sale_tax3 { get; set; }
        decimal sale_tax3Base { get { return sale_tax2Base + sale_tax2Amount; } }
        public Tax Sale_tax3
        {
            get
            {
                if (sale_tax3.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == sale_tax3.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public decimal sale_tax3Amount { get { return Sale_tax3.getTaxAmount(sale_tax3Base); } }
        public uint? sale_tax4 { get; set; }
        decimal sale_tax4Base { get { return sale_tax3Base + sale_tax3Amount; } }
        public Tax Sale_tax4
        {
            get
            {
                if (sale_tax4.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == sale_tax4.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public decimal sale_tax4Amount { get { return Sale_tax4.getTaxAmount(sale_tax4Base); } }
        public uint? sale_tax5 { get; set; }
        decimal sale_tax5Base { get { return sale_tax4Base + sale_tax4Amount; } }
        public Tax Sale_tax5
        {
            get
            {
                if (sale_tax5.HasValue)
                {
                    foreach (Tax Tax in TaxList)
                        if (Tax.Id == sale_tax5.Value)
                            return Tax;
                }
                return new Tax();
            }
        }
        public decimal sale_tax5Amount { get { return Sale_tax5.getTaxAmount(sale_tax5Base); } }
        public decimal sale_taxAmount
        {
            get
            {
                return sale_tax5Amount + sale_tax4Amount +
                    sale_tax3Amount + sale_tax2Amount + sale_tax1Amount;
            }
        }
        public decimal sale_price_tax_included
        {
            get
            {
                _sale_price_tax_included = _price_sale + sale_taxAmount;
                return _sale_price_tax_included;
            }
            set
            {
                _sale_price_tax_included = value;
                _price_sale = Sale_tax1.getBaseTax(Sale_tax2.getBaseTax(Sale_tax3.getBaseTax(Sale_tax4.getBaseTax(Sale_tax5.getBaseTax(_sale_price_tax_included)))));
                // _margin = getMarge();
            }
        }
        decimal _sale_price_tax_included;
        #endregion

        public decimal Price_sale_ht_discount
        {
            get
            {

                _price_sale_ht_discount = getRound(price_sale * (1 - Discount));

                return _price_sale_ht_discount;
            }
        }
        decimal _price_sale_ht_discount;
        #endregion


        #endregion

        #region constructeurs
        public Product(int id, string name, decimal pricesale, string reference, uint? tax1, uint? tax2, uint? tax3, uint? tax4, uint? tax5, decimal discount, decimal salepricetaxincluded)
        {
            Id = id;
            this.name = name;
            this.price_sale = pricesale;
            // this.sale_price_tax_included = salepricetaxincluded;
            this.reference = reference;
            this.sale_tax1 = tax1;
            this.sale_tax2 = tax2;
            this.sale_tax3 = tax3;
            this.sale_tax4 = tax4;
            this.sale_tax5 = tax5;
            this.Discount = discount;
            this.TaxList = App.taxList;
        }
        public Product(int id, string name, decimal pricesale, string reference)
        {
            Id = id;
            this.name = name;
            this.price_sale = pricesale;
            this.reference = reference;
        }
        public Product()
        {
            this.Id = 0;
            this.create_date = DateTime.Now;
            this.actif = true;
            this.IdType = 1;
            this.IdProviding_method = 1;
            this.IdSupply_method = 1;
            this.IdUnite = 1;
            this.batch_units = 0;
            this.IdPrices_calculating_method = 2;
            this.is_composite = false;
            this.TaxList = Tax.getList();
        }
        public Product(bool purchase, bool sale, int type)
        {
            this.Id = 0;
            this.create_date = DateTime.Now;
            this.actif = true;
            this.purchase = purchase;
            this.sale = sale;
            this.IdType = type;
            this.IdProviding_method = 1;
            this.IdSupply_method = 1;
            this.IdUnite = 1;
            this.batch_units = 0;
            this.IdPrices_calculating_method = 2;
            this.is_composite = false;
            this.TaxList = Tax.getList();
        }
        //public Product(int Id)
        //{
        //    if (Id != 0)
        //    {
        //        DataSetCommercialTableAdapters.commercial_productTableAdapter commercial_productTableAdapter;
        //        commercial_productTableAdapter = new DataSetCommercialTableAdapters.commercial_productTableAdapter();
        //        DataView dv = commercial_productTableAdapter.GetDataById(Id).DefaultView;
        //        this.Id = Id;
        //        this.name = dv[0]["name"].ToString();
        //        this.reference = dv[0]["reference"].ToString();
        //        this.barcode = dv[0]["barcode"].ToString();
        //        this.create_date = Convert.ToDateTime(dv[0]["create_date"].ToString());
        //        this.IdCategory = Convert.ToInt32(dv[0]["category"].ToString());
        //        this.actif = Convert.ToBoolean(dv[0]["actif"].ToString());
        //        this.IdType = Convert.ToInt32(dv[0]["type"].ToString());
        //        this.characteristic = dv[0]["characteristic"].ToString();
        //        this.price_purchase = Convert.ToDecimal(dv[0]["price_purchase"].ToString());
        //        this.price_sale = Convert.ToDecimal(dv[0]["price_sale"].ToString());
        //        this.supplier_description = dv[0]["supplier_description"].ToString();
        //        this.time_production = Convert.ToInt32(dv[0]["time_production"].ToString());
        //        this.stock = Convert.ToDecimal(dv[0]["stock"]);
        //        this.line = dv[0]["line"].ToString();
        //        this.row = dv[0]["row"].ToString();
        //        this.square = dv[0]["square"].ToString();
        //        this.customer_description = dv[0]["customer_description"].ToString();
        //        this.time_delivery = Convert.ToInt32(dv[0]["time_delivery"]);
        //        this.guarantee = Convert.ToInt32(dv[0]["guarantee"]);
        //        this.to_weigh = Convert.ToBoolean(dv[0]["to_weigh"].ToString());
        //        this.IdProviding_method = Convert.ToInt32(dv[0]["providing_method"].ToString());
        //        this.IdType = Convert.ToInt32(dv[0]["type"]);
        //        this.IdSupply_method = Convert.ToInt32(dv[0]["supply_method"].ToString());
        //        this.IdUnite = Convert.ToInt32(dv[0]["unite"].ToString());
        //        this.IdCategory_point_sale = Convert.ToInt32(dv[0]["category_point_sale"].ToString());
        //        this.batch_units = Convert.ToInt32(dv[0]["batch_units"].ToString());
        //        this.ngp = dv[0]["ngp"].ToString();
        //        this.max_stock = Convert.ToDecimal(dv[0]["max_stock"].ToString());
        //        this.min_stock = Convert.ToDecimal(dv[0]["min_stock"].ToString());
        //        this.IdPrices_calculating_method = Convert.ToInt32(dv[0]["prices_calculating_method"].ToString());
        //        this.picture = dv[0]["picture"] is byte[]? (byte[])dv[0]["picture"] : (byte[])null;
        //        this.is_composite = Convert.ToBoolean(dv[0]["is_composite"]);
        //        this.public_price = Convert.ToDecimal(dv[0]["public_price"].ToString());
        //        this.purchase = Convert.ToBoolean(dv[0]["purchase"]); ;
        //        this.sale = Convert.ToBoolean(dv[0]["sale"]);
        //        this.pos_synchronous = Convert.ToBoolean(dv[0]["pos_synchronous"]);


        //        this.TaxList = Tax.getList();
        //        this.purchase_tax1 = dv[0]["purchase_tax1"] is uint ? (uint)dv[0]["purchase_tax1"] : (uint?)null;
        //        this.purchase_tax2 = dv[0]["purchase_tax2"] is uint ? (uint)dv[0]["purchase_tax2"] : (uint?)null;
        //        this.purchase_tax3 = dv[0]["purchase_tax3"] is uint ? (uint)dv[0]["purchase_tax3"] : (uint?)null;
        //        this.purchase_tax4 = dv[0]["purchase_tax4"] is uint ? (uint)dv[0]["purchase_tax4"] : (uint?)null;
        //        this.purchase_tax5 = dv[0]["purchase_tax5"] is uint ? (uint)dv[0]["purchase_tax5"] : (uint?)null;
        //        this.sale_tax1 = dv[0]["sale_tax1"] is uint ? (uint)dv[0]["sale_tax1"] : (uint?)null;
        //        this.sale_tax2 = dv[0]["sale_tax2"] is uint ? (uint)dv[0]["sale_tax2"] : (uint?)null;
        //        this.sale_tax3 = dv[0]["sale_tax3"] is uint ? (uint)dv[0]["sale_tax3"] : (uint?)null;
        //        this.sale_tax4 = dv[0]["sale_tax4"] is uint ? (uint)dv[0]["sale_tax4"] : (uint?)null;
        //        this.sale_tax5 = dv[0]["sale_tax5"] is uint ? (uint)dv[0]["sale_tax5"] : (uint?)null;


        //    }


        //}
        #endregion

        void customerSupplierDescriptionTest()
        {
            if (customer_description == null || customer_description == "")
                customer_description = name;
            if (supplier_description == null || supplier_description == "")
                supplier_description = name;
        }

        public class Collection
        {
            #region attributs
            public int Id { get { return _Id; } set { _Id = value; } }
            int _Id;
            [Column("Nom")]
            public string name { get { return _name; } set { _name = value; } }
            string _name;
            [Column("Réference")]
            public string reference { get; set; }
            [Column("Code à  barres")]
            public string barcode { get; set; }
            [Column("Stock actuel")]
            public decimal actual_stock
            {
                get
                {
                    return Math.Round(_actual_stock);
                }
                set { }
            }
            [Browsable(false)]
            public decimal _actual_stock { get; set; }
            [Column("Actif")]
            public bool actif { get; set; }
            [Column("Image")]
            public byte[] picture { get; set; }
            [Browsable(false)]
            [Column("PAHT")]
            public decimal price_purchase { get; set; }
            decimal _price_purchase { get; set; }
            [Browsable(false)]
            [Column("PVHT")]
            public decimal price_sale
            {
                get
                {
                    return Math.Round(_price_sale);
                }
                set { }
            }
            decimal _price_sale { get; set; }
            [Browsable(false)]
            public decimal taxe_purchase { get { return _taxe_purchase; } set { } }
            decimal _taxe_purchase;
            [Browsable(false)]
            public decimal taxe_sale { get { return _taxe_sale; } set { } }
            decimal _taxe_sale;
            [Column("PATTC")]
            public decimal priceTTC_purchase
            {
                get
                {
                    return Math.Round(_price_purchase * (1 + _taxe_purchase));
                }
                set { }
            }
            [Column("PVTTC")]
            public decimal priceTTC_sale
            {
                get
                {

                    return Math.Round(_price_sale * (1 + _taxe_sale));
                }
                set { }
            }
            [Column("Catégorie")]
            public string categoryName { get; set; }

            [Browsable(false)]
            [Column("Stock théorique")]
            public decimal theoretical_stock
            {
                get
                {
                    return Math.Round(_theoretical_stock);
                }
                set { }
            }
            [Browsable(false)]
            public decimal _theoretical_stock { get; set; }
            [Column("Type")]
            public string typeName { get; set; }



            #endregion
            public Collection(int Id, string name, string reference, string barcode,
                bool actif, string typeName, decimal price_purchase, decimal taxe_purchase,
                decimal taxe_sale, decimal price_sale, string categoryName,
                byte[] picture, decimal actual_stock, decimal theoretical_stock, int IdVAT_purchase,
                int IdVAT_sale)
            {
                this.Id = Id;
                this.name = name;
                this.reference = reference;
                this.barcode = barcode;
                this.categoryName = categoryName;
                this.actif = actif;
                this.typeName = typeName;
                this._price_purchase = price_purchase;
                this._price_sale = price_sale;
                this._taxe_purchase = taxe_purchase;
                this._taxe_sale = taxe_sale;
                this.picture = picture;
                this._actual_stock = actual_stock;
                this.theoretical_stock = theoretical_stock;
            }
            public Collection(int Id, string name, string reference, decimal price_purchase, decimal taxe_purchase,
               decimal taxe_sale, decimal price_sale)
            {
                this.Id = Id;
                this.name = name;
                this.reference = reference;
                this._price_purchase = price_purchase;
                this._price_sale = price_sale;
                this._taxe_purchase = taxe_purchase;
                this._taxe_sale = taxe_sale;
            }
            //public Collection(int Id)
            //{
            //    DataSetFathiTableAdapters.commercial_productListTableAdapter commercial_productListTableAdapter;
            //    commercial_productListTableAdapter = new DataSetFathiTableAdapters.commercial_productListTableAdapter();
            //    DataTable dt = commercial_productListTableAdapter.GetDataByIdProduct(Id);
            //    DataRow row = dt.Rows[0];
            //    this.Id = Convert.ToInt32(row["Id"].ToString());
            //    name = row["name"].ToString();
            //    reference = row["reference"].ToString();
            //    barcode = row["barcode"].ToString();
            //    actif = Convert.ToBoolean(row["actif"].ToString());
            //    typeName = row["typeName"].ToString();
            //    _price_purchase = Convert.ToDecimal(row["price_purchase"].ToString());
            //    taxe_purchase = Convert.ToDecimal(row["taxe_purchase"].ToString());
            //    taxe_sale = Convert.ToDecimal(row["taxe_sale"].ToString());
            //    _price_sale = Convert.ToDecimal(row["price_sale"].ToString());
            //    categoryName = row["categoryName"].ToString();
            //    picture = row["picture"] is byte[]? (byte[])row["picture"] : (byte[])null;
            //    actual_stock = Convert.ToDecimal(row["actual_stock"]);
            //    theoretical_stock = Convert.ToDecimal(row["theoretical_stock"]);
            //}
        }
        public class PieceCollection
        {
            #region attributs
            public int Id
            {
                get
                {
                    return _Id;
                }
                set
                {
                    _Id = value;
                }
            }
            int _Id;
            [Column("Nom")]
            public string name
            {
                get
                {
                    return _name;
                }
                set
                {
                    _name = value;
                }
            }
            string _name;
            [Column("Réference")]
            public string reference { get; set; }
            [Column("Code à  barres")]
            public string barcode { get; set; }
            [Column("PVHT")]
            public decimal price_sale
            {
                get
                {
                    return Math.Round(_price_sale);
                }
                set { }
            }
            decimal _price_sale { get; set; }
            [Column("PAHT")]
            public decimal price_purchase
            {
                get
                {
                    return Math.Round(_price_purchase);
                }
                set { }
            }
            decimal _price_purchase { get; set; }
            public int IdVAT_purchase { get; set; }
            public int IdVAT_sale { get; set; }
            public decimal taxe_purchase
            {
                get
                {
                    return _taxe_purchase;
                }
                set
                {
                }
            }
            decimal _taxe_purchase;
            public decimal taxe_sale
            {
                get
                {
                    return _taxe_sale;
                }
                set
                {
                }
            }
            decimal _taxe_sale;
            [Column("PATTC")]
            public decimal priceTTC_purchase { get; set; }
            [Column("PVTTC")]
            public decimal priceTTC_sale { get; set; }
            [Column("Discount")]
            public decimal discount { get; set; }
            [Column("Catégorie")]
            public string categoryName { get; set; }
            [Column("Stock actuel")]
            public decimal actual_stock
            {
                get
                {
                    return Math.Round(_actual_stock);
                }
                set { }
            }
            [Browsable(false)]
            public decimal _actual_stock { get; set; }
            [Browsable(false)]
            [Column("Stock théorique")]
            public decimal theoretical_stock
            {
                get
                {
                    return Math.Round(_theoretical_stock);
                }
                set { }
            }
            [Browsable(false)]
            public decimal _theoretical_stock { get; set; }
            [Column("Type")]
            public string uniteName { get; set; }
            [Column("Description du client")]
            public string customer_description { get; set; }
            [Column("Déscription du fournisseur")]
            public string supplier_description { get; set; }
            public decimal cump { get; set; }

            #endregion
            public PieceCollection(int Id, string name, string reference, string barcode,
                string uniteName, decimal price_purchase, decimal taxe_purchase, decimal price_sale,
                decimal taxe_sale, string categoryName, decimal actual_stock,
                decimal theoretical_stock, int IdVAT_purchase, int IdVAT_sale, string supplier_description,
                string customer_description, decimal priceTTC_purchase, decimal priceTTC_sale, decimal discount,
                decimal cump)
            {
                this.Id = Id;
                this.name = name;
                this.reference = reference;
                this.barcode = barcode;
                this.categoryName = categoryName;
                this.uniteName = uniteName;
                this._price_purchase = price_purchase;
                this._taxe_purchase = taxe_purchase;
                this._price_sale = price_sale;
                this._taxe_sale = taxe_sale;
                this._actual_stock = actual_stock;
                this.theoretical_stock = theoretical_stock;
                this.IdVAT_purchase = IdVAT_purchase;
                this.IdVAT_sale = IdVAT_sale;
                this.supplier_description = supplier_description;
                this.customer_description = customer_description;
                this.priceTTC_purchase = priceTTC_purchase;
                this.priceTTC_sale = priceTTC_sale;
                this.discount = discount;
                this.cump = cump;
            }
            //public PieceCollection(int Id)
            //{
            //    foreach (DataRow row in getobjectDataTableByIdProduct(Id).Rows)
            //    {
            //        this.Id = Id;
            //        this.name = row["name"].ToString();
            //        this.reference = row["reference"].ToString();
            //        this.barcode = row["barcode"].ToString();
            //        this.categoryName = row["categoryName"].ToString();
            //        this.uniteName = row["uniteName"].ToString();
            //        this._price_purchase = Convert.ToDecimal(row["price_purchase"].ToString());
            //        this._taxe_purchase = Convert.ToDecimal(row["taxe_purchase"].ToString());
            //        this._price_sale = Convert.ToDecimal(row["price_sale"].ToString());
            //        this._taxe_sale = Convert.ToDecimal(row["taxe_sale"].ToString());
            //        this._actual_stock = Convert.ToDecimal(row["actual_stock"]);
            //        this.theoretical_stock = Convert.ToDecimal(row["theoretical_stock"]);
            //        this.IdVAT_purchase = Convert.ToInt32(row["IdVAT_purchase"].ToString());
            //        this.IdVAT_sale = Convert.ToInt32(row["IdVAT_sale"].ToString());
            //        this.supplier_description = row["supplier_description"].ToString();
            //        this.customer_description = row["customer_description"].ToString();
            //    }
            //}
        }


        decimal getRound(decimal value) { return value; }
        public static async Task<BindingList<Product>> GetProduct(int partner)
        {

            string sqlCmd = "SELECT commercial_product.Id, commercial_product.name, commercial_product.min_stock, commercial_product.max_stock, commercial_product.reference, commercial_product.barcode, commercial_category_1.name AS categoryName,"
                  + " ROUND(commercial_product.price_purchase, 3) AS price_purchase, ROUND(commercial_product.purchase_price_tax_included, 3) AS priceTTC_purchase, ROUND(COALESCE(commercial_product_partner_price.price,"
                  + " commercial_product.price_sale), 3) AS price_sale, ROUND(commercial_product.sale_price_tax_included, 3) AS priceTTC_sale, "
                  + " commercial_product.supplier_description, commercial_product.customer_description, commercial_unite.name AS uniteName,"
                  + " commercial_product.taxe_purchase AS IdVAT_purchase, COALESCE(commercial_product_partner_price.taxe, commercial_product.taxe_sale) AS IdVAT_sale, coalesce(commercial_vat.value, 0) AS taxe_purchase,sale_tax1,sale_tax2,sale_tax3,sale_tax4,sale_tax5,"
                  + " coalesce(commercial_vat_2.value, commercial_vat_1.value, 0) AS taxe_sale,"
                  + " coalesce(commercial_partner_convention_line.discount,"
                  + " case when customer_discount = 0 then null else customer_discount end,"
                  + " commercial_soldes.discount, 0) AS discount,"
                  + " commercial_product.purchase_tax1, commercial_product.purchase_tax2, commercial_product.purchase_tax3, commercial_product.purchase_tax4,"
                  + " commercial_product.purchase_tax5, commercial_product.sale_tax1, commercial_product.sale_tax2, commercial_product.sale_tax3, commercial_product.sale_tax4, commercial_product.sale_tax5,"
                  + " commercial_product.purchase_price_tax_included, commercial_product.sale_price_tax_included"
                  + " FROM commercial_product LEFT OUTER JOIN"
                  + " commercial_category ON commercial_product.category = commercial_category.Id LEFT OUTER JOIN"
                  + " commercial_unite ON commercial_product.unite = commercial_unite.Id LEFT OUTER JOIN"
                  + " commercial_category commercial_category_1 ON commercial_product.category = commercial_category_1.Id LEFT OUTER JOIN"
                  + " commercial_vat ON commercial_product.taxe_purchase = commercial_vat.Id LEFT OUTER JOIN"
                  + " commercial_vat commercial_vat_1 ON commercial_product.taxe_sale = commercial_vat_1.Id LEFT OUTER JOIN"
                  + " commercial_soldes_product_discount_current commercial_soldes ON commercial_product.Id = commercial_soldes.product LEFT OUTER JOIN"
                  + " commercial_product_partner_price ON commercial_product_partner_price.product = commercial_product.Id AND commercial_product_partner_price.partner = " + partner + " AND"
                  + " commercial_product_partner_price.purchase_sale = 1 LEFT OUTER JOIN"
                  + " commercial_vat commercial_vat_2 ON commercial_product_partner_price.taxe = commercial_vat_2.Id LEFT OUTER JOIN"
                  + " commercial_partner ON commercial_partner.Id = " + partner + " LEFT OUTER JOIN"
                  + " commercial_partner_convention ON commercial_partner.Id = commercial_partner_convention.partner AND commercial_partner_convention.begin_date <= NOW() AND commercial_partner_convention.end_date >= NOW() AND"
                  + " commercial_partner_convention.activated = 1 and commercial_partner_convention.purchase_sale = 1 LEFT OUTER JOIN"
                  + " commercial_partner_convention_line ON commercial_partner_convention_line.commercial_partner_convention = commercial_partner_convention.Id AND commercial_partner_convention_line.product = commercial_product.Id"
                  + " WHERE(commercial_product.actif = 1) AND(commercial_product.sale = 1)"
                  + " GROUP BY commercial_product.Id;";

            BindingList<Product> list = new BindingList<Product>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            try
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.ConnectionString);
                adapter.SelectCommand.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                adapter.Fill(dt);


                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        var index = int.Parse(dr["Id"].ToString());
                                            list.Add(new Product(
                        int.Parse(dr["Id"].ToString()),
                        dr["name"].ToString(),
                        Convert.ToDecimal(dr["price_sale"].ToString()),
                        dr["reference"].ToString(),
                        dr["sale_tax1"] is uint ? (uint?)dr["sale_tax1"] : (uint?)null,
                        dr["sale_tax2"] is uint ? (uint?)dr["sale_tax2"] : (uint?)null,
                        dr["sale_tax3"] is uint ? (uint?)dr["sale_tax3"] : (uint?)null,
                        dr["sale_tax4"] is uint ? (uint?)dr["sale_tax4"] : (uint?)null,
                        dr["sale_tax5"] is uint ? (uint?)dr["sale_tax5"] : (uint?)null,
                        Convert.ToDecimal(dr["discount"].ToString()),
                        Convert.ToDecimal(dr["priceTTC_sale"].ToString())));

                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Warning", "Connection Time out", "Ok");
            }
            DbConnection.Deconnecter();

            //await Task.Delay(1000);
            return list;

        }
    }
}

