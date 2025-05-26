using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ModelView;
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


}