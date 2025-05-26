using MySqlConnector;


namespace SmartPharma5.Model
{
    public class day_off_type
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public int minus { get; set; }

        public day_off_type(int Id, string Name, int minus)
        {
            this.Id = Id;
            this.Description = Name;
            this.minus = minus;
        }
        public day_off_type() { }

        public static day_off_type getTypeById(string name)
        {
            string sqlCmd = "SELECT * FROM hr_day_off_type where name = '" + name + "' ;";
            Console.WriteLine(sqlCmd);


            DbConnection.Connecter();


            /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
            Avant :
                        MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

                        MySqlDataReader reader = cmd.ExecuteReader();
            Après :
                        MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

                        MySqlDataReader reader = cmd.ExecuteReader();
            */
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            MySqlDataReader reader = cmd.ExecuteReader();



            reader.Read();

            Console.WriteLine(reader["id"]);
            Console.WriteLine(reader["name"]);
            Console.WriteLine(reader["minus"]);


            day_off_type day = new day_off_type(Convert.ToInt32(reader["id"]), reader["name"].ToString(), Convert.ToInt32(reader["minus"]));


            reader.Close();
            DbConnection.Deconnecter();
            return day;






        }

        public static List<day_off_type> GetAllDay_off_type()
        {
            string sqlCmd = "SELECT * FROM hr_day_off_type;";
            Console.WriteLine(sqlCmd);

            List<day_off_type> list = new List<day_off_type>();

            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);


            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {




                list.Add(new day_off_type(Convert.ToInt32(reader["id"]),
                    reader["name"].ToString(), Convert.ToInt32(reader["minus"])));







            }
            reader.Close();
            DbConnection.Deconnecter();


            Console.WriteLine("**************************************");
            return list;
        }
    }
}
