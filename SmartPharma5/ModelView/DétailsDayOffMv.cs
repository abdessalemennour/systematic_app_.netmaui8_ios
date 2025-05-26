
//using SmartPharma;
//using SmartPharma.Model;
using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System;
Après :
using SmartPharma5.Model;
using SmartPharma5.View;
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
using SmartPharma5.View;
//using SmartPharma5.view;

namespace SmartPharma5.ModelView
{
    public class DétailsDayOffMv : BaseViewModel
    {

        public int id { get; set; }


        private string description1;
        public string Description1 { get => description1; set => SetProperty(ref description1, value); }

        private DateTime start_date;
        public DateTime Start_date { get => start_date; set => SetProperty(ref start_date, value); }

        private TimeSpan start_time;
        public TimeSpan Start_time { get => start_time; set => SetProperty(ref start_time, value); }

        private TimeSpan end_time;
        public TimeSpan End_time { get => end_time; set => SetProperty(ref end_time, value); }


        private DateTime end_date;

        public DateTime End_date { get => end_date; set => SetProperty(ref end_date, value); }


        private List<day_off_type> day_Off_Types;
        public List<day_off_type> Day_Off_Types { get => day_Off_Types; set => SetProperty(ref day_Off_Types, value); }

        private day_off_type selected_type;
        public day_off_type Selected_type { get => selected_type; set => SetProperty(ref selected_type, value); }

        public AsyncCommand updateCommande { get; }
        public AsyncCommand deleteCommande { get; }

        public AsyncCommand ValiderRequest { get; }
        

        public AsyncCommand RéfuserRequest { get; }
        private bool testCon = false;
        public bool TestCon { get => testCon; set => SetProperty(ref testCon, value); }

        public DétailsDayOffMv(int id)
        {
            this.id = id;
            day_Off_Types = day_off_type.GetAllDay_off_type();
            day_off_request day = new day_off_request();

            try
            {
                day = day_off_request.GetRequestDayOffById(id).Result;

            }
            catch (Exception ex)
            {
                return;
            }

            this.Start_date = day.start_date;
            Console.WriteLine(day.start_date);
            Console.WriteLine(day.end_date);
            Console.WriteLine(day.start_date);
            Console.WriteLine(day.end_date);
            this.End_date = day.end_date;
            this.description1 = day.description;
            day_off_type type = new day_off_type(day_off_type.getTypeById(day.type_congé).Id, day_off_type.getTypeById(day.type_congé).Description, day_off_type.getTypeById(day.type_congé).minus);
            this.Selected_type = type;
            Console.WriteLine("-----------------------------------8");
            Console.WriteLine(selected_type.Description);
            Console.WriteLine(selected_type.Id);
            Console.WriteLine(selected_type.minus);
            Console.WriteLine(Selected_type.ToString());
            this.Start_time = day.start_date.TimeOfDay;
            this.End_time = day.end_date.TimeOfDay;
            updateCommande = new AsyncCommand(update);

            deleteCommande = new AsyncCommand(supprimer);

            ValiderRequest = new AsyncCommand(Valider);
            RéfuserRequest = new AsyncCommand(Réfuser);


        }



        public DétailsDayOffMv() { }

        public async Task Valider()
        {
            //await App.Current.MainPage.Navigation.PushAsync(new ValiderCongéView(id));

        }
        public async Task Réfuser()
        {
            // await App.Current.MainPage.Navigation.PushAsync(new RéfuserCongéView(id));

        }
        public async Task update()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);

            var verif = await App.Current.MainPage.DisplayAlert("Modification demande", "Vous les vous modifier la demamnde ?", "OUI", "NON");



                   if (verif)
                    {
                       if (await DbConnection.Connecter3())
                       {
                           day_off_request.update(this.id, Description1, Start_date, Start_time, End_date, End_time, Selected_type.Id); ;
                    //await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new Day_Off_requestList(0)));
                    await App.Current.MainPage.Navigation.PopAsync();
                       }
                       else
                        {
                          TestCon = true;

                      }

                  }
            UserDialogs.Instance.HideLoading();
            //if (Start_date == End_date)
            //{
            //    if (Start_time >= End_time)
            //    {
            //        await App.Current.MainPage.DisplayAlert("Validation", "la date de retour n'est pat valider", "ok");

            //    }
            //    else
            //    {
            //        var verif = await App.Current.MainPage.DisplayAlert("Modification demande", "Vous les vous modifier la demamnde ?", "OUI", "Ok");



            //        if (verif)
            //        {
            //            if (await DbConnection.Connecter3())
            //            {
            //                day_off_request.update(this.id, Description1, Start_date, Start_time, End_date, End_time, Selected_type.Id); ;
            //                await App.Current.MainPage.Navigation.PushAsync(new Day_Off_requestList(0));
            //            }
            //            else
            //            {
            //                TestCon = true;

            //            }

            //        }

            //    }
            //}
            //else
            //{
            //    var verif = await App.Current.MainPage.DisplayAlert("Modification demande", "Vous les vous modifier la demamnde ?", "OUI", "Ok");


            //    if (verif)
            //    {
            //        if (await DbConnection.Connecter3())
            //        {
            //            var i = day_off_request.InsertCongé(Description1, Start_date, Start_time, End_date, End_time, user_contrat.contract, 1, selected_type.Id, 1); ;
            //            if (i == null)
            //            {
            //                await App.Current.MainPage.DisplayAlert("Warning", "Failed Connection", "OK");

            //            }
            //            else
            //            {
            //                await App.Current.MainPage.Navigation.PushAsync(new Day_Off_requestList(0));
            //            }
            //        }
            //        else
            //        {
            //            TestCon = true;
            //        }



            //    }
            // }



            // var verif = await App.Current.MainPage.DisplayAlert("Modification demande", "Vous les vous modifier la demamnde ?", "OUI", "Ok");


        }

        public async Task supprimer()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            var verif = await App.Current.MainPage.DisplayAlert("Suppression demande", "Vous les vous supprimer la demamnde ?", "OUI", "Ok");
            if (verif)
            {
                if (await DbConnection.Connecter3())
                {
                    day_off_request.delete(id);
                    await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new Day_Off_requestList(0)));
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
