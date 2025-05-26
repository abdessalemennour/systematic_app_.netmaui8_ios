//using Acr.UserDialogs;
using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.View;
//using Xamarin.CommunityToolkit.Converters;

namespace SmartPharma5.Model
{
    public class ValidateProfileChanges
    {

        public int Id { get; set; }
        public string Profile { get; set; }

        public string Current_profile { get; set; }

        public string Partner_Name { get; set; }

        public string User_name { get; set; }

        public DateTime? Create_date { get; set; }

        public int? User_Id { get; set; }
        public int? Employe_Id { get; set; }

        public int profile_id { get; set; }

        public int partner_id { get; set; }

        public AsyncCommand ShowAttributes { get; set; }
        public AsyncCommand ShowMyAttributes { get; set; }


        //add state to separte betwwen valid and réfused
        public bool Validated { get; set; }
        public bool Réfused { get; set; }

        public bool Current { get; set; }






        public ValidateProfileChanges(int id, string profile, string current, string user_name, DateTime create_date, int? user_id, int? employe_id, string partner_name, int partner, int profile_id)
        {
            ShowAttributes = new AsyncCommand(showAttributes);
            ShowMyAttributes = new AsyncCommand(showMyAttributes);
            Id = id;
            Profile = profile;
            Current_profile = current;
            User_name = user_name;
            Create_date = create_date;
            User_Id = user_id;
            Employe_Id = employe_id;
            Partner_Name = partner_name;
            this.partner_id = partner;
            this.profile_id = profile_id;

        }
        


        private async Task showMyAttributes()
        {
            try
            {

                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                await App.Current.MainPage.Navigation.PushAsync(new MyValidateAttributeChangeProfile(this.Id, this.partner_id, this.profile_id));
                UserDialogs.Instance.HideLoading();


            }
            catch (Exception ex)
            {

            }
        }
        private async Task showAttributes()
        {
            try
            {

                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                await App.Current.MainPage.Navigation.PushAsync(new ValidateAttributeChangeProfile(this.Id, this.partner_id, this.profile_id));
                UserDialogs.Instance.HideLoading();


            }
            catch (Exception ex)
            {

            }
        }
        
