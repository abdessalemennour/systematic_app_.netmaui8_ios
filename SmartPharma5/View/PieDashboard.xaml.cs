using SmartPharma5.ModelView;

namespace SmartPharma5.View;

public partial class PieDashboard : ContentPage
{
	public PieDashboard()
	{
		InitializeComponent();
		BindingContext = new PieDashboardMV();

    }
}