using SmartPharma5.ModelView;

namespace SmartPharma5.View;

public partial class AjouterAvance : ContentPage
{
    public AjouterAvance()
    {
        InitializeComponent();
        BindingContext = new InsertAvanceMV();

    }
}