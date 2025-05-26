
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.Model;
Après :
using MvvmHelpers;
*/
using MvvmHelpers;
using MvvmHelpers.Commands;
using SmartPharma5.Model;


namespace SmartPharma5.ViewModel
{
    public class décisionAvanceMV : BaseViewModel
    {

        public int id { get; set; }
        public string description { get; set; }
        public decimal amount { get; set; }

        public int id_deposit { get; set; }

        public DateTime create_date { get; set; }
        public bool validate { get; set; }
        public string decision_description { get; set; }
        public DateTime date_decision { get; set; }
        public string nom_employe { get; set; }
        public string type_avance { get; set; }
        public string state_name { get; set; }

        public Color color { get; set; }

        public bool accepted { get; set; }

        public bool notAccepted { get; set; }

        public List<deposit_line> list_avance_line;
        public List<deposit_line> List_avance_line { get => list_avance_line; set => SetProperty(ref list_avance_line, value); }


        public AsyncCommand ChangeToDepositLine { get; }


        public décisionAvanceMV(int id, bool accept)
        {
            this.accepted = accept;
            this.notAccepted = (!accept);
            this.id = id;
            if (accept == true)
            {
                avance day;
                avance_request request;
                try
                {
                    day = avance.GetDayOffByRequestId(id);
                    request = avance_request.GetRequestDepositById(id).Result;
                }
                catch (Exception ex)
                {
                    return;
                }


                this.description = request.description;
                this.amount = day.amount;
                this.id_deposit = day.id;

                this.create_date = request.create_date;
                this.validate = day.validate;
                this.decision_description = day.description;
                this.date_decision = day.create_date;
                this.nom_employe = day.nom_employe;
                this.type_avance = day.type_avance;
                this.state_name = request.state_name;
                this.color = request.color;


            }
            else
            {
                avance_request request;
                try
                {
                    request = avance_request.GetRequestDepositById(id).Result;
                }
                catch (Exception ex)
                {
                    return;
                }


                this.description = request.description;

                this.create_date = request.create_date;
                this.validate = request.validated;
                this.decision_description = request.state_description;
                this.date_decision = request.create_date;
                this.nom_employe = request.nom_employe;
                this.type_avance = request.type_avance;
                this.state_name = request.state_name;
                this.color = request.color;

            }
            List_avance_line = deposit_line.getAllDepositLineByDeposit(this.id_deposit);
            //ChangeToDepositLine = new AsyncCommand(changeToDepositLineg);



        }


        public décisionAvanceMV() { }

        public async Task changeToDepositLineg()
        {
            int verif = await deposit_line.CheckExistDepositLine(this.id_deposit);
            if (verif == 0)
            {
                await App.Current.MainPage.DisplayAlert("INFO", "D'ONT HAVE ANY DEPOSIT LINE", "OK");
            }
            else
            {
                // await App.Current.MainPage.Navigation.PushAsync(new SmartPharma.View.deposit_line(this.id_deposit));

            }
        }
    }
}
