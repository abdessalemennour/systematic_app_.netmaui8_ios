using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Cash_deskLogView : ContentPage
{
    public Cash_deskLogView()
    {
        InitializeComponent();
    }
    public Cash_deskLogView(CashDesk cd)
    {
        InitializeComponent();
        BindingContext = new Cash_deskLogViewModel(cd);
    }
}