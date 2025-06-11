using Acr.UserDialogs;
using DevExpress.Maui.Editors;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;
using DevExpress.Maui.Controls;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using CommunityToolkit.Maui.Views;
using Syncfusion.Maui.NavigationDrawer;
using Microsoft.Maui.Platform;


namespace SmartPharma5.View;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class OpportunityView : ContentPage
{
    public Opportunity Opportunity;
    private bool isDragging = false;
    int totalMessages = UserModel.TotalUnreadMessages;
    private string fileName;
    private string filePath;
    private bool _isMenuOpen = false;
    private const uint AnimationDuration = 250;
    private bool isSwipeViewOpen = false;
    private double initialX;
    private bool isSwiping = false;
    private double swipeStartX;
    private ChatViewModel _vm;

    public OpportunityView()
    {
        InitializeComponent();

        BindingContext = new DocumentViewModel();
        Console.WriteLine($"Total des messages non lus : {totalMessages}");
        //Loaded += OpportunityView_Loaded;


    }
    //protected override async void OnAppearing()
    //{
    //    base.OnAppearing();

    //    int userId = Preferences.Get("iduser", 0);
    //    int unreadCount = await UserModel.GetUnreadMessagesCountAsync(userId,CurrentData.CurrentModuleId,CurrentData.CurrentNoteModule);

    //    if (unreadCount > 0)
    //    {
    //        UnreadBadgeLabel.Text = unreadCount.ToString();
    //        UnreadBadgeFrame.IsVisible = true;
    //    }
    //    else
    //    {
    //        UnreadBadgeFrame.IsVisible = false;
    //    }
    //}
    /******************************/
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

        // Activer le rafraîchissement périodique
        _refreshEnabled = true;

        // Exécuter le premier rafraîchissement
        await RefreshMessages();

