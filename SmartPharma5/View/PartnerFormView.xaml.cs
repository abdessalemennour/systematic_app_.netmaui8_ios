using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ModelView;
using SmartPharma5.ViewModel;
using System.Globalization;
using System.Windows.Input;

namespace SmartPharma5.View;

public partial class PartnerFormView : ContentPage
{
    public string type;
    public int oppId;
    public int id_partner;

    // Dans PartnerFormView.xaml.cs
private Partner_Form.Collection _currentForm;
public Partner_Form.Collection currentForm
{
    get => _currentForm;
    set
    {
        _currentForm = value;
        UpdateLocalisationButton(value); // Mettre à jour le bouton quand currentForm change
    }
}
    public DataTemplate ItemTemplate { get; set; }
    public PartnerFormView(string type)
    {
        InitializeComponent();
        this.type = type;
        BindingContext = new PartnerFormViewModel(type);
        added.IsVisible = false;

    }
    public PartnerFormView(int opp, int id_partner)
    {
        this.oppId = opp;
        InitializeComponent();
        this.type = type;
        BindingContext = new PartnerFormViewModel(opp);
        this.id_partner = id_partner;

    }


    /*******************************/

    /* public ICommand LocalisationCommand => new Command(async () =>
     {
         try
         {
             // Lire la localisation actuelle
             var location = await Geolocation.GetLocationAsync();

             if (location != null)
             {
                 double latitude = location.Latitude;
                 double longitude = location.Longitude;

                 // Naviguer vers LocationView avec les coordonnées
                 await Application.Current.MainPage.Navigation.PushAsync(new LocationView(latitude, longitude));
             }
             else
             {
                 await Application.Current.MainPage.DisplayAlert("Erreur", "Impossible de récupérer les coordonnées GPS.", "OK");
             }
         }
         catch (Exception ex)
         {
             await Application.Current.MainPage.DisplayAlert("Erreur", ex.Message, "OK");
         }
     });*/



    /* private async void Button_Clickedd(object sender, EventArgs e)
     {
         await Navigation.PushAsync(new LocationView());
     }*/



    /******************************/
    private async void DXCollectionView_Tap(object sender, DevExpress.Maui.CollectionView.CollectionViewGestureEventArgs e)
    {
        try
        {
            Popup.IsOpen = true;
            await Task.Delay(1000);
            var Item = e.Item as Partner_Form.Collection;
            if (Item != null)
            {
                if (await DbConnection.Connecter3())
                {
                    await App.Current.MainPage.Navigation.PushAsync(new QuizQuestionView((Item)));

                }
                else
                {
                    PopupConnection.IsOpen = true;


                }

            }
            Popup.IsOpen = false;
        }
        catch (Exception ex)
        {
            Popup.IsOpen = false;

        }




    }


    public void Search()
    {


        bool FiltreIsTrue = true;
        var list = BindingContext as PartnerFormViewModel;
        // var listOfForm = list.ListOfForm;
        var formList = list.PartnerFormList.ToList();
        // listOfForm = (ObservableRangeCollection<string>)formList.Select(x => x.Form_name).Distinct();
        if (PartnerSearch.SelectedItem != null)
        {
            int partner_id = int.Parse(PartnerSearch.SelectedValue.ToString());
            formList = formList.Where(i => i.Partner_id == partner_id).ToList();

            PartnerCollection.ItemsSource = formList;
            FiltreIsTrue = false;
        }
        if (FormSearch.SelectedItem != null)
        {
            string formn_name = FormSearch.SelectedItem.ToString();
            formList = formList.Where(i => i.Form_name.ToLowerInvariant().Contains(formn_name.ToLowerInvariant())).ToList();
            PartnerCollection.ItemsSource = formList;

            FiltreIsTrue = false;
        }
        if (StartEstimatedSearch.Date != null)
        {
            DateTime Estimated_date = StartEstimatedSearch.Date.Value;
            formList = formList.Where(i => i.Estimated_date.Date >= Estimated_date.Date).ToList();
            PartnerCollection.ItemsSource = formList;
            FiltreIsTrue = false;
        }
        if (EndEstimatedSearch.Date != null)
        {
            DateTime Estimated_date = EndEstimatedSearch.Date.Value;
            formList = formList.Where(i => i.Estimated_date.Date <= Estimated_date.Date).ToList();
            PartnerCollection.ItemsSource = formList;
            FiltreIsTrue = false;
        }
        if (StartCloseDateSearch.Date != null)
        {
            DateTime Close_date = StartCloseDateSearch.Date.Value;
            formList = formList.Where(i => i.Close_date >= Close_date.Date).ToList();
            PartnerCollection.ItemsSource = formList;
            FiltreIsTrue = false;
        }
        if (EndCloseDateSearch.Date != null)
        {
            DateTime Close_date = EndCloseDateSearch.Date.Value;
            formList = formList.Where(i => i.Close_date <= Close_date.Date).ToList();
            PartnerCollection.ItemsSource = formList;
            FiltreIsTrue = false;
        }

        if (CycleSearch.SelectedItem != null)
        {
            int Cycle_id = int.Parse(CycleSearch.SelectedValue.ToString());
            formList = formList.Where(i => i.Cycle_id == Cycle_id).ToList();
            PartnerCollection.ItemsSource = formList;
            FiltreIsTrue = false;
        }
        if (EmployeSearch.SelectedItem != null)
        {
            int Employe_id = int.Parse(EmployeSearch.SelectedValue.ToString());
            formList = formList.Where(i => i.Agent_id == Employe_id).ToList();
            PartnerCollection.ItemsSource = formList;
            FiltreIsTrue = false;
        }
        if (StateSearch.SelectedItem != null)
        {
            try
            {
                var str = StateSearch.SelectedValue.ToString();
                formList = formList.Where(x => x.state == str).ToList();
                PartnerCollection.ItemsSource = formList;
                FiltreIsTrue = false;
            }
            catch (Exception e)
            {

            }


        }
        if (FiltreIsTrue)
        {
            PartnerCollection.ItemsSource = list.PartnerFormList.ToList();
        }
        //UserDialogs.Instance.HideLoading();
    }


