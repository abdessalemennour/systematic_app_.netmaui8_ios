//using Acr.UserDialogs;
using Acr.UserDialogs;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.View;

namespace SmartPharma5.Model
{
    public class PartnerTemp
    {
        public int Id { get; set; }
        public string Name_User { get; set; }
        public string Name_Partner_Temp { get; set; }
        public DateTime created_date { get; set; }
        public int ProfileTemp { get; set; }
        public int profile { get; set; }

        public int Id_employe { get; set; }

        //add states
        public bool Validated { get; set; }
        public bool Réfused { get; set; }
        public bool Current { get; set; }

        public AsyncCommand ShowAttributes { get; set; }
        public AsyncCommand ShowMyAttributes { get; set; }
        

        public PartnerTemp(int id, string name_User, string name_Partner_Temp, DateTime created_date, int profileTemp, int profile,int id_employe)
        {
            ShowAttributes = new AsyncCommand(showAttributes);
            ShowMyAttributes = new AsyncCommand(showMyAttributes);
            Id = id;
            Name_User = name_User;
            Name_Partner_Temp = name_Partner_Temp;
            this.created_date = created_date;
            ProfileTemp = profileTemp;
            this.profile = profile;
            Id_employe= id_employe;

        }

        public PartnerTemp()
        {
            ShowAttributes = new AsyncCommand(showAttributes);
            ShowMyAttributes = new AsyncCommand(showMyAttributes);

        }
        private async Task showAttributes()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await App.Current.MainPage.Navigation.PushAsync(new PartnerTempAttributesView(this.Id, this.profile, Id_employe));
            UserDialogs.Instance.HideLoading();


        }
        private async Task showMyAttributes()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            await App.Current.MainPage.Navigation.PushAsync(new MyPartnerTempAttributesView(this.Id, this.profile, Id_employe));
            UserDialogs.Instance.HideLoading();


        }

        public static async Task<List<PartnerTemp>> GetAllPartnerHistoryTemp()
        {
            //string sqlCmd = "SELECT c.id , c.user ,c.create_date, c.employe ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as name_user ,marketing_profile_attribut_value_temp.string_value\r\n,marketing_profile_instance_temp.id as instance_profile_temp , marketing_profile_instance_temp.profile\r\nFROM commercial_partner_temp c\r\nleft join atooerp_person on  atooerp_person.id = c.employe\r\nleft join  marketing_profile_attribut_value_temp on marketing_profile_attribut_value_temp.partner_temp=c.id and marketing_profile_attribut_value_temp.attribut_name=\"name\"\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=c.id\r\nWHERE c.partner is null and c.state=1 order by c.create_date desc;";
            string sqlCmd = "SELECT c.employe, c.id ,c.state, c.user ,c.create_date, c.employe ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as name_user ,marketing_profile_attribut_value_temp.string_value\r\n,marketing_profile_instance_temp.id as instance_profile_temp ,c.state , marketing_profile_instance_temp.profile\r\nFROM commercial_partner_temp c\r\nleft join atooerp_person on  atooerp_person.id = c.employe\r\nleft join  marketing_profile_attribut_value_temp on marketing_profile_attribut_value_temp.partner_temp=c.id and marketing_profile_attribut_value_temp.attribut_name=\"name\"\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=c.id\r\nWHERE  (c.state=2 or c.state=3) order by c.create_date desc;";
            List<PartnerTemp> list = new List<PartnerTemp>();
            MySqlDataReader reader;
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id_employe = Convert.ToInt32(reader["employe"]);

                    if (reader["instance_profile_temp"].ToString() != "")
                    {
                        PartnerTemp temp = new PartnerTemp(Convert.ToInt32(reader["id"]), reader["name_user"].ToString(), reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"]), Convert.ToInt32(reader["instance_profile_temp"]), Convert.ToInt32(reader["profile"]), id_employe);
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            temp.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            temp.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            temp.Current = true;
                        }
                        list.Add(temp);
                    }
                    else
                    {
                        PartnerTemp temp = new PartnerTemp(Convert.ToInt32(reader["id"]), reader["name_user"].ToString(), reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"]), 0, 0, id_employe);
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            temp.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            temp.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            temp.Current = true;
                        }
                        list.Add(temp);
                    }





                }
                reader.Close();
                return list;



            }
            catch (Exception ex)
            {
                return null;

            }


        }
        public static async Task<List<PartnerTemp>> GetAllPartnerTemp()
        {
            string sqlCmd = "SELECT c.employe,c.id , c.user ,c.create_date,c.state, c.employe ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as name_user ,marketing_profile_attribut_value_temp.string_value\r\n,marketing_profile_instance_temp.id as instance_profile_temp , marketing_profile_instance_temp.profile\r\nFROM commercial_partner_temp c\r\nleft join atooerp_person on  atooerp_person.id = c.employe\r\nleft join  marketing_profile_attribut_value_temp on marketing_profile_attribut_value_temp.partner_temp=c.id and marketing_profile_attribut_value_temp.attribut_name=\"name\"\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=c.id\r\nWHERE c.partner is null and c.state=1 order by c.create_date desc;";
            //string sqlCmd = "SELECT c.id , c.user ,c.create_date, c.employe ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as name_user ,marketing_profile_attribut_value_temp.string_value\r\n,marketing_profile_instance_temp.id as instance_profile_temp ,c.state , marketing_profile_instance_temp.profile\r\nFROM commercial_partner_temp c\r\nleft join atooerp_person on  atooerp_person.id = c.employe\r\nleft join  marketing_profile_attribut_value_temp on marketing_profile_attribut_value_temp.partner_temp=c.id and marketing_profile_attribut_value_temp.attribut_name=\"name\"\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=c.id\r\nWHERE  (c.state=2 or c.state=3) order by c.create_date desc;";
            List<PartnerTemp> list = new List<PartnerTemp>();
            MySqlDataReader reader;
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                reader = cmd.ExecuteReader();
                while (reader.Read())

                /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                Avant :
                                {

                                    if (reader["instance_profile_temp"].ToString() != "")
                Après :
                                {

                                    if (reader["instance_profile_temp"].ToString() != "")
                */
                {
                    

                    if (reader["instance_profile_temp"].ToString() != "")
                    {
                        PartnerTemp temp = new PartnerTemp(Convert.ToInt32(reader["id"]), reader["name_user"].ToString(), reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"]), Convert.ToInt32(reader["instance_profile_temp"]), Convert.ToInt32(reader["profile"]), Convert.ToInt32(reader["employe"]));
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            temp.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            temp.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            temp.Current = true;
                        }
                        list.Add(temp);
                    }
                    else
                    {
                        PartnerTemp temp = new PartnerTemp(Convert.ToInt32(reader["id"]), reader["name_user"].ToString(), reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"]), 0, 0, Convert.ToInt32(reader["employe"]));
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            temp.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            temp.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            temp.Current = true;
                        }
                        list.Add(temp);
                    }





                }
                reader.Close();
                return list;



            }
            catch (Exception ex)
            {
                return null;

            }
        }
        public static async Task<List<PartnerTemp>> GetMyPartnerHistoryTemp()
        {
            int id = user_contrat.id_employe;
            //string sqlCmd = "SELECT c.id , c.user ,c.create_date, c.employe ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as name_user ,marketing_profile_attribut_value_temp.string_value\r\n,marketing_profile_instance_temp.id as instance_profile_temp , marketing_profile_instance_temp.profile\r\nFROM commercial_partner_temp c\r\nleft join atooerp_person on  atooerp_person.id = c.employe\r\nleft join  marketing_profile_attribut_value_temp on marketing_profile_attribut_value_temp.partner_temp=c.id and marketing_profile_attribut_value_temp.attribut_name=\"name\"\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=c.id\r\nWHERE c.partner is null and c.state=1 order by c.create_date desc;";
            string sqlCmd = "SELECT c.employe, c.id ,c.state, c.user ,c.create_date, c.employe ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as name_user ,marketing_profile_attribut_value_temp.string_value\r\n,marketing_profile_instance_temp.id as instance_profile_temp ,c.state , marketing_profile_instance_temp.profile\r\nFROM commercial_partner_temp c\r\nleft join atooerp_person on  atooerp_person.id = c.employe\r\nleft join  marketing_profile_attribut_value_temp on marketing_profile_attribut_value_temp.partner_temp=c.id and marketing_profile_attribut_value_temp.attribut_name=\"name\"\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=c.id\r\nWHERE c.employe=" + id + " and   (c.state=2 or c.state=3) order by c.create_date desc;";
            List<PartnerTemp> list = new List<PartnerTemp>();
            MySqlDataReader reader;
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id_employe = Convert.ToInt32(reader["employe"]);

                    if (reader["instance_profile_temp"].ToString() != "")
                    {
                        PartnerTemp temp = new PartnerTemp(Convert.ToInt32(reader["id"]), reader["name_user"].ToString(), reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"]), Convert.ToInt32(reader["instance_profile_temp"]), Convert.ToInt32(reader["profile"]), id_employe);
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            temp.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            temp.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            temp.Current = true;
                        }
                        list.Add(temp);
                    }
                    else
                    {
                        PartnerTemp temp = new PartnerTemp(Convert.ToInt32(reader["id"]), reader["name_user"].ToString(), reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"]), 0, 0, id_employe);
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            temp.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            temp.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            temp.Current = true;
                        }
                        list.Add(temp);
                    }





                }
                reader.Close();
                return list;



            }
            catch (Exception ex)
            {
                return null;

            }


        }
        public static async Task<List<PartnerTemp>> GetMyPartnerTemp()
        {
            int id = user_contrat.id_employe;
            string sqlCmd = "SELECT c.employe,c.id , c.user ,c.create_date,c.state, c.employe ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as name_user ,marketing_profile_attribut_value_temp.string_value\r\n,marketing_profile_instance_temp.id as instance_profile_temp , marketing_profile_instance_temp.profile\r\nFROM commercial_partner_temp c\r\nleft join atooerp_person on  atooerp_person.id = c.employe\r\nleft join  marketing_profile_attribut_value_temp on marketing_profile_attribut_value_temp.partner_temp=c.id and marketing_profile_attribut_value_temp.attribut_name=\"name\"\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=c.id\r\nWHERE c.employe=" + id + " and c.partner is null and c.state=1 order by c.create_date desc;";
            //string sqlCmd = "SELECT c.id , c.user ,c.create_date, c.employe ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as name_user ,marketing_profile_attribut_value_temp.string_value\r\n,marketing_profile_instance_temp.id as instance_profile_temp ,c.state , marketing_profile_instance_temp.profile\r\nFROM commercial_partner_temp c\r\nleft join atooerp_person on  atooerp_person.id = c.employe\r\nleft join  marketing_profile_attribut_value_temp on marketing_profile_attribut_value_temp.partner_temp=c.id and marketing_profile_attribut_value_temp.attribut_name=\"name\"\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=c.id\r\nWHERE  (c.state=2 or c.state=3) order by c.create_date desc;";
            List<PartnerTemp> list = new List<PartnerTemp>();
            MySqlDataReader reader;
            try
            {
                await DbConnection.Connecter3();
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                reader = cmd.ExecuteReader();
                while (reader.Read())

                /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                Avant :
                                {

                                    if (reader["instance_profile_temp"].ToString() != "")
                Après :
                                {

                                    if (reader["instance_profile_temp"].ToString() != "")
                */
                {


                    if (reader["instance_profile_temp"].ToString() != "")
                    {
                        PartnerTemp temp = new PartnerTemp(Convert.ToInt32(reader["id"]), reader["name_user"].ToString(), reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"]), Convert.ToInt32(reader["instance_profile_temp"]), Convert.ToInt32(reader["profile"]), Convert.ToInt32(reader["employe"]));
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            temp.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            temp.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            temp.Current = true;
                        }
                        list.Add(temp);
                    }
                    else
                    {
                        PartnerTemp temp = new PartnerTemp(Convert.ToInt32(reader["id"]), reader["name_user"].ToString(), reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"]), 0, 0, Convert.ToInt32(reader["employe"]));
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            temp.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            temp.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            temp.Current = true;
                        }
                        list.Add(temp);
                    }





                }
                reader.Close();
                return list;



            }
            catch (Exception ex)
            {
                return null;

            }
        }
        public static async Task<List<PartnerTemp>> GetMyPartnerHistoryRefusedTemp()
        {
            int id = user_contrat.id_employe;
            //string sqlCmd = "SELECT c.id , c.user ,c.create_date, c.employe ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as name_user ,marketing_profile_attribut_value_temp.string_value\r\n,marketing_profile_instance_temp.id as instance_profile_temp , marketing_profile_instance_temp.profile\r\nFROM commercial_partner_temp c\r\nleft join atooerp_person on  atooerp_person.id = c.employe\r\nleft join  marketing_profile_attribut_value_temp on marketing_profile_attribut_value_temp.partner_temp=c.id and marketing_profile_attribut_value_temp.attribut_name=\"name\"\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=c.id\r\nWHERE c.partner is null and c.state=1 order by c.create_date desc;";
            string sqlCmd = "SELECT c.employe, c.id ,c.state, c.user ,c.create_date, c.employe ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as name_user ,marketing_profile_attribut_value_temp.string_value\r\n,marketing_profile_instance_temp.id as instance_profile_temp ,c.state , marketing_profile_instance_temp.profile\r\nFROM commercial_partner_temp c\r\nleft join atooerp_person on  atooerp_person.id = c.employe\r\nleft join  marketing_profile_attribut_value_temp on marketing_profile_attribut_value_temp.partner_temp=c.id and marketing_profile_attribut_value_temp.attribut_name=\"name\"\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=c.id\r\nWHERE c.employe=" + id + " and   (c.state=3) order by c.create_date desc;";
            List<PartnerTemp> list = new List<PartnerTemp>();
            MySqlDataReader reader;
            try
            {
                await DbConnection.Connecter3();
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id_employe = Convert.ToInt32(reader["employe"]);

                    if (reader["instance_profile_temp"].ToString() != "")
                    {
                        PartnerTemp temp = new PartnerTemp(Convert.ToInt32(reader["id"]), reader["name_user"].ToString(), reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"]), Convert.ToInt32(reader["instance_profile_temp"]), Convert.ToInt32(reader["profile"]), id_employe);
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            temp.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            temp.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            temp.Current = true;
                        }
                        list.Add(temp);
                    }
                    else
                    {
                        PartnerTemp temp = new PartnerTemp(Convert.ToInt32(reader["id"]), reader["name_user"].ToString(), reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"]), 0, 0, id_employe);
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            temp.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            temp.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            temp.Current = true;
                        }
                        list.Add(temp);
                    }





                }
                reader.Close();
                return list;



            }
            catch (Exception ex)
            {
                return null;

            }


        }
        public static async Task<List<PartnerTemp>> GetMyPartnerHistoryAcceptedTemp()
        {
            int id = user_contrat.id_employe;
            //string sqlCmd = "SELECT c.id , c.user ,c.create_date, c.employe ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as name_user ,marketing_profile_attribut_value_temp.string_value\r\n,marketing_profile_instance_temp.id as instance_profile_temp , marketing_profile_instance_temp.profile\r\nFROM commercial_partner_temp c\r\nleft join atooerp_person on  atooerp_person.id = c.employe\r\nleft join  marketing_profile_attribut_value_temp on marketing_profile_attribut_value_temp.partner_temp=c.id and marketing_profile_attribut_value_temp.attribut_name=\"name\"\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=c.id\r\nWHERE c.partner is null and c.state=1 order by c.create_date desc;";
            string sqlCmd = "SELECT c.employe, c.id ,c.state, c.user ,c.create_date, c.employe ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as name_user ,marketing_profile_attribut_value_temp.string_value\r\n,marketing_profile_instance_temp.id as instance_profile_temp ,c.state , marketing_profile_instance_temp.profile\r\nFROM commercial_partner_temp c\r\nleft join atooerp_person on  atooerp_person.id = c.employe\r\nleft join  marketing_profile_attribut_value_temp on marketing_profile_attribut_value_temp.partner_temp=c.id and marketing_profile_attribut_value_temp.attribut_name=\"name\"\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=c.id\r\nWHERE c.employe=" + id + " and   c.state=2 order by c.create_date desc;";
            List<PartnerTemp> list = new List<PartnerTemp>();
            MySqlDataReader reader;
            try
            {
                await DbConnection.Connecter3();
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id_employe = Convert.ToInt32(reader["employe"]);

                    if (reader["instance_profile_temp"].ToString() != "")
                    {
                        PartnerTemp temp = new PartnerTemp(Convert.ToInt32(reader["id"]), reader["name_user"].ToString(), reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"]), Convert.ToInt32(reader["instance_profile_temp"]), Convert.ToInt32(reader["profile"]), id_employe);
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            temp.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            temp.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            temp.Current = true;
                        }
                        list.Add(temp);
                    }
                    else
                    {
                        PartnerTemp temp = new PartnerTemp(Convert.ToInt32(reader["id"]), reader["name_user"].ToString(), reader["string_value"].ToString(), Convert.ToDateTime(reader["create_date"]), 0, 0, id_employe);
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            temp.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            temp.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            temp.Current = true;
                        }
                        list.Add(temp);
                    }





                }
                reader.Close();
                return list;



            }
            catch (Exception ex)
            {
                return null;

            }


        }
    }
}
