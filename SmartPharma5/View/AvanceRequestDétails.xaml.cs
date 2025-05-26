using SmartPharma5.ModelView;

namespace SmartPharma5.View;

public partial class AvanceRequestDétails : ContentPage
{
    public AvanceRequestDétails()
    {
        InitializeComponent();

    }
    public AvanceRequestDétails(int id)
    {
        InitializeComponent();
        BindingContext = new DétailsAvanceRequest(id);
    }
}