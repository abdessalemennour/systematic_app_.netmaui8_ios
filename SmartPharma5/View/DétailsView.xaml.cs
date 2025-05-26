using SmartPharma5.ModelView;

namespace SmartPharma5.View;

public partial class DétailsView : ContentPage
{
    public DétailsView(int id)
    {
        InitializeComponent();
        BindingContext = new DétailsDayOffMv(id);
    }
}