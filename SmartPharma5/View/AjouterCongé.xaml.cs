using SmartPharma5.ModelView;

namespace SmartPharma5.View;

public partial class AjouterCongé : ContentPage
{
    public AjouterCongé()
    {
        InitializeComponent();
        BindingContext = new InsertCongé();

    }
}