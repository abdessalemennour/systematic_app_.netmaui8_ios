using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ModelView;

namespace SmartPharma5.View;

public partial class EditContactPage : ContentPage
{
	public int idContact;


    public EditContactPage(int idContact)
	{
		this.idContact = idContact;
		InitializeComponent();
		BindingContext = new editContactPageMV(idContact);
    }
    private async void SimpleButton_Clicked(object sender, EventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading, please wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PopAsync();
        UserDialogs.Instance.HideLoading();
    }


}