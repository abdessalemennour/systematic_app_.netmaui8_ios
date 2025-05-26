using Acr.UserDialogs;
using DevExpress.Maui.Charts;
using SmartPharma5.Model;
using SmartPharma5.ModelView;
using System.Collections.Generic;

namespace SmartPharma5.View;

public partial class BarPieGaugeViews : ContentPage
{
    public string XML_FILE { get; set; }
	public BarPieGaugeViews(string xml_file)
	{
        this.XML_FILE= xml_file;
		InitializeComponent();

		BindingContext = new BarSeriesMV(this.XML_FILE);
}

    public BarPieGaugeViews(List<ModelGridParametre> List_Paramettre_final, string xml_file)
    {
        this.XML_FILE = xml_file;
        InitializeComponent();

        BindingContext = new BarSeriesMV(List_Paramettre_final, this.XML_FILE);
    }

    private void ChartView_SelectionChanged(object sender, DevExpress.Maui.Charts.SelectionChangedEventArgs e)
    {
        try
        {
            var b = e.SelectedObjects;
            DevExpress.Maui.Charts.DataSourceKey a = e.SelectedObjects[0] as DataSourceKey;
            LandBarItem barSelected = a.DataObject as LandBarItem;
            var ovm = BindingContext as BarSeriesMV;
            ovm.List_BarSeries.FirstOrDefault(p => p.Id == barSelected.Id_BarSerie).Selected_Value1 = Convert.ToString((int)Math.Round(barSelected.Number1));
            ovm.List_BarSeries.FirstOrDefault(p => p.Id == barSelected.Id_BarSerie).Selected_Arguemnt = barSelected.Name;

        }
        catch(Exception ex)
        {

        }
     

      
       
        //ovm.List_BarSeries[barSelected.Id_BarSerie].Selected_Value = Convert.ToString(barSelected.Number1);
        // ovm.List_BarSeries[barSelected.Id_BarSerie].Selected_Arguemnt = barSelected.Name;


        //BarSeriesModel.se = barSelected.Name;
        //BarSeriesModel.Selected_Value =Convert.ToString( barSelected.Number1);





    }

    private void ChartView_SelectionChanged_1(object sender, DevExpress.Maui.Charts.SelectionChangedEventArgs e)
    {
        try
        {
            var b = e.SelectedObjects;
            DevExpress.Maui.Charts.DataSourceKey a = e.SelectedObjects[0] as DataSourceKey;
            LandBarItem barSelected = a.DataObject as LandBarItem;
            var ovm = BindingContext as BarSeriesMV;
            ovm.List_BarSeries.FirstOrDefault(p => p.Id == barSelected.Id_BarSerie).Selected_Value1 = Convert.ToString((int)Math.Round(barSelected.Number1));
            ovm.List_BarSeries.FirstOrDefault(p => p.Id == barSelected.Id_BarSerie).Selected_Arguemnt = barSelected.Name;

            ovm.List_BarSeries.FirstOrDefault(p => p.Id == barSelected.Id_BarSerie).Selected_Value2 = Convert.ToString((int)Math.Round(barSelected.Number2));

        }
        catch(Exception ex)
        {

        }
       



       

    }

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        Popup2.IsOpen = true;

    }

    protected override bool OnBackButtonPressed()
    {
        GoDashboardingView().GetAwaiter();



        return true;
    }
    public async Task GoDashboardingView()
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new DashboardingView()));
        UserDialogs.Instance.HideLoading();


    }


    private void bar_chart_SelectionChanged(object sender, DevExpress.Maui.Charts.SelectionChangedEventArgs e)
    {
        try
        {
            var b = e.SelectedObjects;
            DevExpress.Maui.Charts.DataSourceKey a = e.SelectedObjects[0] as DataSourceKey;
            LandBarItem barSelected = a.DataObject as LandBarItem;
            var ovm = BindingContext as BarSeriesMV;
            ovm.List_BarSeries.FirstOrDefault(p => p.Id == barSelected.Id_BarSerie).Selected_Value1 = Convert.ToString((int)Math.Round(barSelected.Number1));
            ovm.List_BarSeries.FirstOrDefault(p => p.Id == barSelected.Id_BarSerie).Selected_Arguemnt = barSelected.Name;

            ovm.List_BarSeries.FirstOrDefault(p => p.Id == barSelected.Id_BarSerie).Selected_Value2 = Convert.ToString((int)Math.Round(barSelected.Number2));



            ovm.List_BarSeries.FirstOrDefault(p => p.Id == barSelected.Id_BarSerie).Selected_Value3 = Convert.ToString((int)Math.Round(barSelected.Number3));
        }
        catch(Exception ex)
        {

        }
       

    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        var ovm = BindingContext as BarSeriesMV;
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new BarPieGaugeViews(ovm.List_Paramettre_final, ovm.xml_file)));
        Popup2.IsOpen = false;
        UserDialogs.Instance.HideLoading();
    }
}