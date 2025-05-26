using DevExpress.Maui.Editors;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class PaymentCustomerListView : ContentPage
{
    public PaymentCustomerListView()
    {
        InitializeComponent();
    }
    public PaymentCustomerListView(Payment Payment)
    {
        InitializeComponent();
        BindingContext = new PaymentCustomerViewModel(Payment);
    }

    private void AutoCompleteEdit_TextChanged(object sender, DevExpress.Maui.Editors.AutoCompleteEditTextChangedEventArgs e)
    {
        AutoCompleteEdit edit = sender as AutoCompleteEdit;
        var search = edit.Text.ToLowerInvariant().ToString();
        var list = BindingContext as PaymentCustomerViewModel;


        if (string.IsNullOrWhiteSpace(search))
        {
            ClientCollectionView.ItemsSource = list.ClientList.ToList();
        }
        else
        {
            ClientCollectionView.ItemsSource = list.ClientList.Where(i => i.Name.ToLowerInvariant().Contains(search)).ToList();
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Partner partner)
        {
            var ovm = BindingContext as PaymentCustomerViewModel;
            ovm.Loading = true;
            await Task.Delay(500);
            uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
            ovm.Payment = new Payment((int)idagent, partner as Partner);
            await App.Current.MainPage.Navigation.PushAsync(new PaymentView(ovm.Payment));
            ovm.Loading = false;

        }
    }
}