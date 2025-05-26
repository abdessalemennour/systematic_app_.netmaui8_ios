using Acr.UserDialogs;
using MySqlConnector;
using SmartPharma5.Model;
using SmartPharma5.ModelView;

namespace SmartPharma5.View;

public partial class ContactPartnerPage : ContentPage
{
    public Partner partner {  get; set; }
    public int oppId { get; set; } = 0;
	public ContactPartnerPage(Partner partner)
	{
        this.partner = partner;
		BindingContext = new ContactPartnerMV(Convert.ToInt32(this.partner.Id));
		InitializeComponent();

	}
    public ContactPartnerPage(Partner partner ,int opp)
    {
        this.oppId = opp;
        this.partner = partner;
        BindingContext = new ContactPartnerMV(Convert.ToInt32(this.partner.Id),this.oppId);
        InitializeComponent();
    }

    public ContactPartnerPage(int entityId, EntityType entityType)
    {
        var modelViewEntityType = (SmartPharma5.ModelView.EntityType)entityType;
        string entityTypenote = CurrentData.CurrentNoteModule;
        string entityTypeactivity = CurrentData.CurrentActivityModule;
       // NavigationDrawer.Initialize(entityId, entityTypenote, entityTypeactivity);      
        //NavigationDrawer.Initialize(entityId, modelViewEntityType);
    }
  /*  private void OpenDrawer_Clicked(object sender, EventArgs e)
    {
        if (NavigationDrawer != null)
        {
            NavigationDrawer.ToggleDrawer();
        }
    }*/
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        UserDialogs.Instance.Toast("List Forms ...");
        await Task.Delay(200);

        if (await DbConnection.Connecter3())
        {

            try
            {
                if (sender is Frame frame && frame.BindingContext is SmartPharma5.Model.Contact_Partner Partner1)
                { if(oppId == 0)
                    {
                        await App.Current.MainPage.Navigation.PushAsync(new FormListView(this.partner, Partner1.id));
                    }
                    else
                    {
                        await App.Current.MainPage.Navigation.PushAsync(new FormListView(this.partner, Partner1.id,this.oppId));
                    }
                    
                }
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
                UserDialogs.Instance.HideLoading();
            }

        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "OK");

        }





    }

    private async void SimpleButton_Clicked(object sender, EventArgs e)
    {

        UserDialogs.Instance.Toast("List Forms ...");
        await Task.Delay(200);

        if (await DbConnection.Connecter3())
        {

            try
            {
               
                    if (oppId == 0)
                    {
                        await App.Current.MainPage.Navigation.PushAsync(new FormListView(this.partner, 0));
                    }
                    else
                    {
                        await App.Current.MainPage.Navigation.PushAsync(new FormListView(this.partner, 0, this.oppId));
                    }

                
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
                UserDialogs.Instance.HideLoading();
            }

        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "OK");

        }

    }
}