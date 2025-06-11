/*using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class PayView : ContentPage
{
    public Payment Payment { get; set; }

    public PayView()
    {
        InitializeComponent();
        Payment = new Payment(); // Assurez-vous d'initialiser Payment
        BindingContext = new PaymentViewModel(Payment);
    }

    public PayView(Payment payment)
    {
        InitializeComponent();
        Payment = payment ?? throw new ArgumentNullException(nameof(payment), "Payment ne peut pas �tre null.");
        BindingContext = new PaymentViewModel(payment);
    }

    private async void OnPaymentButtonClicked(object sender, EventArgs e)
    {
        try
        {
            if (Payment == null || Payment.Id == 0)
            {
                await DisplayAlert("Erreur", "Aucun paiement valide s�lectionn�.", "OK");
                return;
            }

            // Navigation vers PaymentFileSelectionView avec le paiement
            await Navigation.PushAsync(new PaymentFileSelectionView(Payment));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur dans OnPaymentButtonClicked : {ex.Message}");
            await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
        }
    }


}*/
/*
 using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class PayView : ContentPage
{
    public PayView()
    {
        InitializeComponent();
        //BindingContext = new PaymentViewModel();
    }
    public PayView(Payment payment)
    {
        InitializeComponent();
        BindingContext = new PaymentViewModel(payment);
    }
    private void DataGridView_Tap(object sender, DevExpress.Maui.DataGrid.DataGridGestureEventArgs e)
    {
        var ovm = BindingContext as ViewModel.PaymentViewModel;
        ovm.Payment.SetAmount();
    }
}*/

using SmartPharma5.Model;
using SmartPharma5.ViewModel;
using Acr.UserDialogs;

namespace SmartPharma5.View;
[XamlCompilation(XamlCompilationOptions.Compile)]

public partial class PayView : ContentPage
{
    public Payment Payment;
    public PayView()
    {
        InitializeComponent();
        //BindingContext = new PaymentViewModel();
    }
    public PayView(Payment payment)
    {
        InitializeComponent();
        Payment = payment; // Assurez-vous de stocker le paiement
        BindingContext = new PaymentViewModel(payment);
    }

    private void DataGridView_Tap(object sender, DevExpress.Maui.DataGrid.DataGridGestureEventArgs e)
    {
        var ovm = BindingContext as ViewModel.PaymentViewModel;
        ovm.Payment.SetAmount();
    }
    /**************************/
    private bool isSwipeOpen = false;

    private void OnSwipeViewClicked(object sender, EventArgs e)
    {
        if (isSwipeOpen)
        {
            MainSwipeView.Close();
        }
        else
        {
            MainSwipeView.Open(OpenSwipeItem.RightItems);
        }

        isSwipeOpen = !isSwipeOpen;
    }


    private bool _refreshEnabled;
    private bool _firstAppear = true;

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Activer le rafra�chissement p�riodique
        _refreshEnabled = true;

        // Ex�cuter le premier rafra�chissement
        await RefreshMessages();

