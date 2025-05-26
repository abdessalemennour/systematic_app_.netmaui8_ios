using Acr.UserDialogs;
using DevExpress.Maui.Editors;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class MyPartnerTempView : ContentPage
{
	public MyPartnerTempView()
	{
		InitializeComponent();
        BindingContext = new PartnerTempMV("name");
    }


    private void AutoCompleteEdit_TextChanged(object sender, AutoCompleteEditTextChangedEventArgs e)
    {
        AutoCompleteEdit edit = sender as AutoCompleteEdit;
        var search = edit.Text.ToLowerInvariant().ToString();
        var OVM = BindingContext as PartnerTempMV;


        if (string.IsNullOrWhiteSpace(search))
        {
            OpportunitytCollectionView.ItemsSource = OVM.ListPartnerTemp.ToList();
        }
        else
        {
            OpportunitytCollectionView.ItemsSource = OVM.ListPartnerTemp.Where(i => (i.Name_Partner_Temp.ToLowerInvariant().Contains(search))).ToList();
        }

    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        AutoComp.Text = string.Empty;
        var OVM = BindingContext as PartnerTempMV;
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        OVM.BtnCurrentNotif = false;
        OVM.BtnHitstoricNotif = true;
        OVM.ListPartnerTemp = PartnerTemp.GetMyPartnerTemp().Result;
        OpportunitytCollectionView.ItemsSource = PartnerTemp.GetMyPartnerTemp().Result.ToList();
        UserDialogs.Instance.HideLoading();

    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        AutoComp.Text = string.Empty;
        var OVM = BindingContext as PartnerTempMV;
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        OVM.BtnCurrentNotif = false;
        OVM.BtnHitstoricNotif = true;
        OVM.ListPartnerTemp = PartnerTemp.GetMyPartnerHistoryAcceptedTemp().Result;
        OpportunitytCollectionView.ItemsSource = PartnerTemp.GetMyPartnerHistoryAcceptedTemp().Result.ToList();

        UserDialogs.Instance.HideLoading();

    }

    private async void Button_Clicked_2(object sender, EventArgs e)
    {
        AutoComp.Text = string.Empty;
        var OVM = BindingContext as PartnerTempMV;
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        OVM.BtnCurrentNotif = false;
        OVM.BtnHitstoricNotif = true;
        OVM.ListPartnerTemp = PartnerTemp.GetMyPartnerHistoryRefusedTemp().Result;
        OpportunitytCollectionView.ItemsSource = PartnerTemp.GetMyPartnerHistoryRefusedTemp().Result.ToList();
        UserDialogs.Instance.HideLoading();

    }
}