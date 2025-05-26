//using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;

namespace SmartPharma5.Model
{
    public class ProfileAttributes : BaseViewModel
    {
        public int Id { get; set; }

        public int Id_Profile { get; set; }
        public int Rank { get; set; }

        private string labelName;

        public string LabelName { get => labelName; set => SetProperty(ref labelName, value); }

        private bool hasString;
        public bool HasString { get => hasString; set => SetProperty(ref hasString, value); }

        private bool hasNumber;
        public bool HasNumber { get => hasNumber; set => SetProperty(ref hasNumber, value); }

        private bool hasDecimal;
        public bool HasDecimal { get => hasDecimal; set => SetProperty(ref hasDecimal, value); }

        private bool hasDate;
        public bool HasDate { get => hasDate; set => SetProperty(ref hasDate, value); }

        private bool hasMultiple;
        public bool HasMultiple { get => hasMultiple; set => SetProperty(ref hasMultiple, value); }

        private bool hasBool;
        public bool HasBool { get => hasBool; set => SetProperty(ref hasBool, value); }


        private string string_value;
        public string String_value { get => string_value; set => SetProperty(ref string_value, value); }

        private decimal? number_value;
        public decimal? Number_value { get => number_value; set => SetProperty(ref number_value, value); }


        private DateTime? date_value;
        public DateTime? Date_value { get => date_value; set => SetProperty(ref date_value, value); }


        private int multiple_value;
        public int Multiple_value { get => multiple_value; set => SetProperty(ref multiple_value, value); }

        public int type_multi_value { get; set; }
        public int? type_parent_multi_value { get; set; }


        private bool? bool_value;

        public bool? Bool_value { get => bool_value; set => SetProperty(ref bool_value, value); }

        public bool is_null { get; set; } = true;


        public int profile_instance_id { get; set; }



        public AsyncCommand UpdateAllMultiList { get; set; }


        private List<atooerp_element> list_item;
        public List<atooerp_element> List_item { get => list_item; set => SetProperty(ref list_item, value); }

        //liste rest fixe 
        public List<atooerp_element> List_item_fixe;


        public atooerp_element selected_item;
        public atooerp_element Selected_item { get => selected_item; set => SetProperty(ref selected_item, value); }
        public int specific_type = 0;



        public ProfileAttributes(int id, string labelName, int id_profile, int rank)
        {
            UpdateAllMultiList = new AsyncCommand(Update);
            Id = id;
            LabelName = labelName;
            Id_Profile = id_profile;
            Date_value = null;
            Number_value = null;
            Bool_value = null;
            Rank = rank;



        }
        public ProfileAttributes()
        {
            UpdateAllMultiList = new AsyncCommand(Update);
            Date_value = DateTime.Now;

        }

        public async Task Update()
        {
            // selected_item= null;
            // Selected_item = null;
            //this.List_item= new List<atooerp_element>();

        }

