using MvvmHelpers;
using MvvmHelpers.Commands;
using SmartPharma5.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartPharma5.ModelView
{
    public class editQuotationMV : BaseViewModel
    {
        private List<SaleQuotationLine> selectedListQuotation;
        public List<SaleQuotationLine> SelectedListQuotation { get => selectedListQuotation; set => SetProperty(ref selectedListQuotation, value); }

        private SaleQuotation quotation;
        public SaleQuotation Quotation { get => quotation; set => SetProperty(ref quotation, value); }

        private bool isUpdatedBTN;
        public bool IsUpdated { get => isUpdatedBTN; set => SetProperty(ref isUpdatedBTN, value); }

        private bool savebtn;
        public bool Savebtn { get => savebtn; set => SetProperty(ref savebtn, value); }

        public List<int> ListIdDeleted { get; set; }
        public List<int> ListIdUpdated { get; set; }
        public List<SaleQuotationLine> ListLineAdded { get; set; }

        public AsyncCommand<SaleQuotationLine> DeleteCommand { get; }

        public editQuotationMV(int id)
        {
            IsUpdated = true;
            Savebtn = false;
            ListIdDeleted = new List<int>();
            ListIdUpdated= new List<int>();
            ListLineAdded= new List<SaleQuotationLine>();
            DeleteCommand = new AsyncCommand<SaleQuotationLine>(OnDelete);
            SelectedListQuotation = SaleQuotation.getSaleQuoatationLineById(id).Result;
            Quotation= SaleQuotation.getSaleQuoatationById(id).Result;
        }

        public editQuotationMV(SaleQuotation quotation ,ObservableRangeCollection<Product> productList, List<SaleQuotationLine> salesQuotationLines, List<int> ListDeleted, List<int> ListUpdated, List<SaleQuotationLine> ListNew
)
        {
            IsUpdated = true;
            Savebtn = false;
            this.ListIdDeleted = ListDeleted;
            this.ListIdUpdated = ListUpdated;
            this.Quotation= quotation;
            this.ListLineAdded = ListNew;
            DeleteCommand = new AsyncCommand<SaleQuotationLine>(OnDelete);
            this.SelectedListQuotation = salesQuotationLines;
        }
        public editQuotationMV(int id,int partner)
        {
            IsUpdated = true;
            Savebtn = false;
            ListIdDeleted = new List<int>();
            ListIdUpdated = new List<int>();
            ListLineAdded = new List<SaleQuotationLine>();
            DeleteCommand = new AsyncCommand<SaleQuotationLine>(OnDelete);
            SelectedListQuotation = SaleQuotation.getSaleQuoatationLineById(id).Result;
            Quotation = SaleQuotation.getSaleQuoatationById(id).Result;
        }

      

        public editQuotationMV(int id,List<SaleQuotationLine> lines)
        {
            IsUpdated = false;
            Savebtn = true;
            ListIdDeleted = new List<int>();
            ListIdUpdated = new List<int>();
            ListLineAdded = new List<SaleQuotationLine>();
            DeleteCommand = new AsyncCommand<SaleQuotationLine>(OnDelete);
            SelectedListQuotation = lines;
        }

        public async Task  OnDelete(SaleQuotationLine quotation)
        {
            if (SelectedListQuotation.Contains(quotation))
            {
                SelectedListQuotation.Remove(quotation);
            }
        }

    }
}
