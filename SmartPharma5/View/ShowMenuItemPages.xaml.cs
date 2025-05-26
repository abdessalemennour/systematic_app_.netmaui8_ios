using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ModelView;
using SmartPharma5.ViewModel;
using System.Globalization;
using System.Reflection;
using Microsoft.Maui.Controls.Xaml;
namespace SmartPharma5.View;

public class ByteArrayToImageConverter
{
    public static ImageSource Convert(byte[] bytes)
    {
        if (bytes != null && bytes.Length > 0)
        {
            return ImageSource.FromStream(() => new MemoryStream(bytes));
        }
        return null;
    }
}

public partial class ShowMenuItemPages : ContentPage
{
    public string UserNamee;
    public string ListModule;
    public string ListGroup;
    public string DataBase;
    public ShowMenuItemPages()
	{
		InitializeComponent();
       
        BindingContext = new MenuModelView();
        user_contrat.getInfo().GetAwaiter();
        this.UserNamee = user_contrat.nameuser;
        this.ListModule = string.Join(" , ", user_contrat.ListModules);

        //this.ListGroup = string.Join(" , ", user_contrat.ListGroupes);

        this.DataBase= DbConnection.Database;
        UserName.Text= "UserName :" + this.UserNamee;
       // ListGroups.Text= this. ListGroup;
        //ListModules.Text= "List of modules : "+this.ListModule;
        DataBaseName.Text= "Name DataBase : "+this.DataBase;
        var logo = user_contrat.GetLogoFromDatabase();
        Image.Source = ByteArrayToImageConverter.Convert(logo);





    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {

    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {

    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        //MenuPop.IsVisible = true;
        var OVM = BindingContext as MenuModelView;
        OVM.MenuVisisble = !OVM.MenuVisisble;

    }

    private async void TapGestureRecognizer_Tapped_sales(object sender, TappedEventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new SalesModule()));
        UserDialogs.Instance.HideLoading();
    }
    private async void TapGestureRecognizer_Tapped_payment(object sender, TappedEventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new PaymentModule()));
        UserDialogs.Instance.HideLoading();
    }
    private async void TapGestureRecognizer_Tapped_marketing(object sender, TappedEventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        //await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new module()));
        UserDialogs.Instance.HideLoading();

    }
    private async void TapGestureRecognizer_Tapped_profiling(object sender, TappedEventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new ProfilingModule()));
        UserDialogs.Instance.HideLoading();

    }
    private async void TapGestureRecognizer_Tapped_summary(object sender, TappedEventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new SammaryView()));
        UserDialogs.Instance.HideLoading();
    }
    
}