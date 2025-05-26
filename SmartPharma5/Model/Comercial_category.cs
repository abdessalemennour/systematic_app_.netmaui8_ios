using MySqlConnector;

namespace SmartPharma5.Model
{
    class Comercial_category
    {
        public int id { get; set; }
        public string Name { get; set; }

        public Comercial_category()
        {

        }
        public Comercial_category(int id, string name)
        {
            this.id = id;
            Name = name;
        }

        public async static Task<List<Comercial_category>> GetCommercialCategory()
        {
            string sqlCmd = "SELECT Id,name FROM commercial_partner_category ";
            List<Comercial_category> category = new List<Comercial_category>();

            if (await DbConnection.Connecter3())
            {

                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {

                        category.Add(new Comercial_category(
                            Convert.ToInt32(reader["Id"]), reader["name"].ToString()));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                reader.Close();

            }
            else
            {
                return null;
            }





            return category;

        }
    }
}