        // Démarrer le timer pour les rafraîchissements périodiques
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
        // Désactiver le rafraîchissement périodique
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
            Console.WriteLine($"Erreur lors du rafraîchissement: {ex.Message}");
        }
    }

    private async Task AnimateSwipeHint()
    {
        // Faire apparaître l'indicateur
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
    /****************************/

    private async void OnMenuClicked(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet("Options", "Annuler", null, "Modifier", "Supprimer", "Partager");

        if (action == "Modifier")
        {
            await DisplayAlert("Action", "Vous avez choisi Modifier", "OK");
        }
        else if (action == "Supprimer")
        {
            await DisplayAlert("Action", "Vous avez choisi Supprimer", "OK");
        }
        else if (action == "Partager")
        {
            await DisplayAlert("Action", "Vous avez choisi Partager", "OK");
        }
    }
    /*********************floating action button******************/
    //private double _startX, _startY; // Position initiale du bouton

    //private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    //{
    //    switch (e.StatusType)
    //    {
    //        case GestureStatus.Started:
    //            _startX = MainButton.TranslationX;
    //            _startY = MainButton.TranslationY;
    //            MainButton.ZIndex = 10;

    //            break;

    //        case GestureStatus.Running:
    //            MainButton.TranslationX = _startX + e.TotalX;
    //            MainButton.TranslationY = _startY + e.TotalY;
    //            break;

    //        case GestureStatus.Completed:
    //            // MAJ position pour empêcher le débordement hors écran
    //            var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
    //            var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
    //            MainButton.TranslationX = Math.Max(0, Math.Min(screenWidth - MainButton.Width, MainButton.TranslationX));
    //            MainButton.TranslationY = Math.Max(0, Math.Min(screenHeight - MainButton.Height, MainButton.TranslationY));
    //            // Mettre à jour la position des boutons secondaires après déplacement
    //            UpdateSecondaryButtonsPosition();
    //            break;
    //        case GestureStatus.Canceled:
    //            // 🔹 Optionnel : Réinitialiser ZIndex après le déplacement
    //            MainButton.ZIndex = 0;
    //            break;
    //    }
    //}

    //private async void OnMainButtonClicked(object sender, EventArgs e)
    //{
    //    if (ActionButton1Container.IsVisible)
    //    {
    //        await Task.WhenAll(
    //            ActionButton1Container.TranslateTo(MainButton.TranslationX, MainButton.TranslationY, 200, Easing.SinIn),
    //            ActionButton2Container.TranslateTo(MainButton.TranslationX, MainButton.TranslationY, 200, Easing.SinIn),
    //            ActionButton3Container.TranslateTo(MainButton.TranslationX, MainButton.TranslationY, 200, Easing.SinIn),
    //            ActionButton4Container.TranslateTo(MainButton.TranslationX, MainButton.TranslationY, 200, Easing.SinIn),
    //            ActionButton1Container.FadeTo(0, 200),
    //            ActionButton2Container.FadeTo(0, 200),
    //            ActionButton3Container.FadeTo(0, 200),
    //            ActionButton4Container.FadeTo(0, 200),
    //            ActionButton1Container.RotateTo(-360, 200),
    //            ActionButton2Container.RotateTo(-360, 200),
    //            ActionButton3Container.RotateTo(-360, 200),
    //            ActionButton4Container.RotateTo(-360, 200)
    //        );

    //        await Task.Delay(200);
    //        ActionButton1Container.IsVisible = false;
    //        ActionButton2Container.IsVisible = false;
    //        ActionButton3Container.IsVisible = false;
    //        ActionButton4Container.IsVisible = false;
    //    }
    //    else
    //    {
    //        UpdateSecondaryButtonsPosition();

    //        ActionButton1Container.Opacity = 0;
    //        ActionButton2Container.Opacity = 0;
    //        ActionButton3Container.Opacity = 0;
    //        ActionButton4Container.Opacity = 0;

    //        ActionButton1Container.IsVisible = true;
    //        ActionButton2Container.IsVisible = true;
    //        ActionButton3Container.IsVisible = true;
    //        ActionButton4Container.IsVisible = true;
    //        ActionButton1Container.ZIndex = 10;
    //        ActionButton2Container.ZIndex = 10;
    //        ActionButton3Container.ZIndex = 10;
    //        ActionButton4Container.ZIndex = 10;

    //        await Task.WhenAll(
    //            ActionButton1Container.FadeTo(1, 200),
    //            ActionButton2Container.FadeTo(1, 200),
    //            ActionButton3Container.FadeTo(1, 200),
    //            ActionButton4Container.FadeTo(1, 200),
    //            ActionButton1Container.TranslateTo(ActionButton1Container.TranslationX, ActionButton1Container.TranslationY, 200, Easing.SinOut),
    //            ActionButton2Container.TranslateTo(ActionButton2Container.TranslationX, ActionButton2Container.TranslationY, 200, Easing.SinOut),
    //            ActionButton3Container.TranslateTo(ActionButton3Container.TranslationX, ActionButton3Container.TranslationY, 200, Easing.SinOut),
    //            ActionButton4Container.TranslateTo(ActionButton4Container.TranslationX, ActionButton4Container.TranslationY, 200, Easing.SinOut),
    //            ActionButton1Container.RotateTo(360, 200),
    //            ActionButton2Container.RotateTo(360, 200),
    //            ActionButton3Container.RotateTo(360, 200),
    //            ActionButton4Container.RotateTo(360, 200)
    //        );
    //    }
    //}

    //private void UpdateSecondaryButtonsPosition()
    //{
    //    double mainX = MainButton.TranslationX;
    //    double mainY = MainButton.TranslationY;
    //    double horizontalSpacing = -50;

    //    ActionButton1Container.TranslationX = mainX + horizontalSpacing * 1;
    //    ActionButton1Container.TranslationY = mainY;

    //    ActionButton2Container.TranslationX = mainX + horizontalSpacing * 2;
    //    ActionButton2Container.TranslationY = mainY;

    //    ActionButton3Container.TranslationX = mainX + horizontalSpacing * 3;
    //    ActionButton3Container.TranslationY = mainY;

    //    ActionButton4Container.TranslationX = mainX + horizontalSpacing * 4;
    //    ActionButton4Container.TranslationY = mainY;
    //}



    //private async void OnActionButtonClickedmemo(object sender, EventArgs e)
    //{
    //    var button = sender as ImageButton;

    //    UserDialogs.Instance.ShowLoading("Loading...");
    //    await Task.Delay(200);
    //    if (button.Source.ToString().Contains("note3.png"))
    //    {
    //        await Navigation.PushAsync(new FloatingActionButton.MemoView());
    //    }
    //    else
    //    {
    //        string buttonName = button.Source.ToString().Replace("File: ", "");
    //        await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
    //    }
    //    UserDialogs.Instance.HideLoading();

    //}
    //private async void OnActionButtonClickedactivity(object sender, EventArgs e)
    //{
    //    var button = sender as ImageButton;

    //    UserDialogs.Instance.ShowLoading("Loading...");
    //    await Task.Delay(200);
    //    if (button.Source.ToString().Contains("restore.png"))
    //    {
    //        await Navigation.PushAsync(new FloatingActionButton.ActivityView());
    //    }
    //    else
    //    {
    //        string buttonName = button.Source.ToString().Replace("File: ", "");
    //        await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
    //    }
    //    UserDialogs.Instance.HideLoading();

    //}
    //private async void OnActionButtonClickedchat(object sender, EventArgs e)
    //{
    //    var button = sender as ImageButton;

    //    UserDialogs.Instance.ShowLoading("Loading...");
    //    await Task.Delay(200);
    //    if (button.Source.ToString().Contains("circle.png"))
    //    {
    //        await Navigation.PushAsync(new FloatingActionButton.ChatView());
    //    }
    //    else
    //    {
    //        string buttonName = button.Source.ToString().Replace("File: ", "");
    //        await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
    //    }
    //    UserDialogs.Instance.HideLoading();

    //}
    //private async void OnActionButtonClickeddocument(object sender, EventArgs e)
    //{
    //    UserDialogs.Instance.ShowLoading("Loading...");

    //    try
    //    {
    //        var opportunity = this.Opportunity; // Assurez-vous que cette propriété est accessible

    //        // Appeler la même navigation que l'ancien bouton
    //        await Navigation.PushAsync(new FileSelectionView(opportunity.Id, EntityType.Opportunity));
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert("Error", $"Une erreur est survenue: {ex.Message}", "OK");
    //    }
    //    finally
    //    {
    //        UserDialogs.Instance.HideLoading();
    //    }
    //}





    //private double _startX, _startY; // Position initiale du bouton

    //private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    //{
    //    switch (e.StatusType)
    //    {
    //        case GestureStatus.Started:
    //            _startX = MainButton.TranslationX;
    //            _startY = MainButton.TranslationY;
    //            MainButton.ZIndex = 10;

    //            break;

    //        case GestureStatus.Running:
    //            MainButton.TranslationX = _startX + e.TotalX;
    //            MainButton.TranslationY = _startY + e.TotalY;
    //            break;

    //        case GestureStatus.Completed:
    //            // MAJ position pour empêcher le débordement hors écran
    //            var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
    //            var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
    //            MainButton.TranslationX = Math.Max(0, Math.Min(screenWidth - MainButton.Width, MainButton.TranslationX));
    //            MainButton.TranslationY = Math.Max(0, Math.Min(screenHeight - MainButton.Height, MainButton.TranslationY));
    //            // Mettre à jour la position des boutons secondaires après déplacement
    //            UpdateSecondaryButtonsPosition();
    //            break;
    //        case GestureStatus.Canceled:
    //            // 🔹 Optionnel : Réinitialiser ZIndex après le déplacement
    //            MainButton.ZIndex = 0;
    //            break;
    //    }
    //}

    //private async void OnMainButtonClicked(object sender, EventArgs e)
    //{
    //    if (ActionButton1.IsVisible)
    //    {
    //        await Task.WhenAll(
    //            ActionButton1.TranslateTo(MainButton.TranslationX, MainButton.TranslationY, 200, Easing.SinIn),
    //            ActionButton2.TranslateTo(MainButton.TranslationX, MainButton.TranslationY, 200, Easing.SinIn),
    //            ActionButton3.TranslateTo(MainButton.TranslationX, MainButton.TranslationY, 200, Easing.SinIn),
    //            ActionButton4.TranslateTo(MainButton.TranslationX, MainButton.TranslationY, 200, Easing.SinIn),
    //            ActionButton1.FadeTo(0, 200),
    //            ActionButton2.FadeTo(0, 200),
    //            ActionButton3.FadeTo(0, 200),
    //            ActionButton4.FadeTo(0, 200),
    //            ActionButton1.RotateTo(-360, 200),
    //            ActionButton2.RotateTo(-360, 200),
    //            ActionButton3.RotateTo(-360, 200),
    //            ActionButton4.RotateTo(-360, 200)

    //        );
    //        await Task.Delay(200); // Attendre la fin de l'animation avant de cacher les boutons

    //        ActionButton1.IsVisible = false;
    //        ActionButton2.IsVisible = false;
    //        ActionButton3.IsVisible = false;
    //        ActionButton4.IsVisible = false;

    //    }
    //    else
    //    {
    //        UpdateSecondaryButtonsPosition(); // Mettre à jour la position

    //        // 🔹 Rendre les boutons invisibles avant de les afficher
    //        ActionButton1.Opacity = 0;
    //        ActionButton2.Opacity = 0;
    //        ActionButton3.Opacity = 0;
    //        ActionButton4.Opacity = 0;

    //        // ✅ Afficher les boutons mais sans qu'ils apparaissent visiblement
    //        ActionButton1.IsVisible = true;
    //        ActionButton2.IsVisible = true;
    //        ActionButton3.IsVisible = true;
    //        ActionButton4.IsVisible = true;
    //        ActionButton1.ZIndex = 10;
    //        ActionButton2.ZIndex = 10;
    //        ActionButton3.ZIndex = 10;
    //        ActionButton4.ZIndex = 10;


    //        // ✅ Faire l'animation d'apparition (opacité + mouvement)
    //        await Task.WhenAll(
    //            ActionButton1.FadeTo(1, 200),
    //            ActionButton2.FadeTo(1, 200),
    //            ActionButton3.FadeTo(1, 200),
    //            ActionButton4.FadeTo(1, 200),
    //            ActionButton1.TranslateTo(ActionButton1.TranslationX, ActionButton1.TranslationY, 200, Easing.SinOut),
    //            ActionButton2.TranslateTo(ActionButton2.TranslationX, ActionButton2.TranslationY, 200, Easing.SinOut),
    //            ActionButton3.TranslateTo(ActionButton3.TranslationX, ActionButton3.TranslationY, 200, Easing.SinOut),
    //            ActionButton4.TranslateTo(ActionButton4.TranslationX, ActionButton4.TranslationY, 200, Easing.SinOut),
    //            ActionButton1.RotateTo(360, 200),
    //            ActionButton2.RotateTo(360, 200),
    //            ActionButton3.RotateTo(360, 200),
    //            ActionButton4.RotateTo(360, 200)

    //        );
    //    }
    //}


    //private void UpdateSecondaryButtonsPosition()
    //{
    //    // Mettre à jour la position des boutons secondaires en fonction du bouton principal
    //    double mainX = MainButton.TranslationX;
    //    double mainY = MainButton.TranslationY;

    //    // Espacement vertical entre les boutons
    //    double verticalSpacing = -50; // Valeur négative pour aller vers le haut

    //    ActionButton1.TranslationX = mainX;
    //    ActionButton1.TranslationY = mainY + verticalSpacing * 1; // Premier bouton (le plus haut)
    //    ActionButton2.TranslationX = mainX;
    //    ActionButton2.TranslationY = mainY + verticalSpacing * 2; // Deuxième bouton
    //    ActionButton3.TranslationX = mainX;
    //    ActionButton3.TranslationY = mainY + verticalSpacing * 3; // Troisième bouton (le plus bas)
    //    ActionButton4.TranslationX = mainX;
    //    ActionButton4.TranslationY = mainY + verticalSpacing * 4;
    //}
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
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
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
            await Navigation.PushAsync(new FloatingActionButton.DrawingView());
        }
        else
        {
            /* string buttonName = button.Source.ToString().Replace("File: ", "");
             await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");*/
        }
        UserDialogs.Instance.HideLoading();

    }
    private async void OnActionButtonClickedactivity(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        /*  UserDialogs.Instance.ShowLoading("Chargement...");
          await Task.Delay(400);*/
        if (button.Source.ToString().Contains("activity.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.ActivityView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }
        // UserDialogs.Instance.HideLoading();

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
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }
        UserDialogs.Instance.HideLoading();

    }
    private async void OnActionButtonClickeddocument(object sender, EventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading...");

        try
        {
            var opportunity = this.Opportunity; // Assurez-vous que cette propriété est accessible

            // Appeler la même navigation que l'ancien bouton
            await Navigation.PushAsync(new FileSelectionView(opportunity.Id, EntityType.Opportunity));
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
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }
    }


    /*************************************************************/
    /* private void ImageButtonClicked(object sender, EventArgs e)
     {
         navigationDrawer.ToggleDrawer();
     }*/

    /*  protected override async void OnAppearing()
  {
      base.OnAppearing();

      // Récupérer la liste des partenaires
      List<Partner> partners = await Partner.GetPartnerList();

      if (partners != null && partners.Count > 0)
      {
          // Supposons que vous voulez afficher le currency du premier partenaire dans la liste
          uint currency = partners[0].Currency;

          // Afficher le currency dans un DisplayAlert
          await DisplayAlert("Currency", $"Le currency du partenaire est : {currency}", "OK");
      }
      else
      {
          await DisplayAlert("Erreur", "Aucun partenaire trouvé ou erreur de chargement.", "OK");
      }
  }*/
    /* protected override async void OnAppearing()
     {
         base.OnAppearing();

         // Récupérer la liste des partenaires
         var wholesalerList = await Partner.GetWholesalerList();

         // Afficher les devises disponibles
         if (Partner.AllCurrencies.Count > 0)
         {
             string currencies = string.Join(", ", Partner.AllCurrencies.Select(c => c.Item2));
             await DisplayAlert("Devises Disponibles", $"Les devises récupérées sont :\n{currencies}", "OK");
         }
         else
         {
             await DisplayAlert("Aucune Devise", "Aucune devise n'a été récupérée depuis la base de données.", "OK");
         }

         // Vérifier si des partenaires ont été récupérés
        /* if (wholesalerList.Count > 0)
         {
             var firstPartner = wholesalerList.First(); // Prend le premier élément au lieu de wholesalerList[2]
             uint defaultCurrencyId = firstPartner.Currency;

             await DisplayAlert("Devise du premier partenaire",
                 $"Nom : {firstPartner.Name}\nDevise : (ID: {defaultCurrencyId})", "OK");
         }
         else
         {
             await DisplayAlert("Aucun Partenaire", "Aucun partenaire n'a été récupéré depuis la base de données.", "OK");
         }*/
    // }




    /*
     
    double xTranslation, yTranslation;

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                // Début du déplacement (enregistre la position initiale)
                xTranslation = MainButton.TranslationX;
                yTranslation = MainButton.TranslationY;
                break;

            case GestureStatus.Running:
                // Met à jour la position pendant le déplacement
                MainButton.TranslationX = xTranslation + e.TotalX;
                MainButton.TranslationY = yTranslation + e.TotalY;
                break;

            case GestureStatus.Completed:
                // Déplacement terminé (tu peux ajouter une logique ici si nécessaire)
                break;
        }
    }*/




    private async void OnSaveDocumentClicked(object sender, EventArgs e)
    {
        var opportunity = this.Opportunity;
        //await Navigation.PushAsync(new FileSelectionView(opportunity));
        await Navigation.PushAsync(new FileSelectionView(opportunity.Id, EntityType.Opportunity));

    }
    /*
         private async void OnSaveDocumentClicked(object sender, EventArgs e)
    {
        var opportunity = this.Opportunity;  // ou Partner si c'est le cas
        if (opportunity == null || opportunity.Id == 0)
        {
            await DisplayAlert("Erreur", "Aucune opportunité valide sélectionnée.", "OK");
            return;
        }

        // Passer l'ID d'Opportunity ou Partner à la nouvelle page
        await Navigation.PushAsync(new FileSelectionView(opportunity.Id));
    }*/


    /* private async void OnSelectFileClicked(object sender, EventArgs e)
     {
         try
         {
             string action = await DisplayActionSheet(
                 "Choose an option",
                 "Cancel",
                 null,
                 "Add File",
                 "Camera");

             if (action == "Cancel" || action == null)
             {
                 return;
             }

             string filePath = null;
             string fileName = null;

             DateTime selectedDate = DateTime.Now; // Date actuelle au moment de la sélection

             if (action == "Add File")
             {
                 var result = await FilePicker.PickAsync(new PickOptions
                 {
                     PickerTitle = "Please select a file",
                     FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                 {
                     { DevicePlatform.Android, new[] { "image/*", "application/pdf" } }
                 })
                 });

                 if (result != null)
                 {
                     filePath = result.FullPath;
                     fileName = result.FileName;
                 }
             }
             else if (action == "Camera")
             {
                 var photo = await MediaPicker.CapturePhotoAsync();

                 if (photo != null)
                 {
                     filePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                     using (var stream = await photo.OpenReadAsync())
                     using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                     {
                         await stream.CopyToAsync(fileStream);
                     }

                     fileName = photo.FileName;
                 }
             }

             if (filePath == null || fileName == null)
             {
                 return; // Aucun fichier sélectionné ou capturé
             }

             // Ajouter un délai avant d'afficher l'indicateur de chargement
             await Task.Delay(200); // Délai de 200 ms

             // Afficher l'indicateur de chargement
             UserDialogs.Instance.ShowLoading("Loading...");

             // Appeler le pop-up pour récupérer les informations supplémentaires
             try
             {
                 var documentTypes = await Document.GetDocumentTypesAsync();
                 if (documentTypes == null || documentTypes.Count == 0)
                 {
                     await DisplayAlert("Error", "Unable to retrieve document types.", "OK");
                     return;
                 }

                 var popup = new CustomPopup(documentTypes);
                 var result = await this.ShowPopupAsync(popup);

                 if (result == null)
                 {
                     await DisplayAlert("Cancelation", "The selection process has been cancelled.", "OK");
                     return;
                 }

                 var data = (dynamic)result;
                 var memo = data.Memo;
                 var description = data.Description;
                 var selectedTypeId = data.TypeId;

                 // Créer un document temporaire
                 var temporaryDocument = new Document
                 {
                     name = Path.GetFileNameWithoutExtension(fileName),
                     extension = Path.GetExtension(fileName),
                     content = await File.ReadAllBytesAsync(filePath),
                     create_date = DateTime.Now,
                     date = selectedDate,
                     memo = memo,
                     description = description,
                     type_document = (uint)selectedTypeId
                 };

                 // Ajouter le document temporaire à la liste
                 if (BindingContext is OpportunityViewModel viewModel)
                 {
                     viewModel.TemporaryDocuments.Add(temporaryDocument);
                 }

                 // Afficher le message de confirmation
                 ConfirmationLabel.Text = $"File added: {fileName}";
                 ConfirmationFrame.IsVisible = true;
             }
             catch (InvalidOperationException ex)
             {
                 await DisplayAlert("Error", ex.Message, "OK");
             }
             catch (Exception ex)
             {
                 await DisplayAlert("Error", $"An error has occurred: {ex.Message}", "OK");
             }
             finally
             {
                 // Masquer l'indicateur de chargement
                 UserDialogs.Instance.HideLoading();
             }
         }
         catch (Exception ex)
         {
             await DisplayAlert("Error", $"An error has occurred: {ex.Message}", "OK");
         }
     }*/

    private void OnCloseConfirmationClicked(object sender, EventArgs e)
    {
        // Masquer le Frame de confirmation
        ConfirmationFrame.IsVisible = false;

        // Annuler le document temporaire dans le ViewModel
        if (BindingContext is OpportunityViewModel viewModel)
        {
            viewModel.CancelTemporaryDocument();
        }
    }

    // Méthode pour sauvegarder un fichier dans la base de données
    /********************************************/
    /*  private async Task SaveFileToDatabase(string filePath, string fileName, string memo, string description, int typeId, DateTime selectedDate)
      {
          UserDialogs.Instance.ShowLoading("Loading...");
          await Task.Delay(400);
          try
          {
              string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
              string extension = Path.GetExtension(fileName);
              byte[] fileContent = await File.ReadAllBytesAsync(filePath);

              Document document = new Document
              {
                  name = fileNameWithoutExtension,
                  extension = extension,
                  content = fileContent,
                  create_date = DateTime.Now,
                  date = selectedDate,
                  memo = string.IsNullOrWhiteSpace(memo) ? null : memo,
                  description = string.IsNullOrWhiteSpace(description) ? null : description,
                  type_document = (uint)typeId
              };

              bool isSaved = await Document.SaveToDatabase(document);

              if (isSaved)
              {
                  // Afficher le message de confirmation dans le Frame
                  ConfirmationLabel.Text = $"File added: {fileName}";
                  ConfirmationFrame.IsVisible = true; 
              }
              else
              {

                  await DisplayAlert("Error", "An error occurred while saving the document.", "OK");
              }
          }
          catch (Exception ex)
          {

              await DisplayAlert("Error", $"An error has occurred: {ex.Message}", "OK");
          }
          finally
          {
              UserDialogs.Instance.HideLoading();
          }
      }*/



    /****************************************/
    // Handle touch events for the button
    private void OnButtonTouchDown(object sender, TouchEventArgs e)
    {
        // Start dragging when the touch is pressed
        isDragging = true;
    }
    private void OnButtonTouchMove(object sender, TouchEventArgs e)
    {
        if (isDragging)
        {
            // Update the position of the button based on touch coordinates
            var button = (Button)sender;
            //button.TranslationX = e.Location.X;
            //button.TranslationY = e.Location.Y;

        }
    }

    private void OnButtonTouchUp(object sender, TouchEventArgs e)
    {
        // Stop dragging when the touch is released
        isDragging = false;
    }
    public OpportunityView(Opportunity opportunity)
    {
        InitializeComponent();
        DbConnection.Deconnecter();
        Opportunity = opportunity;

        if (Opportunity.Id == 0)
            quotation.IsVisible = false;
        if (Opportunity.Id != 0)
            Opportunity = new Opportunity(Opportunity.Id);

        if (opportunity.Id != 0 && Opportunity.StateLines != null)
            foreach (SmartPharma5.Model.Opportunity.State S in Opportunity.StateLines)
            {
                Label header = new Label
                {
                    BackgroundColor = Colors.White,
                    Margin = 0,
                    Padding = new Thickness(4),
                    FontSize = 12,
                    TextColor = Colors.Black,
                    HorizontalOptions = LayoutOptions.Center

                };

                ImageButton imageButton = new ImageButton
                {
                    HorizontalOptions = LayoutOptions.End,
                    Margin = 0,
                    Padding = 0,
                    BackgroundColor = Colors.White,
                    CornerRadius = 0,


                    Source = "chevronrightsolid.png",
                    BorderColor = Colors.White,
                    Scale = 1,
                };


                header.Text = S.name.ToString() + "\n" + S.Date.ToString("dd/MM/yyyy  h:mm tt");
                TimeLine.Children.Add(header);
                TimeLine.Children.Add(imageButton);
                TimeLine.BackgroundColor = Colors.White;
                TimeLine.Margin = new Thickness(1, 4, 1, 0);
            }
        BindingContext = new OpportunityViewModel(Opportunity);
    }

    private void AutoCompleteEdit_TextChanged(object sender, DevExpress.Maui.Editors.AutoCompleteEditTextChangedEventArgs e)
    {
        AutoCompleteEdit edit = sender as AutoCompleteEdit;
        var search = edit.Text.ToLowerInvariant().ToString();
        var shop = BindingContext as OpportunityViewModel;


        if (string.IsNullOrWhiteSpace(search))
        {
            WholeCollectionView.ItemsSource = shop.WholesalerList.ToList();
        }
        else
        {
            WholeCollectionView.ItemsSource = shop.WholesalerList.Where(i => i.Name.ToLowerInvariant().Contains(search)).ToList();
        }
    }

    [Obsolete]
    protected override bool OnBackButtonPressed()
    {

        Device.BeginInvokeOnMainThread(async () =>
        {
            if (await App.Current.MainPage.DisplayAlert("Alert?", "Are you sure you want to exit this opportunity?\nYou will not be able to continue it.", "Yes", "No"))
            {
                base.OnBackButtonPressed();

                await App.Current.MainPage.Navigation.PopAsync();
            }
        });

        // Always return true because this method is not asynchronous.
        // We must handle the action ourselves: see above.
        return true;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Partner partner)
        {
            var shop = BindingContext as OpportunityViewModel;
            shop.WholesalerPopup = false;

            shop.Opportunity.Dealer = (int)partner.Id;
            shop.Opportunity.dealerName = (string)partner.Name;

            shop.WholeSalerRemoveIsvisible = true;
            shop.WholesalerTitleVisible = true;
        }
    }

    private async void SimpleButton_Clicked(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync("../OpportunityView");

    }

    private async void SimpleButton_Clicked_1(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync("..");
    }


}

