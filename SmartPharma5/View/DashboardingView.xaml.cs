using Acr.UserDialogs;
using MvvmHelpers.Commands;
using SmartPharma5.ModelView;

namespace SmartPharma5.View;

public partial class DashboardingView : ContentPage
{
	public DashboardingView()
	{
		InitializeComponent();
		BindingContext = new DashboardinMV();
        var OVM = BindingContext as DashboardinMV;
        if(OVM.List_Dashboard != null)
        {
            foreach (DashBoardingModel item in OVM.List_Dashboard)
            {
                StackLayout st = new StackLayout();

                st.HorizontalOptions = LayoutOptions.Center;
                ImageButton img = new ImageButton
                {
                    BackgroundColor = Colors.DarkGoldenrod,
                    CornerRadius = 5,
                    HeightRequest = 100,
                    WidthRequest = 100,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(20),
                    Source = "Images/grid1.png",
                    Command = new AsyncCommand(item.GoTOMyDashboar)
                };
                Label lb = new Label();
                lb.Text = item.Name;
                lb.HorizontalOptions = LayoutOptions.Center;
                lb.VerticalOptions = LayoutOptions.Center;
                lb.FontAttributes = FontAttributes.Bold;
                st.Children.Add(img);
                st.Children.Add(lb);
                scroll.Add(st);
            }
            
           
            

           
           

        }


         async Task GoToGrid(int id)
        {
            await OVM.goToGrid();
            
        }
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
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
        UserDialogs.Instance.HideLoading();


    }

}