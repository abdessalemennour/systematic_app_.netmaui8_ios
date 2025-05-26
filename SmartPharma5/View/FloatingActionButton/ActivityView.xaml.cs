using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ModelView;

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
                    if (BindingContext is ActivityViewModel viewModel)
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


    private void OnTabClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.CommandParameter is string viewName)
        {
            NavigateToView(viewName); // Appeler la méthode de navigation
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
                await DisplayAlert("Erreur", "Vue non trouvée", "OK");
                break;
        }
    }

    private void OnEditActivityClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Activity activity)
        {
            // Définir le mémo sélectionné dans le ViewModel
            if (BindingContext is ActivityViewModel viewModel)
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
        if (BindingContext is ActivityViewModel viewModel)
        {
            viewModel.SelectedActivity = null;
        }
    }
    private void OnSaveEditActivityClicked(object sender, EventArgs e)
    {

        if (BindingContext is ActivityViewModel viewModel)
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
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
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
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }

    }
    private async void OnActionButtonClickedactivity(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        if (button.Source.ToString().Contains("activity.png"))
        {
            await Navigation.PushAsync(new ActivityView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }

    }
}