/***************************/
/* private async void OnSelectFileClicked(object sender, EventArgs e)
 {
     try
     {
         // Afficher un menu d'options avant la sélection de fichier
         string action = await DisplayActionSheet(
             "Choose an option",
             "Cancel",
             null,
             "Add File",
             "Camera");

         if (action == "Cancel" || action == null)
         {
             return; // L'utilisateur a annulé l'action
         }

         if (action == "Add File")
         {
             // Ouvrir le sélecteur de fichiers
             var result = await FilePicker.PickAsync(new PickOptions
             {
                 PickerTitle = "Please select a file",
                 FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
             {
                 { DevicePlatform.Android, new[] { "image/*", "application/pdf" } } // Accepter images et PDF
             })
             });

             if (result != null)
             {
                 // Lire les informations du fichier sélectionné
                 string fileName = Path.GetFileNameWithoutExtension(result.FileName); // Nom sans extension
                 string extension = Path.GetExtension(result.FileName); // Extension
                 byte[] fileContent = await File.ReadAllBytesAsync(result.FullPath); // Contenu
                 DateTime selectedDate = DateTime.Now; // Date de sélection

                 // Demander un memo
                 string memo = await DisplayPromptAsync(
                     "Memo",
                     "Enter a note and click Next to continue (optional)",
                     placeholder: "Memo",
                     accept: "Next",
                     cancel: "Pass"
                 );

                 if (memo == null)
                 {
                     await DisplayAlert("Cancelation", "The selection process has been cancelled.", "OK");
                     return;
                 }

                 // Demander une description
                 string description = await DisplayPromptAsync(
                     "Description",
                     "Enter a description (optional)",
                     placeholder: "Description",
                     accept: "Next",
                     cancel: "Pass"
                 );

                 if (description == null)
                 {
                     await DisplayAlert("Cancelation", "The selection process has been cancelled.", "OK");
                     return;
                 }

                 // Récupérer les types de documents depuis la base de données
                 var documentTypes = await Document.GetDocumentTypesAsync();
                 if (documentTypes == null || documentTypes.Count == 0)
                 {
                     await DisplayAlert("Error", "Unable to retrieve document types.", "OK");
                     return;
                 }

                 // Afficher les types de documents
                 string[] typeNames = documentTypes.Values.ToArray();
                 string selectedTypeName = await DisplayActionSheet(
                     "Select a document type",
                     "Cancel",
                     null,
                     typeNames
                 );

                 if (selectedTypeName == null || selectedTypeName == "Cancel")
                 {
                     await DisplayAlert("Cancelation", "The selection process has been cancelled.", "OK");
                     return;
                 }

                 // Obtenir l'ID correspondant au type sélectionné
                 int selectedTypeId = documentTypes.FirstOrDefault(x => x.Value == selectedTypeName).Key;

                 // Créer un objet Document
                 Document document = new Document
                 {
                     name = fileName,
                     extension = extension,
                     content = fileContent,
                     create_date = DateTime.Now,
                     date = selectedDate,
                     memo = string.IsNullOrWhiteSpace(memo) ? null : memo,
                     description = string.IsNullOrWhiteSpace(description) ? null : description,
                     type_document = (uint)selectedTypeId
                 };

                 // Enregistrer dans la base de données
                 bool isSaved = await Document.SaveToDatabase(document);

                 if (isSaved)
                 {
                     await DisplayAlert("Success", "The document has been saved successfully.", "OK");
                 }
                 else
                 {
                     await DisplayAlert("Error", "An error occurred while saving the document.", "OK");
                 }
             }
             else
             {
                 await DisplayAlert("No file", "No files selected.", "OK");
             }
         }
         else if (action == "Camera")
         {
             // Logique pour la caméra (peut être ajoutée ici)
             await DisplayAlert("Camera", "Camera functionality is not yet implemented.", "OK");
         }
     }
     catch (Exception ex)
     {
         await DisplayAlert("Error", $"An error has occurred: {ex.Message}", "OK");
     }
 }*/

