

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.View;
using MvvmHelpers.Commands;
using MySqlConnector;
Après :
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.View;
*/
using Acr.UserDialogs;
using MySqlConnector;
using SmartPharma5.View;
//using SmartPharma5.view;
using Command = MvvmHelpers.Commands.Command;
//using Acr.UserDialogs;

namespace SmartPharma5.Model
{
    public class avance_request
    {
        #region attribut
        public int id { get; set; }
        public string description { get; set; }

        public DateTime create_date { get; set; }

        public string state_description { get; set; }

        public DateTime date_decision { get; set; }

        public bool validated { get; set; }

        public Color color { get; set; }

        public int state { get; set; }

        public decimal amount { get; set; }

        public string nom_employe { get; set; }

        public string type_avance { get; set; }

        public string state_name { get; set; }

        public int contract { get; set; }

        public bool decision_validate { get; set; }

        public bool decision_notvalidated { get; set; }

        public bool deposit_created { get; set; }

        public decimal? deposit_amount { get; set; }
        public Command getDétail { get; }

        public Command getDécision { get; }

        #endregion
        public avance_request() { }
        public avance_request(int id, string description, DateTime create_date, bool validated, int state, string nom_employe, string type_avance, string state_name, int contract, decimal amount)
        {
            this.id = id;
            this.description = description;
            this.create_date = create_date;


            this.validated = validated;
            this.state = state;
            this.nom_employe = nom_employe;
            this.type_avance = type_avance;
            this.state_name = state_name;
            this.contract = contract;
            this.amount = amount;

            if (state_name == "EN ATTENTE")
            {
                this.decision_notvalidated = true;
                this.decision_validate = false;
                color = Colors.Gray;
            }
            else
            {
                this.decision_notvalidated = false;
                this.decision_validate = true;
                if (state_name == "REFUSE")
                {
                    color = Colors.Red;
                }
                if (state_name == "ACCEPTE")
                {
                    color = Colors.Green;
                }

            }

            getDétail = new Command(async () =>
            {
                if (await DbConnection.Connecter3())
                {


                    UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                    await Task.Delay(500);
                    await App.Current.MainPage.Navigation.PushAsync(new AvanceRequestDétails(this.id));
                    UserDialogs.Instance.HideLoading();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Error Connection", "OK");

                }




            });

            getDécision = new Command(async (flag) =>
            {
                if (await DbConnection.Connecter3())
                {
                    if (state_name == "ACCEPTE")
                    {
                        if (avance.getCheckDepositExist(id) == 0)
                        {
                            await App.Current.MainPage.DisplayAlert("INFO", "DEPOSIT NOT CREATED YET", "OK");

                        }
                        else
                        {
                            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                            await Task.Delay(500);
                            await App.Current.MainPage.Navigation.PushAsync(new décision_avance_request(id, true));
                             UserDialogs.Instance.HideLoading();


                        }


                    }
                    else
                    {
                         UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                        await Task.Delay(500);
                         await  App.Current.MainPage.Navigation.PushAsync(new décision_avance_request(id, false));
                         UserDialogs.Instance.HideLoading();
                    }

                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Error Connection", "OK");

                }



            });

        }





