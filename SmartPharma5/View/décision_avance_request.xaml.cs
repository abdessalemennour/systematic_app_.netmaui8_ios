using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class décision_avance_request : ContentPage
{
    public décision_avance_request()
    {
        InitializeComponent();
    }

    public décision_avance_request(int id, bool accept)
    {
        InitializeComponent();
        BindingContext = new décisionAvanceMV(id, accept);
    }
}