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
                    if (BindingContext is ActivityViewModel viewModel)
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


    private void OnTabClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.CommandParameter is string viewName)
        {
            NavigateToView(viewName); // Appeler la m�thode de navigation
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
                await DisplayAlert("Erreur", "Vue non trouv�e", "OK");
                break;
        }
    }

    private void OnEditActivityClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Activity activity)
        {
            // D�finir le m�mo s�lectionn� dans le ViewModel
            if (BindingContext is ActivityViewModel viewModel)
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
        if (BindingContext is ActivityViewModel viewModel)
        {
            viewModel.SelectedActivity = null;
        }
    }
    private void OnSaveEditActivityClicked(object sender, EventArgs e)
    {

        if (BindingContext is ActivityViewModel viewModel)
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
            await DisplayAlert("Action", $"Vous avez cliqu� sur {buttonName}", "OK");
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
            await DisplayAlert("Action", $"Vous avez cliqu� sur {buttonName}", "OK");
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
            await DisplayAlert("Action", $"Vous avez cliqu� sur {buttonName}", "OK");
        }

    }
}