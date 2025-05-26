using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class TransferView : TabbedPage
{
    public TransferView()
    {
        InitializeComponent();
    }

    public TransferView(CashDesk cd)
    {
        InitializeComponent();
        BindingContext = new TransferVewModel(cd);
    }
}