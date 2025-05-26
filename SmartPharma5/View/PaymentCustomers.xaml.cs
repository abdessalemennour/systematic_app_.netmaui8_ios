using Acr.UserDialogs;
using DevExpress.Maui.Editors;
using SmartPharma5.Model;
using SmartPharma5.ModelView;
using SmartPharma5.ViewModel;
using System.Globalization;

namespace SmartPharma5.View;

public partial class PaymentCustomers : ContentPage
{
	public PaymentCustomers()
	{
		InitializeComponent();
		BindingContext = new PaymentCustomersVM();

    }


    public void AutoCompleteEdit_TextChanged(object sender, AutoCompleteEditTextChangedEventArgs e)
    {
        AutoCompleteEdit edit = sender as AutoCompleteEdit;
        var search = edit.Text.ToLowerInvariant().ToString();
        var list = BindingContext as PaymentCustomersVM;


        if (string.IsNullOrWhiteSpace(search))
        {
            ClientCollectionView.ItemsSource = list.Partners.ToList();
        }
        else
        {
            ClientCollectionView.ItemsSource = list.Partners.Where(i => i.Name.ToLowerInvariant().Contains(search)).ToList();
        }

    }



    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        UserDialogs.Instance.Toast("Add New Payment ...");
        UserDialogs.Instance.ShowLoading("Loading please wait ...");
        await Task.Delay(200);
        if (sender is Frame frame && frame.BindingContext is Partner partner)
        {
            var ovm = BindingContext as PaymentCustomersVM;
            
            await Task.Delay(500);
            uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
            ovm.Payment = new Payment((int)idagent, partner as Partner);
            await App.Current.MainPage.Navigation.PushAsync(new PaymentView(ovm.Payment));
            CurrentData.CurrentNoteModule = "Commercial.Payment";
            CurrentData.CurrentActivityModule = "Commercial.Payment, Commercial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            CurrentData.CurrentFormModule = "AtooERP_Standard.Payment.Payment_update, AtooERP_Standard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";


        }
        UserDialogs.Instance.HideLoading();


    }
}

public class BooleanToImageConverter : IValueConverter
{
    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
        bool isAscending = (bool)value;
        return isAscending ? "up_filled.png" : "up.png"; // Adjust based on the button state
    }

    

    public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

   
}