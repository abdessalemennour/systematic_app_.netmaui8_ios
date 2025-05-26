//using SmartPharma;
//using SmartPharma.Model;
using MvvmHelpers;
using MvvmHelpers.Commands;
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System;
Après :
using SmartPharma5.Model;
using System;
*/

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System.Threading.Tasks;

using SmartPharma5.Model;
Après :
using System.Threading.Tasks;
*/


using SmartPharma5.Model;
//using SmartPharma5.view;

namespace SmartPharma5.ModelView
{
    public class decision_congé : BaseViewModel
    {
        public int id { get; set; }
        public string description { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public DateTime create_date { get; set; }
        public bool validate { get; set; }
        public string decision_description { get; set; }
        public DateTime date_decision { get; set; }
        public string nom_employe { get; set; }
        public string type_congé { get; set; }
        public string state_name { get; set; }

        public Color color { get; set; }

        public bool accepted { get; set; }

        public bool notAccepted { get; set; }

        public AsyncCommand ValiderDecicison { get; }

        public AsyncCommand RéfuserDécision { get; }

        public decision_congé(int id, bool accept)
        {
            this.accepted = accept;
            this.notAccepted = (!accept);
            this.id = id;
            if (accept == true)
            {
                day_off_request request = new day_off_request();
                try
                {
                    request = day_off_request.GetRequestDecisionDayOffById(id).Result;

                }
                catch (Exception ex)
                {
                    return;

                }


                day_off day = day_off.GetDayOffByRequestId(id);


                this.description = request.description;
                this.start_date = day.start_date;
                this.end_date = day.end_date;
                this.create_date = day.request_date;
                this.validate = day.validate;
                this.decision_description = day.description;
                this.date_decision = day.create_date;
                this.nom_employe = day.nom_employe;
                this.type_congé = day.type_congé;
                this.state_name = "ACCEPTE";
                this.color = Colors.Green;



            }
            else
            {
                day_off_request request = new day_off_request();
                try
                {
                    request = day_off_request.GetRequestDecisionDayOffById(id).Result;

                }
                catch (Exception ex)
                {
                    return;

                }

                this.description = request.description;

                this.create_date = request.create_date;
                this.validate = request.validate;
                this.decision_description = request.decision_description;
                this.date_decision = request.date_decision;
                this.nom_employe = request.nom_employe;
                this.type_congé = request.type_congé;
                this.state_name = request.state_name;
                this.color = request.color;

            }
            ValiderDecicison = new AsyncCommand(valider);
            RéfuserDécision = new AsyncCommand(refuser);

        }


        public async Task valider()
        {

            // await App.Current.MainPage.Navigation.PushAsync(new ValiderCongéView(this.id));


        }

        public async Task refuser()
        {
            day_off day = day_off.GetDayOffByRequestId(id);

            // await App.Current.MainPage.Navigation.PushAsync(new RéfuserCongéView(this.id,day.id));





        }

        public decision_congé()
        {

        }
    }
}
