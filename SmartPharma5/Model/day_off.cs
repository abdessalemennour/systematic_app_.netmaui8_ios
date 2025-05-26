using MySqlConnector;


namespace SmartPharma5.Model
{
    public class day_off
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

        public DateTime request_date { get; set; }



        public string nom_employe { get; set; }

        public string type_congé { get; set; }









        #endregion
        public day_off() { }
        public day_off(int id, string description, DateTime start_date, DateTime end_date, DateTime create_dateAtt, bool validate, string nom_employe, string type_congé, DateTime request_date)
        {
            this.id = id;
            this.description = description;
            this.start_date = start_date;
            this.end_date = end_date;
            this.create_date = create_dateAtt;
            this.validate = validate;
            this.nom_employe = nom_employe;
            this.type_congé = type_congé;
            this.request_date = request_date;



        }

        public static bool checkDayOffCreated(int deposit_request)
        {
            string sqlCmd = "select count(hr_day_off.id) as number from hr_day_off where hr_day_off.day_off_request = " + deposit_request + ";";
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            List<deposit_line> list_deposit = new List<deposit_line>();

            MySqlDataReader reader = cmd.ExecuteReader();

            reader.Read();
            bool result;
            if (Convert.ToInt32(reader["number"]) == 0)
            {

                result = false;

            }

            /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
            Avant :
                        else {


                            result = true; }
            Après :
                        else {


                            result = true; }
            */
            else
            {


                result = true;
            }

            reader.Close();
            DbConnection.Deconnecter();
            return result;



        }
        public static day_off GetDayOffByRequestId(int id)
        {
            string sqlCmd1 = "select hr_day_off.* , hr_day_off_request.create_date as request_date  , hr_day_off_type.name as name_type ,atooerp_user.first_name as first_name ,atooerp_user.last_name as last_name from hr_day_off\r\nleft join hr_day_off_request on hr_day_off.day_off_request=hr_day_off_request.Id\r\nleft join hr_day_off_type on hr_day_off.day_off_type = hr_day_off_type.Id\r\nleft join hr_contract on hr_day_off.contract = hr_contract.id\r\nleft join  hr_employe on hr_contract.employe= hr_employe.id\r\nleft join atooerp_user on hr_employe.user=atooerp_user.id\r\nwhere hr_day_off.day_off_request= " + id + ";";
            Console.WriteLine(sqlCmd1);
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);


            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();




            day_off day = new day_off(Convert.ToInt32(reader["id"]),
                          reader["description"].ToString(),
                          Convert.ToDateTime(reader["start_date"]),
                          Convert.ToDateTime(reader["end_date"]),
                          Convert.ToDateTime(reader["create_date"]),
                          Convert.ToBoolean(reader["validated"]),
                          Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                          Convert.ToString(reader["name_type"]),
                           Convert.ToDateTime(reader["request_date"])





                          );

            reader.Close();
            DbConnection.Deconnecter();


            return day;


        }

        public void InsertCongé(int id, string description, DateTime start_date, TimeSpan start_time, DateTime end_date, TimeSpan end_time, int contract, int validate, int type)
        {
            string sqlCmd = "INSERT  INTO hr_day_off (description,start_date,end_date,create_date,date,validated,day_off_type,contract,day_off_request,exercice,day_off_report) VALUES (" +
               "'" + description + "' , '" + String.Format("{0:yyyy-M-d}", start_date) + " " + start_time.ToString() + "' , '" + String.Format("{0:yyyy-M-d}", end_date) + " " + end_time.ToString() + "' , '" +
               DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + validate + " , " + type + " , " + contract + " , " + id + " , " + 0 + " , " + 0 + "); ";
            Console.WriteLine(sqlCmd);

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Deconnecter();
            DbConnection.Connecter();


            try
            {
                cmd.ExecuteScalar();
                day_off_request.updateState(2, id);



                App.Current.MainPage.DisplayAlert("Done", "Ajout avec succés", "Ok");
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Done", ex.Message, "Ok");

            }




            DbConnection.Deconnecter();
        }

        public async static Task delete(int id1)
        {
            string sqlCmd = "delete from hr_day_off where id = " + id1 + " ;";

            DbConnection.Connecter();
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);





            try
            {
                bool verif = await App.Current.MainPage.DisplayAlert("Réfuser le demande", "voulez vous réfuser le demande de congé ?", "Oui", "Non");
                if (verif)
                {
                    cmd.ExecuteScalar();


                }





            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Done888", ex.Message, "Ok");

            }

            DbConnection.Deconnecter();



        }
    }

}
