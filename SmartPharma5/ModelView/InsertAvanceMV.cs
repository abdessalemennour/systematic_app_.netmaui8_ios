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
using System;
*/
using
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.Model;
//using SmartPharma5.view;
Après :
//using SmartPharma5.view;
*/
SmartPharma5.Model;
using SmartPharma5.View;
//using SmartPharma5.view;
//using Xamarin.Forms;
//using maui.CommunityToolkit.Converters;

namespace SmartPharma5.ModelView
{
    public class InsertAvanceMV : BaseViewModel
    {
        private string description;
        public string Description { get => description; set => SetProperty(ref description, value); }


        private decimal amount;
        public decimal Amount { get => amount; set => SetProperty(ref amount, value); }




        private List<avance_type> list_avance_types;
        public List<avance_type> List_avance_types { get => list_avance_types; set => SetProperty(ref list_avance_types, value); }



        private avance_type selected_type = null;
        public avance_type Selected_type { get => selected_type; set => SetProperty(ref selected_type, value); }





        private bool testCon = false;
        public bool TestCon { get => testCon; set => SetProperty(ref testCon, value); }

        public AsyncCommand InsertAvanceCommande { get; }

        public InsertAvanceMV()
        {


            list_avance_types = avance_type.GetAllDeposit_type();
            InsertAvanceCommande = new AsyncCommand(insert);


        }


        public async Task insert()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);

            if (await DbConnection.Connecter3())
            {
                try
                {
                    await user_contrat.getActualPeriod();
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Warnning", "You dont have any actual period Please Contact Adminstartor!", "OK");
                    return;
                }

            }
            else
            {
                TestCon = true;
                return;
            }






            try
            {
                var verif = await App.Current.MainPage.DisplayAlert("Add", "Add Deposit request ?", "Yes", "No");
                if (verif)
                {

                    bool test = (selected_type == null);
                    int? i; ;
                    if (test)
                    {

                        i = await avance_request.InsertAvanceRequest(Description, user_contrat.contract, 1, 1, 1, Amount);

                    }
                    else
                    {
                        i = await avance_request.InsertAvanceRequest(Description, user_contrat.contract, 1, selected_type.Id, 1, Amount);


                    }
                    if (i == null)
                    {
                        TestCon = true;



                    }
                    else
                    {
                        avance_request avance = new avance_request();

                        try
                        {
                            avance = await avance_request.getLastDepositRequest();
                        }
                        catch
                        {

                        }




                        await avance_rquest_line.insertRequestLine(avance.id, Convert.ToDecimal(avance.amount), user_contrat.id_actual_period);

                        await App.Current.MainPage.Navigation.PushAsync(new HomeView());

                    }





                }

            }
            catch(Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                

            }

            UserDialogs.Instance.HideLoading();



        }


    }
}