        // D�marrer le timer pour les rafra�chissements p�riodiques
        Device.StartTimer(TimeSpan.FromSeconds(3), () =>
        {
            if (!_refreshEnabled) return false;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await RefreshMessages();
            });
            return _refreshEnabled;
        });

        // Jouer l'animation seulement au premier affichage
        if (_firstAppear)
        {
            _firstAppear = false;
            await AnimateSwipeHint();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // D�sactiver le rafra�chissement p�riodique
        _refreshEnabled = false;
    }

    private async Task RefreshMessages()
    {
        try
        {
            int userId = Preferences.Get("iduser", 0);
            int unreadCount = await UserModel.GetUnreadMessagesCountAsync(
                userId,
                CurrentData.CurrentModuleId,
                CurrentData.CurrentNoteModule);

            Device.BeginInvokeOnMainThread(() =>
            {
                UnreadBadgeLabel.Text = unreadCount.ToString();
                UnreadBadgeFrame.IsVisible = unreadCount > 0;
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du rafra�chissement: {ex.Message}");
        }
    }

    private async Task AnimateSwipeHint()
    {
        // Faire appara�tre l'indicateur
        await swipeHint.FadeTo(0.8, 300);

        // Animation de balayage (2 cycles)
        for (int i = 0; i < 2; i++)
        {
            await swipeHint.TranslateTo(-20, 0, 300, Easing.SinInOut);
            await swipeHint.TranslateTo(0, 0, 300, Easing.SinInOut);
        }

        // Rendre l'indicateur plus discret
        await swipeHint.FadeTo(0.3, 300);
    }
    //private async void OnSaveDocumentClicked(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        if (Payment == null || Payment.Id == 0)
    //        {
    //            await DisplayAlert("Erreur", "Aucun paiement valide s�lectionn�.", "OK");
    //            return;
    //        }

    //       // await Navigation.PushAsync(new PaymentFileSelectionView(Payment));
    //        await Navigation.PushAsync(new FileSelectionView(Payment.Id, EntityType.Payment));

    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Erreur dans OnPaymentButtonClicked : {ex.Message}");
    //        await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
    //    }
    //}







    /*
      private async void OnSaveDocumentClicked(object sender, EventArgs e)
    {
        try
        {
            if (Payment == null || Payment.Id == 0)
            {
                await DisplayAlert("Erreur", "Aucun paiement valide s�lectionn�.", "OK");
                return;
            }

            // Passer l'ID de Payment � la vue de s�lection de fichiers
            await Navigation.PushAsync(new FileSelectionView(Payment.Id));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur dans OnPaymentButtonClicked : {ex.Message}");
            await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
        }
    }*/
    private async void OnActionButtonClickedmemo(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        UserDialogs.Instance.ShowLoading("Loading...");
        await Task.Delay(200);
        if (button.Source.ToString().Contains("note3.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.MemoView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqu� sur {buttonName}", "OK");
        }
        UserDialogs.Instance.HideLoading();

    }
    private async void OnActionButtonClickedactivity(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        //UserDialogs.Instance.ShowLoading("Loading...");
        //await Task.Delay(200);
        if (button.Source.ToString().Contains("activity.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.ActivityView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqu� sur {buttonName}", "OK");
        }
        //UserDialogs.Instance.HideLoading();

    }
    private async void OnActionButtonClickedchat(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        UserDialogs.Instance.ShowLoading("Chargement...");
        await Task.Delay(400);
        if (button.Source.ToString().Contains("chat.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.ChatView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqu� sur {buttonName}", "OK");
        }
        UserDialogs.Instance.HideLoading();

    }

    private async void OnActionButtonClickedreport(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        UserDialogs.Instance.ShowLoading("Loading...");
        await Task.Delay(200);
        if (button.Source.ToString().Contains("rapport.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.ReportView());
        }
        else
        {
            /* string buttonName = button.Source.ToString().Replace("File: ", "");
             await DisplayAlert("Action", $"Vous avez cliqu� sur {buttonName}", "OK");*/
        }
        UserDialogs.Instance.HideLoading();

    }

    private async void OnActionButtonClickedlogs(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        if (button.Source.ToString().Contains("logs.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.LogsView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqu� sur {buttonName}", "OK");
        }
    }

    //private async void onactionbuttonclickedchat(object sender, eventargs e)
    //{
    //    var button = sender as imagebutton;

    //    userdialogs.instance.showloading("loading...");
    //    await task.delay(200);
    //    if (button.source.tostring().contains("chat.png"))
    //    {
    //        await navigation.pushasync(new floatingactionbutton.chatview());
    //    }
    //    else
    //    {
    //        string buttonname = button.source.tostring().replace("file: ", "");
    //        await displayalert("action", $"vous avez cliqu� sur {buttonname}", "ok");
    //    }
    //    userdialogs.instance.hideloading();

    //}
    private async void OnActionButtonClickeddocument(object sender, EventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading...");

        try
        {
            var payment = this.Payment; // Assurez-vous que cette propri�t� est accessible

            // Appeler la m�me navigation que l'ancien bouton
            await Navigation.PushAsync(new FileSelectionView(payment.Id, EntityType.Payment));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Une erreur est survenue: {ex.Message}", "OK");
        }
        finally
        {
            UserDialogs.Instance.HideLoading();
        }
    }
}