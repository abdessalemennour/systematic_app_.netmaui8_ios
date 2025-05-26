//using Android.Renderscripts;
//using DevExpress.iOS.DataForm;
using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;
//using static CoreFoundation.DispatchQueue;
using static DevExpress.Maui.Core.Internal.Either;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Command = MvvmHelpers.Commands.Command;

namespace SmartPharma5.Model
{
    public class MyProfileAttrubutes : BaseViewModel
    {
        public int id { get; set; }
        public int id_partner { get; set; }

        public string profile_name { get; set; }

        public bool HasUpdatedValues { get; set; } = false;

        //public Command SaveTempValue { get; }
        public MvvmHelpers.Commands.Command<int?> getTemp { get; }
        public string nameAttribute { get; set; }
        public string labelAttribute { get; set; }
        public string value { get; set; }
        public bool HasValue { get; set; }
        public string type { get; set; }
        public string type_editor { get; set; }
        public int id_type { get; set; } = 0;
        public int id_element { get; set; } = 0;
        public bool HasTempValue { get; set; } = false;
        public int id_attribute { get; set; } = 0;
        public bool IsAffected { get; set; }


        //------------------------------------------------ selected value ------------------------------------
        public atooerp_element selected_element { get; set; }
        public string string_value { get; set; }

        public decimal numeric_value { get; set; }
        public DateTime date_value { get; set; } = DateTime.Now;
        public bool check_value { get; set; }

        public List<tempValue> tempValues { get; set; }
        public List<atooerp_element> listOfTypeElement { get; set; }

        public bool HasText { get; set; } = false;
        public bool HasMemo { get; set; } = false;
        public bool HasDate { get; set; } = false;
        public bool HasList { get; set; } = false;
        public bool HasNumber { get; set; } = false;
        public bool HasCheck { get; set; } = false;
        public bool HasDecimal { get; set; } = false;
        //--------------------------------------------------------
        // public bool IsUpdated { get; set; } = true;
        // public bool BtnUpdated { get; set; } = true;

        private bool btnUpdated;
        public bool BtnUpdated { get => btnUpdated; set => SetProperty(ref btnUpdated, value); }
        public Command UpdateCommand { get; }
        private bool isUpdated;
        public bool IsUpdated { get => isUpdated; set => SetProperty(ref isUpdated, value); }


        private bool tempValuesList;
        public bool TempValuesList { get => tempValuesList; set => SetProperty(ref tempValuesList, value); }



        private bool btnUp;
        public bool BtnUp { get => btnUp; set => SetProperty(ref btnUp, value); }


        private bool btnDown;
        public bool BtnDown { get => btnDown; set => SetProperty(ref btnDown, value); }


        public AsyncCommand ShowTempValues { get; }

        public AsyncCommand CloseTempValues { get; }


        public AsyncCommand Update { get; }

        public Command closeCommand { get; }




