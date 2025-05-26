using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Cash_deskStateView : ContentPage
{
    public Cash_deskStateView()
    {
        InitializeComponent();
    }
    public Cash_deskStateView(CashDesk cd)
    {
        InitializeComponent();
        BindingContext = new Cash_deskViewModel(cd);
    }
}