        public static async Task<List<ValidateProfileChanges>> GetAllChangesHistoryProfile()
        {
            int id = user_contrat.id_employe;
            List<ValidateProfileChanges> list = new List<ValidateProfileChanges>();
            //string sqlCmd = "select m.*,marketing_profile.name as change_profile , profile.name  as current_profile ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as user_name ,partner.name as partner_name\r\nFROM marketing_profile_instance_temp m\r\nleft join commercial_partner partner on partner.id = m.partner\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join marketing_profile_instances on marketing_profile_instances.partner=m.partner and marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance group by instance.partner)\r\nleft join marketing_profile profile on profile.id=marketing_profile_instances.profil\r\nleft join hr_employe employe on employe.user=m.user\r\nleft join atooerp_person on atooerp_person.id=employe.id\r\nwhere m.partner_temp is null and m.instance is null and m.state=1\r\norder by m.create_date desc;";
            string sqlCmd = "select m.*,marketing_profile.name as change_profile , profile.name  as current_profile ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as user_name ,partner.name as partner_name\r\nFROM marketing_profile_instance_temp m\r\nleft join commercial_partner partner on partner.id = m.partner\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join marketing_profile_instances on marketing_profile_instances.partner=m.partner and marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance group by instance.partner)\r\nleft join marketing_profile profile on profile.id=marketing_profile_instances.profil\r\nleft join hr_employe employe on employe.user=m.user\r\nleft join atooerp_person on atooerp_person.id=m.employe\r\nwhere m.partner_temp is null and (m.state=2 or m.state=3)\r\norder by m.create_date desc;";
            if (await DbConnection.Connecter3())
            {


                MySqlDataReader reader;
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ValidateProfileChanges attribute = new ValidateProfileChanges(Convert.ToInt32(reader["id"]), reader["change_profile"].ToString(), reader["current_profile"].ToString(), reader["user_name"].ToString(), Convert.ToDateTime(reader["create_date"]), Convert.ToInt32(reader["user"]), Convert.ToInt32(reader["employe"]), reader["partner_name"].ToString(), Convert.ToInt32(reader["partner"]), Convert.ToInt32(reader["profile"]));
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            attribute.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            attribute.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            attribute.Current = true;
                        }
                        list.Add(attribute);


                    }
                    reader.Close();
                    return list;


                }
                catch (Exception ex)
                {

                }
                {
                    return null;

                }
            }
            else
            {
                return null;
            }
        }
        public static async Task<List<ValidateProfileChanges>> GetMyChangesHistoryProfile()
        {
            int id = user_contrat.id_employe;
            List<ValidateProfileChanges> list = new List<ValidateProfileChanges>();
            //string sqlCmd = "select m.*,marketing_profile.name as change_profile , profile.name  as current_profile ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as user_name ,partner.name as partner_name\r\nFROM marketing_profile_instance_temp m\r\nleft join commercial_partner partner on partner.id = m.partner\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join marketing_profile_instances on marketing_profile_instances.partner=m.partner and marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance group by instance.partner)\r\nleft join marketing_profile profile on profile.id=marketing_profile_instances.profil\r\nleft join hr_employe employe on employe.user=m.user\r\nleft join atooerp_person on atooerp_person.id=employe.id\r\nwhere m.partner_temp is null and m.instance is null and m.state=1\r\norder by m.create_date desc;";
            string sqlCmd = "select m.*,marketing_profile.name as change_profile , profile.name  as current_profile ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as user_name ,partner.name as partner_name\r\nFROM marketing_profile_instance_temp m\r\nleft join commercial_partner partner on partner.id = m.partner\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join marketing_profile_instances on marketing_profile_instances.partner=m.partner and marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance group by instance.partner)\r\nleft join marketing_profile profile on profile.id=marketing_profile_instances.profil\r\nleft join hr_employe employe on employe.user=m.user\r\nleft join atooerp_person on atooerp_person.id=m.employe\r\nwhere m.employe="+id+" and  m.partner_temp is null and (m.state=2 or m.state=3)\r\norder by m.create_date desc;";
            if (await DbConnection.Connecter3())
            {
                MySqlDataReader reader;
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ValidateProfileChanges attribute = new ValidateProfileChanges(Convert.ToInt32(reader["id"]), reader["change_profile"].ToString(), reader["current_profile"].ToString(), reader["user_name"].ToString(), Convert.ToDateTime(reader["create_date"]), Convert.ToInt32(reader["user"]), Convert.ToInt32(reader["employe"]), reader["partner_name"].ToString(), Convert.ToInt32(reader["partner"]), Convert.ToInt32(reader["profile"]));
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            attribute.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            attribute.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            attribute.Current = true;
                        }
                        list.Add(attribute);


                    }
                    reader.Close();
                    return list;


                }
                catch (Exception ex)
                {

                }
                {
                    return null;

                }
            }
            else
            {
                return null;
            }
        }
        public static async Task<List<ValidateProfileChanges>> GetMyChangesAcceptedHistoryProfile()
        {
            int id = user_contrat.id_employe;
            List<ValidateProfileChanges> list = new List<ValidateProfileChanges>();
            //string sqlCmd = "select m.*,marketing_profile.name as change_profile , profile.name  as current_profile ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as user_name ,partner.name as partner_name\r\nFROM marketing_profile_instance_temp m\r\nleft join commercial_partner partner on partner.id = m.partner\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join marketing_profile_instances on marketing_profile_instances.partner=m.partner and marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance group by instance.partner)\r\nleft join marketing_profile profile on profile.id=marketing_profile_instances.profil\r\nleft join hr_employe employe on employe.user=m.user\r\nleft join atooerp_person on atooerp_person.id=employe.id\r\nwhere m.partner_temp is null and m.instance is null and m.state=1\r\norder by m.create_date desc;";
            string sqlCmd = "select m.*,marketing_profile.name as change_profile , profile.name  as current_profile ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as user_name ,partner.name as partner_name\r\nFROM marketing_profile_instance_temp m\r\nleft join commercial_partner partner on partner.id = m.partner\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join marketing_profile_instances on marketing_profile_instances.partner=m.partner and marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance group by instance.partner)\r\nleft join marketing_profile profile on profile.id=marketing_profile_instances.profil\r\nleft join hr_employe employe on employe.user=m.user\r\nleft join atooerp_person on atooerp_person.id=m.employe\r\nwhere m.employe=" + id + " and  m.partner_temp is null and m.state=2 \r\norder by m.create_date desc;";
            if (await DbConnection.Connecter3())
            {


                MySqlDataReader reader;
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ValidateProfileChanges attribute = new ValidateProfileChanges(Convert.ToInt32(reader["id"]), reader["change_profile"].ToString(), reader["current_profile"].ToString(), reader["user_name"].ToString(), Convert.ToDateTime(reader["create_date"]), Convert.ToInt32(reader["user"]), Convert.ToInt32(reader["employe"]), reader["partner_name"].ToString(), Convert.ToInt32(reader["partner"]), Convert.ToInt32(reader["profile"]));
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            attribute.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            attribute.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            attribute.Current = true;
                        }
                        list.Add(attribute);


                    }
                    reader.Close();
                    return list;


                }
                catch (Exception ex)
                {

                }
                {
                    return null;

                }
            }
            else
            {
                return null;
            }
        }
        public static async Task<List<ValidateProfileChanges>> GetMyChangesRefusedHistoryProfile()
        {
            int id = user_contrat.id_employe;
            List<ValidateProfileChanges> list = new List<ValidateProfileChanges>();
            //string sqlCmd = "select m.*,marketing_profile.name as change_profile , profile.name  as current_profile ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as user_name ,partner.name as partner_name\r\nFROM marketing_profile_instance_temp m\r\nleft join commercial_partner partner on partner.id = m.partner\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join marketing_profile_instances on marketing_profile_instances.partner=m.partner and marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance group by instance.partner)\r\nleft join marketing_profile profile on profile.id=marketing_profile_instances.profil\r\nleft join hr_employe employe on employe.user=m.user\r\nleft join atooerp_person on atooerp_person.id=employe.id\r\nwhere m.partner_temp is null and m.instance is null and m.state=1\r\norder by m.create_date desc;";
            string sqlCmd = "select m.*,marketing_profile.name as change_profile , profile.name  as current_profile ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as user_name ,partner.name as partner_name\r\nFROM marketing_profile_instance_temp m\r\nleft join commercial_partner partner on partner.id = m.partner\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join marketing_profile_instances on marketing_profile_instances.partner=m.partner and marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance group by instance.partner)\r\nleft join marketing_profile profile on profile.id=marketing_profile_instances.profil\r\nleft join hr_employe employe on employe.user=m.user\r\nleft join atooerp_person on atooerp_person.id=m.employe\r\nwhere m.employe=" + id + " and  m.partner_temp is null and m.state=3 \r\norder by m.create_date desc;";
            if (await DbConnection.Connecter3())
            {


                MySqlDataReader reader;
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ValidateProfileChanges attribute = new ValidateProfileChanges(Convert.ToInt32(reader["id"]), reader["change_profile"].ToString(), reader["current_profile"].ToString(), reader["user_name"].ToString(), Convert.ToDateTime(reader["create_date"]), Convert.ToInt32(reader["user"]), Convert.ToInt32(reader["employe"]), reader["partner_name"].ToString(), Convert.ToInt32(reader["partner"]), Convert.ToInt32(reader["profile"]));
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            attribute.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            attribute.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            attribute.Current = true;
                        }
                        list.Add(attribute);


                    }
                    reader.Close();
                    return list;


                }
                catch (Exception ex)
                {

                }
                {
                    return null;

                }
            }
            else
            {
                return null;
            }
        }
        public static async Task<List<ValidateProfileChanges>> GetAllChangesProfile()
        {

            List<ValidateProfileChanges> list = new List<ValidateProfileChanges>();
            string sqlCmd = "select m.*,marketing_profile.name as change_profile , profile.name  as current_profile ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as user_name ,partner.name as partner_name\r\nFROM marketing_profile_instance_temp m\r\nleft join commercial_partner partner on partner.id = m.partner\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join marketing_profile_instances on marketing_profile_instances.partner=m.partner and marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance group by instance.partner)\r\nleft join marketing_profile profile on profile.id=marketing_profile_instances.profil\r\nleft join hr_employe employe on employe.user=m.user\r\nleft join atooerp_person on atooerp_person.id=m.employe\r\nwhere m.partner_temp is null and m.instance is null and m.state=1\r\norder by m.create_date desc;";
            // string sqlCmd = "select m.*,marketing_profile.name as change_profile , profile.name  as current_profile ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as user_name ,partner.name as partner_name \r\nFROM marketing_profile_instance_temp m\r\nleft join commercial_partner partner on partner.id = m.partner\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join marketing_profile_instances on marketing_profile_instances.partner=m.partner and marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance group by instance.partner)\r\nleft join marketing_profile profile on profile.id=marketing_profile_instances.profil\r\nleft join hr_employe employe on employe.user=m.user\r\nleft join atooerp_person on atooerp_person.id=employe.id\r\nwhere m.partner_temp is null and (m.state=2 or m.state=3)\r\norder by m.create_date desc;";
            if (await DbConnection.Connecter3())
            {


                MySqlDataReader reader;
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ValidateProfileChanges attribute = new ValidateProfileChanges(Convert.ToInt32(reader["id"]), reader["change_profile"].ToString(), reader["current_profile"].ToString(), reader["user_name"].ToString(), Convert.ToDateTime(reader["create_date"]), Convert.ToInt32(reader["user"]), Convert.ToInt32(reader["employe"]), reader["partner_name"].ToString(), Convert.ToInt32(reader["partner"]), Convert.ToInt32(reader["profile"]));
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            attribute.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            attribute.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            attribute.Current = true;
                        }
                        list.Add(attribute);


                    }
                    reader.Close();

                    /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                    Avant :
                                        return list;


                                        }catch(Exception ex)
                    Après :
                                        return list;


                                    }
                                    catch(Exception ex)
                    */
                    return list;


                }
                catch (Exception ex)
                {

                }
                {
                    return null;

                }
            }
            else
            {
                return null;

                /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                Avant :
                            }


                        }
                Après :
                            }


                        }
                */
            }


        }
        public static async Task<List<ValidateProfileChanges>> GetMyChangesProfile()
        {
            int id = user_contrat.id_employe;
            List<ValidateProfileChanges> list = new List<ValidateProfileChanges>();
            string sqlCmd = "select m.*,marketing_profile.name as change_profile , profile.name  as current_profile ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as user_name ,partner.name as partner_name\r\nFROM marketing_profile_instance_temp m\r\nleft join commercial_partner partner on partner.id = m.partner\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join marketing_profile_instances on marketing_profile_instances.partner=m.partner and marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance group by instance.partner)\r\nleft join marketing_profile profile on profile.id=marketing_profile_instances.profil\r\nleft join hr_employe employe on employe.user=m.user\r\nleft join atooerp_person on atooerp_person.id=m.employe\r\nwhere  m.employe="+id+" and m.partner_temp is null and m.instance is null and m.state=1\r\norder by m.create_date desc;";
            // string sqlCmd = "select m.*,marketing_profile.name as change_profile , profile.name  as current_profile ,concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as user_name ,partner.name as partner_name \r\nFROM marketing_profile_instance_temp m\r\nleft join commercial_partner partner on partner.id = m.partner\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join marketing_profile_instances on marketing_profile_instances.partner=m.partner and marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance group by instance.partner)\r\nleft join marketing_profile profile on profile.id=marketing_profile_instances.profil\r\nleft join hr_employe employe on employe.user=m.user\r\nleft join atooerp_person on atooerp_person.id=employe.id\r\nwhere m.partner_temp is null and (m.state=2 or m.state=3)\r\norder by m.create_date desc;";
            if (await DbConnection.Connecter3())
            {


                MySqlDataReader reader;
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ValidateProfileChanges attribute = new ValidateProfileChanges(Convert.ToInt32(reader["id"]), reader["change_profile"].ToString(), reader["current_profile"].ToString(), reader["user_name"].ToString(), Convert.ToDateTime(reader["create_date"]), Convert.ToInt32(reader["user"]), Convert.ToInt32(reader["employe"]), reader["partner_name"].ToString(), Convert.ToInt32(reader["partner"]), Convert.ToInt32(reader["profile"]));
                        if (Convert.ToInt32(reader["state"]) == 2)
                        {
                            attribute.Validated = true;

                        }
                        else if (Convert.ToInt32(reader["state"]) == 3)
                        {
                            attribute.Réfused = true;
                        }
                        else if (Convert.ToInt32(reader["state"]) == 1)
                        {
                            attribute.Current = true;
                        }
                        list.Add(attribute);


                    }
                    reader.Close();

                    /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                    Avant :
                                        return list;


                                        }catch(Exception ex)
                    Après :
                                        return list;


                                    }
                                    catch(Exception ex)
                    */
                    return list;


                }
                catch (Exception ex)
                {

                }
                {
                    return null;

                }
            }
            else
            {
                return null;

                /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                Avant :
                            }


                        }
                Après :
                            }


                        }
                */
            }


        }
    }

    public class ValidateAttributeProfile : BaseViewModel
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public int Id_Attribute { get; set; }


        //----------------------CHECH UPDATE -----------------------------------------

        private bool valid_attribute;
        public bool Valid_attribute { get => valid_attribute; set => SetProperty(ref valid_attribute, value); }


        /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
        Avant :
                //-----------------------VALUES ----------------------------------------------



                public string String_value { get; set; }
        Après :
                //-----------------------VALUES ----------------------------------------------



                public string String_value { get; set; }
        */
        //-----------------------VALUES ----------------------------------------------



        public string String_value { get; set; }
        public int? Int_value { get; set; } = null;

        public decimal? Decimal_value { get; set; } = null;
        public bool? Boolean_value { get; set; } = null;
        public DateTime? Date_value { get; set; } = null;

        public int? Type_value { get; set; } = null;
        public string String_Type_value { get; set; }


        //---------------------------Check Values-----------------------------------------

        public bool HasString { get; set; }
        public bool HasInt { get; set; }
        public bool HasDecimal { get; set; }
        public bool HasBool { get; set; }
        public bool HasDate { get; set; }
        public bool HasType { get; set; }



        public ValidateAttributeProfile()
        {
            Valid_attribute = true;


        }



        public static async Task<List<ValidateAttributeProfile>> GetAllAttributesProfile(int id)
        {

            List<ValidateAttributeProfile> list = new List<ValidateAttributeProfile>();
            string sqlCmd = "SELECT m.*,atooerp_type_element.id as type_element ,atooerp_type_element.name as type_name , marketing_profile_attribut.label  FROM marketing_profile_attribut_value_temp m " +
                "left join atooerp_type_element on atooerp_type_element.id = m.type " +
                "left join marketing_profile_attribut on marketing_profile_attribut.id = m.profile_attribute " +
                "where m.profile_instance_temp=" + id + ";";
            if (await DbConnection.Connecter3())
            {


                MySqlDataReader reader;
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ValidateAttributeProfile attribute = new ValidateAttributeProfile();
                        if (Convert.ToString(reader["string_value"]) != null && Convert.ToString(reader["boolean_value"]) == "" && Convert.ToString(reader["date_value"]) == "" && Convert.ToString(reader["decimal_value"]) == "" && Convert.ToString(reader["int_value"]) == "" && Convert.ToString(reader["type"]) == "")
                        {
                            attribute.String_value = reader["string_value"].ToString();
                            attribute.HasString = true;
                        }
                        else if (reader["int_value"].ToString() != "")
                        {
                            attribute.Int_value = Convert.ToInt32(reader["int_value"]);
                            attribute.HasInt = true;

                        }
                        else if (reader["decimal_value"].ToString() != "")
                        {
                            attribute.Decimal_value = Convert.ToDecimal(reader["decimal_value"]);
                            attribute.HasDecimal = true;
                        }
                        else if (reader["boolean_value"].ToString() != "")
                        {
                            attribute.Boolean_value = Convert.ToBoolean(reader["boolean_value"]);
                            attribute.HasBool = true;
                        }
                        else if (reader["date_value"].ToString() != "")
                        {
                            attribute.Date_value = Convert.ToDateTime(reader["date_value"]);
                            attribute.HasDate = true;
                        }
                        else if (reader["type"].ToString() != "")
                        {
                            attribute.Type_value = Convert.ToInt32(reader["type"]);
                            attribute.String_Type_value = reader["type_name"].ToString();
                            attribute.HasType = true;
                        }
                        else
                        {

                        }
                        attribute.Id_Attribute = Convert.ToInt32(reader["profile_attribute"]);
                        attribute.Label = reader["label"].ToString();
                        list.Add(attribute);


                    }
                    reader.Close();
                    return list;


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

        }
    }
}