/*private async Task SaveFileToDatabase(string filePath, string fileName)
{
    try
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        string extension = Path.GetExtension(fileName);
        byte[] fileContent = await File.ReadAllBytesAsync(filePath);

        // Récupérer les types de document
        var documentTypes = await Document.GetDocumentTypesAsync();
        if (documentTypes == null || documentTypes.Count == 0)
        {
            await DisplayAlert("Error", "Unable to retrieve document types.", "OK");
            return;
        }

        string[] typeNames = documentTypes.Values.ToArray();

        // Afficher une boîte de dialogue unique pour recueillir toutes les informations
        string promptResult = await DisplayPromptAsync(
            "Document Information",
            "Enter the details separated by a comma (,):\n\n1. Memo (optional)\n2. Description (optional)\n3. Document Type",
            placeholder: "Memo,Description,Document Type",
            accept: "Save",
            cancel: "Cancel"
        );

        if (string.IsNullOrWhiteSpace(promptResult))
        {
            await DisplayAlert("Cancelation", "The selection process has been cancelled.", "OK");
            return;
        }

        // Séparer les valeurs saisies
        var parts = promptResult.Split(',');
        if (parts.Length < 3)
        {
            await DisplayAlert("Error", "Please provide all required information: Memo, Description, and Document Type.", "OK");
            return;
        }

        string memo = string.IsNullOrWhiteSpace(parts[0]) ? null : parts[0].Trim();
        string description = string.IsNullOrWhiteSpace(parts[1]) ? null : parts[1].Trim();
        string selectedTypeName = parts[2].Trim();

        // Vérifier le type de document sélectionné
        if (!documentTypes.Values.Contains(selectedTypeName))
        {
            await DisplayAlert("Error", "Invalid document type. Please try again.", "OK");
            return;
        }

        int selectedTypeId = documentTypes.FirstOrDefault(x => x.Value == selectedTypeName).Key;

        // Créer le document
        Document document = new Document
        {
            name = fileNameWithoutExtension,
            extension = extension,
            content = fileContent,
            create_date = DateTime.Now,
            date = DateTime.Now,
            memo = memo,
            description = description,
            type_document = (uint)selectedTypeId
        };

        // Enregistrer dans la base de données
        bool isSaved = await Document.SaveToDatabase(document);

        if (isSaved)
        {
            await DisplayAlert("Success", "The document has been saved successfully.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "An error occurred while saving the document.", "OK");
        }
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", $"An error has occurred: {ex.Message}", "OK");
    }
}
*/
/***************************/
//    private void UpdateSecondaryButtonsPosition()
//{
//    // Mettre à jour la position des boutons secondaires en fonction du bouton principal
//    double mainX = MainButton.TranslationX;
//    double mainY = MainButton.TranslationY;

//    // Espacement vertical entre les boutons
//    double verticalSpacing = -50; // Valeur négative pour aller vers le haut

//    ActionButton1.TranslationX = mainX;
//    ActionButton1.TranslationY = mainY + verticalSpacing * 1; // Premier bouton (le plus haut)

//    ActionButton2.TranslationX = mainX;
//    ActionButton2.TranslationY = mainY + verticalSpacing * 2; // Deuxième bouton

//    ActionButton3.TranslationX = mainX;
//    ActionButton3.TranslationY = mainY + verticalSpacing * 3; // Troisième bouton (le plus bas)
//}

//private void UpdateSecondaryButtonsPosition()
//{
//    // Mettre à jour la position des boutons secondaires en fonction du bouton principal
//    double mainX = MainButton.TranslationX;
//    double mainY = MainButton.TranslationY;

//    ActionButton1.TranslationX = mainX;
//    ActionButton1.TranslationY = mainY - 70;

//    ActionButton2.TranslationX = mainX - 50;
//    ActionButton2.TranslationY = mainY - 50;

//    ActionButton3.TranslationX = mainX - 70;
//    ActionButton3.TranslationY = mainY;
//}