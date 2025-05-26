
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5;
Après :
using Acr.UserDialogs;
*/
using Acr.UserDialogs;
//using Foundation;
using MvvmHelpers;
//using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.View;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Data;
//using SmartPharma5.view;
Après :
using SmartPharma5;
using SmartPharma5.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Linq;
//using SmartPharma5.view;
*/
//using SmartPharma5.view;
//using DevExpress.XamarinForms.Charts;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using 
Après :
using
*/
using System.Data;
//using Acr.UserDialogs;

namespace SmartPharma5.Model
{
    public class day_off_request
    {

        #region attribut
        public int id { get; set; }
        public string description { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public DateTime create_date { get; set; }
        public bool validate { get; set; }
        public string decision_description { get; set; }

        public DateTime date_decision { get; set; }

        public Color color { get; set; }

        public string nom_employe { get; set; }

        public string type_congé { get; set; }

        public string state_name { get; set; }

        public int contract { get; set; }

        public bool decision_validate { get; set; }

        public bool decision_notvalidated { get; set; }

        public bool day_off_created { get; set; }

        public bool day_off_not_created { get; set; }

        public DateTime? start_date_finale { get; set; }

        public DateTime? end_date_finale { get; set; }







        public Command getDétails { get; }

        public Command getDécision { get; }

        #endregion

        #region constructor
        public day_off_request()
        {


        }
        public day_off_request(int id, string description, DateTime start_date, DateTime end_date, DateTime create_dateAtt, bool validate, Color color, string nom_employe, string type_congé, string state_name, int contract, bool day_off_created)
        {
            try
            {


                this.id = id;
                this.description = description;

                this.create_date = create_dateAtt;
                this.validate = validate;
                this.contract = contract;
                this.start_date = start_date;
                this.end_date = end_date;


                this.color = color;
                this.nom_employe = nom_employe;
                this.type_congé = type_congé;
                this.state_name = state_name;
                this.day_off_created = day_off_created;
                this.day_off_not_created = !this.day_off_created;


                if (state_name == "EN ATTENTE")
                {
                    this.decision_notvalidated = true;
                    this.decision_validate = false;
                }
                else
                {

                    this.decision_notvalidated = false;
                    this.decision_validate = true;








                }
                getDétails = new Command(async (flag) =>
                {
                    if (await DbConnection.Connecter3())
                    {
                         UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                         await Task.Delay(500);
                        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new SmartPharma5.View.DétailsView(Convert.ToInt32(flag))));
                         UserDialogs.Instance.HideLoading();
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Warning", "ConnectionFailed", "Ok");
                    }




                });


                getDécision = new Command(async (flag) =>
                {

                    if (await DbConnection.Connecter3())
                    {
                        if (state_name == "ACCEPTE")
                        {


                            if (TestCreated(id))
                            {
                                //   UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                                await Task.Delay(500);
                                await App.Current.MainPage.Navigation.PushAsync(new DécisionView(Convert.ToInt32(flag), true));
                                //  UserDialogs.Instance.HideLoading();

                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("Done", "DAY OFF NOT CREATED YET", "Ok");
                            }




                        }
                        else
                        {
                            //  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                            await Task.Delay(500);
                            await App.Current.MainPage.Navigation.PushAsync(new DécisionView(Convert.ToInt32(flag), false));
                            // UserDialogs.Instance.HideLoading();
                        }

                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Warning", "ConnectionFailed", "Ok");

                        /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                        Avant :
                                        }




                                    });
                        Après :
                                        }




                                        });
                        */
                    }




                });
            }
            catch (Exception ex)
            {

            }
        }

        public day_off_request(int id, string description, DateTime start_date, DateTime end_date, DateTime create_dateAtt, bool validate, Color color, string nom_employe, string type_congé, string state_name, int contract, string decisiopn_description, DateTime date_decisionAtt)
        {
            this.id = id;
            this.description = description;

            this.create_date = create_dateAtt;
            this.validate = validate;
            this.contract = contract;
            this.date_decision = date_decisionAtt;
            this.decision_description = decisiopn_description;
            this.start_date = start_date;
            this.end_date = end_date;



            this.color = color;
            this.nom_employe = nom_employe;
            this.type_congé = type_congé;
            this.state_name = state_name;



            if (state_name == "EN ATTENTE")
            {
                this.decision_notvalidated = true;
                this.decision_validate = false;
            }
            else
            {
                this.decision_notvalidated = false;
                this.decision_validate = true;








            }
            getDétails = new Command(async (flag) =>
            {
                if (await DbConnection.Connecter3())
                {
                    //  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                    await Task.Delay(500);
                    // await App.Current.MainPage.Navigation.PushAsync(new SmartPharma5.view.DétailsView(Convert.ToInt32(flag)));
                    //  UserDialogs.Instance.HideLoading();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "ConnectionFailed", "Ok");
                }




            });


            getDécision = new Command(async (flag) =>
            {

                if (await DbConnection.Connecter3())
                {
                    if (state_name == "ACCEPTE")
                    {


                        if (TestCreated(id))
                        {
                            //  UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                            await Task.Delay(500);
                            //  await App.Current.MainPage.Navigation.PushAsync(new DécisionView(Convert.ToInt32(flag), true));
                            // UserDialogs.Instance.HideLoading();

                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Done", "DAY OFF NOT CREATED YET", "Ok");
                        }




                    }
                    else
                    {
                        // UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                        await Task.Delay(500);
                        // await App.Current.MainPage.Navigation.PushAsync(new DécisionView(Convert.ToInt32(flag), false));
                        //UserDialogs.Instance.HideLoading();
                    }

                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "ConnectionFailed", "Ok");
                }




            });
        }

        #endregion

        #region GetMethode
        public async static Task<day_off_request> GetRequestDayOffById(int id)
        {

            string sqlCmd1 = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
            ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
            ",atooerp_user.last_name as last_name,atooerp_user.first_name as first_name from hr_day_off_request " +
            "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
            "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
            "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
            "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
            "left join hr_employe employe on employe.id = contract.employe " +
            "left join atooerp_user  on atooerp_user.id = employe.user " +
            "where hr_day_off_request.id=" + id + " " +
            "order by hr_day_off_request.create_date desc; ";
            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    reader.Read();

                    Color colorCheck;
                    if (Convert.ToInt32(reader["state"]) == 1)
                    {
                        colorCheck = Colors.Gray;
                    }
                    else if (Convert.ToInt32(reader["state"]) == 2)
                    {
                        colorCheck = Colors.Green;
                    }
                    else
                    {
                        colorCheck = Colors.Red;
                    }

                    DateTime start_date1;
                    DateTime end_date1;
                    bool valide;

                    if (Convert.ToString(reader["validated_day_off"]) == "")
                    {
                        start_date1 = Convert.ToDateTime(reader["start_date"]);
                        end_date1 = Convert.ToDateTime(reader["end_date"]);
                        valide = false;
                    }
                    else
                    {
                        start_date1 = Convert.ToDateTime(reader["start_date_final"]);
                        end_date1 = Convert.ToDateTime(reader["end_date_final"]);
                        valide = true;
                    }
                    day_off_request day = new day_off_request(Convert.ToInt32(reader["id"]),
                               reader["description"].ToString(),
                               start_date1,
                               end_date1,
                               Convert.ToDateTime(reader["create_date"]),
                               Convert.ToBoolean(reader["validated"]),
                               colorCheck,
                               Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                               Convert.ToString(reader["name_type"]),
                               Convert.ToString(reader["state_name"]),
                               Convert.ToInt32(reader["contract"]), valide
                               );
                    reader.Close();
                    return day;
                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;
                }

            }
            else
            {
                return null;
            }








        }


        public async static Task<day_off_request> GetRequestDecisionDayOffById(int id)
        {

            string sqlCmd1 = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
              ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
              ",atooerp_user.last_name as last_name,atooerp_user.first_name as first_name from hr_day_off_request " +
              "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
              "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
              "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
              "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
              "left join hr_employe employe on employe.id = contract.employe " +
              "left join atooerp_user  on atooerp_user.id = employe.user " +
              "where hr_day_off_request.id=" + id + " " +
              "order by hr_day_off_request.create_date desc; ";







            Console.WriteLine(sqlCmd1);
            MySqlDataReader reader = null;






            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    reader.Read();

                    Color colorCheck;
                    if (Convert.ToInt32(reader["state"]) == 1)
                    {
                        colorCheck = Colors.Gray;
                    }
                    else if (Convert.ToInt32(reader["state"]) == 2)
                    {
                        colorCheck = Colors.Green;
                    }
                    else
                    {
                        colorCheck = Colors.Red;
                    }
                    DateTime start_date1;
                    DateTime end_date1;

                    if (Convert.ToString(reader["validated_day_off"]) == "")
                    {
                        start_date1 = Convert.ToDateTime(reader["start_date"]);
                        end_date1 = Convert.ToDateTime(reader["end_date"]);




                    }
                    else
                    {
                        start_date1 = Convert.ToDateTime(reader["start_date_final"]);
                        end_date1 = Convert.ToDateTime(reader["end_date_final"]);


                    }



                    day_off_request day = new day_off_request(Convert.ToInt32(reader["id"]),
                               reader["description"].ToString(),
                               start_date1,
                               end_date1,
                               Convert.ToDateTime(reader["create_date"]),
                               Convert.ToBoolean(reader["validated"]),
                               colorCheck,
                               Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                               Convert.ToString(reader["name_type"]),
                               Convert.ToString(reader["state_name"]),
                            Convert.ToInt32(reader["contract"]),
                            Convert.ToString(reader["description_decision"]),
                            Convert.ToDateTime(reader["decision_date"])


                               );

                    reader.Close();
                    return day;

                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;
                }

            }
            else
            {
                return null;

            }





        }
        public async static Task<List<day_off_request>> getDataByDate(string start_date, string end_date)
        {
            string sqlCmd = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
               ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
               ",atooerp_user.last_name as last_name,atooerp_user.first_name as first_name from hr_day_off_request " +
               "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
               "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
               "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
               "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
               "left join hr_employe employe on employe.id = contract.employe " +
               "left join atooerp_user  on atooerp_user.id = employe.user " +
               "where hr_day_off_request.create_date between '" + start_date + "' and '" + end_date + "' " +
               "order by hr_day_off_request.create_date desc; ";




            List<day_off_request> list = new List<day_off_request>();
            MySqlDataReader reader = null;

            if (await DbConnection.Connecter3())
            {

                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Color colorCheck;
                        if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            colorCheck = Colors.Gray;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            colorCheck = Colors.Green;
                        }
                        else
                        {
                            colorCheck = Colors.Red;
                        }
                        bool valide;
                        if (Convert.ToString(reader["validated_day_off"]) == "") { valide = false; } else { valide = true; }

                        day_off_request attribute = new day_off_request(Convert.ToInt32(reader["id"]),
                              reader["description"].ToString(),
                              Convert.ToDateTime(reader["start_date"]),
                              Convert.ToDateTime(reader["end_date"]),
                              Convert.ToDateTime(reader["create_date"]),
                              Convert.ToBoolean(reader["validated"]),
                              colorCheck,
                              Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                              Convert.ToString(reader["name_type"]),
                              Convert.ToString(reader["state_name"]),
                           Convert.ToInt32(reader["contract"]), valide);

                        if (Convert.ToString(reader["validated_day_off"]) != "")
                        {
                            attribute.start_date_finale = Convert.ToDateTime(reader["start_date_final"]);
                            attribute.end_date_finale = Convert.ToDateTime(reader["end_date_final"]);

                        }


                        list.Add(attribute);







                    }
                    reader.Close();

                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;

                }

            }
            else
            {
                return null;
            }





            return list;


        }

        public bool TestCreated(int id1)
        {
            if (day_off.checkDayOffCreated(id1))
                return true;
            else return false;
        }




        public async static Task<List<day_off_request>> GetRequestDayOff()
        {
            string sqlCmd = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
                ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
                ",atooerp_person.last_name as last_name,atooerp_person.first_name as first_name from hr_day_off_request " +
                "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
                "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
                "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
                "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
                "left join hr_employe employe on employe.id = contract.employe " +
                "left join atooerp_user  on atooerp_user.id = employe.user " +
                 "left join atooerp_person  on atooerp_person.id = employe.Id    " +
                "order by hr_day_off_request.create_date desc; ";
            Console.WriteLine(sqlCmd);

            List<day_off_request> list = new List<day_off_request>();
            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {


                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Color colorCheck;
                        if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            colorCheck = Colors.Gray;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            colorCheck = Colors.Green;
                        }
                        else
                        {
                            colorCheck = Colors.Red;
                        }

                        bool valide;
                        if (Convert.ToString(reader["validated_day_off"]) == "") { valide = false; } else { valide = true; }

                        day_off_request attribute = new day_off_request(Convert.ToInt32(reader["id"]),
                              reader["description"].ToString(),
                              Convert.ToDateTime(reader["start_date"]),
                              Convert.ToDateTime(reader["end_date"]),
                              Convert.ToDateTime(reader["create_date"]),
                              Convert.ToBoolean(reader["validated"]),
                              colorCheck,
                              Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                              Convert.ToString(reader["name_type"]),
                              Convert.ToString(reader["state_name"]),
                           Convert.ToInt32(reader["contract"]), valide);

                        if (Convert.ToString(reader["validated_day_off"]) != "")
                        {
                            attribute.start_date_finale = Convert.ToDateTime(reader["start_date_final"]);
                            attribute.end_date_finale = Convert.ToDateTime(reader["end_date_final"]);

                        }

                        list.Add(attribute);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    reader.Close();

                    return null;

                }


            }
            else
            {
                return null;
            }




            return list;

        }
        public static List<day_off_request> getByStartName(string name)
        {
            string sqlCmd = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
                ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
                ",atooerp_user.last_name as last_name,atooerp_user.first_name as first_name from hr_day_off_request " +
                "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
                "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
                "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
                "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
                "left join hr_employe employe on employe.id = contract.employe " +
                "left join atooerp_user  on atooerp_user.id = employe.user " +
                "order by hr_day_off_request.create_date desc; ";

            List<day_off_request> list = new List<day_off_request>();

            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);


            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Color colorCheck;
                if (Convert.ToBoolean(reader["validate"]))
                {
                    colorCheck = Colors.Green;
                }
                else
                {
                    colorCheck = Colors.Red;
                }
                bool valide;
                if (Convert.ToString(reader["validated_day_off"]) == "") { valide = false; } else { valide = true; }

                day_off_request attribute = new day_off_request(Convert.ToInt32(reader["id"]),
                      reader["description"].ToString(),
                      Convert.ToDateTime(reader["start_date"]),
                      Convert.ToDateTime(reader["end_date"]),
                      Convert.ToDateTime(reader["create_date"]),
                      Convert.ToBoolean(reader["validated"]),
                      colorCheck,
                      Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                      Convert.ToString(reader["name_type"]),
                      Convert.ToString(reader["state_name"]),
                   Convert.ToInt32(reader["contract"]), valide);

                if (Convert.ToString(reader["validated_day_off"]) != "")
                {
                    attribute.start_date_finale = Convert.ToDateTime(reader["start_date_final"]);
                    attribute.end_date_finale = Convert.ToDateTime(reader["end_date_final"]);

                }


                list.Add(attribute);







            }
            reader.Close();
            DbConnection.Deconnecter();

            ObservableRangeCollection<day_off_request> list2 = new ObservableRangeCollection<day_off_request>(list);
            List<day_off_request> Result = new List<day_off_request>();
            Result = list2.Where(s => s.nom_employe.StartsWith(name)).ToList();

            foreach (day_off_request item in Result)
            {
                Console.WriteLine(item.description);
            }

            return Result;



        }

        public static day_off_request GetRequestById(int id)
        {

            string sqlCmd1 = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
                ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
                ",atooerp_user.last_name as last_name,atooerp_user.first_name as first_name from hr_day_off_request " +
                "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
                "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
                "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
                "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
                "left join hr_employe employe on employe.id = contract.employe " +
                "left join atooerp_user  on atooerp_user.id = employe.user " +
                " where hr_day_off_request.id =" + id + " " +
                "order by hr_day_off_request.create_date desc; ";

            Console.WriteLine(sqlCmd1);






            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);


            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            Color colorCheck;
            if (Convert.ToInt32(reader["state"]) == 1)
            {
                colorCheck = Colors.Gray;
            }
            else if (Convert.ToInt32(reader["state"]) == 2)
            {
                colorCheck = Colors.Green;
            }
            else
            {
                colorCheck = Colors.Red;
            }
            DateTime start_date;
            DateTime end_date;
            bool valide;
            if (Convert.ToString(reader["validated_day_off"]) == "")
            {
                start_date = Convert.ToDateTime(reader["start_date"]);
                end_date = Convert.ToDateTime(reader["end_date"]);
                valide = false;



            }
            else
            {
                start_date = Convert.ToDateTime(reader["start_date_final"]);
                end_date = Convert.ToDateTime(reader["end_date_final"]);
                valide = true;

            }



            day_off_request day = new day_off_request(Convert.ToInt32(reader["id"]),
                       reader["description"].ToString(),
                       start_date,
                       end_date,
                       Convert.ToDateTime(reader["create_date"]),
                       Convert.ToBoolean(reader["validated"]),
                       colorCheck,
                       Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                       Convert.ToString(reader["name_type"]),
                       Convert.ToString(reader["state_name"]),
                    Convert.ToInt32(reader["contract"]), valide


                       );

            reader.Close();
            DbConnection.Deconnecter();


            return day;


        }


        public async static Task<List<day_off_request>> getDataByState(int state)
        {
            string sqlCmd = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
               ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
               ",atooerp_user.last_name as last_name,atooerp_user.first_name as first_name from hr_day_off_request " +
               "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
               "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
               "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
               "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
               "left join hr_employe employe on employe.id = contract.employe " +
               "left join atooerp_user  on atooerp_user.id = employe.user " +
               "where hr_day_off_request.state = " + state + "  " +
               "order by hr_day_off_request.create_date desc; ";
            List<day_off_request> list = new List<day_off_request>();
            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {

                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Color colorCheck;
                        if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            colorCheck = Colors.Gray;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            colorCheck = Colors.Green;
                        }
                        else
                        {
                            colorCheck = Colors.Red;
                        }
                        bool valide;
                        if (Convert.ToString(reader["validated_day_off"]) == "") { valide = false; } else { valide = true; }

                        day_off_request attribute = new day_off_request(Convert.ToInt32(reader["id"]),
                              reader["description"].ToString(),
                              Convert.ToDateTime(reader["start_date"]),
                              Convert.ToDateTime(reader["end_date"]),
                              Convert.ToDateTime(reader["create_date"]),
                              Convert.ToBoolean(reader["validated"]),
                              colorCheck,
                              Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                              Convert.ToString(reader["name_type"]),
                              Convert.ToString(reader["state_name"]),
                           Convert.ToInt32(reader["contract"]), valide);

                        if (Convert.ToString(reader["validated_day_off"]) != "")
                        {
                            attribute.start_date_finale = Convert.ToDateTime(reader["start_date_final"]);
                            attribute.end_date_finale = Convert.ToDateTime(reader["end_date_final"]);

                        }
                        list.Add(attribute);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;
                }
            }
            else
            {
                return null;

            }
            return list;


        }




        public async static Task<List<day_off_request>> getDataByEndDate(DateTime end_date)
        {
            string sqlCmd = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
               ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
               ",atooerp_user.last_name as last_name,atooerp_user.first_name as first_name from hr_day_off_request " +
               "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
               "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
               "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
               "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
               "left join hr_employe employe on employe.id = contract.employe " +
               "left join atooerp_user  on atooerp_user.id = employe.user " +
               "where hr_day_off_request.end_date between '" + end_date.ToString("yyyy-MM-dd") + "' and   '" + end_date.AddDays(1).ToString("yyyy-MM-dd") + "' " +
               "order by hr_day_off_request.create_date desc; ";

            List<day_off_request> list = new List<day_off_request>();
            MySqlDataReader reader = null;


            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);


                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Color colorCheck;
                        if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            colorCheck = Colors.Gray;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            colorCheck = Colors.Green;
                        }
                        else
                        {
                            colorCheck = Colors.Red;
                        }
                        bool valide;
                        if (Convert.ToString(reader["validated_day_off"]) == "") { valide = false; } else { valide = true; }

                        day_off_request attribute = new day_off_request(Convert.ToInt32(reader["id"]),
                              reader["description"].ToString(),
                              Convert.ToDateTime(reader["start_date"]),
                              Convert.ToDateTime(reader["end_date"]),
                              Convert.ToDateTime(reader["create_date"]),
                              Convert.ToBoolean(reader["validated"]),
                              colorCheck,
                              Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                              Convert.ToString(reader["name_type"]),
                              Convert.ToString(reader["state_name"]),
                           Convert.ToInt32(reader["contract"]), valide);

                        if (Convert.ToString(reader["validated_day_off"]) != "")
                        {
                            attribute.start_date_finale = Convert.ToDateTime(reader["start_date_final"]);
                            attribute.end_date_finale = Convert.ToDateTime(reader["end_date_final"]);

                        }


                        list.Add(attribute);







                    }
                    reader.Close();

                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;
                }

            }
            else
            {
                return null;
            }





            return list;


        }


        public async static Task<List<day_off_request>> getDataByStartDate(DateTime start_date)
        {
            string sqlCmd = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
               ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
               ",atooerp_user.last_name as last_name,atooerp_user.first_name as first_name from hr_day_off_request " +
               "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
               "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
               "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
               "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
               "left join hr_employe employe on employe.id = contract.employe " +
               "left join atooerp_user  on atooerp_user.id = employe.user " +
               "where hr_day_off_request.start_date between '" + start_date.ToString("yyyy-MM-dd") + "' and   '" + start_date.AddDays(1).ToString("yyyy-MM-dd") + "' " +
               "order by hr_day_off_request.create_date desc; ";


            Console.WriteLine(sqlCmd);

            List<day_off_request> list = new List<day_off_request>();
            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Color colorCheck;
                        if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            colorCheck = Colors.Gray;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            colorCheck = Colors.Green;
                        }
                        else
                        {
                            colorCheck = Colors.Red;
                        }
                        bool valide;
                        if (Convert.ToString(reader["validated_day_off"]) == "") { valide = false; } else { valide = true; }

                        day_off_request attribute = new day_off_request(Convert.ToInt32(reader["id"]),
                              reader["description"].ToString(),
                              Convert.ToDateTime(reader["start_date"]),
                              Convert.ToDateTime(reader["end_date"]),
                              Convert.ToDateTime(reader["create_date"]),
                              Convert.ToBoolean(reader["validated"]),
                              colorCheck,
                              Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                              Convert.ToString(reader["name_type"]),
                              Convert.ToString(reader["state_name"]),
                           Convert.ToInt32(reader["contract"]), valide);

                        if (Convert.ToString(reader["validated_day_off"]) != "")
                        {
                            attribute.start_date_finale = Convert.ToDateTime(reader["start_date_final"]);
                            attribute.end_date_finale = Convert.ToDateTime(reader["end_date_final"]);
                        }
                        list.Add(attribute);
                    }
                    reader.Close();

                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;

                }

            }
            else
            {
                return null;
            }
            return list;


        }


        public async static Task<List<day_off_request>> getRequestByUserId()
        {
            string sqlCmd = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
               ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
               ",atooerp_user.last_name as last_name,atooerp_user.first_name as first_name from hr_day_off_request " +
               "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
               "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
               "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
               "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
               "left join hr_employe employe on employe.id = contract.employe " +
               "left join atooerp_user  on atooerp_user.id = employe.user " +
               "where atooerp_user.id = " + user_contrat.iduser + " " +
               "order by hr_day_off_request.create_date desc; ";




            List<day_off_request> list = new List<day_off_request>();
            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Color colorCheck;
                        if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            colorCheck = Colors.Gray;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            colorCheck = Colors.Green;
                        }
                        else
                        {
                            colorCheck = Colors.Red;
                        }
                        bool valide;
                        if (Convert.ToString(reader["validated_day_off"]) == "") { valide = false; } else { valide = true; }

                        day_off_request attribute = new day_off_request(Convert.ToInt32(reader["id"]),
                              reader["description"].ToString(),
                              Convert.ToDateTime(reader["start_date"]),
                              Convert.ToDateTime(reader["end_date"]),
                              Convert.ToDateTime(reader["create_date"]),
                              Convert.ToBoolean(reader["validated"]),
                              colorCheck,
                              Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                              Convert.ToString(reader["name_type"]),
                              Convert.ToString(reader["state_name"]),
                           Convert.ToInt32(reader["contract"]), valide);

                        if (Convert.ToString(reader["validated_day_off"]) != "")
                        {
                            attribute.start_date_finale = Convert.ToDateTime(reader["start_date_final"]);
                            attribute.end_date_finale = Convert.ToDateTime(reader["end_date_final"]);

                        }


                        list.Add(attribute);







                    }
                    reader.Close();

                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;

                }

            }
            else
            {
                return null;

            }





            return list;


        }




        /**********************************************************************/

        public async static Task<List<day_off_request>> getDataByEndDateAndUser(DateTime end_date)
        {
            string sqlCmd = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
               ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
               ",atooerp_user.last_name as last_name,atooerp_user.first_name as first_name from hr_day_off_request " +
               "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
               "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
               "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
               "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
               "left join hr_employe employe on employe.id = contract.employe " +
               "left join atooerp_user  on atooerp_user.id = employe.user " +
               "where hr_day_off_request.end_date between '" + end_date.ToString("yyyy-MM-dd") + "' and   '" + end_date.AddDays(1).ToString("yyyy-MM-dd") + "' and  " +
               "atooerp_user.id = " + user_contrat.iduser + " " +
               "order by hr_day_off_request.create_date desc; ";
            List<day_off_request> list = new List<day_off_request>();
            MySqlDataReader reader = null;

            if (await DbConnection.Connecter3())
            {

                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Color colorCheck;
                        if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            colorCheck = Colors.Gray;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            colorCheck = Colors.Green;
                        }
                        else
                        {
                            colorCheck = Colors.Red;
                        }
                        bool valide;
                        if (Convert.ToString(reader["validated_day_off"]) == "") { valide = false; } else { valide = true; }

                        day_off_request attribute = new day_off_request(Convert.ToInt32(reader["id"]),
                              reader["description"].ToString(),
                              Convert.ToDateTime(reader["start_date"]),
                              Convert.ToDateTime(reader["end_date"]),
                              Convert.ToDateTime(reader["create_date"]),
                              Convert.ToBoolean(reader["validated"]),
                              colorCheck,
                              Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                              Convert.ToString(reader["name_type"]),
                              Convert.ToString(reader["state_name"]),
                           Convert.ToInt32(reader["contract"]), valide);

                        if (Convert.ToString(reader["validated_day_off"]) != "")
                        {
                            attribute.start_date_finale = Convert.ToDateTime(reader["start_date_final"]);
                            attribute.end_date_finale = Convert.ToDateTime(reader["end_date_final"]);
                        }
                        list.Add(attribute);
                    }
                    reader.Close();

                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;

                }

            }
            else
            {
                return null;

            }
            return list;


        }


        public async static Task<List<day_off_request>> getDataByStartDateAndUser(DateTime start_date)
        {
            string sqlCmd = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
               ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
               ",atooerp_user.last_name as last_name,atooerp_user.first_name as first_name from hr_day_off_request " +
               "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
               "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
               "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
               "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
               "left join hr_employe employe on employe.id = contract.employe " +
               "left join atooerp_user  on atooerp_user.id = employe.user " +
               "where hr_day_off_request.start_date between '" + start_date.ToString("yyyy-MM-dd") + "' and   '" + start_date.AddDays(1).ToString("yyyy-MM-dd") + "' and " +
               "atooerp_user.id = " + user_contrat.iduser + "  " +
               "order by hr_day_off_request.create_date desc; ";




            List<day_off_request> list = new List<day_off_request>();
            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Color colorCheck;
                        if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            colorCheck = Colors.Gray;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            colorCheck = Colors.Green;
                        }
                        else
                        {
                            colorCheck = Colors.Red;
                        }
                        bool valide;
                        if (Convert.ToString(reader["validated_day_off"]) == "") { valide = false; } else { valide = true; }

                        day_off_request attribute = new day_off_request(Convert.ToInt32(reader["id"]),
                              reader["description"].ToString(),
                              Convert.ToDateTime(reader["start_date"]),
                              Convert.ToDateTime(reader["end_date"]),
                              Convert.ToDateTime(reader["create_date"]),
                              Convert.ToBoolean(reader["validated"]),
                              colorCheck,
                              Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                              Convert.ToString(reader["name_type"]),
                              Convert.ToString(reader["state_name"]),
                           Convert.ToInt32(reader["contract"]), valide);

                        if (Convert.ToString(reader["validated_day_off"]) != "")
                        {
                            attribute.start_date_finale = Convert.ToDateTime(reader["start_date_final"]);
                            attribute.end_date_finale = Convert.ToDateTime(reader["end_date_final"]);

                        }
                        list.Add(attribute);
                    }
                    reader.Close();

                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;

                }

            }
            else
            {
                return null;

            }





            return list;


        }


        public async static Task<List<day_off_request>> getDataByStateAndUser(int state)
        {
            string sqlCmd = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
               ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
               ",atooerp_user.last_name as last_name,atooerp_user.first_name as first_name from hr_day_off_request " +
               "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
               "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
               "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
               "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
               "left join hr_employe employe on employe.id = contract.employe " +
               "left join atooerp_user  on atooerp_user.id = employe.user " +
               "where hr_day_off_request.state = " + state + " and " +
               "atooerp_user.id = " + user_contrat.iduser + " " +
               "order by hr_day_off_request.create_date desc; ";


            Console.WriteLine(sqlCmd);

            List<day_off_request> list = new List<day_off_request>();
            MySqlDataReader reader = null;

            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Color colorCheck;
                        if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            colorCheck = Colors.Gray;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            colorCheck = Colors.Green;
                        }
                        else
                        {
                            colorCheck = Colors.Red;
                        }
                        bool valide;
                        if (Convert.ToString(reader["validated_day_off"]) == "") { valide = false; } else { valide = true; }

                        day_off_request attribute = new day_off_request(Convert.ToInt32(reader["id"]),
                              reader["description"].ToString(),
                              Convert.ToDateTime(reader["start_date"]),
                              Convert.ToDateTime(reader["end_date"]),
                              Convert.ToDateTime(reader["create_date"]),
                              Convert.ToBoolean(reader["validated"]),
                              colorCheck,
                              Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                              Convert.ToString(reader["name_type"]),
                              Convert.ToString(reader["state_name"]),
                           Convert.ToInt32(reader["contract"]), valide);

                        if (Convert.ToString(reader["validated_day_off"]) != "")
                        {
                            attribute.start_date_finale = Convert.ToDateTime(reader["start_date_final"]);
                            attribute.end_date_finale = Convert.ToDateTime(reader["end_date_final"]);
                        }
                        list.Add(attribute);
                    }
                    reader.Close();


                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;
                }
            }
            else
            {
                return null;

            }
            return list;


        }

        public static List<day_off_request> getByStartNameAndUser(string name)
        {
            string sqlCmd = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
                ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
                ",atooerp_user.last_name as last_name,atooerp_user.first_name as first_name from hr_day_off_request " +
                "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
                "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
                "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
                "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
                "left join hr_employe employe on employe.id = contract.employe " +
                "left join atooerp_user  on atooerp_user.id = employe.user  and " +
                "atooerp_user.id = " + user_contrat.iduser + " " +
                "order by hr_day_off_request.create_date desc; ";

            List<day_off_request> list = new List<day_off_request>();

            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);


            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Color colorCheck;
                if (Convert.ToBoolean(reader["validate"]))
                {
                    colorCheck = Colors.Green;
                }
                else
                {
                    colorCheck = Colors.Red;
                }
                bool valide;
                if (Convert.ToString(reader["validated_day_off"]) == "") { valide = false; } else { valide = true; }

                day_off_request attribute = new day_off_request(Convert.ToInt32(reader["id"]),
                      reader["description"].ToString(),
                      Convert.ToDateTime(reader["start_date"]),
                      Convert.ToDateTime(reader["end_date"]),
                      Convert.ToDateTime(reader["create_date"]),
                      Convert.ToBoolean(reader["validated"]),
                      colorCheck,
                      Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                      Convert.ToString(reader["name_type"]),
                      Convert.ToString(reader["state_name"]),
                   Convert.ToInt32(reader["contract"]), valide);

                if (Convert.ToString(reader["validated_day_off"]) != "")
                {
                    attribute.start_date_finale = Convert.ToDateTime(reader["start_date_final"]);
                    attribute.end_date_finale = Convert.ToDateTime(reader["end_date_final"]);

                }


                list.Add(attribute);






            }
            reader.Close();
            DbConnection.Deconnecter();

            ObservableRangeCollection<day_off_request> list2 = new ObservableRangeCollection<day_off_request>(list);
            List<day_off_request> Result = new List<day_off_request>();
            Result = list2.Where(s => s.nom_employe.StartsWith(name)).ToList();

            foreach (day_off_request item in Result)
            {
                Console.WriteLine(item.description);
            }

            return Result;



        }


        public async static Task<List<day_off_request>> getDataByDateAndUser(string start_date, string end_date)
        {
            string sqlCmd = "select hr_day_off_request.*, day_off.validated as validated_day_off, day_off.create_date as create_date_day_off " +
               ",day_off.start_date start_date_final,day_off.end_date end_date_final, state.name as state_name, hr_day_off_type.name as name_type" +
               ",atooerp_user.last_name as last_name,atooerp_user.first_name as first_name from hr_day_off_request " +
               "left join hr_day_off day_off on day_off.day_off_request = hr_day_off_request.id " +
               "left join hr_day_off_state state on state.id = hr_day_off_request.state " +
               "left join hr_day_off_type  on hr_day_off_type.id = hr_day_off_request.day_off_type " +
               "left join hr_contract contract on contract.id = hr_day_off_request.contract " +
               "left join hr_employe employe on employe.id = contract.employe " +
               "left join atooerp_user  on atooerp_user.id = employe.user " +
               "where hr_day_off_request.create_date between '" + start_date + "' and '" + end_date + "'  and " +
                "atooerp_user.id = " + user_contrat.iduser + " " +
               "order by hr_day_off_request.create_date desc; ";


            Console.WriteLine(sqlCmd);

            List<day_off_request> list = new List<day_off_request>();


            MySqlDataReader reader = null;

            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Color colorCheck;
                        if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            colorCheck = Colors.Gray;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            colorCheck = Colors.Green;
                        }
                        else
                        {
                            colorCheck = Colors.Red;
                        }
                        bool valide;
                        if (Convert.ToString(reader["validated_day_off"]) == "") { valide = false; } else { valide = true; }

                        day_off_request attribute = new day_off_request(Convert.ToInt32(reader["id"]),
                              reader["description"].ToString(),
                              Convert.ToDateTime(reader["start_date"]),
                              Convert.ToDateTime(reader["end_date"]),
                              Convert.ToDateTime(reader["create_date"]),
                              Convert.ToBoolean(reader["validated"]),
                              colorCheck,
                              Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                              Convert.ToString(reader["name_type"]),
                              Convert.ToString(reader["state_name"]),
                           Convert.ToInt32(reader["contract"]), valide);
                        if (Convert.ToString(reader["validated_day_off"]) != "")
                        {
                            attribute.start_date_finale = Convert.ToDateTime(reader["start_date_final"]);
                            attribute.end_date_finale = Convert.ToDateTime(reader["end_date_final"]);
                        }
                        list.Add(attribute);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;
                }
            }
            else
            {
                return null;
            }
            return list;
        }






        /************************************************************************/









        #endregion

        #region InsertMethode
        public async static Task<int?> InsertCongé(string description, DateTime start_date, TimeSpan start_time, DateTime end_date, TimeSpan end_time, int contract, int validate, int type, int state)
        {
            //differenceInDays.ToString().Replace(',', '.')

            DateTime START = start_date + start_time;
            DateTime END = end_date + end_time;
            TimeSpan difference = END - START;

            // Convertir la différence en une variable de type decimal
            decimal differenceInDays = (decimal)difference.TotalDays;
            string sqlCmd = "INSERT  INTO hr_day_off_request (description,start_date,end_date,create_date,date,validated,day_off_type,state,contract,number) VALUES (" +
                "'" + description + "' , '" + String.Format("{0:yyyy-M-d}", start_date) + " " + start_time.ToString() + "' , '" + String.Format("{0:yyyy-M-d}", end_date) + " " + end_time.ToString() + "' , '" +
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + validate + " , " + type + " , " + state + " , " + contract + ","+ differenceInDays.ToString().Replace(',', '.') + "); ";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            if (await DbConnection.Connecter3())
            {
                try
                {
                    cmd.ExecuteScalar();

                }
                catch (Exception ex)
                {
                    return null;


                }

            }
            else
            {
                return null;
            }
            return 1;



        }

        #endregion

        #region delete
        public static void delete(int id)
        {

            string sqlCmd = "delete from hr_day_off_request where id = " + id + " ;";




            DbConnection.Connecter();
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            try
            {
                cmd.ExecuteScalar();



            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Done", ex.Message, "Ok");

            }




            DbConnection.Deconnecter();

        }

        #endregion

        #region update

        public static void update(int id, string description, DateTime start_date, TimeSpan start_time, DateTime end_date, TimeSpan end_time, int type)
        {
            string sqlCmd = "UPDATE hr_day_off_request SET description = '" + description + "' , start_date = '" + String.Format("{0:yyyy-M-d}", start_date) + " " + start_time.ToString()
                + "' , end_date = '" + String.Format("{0:yyyy-M-d}", end_date) + " " + end_time.ToString() + "' , day_off_type = " + type + " WHERE id = " + id + " ;";
            Console.WriteLine(sqlCmd);
            DbConnection.Deconnecter();
            DbConnection.Connecter();
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);



            try
            {
                cmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");

            }




            DbConnection.Deconnecter();

        }

        public async static Task updateState(int state, int id1)
        {
            string sqlCmd = "UPDATE hr_day_off_request SET state = " + state + " WHERE id = " + id1 + " ;";
            Console.WriteLine(sqlCmd);

            DbConnection.Deconnecter();
            DbConnection.Connecter();
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            try
            {
                cmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");

            }




            DbConnection.Deconnecter();





        }
        public async static Task updateDecision(int id, string description, DateTime decision_date)
        {
            string sqlCmd = "UPDATE hr_day_off_request SET description_decision = '" + description + "' , decision_date = '" + String.Format("{0:yyyy-M-d HH:mm:ss}", decision_date) + "' WHERE id = " + id + " ;";
            Console.WriteLine(sqlCmd);
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            try
            {
                cmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("echec", ex.Message, "Ok");

            }




            DbConnection.Deconnecter();

        }


        #endregion














    }

}

