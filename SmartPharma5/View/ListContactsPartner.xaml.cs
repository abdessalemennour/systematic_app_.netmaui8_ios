using Acr.UserDialogs;
using CommunityToolkit.Maui.Views;
using SmartPharma5.Model;
using SmartPharma5.ModelView;

namespace SmartPharma5.View;

public partial class ListContactsPartner : ContentPage
{
	


    public int id { get; set; }

    public ListContactsPartner(int id)
    {
        this.id = id;
        BindingContext = new ContactPartnerMV(Convert.ToInt32(this.id));
        InitializeComponent();
    }

    private async void SimpleButton_Clicked(object sender, EventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        //await App.Current.MainPage.Navigation.PushAsync(new EditContactPage());
        UserDialogs.Instance.HideLoading();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Recharge les données ou effectue des actions spécifiques pour actualiser la page
        BindingContext = new ContactPartnerMV(Convert.ToInt32(this.id)); // Rafraîchir le BindingContext
        await Task.Delay(100); // Si nécessaire, tu peux ajouter une petite attente pour gérer les animations ou chargements
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
                    
                    await Task.Delay(300);
                    if (sender is Frame frame && frame.BindingContext is Contact_Partner contact)
                    {
                        if (await DbConnection.Connecter3())
                        {
                            try
                            {
                                await App.Current.MainPage.Navigation.PushAsync(new EditContactPage(contact.id));
                            }
                            catch (Exception ex)
                            { }


                        }
                        else
                        {
                            
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    

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

    private async void Button_Clicked(object sender, EventArgs e)
    {
        UserDialogs.Instance.Toast("Add New Contact ...");
        await Task.Delay(200);

        if (await DbConnection.Connecter3())
        {
            try
            {
                try
                {

                    await Task.Delay(300);

                    if (await DbConnection.Connecter3())
                    {
                        try
                        {
                            await App.Current.MainPage.Navigation.PushAsync(new AddNewContact(this.id));
                        }
                        catch (Exception ex)
                        {
                        }

                    }



                    

                }
                catch (Exception ex)
                {


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