using Acr.UserDialogs;
using DevExpress.Maui.Editors;
using SmartPharma5.Extensions;
using SmartPharma5.Model;
using SmartPharma5.ModelView;
using SmartPharma5.View;
using System.Globalization;
namespace SmartPharma5.View.FloatingActionButton;

public partial class ActivityView : ContentPage
{
    private ActivityViewModel viewModel;
    private Memo _selectedMemo;
    public void Initialize(int entityId, string entityType, string entityActivityType)
    {
        entityId = CurrentData.CurrentModuleId;
        entityType = CurrentData.CurrentNoteModule;
        entityActivityType = CurrentData.CurrentActivityModule;
        viewModel = new ActivityViewModel(entityId, entityType, entityActivityType);
        BindingContext = viewModel;
    }
    public ActivityView()
    {
        InitializeComponent();
        viewModel = new ActivityViewModel();
        BindingContext = viewModel;
        string currentnoteModule = CurrentData.CurrentNoteModule;
        string currentavtivityModule = CurrentData.CurrentActivityModule;
        int moduleId = CurrentData.CurrentModuleId;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (viewModel != null)
        {
            viewModel.Dispose();
        }
    }

    private async void OnDeleteActivityClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Activity activity)
        {
            // Afficher la boite de confirmation
            bool confirmDelete = await App.Current.MainPage.DisplayAlert(

                " Confirmer la suppression ",
                " tes - vous sr de vouloir supprimer cette activit ",
                "Yes",
                "No"
            );

            // Si l'utilisateur confirme la suppression
            if (confirmDelete)
            {
                bool isDeleted = await Activity.DeleteActivityFromDatabase(activity.Id);

                if (isDeleted)
                {
                    if (BindingContext is ActivityViewModel viewModel)
                    {
                        viewModel.Activities.Remove(activity); // Supprimer l'activit de la liste
                    }
                }
                else
                {
                    Console.WriteLine("Erreur lors de la suppression de l'activit.");
                }
            }
            else
            {
                Console.WriteLine("Suppression annule.");
            }
        }
        else
        {
            Console.WriteLine("Erreur : sender n'est pas un ImageButton ou BindingContext est invalide !");
        }
    }


    private void OnTabClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.CommandParameter is string viewName)
        {
            NavigateToView(viewName); // Appeler la mthode de navigation
        }
    }
    private async void NavigateToView(string viewName)
    {
        switch (viewName)
        {
            case "Memo":
                await Navigation.PushAsync(new MemoView());
                break;
            case "Activity":
                await Navigation.PushAsync(new ActivityView());
                break;
            case "Chat":
                await Navigation.PushAsync(new ChatView());
                break;
            default:
                await DisplayAlert("Erreur", "Vue non trouve", "OK");
                break;
        }
    }

    private void OnEditActivityClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Activity activity)
        {
            // Définir l'activité sélectionnée dans le ViewModel
            if (BindingContext is ActivityViewModel viewModel)
            {
                viewModel.SelectedActivity = activity;

                // Afficher la zone d'édition
                editactivityLayout.IsVisible = true;

                // Remplir les champs avec les valeurs actuelles de l'activité
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

        // Réinitialiser l'activité sélectionnée
        if (BindingContext is ActivityViewModel viewModel)
        {
            viewModel.SelectedActivity = null;
        }
    }
    private void OnSaveEditActivityClicked(object sender, EventArgs e)
    {
        if (BindingContext is ActivityViewModel viewModel)
        {
            // Mettre à jour les valeurs de l'activité sélectionnée
            if (viewModel.SelectedActivity != null)
            {
                viewModel.SelectedActivity.Summary = editSummaryEntry.Text;
                viewModel.SelectedActivity.Memo = editMemoEntry.Text;
            }

            // Exécuter la commande de sauvegarde
            viewModel.SaveActivityEditCommand.Execute(null);
        }
    }
    private async void OnActionButtonClickedchat(object sender, EventArgs e)
    {
        var button = sender as ImageButton;


        if (button.Source.ToString().Contains("chat.png"))
        {
            await Navigation.PushAsync(new ChatView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqu sur {buttonName}", "OK");
        }

    }

    private async void OnActionButtonClickedmemo(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        if (button.Source.ToString().Contains("memo.png"))
        {
            await Navigation.PushAsync(new MemoView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqu sur {buttonName}", "OK");
        }

    }
    private async void OnActionButtonClickedactivity(object sender, EventArgs e)
    {
        // Logique pour ajouter une activité
        if (BindingContext is ActivityViewModel viewModel)
        {
            try
            {
                Console.WriteLine("OnActionButtonClickedactivity: Début de la méthode");
                
                // Réinitialiser les champs
                viewModel.Summary = string.Empty;
                viewModel.ActivityMemo = string.Empty;
                viewModel.DueDate = DateTime.Now;
                viewModel.SelectedEmployee = null;
                viewModel.SelectedActivityType = null;
                viewModel.ParentObject = null;
                viewModel.ParentObjectType = null;
                viewModel.ParentObjectDisplay = string.Empty;
                CurrentData.CurrentFormModule = string.Empty;
                viewModel.FormDisplay = string.Empty;
                viewModel.PreviousActivityEmployeeName = string.Empty;

                // Récupérer le nom de l'employé de la dernière activité si disponible
                if (viewModel.Activities != null && viewModel.Activities.Count > 0)
                {
                    var lastActivity = viewModel.Activities.OrderByDescending(a => a.CreateDate).FirstOrDefault();
                    if (lastActivity != null)
                    {
                        viewModel.PreviousActivityEmployeeName = lastActivity.AssignedEmployeeName ?? "Non assigné";
                        Console.WriteLine($"OnActionButtonClickedactivity: Nom employé dernière activité - {viewModel.PreviousActivityEmployeeName}");
                    }
                }

                // Afficher le nouveau popup d'ajout d'activité
                var popup = new ActivityAddPopupView(viewModel);
                await this.ShowPopupAsync(popup);
                
                Console.WriteLine("OnActionButtonClickedactivity: Popup affiché");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnActionButtonClickedactivity: Exception - {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Erreur", ex.Message, "OK");
            }
        }
    }

    // Mthodes pour le formulaire "Done"
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
                    // Utiliser le filtrage combiné si des switches sont actifs
                    await viewModel.ApplyCombinedFilter();
                }
                else if (viewModel.SelectedStateActivity != null)
                {
                    // Utiliser le filtre par état si un état est sélectionné
                    await viewModel.LoadFilteredActivities();
                }
                else
                {
                    // Sinon, recharger toutes les activités
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

    private async void OnAddNewActivityClicked(object sender, EventArgs e)
    {
        try
        {
            Console.WriteLine("OnAddNewActivityClicked: Début de la méthode");
            
            // Sauvegarder l'activité actuelle
            if (viewModel.SelectedActivity != null)
            {
                Console.WriteLine($"OnAddNewActivityClicked: Activité sélectionnée ID={viewModel.SelectedActivity.Id}");
                
                // Mettre à jour les valeurs
                viewModel.SelectedActivity.Summary = doneSummaryEntry.Text;
                viewModel.SelectedActivity.Memo = doneMemoEntry.Text;
                viewModel.SelectedActivity.State = 2; // État "Done"
                viewModel.SelectedActivity.DoneDate = DateTime.Now;

                // Sauvegarder les modifications dans la base de données
                bool success = await viewModel.SelectedActivity.UpdateStateInDatabaseAsync();
                
                if (success)
                {
                    Console.WriteLine("OnAddNewActivityClicked: Activité sauvegardée avec succès");
                    
                    // Pré-remplir les informations de l'activité terminée
                    await viewModel.PrefillNewActivityFromCompletedActivity(viewModel.SelectedActivity);

                    // Fermer le formulaire Done
                    viewModel.ShowDoneMessage = false;

                    // Afficher le nouveau popup d'ajout d'activité
                    var popup = new ActivityAddPopupView(viewModel);
                    await this.ShowPopupAsync(popup);
                    
                    Console.WriteLine("OnAddNewActivityClicked: Popup affiché");
                }
                else
                {
                    Console.WriteLine("OnAddNewActivityClicked: Erreur lors de la sauvegarde");
                    await Application.Current.MainPage.DisplayAlert("Erreur", "Impossible de sauvegarder l'activité", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"OnAddNewActivityClicked: Exception - {ex.Message}");
            await Application.Current.MainPage.DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private async void OnAddActivityClicked(object sender, EventArgs e)
    {
        try
        {
            Console.WriteLine("OnAddActivityClicked: Début de la méthode");
            
            // Réinitialiser les champs
            viewModel.Summary = string.Empty;
            viewModel.ActivityMemo = string.Empty;
            viewModel.DueDate = DateTime.Now;
            viewModel.SelectedActivityType = null;
            viewModel.ParentObject = null;
            viewModel.ParentObjectType = null;
            viewModel.ParentObjectDisplay = string.Empty;
            CurrentData.CurrentFormModule = string.Empty;
            viewModel.FormDisplay = string.Empty;
            viewModel.PreviousActivityEmployeeName = string.Empty;

            // Récupérer le nom de l'employé de la dernière activité si disponible
            if (viewModel.Activities != null && viewModel.Activities.Count > 0)
            {
                var lastActivity = viewModel.Activities.OrderByDescending(a => a.CreateDate).FirstOrDefault();
                if (lastActivity != null)
                {
                    viewModel.PreviousActivityEmployeeName = lastActivity.AssignedEmployeeName ?? "Non assigné";
                    Console.WriteLine($"OnActionButtonClickedactivity: Nom employé dernière activité - {viewModel.PreviousActivityEmployeeName}");
                }
            }

            // Afficher le nouveau popup d'ajout d'activité
            var popup = new ActivityAddPopupView(viewModel);
            await this.ShowPopupAsync(popup);
            
            Console.WriteLine("OnAddActivityClicked: Popup affiché");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"OnAddActivityClicked: Exception - {ex.Message}");
            await Application.Current.MainPage.DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private void OnCancelAddActivityClicked(object sender, EventArgs e)
    {
        // Réinitialiser les champs
        viewModel.Summary = string.Empty;
        viewModel.ActivityMemo = string.Empty;
        viewModel.SelectedActivityType = null;
        viewModel.PreviousActivityEmployeeName = string.Empty;
        
        // Masquer le popup
        //addActivityPopup.IsVisible = false;
    }
}