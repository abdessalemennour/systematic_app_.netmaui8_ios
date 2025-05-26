
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.Model;
using MvvmHelpers;
Après :
using MvvmHelpers;
using MvvmHelpers.Commands;
using SmartPharma5.Model;
*/
using Acr.UserDialogs;
using MvvmHelpers;
using SmartPharma5.
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
Après :
using Command = MvvmHelpers.Commands.Command;
*/
Model;
using Command = MvvmHelpers.Commands.Command;

namespace SmartPharma5.ViewModel
{
    public class FormListViewModel : BaseViewModel
    {
        private ObservableRangeCollection<Form> formlist;
        public ObservableRangeCollection<Form> FormList { get => formlist; set => SetProperty(ref formlist, value); }

        private bool noForm;
        public bool NoForm { get => noForm; set => SetProperty(ref noForm, value); }

        public System.Windows.Input.ICommand ITapCommand { get; }
        public Partner Partner { get; set; }
        public FormListViewModel()
        {
            Task.Run(async () => await LoadAllForm());
            this.ITapCommand = new Command(OnItemTap);
        }
        private void OnItemTap()
        {

        }
        public FormListViewModel(Partner partner)
        {
            
            Partner = partner;
            Task.Run(async () => await LoadAllForm());

        }

        public FormListViewModel(int category)
        {
            //Partner = partner;
            Task.Run(async () => await LoadAllForm());

        }

        private async Task LoadAllForm()
        {
            //UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            //await Task.Delay(500);
            var C = Task.Run(() => Form.GetFormsByPartnerCategory(Partner.Category));
            FormList = new ObservableRangeCollection<Form>(await C);
            if (FormList.Count == 0)
            {
                NoForm = true;
            }
           // UserDialogs.Instance.HideLoading();

        }

        public async Task Reload()
        {
            await Task.Run(async () => await LoadAllForm());
        }
    }
}
