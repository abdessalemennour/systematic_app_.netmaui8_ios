using DevExpress.Maui.Editors;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;
using System.Runtime.CompilerServices;
using static SmartPharma5.Model.Opportunity;

namespace SmartPharma5.View;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class PaymentListView : ContentPage
{
    bool isMy = false;
    public PaymentListView()
    {
        InitializeComponent();
        BindingContext = new PaymentListViewModel();

    }
    public PaymentListView(int agent)
    {
        InitializeComponent();
        isMy = true;
        BindingContext = new PaymentListViewModel(agent);
    }


    private void AutoCompleteEdit_TextChanged(object sender, DevExpress.Maui.Editors.AutoCompleteEditTextChangedEventArgs e)
    {
        AutoCompleteEdit edit = sender as AutoCompleteEdit;
        var search = edit.Text.ToLowerInvariant().ToString();
        var OVM = BindingContext as PaymentListViewModel;


        if (string.IsNullOrWhiteSpace(search))
        {
            PaymentCollectionView.ItemsSource = OVM.PaymentList.ToList();
        }
        else
        {
            PaymentCollectionView.ItemsSource = OVM.PaymentList.Where(i => (i.code.ToLowerInvariant().Contains(search)) || (i.NameAgent.ToLowerInvariant().Contains(search)) || (i.code.ToLowerInvariant().Contains(search)) || (i.PayementMethod.ToLowerInvariant().Contains(search)) || (i.Partner.ToLowerInvariant().Contains(search)) || (i.Amount.ToString().ToLowerInvariant().Contains(search)) || (i.date.ToString().ToLowerInvariant().Contains(search)) || (i.Due_date.ToString().ToLowerInvariant().Contains(search))).ToList();
        }

    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Payment.Collection partner)
        {
            var ovm = BindingContext as PaymentListViewModel;
            ovm.Loading = true;
            await Task.Delay(500);
            if (partner != null)
            {
                if (DbConnection.Connecter())
                {
                    var payment = partner as Payment.Collection;

                    ovm.Payment = new Payment(payment.Id);
                    CurrentData.CurrentNoteModule = "Commercial.Payment";
                    CurrentData.CurrentActivityModule = "Commercial.Payment, Commercial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    CurrentData.CurrentFormModule = "AtooERP_Standard.Payment.Payment_update, AtooERP_Standard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

                    // CurrentData.CurrentDocumentModule = "CRM.Opportunity, CRM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    CurrentData.CurrentModuleId = payment.Id;
                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new  PayView(ovm.Payment)));
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "Ok");
                }
            }
            ovm.Loading = false;

        }
    }
}