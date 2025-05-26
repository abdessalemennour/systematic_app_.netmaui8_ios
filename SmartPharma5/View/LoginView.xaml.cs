using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class LoginView : ContentPage
{
    public LoginView()
    {
        //App.Current.MainPage = new AppShell();
        // Dans le constructeur de votre page
        InitializeComponent();
        
        BindingContext = new LoginViewModel();

    }
}