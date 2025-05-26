using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class PartnerTempView : ContentPage
{
    public PartnerTempView()
    {
        InitializeComponent();
        BindingContext = new PartnerTempMV();
    }
}