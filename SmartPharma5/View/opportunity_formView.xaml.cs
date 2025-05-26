using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ModelView;

namespace SmartPharma5.View;

public partial class opportunity_formView : ContentPage
{
    public Partner partner;
    public int opportunitieId;
	public opportunity_formView(int id,int opp)
	{
        this.opportunitieId = opp;
        this.partner = Partner.GetCommercialPartnerById(id).Result;
        BindingContext = new OpportunitieFormMV(partner, opportunitieId);

        InitializeComponent();
	}


    private async void OnFabClicked(object sender, EventArgs e)
    {


            try
            {
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new ContactPartnerPage(partner, opportunitieId)));
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }

        
    }

    
}