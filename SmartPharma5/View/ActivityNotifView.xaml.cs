using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ModelView;
using DevExpress.Maui.Editors;
using static SmartPharma5.Model.Activity;
using CommunityToolkit.Maui.Views;
using System.Globalization;

namespace SmartPharma5.View;

public partial class ActivityNotifView : ContentPage
{
    private ActivityNotifViewModel viewModel;
    private Memo _selectedMemo;
    public void Initialize()
    {
        viewModel = new ActivityNotifViewModel();
        BindingContext = viewModel;
    }
    public ActivityNotifView()
    {
        InitializeComponent();
        viewModel = new ActivityNotifViewModel();
        BindingContext = viewModel;
    }
    private async void OnDeleteActivityClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Activity activity)
        {
            // Afficher la boîte de confirmation
            bool confirmDelete = await App.Current.MainPage.DisplayAlert(

                " Confirmer la suppression ",
                " Êtes - vous sûr de vouloir supprimer cette activité ? ",
                "Yes",
                "No"
            );

            // Si l'utilisateur confirme la suppression
            if (confirmDelete)
            {
                bool isDeleted = await Activity.DeleteActivityFromDatabase(activity.Id);

                if (isDeleted)
                {
                    if (BindingContext is ActivityNotifViewModel viewModel)
                    {
                        viewModel.Activities.Remove(activity); // Supprimer l'activité de la liste
                    }
                }
                else
                {
                    Console.WriteLine("Erreur lors de la suppression de l'activité.");
                }
            }
            else
            {
                Console.WriteLine("Suppression annulée.");
            }
        }
        else
        {
            Console.WriteLine("Erreur : sender n'est pas un ImageButton ou BindingContext est invalide !");
        }
    }  
    private void OnEditActivityClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Activity activity)
        {
            // Définir le mémo sélectionné dans le ViewModel
            if (BindingContext is ActivityNotifViewModel viewModel)
            {
                viewModel.SelectedActivity = activity;

                // Afficher la zone d'édition
                editactivityLayout.IsVisible = true;

                // Remplir les champs avec les valeurs actuelles du mémo
                editSummaryEntry.Text = activity.Summary;
                editMemoEntry.Text = activity.Memo;
            }
        }
        else
        {
            Console.WriteLine("Erreur : sender n'est pas un ImageButton ou BindingContext est invalide !");
        }
    }
    private void OnCancelEditActivityClicked(object sender, EventArgs e)
    {
        // Masquer la zone d'édition
        editactivityLayout.IsVisible = false;

        // Réinitialiser le mémo sélectionné
        if (BindingContext is ActivityNotifViewModel viewModel)
        {
            viewModel.SelectedActivity = null;
        }
    }
    private void OnSaveEditActivityClicked(object sender, EventArgs e)
    {

        if (BindingContext is ActivityNotifViewModel viewModel)
        {
            // Mettre à jour les valeurs du mémo sélectionné
            if (viewModel.SelectedActivity != null)
            {
                viewModel.SelectedActivity.Summary = editSummaryEntry.Text;
                viewModel.SelectedActivity.Memo = editMemoEntry.Text;
            }

            // Exécuter la commande de sauvegarde
            viewModel.SaveActivityEditCommand.Execute(null);
        }

    }
    private async void OnCloseDoneActivityClicked(object sender, EventArgs e)
    {
        viewModel.ShowDoneMessage = false;
        viewModel.ResetIndividualStateChangeFlag();
    }

    private async void OnSaveDoneActivityClicked(object sender, EventArgs e)
    {
        if (viewModel.SelectedActivity != null)
        {
            // Mettre à jour les valeurs
            viewModel.SelectedActivity.Summary = doneSummaryEntry.Text;
            viewModel.SelectedActivity.Memo = doneMemoEntry.Text;
            viewModel.SelectedActivity.State = 2; // État "Done"
            viewModel.SelectedActivity.DoneDate = DateTime.Now;

            // Sauvegarder les modifications dans la base de données
            bool success = await viewModel.SelectedActivity.UpdateStateInDatabaseAsync();
            
            if (success)
            {
                // Recharger la liste en fonction des filtres actifs
                if (viewModel.IsLateChecked || viewModel.IsTodayChecked || viewModel.IsFutureChecked)
                {
                    await viewModel.FilterAllActivities();
                }
                else if (viewModel.SelectedStateActivity != null)
                {

                    await viewModel.LoadFilteredAllActivities();
                }
                else
                {
                    await viewModel.LoadActivities();
                }

                viewModel.ShowDoneMessage = false;
                viewModel.ResetIndividualStateChangeFlag();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", 
                    "Impossible de sauvegarder les modifications", "OK");
            }
        }
    }

    private async void OnAddNewActivityClicked(object sender, EventArgs e)
    {
        try
        {
            // Sauvegarder l'activité actuelle
            if (viewModel.SelectedActivity != null)
            {
                viewModel.SelectedActivity.Summary = doneSummaryEntry.Text;
                viewModel.SelectedActivity.Memo = doneMemoEntry.Text;
                bool success = await Activity.UpdateAllActivity(viewModel.SelectedActivity);

                if (success)
                {
                    // Pré-remplir les informations de l'activité terminée
                    await viewModel.PrefillNewActivityFromCompletedActivity(viewModel.SelectedActivity);

                    // Fermer le formulaire Done
                    viewModel.ShowDoneMessage = false;

                    // Afficher le popup d'ajout d'activité
                    var popup = new AddActivityPopupView(viewModel);
                    await this.ShowPopupAsync(popup);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur", "Impossible de sauvegarder l'activité", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private void OnCancelAddActivityClicked(object sender, EventArgs e)
    {
        // Réinitialiser les champs
        viewModel.Summary = string.Empty;
        viewModel.ActivityMemo = string.Empty;
        viewModel.DueDate = DateTime.Now;
        viewModel.SelectedEmployee = null;
        viewModel.SelectedActivityType = null;
        viewModel.ParentObjectDisplay = string.Empty;
        viewModel.ParentObject = null;
        viewModel.ParentObjectType = null;
        viewModel.EntityFormType = string.Empty;
        viewModel.FormDisplay = string.Empty;

        // Fermer le popup
        addActivityPopup.IsVisible = false;
    }

    private async void OnGpsClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Activity activity && !string.IsNullOrWhiteSpace(activity.Gps))
        {
            try
            {
                var coords = activity.Gps.Split(',');
                if (coords.Length == 2 &&
                    double.TryParse(coords[0], CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(coords[1], CultureInfo.InvariantCulture, out double lng))
                {
                    string uri = $"https://www.google.com/maps/search/?api=1&query={lat.ToString("F6", CultureInfo.InvariantCulture)},{lng.ToString("F6", CultureInfo.InvariantCulture)}";
                    await Launcher.Default.OpenAsync(new Uri(uri));
                }
                else
                {
                    await DisplayAlert("Erreur", "Coordonnées GPS invalides.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", "Impossible d'ouvrir Google Maps : " + ex.Message, "OK");
            }
        }
        else
        {
            await DisplayAlert("GPS", "Aucune coordonnée GPS disponible pour cette activité", "OK");
        }
    }
}