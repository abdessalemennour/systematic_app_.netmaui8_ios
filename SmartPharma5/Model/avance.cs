using MySqlConnector;

namespace SmartPharma5.Model
{
    public class avance
    {

        #region attribut
        public int id { get; set; }

        /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
        Avant :
                public string description { get; set; }

                public decimal amount { get; set; }
        Après :
                public string description { get; set; }

                public decimal amount { get; set; }
        */
        public string description { get; set; }

        public decimal amount { get; set; }
        public DateTime create_date { get; set; }
        public bool validate { get; set; }
        public string decision_description { get; set; }
        public DateTime date_decision { get; set; }



        public string nom_employe { get; set; }

        public string type_avance { get; set; }









        #endregion


        public avance() { }
        public avance(int id, string description, decimal amount, DateTime create_dateAtt, bool validate, string nom_employe, string type_avance)
        {
            this.id = id;
            this.description = description;
            this.amount = amount;

            this.create_date = create_dateAtt;
            this.validate = validate;
            this.nom_employe = nom_employe;
            this.type_avance = type_avance;



        }
        public static avance GetDayOffByRequestId(int id)
        {
            string sqlCmd1 = "SELECT hr_deposit.* , hr_deposit_type.name as name_type ,atooerp_user.first_name as first_name ,atooerp_user.last_name as last_name  FROM atooerp_user ,hr_deposit ,hr_contract , hr_employe  , hr_deposit_type " +
                " where  hr_deposit.contract = hr_contract.id and hr_contract.employe= hr_employe.id and hr_deposit.deposit_type=hr_deposit_type.id and " +
                "  hr_employe.user=atooerp_user.id and hr_deposit.deposit_request=" + id + " ;";
            Console.WriteLine(sqlCmd1);
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);


            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();




            avance day = new avance(Convert.ToInt32(reader["id"]),
                          reader["description"].ToString(),
                          Convert.ToDecimal(reader["amount"]),

                          Convert.ToDateTime(reader["create_date"]),
                          Convert.ToBoolean(reader["validated"]),
                          Convert.ToString(reader["first_name"] + "  " + reader["last_name"]),
                          Convert.ToString(reader["name_type"])



                          );

            reader.Close();
            DbConnection.Deconnecter();


            return day;


        }
        public static int getCheckDepositExist(int id)
        {
            string sqlCmd1 = "SELECT hr_deposit.id , count(hr_deposit.id) as number from hr_deposit where hr_deposit.deposit_request = " + id + " ;";

            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);


            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            int result;
            result = Convert.ToInt32(reader["number"]);
            reader.Close();
            DbConnection.Deconnecter();


            return result;



        }

    }
}
