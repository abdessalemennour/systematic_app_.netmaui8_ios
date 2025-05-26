using Acr.UserDialogs;
using DevExpress.Maui.Editors;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class MyNotificationsProfilePartner : ContentPage
{
	public MyNotificationsProfilePartner()
	{
		InitializeComponent();
        BindingContext = new ViewModel.NotificationProfilePartnerMV("name");
    }

    private void AutoCompleteEdit_TextChanged(object sender, AutoCompleteEditTextChangedEventArgs e)
    {
        AutoCompleteEdit edit = sender as AutoCompleteEdit;
        var search = edit.Text.ToLowerInvariant().ToString();
        var OVM = BindingContext as NotificationProfilePartnerMV;


        if (string.IsNullOrWhiteSpace(search))
        {
            OpportunitytCollectionView.ItemsSource = OVM.ListNotification.ToList();
        }
        else
        {
            OpportunitytCollectionView.ItemsSource = OVM.ListNotification.Where(i => (i.profile_user.ToLowerInvariant().Contains(search)) || (i.name_attribute.ToLowerInvariant().Contains(search)) ).ToList();
        }

    }

  

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        AutoComp.Text= string.Empty;
        var OVM = BindingContext as NotificationProfilePartnerMV;
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        OVM.BtnCurrentNotif = true;
        OVM.BtnHitstoricNotif = false;
        OVM.ListNotification= NotificationProfilePartner.getMyAccepteHistoryTempValues().ToList(); ;
        OpportunitytCollectionView.ItemsSource = NotificationProfilePartner.getMyAccepteHistoryTempValues().ToList();
        UserDialogs.Instance.HideLoading();

    }

    private async void Button_Clicked_2(object sender, EventArgs e)
    {
        AutoComp.Text = string.Empty;
        var OVM = BindingContext as NotificationProfilePartnerMV;
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        OVM.BtnCurrentNotif = true;
        OVM. BtnHitstoricNotif = false;
        OVM.ListNotification = NotificationProfilePartner.getMyRefuseHistoryTempValues().ToList(); ;
        OpportunitytCollectionView.ItemsSource = NotificationProfilePartner.getMyRefuseHistoryTempValues().ToList();
        UserDialogs.Instance.HideLoading();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        AutoComp.Text = string.Empty;
        var OVM = BindingContext as NotificationProfilePartnerMV;
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        OVM.BtnCurrentNotif = true;
        OVM.BtnHitstoricNotif = false;
        OVM.ListNotification = NotificationProfilePartner.getMyTempValues().ToList(); ;
        OpportunitytCollectionView.ItemsSource = NotificationProfilePartner.getMyTempValues().ToList();
        UserDialogs.Instance.HideLoading();
    }
}