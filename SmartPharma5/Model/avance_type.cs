using MySqlConnector;
//using SmartPharma5.view;


namespace SmartPharma5.Model
{
    public class avance_type
    {
        public int Id { get; set; }
        public string Description { get; set; }



        public avance_type(int Id, string Name)
        {
            this.Id = Id;
            this.Description = Name;

        }
        public avance_type() { }

        public static avance_type getTypeById(string name)
        {
            string sqlCmd = "SELECT * FROM hr_deposit_type where name = '" + name + "' ;";
            Console.WriteLine(sqlCmd);


            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            MySqlDataReader reader = cmd.ExecuteReader();



            reader.Read();

            Console.WriteLine(reader["id"]);
            Console.WriteLine(reader["name"]);



            avance_type day = new avance_type(Convert.ToInt32(reader["id"]), reader["name"].ToString());


            reader.Close();
            DbConnection.Deconnecter();
            return day;






        }

        public static List<avance_type> GetAllDeposit_type()
        {
            string sqlCmd = "SELECT * FROM hr_deposit_type;";
            Console.WriteLine(sqlCmd);

            List<avance_type> list = new List<avance_type>();


            DbConnection.Deconnecter();
            DbConnection.Connecter();


            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);


            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {


                list.Add(new avance_type(Convert.ToInt32(reader["id"]),
                    reader["name"].ToString()));







            }
            reader.Close();
            DbConnection.Deconnecter();



            return list;
        }
    }
}