        //GetAllDepositRequest
        public async static Task<List<avance_request>> GetRequestAvance()
        {
            string sqlCmd = "SELECT hr_deposit_request.* , hr_deposit_type.name as name_type , state.name as state_name ,atooerp_person.first_name as first_name , atooerp_person.last_name as last_name\r\n, deposit.id as deposit , deposit.amount as deposit_amount\r\nfrom hr_deposit_request\r\nleft join hr_deposit_state state  on state.id = hr_deposit_request.state\r\nleft join hr_deposit_type  on hr_deposit_type.id = hr_deposit_request.deposit_type\r\nleft join hr_contract contract on contract.id = hr_deposit_request.contract\r\nleft join hr_employe employe on employe.id = contract.employe\r\nleft join atooerp_user  on atooerp_user.id = employe.user\r\nleft join atooerp_person  on atooerp_person.id = employe.Id\r\nleft join hr_deposit deposit  on deposit.deposit_request = hr_deposit_request.id\r\norder by  hr_deposit_request.create_date desc ;";
            Console.WriteLine(sqlCmd);

            List<avance_request> list = new List<avance_request>();
            MySqlDataReader reader = null;

            //DbConnection.Deconnecter();

            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        avance_request avance_att = new avance_request(Convert.ToInt32(reader["id"]),
                                     reader["descrption"].ToString(),
                                     Convert.ToDateTime(reader["create_date"]),
                                     Convert.ToBoolean(reader["validated"]),
                                     Convert.ToInt32(reader["state"]),
                                     Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                                     Convert.ToString(reader["name_type"]),
                                     Convert.ToString(reader["state_name"]),
                                     Convert.ToInt32(reader["contract"]),
                                     Convert.ToDecimal(reader["amount"])
                                     );
                        if (Convert.ToString(reader["deposit"]) == "")
                        {
                            avance_att.deposit_created = false;
                        }
                        else
                        {
                            avance_att.deposit_created = true;
                            avance_att.deposit_amount = Convert.ToDecimal(reader["deposit_amount"]);
                        }
                        list.Add(avance_att);
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


        //GetDepositRequestById
        public async static Task<avance_request> GetRequestDepositById(int id)
        {

            string sqlCmd1 = "SELECT hr_deposit_request.* , hr_deposit_type.name as name_type , state.name as state_name ,atooerp_user.first_name as first_name ," +
               "atooerp_user.last_name as last_name , deposit.id as deposit , deposit.amount as deposit_amount from hr_deposit_request " +
               "left join hr_deposit_state state  on state.id = hr_deposit_request.state " +
               "left join hr_deposit_type  on hr_deposit_type.id = hr_deposit_request.deposit_type " +
               "left join hr_contract contract on contract.id = hr_deposit_request.contract " +
               "left join hr_employe employe on employe.id = contract.employe " +
               "left join atooerp_user  on atooerp_user.id = employe.user " +
               "left join hr_deposit deposit  on  deposit.deposit_request = hr_deposit_request.id  " +
               " where hr_deposit_request.id = " + id + "; ";
            MySqlDataReader reader = null;

            //DbConnection.Deconnecter();

            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    avance_request day = new avance_request(Convert.ToInt32(reader["id"]),
                               reader["descrption"].ToString(),
                               Convert.ToDateTime(reader["create_date"]),
                               Convert.ToBoolean(reader["validated"]),
                               Convert.ToInt32(reader["state"]),
                               Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                               Convert.ToString(reader["name_type"]),
                               Convert.ToString(reader["state_name"]),
                               Convert.ToInt32(reader["contract"]),
                            Convert.ToDecimal(reader["amount"])
                               );
                    if (Convert.ToString((reader["deposit"])) == "")
                    {
                        day.deposit_created = false;
                    }
                    else
                    {
                        day.deposit_created = true;
                        day.deposit_amount = Convert.ToDecimal(reader["deposit_amount"]);
                    }



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


        //GetLastDepositRequest
        public async static Task<avance_request> getLastDepositRequest()
        {
            string sqlCmd1 = "SELECT hr_deposit_request.* , hr_deposit_type.name as name_type , state.name as state_name ,atooerp_user.first_name as first_name ," +
                "atooerp_user.last_name as last_name , deposit.id as deposit from hr_deposit_request " +
                "left join hr_deposit deposit  on hr_deposit_request.id = deposit.deposit_request " +
                "left join hr_deposit_state state  on state.id = hr_deposit_request.state " +
                "left join hr_deposit_type  on hr_deposit_type.id = hr_deposit_request.deposit_type " +
                "left join hr_contract contract on contract.id = hr_deposit_request.contract " +
                "left join hr_employe employe on employe.id = contract.employe " +
                "left join atooerp_user  on atooerp_user.id = employe.user " +
                " where atooerp_user.id =" + user_contrat.iduser +
                " order by  hr_deposit_request.create_date desc; ";
            List<avance_request> avance = new List<avance_request>();
            // DbConnection.Deconnecter();

            MySqlDataReader reader = null;

            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        avance_request avance_att = new avance_request(Convert.ToInt32(reader["id"]),
                                  reader["descrption"].ToString(),
                                  Convert.ToDateTime(reader["create_date"]),
                                  Convert.ToBoolean(reader["validated"]),
                                  Convert.ToInt32(reader["state"]),
                                  Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                                  Convert.ToString(reader["name_type"]),
                                  Convert.ToString(reader["state_name"]),
                                  Convert.ToInt32(reader["contract"]),
                               Convert.ToDecimal(reader["amount"])
                                  );
                        if (Convert.ToString((reader["deposit"])) == "")
                        {
                            avance_att.deposit_created = false;
                        }
                        else
                        {
                            avance_att.deposit_created = true;
                           // avance_att.deposit_amount = Convert.ToDecimal(reader["deposit_amount"]);

                        }
                        avance.Add(avance_att);
                        //reader.Close();
                    }

                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;

                }

            }
            else
            {
                reader.Close();
                return null;


                /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                Avant :
                            }