        public MyProfileAttrubutes() { }
        public MyProfileAttrubutes(string labelAttribute, string value)
        {
            this.labelAttribute = labelAttribute;
            this.value = value;
            this.Update = new AsyncCommand(update_fnc);
        }
        public MyProfileAttrubutes(int id, string nameAttribute, string labelAttribute, string type, string type_editor, bool hasValue, string value, string profile_name,int type_id,int id_attribute,bool isAffected )
        {
            BtnUpdated = true;
            BtnDown = true;
            this.id = id;

            this.nameAttribute = nameAttribute;
            this.value = value;
            HasValue = hasValue;
            this.type = type;
            this.labelAttribute = labelAttribute;
            this.type_editor = type_editor;
            this.profile_name = profile_name;
            this.id_type=type_id;
            this.id_attribute = id_attribute;
            this.IsAffected = isAffected;
            this.Update = new AsyncCommand(update_fnc);
            ShowTempValues = new AsyncCommand(showTemp);
            CloseTempValues = new AsyncCommand(closeTemp);


            getTemp = new MvvmHelpers.Commands.Command<int?>(async (param) =>
            {
                var parametre = param;

                string sqlcmd = "";

                if (await App.Current.MainPage.DisplayAlert("Info", "DO YOU WANT TO SAVE CHANGES?", "YES", "NO"))
                {
                    if (this.HasText == true)
                    {

                        sqlcmd = "insert into marketing_profile_attribut_value_temp(name,string_value,attribut_value,create_date,user,state,employe) " +
                        "values ('string','" + this.string_value + "'," + this.id + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + user_contrat.iduser + ",1," + user_contrat.id_employe + " );";
                      

                    }
                    else if (this.HasMemo == true)
                    {
                        sqlcmd = "insert into marketing_profile_attribut_value_temp(name,string_value,attribut_value,create_date,user,state,employe) " +
                        "values ('memo','" + this.string_value + "'," + this.id + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + user_contrat.iduser + ",1," + user_contrat.id_employe + " );";
                    }
                    else if (this.HasNumber == true)
                    {

                        if (this.numeric_value.ToString() == "")
                        {
                            await App.Current.MainPage.DisplayAlert("Warnning", "you must write a number", "OK");
                            return;

                        }
                        else
                        {

                            sqlcmd = "insert into marketing_profile_attribut_value_temp(name,int_value,attribut_value,create_date,user,state,employe) " +
                            "values ('string'," + Convert.ToInt32(this.numeric_value) + "," + this.id + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + user_contrat.iduser + ",1," + user_contrat.id_employe + " );";
                        }


                    }
                    else if (this.HasDate == true)
                    {
                        sqlcmd = "insert into marketing_profile_attribut_value_temp(name,date_value,attribut_value,create_date,user,state,employe) " +
                       "values ('string','" + this.date_value.ToString("yyyy-MM-dd HH:mm:ss") + "'," + this.id + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + user_contrat.iduser + ",1," + user_contrat.id_employe + " );";
                    }
                    else if (this.HasCheck == true)
                    {
                        sqlcmd = "insert into marketing_profile_attribut_value_temp(name,boolean_value,attribut_value,create_date,user,state,employe) " +
                       "values ('string'," + Convert.ToBoolean(this.check_value) + "," + this.id + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + user_contrat.iduser + ",1," + user_contrat.id_employe + " );";
                    }
                    else if (this.HasList == true)
                    {
                        if (this.selected_element == null)
                        {
                            await App.Current.MainPage.DisplayAlert("Warnning", "You must pick item", "OK");
                            return;
                        }
                        else
                        {
                            if (!this.IsAffected)
                            {

                                string sqlcmd1 = "insert into marketing_profile_attribut_value(create_date,attribut,profile_instance,type) values ('"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',"+ this.id + ","+parametre+ ",null);select max(id) from marketing_profile_attribut_value;";
                                DbConnection.Deconnecter();
                                await DbConnection.Connecter3();
                                MySqlCommand cmd1 = new MySqlCommand(sqlcmd1, DbConnection.con);
                                int  result = int.Parse(cmd1.ExecuteScalar().ToString());


                                sqlcmd = "insert into marketing_profile_attribut_value_temp(name,type,attribut_value,create_date,user,state,employe) " +
                                    "values ('string'," + Convert.ToInt32(this.selected_element.id) + "," + result + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + user_contrat.iduser + ",1," + user_contrat.id_employe + " );";

                            }
                            else
                            {
                                sqlcmd = "insert into marketing_profile_attribut_value_temp(name,type,attribut_value,create_date,user,state,employe) " +
                                        "values ('string'," + Convert.ToInt32(this.selected_element.id) + "," + this.id + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + user_contrat.iduser + ",1," + user_contrat.id_employe + " );";
                            }
                        }

                    }
                    else if (this.HasDecimal == true)
                    {
                        if (this.numeric_value.ToString() == "")
                        {
                            await App.Current.MainPage.DisplayAlert("Warnning", "you must write a decimal", "OK");
                            return;

                        }
                        else
                        {

                            sqlcmd = "insert into marketing_profile_attribut_value_temp(name,decimal_value,attribut_value,create_date,user,state,employe) " +
                           "values ('string'," + Convert.ToDecimal(this.numeric_value).ToString().Replace(',', '.') + "," + this.id + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + user_contrat.iduser + ",1," + user_contrat.id_employe + " );";
                        }


                    }

                    DbConnection.Deconnecter();
                    DbConnection.Connecter();

                    MySqlCommand cmd = new MySqlCommand(sqlcmd, DbConnection.con);
                    cmd.ExecuteReader();
                    this.IsUpdated = false;
                    this.BtnUpdated = true;
                    await App.Current.MainPage.DisplayAlert("DONE", "REQUEST UPDATE SAVED SUCCEFULY", "OK");


                }





            });

            //UpdateCommand = new Command(() =>
            //{
            //    IsUpdated = true;
            //    BtnUpdated = false;

            //});


            closeCommand = new Command(() =>
            {

                IsUpdated = false;
                BtnUpdated = true;
            });




        }



