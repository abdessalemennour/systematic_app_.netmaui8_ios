using Syncfusion.Maui.NavigationDrawer;
using Microsoft.Maui.Devices;
using SmartPharma5.ModelView;
using SmartPharma5.Model;
using MySqlConnector;

namespace SmartPharma5.View
{
    public partial class SimpleNavigationView : ContentPage
    {
        private CustomNavigationDrawerViewModel viewModel;
        private Memo _selectedMemo;

        public void Initialize(int entityId, string entityType, string entityActivityType)
        {
            entityId = CurrentData.CurrentModuleId;
            entityType = CurrentData.CurrentNoteModule;
            entityActivityType = CurrentData.CurrentActivityModule;
            viewModel = new CustomNavigationDrawerViewModel(entityId, entityType, entityActivityType);
            BindingContext = viewModel;
        }

        public SimpleNavigationView()
        {
            InitializeComponent();
            viewModel = new CustomNavigationDrawerViewModel();
            BindingContext = viewModel;
            string currentnoteModule = CurrentData.CurrentNoteModule;
            string currentavtivityModule = CurrentData.CurrentActivityModule;
            int moduleId = CurrentData.CurrentModuleId;
        }

        private async void OnDeleteMemoClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is Memo memo)
            {
                bool confirmDelete = await App.Current.MainPage.DisplayAlert(
                    "Confirm deletion\r\n", 
                    "Are you sure you want to delete this memo ?", 
                    "Yes", 
                    "No" 
                );

                if (confirmDelete)
                {
                    bool isDeleted = await Memo.DeleteMemoFromDatabase(memo.Id);

                    if (isDeleted)
                    {
                        if (BindingContext is CustomNavigationDrawerViewModel viewModel)
                        {
                            viewModel.Memos.Remove(memo); 
                        }
                    }
                    else
                    {
                        Console.WriteLine("Erreur lors de la suppression du mémo.");
                    }
                }
                else
                {
                    // Si l'utilisateur annule, ne rien faire
                    Console.WriteLine("Suppression annulée.");
                }
            }
            else
            {
                Console.WriteLine("Erreur : sender n'est pas un ImageButton ou BindingContext est invalide !");
            }
        }

        private async void OnDeleteActivityClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is Activity activity)
            {
                // Afficher la boîte de confirmation
                bool confirmDelete = await App.Current.MainPage.DisplayAlert(
                    "Confirm deletion",
                    "Are you sure you want to delete this activity?",
                    "Yes",
                    "No"
                );

                // Si l'utilisateur confirme la suppression
                if (confirmDelete)
                {
                    bool isDeleted = await Activity.DeleteActivityFromDatabase(activity.Id);

                    if (isDeleted)
                    {
                        if (BindingContext is CustomNavigationDrawerViewModel viewModel)
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
            if (sender is ImageButton button && button.CommandParameter is string view)
            {
                if (BindingContext is CustomNavigationDrawerViewModel viewModel)
                {
                    viewModel.SelectedView = view;
                }
            }
        }


        private void OnEditMemoClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is Memo memo)
            {
                // Définir le mémo sélectionné dans le ViewModel
                if (BindingContext is CustomNavigationDrawerViewModel viewModel)
                {
                    viewModel.SelectedMemo = memo;

                    // Afficher la zone d'édition
                    editMemoLayout.IsVisible = true;

                    // Remplir les champs avec les valeurs actuelles du mémo
                    editNameEntry.Text = memo.Name;
                    editDescriptionEntry.Text = memo.Description;
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
                if (BindingContext is CustomNavigationDrawerViewModel viewModel)
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

        private void OnCancelEditClicked(object sender, EventArgs e)
        {
            // Masquer la zone d'édition
            editMemoLayout.IsVisible = false;

            // Réinitialiser le mémo sélectionné
            if (BindingContext is CustomNavigationDrawerViewModel viewModel)
            {
                viewModel.SelectedMemo = null;
            }
        }
        private void OnCancelEditActivityClicked(object sender, EventArgs e)
        {
            // Masquer la zone d'édition
            editactivityLayout.IsVisible = false;

            // Réinitialiser le mémo sélectionné
            if (BindingContext is CustomNavigationDrawerViewModel viewModel)
            {
                viewModel.SelectedActivity = null;
            }
        }
        private void OnSaveEditClicked(object sender, EventArgs e)
        {
            if (BindingContext is CustomNavigationDrawerViewModel viewModel)
            {
                // Mettre à jour les valeurs du mémo sélectionné
                if (viewModel.SelectedMemo != null)
                {
                    viewModel.SelectedMemo.Name = editNameEntry.Text;
                    viewModel.SelectedMemo.Description = editDescriptionEntry.Text;
                }

                // Exécuter la commande de sauvegarde
                viewModel.SaveEditCommand.Execute(null);
            }
        }
        private void OnSaveEditActivityClicked(object sender, EventArgs e)
        {
            if (BindingContext is CustomNavigationDrawerViewModel viewModel)
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
}
