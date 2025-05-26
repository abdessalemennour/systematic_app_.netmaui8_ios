namespace SmartPharma5.View;

public partial class HrModule : ContentPage
{
	public HrModule()
	{
		InitializeComponent();
	}
   

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SammaryView));


    }

    private async void ImageButton_Clicked_1(object sender, EventArgs e)
    {
        var a = Shell.Current;

        await Shell.Current.GoToAsync(nameof(SammaryView));
    }
}