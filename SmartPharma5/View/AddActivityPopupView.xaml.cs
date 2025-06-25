using SmartPharma5.ModelView;
using CommunityToolkit.Maui.Views;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using SmartPharma5.Model;

namespace SmartPharma5.View;

public partial class AddActivityPopupView : Popup
{
    private ActivityNotifViewModel _viewModel;

    public AddActivityPopupView(ActivityNotifViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Définir l'employé actuel comme employé sélectionné
        if (_viewModel.SelectedEmployee == null)
        {
            var currentUserId = Preferences.Get("iduser", 0);
            var employeeId = Task.Run(async () => await Activity.GetEmployeeIdByUserId(currentUserId)).Result;
            if (employeeId > 0)
            {
                _viewModel.SelectedEmployee = _viewModel.Employees?.FirstOrDefault(e => e.Id == employeeId);
            }
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        // Réinitialiser les champs
        _viewModel.Summary = string.Empty;
        _viewModel.ActivityMemo = string.Empty;
        _viewModel.DueDate = DateTime.Now;
        _viewModel.SelectedEmployee = null;
        _viewModel.SelectedActivityType = null;

        // Fermer le popup
        await CloseAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Fermer le popup immédiatement
        await CloseAsync();

        // Exécuter la commande d'ajout d'activité après la fermeture
        if (_viewModel.AddActivityCommand.CanExecute(null))
        {
            _viewModel.AddActivityCommand.Execute(null);
        }
    }
}