//using SmartPharma;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.Model;
Après :
using MvvmHelpers;
*/
using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using SmartPharma5.Model;
using SmartPharma5.View;

//using SmartPharma5.view;

namespace SmartPharma5.ModelView
{
    public class InsertCongé : BaseViewModel
    {
        private string description;
        public string Description { get => description; set => SetProperty(ref description, value); }

        private DateTime start_date;
        public DateTime Start_date { get => start_date; set => SetProperty(ref start_date, value); }

        private TimeSpan start_time;
        public TimeSpan Start_time { get => start_time; set => SetProperty(ref start_time, value); }

        private TimeSpan end_time;
        public TimeSpan End_time { get => end_time; set => SetProperty(ref end_time, value); }


        private DateTime end_date;

        public bool isAdded { get; set; }

        public DateTime End_date { get => end_date; set => SetProperty(ref end_date, value); }


        private DateTime minimum_date;

        public DateTime Minimum_date { get => minimum_date; set => SetProperty(ref minimum_date, value); }


        private DateTime maximum_date;

        public DateTime Maximum_date { get => maximum_date; set => SetProperty(ref maximum_date, value); }


        private List<day_off_type> day_Off_Types;
        public List<day_off_type> Day_Off_Types { get => day_Off_Types; set => SetProperty(ref day_Off_Types, value); }



        private day_off_type selected_type;
        public day_off_type Selected_type { get => selected_type; set => SetProperty(ref selected_type, value); }


        private bool testCon = false;
        public bool TestCon { get => testCon; set => SetProperty(ref testCon, value); }




        public AsyncCommand InsertCongeCommandet { get; }



        public InsertCongé()
        {
            user_contrat.getInfo().GetAwaiter().GetResult();
            Start_date = DateTime.Now;
            End_date = DateTime.Now.AddDays(1);
            day_Off_Types = day_off_type.GetAllDay_off_type();
            InsertCongeCommandet = new AsyncCommand(insert);
            Minimum_date = DateTime.Now;
            Maximum_date = DateTime.Now.AddYears(1);
        }

        /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
        Avant :
                private async Task  insert() {


                    if(Start_date == End_date)
        Après :
                private async Task  insert()
                {


                    if(Start_date == End_date)
        */
        private async Task insert()
        {
            //val.decimal_value.ToString().Replace(',', '.')
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);

            if (Start_date == End_date)
            {
                if (Start_time >= End_time)
                {
                    await App.Current.MainPage.DisplayAlert("Validation", "la date de retour n'est pat valider", "ok");

                }
                else
                {
                    var verif = await App.Current.MainPage.DisplayAlert("Ajouter une Demande Congé", "Voulez vous ajouter le demande ?", "Oui", "Non");


                    if (verif)
                    {
                        int? i = await day_off_request.InsertCongé(Description, Start_date, Start_time, End_date, End_time, user_contrat.contract, 0, selected_type.Id, 1);
                        if (i == null)
                        {
                            UserDialogs.Instance.HideLoading();
                            TestCon = true; return;
                            
                        }
                        else
                        {
                             await App.Current.MainPage.Navigation.PushAsync(new Day_Off_requestList(0));
                            UserDialogs.Instance.HideLoading();
                        }





                    }

                }
            }
            else
            {
                var verif = await App.Current.MainPage.DisplayAlert("Ajouter une Demande Congé", "Voulez vous ajouter le demande ?", "Oui", "Non");


                if (verif)
                {

                    int? i = await day_off_request.InsertCongé(Description, Start_date, Start_time, End_date, End_time, user_contrat.contract, 1, selected_type.Id, 1);
                    if (i == null)
                    {
                        UserDialogs.Instance.HideLoading();
                        TestCon = true; return;
                    }
                    else
                    {
                       
                        await App.Current.MainPage.Navigation.PushAsync(new Day_Off_requestList(0));
                        UserDialogs.Instance.HideLoading();

                    }

                }
            }









        }
    }
}
