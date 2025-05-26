using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class ValidateUpdateOfProfile : ContentPage
{
    public ValidateUpdateOfProfile()
    {
        InitializeComponent();
        BindingContext = new ValidateUpdateProfileMV();
    }
}