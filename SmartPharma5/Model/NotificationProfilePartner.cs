using Acr.UserDialogs;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.View;

namespace SmartPharma5.Model
{
    public class NotificationProfilePartner
    {
        public int id_temp { get; set; }
        public string profile { get; set; }

        public string profile_user { get; set; }
        public string Temp_Value { get; set; }

        //public int id_category { get; set; }

        public string name_attribute { get; set; }

        public string current_value { get; set; }

        public string value_temp { get; set; }

        public string create_date { get; set; }

        public string created_by { get; set; }

        public int id_profile { get; set; }

        public bool HasAccepted { get; set; } = false;

        public bool HasRefused { get; set; } = false;

        public bool HasNoState { get; set; } = false;

        public int id_partner { get; set; }
        public int id_current_value { get; set; }

        public int state { get; set; }

        public int id_instance { get; set; }

        public int id_type { get; set; }
        public int id_current_type { get; set; }


        public bool IsPartnerAttribute { get; set; }

        public AsyncCommand AcceptTempValue { get; set; }
        public AsyncCommand ReffuseTempValue { get; set; }
        public AsyncCommand changeToProfil { get; set; }

        public NotificationProfilePartner()
        {
            AcceptTempValue = new AsyncCommand(Accept_profile_values);
            ReffuseTempValue = new AsyncCommand(Refuse_profile_values);
            changeToProfil = new AsyncCommand(Détails);

        }
        private async Task Détails()
        {
            // await App.Current.MainPage.Navigation.PushAsync(new View.ProfileView(this.id_partner));
        }
        public async Task insertNewInstance(int id_partner, int id_profile)
        {

            string sqlcmd = " insert into marketing_profile_instances(partner,create_date,profil) values (" + id_partner + ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ," + id_profile + ") ;";
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlcmd, DbConnection.con);



            /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
            Avant :
                        cmd.ExecuteReader();


                        DbConnection.Deconnecter();
            Après :
                        cmd.ExecuteReader();


                        DbConnection.Deconnecter();
            */
            cmd.ExecuteReader();


