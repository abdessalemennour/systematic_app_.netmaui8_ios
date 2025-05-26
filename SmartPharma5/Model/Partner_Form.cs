//using Xamarin.Essentials;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.Model;
using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
//using Xamarin.Essentials;
Après :
using Xamarin.Forms;
using Microsoft.Maui.Controls;
using Microsoft.ComponentModel;
using MySqlConnector;
using SmartPharma5.Model;
using Xamarin.Essentials;
*/
//using System.Maui.Graphics;
//using System;
using Acr.UserDialogs;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.View;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
Après :
using System.Net;
using System.Threading.Tasks;
*/


namespace SmartPharma5.Model
{
    public class Partner_Form
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public DateTime Create_Date { get; set; }
        public string Memo { get; set; }
        public bool Is_Gps_Enable { get; set; }
        public DateTime Begin_Date { get; set; }
        public DateTime End_Date { get; set; }
        public DateTime Open_Date { get; set; }
        public DateTime Close_Date { get; set; }
        public DateTime Estimated_Date { get; set; }
        public bool Validate { get; set; }
        public string Mac_Adress { get; set; }
        public string Ip { get; set; }
        public string Gps { get; set; }
        public int? Cycle_Id { get; set; }
        public int Form_Id { get; set; }
        public int Partner_Id { get; set; }
        public int Emplye_Id { get; set; }
        public int User_Id { get; set; }




