using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ModelView;
using SmartPharma5.ViewModel;
using System.Security.Cryptography;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Media;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Maui.Alerts;
using System.Globalization;
namespace SmartPharma5.View;

public partial class AllPartnerForForms : ContentPage
{
    public AllPartnerForForms()
    {
        InitializeComponent();
        BindingContext = new AllPartnerMV1();
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

    private CancellationTokenSource cancellationTokenSource = new();

    private async Task Listen(CancellationToken cancellationToken)
    {
        var speechToText = SpeechToText.Default;

        var isGranted = await speechToText.RequestPermissions(cancellationToken);
        if (!isGranted)
        {
            await Toast.Make("Permission microphone non accordée").Show(CancellationToken.None);
            return;
        }

        string language = "fr-FR"; // ou "en-US"
        string recognitionText = "";

        var recognitionResult = await speechToText.ListenAsync(
            CultureInfo.GetCultureInfo(language),
            new Progress<string>(partialText =>
            {
                // Mise à jour en temps réel si besoin
                recognitionText = partialText;
                PartnerSearch.Text = recognitionText;
            }),
            cancellationToken);

        if (recognitionResult.IsSuccessful)
        {
            PartnerSearch.Text = recognitionResult.Text;
            // Déclenche automatiquement Search_Changed via le binding
        }
        else
        {
            await Toast.Make(recognitionResult.Exception?.Message ?? "Reconnaissance vocale échouée")
                       .Show(CancellationToken.None);
        }
    }

    private async void OnListenClicked(object sender, EventArgs e)
    {
        // Annule toute écoute en cours avant de recommencer
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        await Listen(cancellationTokenSource.Token);
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {

            var ovm = BindingContext as AllPartnerMV1;
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
        try
        {
            //formList = new List<Partner>();

            var list = BindingContext as AllPartnerMV1;
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
        catch (Exception ex)
        {

        }

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
        

        if (await DbConnection.Connecter3())
        {

            try
            {
                if (sender is Frame frame && frame.BindingContext is SmartPharma5.Model.Partner Partner)
                {
                    
                    List<Contact_Partner> partners = ContactPartnerMV.getContactsOfPartner(Convert.ToInt32(Partner.Id)).Result.ToList();

                    CurrentData.CurrentNoteModule = "Marketing.Quiz.Partners_Forms";
                    CurrentData.CurrentActivityModule = "Marketing.Quiz.Partners_Forms, Marketing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    CurrentData.CurrentFormModule = "AtooERP_Marketing.Quiz.Partners_Forms.Partners_Forms_update, AtooERP_Marketing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

                    if (partners.Count > 0)
                    {
                        UserDialogs.Instance.Toast("List Contacts ...");
                        await Task.Delay(200);

                        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                        await Task.Delay(500);
                        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new ContactPartnerPage(Partner)));
                        UserDialogs.Instance.HideLoading();

                    }
                    else
                    {
                        UserDialogs.Instance.Toast("List Forms ...");
                        await Task.Delay(200);

                        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                        await Task.Delay(500);

                        
                            await App.Current.MainPage.Navigation.PushAsync(new FormListView(Partner, 0));
                        
                       



                        UserDialogs.Instance.HideLoading();

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
}