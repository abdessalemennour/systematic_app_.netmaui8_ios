using SmartPharma5.ModelView;
using SmartPharma5.Extensions;
using SmartPharma5.Model;

namespace SmartPharma5.View.FloatingActionButton
{
    public partial class ActivityAddPopupView : ContentView, IDisposable
    {
        private ActivityViewModel _viewModel;

        public ActivityAddPopupView(ActivityViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
            
            // S'abonner à l'événement de fermeture du popup
            _viewModel.ActivityAdded += OnActivityAdded;
        }

        private async void OnActivityAdded(object sender, EventArgs e)
        {
            // Fermer le popup
            if (Parent?.Parent is ContentPage popupPage)
            {
                await popupPage.Navigation.PopModalAsync();
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            // Réinitialiser les champs
            _viewModel.Summary = string.Empty;
            _viewModel.ActivityMemo = string.Empty;
            _viewModel.DueDate = DateTime.Now;
            _viewModel.SelectedActivityType = null;
            _viewModel.ParentObject = null;
            _viewModel.ParentObjectType = null;
            _viewModel.ParentObjectDisplay = string.Empty;
            CurrentData.CurrentFormModule = string.Empty;
            _viewModel.FormDisplay = string.Empty;
            _viewModel.PreviousActivityEmployeeName = string.Empty;

            // Fermer le popup
            if (Parent?.Parent is ContentPage popupPage)
            {
                await popupPage.Navigation.PopModalAsync();
            }
        }

        public void Dispose()
        {
            // Se désabonner de l'événement
            if (_viewModel != null)
            {
                _viewModel.ActivityAdded -= OnActivityAdded;
            }
        }
    }
} 