using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ModelView;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class ShowTest : ContentPage
{
    bool TestLoad { get; set; } = true;
    public ShowTest()
    {
        InitializeComponent();
        BindingContext = new AllPartnerMV2();



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
    private void Search_Changed(object sender, EventArgs e)
    {
        Search();

    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {

            var ovm = BindingContext as AllPartnerMV2;
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
    public void Search()
    {

        var list = BindingContext as AllPartnerMV2;
        // var listOfForm = list.ListOfForm;
        var formList = list.Partners.ToList();
        //CategorySearch.ItemsSource = list.Catgory_list.ToList();

        // listOfForm = (ObservableRangeCollection<string>)formList.Select(x => x.Form_name).Distinct();
        if (PartnerSearch.Text != null)
        {
            string[] motsCles = PartnerSearch.Text.Split(' ');
            string partner_string = PartnerSearch.Text;
            formList = formList.Where(x => motsCles.All(mc => x.Name.ToLower().Contains(mc.ToLower()))).ToList();
            //formList = formList.Where(i => i.Name.ToLower().Contains(partner_string.ToLower())).ToList();

            // var resultats = formList.Where(x => motsCles.All(mc => x.Name.Contains(mc)));


        }
        if (EmailSearch.Text != null)
        {
            string email_string = EmailSearch.Text;
            formList = formList.Where(i => i.Email.ToLower().Contains(email_string.ToLower())).ToList();


        }
        if (CategorySearch.SelectedItem != null)
        {
            string category_name = CategorySearch.SelectedItem.ToString();
            formList = formList.Where(i => i.Category_Name.ToLower().Contains(category_name.ToLower())).ToList();


        }
        if (StateShearch.SelectedItem != null)
        {
            string state_name = StateShearch.SelectedItem.ToString();
            formList = formList.Where(i => i.State.ToLower().Contains(state_name.ToLower())).ToList();


        }

        ClientCollectionView.ItemsSource = formList.ToList();



    }
    private void SimpleButton_Clicked(object sender, EventArgs e)
    {
        StateShearch.SelectedItem = null;
        CategorySearch.SelectedItem = null;
        PartnerSearch.Text = null;
        EmailSearch.Text = null;


    }
    private async void 
        ClientCollectionView_Tap(object sender, DevExpress.Maui.CollectionView.CollectionViewGestureEventArgs e)
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

    private void ClientCollectionView_Tap_1(object sender, DevExpress.Maui.CollectionView.CollectionViewGestureEventArgs e)
    {

    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

        if (sender is Frame frame && frame.BindingContext is SmartPharma5.Model.Partner Partner)
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            // Opportunity = new Opportunity();
            CurrentData.CurrentNoteModule = "Commercial.Partner";
            CurrentData.CurrentActivityModule = "Commercial.Partner, Commercial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            CurrentData.CurrentFormModule = "AtooERP_Standard.Partner.Partner_update, AtooERP_Standard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

            // CurrentData.CurrentDocumentModule = "CRM.Opportunity, CRM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            CurrentData.CurrentModuleId = (int)Partner.Id;
            await App.Current.MainPage.Navigation.PushAsync(new ProfileUpdate(Partner.Id));
            UserDialogs.Instance.HideLoading();
        }



    }



    private void Button_Clicked_1(object sender, EventArgs e)
    {

    }
}