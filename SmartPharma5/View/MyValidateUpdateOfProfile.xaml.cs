using Acr.UserDialogs;
using DevExpress.Maui.Editors;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class MyValidateUpdateOfProfile : ContentPage
{
	public MyValidateUpdateOfProfile()
	{
		InitializeComponent();
        BindingContext = new ValidateUpdateProfileMV("my");
    }

    private void AutoCompleteEdit_TextChanged(object sender, AutoCompleteEditTextChangedEventArgs e)
    {
        AutoCompleteEdit edit = sender as AutoCompleteEdit;
        var search = edit.Text.ToLowerInvariant().ToString();
        var OVM = BindingContext as ValidateUpdateProfileMV;


        if (string.IsNullOrWhiteSpace(search))
        {
            OpportunitytCollectionView.ItemsSource = OVM.ListOfRequests.ToList();
        }
        else
        {
            OpportunitytCollectionView.ItemsSource = OVM.ListOfRequests.Where(i => (i.Partner_Name.ToLowerInvariant().Contains(search))).ToList();
        }

    }

    private async void Button_Clicked(object sender, EventArgs e)
    {

        AutoComp.Text = string.Empty;
        var OVM = BindingContext as ValidateUpdateProfileMV;
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        OVM.BtnCurrentNotif = false;
        OVM.BtnHitstoricNotif = true;
        OVM.ListOfRequests = ValidateProfileChanges.GetMyChangesProfile().Result; ;
        OpportunitytCollectionView.ItemsSource = ValidateProfileChanges.GetMyChangesProfile().Result;
        UserDialogs.Instance.HideLoading();

    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {

        AutoComp.Text = string.Empty;
        var OVM = BindingContext as ValidateUpdateProfileMV;
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        OVM.BtnCurrentNotif = false;
        OVM.BtnHitstoricNotif = true;
        OVM.ListOfRequests = ValidateProfileChanges.GetMyChangesRefusedHistoryProfile().Result;
        OpportunitytCollectionView.ItemsSource = ValidateProfileChanges.GetMyChangesRefusedHistoryProfile().Result;
        UserDialogs.Instance.HideLoading();
    }

    private async void Button_Clicked_2(object sender, EventArgs e)
    {

        AutoComp.Text = string.Empty;
        var OVM = BindingContext as ValidateUpdateProfileMV;
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        OVM.BtnCurrentNotif = false;
        OVM.BtnHitstoricNotif = true;
        OVM.ListOfRequests = ValidateProfileChanges.GetMyChangesAcceptedHistoryProfile().Result;
        OpportunitytCollectionView.ItemsSource = ValidateProfileChanges.GetMyChangesAcceptedHistoryProfile().Result;
        UserDialogs.Instance.HideLoading();
    }
}