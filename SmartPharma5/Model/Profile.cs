//using Acr.UserDialogs;
using MySqlConnector;

namespace SmartPharma5.Model
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Profile()
        {

        }
        public Profile(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

        public static async Task<List<Profile>> getAllProfile()
        {
            string sqlCmd = "SELECT Id,name from marketing_profile where type = 1 ;";
            //string sqlCmd = "SELECT Id,name from marketing_profile ;";
            List<Profile> profiles = new List<Profile>();

            if (await DbConnection.Connecter3())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {
                        Profile profile = new Profile();
                        profile = new Profile(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name"]));
                        profiles.Add(profile);
                    }
                    catch (Exception ex)
                    {
                        reader.Close();
                        Console.WriteLine(ex.Message);
                    }
                }
                reader.Close();
                DbConnection.Deconnecter();
            }
            return profiles;
        }
        public static async Task<int> InsertNewInstanceProfileTemp(int profile_id, int id_partner)
        {
            int p = 0;
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "insert into marketing_profile_instance_temp(profile,partner,state,create_date,user,employe) values(" + profile_id + "," + id_partner + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + user_contrat.iduser + "," + user_contrat.id_employe + ");select max(id) from marketing_profile_instance_temp ;";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    p = int.Parse(cmd.ExecuteScalar().ToString());
                    // UserDialogs.Instance.Alert("CHANGE PROFILE ATTRIBUTES");



                }
                catch (Exception ex)
                {


                }
            }
            else
            {

            }
            return p;
        }
    }
}