        public static async Task<bool> CheckVatCodeExiste(string vat_code)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "select count(vat_code) from commercial_partner where vat_code = " + vat_code + " ;";
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                try
                {

                    int? p = int.Parse(cmd.ExecuteScalar().ToString());
                    if (p > 0)
                    {
                        return true;
                    }

                }
                catch (Exception ex)
                {
                    return true;
                }
            }
            return false;
        }
        public static async Task deleteInstanceProfileTemp(int id_profile_instance_temp)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "delete from marketing_profile_instance_temp where id = " + id_profile_instance_temp + ";";
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                try
                {
                    cmd.ExecuteScalar();
                    // UserDialogs.Instance.Alert("INSATANCE PROFILE TEMP DELETED");
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
        }


        public static async Task<int?> InsertInstanceWithTempPartner(int? id_temp_partner, int id_profile)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "insert into marketing_profile_instance_temp(partner_temp,profile,user,employe,state,create_date) values(" + id_temp_partner + "," + id_profile + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "') ;select max(id) from marketing_profile_instance_temp;";
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                try
                {

                    int? p = int.Parse(cmd.ExecuteScalar().ToString());
                    //UserDialogs.Instance.Alert("INSATANCE PROFILE TEMP CREATED SUCCEFULY");
                    return p;
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
            return null;
        }


        public static async Task<int?> InsertTempPartner(string name, string country, string street, string city, string state, string postal_code, string email, string fax, int category, bool customer, bool supplier, string vat_code, string gpsCoordinates)
        {
            if (await DbConnection.Connecter3())
            {
                int? Category = null;
                if (category.ToString() != "")
                {
                    Category = category;
                }
                string sqlCmd = "insert into  commercial_partner_temp(state,create_date,user,employe) values (1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + user_contrat.iduser + "," + user_contrat.id_employe + ");select max(id) from commercial_partner_temp;";
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                try
                {


                    // UserDialogs.Instance.Alert("PARTNER TEMP CREATED");
                    //await App.Current.MainPage.Navigation.PopAsync();
                    int temp = int.Parse(cmd.ExecuteScalar().ToString());

                    string sqlCmd2 = "insert into marketing_profile_attribut_value_temp(state,attribut_name,string_value,create_date,partner_temp,user,employe) values(1,'name','" + name + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + temp + "," + user_contrat.iduser + "," + user_contrat.id_employe + ");" +//name
                        "insert into marketing_profile_attribut_value_temp(state,attribut_name,string_value,create_date,partner_temp,user,employe) values(1,'fax','" + fax + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + temp + "," + user_contrat.iduser + "," + user_contrat.id_employe + ");" +//fax
                        "insert into marketing_profile_attribut_value_temp(state,attribut_name,string_value,create_date,partner_temp,user,employe) values(1,'street','" + street + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + temp + "," + user_contrat.iduser + "," + user_contrat.id_employe + ");" +//street
                        "insert into marketing_profile_attribut_value_temp(state,attribut_name,string_value,create_date,partner_temp,user,employe) values(1,'country','" + country + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + temp + "," + user_contrat.iduser + "," + user_contrat.id_employe + ");" +//country
                        "insert into marketing_profile_attribut_value_temp(state,attribut_name,string_value,create_date,partner_temp,user,employe) values(1,'city','" + city + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + temp + "," + user_contrat.iduser + "," + user_contrat.id_employe + ");" +//city
                        "insert into marketing_profile_attribut_value_temp(state,attribut_name,string_value,create_date,partner_temp,user,employe) values(1,'state','" + state + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + temp + "," + user_contrat.iduser + "," + user_contrat.id_employe + ");" +//state
                        "insert into marketing_profile_attribut_value_temp(state,attribut_name,string_value,create_date,partner_temp,user,employe) values(1,'postal_code','" + postal_code + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + temp + "," + user_contrat.iduser + "," + user_contrat.id_employe + ");" +//postal_code
                        "insert into marketing_profile_attribut_value_temp(state,attribut_name,string_value,create_date,partner_temp,user,employe) values(1,'email','" + email + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + temp + "," + user_contrat.iduser + "," + user_contrat.id_employe + ");" +//email
                        "insert into marketing_profile_attribut_value_temp(state,attribut_name,boolean_value,create_date,partner_temp,user,employe) values(1,'customer'," + customer + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + temp + "," + user_contrat.iduser + "," + user_contrat.id_employe + ");" +//customer
                        "insert into marketing_profile_attribut_value_temp(state,attribut_name,boolean_value,create_date,partner_temp,user,employe) values(1,'supplier'," + supplier + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + temp + "," + user_contrat.iduser + "," + user_contrat.id_employe + ");" +//supplier
                        "insert into marketing_profile_attribut_value_temp(state,attribut_name,int_value,create_date,partner_temp,user,employe) values(1,'category'," + (category) + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + temp + "," + user_contrat.iduser + "," + user_contrat.id_employe + ");" +
                        "insert into marketing_profile_attribut_value_temp(state,attribut_name,string_value,create_date,partner_temp,user,employe) values(1,'vat_code','" + vat_code + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + temp + "," + user_contrat.iduser + "," + user_contrat.id_employe + ");"+//category
                        "insert into marketing_profile_attribut_value_temp(state,attribut_name,string_value,create_date,partner_temp,user,employe) values(1,'gps','" + gpsCoordinates + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," +temp + "," + user_contrat.iduser + "," + user_contrat.id_employe + ");"; // GPS

                    MySqlCommand cmd2 = new MySqlCommand(sqlCmd2, DbConnection.con);
                    cmd2.ExecuteScalar();
                    return temp;


                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
            return null;
        }


        //------------------------------------------------------------------------------
        public static async Task<List<ProfileAttributes>> GetAttributesProfile2(int id_profile)
        {
            bool test = true;
            int id = id_profile;
            List<ProfileAttributes> list = new List<ProfileAttributes>();
            while (test)
            {
                string sqlCmd1 = "select m.Id,m.label,m.is_null,m.attribut_rank,m.attribut_type,marketing_profile.parent,marketing_profile.id as id_profile,atooerp_type.id as type_id,atooerp_type.parent as parent_type\r\nfrom marketing_profile_attribut m\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join atooerp_type on m.attribut_type=atooerp_type.Id\r\n where m.profile=" + id + "" +
                    " order by marketing_profile.id,m.attribut_rank;";

                MySqlDataReader reader = null;
                if (await DbConnection.Connecter3())
                {

                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int attribut = Convert.ToInt32(reader["attribut_type"]);

                            ProfileAttributes ProfileAttribut = new ProfileAttributes(Convert.ToInt32(reader["Id"]), ((Convert.ToBoolean(reader["is_null"]) == false) ? reader["label"].ToString() + "*" : reader["label"].ToString()), Convert.ToInt32(reader["id_profile"]), reader["attribut_rank"].ToString() == "" ? 0 : Convert.ToInt32(reader["attribut_rank"]));

                            ProfileAttribut.is_null = Convert.ToBoolean(reader["is_null"]);
                            ProfileAttribut.type_multi_value = Convert.ToInt32(reader["type_id"]);

                            //-----------------------------
                            if ((reader["parent"].ToString() == ""))
                            {
                                test = false;
                            }
                            else
                            {
                                id = Convert.ToInt32(reader["parent"]);
                            }

                            //-----------------------------
                            if (attribut == 1)
                            {
                                ProfileAttribut.HasString = true;

                            }
                            else if (attribut == 2)
                            {
                                ProfileAttribut.HasNumber = true;

                            }
                            else if (attribut == 3)
                            {
                                ProfileAttribut.HasDecimal = true;
                            }
                            else if (attribut == 4)
                            {
                                ProfileAttribut.HasBool = true;
                            }
                            else if (attribut == 5)
                            {

                            }
                            else if (attribut == 6)
                            {
                                ProfileAttribut.HasDate = true;
                            }
                            else
                            {
                                ProfileAttribut.HasMultiple = true;
                                ProfileAttribut.specific_type = attribut;
                                if (reader["parent_type"].ToString() == "")
                                {
                                    ProfileAttribut.type_parent_multi_value = null;
                                }
                                else
                                {
                                    ProfileAttribut.type_parent_multi_value = Convert.ToInt32(reader["parent_type"]);

                                }




                            }
                            //ProfileAttribut.profile_instance_id = id_instance;
                            list.Add(ProfileAttribut);


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

                foreach (ProfileAttributes att in list)
                {
                    if (att.HasMultiple)
                    {
                        try
                        {
                            att.List_item = new List<atooerp_element>(atooerp_element.getAtooerpElementByIdType(att.specific_type, att.Rank));
                            att.List_item_fixe = att.List_item;

                        }
                        catch
                        {

                        }

                    }

                }

                list = list.OrderBy(p => p.Id_Profile).ToList();






            }



            return list;


        }

        //------------------------------------------------------------------------------


        public static async Task<List<ProfileAttributes>> GetAttributesProfile(int id_profile)
        {
            string sqlCmd1 = "select m.Id,m.label,m.attribut_type ,marketing_profile.id as id_profile from marketing_profile_attribut m\r\nleft join marketing_profile on marketing_profile.id=m.profile " +
                "where m.profile=" + id_profile + " or m.profile=(select parent from marketing_profile profile2 where profile2.id=" + id_profile + ");";
            List<ProfileAttributes> list = new List<ProfileAttributes>();
            MySqlDataReader reader = null;
            if (await DbConnection.Connecter3())
            {

                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int attribut = Convert.ToInt32(reader["attribut_type"]);
                        ProfileAttributes ProfileAttribut = new ProfileAttributes(Convert.ToInt32(reader["Id"]), reader["label"].ToString(), Convert.ToInt32(reader["id_profile"]), (reader["attribut_rank"].ToString() != "") ? Convert.ToInt32(reader["attribut_rank"]) : 0);
                        if (attribut == 1)
                        {
                            ProfileAttribut.HasString = true;

                        }
                        else if (attribut == 2)
                        {
                            ProfileAttribut.HasNumber = true;

                        }
                        else if (attribut == 3)
                        {
                            ProfileAttribut.HasDecimal = true;
                        }
                        else if (attribut == 4)
                        {
                            ProfileAttribut.HasBool = true;
                        }
                        else if (attribut == 5)
                        {

                        }
                        else if (attribut == 6)
                        {
                            ProfileAttribut.HasDate = true;
                        }
                        else
                        {
                            ProfileAttribut.HasMultiple = true;
                            ProfileAttribut.specific_type = attribut;




                        }
                        //ProfileAttribut.profile_instance_id = id_instance;
                        list.Add(ProfileAttribut);


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

            foreach (ProfileAttributes att in list)
            {
                if (att.HasMultiple)
                {
                    try
                    {
                        att.List_item = new List<atooerp_element>(atooerp_element.getAtooerpElementByIdType(att.specific_type, att.Rank));

                    }
                    catch
                    {

                    }

                }

            }

            return list;

        }


    }
}
