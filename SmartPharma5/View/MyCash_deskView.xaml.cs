using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MyCash_deskView : ContentPage
{
    public MyCash_deskView()
    {
        InitializeComponent();
        BindingContext = new MyCash_deskViewModel();
    }
    public MyCash_deskView(string c)
    {
        InitializeComponent();
        BindingContext = new MyCash_deskViewModel(c);
    }

    private void Button_Focused(object sender, FocusEventArgs e)
    {

    }
}