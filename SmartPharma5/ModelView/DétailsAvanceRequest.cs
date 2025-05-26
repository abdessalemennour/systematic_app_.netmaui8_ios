//using SmartPharma;
//using SmartPharma.Model;
//using SmartPharma.View;
using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SQLitePCL;
Après :
using SmartPharma5.Model;
using SmartPharma5.View;
using SQLitePCL;
*/
using
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System.Threading.Tasks;
using SmartPharma5.Model;
using SmartPharma5.View;
Après :
using System.Threading.Tasks;
*/
SmartPharma5.Model;
using SmartPharma5.View;

namespace SmartPharma5.ModelView
{
    public class DétailsAvanceRequest : BaseViewModel
    {
        public int id { get; set; }


        private string description1;
        public string Description1 { get => description1; set => SetProperty(ref description1, value); }

        private string total_amount;
        public string Total_amout { get => total_amount; set => SetProperty(ref total_amount, value); }




        private List<avance_type> list_avance_types;
        public List<avance_type> List_avance_types { get => list_avance_types; set => SetProperty(ref list_avance_types, value); }

        private avance_type selected_type;
        public avance_type Selected_type { get => selected_type; set => SetProperty(ref selected_type, value); }

        public AsyncCommand updateCommande { get; }
        public AsyncCommand deleteCommande { get; }

        public AsyncCommand ValiderRequest { get; }

        public AsyncCommand RéfuserRequest { get; }

        private bool testCon = false;
        public bool TestCon { get => testCon; set => SetProperty(ref testCon, value); }

        public DétailsAvanceRequest(int id)
        {

            avance_request avanceRequestAtt;
            this.id = id;
            List_avance_types = avance_type.GetAllDeposit_type();
            try
            {
                avanceRequestAtt = avance_request.GetRequestDepositById(this.id).Result;
            }
            catch (Exception ex)
            {
                return;
            }

            this.Description1 = avanceRequestAtt.description;

            this.Total_amout = avanceRequestAtt.amount.ToString();
            avance_type type = new avance_type(avance_type.getTypeById(avanceRequestAtt.type_avance).Id, avance_type.getTypeById(avanceRequestAtt.type_avance).Description);
            this.Selected_type = type;
            updateCommande = new AsyncCommand(update);
            deleteCommande = new AsyncCommand(delete);
            ValiderRequest = new AsyncCommand(valider);



        }

        public async Task valider()
        {
            // await App.Current.MainPage.Navigation.PushAsync(new Validation_AvanceRequest());
        }

        public async Task delete()
        {
            var verif = await App.Current.MainPage.DisplayAlert("DELETE", "Delete Deposit Request ?", "Yes", "No");
            if (verif)
            {
                if (await DbConnection.Connecter3())
                {
                    await avance_request.delete(this.id);
                    await avance_rquest_line.delete(this.id);
                    await App.Current.MainPage.Navigation.PushAsync(new Avance_list(0));

                }
                else
                {
                    TestCon = true;
                }

            }

        }
        public DétailsAvanceRequest()
        {
            List_avance_types = avance_type.GetAllDeposit_type();
        }
        public async Task update()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            var verif = await App.Current.MainPage.DisplayAlert("Update", "Update Deposit Request ?", "Yes", "No");
            if (verif)
            {
                if (await DbConnection.Connecter3())
                {
                    await avance_request.update(this.id, Convert.ToDecimal(Total_amout), Description1, selected_type.Id);
                    await avance_rquest_line.update(this.id, Convert.ToDecimal(Total_amout));
                    await App.Current.MainPage.Navigation.PopAsync();

                }
                else
                {
                    TestCon = true;
                }

            }
            UserDialogs.Instance.HideLoading();

        }
    }
}
