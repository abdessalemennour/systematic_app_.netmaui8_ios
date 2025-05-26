using Syncfusion.Maui.NavigationDrawer;
using Microsoft.Maui.Devices;
using SmartPharma5.ModelView;
using SmartPharma5.Model;

namespace SmartPharma5.View
{
    public partial class CustomNavigationDrawer : SfNavigationDrawer
    {
        private CustomNavigationDrawerViewModel viewModel;

        public void Initialize(int entityId, string entityType,string entityActivityType)
        {
            entityId = CurrentData.CurrentModuleId;
            entityType = CurrentData.CurrentNoteModule;
            entityActivityType = CurrentData.CurrentActivityModule;
            viewModel = new CustomNavigationDrawerViewModel(entityId, entityType, entityActivityType);
            BindingContext = viewModel;
        }
        public CustomNavigationDrawer()
        {
            InitializeComponent();
            viewModel = new CustomNavigationDrawerViewModel();
            BindingContext = viewModel;
            AdjustDrawerWidth();
            string currentnoteModule = CurrentData.CurrentNoteModule;
            string currentavtivityModule = CurrentData.CurrentActivityModule;
            int moduleId = CurrentData.CurrentModuleId;

        }

        private void AdjustDrawerWidth()
            {
            double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
            double drawerWidth = screenWidth * 0.75;
            this.DrawerSettings.DrawerWidth = drawerWidth;
        }
    }
}
