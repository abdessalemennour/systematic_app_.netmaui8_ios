using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class TransferPaymentView : ContentPage
{
	public TransferPaymentView()
	{
		InitializeComponent();
	}

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {


        if (sender is Frame frame && frame.BindingContext is Transfer.Payment Payment)
        {
            var ovm = BindingContext as TransferVewModel;
            if (Model.Transfer.Payment.IsPaymentAvailable((Payment.Id)) || Payment.Is_checked)
            {
                foreach (Transfer.Payment p in ovm.PaymentList)
                {
                    ovm.Transfer.PaymentList.Clear();
                    if (Payment.Id == p.Id)
                    {
                        p.Is_checked = !p.Is_checked;
                        foreach (Transfer.Payment payment in ovm.PaymentList)
                            if (payment.Is_checked == true)
                            {
                                ovm.Transfer.PaymentList.Add(new Transfer.Payment_transfer(0, payment.Id, 0, payment.Montant, payment.code, payment.Partner));
                            }
                        if (p.Is_checked)
                        {
                            ovm.SuccessPopupMessage = "Item has been added with success";
                            ovm.SuccessPopup = true;
                            await Task.Delay(1000);
                            ovm.SuccessPopup = false;
                        }
                        else
                        {
                            ovm.SuccessPopupMessage = "Item has been deleted with success";
                            ovm.FieldPopup = true;
                            await Task.Delay(1000);
                            ovm.FieldPopup = false;
                        }

                        break;
                    }
                }
                ovm.Transfer.SetTransferAmount();
            }
            else
                await App.Current.MainPage.DisplayAlert("Warning", "Payment already affected! ", "Ok");
        }



           
    }

  
}