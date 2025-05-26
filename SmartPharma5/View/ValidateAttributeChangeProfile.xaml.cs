using SmartPharma5.ViewModel;
//using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class ValidateAttributeChangeProfile : ContentPage
{
    public ValidateAttributeChangeProfile(int id, int id_partner, int id_profile)
    {
        InitializeComponent();
        BindingContext = new ValidateChnageProfileAttributesMV(id, id_partner, id_profile);
    }
  
}