        public Partner_Form(uint id, string name, DateTime create_Date, string memo, bool is_Gps_Enable, DateTime begin_Date, DateTime end_Date, DateTime open_Date, DateTime close_Date, DateTime estimated_Date, bool validate, string mac_Adress, string ip, string gps, int? cycle_Id, int form_Id, int partner_Id, int emplye_Id, int user_Id)
        {
            Id = id;
            Name = name;
            Create_Date = create_Date;
            Memo = memo;
            Is_Gps_Enable = is_Gps_Enable;
            Begin_Date = begin_Date;
            End_Date = end_Date;
            Open_Date = open_Date;
            Close_Date = close_Date;
            Estimated_Date = estimated_Date;
            Validate = validate;
            Mac_Adress = mac_Adress;
            Ip = ip;
            Gps = gps;
            Cycle_Id = cycle_Id;
            Form_Id = form_Id;
            Partner_Id = partner_Id;
            Emplye_Id = emplye_Id;
            User_Id = user_Id;


        }
        public Partner_Form(uint id, string name, DateTime create_Date, int partner_Id, int? cycle_id, int form_Id)
        {
            Id = id;
            Name = name;
            Create_Date = create_Date;
            Partner_Id = partner_Id;
            Form_Id = form_Id;
            Cycle_Id = cycle_id;
        }
        //REAL
        public async static Task<BindingList<Collection>> GetPartnerFormByOpp(int opp)
        {
            uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
            string sqlCmd = "select marketing_quiz_partner_form.Id as Partner_form_id,marketing_quiz_partner_form.begin_date,estimated_date, marketing_quiz_partner_form.gps , " +
                "marketing_quiz_partner_form.end_date,marketing_quiz_partner_form.close_date,marketing_quiz_partner_form.open_date," +
                "marketing_quiz_form.name as Form_name,commercial_partner.name as Partner_name,partner_id,cycle_id,marketing_quiz_cycle.name as Cycle_name,validated " +
                "from marketing_quiz_partner_form left join " +
                "commercial_partner on marketing_quiz_partner_form.partner_id = commercial_partner.Id left join " +
                "marketing_quiz_form on marketing_quiz_partner_form.form_id=marketing_quiz_form.Id left join " +
                "marketing_quiz_cycle on marketing_quiz_partner_form.cycle_id=marketing_quiz_cycle.Id " +
                "where marketing_quiz_partner_form.crm_opportunity=" + opp + " group by marketing_quiz_partner_form.Id order by estimated_date";

            int i = 0;
            BindingList<Collection> list = new BindingList<Collection>();

            // Display loading indicator
            using (var loading = UserDialogs.Instance.Loading("Loading Please wait...", null, null, true, MaskType.Black))
            {
                // Set a timeout of 10 seconds
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    try
                    {
                        if (await DbConnection.Connecter3())
                        {
                            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    try
                                    {
                                        list.Add(new Collection(
                                            Convert.ToInt32(reader["Partner_form_id"]),
                                            i,
                                            reader["Form_name"].ToString(),
                                            Convert.ToInt32(reader["partner_Id"]),
                                            reader["Partner_name"].ToString(),
                                            reader["cycle_Id"] is uint ? Convert.ToInt32(reader["cycle_Id"]) : (int?)null,
                                            reader["Cycle_name"].ToString(),
                                            Convert.ToDateTime(reader["begin_date"]),
                                            Convert.ToDateTime(reader["end_date"]),
                                            Convert.ToDateTime(reader["estimated_date"]),
                                            reader["open_date"] is DateTime ? Convert.ToDateTime(reader["open_date"]) : (DateTime?)null,
                                            reader["close_date"] is DateTime ? Convert.ToDateTime(reader["close_date"]) : (DateTime?)null,
                                            Convert.ToBoolean(reader["validated"]),
                                            reader["gps"].ToString()
                                        ));
                                    }
                                    catch (Exception ex)
                                    {
                                        reader.Close();
                                        return null;
                                    }
                                }
                            }

                            DbConnection.Deconnecter();
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions as needed
                        return null;
                    }
                }
            }

            return list;
        }

        public async static Task<BindingList<Collection>> GetMyPartnerForm()
        {
            uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
            string sqlCmd = "select marketing_quiz_partner_form.Id as Partner_form_id, " +
                "marketing_quiz_partner_form.begin_date, estimated_date, marketing_quiz_partner_form.gps, " +
                "marketing_quiz_partner_form.end_date, marketing_quiz_partner_form.close_date, marketing_quiz_partner_form.open_date, " +
                "marketing_quiz_form.name as Form_name, commercial_partner.name as Partner_name, commercial_partner.gps as Gps_Partner, " +
                "partner_id, cycle_id, marketing_quiz_cycle.name as Cycle_name, validated " +
                "from marketing_quiz_partner_form " +
                "left join commercial_partner on marketing_quiz_partner_form.partner_id = commercial_partner.Id " +
                "left join marketing_quiz_form on marketing_quiz_partner_form.form_id = marketing_quiz_form.Id " +
                "left join marketing_quiz_cycle on marketing_quiz_partner_form.cycle_id = marketing_quiz_cycle.Id " +
                "where marketing_quiz_partner_form.employe_id = " + idagent + " " +
                "group by marketing_quiz_partner_form.Id order by estimated_date";

            int i = 0;
            BindingList<Collection> list = new BindingList<Collection>();

            using (var loading = UserDialogs.Instance.Loading("Loading Please wait...", null, null, true, MaskType.Black))
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    try
                    {
                        if (await DbConnection.Connecter3())
                        {
                            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    try
                                    {
                                        list.Add(new Collection(
                                            Convert.ToInt32(reader["Partner_form_id"]),
                                            i,
                                            reader["Form_name"].ToString(),
                                            Convert.ToInt32(reader["partner_Id"]),
                                            reader["Partner_name"].ToString(),
                                            reader["cycle_Id"] is uint ? Convert.ToInt32(reader["cycle_Id"]) : (int?)null,
                                            reader["Cycle_name"].ToString(),
                                            Convert.ToDateTime(reader["begin_date"]),
                                            Convert.ToDateTime(reader["end_date"]),
                                            Convert.ToDateTime(reader["estimated_date"]),
                                            reader["open_date"] is DateTime ? Convert.ToDateTime(reader["open_date"]) : (DateTime?)null,
                                            reader["close_date"] is DateTime ? Convert.ToDateTime(reader["close_date"]) : (DateTime?)null,
                                            Convert.ToBoolean(reader["validated"]),
                                            reader["gps"].ToString(),
                                            reader["Gps_Partner"].ToString()

                                        ));
                                    }
                                    catch (Exception ex)
                                    {
                                        reader.Close();
                                        return null;
                                    }
                                }
                            }

                            DbConnection.Deconnecter();
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }

            return list;
        }

        //public async static Task<BindingList<Collection>> GetMyPartnerForm()
        //{
        //    uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
        //    string sqlCmd = "select marketing_quiz_partner_form.Id as Partner_form_id,marketing_quiz_partner_form.begin_date,estimated_date, marketing_quiz_partner_form.gps , " +
        //        "marketing_quiz_partner_form.end_date,marketing_quiz_partner_form.close_date,marketing_quiz_partner_form.open_date," +
        //        "marketing_quiz_form.name as Form_name,commercial_partner.name as Partner_name,partner_id,cycle_id,marketing_quiz_cycle.name as Cycle_name,validated " +
        //        "from marketing_quiz_partner_form left join " +
        //        "commercial_partner on marketing_quiz_partner_form.partner_id = commercial_partner.Id left join " +
        //        "marketing_quiz_form on marketing_quiz_partner_form.form_id=marketing_quiz_form.Id left join " +
        //        "marketing_quiz_cycle on marketing_quiz_partner_form.cycle_id=marketing_quiz_cycle.Id " +
        //        "where marketing_quiz_partner_form.employe_id=" + idagent + " group by marketing_quiz_partner_form.Id order by estimated_date";

        //    int i = 0;
        //    BindingList<Collection> list = new BindingList<Collection>();

        //    // Display loading indicator
        //    using (var loading = UserDialogs.Instance.Loading("Loading Please wait...", null, null, true, MaskType.Black))
        //    {
        //        // Set a timeout of 10 seconds
        //        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
        //        {
        //            try
        //            {
        //                if (await DbConnection.Connecter3())
        //                {
        //                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
        //                    using (MySqlDataReader reader = cmd.ExecuteReader())
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            try
        //                            {
        //                                list.Add(new Collection(
        //                                    Convert.ToInt32(reader["Partner_form_id"]),
        //                                    i,
        //                                    reader["Form_name"].ToString(),
        //                                    Convert.ToInt32(reader["partner_Id"]),
        //                                    reader["Partner_name"].ToString(),
        //                                    reader["cycle_Id"] is uint ? Convert.ToInt32(reader["cycle_Id"]) : (int?)null,
        //                                    reader["Cycle_name"].ToString(),
        //                                    Convert.ToDateTime(reader["begin_date"]),
        //                                    Convert.ToDateTime(reader["end_date"]),
        //                                    Convert.ToDateTime(reader["estimated_date"]),
        //                                    reader["open_date"] is DateTime ? Convert.ToDateTime(reader["open_date"]) : (DateTime?)null,
        //                                    reader["close_date"] is DateTime ? Convert.ToDateTime(reader["close_date"]) : (DateTime?)null,
        //                                    Convert.ToBoolean(reader["validated"]),
        //                                    reader["gps"].ToString()
        //                                ));
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                reader.Close();
        //                                return null;
        //                            }
        //                        }
        //                    }

        //                    DbConnection.Deconnecter();
        //                }
        //                else
        //                {
        //                    return null;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                // Handle exceptions as needed
        //                return null;
        //            }
        //        }
        //    }

        //    return list;
        //}
        public async static Task<BindingList<Collection>> GetAllPartnerForm()
        {
            int i = 0;
            //string sqlCmd = "select marketing_quiz_partner_form.Id as Partner_form_id,marketing_quiz_partner_form.begin_date,estimated_date, marketing_quiz_partner_form.gps, " +
            //    "marketing_quiz_partner_form.end_date,marketing_quiz_partner_form.close_date,marketing_quiz_partner_form.open_date," +
            //    "marketing_quiz_form.name as Form_name,commercial_partner.name as Partner_name,partner_id,marketing_quiz_partner_form.employe_id,CONCAT(atooerp_person.first_name,' ',atooerp_person.last_name) as agentName,cycle_id,marketing_quiz_cycle.name as Cycle_name,validated " +
            //    "from marketing_quiz_partner_form left join " +
            //    "commercial_partner on marketing_quiz_partner_form.partner_id = commercial_partner.Id left join " +
            //    "marketing_quiz_form on marketing_quiz_partner_form.form_id=marketing_quiz_form.Id left join " +
            //    "marketing_quiz_cycle on marketing_quiz_partner_form.cycle_id=marketing_quiz_cycle.Id left join " +
            //    "atooerp_person on marketing_quiz_partner_form.employe_id=atooerp_person.Id " +
            //    " group by marketing_quiz_partner_form.Id order by estimated_date";
            string sqlCmd = "select marketing_quiz_partner_form.Id as Partner_form_id, " +
                "marketing_quiz_partner_form.begin_date, estimated_date, marketing_quiz_partner_form.gps, " +
                "marketing_quiz_partner_form.end_date, marketing_quiz_partner_form.close_date, marketing_quiz_partner_form.open_date, " +
                "marketing_quiz_form.name as Form_name, commercial_partner.name as Partner_name, commercial_partner.gps as Gps_Partner, partner_id, " +
                "marketing_quiz_partner_form.employe_id, CONCAT(atooerp_person.first_name,' ',atooerp_person.last_name) as agentName, " +
                "cycle_id, marketing_quiz_cycle.name as Cycle_name, validated " +
                "from marketing_quiz_partner_form " +
                "left join commercial_partner on marketing_quiz_partner_form.partner_id = commercial_partner.Id " +
                "left join marketing_quiz_form on marketing_quiz_partner_form.form_id = marketing_quiz_form.Id " +
                "left join marketing_quiz_cycle on marketing_quiz_partner_form.cycle_id = marketing_quiz_cycle.Id " +
                "left join atooerp_person on marketing_quiz_partner_form.employe_id = atooerp_person.Id " +
                "group by marketing_quiz_partner_form.Id order by estimated_date";


            BindingList<Collection> list = new BindingList<Collection>();

            // Display loading indicator
            using (var loading = UserDialogs.Instance.Loading("Loading Please wait...", null, null, true, MaskType.Black))
            {
                // Set a timeout of 5 seconds
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    try
                    {
                        // DbConnection.Deconnecter();
                        if (await DbConnection.Connecter3())
                        {
                            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    try
                                    {
                                        list.Add(new Collection(
                                            Convert.ToInt32(reader["Partner_form_id"]),
                                            i,
                                            reader["Form_name"].ToString(),
                                            Convert.ToInt32(reader["partner_id"]),
                                            reader["Partner_name"].ToString(),
                                            Convert.ToInt32(reader["employe_id"]),
                                            reader["agentName"].ToString(),
                                            reader["cycle_Id"] is uint ? Convert.ToInt32(reader["cycle_Id"]) : (int?)null,
                                            reader["Cycle_name"].ToString(),
                                            Convert.ToDateTime(reader["begin_date"]),
                                            Convert.ToDateTime(reader["end_date"]),
                                            Convert.ToDateTime(reader["estimated_date"]),
                                            reader["open_date"] is DateTime ? Convert.ToDateTime(reader["open_date"]) : (DateTime?)null,
                                            reader["close_date"] is DateTime ? Convert.ToDateTime(reader["close_date"]) : (DateTime?)null,
                                            Convert.ToBoolean(reader["validated"]),
                                            reader["gps"].ToString(),
                                            reader["Gps_Partner"].ToString()              

                                        ));
                                    }
                                    catch (Exception ex)
                                    {
                                        reader.Close();
                                        return null;
                                    }
                                }
                            }

                            DbConnection.Deconnecter();
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions as needed
                        return null;
                    }
                }
            }

            return list;
        }
        public static Collection GetPartnerFormById(int? idpartenerform)
        {
            int i = 0;
            string sqlCmd = "select marketing_quiz_partner_form.Id as Partner_form_id,marketing_quiz_partner_form.begin_date,estimated_date, marketing_quiz_partner_form.gps, " +
                "marketing_quiz_partner_form.end_date,marketing_quiz_partner_form.close_date,marketing_quiz_partner_form.open_date," +
                "marketing_quiz_form.name as Form_name,commercial_partner.name as Partner_name,partner_id,marketing_quiz_partner_form.employe_id,CONCAT(atooerp_person.first_name,' ',atooerp_person.last_name) as agentName,cycle_id,marketing_quiz_cycle.name as Cycle_name,validated " +
                "from marketing_quiz_partner_form left join " +
                "commercial_partner on marketing_quiz_partner_form.partner_id = commercial_partner.Id left join " +
                "marketing_quiz_form on marketing_quiz_partner_form.form_id=marketing_quiz_form.Id left join " +
                "marketing_quiz_cycle on marketing_quiz_partner_form.cycle_id=marketing_quiz_cycle.Id left join " +
                "atooerp_person on marketing_quiz_partner_form.employe_id=atooerp_person.Id where marketing_quiz_partner_form.Id=" + idpartenerform + " " +
                " group by marketing_quiz_partner_form.Id order by estimated_date";

            Collection partnerform = new Collection();


            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {

                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        i++;
                        try
                        {
                            partnerform = new Collection(
                                Convert.ToInt32(reader["Partner_form_id"]),
                                i,
                                reader["Form_name"].ToString(),
                                Convert.ToInt32(reader["partner_id"]),
                                reader["Partner_name"].ToString(),
                                Convert.ToInt32(reader["employe_id"]),
                                reader["agentName"].ToString(),
                                reader["cycle_Id"] is uint ? Convert.ToInt32(reader["cycle_Id"]) : (int?)null,
                                reader["Cycle_name"].ToString(),
                                Convert.ToDateTime(reader["begin_date"]),
                                Convert.ToDateTime(reader["end_date"]),
                                Convert.ToDateTime(reader["estimated_date"]),
                                reader["open_date"] is DateTime ? Convert.ToDateTime(reader["open_date"]) : (DateTime?)null,
                                reader["close_date"] is DateTime ? Convert.ToDateTime(reader["close_date"]) : (DateTime?)null,
                                Convert.ToBoolean(reader["validated"])
                                );
                        }
                        catch (Exception ex)
                        {
                            App.Current.MainPage.DisplayAlert("Warning", "Connetion Time out", "Ok");
                            App.Current.MainPage.Navigation.PopAsync();
                        }

                    }

                    reader.Close();
                    DbConnection.Deconnecter();
                }
                catch (Exception ex)
                { }

            }

            return partnerform;
        }
        public static async Task<BindingList<Collection>> GetPartnerFormByIdPartner(int Id)
        {
            string sqlCmd = "select marketing_quiz_partner_form.Id as Partner_form_id,marketing_quiz_partner_form.begin_date, marketing_quiz_partner_form.gps, " +
                "marketing_quiz_partner_form.end_date,marketing_quiz_partner_form.close_date,marketing_quiz_partner_form.open_date," +
                "marketing_quiz_form.name as Form_name,commercial_partner.name as Partner_name,partner_id,cycle_id,marketing_quiz_cycle.name as Cycle_name,validated " +
                "from marketing_quiz_partner_form left join " +
                "commercial_partner on marketing_quiz_partner_form.partner_id = commercial_partner.Id left join " +
                "marketing_quiz_form on marketing_quiz_partner_form.form_id=marketing_quiz_form.Id left join " +
                "marketing_quiz_cycle on marketing_quiz_partner_form.cycle_id=marketing_quiz_cycle.Id " +
                "where marketing_quiz_partner_form.partner_id = " + Id + " group by marketing_quiz_partner_form.Id order by estimated_date";


            BindingList<Collection> list = new BindingList<Collection>();


            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {

                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {

                        try
                        {
                            list.Add(new Collection(
                                Convert.ToInt32(reader["Partner_form_id"]),
                                reader["Form_name"].ToString(),
                                Convert.ToInt32(reader["partner_Id"]),
                                reader["Partner_name"].ToString(),
                                reader["cycle_Id"] is uint ? Convert.ToInt32(reader["cycle_Id"]) : (int?)null,
                                reader["Cycle_name"].ToString(),
                                Convert.ToDateTime(reader["begin_date"]),
                                Convert.ToDateTime(reader["end_date"]),
                                Convert.ToDateTime(reader["estimated_date"]),
                                Convert.ToDateTime(reader["open_date"]),
                                Convert.ToDateTime(reader["close_date"]),
                                Convert.ToBoolean(reader["validated"]),
                                reader["gps"].ToString()
                                ));
                        }
                        catch (Exception ex)
                        {
                            await App.Current.MainPage.DisplayAlert("Warning", "Connetion Time out", "Ok");
                            await App.Current.MainPage.Navigation.PopAsync();
                        }

                    }

                    reader.Close();
                    DbConnection.Deconnecter();
                }
                catch (Exception ex)
                { }
            }
            return list;
        }
        internal static void Update(Partner_Form.Collection partner_Form)
        {
            string macaddress = DbConnection.macAdresse();
            //string macaddress = "";
            string ipaddress = DbConnection.IpAddress();
            string sqlCmd = "Update marketing_quiz_partner_form SET close_date=NOW(),open_date = coalesce(open_date,Now()),validated=1,mac_address='" + macaddress + "',ip='" + ipaddress + "' where Id=" + partner_Form.Id + ";";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();
        }

        internal static void UpdateOpenDate(Collection partner_Form)
        {
            string sqlCmd = "Update marketing_quiz_partner_form SET open_date = coalesce(open_date,Now()) where Id=" + partner_Form.Id + ";";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();
        }
        internal static async Task<int?> InsertPartnerFormAndGetLastId(Partner partner, Form item)
        {
            int idpartnerform = 0;
            string ip = DbConnection.IpAddress();
            uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
            int iduser = Preferences.Get("iduser", 0);
            string sqlCmd = "INSERT INTO marketing_quiz_partner_form SET name='" + partner.Name + "', create_date=NOW(),is_gps_enable=" + item.IsGpsEnable + ",validated=0,estimated_date=Now(),begin_date=Now(),end_date=DATE_ADD(Now(), INTERVAL 1 DAY),open_date=NOW(),ip='" + ip + "',form_id=" + item.Id + ",partner_id=" + partner.Id + ",employe_id=" + idagent + ",user_id=" + iduser + "; Select MAX(Id) From marketing_quiz_partner_form;";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            if (await DbConnection.Connecter3())
            {
                try
                {
                    idpartnerform = int.Parse(cmd.ExecuteScalar().ToString());
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


            return idpartnerform;
        }
        internal static async Task<int?> InsertPartnerFormWithOppAndGetLastId(Partner partner, Form item, int oppId, int contact)
        {
            int idpartnerform = 0;
            string ip = DbConnection.IpAddress();
            uint idagent = (uint)Preferences.Get("idagent", Convert.ToUInt32(null));
            int iduser = Preferences.Get("iduser", 0);
            string sqlCmd = "";

            if (oppId == 0 && contact == 0)
            {
                sqlCmd = "INSERT INTO marketing_quiz_partner_form SET name='" + partner.Name + "', create_date=NOW(),is_gps_enable=" + item.IsGpsEnable + ",validated=0,estimated_date=Now(),begin_date=Now(),end_date=DATE_ADD(Now(), INTERVAL 1 DAY),open_date=NOW(),ip='" + ip + "',form_id=" + item.Id + ",partner_id=" + partner.Id + ",employe_id=" + idagent + ",user_id=" + iduser + "; Select MAX(Id) From marketing_quiz_partner_form;";


            }
            else if (oppId == 0 && contact != 0)
            {
                sqlCmd = "INSERT INTO marketing_quiz_partner_form SET name='" + partner.Name + "', create_date=NOW(),is_gps_enable=" + item.IsGpsEnable + ",validated=0,estimated_date=Now(),contact=" + contact + ",begin_date=Now(),end_date=DATE_ADD(Now(), INTERVAL 1 DAY),open_date=NOW(),ip='" + ip + "',form_id=" + item.Id + ",partner_id=" + partner.Id + ",employe_id=" + idagent + ",user_id=" + iduser + "; Select MAX(Id) From marketing_quiz_partner_form;";


            }
            else if (oppId != 0 && contact == 0)
            {
                sqlCmd = "INSERT INTO marketing_quiz_partner_form SET name='" + partner.Name + "', create_date=NOW(),is_gps_enable=" + item.IsGpsEnable + ",validated=0,estimated_date=Now(),crm_opportunity=" + oppId + ",begin_date=Now(),end_date=DATE_ADD(Now(), INTERVAL 1 DAY),open_date=NOW(),ip='" + ip + "',form_id=" + item.Id + ",partner_id=" + partner.Id + ",employe_id=" + idagent + ",user_id=" + iduser + "; Select MAX(Id) From marketing_quiz_partner_form;";

            }
            else
            {
                sqlCmd = "INSERT INTO marketing_quiz_partner_form SET name='" + partner.Name + "', create_date=NOW(),is_gps_enable=" + item.IsGpsEnable + ",validated=0,estimated_date=Now(),crm_opportunity=" + oppId + ",contact=" + contact + ",begin_date=Now(),end_date=DATE_ADD(Now(), INTERVAL 1 DAY),open_date=NOW(),ip='" + ip + "',form_id=" + item.Id + ",partner_id=" + partner.Id + ",employe_id=" + idagent + ",user_id=" + iduser + "; Select MAX(Id) From marketing_quiz_partner_form;";

            }
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            if (await DbConnection.Connecter3())
            {
                try
                {
                    idpartnerform = int.Parse(cmd.ExecuteScalar().ToString());
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


            return idpartnerform;
        }

        //internal static async Task<Partner_Form.Collection> GetPartnerFormById(int idpartenerform)
        //{
        //    Partner_Form.Collection partnerform = null;

        //    string sqlCmd = "  SELECT * " +
        //       "FROM marketing_quiz_partner_form " +
        //       "where marketing_quiz_partner_form.Id = " + idpartenerform + "  ; ";



        //    DbConnection.Deconnecter();
        //    if (DbConnection.Connecter())
        //    {
        //        try
        //        {

        //            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
        //            MySqlDataReader reader = cmd.ExecuteReader();

        //            while (reader.Read())
        //            {

        //                try
        //                {
        //                    partnerform = new Partner_Form.Collection(
        //                        Convert.ToInt32(reader["Id"]));
        //                }
        //                catch (Exception ex)
        //                {
        //                    await App.Current.MainPage.DisplayAlert("Warning", "Connetion Time out", "Ok");
        //                    await App.Current.MainPage.Navigation.PopAsync();
        //                }

        //            }

        //            reader.Close();
        //            DbConnection.Deconnecter();
        //        }
        //        catch (Exception ex)
        //        { }

        //    }
        //    return partnerform;
        //}

        /*****************************************update gps**********************************************/
        public static async Task<bool> UpdateGps(int id_partner_form)
        {
            try
            {
                // Vérifier si l'application a la permission d'accéder à la localisation
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    // Demander la permission à l'utilisateur
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                    if (status != PermissionStatus.Granted)
                    {
                        // Si l'utilisateur refuse, on retourne false avec un code spécial
                        return false;
                    }
                }

                // Essayer d'obtenir la localisation rapidement pour vérifier si le service est activé
                bool locationEnabled = false;
                try
                {
                    var quickRequest = new GeolocationRequest(GeolocationAccuracy.Lowest, TimeSpan.FromSeconds(2));
                    var quickLocation = await Geolocation.GetLocationAsync(quickRequest);
                    locationEnabled = quickLocation != null;
                }
                catch (FeatureNotEnabledException)
                {
                    locationEnabled = false;
                }
                catch (Exception)
                {
                    locationEnabled = false;
                }

                // Si la localisation est désactivée, on retourne false
                if (!locationEnabled)
                {
                    return false;
                }

                // Si nous arrivons ici, la localisation devrait être disponible
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    // Utiliser CultureInfo.InvariantCulture pour forcer le point décimal
                    string gpsCoordinates = $"{location.Latitude.ToString(CultureInfo.InvariantCulture)},{location.Longitude.ToString(CultureInfo.InvariantCulture)}";
                    string sqlCmd = "UPDATE marketing_quiz_partner_form SET gps = @Gps WHERE id = @IdPartnerForm;";

                    // Connexion à la base de données
                    DbConnection.Connecter();

                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    cmd.Parameters.AddWithValue("@Gps", gpsCoordinates);
                    cmd.Parameters.AddWithValue("@IdPartnerForm", id_partner_form);

                    cmd.ExecuteNonQuery();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                // Déconnexion de la base de données
                DbConnection.Deconnecter();
            }
        }
        //public static async Task UpdateGps(int id_partner_form)
        //{
        //    try
        //    {
        //        // Récupérer la position actuelle
        //        var location = await Geolocation.GetLocationAsync();
        //        if (location != null)
        //        {
        //            // Utiliser CultureInfo.InvariantCulture pour forcer le point décimal
        //            string gpsCoordinates = $"{location.Latitude.ToString(CultureInfo.InvariantCulture)},{location.Longitude.ToString(CultureInfo.InvariantCulture)}";

        //            string sqlCmd = "UPDATE marketing_quiz_partner_form SET gps = @Gps WHERE id = @IdPartnerForm;";
        //            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
        //            cmd.Parameters.AddWithValue("@Gps", gpsCoordinates);
        //            cmd.Parameters.AddWithValue("@IdPartnerForm", id_partner_form);

        //            // Connexion à la base de données
        //            DbConnection.Connecter();
        //            cmd.ExecuteNonQuery();
        //        }
        //        else
        //        {
        //            Console.WriteLine("La récupération de la position GPS a échoué.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    finally
        //    {
        //        // Déconnexion de la base de données
        //        DbConnection.Deconnecter();
        //    }
        //}
        /********************get gps***************************/
        public static async Task<string> GetGpsCoordinatesFromDatabase(int id_partner_form)
        {
            try
            {
                string gpsCoordinates = null;
                string sqlCmd = "SELECT gps FROM marketing_quiz_partner_form WHERE id = @IdPartnerForm;";

                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                cmd.Parameters.AddWithValue("@IdPartnerForm", id_partner_form);

                // Connexion à la base de données
                DbConnection.Connecter();

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    gpsCoordinates = reader.GetString("gps");
                }

                reader.Close();
                return gpsCoordinates;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                // Déconnexion de la base de données
                DbConnection.Deconnecter();
            }
        }
        /*************Mettre à jour la colonne DeliveryNumber dans ta table commercial_partner(gps)*************/




        //---------------------------------------Added By Sahli Mohamed 10/01/2023 ------------------------------------------------------------------
        public static void UpdateMacAdresse(int id_partner_form)
        {

            string sqlCmd = "update marketing_quiz_partner_form set mac_address = " + user_contrat.GetMACAddress() + " where id = " + id_partner_form + " ;";

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();


        }
        //-------------------------------------------------------------------------------------------------------------------------------------------

        internal static void UpdateEstimatedDate(int id, DateTime start, DateTime begin_date, DateTime end_date, TimeSpan begin_time, TimeSpan end_time)
        {
            if (start > end_date)
            {
                App.Current.MainPage.DisplayAlert("Warning", "estimated date bigger then end date", "OK");
                return;
            }
            string sqlCmd = "Update marketing_quiz_partner_form SET estimated_date='" + start.ToString("yyyy-MM-dd hh:mm:ss") + "',begin_date='" + begin_date.ToString("yyyy-MM-dd") + " " + begin_time + "' ,end_date ='" + end_date.ToString("yyyy-MM-dd") + " " + end_time + "' where Id=" + id + ";";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            DbConnection.Deconnecter();
        }

        public class Collection 
        {
            public int Id { get; set; }
            public int Id_Calender { get; set; }
            public string Form_name { get; set; }
            public int Partner_id { get; set; }
            public string Partner_name { get; set; }
            public string GpsPartner { get; set; }

            public string Gps { get; set; }
            public bool IsGpsAvailable => !string.IsNullOrEmpty(Gps) && !string.IsNullOrEmpty(GpsPartner);

            public bool IsGps1Available => string.IsNullOrEmpty(Gps) && string.IsNullOrEmpty(GpsPartner);

            public bool IsGps2Available => string.IsNullOrEmpty(Gps) && !string.IsNullOrEmpty(GpsPartner);

            public bool IsGps3Available => !string.IsNullOrEmpty(Gps) && string.IsNullOrEmpty(GpsPartner);

            public int? Agent_id { get; set; }
            public string Agent_name { get; set; }
            public int? Cycle_id { get; set; }
            public bool Cycle_IsVisible
            {
                get
                {
                    return Cycle_id.HasValue;
                }

            }
            public string Cycle_name { get; set; }
            public DateTime Begin_date { get; set; }
            public DateTime End_date { get; set; }

            public TimeSpan BeginTime { get; set; }

            public TimeSpan EndTime { get; set; }
            public DateTime Estimated_date { get; set; }
            public DateTime Estimated_date_End
            {
                get
                {
                    return Estimated_date.Add(TimeSpan.FromDays(1));
                }
            }

            //command 
            public AsyncCommand goLocalisation { get; set; }
            /* public Color Estimated_dateColor
             {
                 get
                 {
                     if (Validated == false && Begin_date<DateTime.Now && DateTime.Now<End_date)
                         return Color.FromRgb(255, 203, 60);
                     else if (Validated == false && Begin_date < DateTime.Now && DateTime.Now < End_date && Open_date != null)
                         return Color.FromRgb(255, 83, 83);
                     else
                         return Color.DarkGray;

                 }
             }*/
            public DateTime? Open_date { get; set; }
            public DateTime? Close_date { get; set; }
            public bool Close_date_IsVisible
            {
                get
                {
                    if (Close_date.HasValue)
                        return true;
                    return false;
                }
            }
            public bool Validated { get; set; }
            public string state
            {
                get
                {
                    if (Validated) { return "Validated"; }
                    if (!Validated && DateTime.Now > End_date) { return "Closed"; }
                    if (!Validated && Open_date.HasValue) { return "Opned"; }
                    if (!Validated && DateTime.Now >= Begin_date && DateTime.Now <= End_date) { return "Waiting"; }
                    return null;
                }
            }
            public Color ValidatedColor
            {
                get
                {
                    //if (Validated)
                    //    return Color.Green;
                    //else if (DateTime.Now>End_date)
                    //    return Color.Red;
                    //else
                    //    return Color.Gray;
                    switch (state)
                    {
                        case "Validated":
                            return Colors.Green;
                            break;
                        case "Closed":
                            return Colors.Red;
                            break;
                        case "Opned":
                            return Colors.Orange;
                            break;
                        case "Waiting":
                            return Colors.Gray;
                            break;
                        default:
                            return Colors.White;
                            break;
                    }

                }
            }
            public bool Agent_IsVisible
            {
                get
                {
                    return Agent_id.HasValue;
                }
            }

            public TimeSpan dateDiff { get; set; }
            public string SigneDateDiff { get; set; }
            public string ColorDefDate { get; set; }
            public string messageRestDate { get; set; }

            public Collection() { }
            public Collection(int id, string form_name, int partner_id, string partner_name, int? cycle_id, string cycle_name, DateTime begin_date, DateTime end_date, DateTime estimated_date, DateTime? open_date, DateTime? close_date, bool validated)
            {
                Id = id;
                Form_name = form_name;
                Partner_name = partner_name;
                Partner_id = partner_id;
                Cycle_id = cycle_id;
                Cycle_name = cycle_name;
                Begin_date = begin_date;
                End_date = end_date;
                Estimated_date = estimated_date;
                Open_date = open_date;
                Close_date = close_date;
                Validated = validated;
                BeginTime = begin_date.TimeOfDay;
                EndTime = end_date.TimeOfDay;


                dateDiff = DateTime.Now - Estimated_date.Date;

                if (!Close_date.HasValue)
                    if (DateTime.Now > Estimated_date.Date)
                    {
                        SigneDateDiff = "Ago";
                        ColorDefDate = "Black";
                        messageRestDate = dateDiff.Days + " Days / " + dateDiff.Hours + " Hours / " + dateDiff.Minutes + " m " + SigneDateDiff;
                    }
                    else
                    {
                        SigneDateDiff = "Rest";
                        ColorDefDate = "Green";
                        messageRestDate = dateDiff.Days + " Days / " + dateDiff.Hours + " Hours / " + dateDiff.Minutes + " m " + SigneDateDiff;
                    }
                goLocalisation = new AsyncCommand(async () => await localisationFunction(this.Id, (uint)this.Partner_id));
            }

            /********************gps***************************/
            public Collection(int id, string form_name, int partner_id, string partner_name, int? cycle_id, string cycle_name, DateTime begin_date, DateTime end_date, DateTime estimated_date, DateTime? open_date, DateTime? close_date, bool validated, string gps)
            {
                Id = id;
                Gps = gps;
                Form_name = form_name;
                Partner_name = partner_name;
                Partner_id = partner_id;
                Cycle_id = cycle_id;
                Cycle_name = cycle_name;
                Begin_date = begin_date;
                End_date = end_date;
                Estimated_date = estimated_date;
                Open_date = open_date;
                Close_date = close_date;
                Validated = validated;
                BeginTime = begin_date.TimeOfDay;
                EndTime = end_date.TimeOfDay;


                dateDiff = DateTime.Now - Estimated_date.Date;

                if (!Close_date.HasValue)
                    if (DateTime.Now > Estimated_date.Date)
                    {
                        SigneDateDiff = "Ago";
                        ColorDefDate = "Black";
                        messageRestDate = dateDiff.Days + " Days / " + dateDiff.Hours + " Hours / " + dateDiff.Minutes + " m " + SigneDateDiff;
                    }
                    else
                    {
                        SigneDateDiff = "Rest";
                        ColorDefDate = "Green";
                        messageRestDate = dateDiff.Days + " Days / " + dateDiff.Hours + " Hours / " + dateDiff.Minutes + " m " + SigneDateDiff;
                    }
                goLocalisation = new AsyncCommand(async () => await localisationFunction(this.Id, (uint)this.Partner_id));
            }

            public Collection(int id)
            {
                Id = id;
                goLocalisation = new AsyncCommand(async () => await localisationFunction(this.Id, (uint)this.Partner_id));
            }
            public Collection(int id, string form_name, string partner_name, bool validated)
            {
                Id = id;
                Form_name = form_name;
                Partner_name = partner_name;
                Validated = validated;
                goLocalisation = new AsyncCommand(async () => await localisationFunction(this.Id, (uint)this.Partner_id));
            }
            public Collection(int id, int id_calender, string form_name, int partner_id, string partner_name, int? cycle_id, string cycle_name, DateTime begin_date, DateTime end_date, DateTime estimated_date, DateTime? open_date, DateTime? close_date, bool validated)
            {
                Id = id;
                Id_Calender = id_calender;
                Form_name = form_name;
                Partner_name = partner_name;
                Partner_id = partner_id;
                Cycle_id = cycle_id;
                Cycle_name = cycle_name;
                Begin_date = begin_date;
                End_date = end_date;
                Estimated_date = estimated_date;
                Open_date = open_date;
                Close_date = close_date;
                Validated = validated;
                BeginTime = begin_date.TimeOfDay;
                EndTime = end_date.TimeOfDay;
                dateDiff = DateTime.Now - Estimated_date.Date;
                if (!Close_date.HasValue)
                    if (DateTime.Now > Estimated_date.Date)
                    {
                        SigneDateDiff = "Ago";
                        ColorDefDate = "Black";
                        messageRestDate = dateDiff.Days + " Days / " + dateDiff.Hours + " Hours / " + dateDiff.Minutes + " m " + SigneDateDiff;
                    }
                    else
                    {
                        SigneDateDiff = "Rest";
                        ColorDefDate = "Green";
                        messageRestDate = dateDiff.Days + " Days / " + dateDiff.Hours + " Hours / " + dateDiff.Minutes + " m " + SigneDateDiff;
                    }
                goLocalisation = new AsyncCommand(async () => await localisationFunction(this.Id, (uint)this.Partner_id));


            }
            /******************gps*************************/
            public Collection(int id, int id_calender, string form_name, int partner_id, string partner_name, int? cycle_id, string cycle_name, DateTime begin_date, DateTime end_date, DateTime estimated_date, DateTime? open_date, DateTime? close_date, bool validated, string gps, string gpsPartner)
            {
                Id = id;
                Gps = gps;
                Id_Calender = id_calender;
                Form_name = form_name;
                Partner_name = partner_name;
                Partner_id = partner_id;
                Cycle_id = cycle_id;
                Cycle_name = cycle_name;
                Begin_date = begin_date;
                End_date = end_date;
                Estimated_date = estimated_date;
                Open_date = open_date;
                Close_date = close_date;
                Validated = validated;
                BeginTime = begin_date.TimeOfDay;
                EndTime = end_date.TimeOfDay;
                this.GpsPartner = gpsPartner;

                dateDiff = DateTime.Now - Estimated_date.Date;
                if (!Close_date.HasValue)
                    if (DateTime.Now > Estimated_date.Date)
                    {
                        SigneDateDiff = "Ago";
                        ColorDefDate = "Black";
                        messageRestDate = dateDiff.Days + " Days / " + dateDiff.Hours + " Hours / " + dateDiff.Minutes + " m " + SigneDateDiff;
                    }
                    else
                    {
                        SigneDateDiff = "Rest";
                        ColorDefDate = "Green";
                        messageRestDate = dateDiff.Days + " Days / " + dateDiff.Hours + " Hours / " + dateDiff.Minutes + " m " + SigneDateDiff;
                    }
                goLocalisation = new AsyncCommand(async () => await localisationFunction(this.Id, (uint)this.Partner_id));

            }
            public Collection(int id, int id_calender, string form_name, int partner_id, string partner_name, int? cycle_id, string cycle_name, DateTime begin_date, DateTime end_date, DateTime estimated_date, DateTime? open_date, DateTime? close_date, bool validated, string gps)
            {
                Id = id;
                Gps = gps;
                Id_Calender = id_calender;
                Form_name = form_name;
                Partner_name = partner_name;
                Partner_id = partner_id;
                Cycle_id = cycle_id;
                Cycle_name = cycle_name;
                Begin_date = begin_date;
                End_date = end_date;
                Estimated_date = estimated_date;
                Open_date = open_date;
                Close_date = close_date;
                Validated = validated;
                BeginTime = begin_date.TimeOfDay;
                EndTime = end_date.TimeOfDay;
                dateDiff = DateTime.Now - Estimated_date.Date;
                if (!Close_date.HasValue)
                    if (DateTime.Now > Estimated_date.Date)
                    {
                        SigneDateDiff = "Ago";
                        ColorDefDate = "Black";
                        messageRestDate = dateDiff.Days + " Days / " + dateDiff.Hours + " Hours / " + dateDiff.Minutes + " m " + SigneDateDiff;
                    }
                    else
                    {
                        SigneDateDiff = "Rest";
                        ColorDefDate = "Green";
                        messageRestDate = dateDiff.Days + " Days / " + dateDiff.Hours + " Hours / " + dateDiff.Minutes + " m " + SigneDateDiff;
                    }
                goLocalisation = new AsyncCommand(async () => await localisationFunction(this.Id, (uint)this.Partner_id));

            }

            public Collection(int id, int id_calender, string form_name, int partner_id, string partner_name, int agent_id, string agent_name, int? cycle_id, string cycle_name, DateTime begin_date, DateTime end_date, DateTime estimated_date, DateTime? open_date, DateTime? close_date, bool validated)
            {
                Id = id;
                Id_Calender = id_calender;
                Form_name = form_name;
                Partner_name = partner_name;
                Partner_id = partner_id;
                Agent_id = agent_id;
                Agent_name = agent_name;
                Cycle_id = cycle_id;
                Cycle_name = cycle_name;
                Begin_date = begin_date;
                End_date = end_date;
                Estimated_date = estimated_date;
                Open_date = open_date;
                Close_date = close_date;
                Validated = validated;
                dateDiff = Estimated_date.Date - DateTime.Now;
                BeginTime = Begin_date.TimeOfDay;
                EndTime = End_date.TimeOfDay;
                //if (DateTime.Now > Estimated_date.Date)
                if (!Close_date.HasValue)
                    if (DateTime.Now > Estimated_date.Date)
                    {
                        SigneDateDiff = "Ago";
                        ColorDefDate = "Black";
                        messageRestDate = dateDiff.Days + " Days / " + dateDiff.Hours + " Hours / " + dateDiff.Minutes + " m " + SigneDateDiff;
                    }
                    else
                    {
                        SigneDateDiff = "Rest";
                        ColorDefDate = "Green";
                        messageRestDate = dateDiff.Days + " Days / " + dateDiff.Hours + " Hours / " + dateDiff.Minutes + " m " + SigneDateDiff;
                    }
                goLocalisation = new AsyncCommand(async () => await localisationFunction(this.Id, (uint)this.Partner_id));
            }

            /**********************GPS**************************************/
            public Collection(int id, int id_calender, string form_name, int partner_id, string partner_name, int agent_id, string agent_name, int? cycle_id, string cycle_name, DateTime begin_date, DateTime end_date, DateTime estimated_date, DateTime? open_date, DateTime? close_date, bool validated, string gps, string gpsPartner)
            {
                Id = id;
                Gps = gps;
                Id_Calender = id_calender;
                Form_name = form_name;
                Partner_name = partner_name;
                Partner_id = partner_id;
                Agent_id = agent_id;
                Agent_name = agent_name;
                Cycle_id = cycle_id;
                Cycle_name = cycle_name;
                Begin_date = begin_date;
                End_date = end_date;
                Estimated_date = estimated_date;
                Open_date = open_date;
                Close_date = close_date;
                Validated = validated;
                dateDiff = Estimated_date.Date - DateTime.Now;
                BeginTime = Begin_date.TimeOfDay;
                EndTime = End_date.TimeOfDay;
                this.GpsPartner = gpsPartner;

                //if (DateTime.Now > Estimated_date.Date)
                if (!Close_date.HasValue)
                    if (DateTime.Now > Estimated_date.Date)
                    {
                        SigneDateDiff = "Ago";
                        ColorDefDate = "Black";
                        messageRestDate = dateDiff.Days + " Days / " + dateDiff.Hours + " Hours / " + dateDiff.Minutes + " m " + SigneDateDiff;
                    }
                    else
                    {
                        SigneDateDiff = "Rest";
                        ColorDefDate = "Green";
                        messageRestDate = dateDiff.Days + " Days / " + dateDiff.Hours + " Hours / " + dateDiff.Minutes + " m " + SigneDateDiff;
                    }
                goLocalisation = new AsyncCommand(async () => await localisationFunction(this.Id, (uint)this.Partner_id));

            }

            /*   public async Task localisationFunction()
               {
                   UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                   await Task.Delay(500);



                   if (await DbConnection.Connecter3())
                   {

                       await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new LocationView(this.Gps)));


                   }
                   else
                   {
                       await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "OK");
                   }

                   UserDialogs.Instance.HideLoading();


               }*/

            /********************gps***************************/

            public async Task InitGpsAsync()
            {
                this.GpsPartner = await Partner.GetGpsCoordinates((uint)this.Partner_id);
            }



            public async Task localisationFunction(int id_partner_form, uint id_partner)
            {
                UserDialogs.Instance.ShowLoading("Loading Please wait ...");
                await Task.Delay(500);

                if (await DbConnection.Connecter3())
                {
                    string gps1 = await Partner_Form.GetGpsCoordinatesFromDatabase(id_partner_form);
                    string gps2 = await Partner.GetGpsCoordinates(id_partner);

                    // Si une des coordonnées est nulle, on peut transmettre l'autre coordonnée seule
                    if (!string.IsNullOrEmpty(gps1) || !string.IsNullOrEmpty(gps2))
                    {
                        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new LocationView(gps1, gps2)));
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Erreur", "Impossible de récupérer les coordonnées", "OK");
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "OK");
                }

                UserDialogs.Instance.HideLoading();
            }



        }


        public class State
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public State()
            {
            }

            public State(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }
    }
}
