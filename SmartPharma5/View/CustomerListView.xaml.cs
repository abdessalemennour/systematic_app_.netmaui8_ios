using DevExpress.Maui.Editors;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class CustomerListView : ContentPage
{
    Opportunity Opportunity { get; set; }
    public CustomerListView()
    {

        InitializeComponent();
        BindingContext = new CustomerViewModel();


    }
    public void AutoCompleteEdit_TextChanged(object sender, AutoCompleteEditTextChangedEventArgs e)
    {
        AutoCompleteEdit edit = sender as AutoCompleteEdit;
        var search = edit.Text.ToLowerInvariant().ToString();
        var list = BindingContext as CustomerViewModel;


        if (string.IsNullOrWhiteSpace(search))
        {
            ClientCollectionView.ItemsSource = list.ClientList.ToList();
        }
        else
        {
            ClientCollectionView.ItemsSource = list.ClientList.Where(i => i.Name.ToLowerInvariant().Contains(search)).ToList();
        }

    }

    private void ClientCollectionView_PullToRefresh(object sender, EventArgs e)
    {

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ClientCollectionView.SelectedItem = null;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Partner partner)
        {
           
                uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
                Opportunity = new Opportunity((int)idagent, partner as Partner);
                await App.Current.MainPage.Navigation.PushAsync(new OpportunityView(Opportunity));
           

        }
    }
} 