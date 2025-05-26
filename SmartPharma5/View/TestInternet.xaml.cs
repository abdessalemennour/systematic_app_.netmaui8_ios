using Acr.UserDialogs;
using SmartPharma5.Model;

namespace SmartPharma5.View;

public partial class TestInternet : ContentPage
{
    public TestInternet()
    {
        InitializeComponent();
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        loading.IsVisible = true;

        int t = await App.UserCheckModule();
        if (DbConnection.Connecter() == false)
        {
            t = 0;
            //App.Current.MainPage = new TestInternet();
            await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new TestInternet()));
            await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");
            return;

        }
        var IdEmploye = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
        if (IdEmploye == 0)
        {
            //App.Current.MainPage = new LoginView();
            await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new LoginView()));

        }
        else
        {
            switch (t)
            {
                case 27:
                    //await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new ShowTest()));

                    //App.Current.MainPage = new NavigationPage(new HomeView());
                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
                    break;
                case 28:

                    //App.Current.MainPage = new NavigationPage(new SammaryView());
                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new SammaryView()));
                    break;
                case 32:
                    //App.Current.MainPage = new NavigationPage(new SammaryView());
                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new SammaryView()));
                    break;
                case 37:

                    //App.Current.MainPage = new NavigationPage(new SammaryView());
                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new SammaryView()));
                    break;
                default:

                    //App.Current.MainPage = new NavigationPage(new HomeView());
                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
                    break;
            }

        }






    }

    private async void ImageButton_Clicked_1(object sender, EventArgs e)
    {
        await App.Current.MainPage.Navigation.PopToRootAsync();
    }

    private async void ImageButton_Clicked_2(object sender, EventArgs e)
    {

        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new LoginView()));
        UserDialogs.Instance.HideLoading();


    }
}