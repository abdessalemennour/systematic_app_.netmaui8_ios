using SmartPharma5.ModelView;

namespace SmartPharma5.View;

public partial class DécisionView : ContentPage
{
    public DécisionView(int id, bool accept)
    {
        InitializeComponent();
        BindingContext = new decision_congé(id, accept);
    }
    public DécisionView()
    {
        InitializeComponent();
    }
}