                            return avance[0];
                Après :
                            }








                            return avance[0];
                */
            }








            return avance[0];


        }



        //InsertRequestDeposit
        public async static Task<int?> InsertAvanceRequest(string description, int contract, int validate, int type, int state, decimal amount)
        {
            int? result;
            Console.WriteLine("gtgtgtgtgt");
            string sqlCmd = "INSERT  INTO hr_deposit_request (descrption,create_date,date,validated,deposit_type,state,contract,amount) VALUES (" +
                "'" + description + "' , '" +
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + 0 + " , " + type + " , " + state + " , " + contract + " , " + amount + ");select max(id) from hr_deposit_request";


            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            if (await DbConnection.Connecter3())
            {
                try
                {
                    //cmd.ExecuteScalar();
                    result = int.Parse(cmd.ExecuteScalar().ToString());

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
            return result;










        }





        public async static Task<List<avance_request>> getDepositRequestByUserId()
        {
            string sqlCmd1 = "SELECT hr_deposit_request.* , hr_deposit_type.name as name_type , state.name as state_name ,atooerp_user.first_name as first_name ," +
                "atooerp_user.last_name as last_name , deposit.id as deposit ,deposit.amount as deposit_amount from hr_deposit_request " +
                "left join hr_deposit deposit  on hr_deposit_request.id = deposit.deposit_request " +
                "left join hr_deposit_state state  on state.id = hr_deposit_request.state " +
                "left join hr_deposit_type  on hr_deposit_type.id = hr_deposit_request.deposit_type " +
                "left join hr_contract contract on contract.id = hr_deposit_request.contract " +
                "left join hr_employe employe on employe.id = contract.employe " +
                "left join atooerp_user  on atooerp_user.id = employe.user " +
                "where atooerp_user.id =" + user_contrat.iduser + " " +
                " order by  hr_deposit_request.create_date desc; ";

            Console.WriteLine(sqlCmd1);
            List<avance_request> avance = new List<avance_request>();
            //DbConnection.Deconnecter();
            MySqlDataReader reader = null;


            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        avance_request avance_att = new avance_request(Convert.ToInt32(reader["id"]),
                                   reader["descrption"].ToString(),
                                   Convert.ToDateTime(reader["create_date"]),
                                   Convert.ToBoolean(reader["validated"]),
                                   Convert.ToInt32(reader["state"]),
                                   Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                                   Convert.ToString(reader["name_type"]),
                                   Convert.ToString(reader["state_name"]),
                                   Convert.ToInt32(reader["contract"]),
                                Convert.ToDecimal(reader["amount"])
                                   );
                        if (Convert.ToString((reader["deposit"])) == "")
                        {
                            avance_att.deposit_created = false;
                        }
                        else
                        {
                            avance_att.deposit_created = true;
                            avance_att.deposit_amount = Convert.ToDecimal(reader["deposit_amount"]);


                        }
                        avance.Add(avance_att);
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



            //DbConnection.Deconnecter();



            return avance;


        }



        //GetAllRequestsByState
        public async static Task<List<avance_request>> getDepositRequestByState(int state)
        {
            string sqlCmd1 = "SELECT hr_deposit_request.* , hr_deposit_type.name as name_type , state.name as state_name ,atooerp_user.first_name as first_name ," +
               "atooerp_user.last_name as last_name , deposit.id as deposit , deposit.amount as deposit_amount from hr_deposit_request " +
               "left join hr_deposit_state state  on state.id = hr_deposit_request.state " +
               "left join hr_deposit_type  on hr_deposit_type.id = hr_deposit_request.deposit_type " +
               "left join hr_contract contract on contract.id = hr_deposit_request.contract " +
               "left join hr_employe employe on employe.id = contract.employe " +
               "left join atooerp_user  on atooerp_user.id = employe.user " +
               "left join hr_deposit deposit  on deposit.deposit_request = hr_deposit_request.id " +
               "where   hr_deposit_request.state = " + state + "  " + " " +
               " order by  hr_deposit_request.create_date desc; ";
            Console.WriteLine(sqlCmd1);
            List<avance_request> avance = new List<avance_request>();

            MySqlDataReader reader = null;

            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        avance_request avance_att = new avance_request(Convert.ToInt32(reader["id"]),
                                  reader["descrption"].ToString(),
                                  Convert.ToDateTime(reader["create_date"]),
                                  Convert.ToBoolean(reader["validated"]),
                                  Convert.ToInt32(reader["state"]),
                                  Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                                  Convert.ToString(reader["name_type"]),
                                  Convert.ToString(reader["state_name"]),
                                  Convert.ToInt32(reader["contract"]),
                               Convert.ToDecimal(reader["amount"])
                                  );
                        if (Convert.ToString((reader["deposit"])) == "")
                        {
                            avance_att.deposit_created = false;
                        }
                        else
                        {
                            avance_att.deposit_created = true;
                            avance_att.deposit_amount = Convert.ToDecimal(reader["deposit_amount"]);

                        }
                        avance.Add(avance_att);
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






            return avance;


        }


        //GetUserRequestByState
        public async static Task<List<avance_request>> getDepositRequestByStateAndUser(int state)
        {


            string sqlCmd1 = "SELECT hr_deposit_request.* , hr_deposit_type.name as name_type , state.name as state_name ,atooerp_user.first_name as first_name ," +
               "atooerp_user.last_name as last_name , deposit.id as deposit ,deposit.amount as deposit_amount from hr_deposit_request " +
               "left join hr_deposit_state state  on state.id = hr_deposit_request.state " +
               "left join hr_deposit_type  on hr_deposit_type.id = hr_deposit_request.deposit_type " +
               "left join hr_contract contract on contract.id = hr_deposit_request.contract " +
               "left join hr_employe employe on employe.id = contract.employe " +
               "left join atooerp_user  on atooerp_user.id = employe.user " +
               "left join hr_deposit deposit  on deposit.deposit_request = hr_deposit_request.id  " +
               "where   atooerp_user.id = " + user_contrat.iduser + " and hr_deposit_request.state = " + state +
               " order by  hr_deposit_request.create_date desc; ";
            Console.WriteLine(sqlCmd1);
            List<avance_request> avance = new List<avance_request>();
            MySqlDataReader reader = null;

            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        avance_request avance_att = new avance_request(Convert.ToInt32(reader["id"]),
                                   reader["descrption"].ToString(),
                                   Convert.ToDateTime(reader["create_date"]),
                                   Convert.ToBoolean(reader["validated"]),
                                   Convert.ToInt32(reader["state"]),
                                   Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                                   Convert.ToString(reader["name_type"]),
                                   Convert.ToString(reader["state_name"]),
                                   Convert.ToInt32(reader["contract"]),
                                Convert.ToDecimal(reader["amount"])
                                   );
                        if (Convert.ToString((reader["deposit"])) == "")
                        {
                            avance_att.deposit_created = false;
                        }
                        else
                        {
                            avance_att.deposit_created = true;
                            avance_att.deposit_amount = Convert.ToDecimal(reader["deposit_amount"]);

                        }
                        avance.Add(avance_att);
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






            return avance;


        }



        public async static Task<List<avance_request>> getDepositRequestByCreateDateAndUser(string start_date, string end_date)
        {
            string sqlCmd1 = "SELECT hr_deposit_request.* , hr_deposit_type.name as name_type , state.name as state_name ,atooerp_user.first_name as first_name ," +
                "atooerp_user.last_name as last_name , deposit.id as deposit ,deposit.amount as deposit_amount from hr_deposit_request " +
                "left join hr_deposit_state state  on state.id = hr_deposit_request.state " +
                "left join hr_deposit_type  on hr_deposit_type.id = hr_deposit_request.deposit_type " +
                "left join hr_contract contract on contract.id = hr_deposit_request.contract " +
                "left join hr_employe employe on employe.id = contract.employe " +
                "left join atooerp_user  on atooerp_user.id = employe.user " +
                "left join hr_deposit deposit  on deposit.deposit_request = hr_deposit_request.id  " +
                "where atooerp_user.id =" + user_contrat.iduser + " and " + " hr_deposit_request.create_date between '" + start_date + "' and '" + end_date + "'  " +
                " order by  hr_deposit_request.create_date desc ; ";

            Console.WriteLine(sqlCmd1);
            List<avance_request> avance = new List<avance_request>();
            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);
                    reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        avance_request avance_att = new avance_request(Convert.ToInt32(reader["id"]),
                                    reader["descrption"].ToString(),
                                    Convert.ToDateTime(reader["create_date"]),
                                    Convert.ToBoolean(reader["validated"]),
                                    Convert.ToInt32(reader["state"]),
                                    Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                                    Convert.ToString(reader["name_type"]),
                                    Convert.ToString(reader["state_name"]),
                                    Convert.ToInt32(reader["contract"]),
                                 Convert.ToDecimal(reader["amount"])
                                    );
                        if (Convert.ToString((reader["deposit"])) == "")
                        {
                            avance_att.deposit_created = false;
                        }
                        else
                        {
                            avance_att.deposit_created = true;
                            avance_att.deposit_amount = Convert.ToDecimal(reader["deposit_amount"]);

                        }
                        avance.Add(avance_att);
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





            return avance;


        }

        public async static Task<List<avance_request>> getDepositRequestByCreateDate(string start_date, string end_date)
        {
            string sqlCmd1 = "SELECT hr_deposit_request.* , hr_deposit_type.name as name_type , state.name as state_name ,atooerp_user.first_name as first_name ," +
                "atooerp_user.last_name as last_name , deposit.id as deposit ,deposit.amount as deposit_amount from hr_deposit_request " +
                "left join hr_deposit_state state  on state.id = hr_deposit_request.state " +
                "left join hr_deposit_type  on hr_deposit_type.id = hr_deposit_request.deposit_type " +
                "left join hr_contract contract on contract.id = hr_deposit_request.contract " +
                "left join hr_employe employe on employe.id = contract.employe " +
                "left join atooerp_user  on atooerp_user.id = employe.user " +
                "left join hr_deposit deposit  on deposit.deposit_request = hr_deposit_request.id  " +
                "where  hr_deposit_request.create_date between '" + start_date + "' and '" + end_date + "'  " +
                " order by  hr_deposit_request.create_date desc ; ";


            List<avance_request> avance = new List<avance_request>();
            MySqlDataReader reader = null;

            if (await DbConnection.Connecter3())
            {
                try
                {

                    MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        avance_request avance_att = new avance_request(Convert.ToInt32(reader["id"]),
                                  reader["descrption"].ToString(),
                                  Convert.ToDateTime(reader["create_date"]),
                                  Convert.ToBoolean(reader["validated"]),
                                  Convert.ToInt32(reader["state"]),
                                  Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                                  Convert.ToString(reader["name_type"]),
                                  Convert.ToString(reader["state_name"]),
                                  Convert.ToInt32(reader["contract"]),
                               Convert.ToDecimal(reader["amount"])
                                  );
                        if (Convert.ToString((reader["deposit"])) == "")
                        {
                            avance_att.deposit_created = false;
                        }
                        else
                        {
                            avance_att.deposit_created = true;
                            avance_att.deposit_amount = Convert.ToDecimal(reader["deposit_amount"]);

                        }
                        avance.Add(avance_att);




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




            return avance;


        }


        public static List<avance_request> getByStartName()
        {
            string sqlCmd1 = "SELECT hr_deposit_request.* , hr_deposit_type.name as name_type  , hr_deposit_state.name as state_name ,atooerp_user.first_name as first_name ,atooerp_user.last_name as last_name ,deposit.amount as deposit_amount FROM atooerp_user ,hr_contract " +
                ", hr_employe ,hr_deposit_state , hr_deposit_type ,hr_deposit_request " +
                "left join hr_deposit deposit  on deposit.deposit_request = hr_deposit_request.id  " +
                "where hr_deposit_request.contract = hr_contract.id " +
                "and hr_contract.employe = hr_employe.id and hr_deposit_request.deposit_type = hr_deposit_type.id and hr_deposit_request.state = hr_deposit_state.id  " +
                "and  hr_employe.user=atooerp_user.id and atooerp_user.id = " + user_contrat.iduser + " order by  hr_deposit_request.create_date desc; ";

            Console.WriteLine(sqlCmd1);
            List<avance_request> avance = new List<avance_request>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();
            MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                avance.Add(new avance_request(Convert.ToInt32(reader["id"]),
                          reader["descrption"].ToString(),
                          Convert.ToDateTime(reader["create_date"]),
                          Convert.ToBoolean(reader["validated"]),
                          Convert.ToInt32(reader["state"]),
                          Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                          Convert.ToString(reader["name_type"]),
                          Convert.ToString(reader["state_name"]),
                          Convert.ToInt32(reader["contract"]),
                       Convert.ToDecimal(reader["amount"])
                          ));
            }


            reader.Close();
            DbConnection.Deconnecter();
            Console.WriteLine("______________gggg_");
            Console.WriteLine(user_contrat.iduser);
            Console.WriteLine("______________gggg_");


            return avance;


        }


        /********************************************************************************/


        public async static Task update(int id, decimal amount, string description, int type)
        {
            string _Amount = amount.ToString().Replace(',', '.');
            string sqlCmd = "update hr_deposit_request set amount = " + _Amount + " , deposit_type = " + type + " , descrption = '" + description + "'  where id = " + id + " ;";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
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

        public async static Task delete(int id)
        {
            string sqlCmd = "delete from hr_deposit_request where id = " + id + " ;";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
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




    }
}
