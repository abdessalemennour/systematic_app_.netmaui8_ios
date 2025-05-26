using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;


public partial class AllPartnerView : ContentPage
{
    bool TestLoad { get; set; } = true;
    public AllPartnerView()
    {
        InitializeComponent();
        BindingContext = new AllPartnerMV();
        List<string> list = user_contrat.ListInVisibleBtn.ToList();
    }

    private void Search_Changed(object sender, EventArgs e)
    {
        Search();

    }
    public void Search()
    {

        var list = BindingContext as AllPartnerMV;
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
    private async void ClientCollectionView_Tap(object sender, DevExpress.Maui.CollectionView.CollectionViewGestureEventArgs e)
    {
        var Item = e.Item as Model.Partner;


        if (Item != null)
        {
            //UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            // await App.Current.MainPage.Navigation.PushAsync(new ProfileUpdate(Item.Id));
            //UserDialogs.Instance.HideLoading();
        }



    }
    protected override bool OnBackButtonPressed()
    {
        var ovm = BindingContext as AllPartnerMV;
        if (ovm.ActPopup == false)
        {
            App.Current.MainPage.Navigation.PushAsync(new HomeView());
            return true;
        }
        return true;

    }
    private void Button_Clicked(object sender, EventArgs e)
    {

    }

    /*protected async override void OnAppearing()
    {

        base.OnAppearing();

        var OVM = BindingContext as AllPartnerMV;
        try
        {
           await OVM.RefreshOnApp();
            //Task.Run(() => loadConnectionState());
        }
        catch (Exception ex)
        {

        }





    }*/
    public async Task loadConnectionState()
    {
        var OVM = BindingContext as AllPartnerMV;
        /* while (test_model_view.TestLoad)
         {

             await OVM.ChangeConnexionState();

         }*/

    }
    protected override void OnDisappearing()
    {
        this.TestLoad = false;
        base.OnDisappearing();




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
}
