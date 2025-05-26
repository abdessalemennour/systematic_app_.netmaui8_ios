using Acr.UserDialogs;
using DevExpress.Maui.Editors;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;
using DevExpress.Maui.Controls;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using CommunityToolkit.Maui.Views;
using Syncfusion.Maui.NavigationDrawer;

namespace SmartPharma5.View.FloatingActionButton;
[XamlCompilation(XamlCompilationOptions.Compile)]

public partial class FloatingActionButton : ContentView
{
    private double _startX, _startY;
    public FloatingActionButton()
    {
        InitializeComponent();
    }
    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _startX = MainButton.TranslationX;
                _startY = MainButton.TranslationY;
                MainButton.ZIndex = 10;

                break;

            case GestureStatus.Running:
                MainButton.TranslationX = _startX + e.TotalX;
                MainButton.TranslationY = _startY + e.TotalY;
                break;

            case GestureStatus.Completed:
                // MAJ position pour empêcher le débordement hors écran
                var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
                var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
                MainButton.TranslationX = Math.Max(0, Math.Min(screenWidth - MainButton.Width, MainButton.TranslationX));
                MainButton.TranslationY = Math.Max(0, Math.Min(screenHeight - MainButton.Height, MainButton.TranslationY));
                // Mettre à jour la position des boutons secondaires après déplacement
                UpdateSecondaryButtonsPosition();
                break;
            case GestureStatus.Canceled:
                // 🔹 Optionnel : Réinitialiser ZIndex après le déplacement
                MainButton.ZIndex = 0;
                break;
        }
    }

    private async void OnMainButtonClicked(object sender, EventArgs e)
    {
        if (ActionButton1.IsVisible)
        {
            await Task.WhenAll(
                ActionButton1.TranslateTo(MainButton.TranslationX, MainButton.TranslationY, 200, Easing.SinIn),
                ActionButton2.TranslateTo(MainButton.TranslationX, MainButton.TranslationY, 200, Easing.SinIn),
                ActionButton3.TranslateTo(MainButton.TranslationX, MainButton.TranslationY, 200, Easing.SinIn),
                ActionButton1.FadeTo(0, 200),
                ActionButton2.FadeTo(0, 200),
                ActionButton3.FadeTo(0, 200),
                ActionButton1.RotateTo(-360, 200),
                ActionButton2.RotateTo(-360, 200),
                ActionButton3.RotateTo(-360, 200)
            );

            await Task.Delay(200); // Attendre la fin de l'animation avant de cacher les boutons

            ActionButton1.IsVisible = false;
            ActionButton2.IsVisible = false;
            ActionButton3.IsVisible = false;
        }
        else
        {
            UpdateSecondaryButtonsPosition(); // Mettre à jour la position

            // 🔹 Rendre les boutons invisibles avant de les afficher
            ActionButton1.Opacity = 0;
            ActionButton2.Opacity = 0;
            ActionButton3.Opacity = 0;

            // ✅ Afficher les boutons mais sans qu'ils apparaissent visiblement
            ActionButton1.IsVisible = true;
            ActionButton2.IsVisible = true;
            ActionButton3.IsVisible = true;
            ActionButton1.ZIndex = 10;
            ActionButton2.ZIndex = 10;
            ActionButton3.ZIndex = 10;

            // ✅ Faire l'animation d'apparition (opacité + mouvement)
            await Task.WhenAll(
                ActionButton1.FadeTo(1, 200),
                ActionButton2.FadeTo(1, 200),
                ActionButton3.FadeTo(1, 200),
                ActionButton1.TranslateTo(ActionButton1.TranslationX, ActionButton1.TranslationY, 200, Easing.SinOut),
                ActionButton2.TranslateTo(ActionButton2.TranslationX, ActionButton2.TranslationY, 200, Easing.SinOut),
                ActionButton3.TranslateTo(ActionButton3.TranslationX, ActionButton3.TranslationY, 200, Easing.SinOut),
                ActionButton1.RotateTo(360, 200),
                ActionButton2.RotateTo(360, 200),
                ActionButton3.RotateTo(360, 200)
            );
        }
    }


    private void UpdateSecondaryButtonsPosition()
    {
        // Mettre à jour la position des boutons secondaires en fonction du bouton principal
        double mainX = MainButton.TranslationX;
        double mainY = MainButton.TranslationY;

        // Espacement vertical entre les boutons
        double verticalSpacing = -50; // Valeur négative pour aller vers le haut

        ActionButton1.TranslationX = mainX;
        ActionButton1.TranslationY = mainY + verticalSpacing * 1; // Premier bouton (le plus haut)

        ActionButton2.TranslationX = mainX;
        ActionButton2.TranslationY = mainY + verticalSpacing * 2; // Deuxième bouton

        ActionButton3.TranslationX = mainX;
        ActionButton3.TranslationY = mainY + verticalSpacing * 3; // Troisième bouton (le plus bas)
    }

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

    private async void OnActionButtonClickedmemo(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        UserDialogs.Instance.ShowLoading("Loading...");
        await Task.Delay(200);
        if (button.Source.ToString().Contains("memo.png"))
        {
           // await Navigation.PushAsync(new MemoView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
           // await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }
        UserDialogs.Instance.HideLoading();

    }
    private async void OnActionButtonClickedactivity(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        UserDialogs.Instance.ShowLoading("Loading...");
        await Task.Delay(200);
        if (button.Source.ToString().Contains("activity.png"))
        {
           // await Navigation.PushAsync(new ActivityView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            //await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }
        UserDialogs.Instance.HideLoading();

    }
    private async void OnActionButtonClickedchat(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        UserDialogs.Instance.ShowLoading("Loading...");
        await Task.Delay(200);
        if (button.Source.ToString().Contains("chat.png"))
        {
           // await Navigation.PushAsync(new ChatView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            //await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }
        UserDialogs.Instance.HideLoading();

    }




}
//public partial class FloatingActionButton : ContentPage
//{
//	public FloatingActionButton()
//	{
//		InitializeComponent();
//	}
//    protected override void OnAppearing()
//    {
//        OnAppearing();
//        InitializeComponent();

//        // Réinitialiser l'état des boutons secondaires
//        ActionButton1.IsVisible = false;
//        ActionButton2.IsVisible = false;
//        ActionButton3.IsVisible = false;
//            ResetButtons();

//        // Réinitialiser les positions des boutons
//        UpdateSecondaryButtonsPosition();
//    }

//    private double _startX, _startY; // Position initiale du bouton

//    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
//    {
//        switch (e.StatusType)
//        {
//            case GestureStatus.Started:
//                _startX = MainButton.TranslationX;
//                _startY = MainButton.TranslationY;
//                MainButton.ZIndex = 10;

//                break;

//            case GestureStatus.Running:
//                MainButton.TranslationX = _startX + e.TotalX;
//                MainButton.TranslationY = _startY + e.TotalY;
//                UpdateSecondaryButtonsPosition();

//                break;

//            case GestureStatus.Completed:
//                // MAJ position pour empêcher le débordement hors écran
//                var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
//                var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
//                MainButton.TranslationX = Math.Max(0, Math.Min(screenWidth - MainButton.Width, MainButton.TranslationX));
//                MainButton.TranslationY = Math.Max(0, Math.Min(screenHeight - MainButton.Height, MainButton.TranslationY));

//                // Mettre à jour la position des boutons secondaires après le déplacement
//                UpdateSecondaryButtonsPosition();
//                break;

//            case GestureStatus.Canceled:
//                // 🔹 Optionnel : Réinitialiser ZIndex après le déplacement
//                MainButton.ZIndex = 0;
//                break;
//        }
//    }
//    private async void OnMainButtonClicked(object sender, EventArgs e)
//    {
//        if (ActionButton1.IsVisible)
//        {
//            // Masquer avec animation
//            await Task.WhenAll(
//                ActionButton1.FadeTo(0, 200),
//                ActionButton2.FadeTo(0, 200),
//                ActionButton3.FadeTo(0, 200)
//            );

//            // Masquer après animation
//            ActionButton1.IsVisible = false;
//            ActionButton2.IsVisible = false;
//            ActionButton3.IsVisible = false;
//        }
//        else
//        {
//            // Assurez-vous que les boutons sont visibles avant d'animer
//            ActionButton1.Opacity = 0;
//            ActionButton2.Opacity = 0;
//            ActionButton3.Opacity = 0;

//            ActionButton1.IsVisible = true;
//            ActionButton2.IsVisible = true;
//            ActionButton3.IsVisible = true;

//            // Mettre à jour la position avant d'afficher
//            UpdateSecondaryButtonsPosition();

//            // Afficher avec animation
//            await Task.WhenAll(
//                ActionButton1.FadeTo(1, 200),
//                ActionButton2.FadeTo(1, 200),
//                ActionButton3.FadeTo(1, 200)
//            );
//        }
//    }

//    private void ResetButtons()
//    {
//        ActionButton1.IsVisible = false;
//        ActionButton2.IsVisible = false;
//        ActionButton3.IsVisible = false;
//    }



//    private void UpdateSecondaryButtonsPosition()
//    {
//        // Mettre à jour la position des boutons secondaires en fonction du bouton principal
//        double mainX = MainButton.TranslationX;
//        double mainY = MainButton.TranslationY;

//        ActionButton1.TranslationX = mainX;
//        ActionButton1.TranslationY = mainY - 70;

//        ActionButton2.TranslationX = mainX - 50;
//        ActionButton2.TranslationY = mainY - 50;

//        ActionButton3.TranslationX = mainX - 70;
//        ActionButton3.TranslationY = mainY;
//    }

//    private void OnActionButtonClicked(object sender, EventArgs e)
//    {
//        // Identifier quel bouton a été cliqué
//        var button = sender as ImageButton;
//        // Afficher une alerte avec le nom de l'image cliquée
//        string buttonName = button.Source.ToString().Replace("File: ", "");
//        DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
//    }

//    internal class ActivityView
//    {
//        public ActivityView()
//        {
//        }
//    }
//}