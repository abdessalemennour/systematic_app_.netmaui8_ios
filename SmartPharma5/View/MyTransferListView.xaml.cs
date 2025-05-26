using DevExpress.Maui.Editors;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MyTransferListView : ContentPage
{
    public MyTransferListView()
    {
        InitializeComponent();
        BindingContext = new MyTransferListViewModel();
    }
    public MyTransferListView(string ch)
    {
        InitializeComponent();
        BindingContext = new MyTransferListViewModel(ch);
    }
    public MyTransferListView(CashDesk cd)
    {
        InitializeComponent();
        BindingContext = new MyTransferListViewModel(cd);
    }
    public MyTransferListView(int i)
    {
        InitializeComponent();
        BindingContext = new MyTransferListViewModel(i);
    }
    private void AutoCompleteEdit_TextChanged(object sender, DevExpress.Maui.Editors.AutoCompleteEditTextChangedEventArgs e)
    {
        AutoCompleteEdit edit = sender as AutoCompleteEdit;
        var search = edit.Text.ToLowerInvariant().ToString();
        var OVM = BindingContext as MyTransferListViewModel;


        if (string.IsNullOrWhiteSpace(search))
        {
            PaymentCollectionView.ItemsSource = OVM.TransferList.ToList();
        }
        else
        {
            PaymentCollectionView.ItemsSource = OVM.TransferList.Where(i => (i.cash_desk_out.ToLowerInvariant().Contains(search)) || (i.cash_desk_in.ToLowerInvariant().Contains(search)) || (i.state.ToLowerInvariant().Contains(search)) || (i.Amount.ToString().ToLowerInvariant().Contains(search)) || (i.create_date.ToString().ToLowerInvariant().Contains(search))).ToList();
        }

    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is SmartPharma5.Model.Transfer.Collection collection)
        {
            var OVM = BindingContext as MyTransferListViewModel;


            OVM. Loading = true;
            await Task.Delay(500);

            if (collection != null)
            {
                if (DbConnection.Connecter())
                {
                    //var Tr = collection as Collection;

                    Transfer transfer = new Transfer(collection.Id);

                    await App.Current.MainPage.Navigation.PushAsync(new TransferStateView(transfer));
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "Ok");
                }
            }
            OVM.Loading = false;

        }


            /* Loading = true;
                await Task.Delay(500);

                if (obj != null)
                {
                    if (DbConnection.Connecter())
                    {
                        var Tr = obj as Collection;

                        Transfer transfer = new Transfer(Tr.Id);

                        await App.Current.MainPage.Navigation.PushAsync(new TransferStateView(transfer));
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "Ok");
                    }
                }
                Loading = false; */
        }
}