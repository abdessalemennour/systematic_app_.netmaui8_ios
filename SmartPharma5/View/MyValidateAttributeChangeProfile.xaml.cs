using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class MyValidateAttributeChangeProfile : ContentPage
{
    public MyValidateAttributeChangeProfile(int id, int id_partner, int id_profile)
    {
        InitializeComponent();
        BindingContext = new ValidateChnageProfileAttributesMV(id, id_partner, id_profile);
    }
}