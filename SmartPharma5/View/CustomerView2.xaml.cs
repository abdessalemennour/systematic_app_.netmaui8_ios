using Acr.UserDialogs;
using DevExpress.Maui.Editors;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class CustomerView2 : ContentPage
{
    Opportunity Opportunity { get; set; }
    public CustomerView2()
    {
        InitializeComponent();
        BindingContext = new AllPartnerMV();
    }

   
    private void OnItemTapped(object sender, EventArgs e)
    {
        var i = e;
        if (sender is Button label)
        {
            var a = label.Parent;
            var b = a.Parent;
            var c = b.Parent;
            var p = c.GetValue;

            //DisplayAlert("Item Tapped", $"You tapped on '{tappedItem}'", "OK");
        }


        //UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        //await Task.Delay(500);
        //-----------------return
        App.Current.MainPage.Navigation.PushAsync(new HomeView());
        //UserDialogs.Instance.HideLoading();

    }

    public void AutoCompleteEdit_TextChanged(object sender, AutoCompleteEditTextChangedEventArgs e)
    {
        AutoCompleteEdit edit = sender as AutoCompleteEdit;
        var search = edit.Text.ToLowerInvariant().ToString();
        var list = BindingContext as AllPartnerMV;


        if (string.IsNullOrWhiteSpace(search))
        {
            ClientCollectionView.ItemsSource = list.Partners.ToList();
        }
        else
        {
            ClientCollectionView.ItemsSource = list.Partners.Where(i => i.Name.ToLowerInvariant().Contains(search)).ToList();
        }

    }


    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {

            var ovm = BindingContext as AllPartnerMV;
            if (ovm.ActPopup == false)
            {
                await App.Current.MainPage.Navigation.PushAsync(new HomeView());

            }

        }
        catch (Exception ex)
        {
            await DbConnection.ErrorConnection();
        }

    }
    
  
    private async void ClientCollectionView_Tap(object sender, DevExpress.Maui.CollectionView.CollectionViewGestureEventArgs e)
    {
        var Item = e.Item as Model.Partner;


        if (Item != null)
        {
            //UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            //await Task.Delay(500);
            //-----------------return
            //await App.Current.MainPage.Navigation.PushAsync(new ProfileUpdate());
            //UserDialogs.Instance.HideLoading();
        }



    }
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading Please wait ...");
        await Task.Delay(500);
        if (await DbConnection.Connecter3())
        {
            
           
            if (sender is Frame frame && frame.BindingContext is Partner partner)
            {

                uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                Opportunity = new Opportunity((int)idagent, partner as Partner);
                //await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new OpportunityView(Opportunity)));
                //ios
                await App.Current.MainPage.Navigation.PushAsync(new OpportunityView(Opportunity));
                CurrentData.CurrentNoteModule = "CRM.Opportunity";
                CurrentData.CurrentActivityModule = "CRM.Opportunity, CRM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                CurrentData.CurrentFormModule = "AtooERP_Standard.CRM_Opportunity.Opportunity_update, AtooERP_Standard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

                //await Shell.Current.GoToAsync("OpportunityView");
                UserDialogs.Instance.HideLoading();

            }

            UserDialogs.Instance.HideLoading();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "Ok");
            UserDialogs.Instance.HideLoading();
        }
        
    }

    private void ClientCollectionView_Tap_1(object sender, DevExpress.Maui.CollectionView.CollectionViewGestureEventArgs e)
    {

    }
}