    //private void SimpleButton_Clicked(object sender, EventArgs e)
    //{

    //    PartnerSearch.SelectedItem =  CycleSearch.SelectedItem = StateSearch.SelectedItem = StartEstimatedSearch.Date = EndEstimatedSearch.Date = StartCloseDateSearch.Date = EndCloseDateSearch.Date= null;
    //    FormSearch.SelectedItem = null;
    //    Search();

    //}

    private void Search_Changed(object sender, EventArgs e)
    {
        Search();
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        //UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        //await Task.Delay(500);
        var vm = BindingContext as PartnerFormViewModel;
        vm.Loading = true;
        await Task.Delay(1000);
        await App.Current.MainPage.Navigation.PushAsync(new QuizPartnerFormCalender(vm.PartnerFormList));
        vm.Loading = false;
        // UserDialogs.Instance.HideLoading();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Supposons que vous avez accès à currentForm
        if (currentForm != null)
        {
            await UpdateLocalisationButton(currentForm);
        }
    }

    private async Task UpdateLocalisationButton(Partner_Form.Collection currentForm)
    {
        string gps1 = await Partner_Form.GetGpsCoordinatesFromDatabase((int)currentForm.Id);
        string gps2 = await Partner.GetGpsCoordinates((uint)currentForm.Partner_id);

        bool hasGps1 = !string.IsNullOrEmpty(gps1);
        bool hasGps2 = !string.IsNullOrEmpty(gps2);

        if (hasGps1 && hasGps2)
        {
            //ok
        }
        else if (!hasGps1 && !hasGps2)
        {
        }
        else if (hasGps1)
        {
        }
        else // seulement hasGps2
        {
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        UserDialogs.Instance.Toast("Quiz Form ...");
        await Task.Delay(200);

        if (await DbConnection.Connecter3())
        {
            try
            {
                try
                {
                    Popup.IsOpen = true;
                    await Task.Delay(300);
                    if (sender is Frame frame && frame.BindingContext is Partner_Form.Collection tappedItem)
                    {
                        if (await DbConnection.Connecter3())
                        {
                            try
                            {
                                CurrentData.CurrentNoteModule = "Marketing.Quiz.Partners_Forms";
                                CurrentData.CurrentModuleId = tappedItem.Id;
                                CurrentData.CurrentActivityModule = "Marketing.Quiz.Partners_Forms, Marketing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                                CurrentData.CurrentFormModule = "AtooERP_Marketing.Quiz.Partners_Forms.Partners_Forms_update, AtooERP_Marketing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

                                await App.Current.MainPage.Navigation.PushAsync(new QuizForm2(tappedItem));
                            }
                            catch (Exception ex)
                            { }


                        }
                        else
                        {
                            PopupConnection.IsOpen = true;
                        }
                    }
                    Popup.IsOpen = false;
                }
                catch (Exception ex)
                {
                    Popup.IsOpen = false;

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

    private void SimpleButton_Clicked(object sender, EventArgs e)
    {
        PartnerSearch.SelectedItem = CycleSearch.SelectedItem = StateSearch.SelectedItem = StartEstimatedSearch.Date = EndEstimatedSearch.Date = StartCloseDateSearch.Date = EndCloseDateSearch.Date = null;
        FormSearch.SelectedItem = null;
        EmployeSearch.SelectedItem = null;
        Search();

    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        filtredPannel.IsOpen = true;
    }

    private void SimpleButton_Clicked_1(object sender, EventArgs e)
    {
        filtredPannel.IsOpen = false;
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        try
        {
            Partner partner = Partner.GetCommercialPartnerById(id_partner).Result;
            List<Contact_Partner> partners = ContactPartnerMV.getContactsOfPartner(id_partner).Result.ToList();
            if (partners.Count > 0)
            {

                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new ContactPartnerPage(partner, oppId)));
                UserDialogs.Instance.HideLoading();

            }
            else
            {
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);

                if (oppId == 0)
                {
                    await App.Current.MainPage.Navigation.PushAsync(new FormListView(partner, 0));
                }
                else
                {
                    await App.Current.MainPage.Navigation.PushAsync(new FormListView(partner, 0, this.oppId));
                }



                UserDialogs.Instance.HideLoading();

            }


        }
        catch (Exception ex)
        {
            await DbConnection.ErrorConnection();
        }

    }


}