            DbConnection.Deconnecter();

        }

        public async Task<List<values>> getValuesByProfile(int instance_profile)
        {
            string sqlcmd = "select marketing_profile_attribut_value.* , atooerp_type.id as type_attribute  " +
                "from marketing_profile_attribut_value\r\n" +
                "left join marketing_profile_attribut on marketing_profile_attribut.id = marketing_profile_attribut_value.attribut\r\n" +
                "left join atooerp_type on atooerp_type.id = marketing_profile_attribut.attribut_type " +
                "where marketing_profile_attribut_value.profile_instance = " + instance_profile + " ;";
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlcmd, DbConnection.con);

            List<values> list = new List<values>();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {

                values attribute = new values();
                attribute.id = Convert.ToInt32(reader["Id"]);
                if (Convert.ToString(reader["create_date"]) == "")
                {
                    attribute.create_date = DateTime.Now;
                }
                else
                {
                    attribute.create_date = Convert.ToDateTime(reader["create_date"]);
                }

                attribute.name = Convert.ToString(reader["name"]); ;
                attribute.memo = Convert.ToString(reader["memo"]);
                attribute.attribut_type= Convert.ToInt32(reader["type_attribute"]);

                /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                Avant :
                                attribute.attribute = Convert.ToInt32(reader["attribut"]);



                                if (Convert.ToString(reader["string_value"]) != null && Convert.ToString(reader["boolean_value"]) == "" && Convert.ToString(reader["date_value"]) == "" && Convert.ToString(reader["decimal_value"]) == "" && Convert.ToString(reader["int_value"]) == "" && Convert.ToString(reader["type"]) == "")
                                {

                                    if (Convert.ToInt32(reader["Id"]) == this.id_current_value)
                Après :
                                attribute.attribute = Convert.ToInt32(reader["attribut"]);



                                if (Convert.ToString(reader["string_value"]) != null && Convert.ToString(reader["boolean_value"]) == "" && Convert.ToString(reader["date_value"]) == "" && Convert.ToString(reader["decimal_value"]) == "" && Convert.ToString(reader["int_value"]) == "" && Convert.ToString(reader["type"]) == "")
                                {

                                    if (Convert.ToInt32(reader["Id"]) == this.id_current_value)
                */
                attribute.attribute = Convert.ToInt32(reader["attribut"]);



                if (attribute.attribut_type==1)
                {

                    if (Convert.ToInt32(reader["Id"]) == this.id_current_value)
                    {
                        attribute.string_value = this.Temp_Value;
                    }
                    else
                    {
                        attribute.string_value = Convert.ToString(reader["string_value"]);
                    }


                }
                else if (attribute.attribut_type == 4)
                {

                    if (Convert.ToInt32(reader["Id"]) == this.id_current_value)
                    {
                        attribute.boolean_value = Convert.ToBoolean(this.Temp_Value);

                    }
                    else
                    {
                        attribute.boolean_value = Convert.ToBoolean(reader["boolean_value"]);
                    }


                }
                else if (attribute.attribut_type == 6)
                {

                    if (Convert.ToInt32(reader["Id"]) == this.id_current_value)
                    {
                        attribute.date_value = Convert.ToDateTime(this.Temp_Value);
                    }
                    else
                    {
                        attribute.date_value = Convert.ToDateTime(reader["date_value"]);
                    }



                }
                else if (attribute.attribut_type == 3)
                {

                    if (Convert.ToInt32(reader["Id"]) == this.id_current_value)
                    {
                        attribute.decimal_value = Convert.ToDecimal(this.Temp_Value);
                    }
                    else
                    {
                        attribute.decimal_value = Convert.ToDecimal(reader["decimal_value"]);

                    }


                }
                else if (attribute.attribut_type == 2)
                {

                    if (Convert.ToInt32(reader["Id"]) == this.id_current_value)
                    {
                        attribute.int_value = Convert.ToInt32(this.Temp_Value);
                    }
                    else
                    {

                        attribute.int_value = Convert.ToInt32(reader["int_value"]);

                    }


                }
                else if (attribute.attribut_type == 5)
                {

                    if (Convert.ToInt32(reader["Id"]) == this.id_current_value)
                    {
                        attribute.bloob_value = Convert.ToByte(this.Temp_Value);
                    }
                    else
                    {
                        attribute.bloob_value = Convert.ToByte(reader["blob_value"]);

                    }

                }
                else
                {
                    var a = Convert.ToInt32(reader["Id"]);

                    if (Convert.ToInt32(reader["Id"]) == this.id_current_value)
                    {
                        attribute.type = Convert.ToInt32(id_type);
                    }
                    else
                    {
                        try
                        {
                            if (Convert.ToString(reader["type"])=="")
                            {
                                attribute.type = null;

                            }
                            else
                            {
                                attribute.type = Convert.ToInt32(reader["type"]);

                            }
                           
                        }
                        catch (Exception ex) 
                        { 
                        }
                        
                    }

                }
                list.Add(attribute);

            }
            reader.Close();
            DbConnection.Deconnecter();
            return list;

        }








        public async Task<int> getLastIdInstance()
        {

            string sqlcmd = "select max(marketing_profile_instances.Id) as number from marketing_profile_instances ;";
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlcmd, DbConnection.con);

            List<NotificationProfilePartner> list = new List<NotificationProfilePartner>();
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            return Convert.ToInt32(reader["number"]);
            // reader.Close

        }

        public async Task<int> getLastIdValue()
        {

            string sqlcmd = "select max(marketing_profile_attribut_value.Id) as number from marketing_profile_attribut_value ;";
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlcmd, DbConnection.con);

            List<NotificationProfilePartner> list = new List<NotificationProfilePartner>();
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            return Convert.ToInt32(reader["number"]);
            // reader.Close

        }
        public async Task Accept_profile_values()
        {

            if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO ACCEPT CHANGES", "YES", "NO"))
            {
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                if (this.IsPartnerAttribute)
                {
                    await this.insertNewInstance(this.id_partner, this.id_profile);
                    int last_instance = await this.getLastIdInstance();
                    await this.updateStatTempValue(id_temp, 2);
                    List<values> values = await this.getValuesByProfile(this.id_instance);
                    foreach (values val in values)
                    {
                        string sqlCmd = "";
                        DbConnection.Deconnecter();
                        DbConnection.Connecter();
                        if (val.attribut_type==1)
                        {
                            sqlCmd = "insert into marketing_profile_attribut_value(name,memo,create_date,attribut,profile_instance,string_value) values ('" + val.name + "' , '" + val.memo + "' , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + val.attribute + " ," +
                                " " + last_instance + " , '" + val.string_value + "');update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + ";";
                            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                            cmd.ExecuteReader();
                            DbConnection.Deconnecter();
                            int lastValue = await getLastIdValue();
                            await updateTempValue(val.id, lastValue);


                        }
                        else if (val.attribut_type == 2)
                        {
                            sqlCmd = "insert into marketing_profile_attribut_value(name,memo,create_date,attribut,profile_instance,int_value) values ('" + val.name + "' , '" + val.memo + "' , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + val.attribute + " ," +
                               " " + last_instance + " , " + val.int_value + ");update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + ";";
                            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                            cmd.ExecuteReader();
                            DbConnection.Deconnecter();
                            int lastValue = await getLastIdValue();
                            await updateTempValue(val.id, lastValue);
                        }
                        else if (val.attribut_type == 3)
                        {
                            sqlCmd = "insert into marketing_profile_attribut_value(name,memo,create_date,attribut,profile_instance,decimal_value) values ('" + val.name + "' , '" + val.memo + "' , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + val.attribute + " ," +
                               " " + last_instance + " , " + val.decimal_value.ToString().Replace(',', '.') + ");update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + ";";
                            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                            cmd.ExecuteReader();
                            DbConnection.Deconnecter();
                            int lastValue = await getLastIdValue();
                            await updateTempValue(val.id, lastValue);
                        }
                        else if (val.attribut_type == 5)
                        {
                            sqlCmd = "insert into marketing_profile_attribut_value(name,memo,create_date,attribut,profile_instance,bloob_value) values ('" + val.name + "' , '" + val.memo + "' , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + val.attribute + " ," +
                               " " + last_instance + " , '" + val.bloob_value + "');update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + ";";
                            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                            cmd.ExecuteReader();
                            DbConnection.Deconnecter();
                            int lastValue = await getLastIdValue();
                            await updateTempValue(val.id, lastValue);
                        }
                        else if (val.attribut_type == 4)
                        {
                            sqlCmd = "insert into marketing_profile_attribut_value(name,memo,create_date,attribut,profile_instance,boolean_value) values ('" + val.name + "' , '" + val.memo + "' , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + val.attribute + " ," +
                               " " + last_instance + " , " + val.boolean_value + ");update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + ";";
                            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                            cmd.ExecuteReader();
                            DbConnection.Deconnecter();
                            int lastValue = await getLastIdValue();
                            await updateTempValue(val.id, lastValue);
                        }
                        else if (val.attribut_type==6)
                        {
                            sqlCmd = "insert into marketing_profile_attribut_value(name,memo,create_date,attribut,profile_instance,date_value) values ('" + val.name + "' , '" + val.memo + "' , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + val.attribute + " ," +
                               " " + last_instance + " , '" + val.date_value?.ToString("yyyy-MM-dd HH:mm:ss") + "');update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + ";";
                            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                            cmd.ExecuteReader();
                            DbConnection.Deconnecter();
                            int lastValue = await getLastIdValue();
                            await updateTempValue(val.id, lastValue);
                        }
                        else 
                        {
                            if (val.type == null)
                            {
                                sqlCmd = "insert into marketing_profile_attribut_value(name,memo,create_date,attribut,profile_instance,type) values ('" + val.name + "' , '" + val.memo + "' , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + val.attribute + " ," +
                            " " + last_instance + " , null);update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + ";";

                            }
                            else
                            {
                                sqlCmd = "insert into marketing_profile_attribut_value(name,memo,create_date,attribut,profile_instance,type) values ('" + val.name + "' , '" + val.memo + "' , '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , " + val.attribute + " ," +
                            " " + last_instance + " , " + val.type + ");update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + ";";

                            }
                         
                            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                            cmd.ExecuteReader();
                            DbConnection.Deconnecter();
                            int lastValue = await getLastIdValue();
                            await updateTempValue(val.id, lastValue);
                        }
                    
                   


                    }
                }
                else
                {
                    string sqlCmd = "";
                    if (this.name_attribute.ToLower() == "name")
                    {
                        sqlCmd = "update commercial_partner set name = '" + Temp_Value + "'  where id = " + id_partner + ";update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + "; ";


                    }
                    else if (this.name_attribute.ToLower() == "category")
                    {
                        sqlCmd = "update commercial_partner set category = " + Convert.ToInt32(value_temp) + "  where id = " + id_partner + ";update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + "; ";
                    }
                    else if (this.name_attribute.ToLower() == "number")
                    {
                        sqlCmd = "update commercial_partner set number = '" + Temp_Value + "'  where id = " + id_partner + ";update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + "; ";



                    }
                    else if (this.name_attribute.ToLower() == "street")
                    {
                        sqlCmd = "update commercial_partner set street = '" + Temp_Value + "'  where id = " + id_partner + ";update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + "; ";


                    }
                    else if (this.name_attribute.ToLower() == "city")
                    {
                        sqlCmd = "update commercial_partner set city = '" + Temp_Value + "'  where id = " + id_partner + ";update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + "; ";


                    }
                    else if (this.name_attribute.ToLower() == "state")
                    {
                        sqlCmd = "update commercial_partner set state = '" + Temp_Value + "'  where id = " + id_partner + ";update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + "; ";


                    }
                    else if (this.name_attribute.ToLower() == "country")
                    {
                        sqlCmd = "update commercial_partner set country = '" + Temp_Value + "'  where id = " + id_partner + ";update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + "; ";


                    }
                    else if (this.name_attribute.ToLower() == "postal_code")
                    {
                        sqlCmd = "update commercial_partner set postal_code = '" + Temp_Value + "'  where id = " + id_partner + ";update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + "; ";


                    }
                    else if (this.name_attribute.ToLower() == "email")
                    {
                        sqlCmd = "update commercial_partner set email = '" + Temp_Value + "'  where id = " + id_partner + ";update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + "; ";


                    }
                    else if (this.name_attribute.ToLower() == "fax")
                    {
                        sqlCmd = "update commercial_partner set fax = '" + Temp_Value + "' where id = " + id_partner + ";update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + "; ";
                    }
                    else if (this.name_attribute.ToLower() == "customer")
                    {
                        sqlCmd = "update commercial_partner set customer = " + Temp_Value + "  where id = " + id_partner + ";update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + "; ";


                    }
                    else if (this.name_attribute.ToLower() == "supplier")
                    {
                        sqlCmd = "update commercial_partner set supplier = " + Temp_Value + "  where id = " + id_partner + ";update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + "; ";


                    }
                    else if (this.name_attribute.ToLower() == "vat_code")
                    {
                        sqlCmd = "update commercial_partner set vat_code = '" + Temp_Value + "'  where id = " + id_partner + ";update marketing_profile_attribut_value_temp set state = 2 where id =" + id_temp + "; ";


                    }
                    await App.Current.MainPage.DisplayAlert("DONE", "UPDATED SUCCEFULY!", "OK");

                    DbConnection.Connecter();
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    cmd.ExecuteReader();


                }
            }


            
            await App.Current.MainPage.Navigation.PushAsync(new TabPageValidationUpdates());
            UserDialogs.Instance.HideLoading();







        }
        public async Task Refuse_profile_values()
        {
            if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO REFUSE THIS REQUEST", "YES", "NO"))
            {
                await this.updateStatTempValue(id_temp, 3);
                string sqlCmd = "update marketing_profile_attribut_value_temp set state = 3 where id =" + id_temp + "; ";
                try
                {
                    DbConnection.Connecter();
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    cmd.ExecuteReader();


                }
                catch (Exception ex)
                {

                }


                await App.Current.MainPage.DisplayAlert("DONE", "CHANGE REFUSED SUCCEFULY!", "OK");
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                await App.Current.MainPage.Navigation.PushAsync(new TabPageValidationUpdates());
                UserDialogs.Instance.HideLoading();
            }






        }

        public async Task updateTempValue(int current_value, int new_value)
        {

            string sqlcmd = "update marketing_profile_attribut_value_temp set attribut_value = " + new_value + "  where attribut_value = " + current_value + " ;";
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlcmd, DbConnection.con);
            cmd.ExecuteReader();
            DbConnection.Deconnecter();

        }


        public async Task updateStatTempValue(int id_temp, int state)
        {
            string sqlcmd = "update marketing_profile_attribut_value_temp set state = " + state + " where id = " + id_temp + " ;";
            DbConnection.Deconnecter();
            DbConnection.Connecter();
            MySqlCommand cmd = new MySqlCommand(sqlcmd, DbConnection.con);
            cmd.ExecuteReader();
            DbConnection.Deconnecter();
        }
        
        public static List<NotificationProfilePartner> getMyHistoryTempValues()
        {
            int id = user_contrat.id_employe;
            //string  sqlcmd = "SELECT marketing_profile_attribut_value_temp.*  ,marketing_profile_attribut.label as label_attribut , marketing_profile.Id as id_profile,com2.name as name_partner2 ,com2.*,com2.name as name_partner, com2.state as state_name,\r\nmarketing_profile_attribut_value.Id as id_value ,marketing_profile_instances.Id id_instance, commercial_partner.Id as id_partner ,current_category.name as category_current ,\r\ncommercial_partner.name as partner_name , concat(person.first_name,' ',person.last_name) as user_name , marketing_profile.name as profile_name,temp_category.name as category_temp ,\r\nmarketing_profile_attribut_value.string_value as string_value_current, marketing_profile_attribut_value.boolean_value as boolean_value_current ,\r\nmarketing_profile_attribut_value.date_value as date_value_current , marketing_profile_attribut_value.int_value  as int_value_current ,\r\nmarketing_profile_attribut_value.decimal_value decimal_value_current , marketing_profile_attribut_value.blob_value as blob_value_current ,element1.name as name_type,element2.name as name_current_type,\r\nmarketing_profile_attribut_value.type as type_current from marketing_profile_attribut_value_temp\r\nleft join atooerp_user on   atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\nleft join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\nleft join marketing_profile_attribut on marketing_profile_attribut_value.attribut= marketing_profile_attribut.Id\r\nleft join marketing_profile_instances on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\nleft join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\nleft join commercial_partner on marketing_profile_instances.partner=commercial_partner.Id\r\nleft join commercial_partner as com2 on marketing_profile_attribut_value_temp.partner=com2.Id\r\nleft join commercial_partner_category as current_category on com2.category=current_category.id\r\nleft join atooerp_type_element as element1 on marketing_profile_attribut_value_temp.type = element1.id\r\nleft join atooerp_type_element as element2 on marketing_profile_attribut_value.type = element2.id\r\nleft join atooerp_person as person on person.id="+user_contrat.id_employe+ "   \r\n\r\nleft join commercial_partner_category as temp_category on marketing_profile_attribut_value_temp.int_value=temp_category.id\r\nwhere marketing_profile_attribut_value_temp.profile_instance_temp is null and  marketing_profile_attribut_value_temp.partner_temp is null\r\nand marketing_profile_attribut_value_temp.state = 1\r\nand (marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance  group by instance.partner) or marketing_profile_attribut_value_temp.attribut_name is not null) order by marketing_profile_attribut_value_temp.create_date desc;";
            string sqlcmd = "SELECT marketing_profile_attribut_value_temp.*  ,marketing_profile_attribut.label as label_attribut , marketing_profile.Id as id_profile,com2.name as name_partner2 ,com2.*,com2.name as name_partner, com2.state as state_name,\r\nmarketing_profile_attribut_value.Id as id_value ,marketing_profile_instances.Id id_instance, commercial_partner.Id as id_partner ,current_category.name as category_current ,\r\ncommercial_partner.name as partner_name , concat(person.first_name,' ',person.last_name) as user_name , marketing_profile.name as profile_name,temp_category.name as category_temp ,\r\nmarketing_profile_attribut_value.string_value as string_value_current, marketing_profile_attribut_value.boolean_value as boolean_value_current ,\r\nmarketing_profile_attribut_value.date_value as date_value_current , marketing_profile_attribut_value.int_value  as int_value_current ,\r\nmarketing_profile_attribut_value.decimal_value decimal_value_current , marketing_profile_attribut_value.blob_value as blob_value_current ,element1.name as name_type,element2.name as name_current_type,\r\nmarketing_profile_attribut_value.type as type_current from marketing_profile_attribut_value_temp\r\nleft join atooerp_user on   atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\nleft join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\nleft join marketing_profile_attribut on marketing_profile_attribut_value.attribut= marketing_profile_attribut.Id\r\nleft join marketing_profile_instances on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\nleft join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\nleft join commercial_partner on marketing_profile_instances.partner=commercial_partner.Id\r\nleft join commercial_partner as com2 on marketing_profile_attribut_value_temp.partner=com2.Id\r\nleft join commercial_partner_category as current_category on com2.category=current_category.id\r\nleft join atooerp_type_element as element1 on marketing_profile_attribut_value_temp.type = element1.id\r\nleft join atooerp_type_element as element2 on marketing_profile_attribut_value.type = element2.id\r\nleft join atooerp_person as person on person.id=marketing_profile_attribut_value_temp.employe\r\n\r\nleft join commercial_partner_category as temp_category on marketing_profile_attribut_value_temp.int_value=temp_category.id\r\nwhere marketing_profile_attribut_value_temp.employe="+id+" and marketing_profile_attribut_value_temp.profile_instance_temp is null and  marketing_profile_attribut_value_temp.partner_temp is null\r\nand (marketing_profile_attribut_value_temp.state = 2 or marketing_profile_attribut_value_temp.state = 3 )\r\nand (marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance  group by instance.partner) or marketing_profile_attribut_value_temp.attribut_name is not null) order by marketing_profile_attribut_value_temp.create_date desc;";
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlcmd, DbConnection.con);

            List<NotificationProfilePartner> list = new List<NotificationProfilePartner>();
            MySqlDataReader reader = cmd.ExecuteReader();



            while (reader.Read())
            {
                try
                {
                    NotificationProfilePartner attribute = new NotificationProfilePartner();
                    if (reader["attribut_name"].ToString() == "")
                    {
                        attribute.IsPartnerAttribute = true;
                        attribute.id_temp = Convert.ToInt32(reader["id"]);
                        attribute.created_by = Convert.ToString(reader["user_name"]);
                        attribute.profile = Convert.ToString(reader["profile_name"]);
                        attribute.profile_user = Convert.ToString(reader["partner_name"]);
                        attribute.create_date = Convert.ToString(reader["create_date"]);
                        attribute.id_partner = Convert.ToInt32(reader["id_partner"]);
                        attribute.id_profile = Convert.ToInt32(reader["id_profile"]);
                        attribute.id_current_value = Convert.ToInt32(reader["id_value"]);
                        attribute.state = Convert.ToInt32(reader["state"]);
                        attribute.name_attribute = Convert.ToString(reader["label_attribut"]).ToUpper();
                        attribute.id_instance = Convert.ToInt32(reader["id_instance"]);
                    }
                    else
                    {
                        attribute.IsPartnerAttribute = false;
                        attribute.id_temp = Convert.ToInt32(reader["id"]);
                        attribute.created_by = Convert.ToString(reader["user_name"]).ToUpper();
                        attribute.profile = Convert.ToString(reader["profile_name"]);
                        attribute.profile_user = Convert.ToString(reader["name_partner2"]);
                        attribute.create_date = Convert.ToString(reader["create_date"]);
                        attribute.id_partner = Convert.ToInt32(reader["partner"]);
                        attribute.state = Convert.ToInt32(reader["state"]);
                        attribute.name_attribute = Convert.ToString(reader["attribut_name"]).ToUpper();


                    }

                    if (Convert.ToString(reader["state"]) == "2")
                    {
                        attribute.HasAccepted = true;

                    }
                    else if (Convert.ToString(reader["state"]) == "3")
                    {
                        attribute.HasRefused = true;

                    }
                    else
                    {
                        attribute.HasNoState = true;
                    }


                    if (attribute.IsPartnerAttribute)
                    {
                        if (Convert.ToString(reader["string_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["string_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);

                        }
                        else if (Convert.ToString(reader["boolean_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["boolean_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["boolean_value"]);

                        }
                        else if (Convert.ToString(reader["date_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["date_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["date_value"]);

                        }
                        else if (Convert.ToString(reader["decimal_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["decimal_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["decimal_value"]);
                        }
                        else if (Convert.ToString(reader["int_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["int_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["int_value"]);

                        }
                        else if (Convert.ToString(reader["blob_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["blob_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["blob_value"]);
                        }
                        else if (Convert.ToString(reader["type"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["name_current_type"]);
                            attribute.Temp_Value = Convert.ToString(reader["name_type"]);
                            attribute.id_type = Convert.ToInt32(reader["type"]);
                            attribute.id_current_type = Convert.ToInt32(reader["type_current"]);

                        }

                    }
                    else
                    {
                        if (Convert.ToString(reader["attribut_name"]) == "name")
                        {
                            attribute.current_value = Convert.ToString(reader["name_partner"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);


                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "category")
                        {
                            attribute.current_value = Convert.ToString(reader["category_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["category_temp"]);
                            attribute.value_temp = Convert.ToString(reader["int_value"]);

                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "number")
                        {
                            attribute.current_value = Convert.ToString(reader["number"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);

                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "street")
                        {
                            attribute.current_value = Convert.ToString(reader["street"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "city")
                        {
                            attribute.current_value = Convert.ToString(reader["city"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "state")
                        {
                            attribute.current_value = Convert.ToString(reader["state_name"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "country")
                        {
                            attribute.current_value = Convert.ToString(reader["country"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "postal_code")
                        {
                            attribute.current_value = Convert.ToString(reader["postal_code"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "email")
                        {
                            attribute.current_value = Convert.ToString(reader["email"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "fax")
                        {
                            attribute.current_value = Convert.ToString(reader["fax"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "customer")
                        {
                            attribute.current_value = Convert.ToString(reader["customer"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "supplier")
                        {
                            attribute.current_value = Convert.ToString(reader["supplier"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "vat_code")
                        {
                            attribute.current_value = Convert.ToString(reader["vat_code"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }


                    }


                    list.Add(attribute);

                }
                catch (Exception ex)
                {

                }




            }
            reader.Close();
            DbConnection.Deconnecter();
            return list;
        }
        public static List<NotificationProfilePartner> getMyAccepteHistoryTempValues()
        {
            int id = user_contrat.id_employe;
            //string  sqlcmd = "SELECT marketing_profile_attribut_value_temp.*  ,marketing_profile_attribut.label as label_attribut , marketing_profile.Id as id_profile,com2.name as name_partner2 ,com2.*,com2.name as name_partner, com2.state as state_name,\r\nmarketing_profile_attribut_value.Id as id_value ,marketing_profile_instances.Id id_instance, commercial_partner.Id as id_partner ,current_category.name as category_current ,\r\ncommercial_partner.name as partner_name , concat(person.first_name,' ',person.last_name) as user_name , marketing_profile.name as profile_name,temp_category.name as category_temp ,\r\nmarketing_profile_attribut_value.string_value as string_value_current, marketing_profile_attribut_value.boolean_value as boolean_value_current ,\r\nmarketing_profile_attribut_value.date_value as date_value_current , marketing_profile_attribut_value.int_value  as int_value_current ,\r\nmarketing_profile_attribut_value.decimal_value decimal_value_current , marketing_profile_attribut_value.blob_value as blob_value_current ,element1.name as name_type,element2.name as name_current_type,\r\nmarketing_profile_attribut_value.type as type_current from marketing_profile_attribut_value_temp\r\nleft join atooerp_user on   atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\nleft join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\nleft join marketing_profile_attribut on marketing_profile_attribut_value.attribut= marketing_profile_attribut.Id\r\nleft join marketing_profile_instances on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\nleft join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\nleft join commercial_partner on marketing_profile_instances.partner=commercial_partner.Id\r\nleft join commercial_partner as com2 on marketing_profile_attribut_value_temp.partner=com2.Id\r\nleft join commercial_partner_category as current_category on com2.category=current_category.id\r\nleft join atooerp_type_element as element1 on marketing_profile_attribut_value_temp.type = element1.id\r\nleft join atooerp_type_element as element2 on marketing_profile_attribut_value.type = element2.id\r\nleft join atooerp_person as person on person.id="+user_contrat.id_employe+ "   \r\n\r\nleft join commercial_partner_category as temp_category on marketing_profile_attribut_value_temp.int_value=temp_category.id\r\nwhere marketing_profile_attribut_value_temp.profile_instance_temp is null and  marketing_profile_attribut_value_temp.partner_temp is null\r\nand marketing_profile_attribut_value_temp.state = 1\r\nand (marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance  group by instance.partner) or marketing_profile_attribut_value_temp.attribut_name is not null) order by marketing_profile_attribut_value_temp.create_date desc;";
            string sqlcmd = "SELECT marketing_profile_attribut_value_temp.*  ,marketing_profile_attribut.label as label_attribut , marketing_profile.Id as id_profile,com2.name as name_partner2 ,com2.*,com2.name as name_partner, com2.state as state_name,\r\nmarketing_profile_attribut_value.Id as id_value ,marketing_profile_instances.Id id_instance, commercial_partner.Id as id_partner ,current_category.name as category_current ,\r\ncommercial_partner.name as partner_name , concat(person.first_name,' ',person.last_name) as user_name , marketing_profile.name as profile_name,temp_category.name as category_temp ,\r\nmarketing_profile_attribut_value.string_value as string_value_current, marketing_profile_attribut_value.boolean_value as boolean_value_current ,\r\nmarketing_profile_attribut_value.date_value as date_value_current , marketing_profile_attribut_value.int_value  as int_value_current ,\r\nmarketing_profile_attribut_value.decimal_value decimal_value_current , marketing_profile_attribut_value.blob_value as blob_value_current ,element1.name as name_type,element2.name as name_current_type,\r\nmarketing_profile_attribut_value.type as type_current from marketing_profile_attribut_value_temp\r\nleft join atooerp_user on   atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\nleft join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\nleft join marketing_profile_attribut on marketing_profile_attribut_value.attribut= marketing_profile_attribut.Id\r\nleft join marketing_profile_instances on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\nleft join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\nleft join commercial_partner on marketing_profile_instances.partner=commercial_partner.Id\r\nleft join commercial_partner as com2 on marketing_profile_attribut_value_temp.partner=com2.Id\r\nleft join commercial_partner_category as current_category on com2.category=current_category.id\r\nleft join atooerp_type_element as element1 on marketing_profile_attribut_value_temp.type = element1.id\r\nleft join atooerp_type_element as element2 on marketing_profile_attribut_value.type = element2.id\r\nleft join atooerp_person as person on person.id=marketing_profile_attribut_value_temp.employe\r\n\r\nleft join commercial_partner_category as temp_category on marketing_profile_attribut_value_temp.int_value=temp_category.id\r\nwhere marketing_profile_attribut_value_temp.employe=" + id + " and marketing_profile_attribut_value_temp.profile_instance_temp is null and  marketing_profile_attribut_value_temp.partner_temp is null\r\nand (marketing_profile_attribut_value_temp.state = 2  )\r\nand (marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance  group by instance.partner) or marketing_profile_attribut_value_temp.attribut_name is not null) order by marketing_profile_attribut_value_temp.create_date desc;";
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlcmd, DbConnection.con);

            List<NotificationProfilePartner> list = new List<NotificationProfilePartner>();
            MySqlDataReader reader = cmd.ExecuteReader();



            while (reader.Read())
            {
                try
                {
                    NotificationProfilePartner attribute = new NotificationProfilePartner();
                    if (reader["attribut_name"].ToString() == "")
                    {
                        attribute.IsPartnerAttribute = true;
                        attribute.id_temp = Convert.ToInt32(reader["id"]);
                        attribute.created_by = Convert.ToString(reader["user_name"]);
                        attribute.profile = Convert.ToString(reader["profile_name"]);
                        attribute.profile_user = Convert.ToString(reader["partner_name"]);
                        attribute.create_date = Convert.ToString(reader["create_date"]);
                        attribute.id_partner = Convert.ToInt32(reader["id_partner"]);
                        attribute.id_profile = Convert.ToInt32(reader["id_profile"]);
                        attribute.id_current_value = Convert.ToInt32(reader["id_value"]);
                        attribute.state = Convert.ToInt32(reader["state"]);
                        attribute.name_attribute = Convert.ToString(reader["label_attribut"]).ToUpper();
                        attribute.id_instance = Convert.ToInt32(reader["id_instance"]);
                    }
                    else
                    {
                        attribute.IsPartnerAttribute = false;
                        attribute.id_temp = Convert.ToInt32(reader["id"]);
                        attribute.created_by = Convert.ToString(reader["user_name"]).ToUpper();
                        attribute.profile = Convert.ToString(reader["profile_name"]);
                        attribute.profile_user = Convert.ToString(reader["name_partner2"]);
                        attribute.create_date = Convert.ToString(reader["create_date"]);
                        attribute.id_partner = Convert.ToInt32(reader["partner"]);
                        attribute.state = Convert.ToInt32(reader["state"]);
                        attribute.name_attribute = Convert.ToString(reader["attribut_name"]).ToUpper();


                    }

                    if (Convert.ToString(reader["state"]) == "2")
                    {
                        attribute.HasAccepted = true;

                    }
                    else if (Convert.ToString(reader["state"]) == "3")
                    {
                        attribute.HasRefused = true;

                    }
                    else
                    {
                        attribute.HasNoState = true;
                    }


                    if (attribute.IsPartnerAttribute)
                    {
                        if (Convert.ToString(reader["string_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["string_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);

                        }
                        else if (Convert.ToString(reader["boolean_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["boolean_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["boolean_value"]);

                        }
                        else if (Convert.ToString(reader["date_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["date_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["date_value"]);

                        }
                        else if (Convert.ToString(reader["decimal_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["decimal_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["decimal_value"]);
                        }
                        else if (Convert.ToString(reader["int_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["int_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["int_value"]);

                        }
                        else if (Convert.ToString(reader["blob_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["blob_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["blob_value"]);
                        }
                        else if (Convert.ToString(reader["type"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["name_current_type"]);
                            attribute.Temp_Value = Convert.ToString(reader["name_type"]);
                            attribute.id_type = Convert.ToInt32(reader["type"]);
                            attribute.id_current_type = Convert.ToInt32(reader["type_current"]);

                        }

                    }
                    else
                    {
                        if (Convert.ToString(reader["attribut_name"]) == "name")
                        {
                            attribute.current_value = Convert.ToString(reader["name_partner"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);


                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "category")
                        {
                            attribute.current_value = Convert.ToString(reader["category_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["category_temp"]);
                            attribute.value_temp = Convert.ToString(reader["int_value"]);

                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "number")
                        {
                            attribute.current_value = Convert.ToString(reader["number"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);

                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "street")
                        {
                            attribute.current_value = Convert.ToString(reader["street"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "city")
                        {
                            attribute.current_value = Convert.ToString(reader["city"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "state")
                        {
                            attribute.current_value = Convert.ToString(reader["state_name"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "country")
                        {
                            attribute.current_value = Convert.ToString(reader["country"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "postal_code")
                        {
                            attribute.current_value = Convert.ToString(reader["postal_code"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "email")
                        {
                            attribute.current_value = Convert.ToString(reader["email"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "fax")
                        {
                            attribute.current_value = Convert.ToString(reader["fax"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "customer")
                        {
                            attribute.current_value = Convert.ToString(reader["customer"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "supplier")
                        {
                            attribute.current_value = Convert.ToString(reader["supplier"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "vat_code")
                        {
                            attribute.current_value = Convert.ToString(reader["vat_code"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }


                    }


                    list.Add(attribute);

                }
                catch (Exception ex)
                {

                }




            }
            reader.Close();
            DbConnection.Deconnecter();
            return list;
        }
        public static List<NotificationProfilePartner> getMyRefuseHistoryTempValues()
        {
            int id = user_contrat.id_employe;
            //string  sqlcmd = "SELECT marketing_profile_attribut_value_temp.*  ,marketing_profile_attribut.label as label_attribut , marketing_profile.Id as id_profile,com2.name as name_partner2 ,com2.*,com2.name as name_partner, com2.state as state_name,\r\nmarketing_profile_attribut_value.Id as id_value ,marketing_profile_instances.Id id_instance, commercial_partner.Id as id_partner ,current_category.name as category_current ,\r\ncommercial_partner.name as partner_name , concat(person.first_name,' ',person.last_name) as user_name , marketing_profile.name as profile_name,temp_category.name as category_temp ,\r\nmarketing_profile_attribut_value.string_value as string_value_current, marketing_profile_attribut_value.boolean_value as boolean_value_current ,\r\nmarketing_profile_attribut_value.date_value as date_value_current , marketing_profile_attribut_value.int_value  as int_value_current ,\r\nmarketing_profile_attribut_value.decimal_value decimal_value_current , marketing_profile_attribut_value.blob_value as blob_value_current ,element1.name as name_type,element2.name as name_current_type,\r\nmarketing_profile_attribut_value.type as type_current from marketing_profile_attribut_value_temp\r\nleft join atooerp_user on   atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\nleft join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\nleft join marketing_profile_attribut on marketing_profile_attribut_value.attribut= marketing_profile_attribut.Id\r\nleft join marketing_profile_instances on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\nleft join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\nleft join commercial_partner on marketing_profile_instances.partner=commercial_partner.Id\r\nleft join commercial_partner as com2 on marketing_profile_attribut_value_temp.partner=com2.Id\r\nleft join commercial_partner_category as current_category on com2.category=current_category.id\r\nleft join atooerp_type_element as element1 on marketing_profile_attribut_value_temp.type = element1.id\r\nleft join atooerp_type_element as element2 on marketing_profile_attribut_value.type = element2.id\r\nleft join atooerp_person as person on person.id="+user_contrat.id_employe+ "   \r\n\r\nleft join commercial_partner_category as temp_category on marketing_profile_attribut_value_temp.int_value=temp_category.id\r\nwhere marketing_profile_attribut_value_temp.profile_instance_temp is null and  marketing_profile_attribut_value_temp.partner_temp is null\r\nand marketing_profile_attribut_value_temp.state = 1\r\nand (marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance  group by instance.partner) or marketing_profile_attribut_value_temp.attribut_name is not null) order by marketing_profile_attribut_value_temp.create_date desc;";
            string sqlcmd = "SELECT marketing_profile_attribut_value_temp.*  ,marketing_profile_attribut.label as label_attribut , marketing_profile.Id as id_profile,com2.name as name_partner2 ,com2.*,com2.name as name_partner, com2.state as state_name,\r\nmarketing_profile_attribut_value.Id as id_value ,marketing_profile_instances.Id id_instance, commercial_partner.Id as id_partner ,current_category.name as category_current ,\r\ncommercial_partner.name as partner_name , concat(person.first_name,' ',person.last_name) as user_name , marketing_profile.name as profile_name,temp_category.name as category_temp ,\r\nmarketing_profile_attribut_value.string_value as string_value_current, marketing_profile_attribut_value.boolean_value as boolean_value_current ,\r\nmarketing_profile_attribut_value.date_value as date_value_current , marketing_profile_attribut_value.int_value  as int_value_current ,\r\nmarketing_profile_attribut_value.decimal_value decimal_value_current , marketing_profile_attribut_value.blob_value as blob_value_current ,element1.name as name_type,element2.name as name_current_type,\r\nmarketing_profile_attribut_value.type as type_current from marketing_profile_attribut_value_temp\r\nleft join atooerp_user on   atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\nleft join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\nleft join marketing_profile_attribut on marketing_profile_attribut_value.attribut= marketing_profile_attribut.Id\r\nleft join marketing_profile_instances on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\nleft join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\nleft join commercial_partner on marketing_profile_instances.partner=commercial_partner.Id\r\nleft join commercial_partner as com2 on marketing_profile_attribut_value_temp.partner=com2.Id\r\nleft join commercial_partner_category as current_category on com2.category=current_category.id\r\nleft join atooerp_type_element as element1 on marketing_profile_attribut_value_temp.type = element1.id\r\nleft join atooerp_type_element as element2 on marketing_profile_attribut_value.type = element2.id\r\nleft join atooerp_person as person on person.id=marketing_profile_attribut_value_temp.employe\r\n\r\nleft join commercial_partner_category as temp_category on marketing_profile_attribut_value_temp.int_value=temp_category.id\r\nwhere marketing_profile_attribut_value_temp.employe=" + id + " and marketing_profile_attribut_value_temp.profile_instance_temp is null and  marketing_profile_attribut_value_temp.partner_temp is null\r\nand (marketing_profile_attribut_value_temp.state = 3 )\r\nand (marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance  group by instance.partner) or marketing_profile_attribut_value_temp.attribut_name is not null) order by marketing_profile_attribut_value_temp.create_date desc;";
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlcmd, DbConnection.con);

            List<NotificationProfilePartner> list = new List<NotificationProfilePartner>();
            MySqlDataReader reader = cmd.ExecuteReader();



            while (reader.Read())
            {
                try
                {
                    NotificationProfilePartner attribute = new NotificationProfilePartner();
                    if (reader["attribut_name"].ToString() == "")
                    {
                        attribute.IsPartnerAttribute = true;
                        attribute.id_temp = Convert.ToInt32(reader["id"]);
                        attribute.created_by = Convert.ToString(reader["user_name"]);
                        attribute.profile = Convert.ToString(reader["profile_name"]);
                        attribute.profile_user = Convert.ToString(reader["partner_name"]);
                        attribute.create_date = Convert.ToString(reader["create_date"]);
                        attribute.id_partner = Convert.ToInt32(reader["id_partner"]);
                        attribute.id_profile = Convert.ToInt32(reader["id_profile"]);
                        attribute.id_current_value = Convert.ToInt32(reader["id_value"]);
                        attribute.state = Convert.ToInt32(reader["state"]);
                        attribute.name_attribute = Convert.ToString(reader["label_attribut"]).ToUpper();
                        attribute.id_instance = Convert.ToInt32(reader["id_instance"]);
                    }
                    else
                    {
                        attribute.IsPartnerAttribute = false;
                        attribute.id_temp = Convert.ToInt32(reader["id"]);
                        attribute.created_by = Convert.ToString(reader["user_name"]).ToUpper();
                        attribute.profile = Convert.ToString(reader["profile_name"]);
                        attribute.profile_user = Convert.ToString(reader["name_partner2"]);
                        attribute.create_date = Convert.ToString(reader["create_date"]);
                        attribute.id_partner = Convert.ToInt32(reader["partner"]);
                        attribute.state = Convert.ToInt32(reader["state"]);
                        attribute.name_attribute = Convert.ToString(reader["attribut_name"]).ToUpper();


                    }

                    if (Convert.ToString(reader["state"]) == "2")
                    {
                        attribute.HasAccepted = true;

                    }
                    else if (Convert.ToString(reader["state"]) == "3")
                    {
                        attribute.HasRefused = true;

                    }
                    else
                    {
                        attribute.HasNoState = true;
                    }


                    if (attribute.IsPartnerAttribute)
                    {
                        if (Convert.ToString(reader["string_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["string_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);

                        }
                        else if (Convert.ToString(reader["boolean_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["boolean_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["boolean_value"]);

                        }
                        else if (Convert.ToString(reader["date_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["date_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["date_value"]);

                        }
                        else if (Convert.ToString(reader["decimal_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["decimal_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["decimal_value"]);
                        }
                        else if (Convert.ToString(reader["int_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["int_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["int_value"]);

                        }
                        else if (Convert.ToString(reader["blob_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["blob_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["blob_value"]);
                        }
                        else if (Convert.ToString(reader["type"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["name_current_type"]);
                            attribute.Temp_Value = Convert.ToString(reader["name_type"]);
                            attribute.id_type = Convert.ToInt32(reader["type"]);
                            attribute.id_current_type = Convert.ToInt32(reader["type_current"]);

                        }

                    }
                    else
                    {
                        if (Convert.ToString(reader["attribut_name"]) == "name")
                        {
                            attribute.current_value = Convert.ToString(reader["name_partner"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);


                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "category")
                        {
                            attribute.current_value = Convert.ToString(reader["category_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["category_temp"]);
                            attribute.value_temp = Convert.ToString(reader["int_value"]);

                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "number")
                        {
                            attribute.current_value = Convert.ToString(reader["number"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);

                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "street")
                        {
                            attribute.current_value = Convert.ToString(reader["street"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "city")
                        {
                            attribute.current_value = Convert.ToString(reader["city"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "state")
                        {
                            attribute.current_value = Convert.ToString(reader["state_name"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "country")
                        {
                            attribute.current_value = Convert.ToString(reader["country"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "postal_code")
                        {
                            attribute.current_value = Convert.ToString(reader["postal_code"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "email")
                        {
                            attribute.current_value = Convert.ToString(reader["email"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "fax")
                        {
                            attribute.current_value = Convert.ToString(reader["fax"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "customer")
                        {
                            attribute.current_value = Convert.ToString(reader["customer"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "supplier")
                        {
                            attribute.current_value = Convert.ToString(reader["supplier"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "vat_code")
                        {
                            attribute.current_value = Convert.ToString(reader["vat_code"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }


                    }


                    list.Add(attribute);

                }
                catch (Exception ex)
                {

                }




            }
            reader.Close();
            DbConnection.Deconnecter();
            return list;
        }
        public static List<NotificationProfilePartner> getAllHistoryTempValues()
        {
            
            //string  sqlcmd = "SELECT marketing_profile_attribut_value_temp.*  ,marketing_profile_attribut.label as label_attribut , marketing_profile.Id as id_profile,com2.name as name_partner2 ,com2.*,com2.name as name_partner, com2.state as state_name,\r\nmarketing_profile_attribut_value.Id as id_value ,marketing_profile_instances.Id id_instance, commercial_partner.Id as id_partner ,current_category.name as category_current ,\r\ncommercial_partner.name as partner_name , concat(person.first_name,' ',person.last_name) as user_name , marketing_profile.name as profile_name,temp_category.name as category_temp ,\r\nmarketing_profile_attribut_value.string_value as string_value_current, marketing_profile_attribut_value.boolean_value as boolean_value_current ,\r\nmarketing_profile_attribut_value.date_value as date_value_current , marketing_profile_attribut_value.int_value  as int_value_current ,\r\nmarketing_profile_attribut_value.decimal_value decimal_value_current , marketing_profile_attribut_value.blob_value as blob_value_current ,element1.name as name_type,element2.name as name_current_type,\r\nmarketing_profile_attribut_value.type as type_current from marketing_profile_attribut_value_temp\r\nleft join atooerp_user on   atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\nleft join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\nleft join marketing_profile_attribut on marketing_profile_attribut_value.attribut= marketing_profile_attribut.Id\r\nleft join marketing_profile_instances on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\nleft join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\nleft join commercial_partner on marketing_profile_instances.partner=commercial_partner.Id\r\nleft join commercial_partner as com2 on marketing_profile_attribut_value_temp.partner=com2.Id\r\nleft join commercial_partner_category as current_category on com2.category=current_category.id\r\nleft join atooerp_type_element as element1 on marketing_profile_attribut_value_temp.type = element1.id\r\nleft join atooerp_type_element as element2 on marketing_profile_attribut_value.type = element2.id\r\nleft join atooerp_person as person on person.id="+user_contrat.id_employe+ "   \r\n\r\nleft join commercial_partner_category as temp_category on marketing_profile_attribut_value_temp.int_value=temp_category.id\r\nwhere marketing_profile_attribut_value_temp.profile_instance_temp is null and  marketing_profile_attribut_value_temp.partner_temp is null\r\nand marketing_profile_attribut_value_temp.state = 1\r\nand (marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance  group by instance.partner) or marketing_profile_attribut_value_temp.attribut_name is not null) order by marketing_profile_attribut_value_temp.create_date desc;";
            string sqlcmd = "SELECT marketing_profile_attribut_value_temp.*  ,marketing_profile_attribut.label as label_attribut , marketing_profile.Id as id_profile,com2.name as name_partner2 ,com2.*,com2.name as name_partner, com2.state as state_name,\r\nmarketing_profile_attribut_value.Id as id_value ,marketing_profile_instances.Id id_instance, commercial_partner.Id as id_partner ,current_category.name as category_current ,\r\ncommercial_partner.name as partner_name , concat(person.first_name,' ',person.last_name) as user_name , marketing_profile.name as profile_name,temp_category.name as category_temp ,\r\nmarketing_profile_attribut_value.string_value as string_value_current, marketing_profile_attribut_value.boolean_value as boolean_value_current ,\r\nmarketing_profile_attribut_value.date_value as date_value_current , marketing_profile_attribut_value.int_value  as int_value_current ,\r\nmarketing_profile_attribut_value.decimal_value decimal_value_current , marketing_profile_attribut_value.blob_value as blob_value_current ,element1.name as name_type,element2.name as name_current_type,\r\nmarketing_profile_attribut_value.type as type_current from marketing_profile_attribut_value_temp\r\nleft join atooerp_user on   atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\nleft join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\nleft join marketing_profile_attribut on marketing_profile_attribut_value.attribut= marketing_profile_attribut.Id\r\nleft join marketing_profile_instances on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\nleft join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\nleft join commercial_partner on marketing_profile_instances.partner=commercial_partner.Id\r\nleft join commercial_partner as com2 on marketing_profile_attribut_value_temp.partner=com2.Id\r\nleft join commercial_partner_category as current_category on com2.category=current_category.id\r\nleft join atooerp_type_element as element1 on marketing_profile_attribut_value_temp.type = element1.id\r\nleft join atooerp_type_element as element2 on marketing_profile_attribut_value.type = element2.id\r\nleft join atooerp_person as person on person.id=marketing_profile_attribut_value_temp.employe\r\n\r\nleft join commercial_partner_category as temp_category on marketing_profile_attribut_value_temp.int_value=temp_category.id\r\nwhere marketing_profile_attribut_value_temp.profile_instance_temp is null and  marketing_profile_attribut_value_temp.partner_temp is null\r\nand (marketing_profile_attribut_value_temp.state = 2 or marketing_profile_attribut_value_temp.state = 3 )\r\nand (marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance  group by instance.partner) or marketing_profile_attribut_value_temp.attribut_name is not null) order by marketing_profile_attribut_value_temp.create_date desc;";
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlcmd, DbConnection.con);

            List<NotificationProfilePartner> list = new List<NotificationProfilePartner>();
            MySqlDataReader reader = cmd.ExecuteReader();



            while (reader.Read())
            {
                try
                {
                    NotificationProfilePartner attribute = new NotificationProfilePartner();
                    if (reader["attribut_name"].ToString() == "")
                    {
                        attribute.IsPartnerAttribute = true;
                        attribute.id_temp = Convert.ToInt32(reader["id"]);
                        attribute.created_by = Convert.ToString(reader["user_name"]);
                        attribute.profile = Convert.ToString(reader["profile_name"]);
                        attribute.profile_user = Convert.ToString(reader["partner_name"]);
                        attribute.create_date = Convert.ToString(reader["create_date"]);
                        attribute.id_partner = Convert.ToInt32(reader["id_partner"]);
                        attribute.id_profile = Convert.ToInt32(reader["id_profile"]);
                        attribute.id_current_value = Convert.ToInt32(reader["id_value"]);
                        attribute.state = Convert.ToInt32(reader["state"]);
                        attribute.name_attribute = Convert.ToString(reader["label_attribut"]).ToUpper();
                        attribute.id_instance = Convert.ToInt32(reader["id_instance"]);
                    }
                    else
                    {
                        attribute.IsPartnerAttribute = false;
                        attribute.id_temp = Convert.ToInt32(reader["id"]);
                        attribute.created_by = Convert.ToString(reader["user_name"]).ToUpper();
                        attribute.profile = Convert.ToString(reader["profile_name"]);
                        attribute.profile_user = Convert.ToString(reader["name_partner2"]);
                        attribute.create_date = Convert.ToString(reader["create_date"]);
                        attribute.id_partner = Convert.ToInt32(reader["partner"]);
                        attribute.state = Convert.ToInt32(reader["state"]);
                        attribute.name_attribute = Convert.ToString(reader["attribut_name"]).ToUpper();


                    }

                    if (Convert.ToString(reader["state"]) == "2")
                    {
                        attribute.HasAccepted = true;

                    }
                    else if (Convert.ToString(reader["state"]) == "3")
                    {
                        attribute.HasRefused = true;

                    }
                    else
                    {
                        attribute.HasNoState = true;
                    }


                    if (attribute.IsPartnerAttribute)
                    {
                        if (Convert.ToString(reader["string_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["string_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);

                        }
                        else if (Convert.ToString(reader["boolean_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["boolean_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["boolean_value"]);

                        }
                        else if (Convert.ToString(reader["date_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["date_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["date_value"]);

                        }
                        else if (Convert.ToString(reader["decimal_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["decimal_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["decimal_value"]);
                        }
                        else if (Convert.ToString(reader["int_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["int_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["int_value"]);

                        }
                        else if (Convert.ToString(reader["blob_value"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["blob_value_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["blob_value"]);
                        }
                        else if (Convert.ToString(reader["type"]) != "")
                        {
                            attribute.current_value = Convert.ToString(reader["name_current_type"]);
                            attribute.Temp_Value = Convert.ToString(reader["name_type"]);
                            attribute.id_type = Convert.ToInt32(reader["type"]);
                            attribute.id_current_type = Convert.ToInt32(reader["type_current"]);

                        }

                    }
                    else
                    {
                        if (Convert.ToString(reader["attribut_name"]) == "name")
                        {
                            attribute.current_value = Convert.ToString(reader["name_partner"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);


                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "category")
                        {
                            attribute.current_value = Convert.ToString(reader["category_current"]);
                            attribute.Temp_Value = Convert.ToString(reader["category_temp"]);
                            attribute.value_temp = Convert.ToString(reader["int_value"]);

                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "number")
                        {
                            attribute.current_value = Convert.ToString(reader["number"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);

                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "street")
                        {
                            attribute.current_value = Convert.ToString(reader["street"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "city")
                        {
                            attribute.current_value = Convert.ToString(reader["city"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "state")
                        {
                            attribute.current_value = Convert.ToString(reader["state_name"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "country")
                        {
                            attribute.current_value = Convert.ToString(reader["country"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "postal_code")
                        {
                            attribute.current_value = Convert.ToString(reader["postal_code"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "email")
                        {
                            attribute.current_value = Convert.ToString(reader["email"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "fax")
                        {
                            attribute.current_value = Convert.ToString(reader["fax"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "customer")
                        {
                            attribute.current_value = Convert.ToString(reader["customer"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "supplier")
                        {
                            attribute.current_value = Convert.ToString(reader["supplier"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }
                        else if (Convert.ToString(reader["attribut_name"]) == "vat_code")
                        {
                            attribute.current_value = Convert.ToString(reader["vat_code"]);
                            attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                        }


                    }


                    list.Add(attribute);

                }
                catch (Exception ex)
                {

                }
                



            }
            reader.Close();
            DbConnection.Deconnecter();
            return list;
        }
        public static List<NotificationProfilePartner> getAllTempValues()
        {

            string sqlcmd = "SELECT marketing_profile_attribut_value_temp.*  ,marketing_profile_attribut.label as label_attribut , marketing_profile.Id as id_profile,com2.name as name_partner2 ,com2.*,com2.name as name_partner, com2.state as state_name,\r\nmarketing_profile_attribut_value.Id as id_value ,marketing_profile_instances.Id id_instance, commercial_partner.Id as id_partner ,current_category.name as category_current ,\r\ncommercial_partner.name as partner_name , concat(person.first_name,' ',person.last_name) as user_name , marketing_profile.name as profile_name,temp_category.name as category_temp ,\r\nmarketing_profile_attribut_value.string_value as string_value_current, marketing_profile_attribut_value.boolean_value as boolean_value_current ,\r\nmarketing_profile_attribut_value.date_value as date_value_current , marketing_profile_attribut_value.int_value  as int_value_current ,\r\nmarketing_profile_attribut_value.decimal_value decimal_value_current , marketing_profile_attribut_value.blob_value as blob_value_current ,element1.name as name_type,element2.name as name_current_type,\r\nmarketing_profile_attribut_value.type as type_current from marketing_profile_attribut_value_temp\r\nleft join atooerp_user on   atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\nleft join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\nleft join marketing_profile_attribut on marketing_profile_attribut_value.attribut= marketing_profile_attribut.Id\r\nleft join marketing_profile_instances on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\nleft join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\nleft join commercial_partner on marketing_profile_instances.partner=commercial_partner.Id\r\nleft join commercial_partner as com2 on marketing_profile_attribut_value_temp.partner=com2.Id\r\nleft join commercial_partner_category as current_category on com2.category=current_category.id\r\nleft join atooerp_type_element as element1 on marketing_profile_attribut_value_temp.type = element1.id\r\nleft join atooerp_type_element as element2 on marketing_profile_attribut_value.type = element2.id\r\nleft join atooerp_person as person on person.id=marketing_profile_attribut_value_temp.employe\r\n\r\nleft join commercial_partner_category as temp_category on marketing_profile_attribut_value_temp.int_value=temp_category.id\r\nwhere marketing_profile_attribut_value_temp.profile_instance_temp is null and  marketing_profile_attribut_value_temp.partner_temp is null\r\nand marketing_profile_attribut_value_temp.state = 1\r\nand (marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance  group by instance.partner) or marketing_profile_attribut_value_temp.attribut_name is not null) order by marketing_profile_attribut_value_temp.create_date desc;";
            // string sqlcmd = "SELECT marketing_profile_attribut_value_temp.*  ,marketing_profile_attribut.label as label_attribut , marketing_profile.Id as id_profile,com2.name as name_partner2 ,com2.*,com2.name as name_partner, com2.state as state_name,\r\nmarketing_profile_attribut_value.Id as id_value ,marketing_profile_instances.Id id_instance, commercial_partner.Id as id_partner ,current_category.name as category_current ,\r\ncommercial_partner.name as partner_name , concat(person.first_name,' ',person.last_name) as user_name , marketing_profile.name as profile_name,temp_category.name as category_temp ,\r\nmarketing_profile_attribut_value.string_value as string_value_current, marketing_profile_attribut_value.boolean_value as boolean_value_current ,\r\nmarketing_profile_attribut_value.date_value as date_value_current , marketing_profile_attribut_value.int_value  as int_value_current ,\r\nmarketing_profile_attribut_value.decimal_value decimal_value_current , marketing_profile_attribut_value.blob_value as blob_value_current ,element1.name as name_type,element2.name as name_current_type,\r\nmarketing_profile_attribut_value.type as type_current from marketing_profile_attribut_value_temp\r\nleft join atooerp_user on   atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\nleft join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\nleft join marketing_profile_attribut on marketing_profile_attribut_value.attribut= marketing_profile_attribut.Id\r\nleft join marketing_profile_instances on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\nleft join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\nleft join commercial_partner on marketing_profile_instances.partner=commercial_partner.Id\r\nleft join commercial_partner as com2 on marketing_profile_attribut_value_temp.partner=com2.Id\r\nleft join commercial_partner_category as current_category on com2.category=current_category.id\r\nleft join atooerp_type_element as element1 on marketing_profile_attribut_value_temp.type = element1.id\r\nleft join atooerp_type_element as element2 on marketing_profile_attribut_value.type = element2.id\r\nleft join atooerp_person as person on person.id=" + user_contrat.id_employe + "   \r\n\r\nleft join commercial_partner_category as temp_category on marketing_profile_attribut_value_temp.int_value=temp_category.id\r\nwhere marketing_profile_attribut_value_temp.profile_instance_temp is null and  marketing_profile_attribut_value_temp.partner_temp is null\r\nand (marketing_profile_attribut_value_temp.state = 2 or marketing_profile_attribut_value_temp.state = 3 )\r\nand (marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance  group by instance.partner) or marketing_profile_attribut_value_temp.attribut_name is not null) order by marketing_profile_attribut_value_temp.create_date desc;"; 
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlcmd, DbConnection.con);

            List<NotificationProfilePartner> list = new List<NotificationProfilePartner>();
            MySqlDataReader reader = cmd.ExecuteReader();



            while (reader.Read())
            {
                try
                {

                }catch (Exception ex)
                {

                }
                NotificationProfilePartner attribute = new NotificationProfilePartner();
                if (reader["attribut_name"].ToString() == "")
                {
                    attribute.IsPartnerAttribute = true;
                    attribute.id_temp = Convert.ToInt32(reader["id"]);
                    attribute.created_by = Convert.ToString(reader["user_name"]);
                    attribute.profile = Convert.ToString(reader["profile_name"]);
                    attribute.profile_user = Convert.ToString(reader["partner_name"]);
                    attribute.create_date = Convert.ToString(reader["create_date"]);
                    attribute.id_partner = Convert.ToInt32(reader["id_partner"]);
                    attribute.id_profile = Convert.ToInt32(reader["id_profile"]);
                    attribute.id_current_value = Convert.ToInt32(reader["id_value"]);
                    attribute.state = Convert.ToInt32(reader["state"]);
                    attribute.name_attribute = Convert.ToString(reader["label_attribut"]).ToUpper();
                    attribute.id_instance = Convert.ToInt32(reader["id_instance"]);
                }
                else
                {
                    attribute.IsPartnerAttribute = false;
                    attribute.id_temp = Convert.ToInt32(reader["id"]);
                    attribute.created_by = Convert.ToString(reader["user_name"]).ToUpper();
                    attribute.profile = Convert.ToString(reader["profile_name"]);
                    attribute.profile_user = Convert.ToString(reader["name_partner2"]);
                    attribute.create_date = Convert.ToString(reader["create_date"]);
                    attribute.id_partner = Convert.ToInt32(reader["partner"]);
                    attribute.state = Convert.ToInt32(reader["state"]);
                    attribute.name_attribute = Convert.ToString(reader["attribut_name"]).ToUpper();


                }

                if (Convert.ToString(reader["state"]) == "2")
                {
                    attribute.HasAccepted = true;

                }
                else if (Convert.ToString(reader["state"]) == "3")
                {
                    attribute.HasRefused = true;

                }
                else
                {
                    attribute.HasNoState = true;
                }


                if (attribute.IsPartnerAttribute)
                {
                    if (Convert.ToString(reader["string_value"]) != "")
                    {
                        attribute.current_value = Convert.ToString(reader["string_value_current"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);

                    }
                    else if (Convert.ToString(reader["boolean_value"]) != "")
                    {
                        attribute.current_value = Convert.ToString(reader["boolean_value_current"]);
                        attribute.Temp_Value = Convert.ToString(reader["boolean_value"]);

                    }
                    else if (Convert.ToString(reader["date_value"]) != "")
                    {
                        attribute.current_value = Convert.ToString(reader["date_value_current"]);
                        attribute.Temp_Value = Convert.ToString(reader["date_value"]);

                    }
                    else if (Convert.ToString(reader["decimal_value"]) != "")
                    {
                        attribute.current_value = Convert.ToString(reader["decimal_value_current"]);
                        attribute.Temp_Value = Convert.ToString(reader["decimal_value"]);
                    }
                    else if (Convert.ToString(reader["int_value"]) != "")
                    {
                        attribute.current_value = Convert.ToString(reader["int_value_current"]);
                        attribute.Temp_Value = Convert.ToString(reader["int_value"]);

                    }
                    else if (Convert.ToString(reader["blob_value"]) != "")
                    {
                        attribute.current_value = Convert.ToString(reader["blob_value_current"]);
                        attribute.Temp_Value = Convert.ToString(reader["blob_value"]);
                    }
                    else if (Convert.ToString(reader["type"]) != "")
                    {
                        try
                        {
                            attribute.current_value = Convert.ToString(reader["name_current_type"]);
                            attribute.Temp_Value = Convert.ToString(reader["name_type"]);
                            attribute.id_type = Convert.ToInt32(reader["type"]);
                            attribute.id_current_type = Convert.ToInt32(reader["type_current"]);

                        }
                        catch(Exception ex)
                        {

                        }
                       

                    }

                }
                else
                {
                    if (Convert.ToString(reader["attribut_name"]) == "name")
                    {
                        attribute.current_value = Convert.ToString(reader["name_partner"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);


                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "category")
                    {
                        attribute.current_value = Convert.ToString(reader["category_current"]);
                        attribute.Temp_Value = Convert.ToString(reader["category_temp"]);
                        attribute.value_temp = Convert.ToString(reader["int_value"]);

                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "number")
                    {
                        attribute.current_value = Convert.ToString(reader["number"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);

                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "street")
                    {
                        attribute.current_value = Convert.ToString(reader["street"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "city")
                    {
                        attribute.current_value = Convert.ToString(reader["city"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "state")
                    {
                        attribute.current_value = Convert.ToString(reader["state_name"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "country")
                    {
                        attribute.current_value = Convert.ToString(reader["country"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "postal_code")
                    {
                        attribute.current_value = Convert.ToString(reader["postal_code"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "email")
                    {
                        attribute.current_value = Convert.ToString(reader["email"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "fax")
                    {
                        attribute.current_value = Convert.ToString(reader["fax"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "customer")
                    {
                        attribute.current_value = Convert.ToString(reader["customer"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "supplier")
                    {
                        attribute.current_value = Convert.ToString(reader["supplier"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "vat_code")
                    {
                        attribute.current_value = Convert.ToString(reader["vat_code"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }


                }


                list.Add(attribute);



            }
            reader.Close();
            DbConnection.Deconnecter();
            return list;
        }
        public static List<NotificationProfilePartner> getMyTempValues()
        {
            int id = user_contrat.id_employe;
            string sqlcmd = "SELECT marketing_profile_attribut_value_temp.*  ,marketing_profile_attribut.label as label_attribut , marketing_profile.Id as id_profile,com2.name as name_partner2 ,com2.*,com2.name as name_partner, com2.state as state_name,\r\nmarketing_profile_attribut_value.Id as id_value ,marketing_profile_instances.Id id_instance, commercial_partner.Id as id_partner ,current_category.name as category_current ,\r\ncommercial_partner.name as partner_name , concat(person.first_name,' ',person.last_name) as user_name , marketing_profile.name as profile_name,temp_category.name as category_temp ,\r\nmarketing_profile_attribut_value.string_value as string_value_current, marketing_profile_attribut_value.boolean_value as boolean_value_current ,\r\nmarketing_profile_attribut_value.date_value as date_value_current , marketing_profile_attribut_value.int_value  as int_value_current ,\r\nmarketing_profile_attribut_value.decimal_value decimal_value_current , marketing_profile_attribut_value.blob_value as blob_value_current ,element1.name as name_type,element2.name as name_current_type,\r\nmarketing_profile_attribut_value.type as type_current from marketing_profile_attribut_value_temp\r\nleft join atooerp_user on   atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\nleft join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\nleft join marketing_profile_attribut on marketing_profile_attribut_value.attribut= marketing_profile_attribut.Id\r\nleft join marketing_profile_instances on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\nleft join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\nleft join commercial_partner on marketing_profile_instances.partner=commercial_partner.Id\r\nleft join commercial_partner as com2 on marketing_profile_attribut_value_temp.partner=com2.Id\r\nleft join commercial_partner_category as current_category on com2.category=current_category.id\r\nleft join atooerp_type_element as element1 on marketing_profile_attribut_value_temp.type = element1.id\r\nleft join atooerp_type_element as element2 on marketing_profile_attribut_value.type = element2.id\r\nleft join atooerp_person as person on person.id=marketing_profile_attribut_value_temp.employe\r\n\r\nleft join commercial_partner_category as temp_category on marketing_profile_attribut_value_temp.int_value=temp_category.id\r\nwhere marketing_profile_attribut_value_temp.employe="+id+" and marketing_profile_attribut_value_temp.profile_instance_temp is null and  marketing_profile_attribut_value_temp.partner_temp is null\r\nand marketing_profile_attribut_value_temp.state = 1\r\nand (marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance  group by instance.partner) or marketing_profile_attribut_value_temp.attribut_name is not null) order by marketing_profile_attribut_value_temp.create_date desc;";
            // string sqlcmd = "SELECT marketing_profile_attribut_value_temp.*  ,marketing_profile_attribut.label as label_attribut , marketing_profile.Id as id_profile,com2.name as name_partner2 ,com2.*,com2.name as name_partner, com2.state as state_name,\r\nmarketing_profile_attribut_value.Id as id_value ,marketing_profile_instances.Id id_instance, commercial_partner.Id as id_partner ,current_category.name as category_current ,\r\ncommercial_partner.name as partner_name , concat(person.first_name,' ',person.last_name) as user_name , marketing_profile.name as profile_name,temp_category.name as category_temp ,\r\nmarketing_profile_attribut_value.string_value as string_value_current, marketing_profile_attribut_value.boolean_value as boolean_value_current ,\r\nmarketing_profile_attribut_value.date_value as date_value_current , marketing_profile_attribut_value.int_value  as int_value_current ,\r\nmarketing_profile_attribut_value.decimal_value decimal_value_current , marketing_profile_attribut_value.blob_value as blob_value_current ,element1.name as name_type,element2.name as name_current_type,\r\nmarketing_profile_attribut_value.type as type_current from marketing_profile_attribut_value_temp\r\nleft join atooerp_user on   atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\nleft join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\nleft join marketing_profile_attribut on marketing_profile_attribut_value.attribut= marketing_profile_attribut.Id\r\nleft join marketing_profile_instances on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\nleft join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\nleft join commercial_partner on marketing_profile_instances.partner=commercial_partner.Id\r\nleft join commercial_partner as com2 on marketing_profile_attribut_value_temp.partner=com2.Id\r\nleft join commercial_partner_category as current_category on com2.category=current_category.id\r\nleft join atooerp_type_element as element1 on marketing_profile_attribut_value_temp.type = element1.id\r\nleft join atooerp_type_element as element2 on marketing_profile_attribut_value.type = element2.id\r\nleft join atooerp_person as person on person.id=" + user_contrat.id_employe + "   \r\n\r\nleft join commercial_partner_category as temp_category on marketing_profile_attribut_value_temp.int_value=temp_category.id\r\nwhere marketing_profile_attribut_value_temp.profile_instance_temp is null and  marketing_profile_attribut_value_temp.partner_temp is null\r\nand (marketing_profile_attribut_value_temp.state = 2 or marketing_profile_attribut_value_temp.state = 3 )\r\nand (marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance  group by instance.partner) or marketing_profile_attribut_value_temp.attribut_name is not null) order by marketing_profile_attribut_value_temp.create_date desc;"; 
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlcmd, DbConnection.con);

            List<NotificationProfilePartner> list = new List<NotificationProfilePartner>();
            MySqlDataReader reader = cmd.ExecuteReader();



            while (reader.Read())
            {
                try
                {

                }
                catch (Exception ex)
                {

                }
                NotificationProfilePartner attribute = new NotificationProfilePartner();
                if (reader["attribut_name"].ToString() == "")
                {
                    attribute.IsPartnerAttribute = true;
                    attribute.id_temp = Convert.ToInt32(reader["id"]);
                    attribute.created_by = Convert.ToString(reader["user_name"]);
                    attribute.profile = Convert.ToString(reader["profile_name"]);
                    attribute.profile_user = Convert.ToString(reader["partner_name"]);
                    attribute.create_date = Convert.ToString(reader["create_date"]);
                    attribute.id_partner = Convert.ToInt32(reader["id_partner"]);
                    attribute.id_profile = Convert.ToInt32(reader["id_profile"]);
                    attribute.id_current_value = Convert.ToInt32(reader["id_value"]);
                    attribute.state = Convert.ToInt32(reader["state"]);
                    attribute.name_attribute = Convert.ToString(reader["label_attribut"]).ToUpper();
                    attribute.id_instance = Convert.ToInt32(reader["id_instance"]);
                }
                else
                {
                    attribute.IsPartnerAttribute = false;
                    attribute.id_temp = Convert.ToInt32(reader["id"]);
                    attribute.created_by = Convert.ToString(reader["user_name"]).ToUpper();
                    attribute.profile = Convert.ToString(reader["profile_name"]);
                    attribute.profile_user = Convert.ToString(reader["name_partner2"]);
                    attribute.create_date = Convert.ToString(reader["create_date"]);
                    attribute.id_partner = Convert.ToInt32(reader["partner"]);
                    attribute.state = Convert.ToInt32(reader["state"]);
                    attribute.name_attribute = Convert.ToString(reader["attribut_name"]).ToUpper();


                }

                if (Convert.ToString(reader["state"]) == "2")
                {
                    attribute.HasAccepted = true;

                }
                else if (Convert.ToString(reader["state"]) == "3")
                {
                    attribute.HasRefused = true;

                }
                else
                {
                    attribute.HasNoState = true;
                }


                if (attribute.IsPartnerAttribute)
                {
                    if (Convert.ToString(reader["string_value"]) != "")
                    {
                        attribute.current_value = Convert.ToString(reader["string_value_current"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);

                    }
                    else if (Convert.ToString(reader["boolean_value"]) != "")
                    {
                        attribute.current_value = Convert.ToString(reader["boolean_value_current"]);
                        attribute.Temp_Value = Convert.ToString(reader["boolean_value"]);

                    }
                    else if (Convert.ToString(reader["date_value"]) != "")
                    {
                        attribute.current_value = Convert.ToString(reader["date_value_current"]);
                        attribute.Temp_Value = Convert.ToString(reader["date_value"]);

                    }
                    else if (Convert.ToString(reader["decimal_value"]) != "")
                    {
                        attribute.current_value = Convert.ToString(reader["decimal_value_current"]);
                        attribute.Temp_Value = Convert.ToString(reader["decimal_value"]);
                    }
                    else if (Convert.ToString(reader["int_value"]) != "")
                    {
                        attribute.current_value = Convert.ToString(reader["int_value_current"]);
                        attribute.Temp_Value = Convert.ToString(reader["int_value"]);

                    }
                    else if (Convert.ToString(reader["blob_value"]) != "")
                    {
                        attribute.current_value = Convert.ToString(reader["blob_value_current"]);
                        attribute.Temp_Value = Convert.ToString(reader["blob_value"]);
                    }
                    else if (Convert.ToString(reader["type"]) != "")
                    {
                        try
                        {
                            attribute.current_value = Convert.ToString(reader["name_current_type"]);
                            attribute.Temp_Value = Convert.ToString(reader["name_type"]);
                            attribute.id_type = Convert.ToInt32(reader["type"]);
                            attribute.id_current_type = Convert.ToInt32(reader["type_current"]);

                        }
                        catch (Exception ex)
                        {

                        }


                    }

                }
                else
                {
                    if (Convert.ToString(reader["attribut_name"]) == "name")
                    {
                        attribute.current_value = Convert.ToString(reader["name_partner"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);


                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "category")
                    {
                        attribute.current_value = Convert.ToString(reader["category_current"]);
                        attribute.Temp_Value = Convert.ToString(reader["category_temp"]);
                        attribute.value_temp = Convert.ToString(reader["int_value"]);

                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "number")
                    {
                        attribute.current_value = Convert.ToString(reader["number"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);

                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "street")
                    {
                        attribute.current_value = Convert.ToString(reader["street"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "city")
                    {
                        attribute.current_value = Convert.ToString(reader["city"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "state")
                    {
                        attribute.current_value = Convert.ToString(reader["state_name"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "country")
                    {
                        attribute.current_value = Convert.ToString(reader["country"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "postal_code")
                    {
                        attribute.current_value = Convert.ToString(reader["postal_code"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "email")
                    {
                        attribute.current_value = Convert.ToString(reader["email"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "fax")
                    {
                        attribute.current_value = Convert.ToString(reader["fax"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "customer")
                    {
                        attribute.current_value = Convert.ToString(reader["customer"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "supplier")
                    {
                        attribute.current_value = Convert.ToString(reader["supplier"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }
                    else if (Convert.ToString(reader["attribut_name"]) == "vat_code")
                    {
                        attribute.current_value = Convert.ToString(reader["vat_code"]);
                        attribute.Temp_Value = Convert.ToString(reader["string_value"]);
                    }


                }


                list.Add(attribute);



            }
            reader.Close();
            DbConnection.Deconnecter();
            return list;
        }
    }

    public class values
    {
        public int id { get; set; }
        public string name { get; set; }
        public string memo { get; set; }
        public DateTime create_date { get; set; }
        public int? profile_instance { get; set; } = null;
        public int? attribute { get; set; } = null;
        public string string_value { get; set; }
        public int? int_value { get; set; } = null;
        public decimal? decimal_value { get; set; } = null;
        public bool? boolean_value { get; set; } = null;
        public byte? bloob_value { get; set; } = null;
        public DateTime? date_value { get; set; } = null;
        public int? type { get; set; } = null;
        public int attribut_type { get; set; } 

        public values() { }
    }
}
