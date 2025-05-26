using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class TransferStateView : ContentPage
{
    public TransferStateView()
    {
        InitializeComponent();
    }
    public TransferStateView(Transfer Tr)
    {
        InitializeComponent();
        BindingContext = new TransferVewModel(Tr);
    }
}