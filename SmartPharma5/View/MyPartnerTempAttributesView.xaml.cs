using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class MyPartnerTempAttributesView : ContentPage
{
	public MyPartnerTempAttributesView(int partner_temp, int id_profile, int id_employe)
	{
		InitializeComponent();
        BindingContext = new PartnerTempAttributesMV(partner_temp, id_profile, id_employe);
    }
}