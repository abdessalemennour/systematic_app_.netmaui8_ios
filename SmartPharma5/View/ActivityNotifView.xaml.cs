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
            // Afficher la bo�te de confirmation
            bool confirmDelete = await App.Current.MainPage.DisplayAlert(

                " Confirmer la suppression ",
                " �tes - vous s�r de vouloir supprimer cette activit� ? ",
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
                        viewModel.Activities.Remove(activity); // Supprimer l'activit� de la liste
                    }
                }
                else
                {
                    Console.WriteLine("Erreur lors de la suppression de l'activit�.");
                }
            }
            else
            {
                Console.WriteLine("Suppression annul�e.");
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
            // D�finir le m�mo s�lectionn� dans le ViewModel
            if (BindingContext is ActivityNotifViewModel viewModel)
            {
                viewModel.SelectedActivity = activity;

                // Afficher la zone d'�dition
                editactivityLayout.IsVisible = true;

                // Remplir les champs avec les valeurs actuelles du m�mo
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
        // Masquer la zone d'�dition
        editactivityLayout.IsVisible = false;

        // R�initialiser le m�mo s�lectionn�
        if (BindingContext is ActivityNotifViewModel viewModel)
        {
            viewModel.SelectedActivity = null;
        }
    }
    private void OnSaveEditActivityClicked(object sender, EventArgs e)
    {

        if (BindingContext is ActivityNotifViewModel viewModel)
        {
            // Mettre � jour les valeurs du m�mo s�lectionn�
            if (viewModel.SelectedActivity != null)
            {
                viewModel.SelectedActivity.Summary = editSummaryEntry.Text;
                viewModel.SelectedActivity.Memo = editMemoEntry.Text;
            }

            // Ex�cuter la commande de sauvegarde
            viewModel.SaveActivityEditCommand.Execute(null);
        }

    }


}