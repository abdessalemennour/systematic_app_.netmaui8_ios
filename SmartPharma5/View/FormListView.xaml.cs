using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class FormListView : ContentPage
{
    public Partner Partner { get; set; }
    public int contact_id { get; set; } = 0;
    public int opportunity_id { get; set; } = 0;
    public FormListView()
    {
        InitializeComponent();
        BindingContext = new FormListViewModel();
        var OVM = BindingContext as FormListViewModel;
        //Task.Run(()=> OVM.Reload());
    }
    public FormListView(Partner partner)
    {
        Partner = partner;
        InitializeComponent();
        BindingContext = new FormListViewModel(Partner);
        //var OVM = BindingContext as FormListViewModel;
        //OVM.Partner= partner;
        //Task.Run(()=> OVM.Reload().GetAwaiter());
    }
    public FormListView(int category)
    {
        //Partner = partner;
        InitializeComponent();
        BindingContext = new FormListViewModel(category);
        var OVM = BindingContext as FormListViewModel;
        //Task.Run(()=> OVM.Reload());
    }
    public FormListView(Partner partner,int contact_id)
    {
        Partner = partner;
        this.contact_id=contact_id;
        InitializeComponent();
        BindingContext = new FormListViewModel(Partner);
    }
    public FormListView(Partner partner, int contact_id,int opp)
    {
        this.opportunity_id = opp;
        Partner = partner;
        this.contact_id = contact_id;
        InitializeComponent();
        BindingContext = new FormListViewModel(Partner);
    }

    public async void DXCollectionView_Tap(object sender, DevExpress.Maui.CollectionView.CollectionViewGestureEventArgs e)
    {

        var Item = e.Item as Form;
        if (Item != null)
        {
            try
            {
                //SavingPopup.IsOpen=true;
                //await Task.Delay(1000);
                int? r;
                if (opportunity_id != 0)
                {
                     r = Partner_Form.InsertPartnerFormWithOppAndGetLastId(Partner, Item,this.opportunity_id,this.contact_id).Result;
                }
                else
                {
                     r = Partner_Form.InsertPartnerFormAndGetLastId(Partner, Item).Result;
                }
                
                int? idpartenerform =  r;
                if (idpartenerform != null)
                {
                    var p = Partner_Form.GetPartnerFormById(idpartenerform);
                    SavingPopup.IsOpen = false;
                    SuccessPopup.IsOpen = true;
                    await Task.Delay(1000);
                    SuccessPopup.IsOpen = false;
                    await App.Current.MainPage.Navigation.PushAsync(new QuizQuestionView(p));

                }
                else
                {
                    Popup.IsOpen = true;
                    //SuccessPopup.IsOpen = false;


                }
            }
            catch
            {
                Popup.IsOpen = true;
                // SuccessPopup.IsOpen = false;
                // await DisplayAlert("Warning", "There is a problem in the server", "Ok");

            }

        }
        else
        {
            Popup.IsOpen = true;
            //await DisplayAlert("Warrning", "Select a Cycle", "Ok");
        }
        //Partner_Form partner_form = new Partner_Form()
        //await App.Current.MainPage.Navigation.PushAsync(new FormListView(Item));



    }

    private async void OnItemTapped(object sender, EventArgs e)
    {





        UserDialogs.Instance.Toast("New Quiz ...");
        await Task.Delay(200);

        if (await DbConnection.Connecter3())
        {

            try
            {
                SavingPopup.IsOpen = false;
                SuccessPopup.IsOpen = true;
                await Task.Delay(100);
                if (sender is Frame frame && frame.BindingContext is Form tappedItem)
                {
                    try
                    {
                        //SavingPopup.IsOpen=true;
                        //await Task.Delay(1000);

                        int? r;
                        if (opportunity_id != 0)
                        {
                            r = Partner_Form.InsertPartnerFormWithOppAndGetLastId(Partner, tappedItem, this.opportunity_id, this.contact_id).Result;
                        }
                        else
                        {
                            r = Partner_Form.InsertPartnerFormAndGetLastId(Partner, tappedItem).Result;
                        }

                       
                        int? idpartenerform =  r;
                        if (idpartenerform != null)
                        {
                            var p = Partner_Form.GetPartnerFormById(idpartenerform);

                            await App.Current.MainPage.Navigation.PushAsync(new QuizQuestionView(p));
                            SuccessPopup.IsOpen = false;


                        }
                        else
                        {
                            Popup.IsOpen = true;
                            //SuccessPopup.IsOpen = false;


                        }
                    }
                    catch
                    {
                        Popup.IsOpen = true;
                        // SuccessPopup.IsOpen = false;
                        // await DisplayAlert("Warning", "There is a problem in the server", "Ok");

                    }
                }

            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
                UserDialogs.Instance.HideLoading();
            }

        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "OK");

        }










       
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();


    }
}