using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;
using System;
using System.Threading.Tasks;

namespace SmartPharma5.View;



public partial class HomeView : ContentPage
{
    private bool _isChatVisible = false;
    /*******************************/
    private async void OnChatButtonClicked(object sender, EventArgs e)
    {
        // Naviguer vers la page de chat
        await Navigation.PushAsync(new ChatNotifView());

        // Ou vous pourriez ouvrir la page de chat comme une modale
        // await Navigation.PushModalAsync(new ChatPage());
    }
    private async void OnActivityButtonClicked(object sender, EventArgs e)
    {
        // Naviguer vers la page de chat
        await Navigation.PushAsync(new ActivityNotifView());

        // Ou vous pourriez ouvrir la page de chat comme une modale
        // await Navigation.PushModalAsync(new ChatPage());
    }
    private async void OnMoreButtonClicked(object sender, EventArgs e)
    {
        if (DeviceInfo.Platform != DevicePlatform.iOS)
            return;
        var vm = BindingContext as HomeViewModel;
        if (vm == null) return;

        var action = await DisplayActionSheet("Options", "Annuler", null,
            "Ajouter une connexion",
            "Gérer les connexions",
            "Déconnexion");

        switch (action)
        {
            case "Ajouter une connexion":
                if (vm.AddCommand?.CanExecute(null) ?? false)
                    await vm.AddCommand.ExecuteAsync();
                break;

            case "Gérer les connexions":
                if (vm.ManageConnectionsCommand?.CanExecute(null) ?? false)
                    vm.ManageConnectionsCommand.Execute(null); // standard ICommand
                break;

            case "Déconnexion":
                if (vm.LogoutCommand?.CanExecute(null) ?? false)
                    await vm.LogoutCommand.ExecuteAsync();
                break;
        }
    }
    /************************/
    public HomeView()
    {
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            IsVisible = false
        });

        InitializeComponent();

        var grid = globle_grid as Grid;
        user_contrat.getBtnIInvisibles().GetAwaiter();

        /******************/
        this.Loaded += OnPageLoaded;

        // S'abonner aux changements de la collection
        if (BindingContext is HomeViewModel vm)
        {
            vm.SavedConnections.CollectionChanged += (s, e) =>
            {
                Device.BeginInvokeOnMainThread(UpdateConnectionToolbarItems);
            };
        }

        //if (DeviceInfo.Platform == DevicePlatform.Android)
        //{
        //    // Ajouter les ToolbarItems comme dans ta méthode UpdateConnectionToolbarItems
        //    UpdateConnectionToolbarItems();
        //}


        async void OnPageLoaded(object sender, EventArgs e)
        {
            await (BindingContext as HomeViewModel)?.LoadConnections();
            UpdateConnectionToolbarItems();
        }

        void UpdateConnectionToolbarItems()
        {
            if (DeviceInfo.Platform != DevicePlatform.Android)
                return; // Ne pas exécuter sur iOS, Windows, etc.

            var vm = BindingContext as HomeViewModel;
            if (vm == null) return;

            // Supprimer tous les items sauf "Logout" et "Ajouter une connexion"
            var itemsToRemove = this.ToolbarItems
                .Where(x => x.Text != "Logout" && x.Text != "Ajouter une connexion")
                .ToList();

            foreach (var item in itemsToRemove)
            {
                this.ToolbarItems.Remove(item);
            }

            // Ajouter "Ajouter une connexion" s'il n'existe pas
            if (!this.ToolbarItems.Any(x => x.Text == "Ajouter une connexion"))
            {
                this.ToolbarItems.Insert(0, new ToolbarItem
                {
                    Text = "Ajouter une connexion",
                    Order = ToolbarItemOrder.Secondary,
                    Priority = 0,
                    Command = vm.AddCommand
                });
            }

            // Ajouter "Logout" s'il n'existe pas
            if (!this.ToolbarItems.Any(x => x.Text == "Logout"))
            {
                this.ToolbarItems.Add(new ToolbarItem
                {
                    Text = "Logout",
                    Order = ToolbarItemOrder.Secondary,
                    Priority = 1,
                    Command = vm.LogoutCommand
                });
            }

            // Ajouter "Gérer les connexions" s'il n'existe pas
            if (!this.ToolbarItems.Any(x => x.Text == "Gérer les connexions"))
            {
                this.ToolbarItems.Insert(0, new ToolbarItem
                {
                    Text = "Gérer les connexions",
                    Order = ToolbarItemOrder.Secondary,
                    Priority = 0,
                    Command = vm.ManageConnectionsCommand
                });
            }

            // Ajouter les connexions enregistrées
            foreach (var connection in vm.SavedConnections)
            {
                this.ToolbarItems.Add(new ToolbarItem
                {
                    Text = connection.CustomName,
                    Order = ToolbarItemOrder.Secondary,
                    Command = vm.SelectConnectionCommand,
                    CommandParameter = connection
                });
            }
        }
        /******************/

        foreach (string btn in user_contrat.ListInVisibleBtn)
        {
            try
            {
                StackLayout st1 = (StackLayout)FindByName(btn);
                st1.IsVisible = false;
            }catch (Exception ex)
            {

            }
            
        }


        /*   Grid gridInsideFrame = (Grid)frame.Content;
      StackLayout stackLayout1 = (StackLayout)gridInsideFrame.FindByName("achat_all_opportunity");
      if (stackLayout1 != null)
      {

      }*/

        try
        {
            var logo = user_contrat.GetLogoFromDatabase();
            Image.Source = ByteArrayToImageConverter.Convert(logo);

            //Popup.IsOpen = false;
            //Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

            var ovm = BindingContext as HomeViewModel;
            TitleValidation.Text = "(" + ovm.LoadNumbers().Result.ToString() + ") Validations";

            var a = Shell.Current;

        }
        catch(Exception ex)
        {

        }

    
    }
    /******************************/
    private bool _refreshEnabled;
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _refreshEnabled = true;
        await RefreshMessages();
        Device.StartTimer(TimeSpan.FromSeconds(3), () =>
        {
            if (!_refreshEnabled) return false;
            if (!_refreshEnabled) return false;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await RefreshMessages();
            });
            return _refreshEnabled; // Continue si true
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _refreshEnabled = false;
    }
    private async Task RefreshMessages()
    {
        try
        {
            // Rafraîchir le compteur de messages non lus
            int userId = Preferences.Get("iduser", 0);
            int unreadCount = await UserModel.GetAllUnreadMessagesCountAsync(userId);

            Device.BeginInvokeOnMainThread(() =>
            {
                if (unreadCount > 0)
                {
                    UnreadBadgeLabel.Text = unreadCount.ToString();
                    UnreadBadgeFrame.IsVisible = true;
                }
                else
                {
                    UnreadBadgeFrame.IsVisible = false;
                }
            });


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du rafraîchissement: {ex.Message}");
        }
    }
    /****************************/

    protected override bool OnBackButtonPressed()
    {
        App.Current.MainPage.DisplayAlert("INFO", "Go To Menu", "OK");
        return true;
    }

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        ScrolView.ScrollToAsync(0, 0, true);
        var OVM = BindingContext as HomeViewModel;
        OVM.MenuVisisble = !OVM.MenuVisisble;
    }


    private async void TapGestureRecognizer_Tapped_sales(object sender, TappedEventArgs e)
    {
        var OVM = BindingContext as HomeViewModel;
        OVM.MenuVisisble = !OVM.MenuVisisble;
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new ShowMenuItemPages()));
        UserDialogs.Instance.HideLoading();
    }
    private async void TapGestureRecognizer_Tapped_payment(object sender, TappedEventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new PaymentModule()));
        UserDialogs.Instance.HideLoading();
    }
    private async void TapGestureRecognizer_Tapped_marketing(object sender, TappedEventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        //await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new module()));
        UserDialogs.Instance.HideLoading();

    }

    private async void TapGestureRecognizer_Tapped_profiling(object sender, TappedEventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new ProfilingModule()));
        UserDialogs.Instance.HideLoading();

    }
    private async void TapGestureRecognizer_Tapped_summary(object sender, TappedEventArgs e)
    {

        
        var OVM = BindingContext as HomeViewModel;
        OVM.MenuVisisble = !OVM.MenuVisisble;
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        if (OVM.AllOppIsVisible)
        {

            await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new SammaryView()));
        }
        else
        {
            await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new SammaryView((uint)Preferences.Get("idagent", Convert.ToUInt32(null)))));
       }
       
        UserDialogs.Instance.HideLoading();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new PieDashboard()));
        UserDialogs.Instance.HideLoading();
    }
}