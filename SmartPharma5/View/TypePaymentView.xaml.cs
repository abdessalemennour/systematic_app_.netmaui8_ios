using SmartPharma5.Model;
using SmartPharma5.ViewModel;
using static SmartPharma5.Model.Payment;

namespace SmartPharma5.View;

public partial class TypePaymentView : ContentPage
{
    public TypePaymentView()
    {
        InitializeComponent();
        //BindingContext = new PaymentViewModel();

    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Piece partner)
        {
            var ovm = BindingContext as PaymentViewModel;
            if (ovm.PaymentTypeListSelectedItem.Id == 2 || ovm.PaymentTypeListSelectedItem.Id == 3)
            {
                foreach (Payment.Piece p in ovm.UnpaiedList)
                {
                    ovm.Payment.Payment_pieceList.Clear();
                    if (partner.Id == p.Id)
                    {
                        p.Is_checked = !p.Is_checked;
                        foreach (Payment.Piece piece in ovm.UnpaiedList)
                            if (piece.Is_checked == true)
                            {
                                ovm.Payment.Payment_pieceList.Add(new Payment.Payment_piece(0, piece.Id, piece.piece_type, piece.piece_type_name, piece.code, 0, piece.rest_amount, piece.total_amount, piece.paied_amount, piece.rest_amount, piece.currencySymbol,piece.DecimalNumbre));
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
                ovm.Payment.SetAmount();
            }

        }

    }

    private void ComboBoxEdit_SelectionChanged(object sender, EventArgs e)
    {

    }
}