using Acr.UserDialogs;
using SmartPharma5.ModelView;

namespace SmartPharma5.View;

public partial class DashBoardCustomazedView : ContentPage
{
	public int Id { get; set; }
	public DashBoardCustomazedView(int id)
	{
        BindingContext = new DashBoardingCustomazedMV(id);
		InitializeComponent();
		this.Id=id;
	}

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        var ovm = BindingContext as DashBoardingCustomazedMV;
        /* 
         * string querry2 = AddSpacesToParameters(Requette);
            this.Template_Requette = querry2;
            string querry1 = ReplaceParameters(querry2, ParametresList);*/
        string requette = DashBoardingCustomazedMV.ReplaceParameters(ovm.Template_Requette, ovm.List_Paramettre_final);

        //customazed grid
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new ChartView(this.Id));
        UserDialogs.Instance.HideLoading();

    }

    private async void ImageButton_Clicked_1(object sender, EventArgs e)
    {
        var ovm = BindingContext as DashBoardingCustomazedMV;
        string requette = DashBoardingCustomazedMV.ReplaceParameters(ovm.Template_Requette, ovm.List_Paramettre_final);
        //customazed dashboard
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new BarPieGaugeViews(ovm.XML_FILE));
        UserDialogs.Instance.HideLoading();

    }
    protected override bool OnBackButtonPressed()
    {
        GoHome().GetAwaiter();


        return true;
    }

    public async Task GoHome()
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new DashboardingView()));
        UserDialogs.Instance.HideLoading();


    }
   

    private void ImageButton_Clicked_2(object sender, EventArgs e)
    {
        Popup2.IsOpen = true;

    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Popup2.IsOpen = false;
    }
}