        public async Task update_fnc()
        {
            this.IsUpdated = true;
            this.BtnUpdated = false;
        }

        public async Task closeTemp()
        {
            TempValuesList = false;
            BtnDown = true;
            BtnUp = false;
        }

        public async Task showTemp()
        {
            TempValuesList = true;
            BtnDown = false;
            BtnUp = true;

        }
        


        public static List<MyProfileAttrubutes> getMyAttributbyId(int id_partner)
        {
            string sqlCmd1 = "select marketing_profile_attribut.Id ,  marketing_profile_attribut.label , marketing_profile_attribut.name as name_attribute ,\r\n    " +
                "marketing_profile.name as profile_name , atooerp_type.name as name_type , atooerp_type.Id as type_id,atooerp_type.name as name_type ,\r\n    " +
                "atooerp_input_editors.name as editor_type from marketing_profile_attribut\r\n    " +
                "left join marketing_profile on marketing_profile.Id=marketing_profile_attribut.profile\r\n    " +
                "left join atooerp_type on atooerp_type.id = marketing_profile_attribut.attribut_type\r\n    " +
                "left join atooerp_input_editors on atooerp_input_editors.id = marketing_profile_attribut.editor\r\n " +
                "where marketing_profile_attribut.profile= (select marketing_profile_instances.profil " +
                "FROM marketing_profile_instances where marketing_profile_instances.Id =\r\n " +
                "(select max(marketing_profile_instances.Id) from marketing_profile_instances where marketing_profile_instances.partner = "+ id_partner + "));";
            DbConnection.Deconnecter();
            DbConnection.Connecter();
            List<MyProfileAttrubutes> list = new List<MyProfileAttrubutes>();
            MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    MyProfileAttrubutes attribute = new MyProfileAttrubutes();

                    if (Convert.ToInt64(reader["type_id"]) == 1)
                    {
                        attribute = new MyProfileAttrubutes(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name_attribute"]), Convert.ToString(reader["label"]).ToUpper(), Convert.ToString(reader["name_type"]), Convert.ToString(reader["editor_type"]), true, "", Convert.ToString(reader["profile_name"]), Convert.ToInt32(reader["type_id"]),Convert.ToInt32(reader["Id"]),false);
                        attribute.HasText = true;

                    }
                    else if (Convert.ToInt64(reader["type_id"]) == 2)
                    {
                        attribute = new MyProfileAttrubutes(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name_attribute"]), Convert.ToString(reader["label"]).ToUpper(), Convert.ToString(reader["name_type"]), Convert.ToString(reader["editor_type"]), true, "", Convert.ToString(reader["profile_name"]), Convert.ToInt32(reader["type_id"]),Convert.ToInt32(reader["Id"]), false);
                        attribute.HasNumber = true;
                    }
                    else if (Convert.ToInt64(reader["type_id"]) == 3)
                    {
                        attribute = new MyProfileAttrubutes(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name_attribute"]), Convert.ToString(reader["label"]).ToUpper(), Convert.ToString(reader["name_type"]), Convert.ToString(reader["editor_type"]), true, "", Convert.ToString(reader["profile_name"]), Convert.ToInt32(reader["type_id"]),Convert.ToInt32(reader["Id"]), false);
                        attribute.HasDecimal = true;
                    }
                    else if (Convert.ToInt64(reader["type_id"]) == 4)
                    {
                        attribute = new MyProfileAttrubutes(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name_attribute"]), Convert.ToString(reader["label"]).ToUpper(), Convert.ToString(reader["name_type"]), Convert.ToString(reader["editor_type"]), true, "", Convert.ToString(reader["profile_name"]), Convert.ToInt32(reader["type_id"]),Convert.ToInt32(reader["Id"]), false);
                        attribute.HasCheck = true;
                    }
                    else if (Convert.ToInt64(reader["type_id"]) == 6)
                    {
                        attribute = new MyProfileAttrubutes(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name_attribute"]), Convert.ToString(reader["label"]).ToUpper(), Convert.ToString(reader["name_type"]), Convert.ToString(reader["editor_type"]), true, "", Convert.ToString(reader["profile_name"]), Convert.ToInt32(reader["type_id"]),Convert.ToInt32(reader["Id"]), false);
                        attribute.HasDate = true;
                    }
                    else
                    {
                        attribute = new MyProfileAttrubutes(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name_attribute"]), Convert.ToString(reader["label"]).ToUpper(), Convert.ToString(reader["name_type"]), Convert.ToString(reader["editor_type"]), true, "", Convert.ToString(reader["profile_name"]), Convert.ToInt32(reader["type_id"]),Convert.ToInt32(reader["Id"]), false);
                        attribute.HasList = true;
                        //attribute.listOfTypeElement = atooerp_element.getAtooerpElementByIdType(attribute.id_type, 0);

                    }
                    list.Add(attribute);

                }
                catch (Exception ex)
                {

                }
                
                
            }
            reader.Close();
            DbConnection.Deconnecter();
            foreach (MyProfileAttrubutes attributes in list)
            {
                if (attributes.HasList == true)
                {
                    attributes.listOfTypeElement = atooerp_element.getAtooerpElementByIdType(attributes.id_type, 0);
                }
            }
            return list;

        }


        public static List<MyProfileAttrubutes> getMyAttributValuebyId(int id_partner)
        {
            string sqlCmd1 = "select marketing_profile_attribut_value.Id , marketing_profile_attribut.Id as attribut_id, marketing_profile_attribut.label , marketing_profile_attribut.name as name_attribute ,\r\n marketing_profile_attribut_value.string_value , marketing_profile_attribut_value.boolean_value , marketing_profile_attribut_value.date_value ,\r\n  marketing_profile_attribut_value.int_value , marketing_profile_attribut_value.decimal_value , marketing_profile_attribut_value.blob_value ,\r\n  marketing_profile.name as profile_name , atooerp_type.name as name_type , atooerp_type.Id as type_id ,atooerp_type_element.Id as id_element ,\r\n  atooerp_type_element.name as type_element , atooerp_input_editors.name as editor_type" +
                " from marketing_profile_attribut_value\r\n  left join marketing_profile_instances  on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\n  left join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\n  left join marketing_profile_attribut on marketing_profile_attribut_value.attribut=marketing_profile_attribut.Id\r\n  left join atooerp_type on marketing_profile_attribut.attribut_type = atooerp_type.Id\r\n  left join atooerp_type_element on atooerp_type_element.id = marketing_profile_attribut_value.type\r\n\r\n\r\n  left join atooerp_input_editors on marketing_profile_attribut.editor = atooerp_input_editors.Id\r\n  where  marketing_profile_instances.Id = (select max(instance.Id) from marketing_profile_instances instance where instance.partner = " + id_partner + " );";
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);

            List<MyProfileAttrubutes> list = new List<MyProfileAttrubutes>();
            MySqlDataReader reader = cmd.ExecuteReader();



            while (reader.Read())
            {
                MyProfileAttrubutes attribute = new MyProfileAttrubutes();
                string str = Convert.ToString(reader["string_value"]);
                if (Convert.ToString(reader["editor_type"]) == "SearchLookUpEdit")
                {
                    if (Convert.ToString(reader["type_element"]) != "")
                    {
                        attribute = new MyProfileAttrubutes(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name_attribute"]), Convert.ToString(reader["label"]).ToUpper(), Convert.ToString(reader["name_type"]), Convert.ToString(reader["editor_type"]), true, Convert.ToString(reader["type_element"]), Convert.ToString(reader["profile_name"]), Convert.ToInt32(reader["type_id"]), Convert.ToInt32(reader["attribut_id"]),true);
                        attribute.HasList = true;
                        attribute.id_element = Convert.ToInt32(reader["id_element"]);
                    }
                    else
                    {
                        attribute = new MyProfileAttrubutes(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name_attribute"]), Convert.ToString(reader["label"]).ToUpper(), Convert.ToString(reader["name_type"]), Convert.ToString(reader["editor_type"]), true, Convert.ToString(reader["type_element"]), Convert.ToString(reader["profile_name"]), Convert.ToInt32(reader["type_id"]), Convert.ToInt32(reader["attribut_id"]),true);
                        attribute.HasList = true;

                    }

                }
                else if (Convert.ToString(reader["string_value"]) != null && Convert.ToString(reader["boolean_value"]) == "" && Convert.ToString(reader["date_value"]) == "" && Convert.ToString(reader["decimal_value"]) == "" && Convert.ToString(reader["int_value"]) == "" && Convert.ToString(reader["type_element"]) == "")
                {
                    attribute = new MyProfileAttrubutes(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name_attribute"]), Convert.ToString(reader["label"]).ToUpper(), Convert.ToString(reader["name_type"]), Convert.ToString(reader["editor_type"]), true, Convert.ToString(reader["string_value"]), Convert.ToString(reader["profile_name"]), Convert.ToInt32(reader["type_id"]), Convert.ToInt32(reader["attribut_id"]),true);
                    if (Convert.ToString(reader["editor_type"]).ToLower() == "memoedit")
                    {
                        attribute.HasMemo = true;

                    }
                    else
                    {
                        attribute.HasText = true;
                    }
                }
                else if (Convert.ToString(reader["boolean_value"]) != "")
                {
                    attribute = new MyProfileAttrubutes(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name_attribute"]), Convert.ToString(reader["label"]).ToUpper(), Convert.ToString(reader["name_type"]), Convert.ToString(reader["editor_type"]), true, Convert.ToString(reader["boolean_value"]), Convert.ToString(reader["profile_name"]), Convert.ToInt32(reader["type_id"]), Convert.ToInt32(reader["attribut_id"]), true);
                    attribute.HasCheck = true;
                }
                else if (Convert.ToString(reader["date_value"]) != "")
                {
                    attribute = new MyProfileAttrubutes(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name_attribute"]), Convert.ToString(reader["label"]).ToUpper(), Convert.ToString(reader["name_type"]), Convert.ToString(reader["editor_type"]), true, Convert.ToString(reader["date_value"]), Convert.ToString(reader["profile_name"]), Convert.ToInt32(reader["type_id"]), Convert.ToInt32(reader["attribut_id"]), true);
                    attribute.HasDate = true;
                }
                else if (Convert.ToString(reader["decimal_value"]) != "")
                {
                    attribute = new MyProfileAttrubutes(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name_attribute"]), Convert.ToString(reader["label"]).ToUpper(), Convert.ToString(reader["name_type"]), Convert.ToString(reader["editor_type"]), true, Convert.ToString(reader["decimal_value"]), Convert.ToString(reader["profile_name"]), Convert.ToInt32(reader["type_id"]), Convert.ToInt32(reader["attribut_id"]), true);
                    attribute.HasDecimal = true;
                }
                else if (Convert.ToString(reader["int_value"]) != "")
                {
                    attribute = new MyProfileAttrubutes(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name_attribute"]), Convert.ToString(reader["label"]).ToUpper(), Convert.ToString(reader["name_type"]), Convert.ToString(reader["editor_type"]), true, Convert.ToString(reader["int_value"]), Convert.ToString(reader["profile_name"]), Convert.ToInt32(reader["type_id"]), Convert.ToInt32(reader["attribut_id"]), true);
                    attribute.HasNumber = true;

                }
                else if (Convert.ToString(reader["blob_value"]) != "")
                {
                    attribute = new MyProfileAttrubutes(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name_attribute"]), Convert.ToString(reader["label"]).ToUpper(), Convert.ToString(reader["name_type"]), Convert.ToString(reader["editor_type"]), true, Convert.ToString(reader["blob_value"]), Convert.ToString(reader["profile_name"]), Convert.ToInt32(reader["type_id"]), Convert.ToInt32(reader["attribut_id"]), true);
                }
     
                if (Convert.ToString(attribute.id_type) == "")
                {
                    attribute.id_type = Convert.ToInt32(reader["type_id"]);

                }







                list.Add(attribute);



            }

            reader.Close();
            DbConnection.Deconnecter();

            foreach (MyProfileAttrubutes attributes in list)
            {
                attributes.tempValues = tempValue.getAllTempValurById(attributes.id);
                if (attributes.tempValues.Count > 0)
                {
                    attributes.HasTempValue = true;
                }
                if (attributes.HasList == true)
                {
                    attributes.listOfTypeElement = atooerp_element.getAtooerpElementByIdType(attributes.id_type, 0);
                }

            }



            return list;


        }
    }

    public class atooerp_element
    {
        public int id { get; set; }
        public string name { get; set; }

        public int id_type { get; set; }

        public int? parent { get; set; }

        public int Rank { get; set; }
        public atooerp_element()
        {

        }
        public atooerp_element(int id, string name, int id_type, int? parent)
        {
            this.id = id;
            this.name = name;
            this.id_type = id_type;
            this.parent = parent;


        }


        public static List<atooerp_element> getAtooerpElementByIdTypeForUpdate(int id, int rank)
        {
            List<atooerp_element> list = new List<atooerp_element>();
            try
            {
                string sqlCmd1 = "select atooerp_type_element.Id ,atooerp_type_element.parent, atooerp_type_element.name , atooerp_type_element.type_id  from atooerp_type_element where atooerp_type_element.type_id in ( select e.type_id from atooerp_type_element e where e.Id=" + id + " );";

                DbConnection.Deconnecter();
                DbConnection.Connecter();

                MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {

                        if (reader["parent"].ToString() == "")
                        {
                            atooerp_element element = new atooerp_element(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name"]), Convert.ToInt32(reader["type_id"]), null); element.Rank = rank;
                            list.Add(element);
                        }
                        else
                        {
                            atooerp_element element = new atooerp_element(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name"]), Convert.ToInt32(reader["type_id"]), Convert.ToInt32(reader["parent"])); element.Rank = rank; list.Add(element);
                        }


                    }
                    catch (Exception ex)
                    {

                    }

                }
                reader.Close();
                DbConnection.Deconnecter();




            }
            catch (Exception e)
            {


            }
            return list;







        }
        public static List<atooerp_element> getAtooerpElementByIdType(int id, int rank)
        {
            List<atooerp_element> list = new List<atooerp_element>();
            try
            {
                string sqlCmd1 = "select atooerp_type_element.Id ,atooerp_type_element.parent, atooerp_type_element.name , atooerp_type_element.type_id  from atooerp_type_element where atooerp_type_element.type_id = " + id + " ;";

                DbConnection.Deconnecter();
                DbConnection.Connecter();

                MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {

                        if (reader["parent"].ToString() == "")
                        {
                            atooerp_element element = new atooerp_element(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name"]), Convert.ToInt32(reader["type_id"]), null); element.Rank = rank;
                            list.Add(element);
                        }
                        else
                        {
                            atooerp_element element = new atooerp_element(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["name"]), Convert.ToInt32(reader["type_id"]), Convert.ToInt32(reader["parent"])); element.Rank = rank; list.Add(element);
                        }


                    }
                    catch (Exception ex)
                    {

                    }

                }
                reader.Close();
                DbConnection.Deconnecter();




            }
            catch (Exception e)
            {


            }
            return list;







        }
    }
    public class tempValue
    {
        public int id { get; set; }
        public string value { get; set; }
        public string create_date { get; set; }
        public string user { get; set; }
        public tempValue()
        {

        }
        public tempValue(int id, string value, string create_date, string user)
        {
            this.id = id;
            this.value = value;
            this.create_date = create_date;
            this.user = user;

        }
        public static List<tempValue> getAllTempValurById(int id)
        {

            string sqlCmd1 = "SELECT marketing_profile_attribut_value_temp.*  ,atooerp_type_element.name as element_type, concat(atooerp_person.first_name,\" \",atooerp_person.last_name) as user_name\r\nfrom marketing_profile_attribut_value_temp\r\n" +
                "left join atooerp_user on  " +
                " atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\n" +
                "left join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\n" +
                "left join atooerp_type_element on atooerp_type_element.Id = marketing_profile_attribut_value_temp.type\r\n" +
                "left join atooerp_person on atooerp_person.id= " + user_contrat.id_employe + "\r\n " +
                " where marketing_profile_attribut_value.Id= " + id + " and marketing_profile_attribut_value_temp.state=1 ;";
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);

            List<tempValue> list = new List<tempValue>();
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                tempValue attribute = new tempValue();
                string str = Convert.ToString(reader["string_value"]);
                if (Convert.ToString(reader["string_value"]) != null && Convert.ToString(reader["boolean_value"]) == "" && Convert.ToString(reader["date_value"]) == "" && Convert.ToString(reader["decimal_value"]) == "" && Convert.ToString(reader["int_value"]) == "" && Convert.ToString(reader["type"]) == "")
                {
                    attribute = new tempValue(Convert.ToInt32(reader["id"]), Convert.ToString(reader["string_value"]).ToUpper(), Convert.ToString(reader["create_date"]), Convert.ToString(reader["user_name"]));

                }
                else if (Convert.ToString(reader["boolean_value"]) != "")
                {
                    attribute = new tempValue(Convert.ToInt32(reader["id"]), Convert.ToString(reader["boolean_value"]).ToUpper(), Convert.ToString(reader["create_date"]), Convert.ToString(reader["user_name"]));
                }
                else if (Convert.ToString(reader["date_value"]) != "")
                {
                    attribute = new tempValue(Convert.ToInt32(reader["id"]), Convert.ToString(reader["date_value"]).ToUpper(), Convert.ToString(reader["create_date"]), Convert.ToString(reader["user_name"]));
                }
                else if (Convert.ToString(reader["decimal_value"]) != "")
                {
                    attribute = new tempValue(Convert.ToInt32(reader["id"]), Convert.ToString(reader["decimal_value"]).ToUpper(), Convert.ToString(reader["create_date"]), Convert.ToString(reader["user_name"]));
                }
                else if (Convert.ToString(reader["int_value"]) != "")
                {
                    attribute = new tempValue(Convert.ToInt32(reader["id"]), Convert.ToString(reader["int_value"]).ToUpper(), Convert.ToString(reader["create_date"]), Convert.ToString(reader["user_name"]));

                }
                else if (Convert.ToString(reader["blob_value"]) != "")
                {
                    attribute = new tempValue(Convert.ToInt32(reader["id"]), Convert.ToString(reader["blob_value"]).ToUpper(), Convert.ToString(reader["create_date"]), Convert.ToString(reader["user_name"]));
                }
                else if (Convert.ToString(reader["type"]) != "")
                {
                    attribute = new tempValue(Convert.ToInt32(reader["id"]), Convert.ToString(reader["element_type"]).ToUpper(), Convert.ToString(reader["create_date"]), Convert.ToString(reader["user_name"]));



                }




                list.Add(attribute);



            }

            reader.Close();
            DbConnection.Deconnecter();



            return list;


        }


    }
}

