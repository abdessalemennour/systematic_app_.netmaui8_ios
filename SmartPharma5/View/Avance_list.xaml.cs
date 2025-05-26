using SmartPharma5.ModelView;

namespace SmartPharma5.View;

public partial class Avance_list : ContentPage
{
    public Avance_list(int id)
    {
        InitializeComponent();
        BindingContext = new AvanceMV(id);
    }
    public Avance_list()
    {
        InitializeComponent();
        BindingContext = new AvanceMV();
    }
}