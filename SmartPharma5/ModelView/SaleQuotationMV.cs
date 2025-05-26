using MvvmHelpers;
using MvvmHelpers.Interfaces;
using SmartPharma5.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.ModelView
{
    public class SaleQuotationMV : BaseViewModel
    {
        public int opportunite;
        public Opportunity opp;

        public bool savingpopup = false;
        public bool SavingPopup { get => savingpopup; set => SetProperty(ref savingpopup, value); }


        private List<SaleQuotation> listQuotation;
        public List<SaleQuotation> ListQuotation { get => listQuotation; set => SetProperty(ref listQuotation, value); }


        private List<SaleQuotationLine> selectedListQuotation;
        public List<SaleQuotationLine> SelectedListQuotation { get => selectedListQuotation; set => SetProperty(ref selectedListQuotation, value); }

        private bool isSelected;
        public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }

        private int selectedQuotation;
        public int SelectedQuotation { get => selectedQuotation; set => SetProperty(ref selectedQuotation, value); }

        private bool successsavingpopup;
        public bool SuccessSavingPopup { get => successsavingpopup; set => SetProperty(ref successsavingpopup, value); }

        private string successpopupmessage;
        public string SuccessPopupMessage { get => successpopupmessage; set => SetProperty(ref successpopupmessage, value); }

        private bool successpopup = false;
        public bool SuccessPopup { get => successpopup; set => SetProperty(ref successpopup, value); }

        public Command QuotationCommand { get; }

        public SaleQuotationMV(Opportunity id)
        {
            this.opp= id;
            this.IsSelected = false;
            this.opportunite = id.Id;
            ListQuotation = new List<SaleQuotation>();
            ListQuotation = SaleQuotation.getSaleQuoatationByOpportunity(this.opportunite).Result;
            ListQuotation = ListQuotation.OrderByDescending(item => item.CreateDate).ToList();
            QuotationCommand = new Command(quotationFun);

        }

        private async void quotationFun()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to generate New Quotation for this Opportunity!", "Yes", "No");
            if (r)
            {
                SavingPopup = true;
                await Task.Delay(1000);
                if (DbConnection.Connecter())
                {
                    this.opp.TransferToQuotation();
                    SavingPopup = false;
                    SuccessPopupMessage = "'Quotation' has been successfully generated ";
                    SuccessPopup = true;
                    await Task.Delay(1000);
                    SuccessPopup = false;
                    await App.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    SavingPopup = false;
                    await Task.Delay(1000);
                    //Message = "Connection Failed";
                    //FieldPopup = true;
                }
            }
        }
    }
}
