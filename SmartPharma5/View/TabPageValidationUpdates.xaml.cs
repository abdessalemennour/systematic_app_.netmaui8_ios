using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class TabPageValidationUpdates : TabbedPage
{
    public TabPageValidationUpdates()
    {
        BindingContext = new TabMV();
        user_contrat.getInfo().Wait();
        InitializeComponent();
        var ovm = BindingContext as TabMV;

        //first.Title = "(" + ovm.Number_Info_change + ") INFO";
        second.Title = "(" + ovm.Number_profile_change.ToString() + ") PROFILE";
        third.Title = "(" + ovm.Number_partner_add.ToString() + ") NEW PARTNER";
    }
    protected  override bool OnBackButtonPressed()
    {
        GoHome().GetAwaiter();


        return true;
    }

    public async Task GoHome()
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
        UserDialogs.Instance.HideLoading();


    }


    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));

        UserDialogs.Instance.HideLoading();
    }
}