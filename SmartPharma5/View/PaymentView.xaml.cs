using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class PaymentView : TabbedPage
{
    public PaymentView()
    {
        InitializeComponent();
    }
    public PaymentView(Payment payment)
    {
        InitializeComponent();
        BindingContext = new PaymentViewModel(payment);
    }
}