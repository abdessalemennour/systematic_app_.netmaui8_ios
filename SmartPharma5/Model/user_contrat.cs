using MySqlConnector;
using System.Net.NetworkInformation;
//using Xamarin.Essentials;

namespace SmartPharma5.Model
{
    public class user_contrat
    {

        public static int iduser
        {
            get
            {
                return Preferences.Get("iduser", 0);
            }
        }

        public static string nameuser { get; set; }

        public static int contract { get; set; }

        public static int id_employe { get; set; }

        public static bool contactIsValidated { get; set; }

        public static bool isHrResponsable { get; set; }

        public static bool isHrEmployes { get; set; }

        public static string name_exercice { get; set; }

        public static decimal day_off_number { get; set; }

        public static decimal day_off_use { get; set; }
        public static decimal day_off_rest { get; set; }

        public static int id_actual_period { get; set; }

        public static string name_actual_period { get; set; }

        public static int id_actual_exercice { get; set; }

        public static string name_actual_exercice { get; set; }

        public static decimal pay_slip_day_off_number { get; set; }

        public static bool ValiderRequestWithPaySlip { get; set; }

        public static bool isShearchWithName { get; set; } = true;

        public static bool isShearchWithDate { get; set; }

        public static bool cantDeposit { get; set; }

        public static bool HasNoContract { get; set; } = true;


        //Permmison attributes

        public static List<string> ListModules { get; set; }

        public static List<string> ListGroupes { get; set; }


        public static List<string> ListInVisibleBtn { get; set; }




        static user_contrat()
        {

            // getInfo();


        }
        public  static byte[] GetLogoFromDatabase()
        {
            byte[] logoBytes = null;
            if ( DbConnection.Connecter2())
            {
                string querry = "SELECT logo FROM atooerp_socity;";

                try
                {
                    MySqlCommand command = new MySqlCommand(querry, DbConnection.con);


                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                logoBytes = (byte[])reader.GetValue(0);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions here
                    return null;
                }
                finally
                {
                    // Ensure the reader is closed in case of an exception
                    DbConnection.con.Close();
                }
            }
            else
            {
                // Handle the case when the database connection fails
                return null;
            }

            return logoBytes;

        }

        public static void getResponsabilities()
        {
            string sqlCmd = "select  count( atooerp_user_module_group.id) as number from atooerp_user_module_group  where atooerp_user_module_group.group = 20 and atooerp_user_module_group.module =9 and atooerp_user_module_group.user =" + Preferences.Get("iduser", 0) + " ;";
            DbConnection.Deconnecter();
            DbConnection.Connecter();
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            if (Convert.ToInt32(reader["number"]) == 0)
            {
                isHrResponsable = false;

            }
            else
            {
                isHrResponsable = true;
            }

            reader.Close();
            DbConnection.Deconnecter();


            /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
            Avant :
                    }

                    public async static Task getActualExercice()
            Après :
                    }

                    public async static Task getActualExercice()
            */
        }

        public async static Task getActualExercice()
        {
            string sqlCmd = " select hr_exercice.id , hr_exercice.name from hr_exercice where now()>= hr_exercice.beginning_date and now()< hr_exercice.end_date order by id desc;";
            DbConnection.Deconnecter();
            DbConnection.Connecter();
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            id_actual_exercice = Convert.ToInt32(reader["id"]);
            name_actual_exercice = Convert.ToString(reader["name"]);
            reader.Close();
            DbConnection.Deconnecter();

        }

        public async static Task getActualPeriod()
        {
            string sqlCmd = "select hr_period.Id , hr_period.name from hr_period where now()>= hr_period.beginning_date and now()< hr_period.end_date order by id desc;";
            DbConnection.Deconnecter();
            DbConnection.Connecter();
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();

            reader.Read();

            id_actual_period = Convert.ToInt32(reader["Id"]);
            name_actual_period = Convert.ToString(reader["name"]);





            reader.Close();
            DbConnection.Deconnecter();

        }
        public async static Task<bool> CheckContratValide()
        {
            string sqlCmd = "SELECT count(hr_employe.Id) as number FROM hr_employe left join hr_contract on hr_contract.employe= hr_employe.Id " +
                "where now()>=hr_contract.beginning_date and coalesce(hr_contract.disabled_date,hr_contract.end_date,'3000-01-01 00:00:00') and hr_employe.user =" + Preferences.Get("iduser", 0) + ";";
            DbConnection.Deconnecter();
            DbConnection.Connecter();
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            if (Convert.ToInt32(reader["number"]) == 0)
            {
                reader.Close();
                DbConnection.Deconnecter();

                return false;
            }
            else
            {
                reader.Close();
                DbConnection.Deconnecter();

                return true;
            }




        }

        public async static Task getModules()
        {
            string sqlCmd = "select lower(atooerp_module.name) as name  from atooerp_user_module_group\r\nleft join atooerp_module on atooerp_module.Id=atooerp_user_module_group.module\r\nwhere atooerp_user_module_group.user=" + iduser + ";";
            if (await DbConnection.Connecter3())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                MySqlDataReader reader = cmd.ExecuteReader();

                ListModules = new List<string>();

                while (reader.Read())
                {
                    try
                    {
                        string module = Convert.ToString(reader["name"]);
                        ListModules.Add(module);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                reader.Close();
            }




        }


        public async static Task getBtnIInvisibles()
        {
            string sqlCmd1 = "select distinct(lower(atooerp_app_control.name)) as name from atooerp_app_control " +
                "left join atooerp_app_group_control on atooerp_app_group_control.app_control=atooerp_app_control.id " +
                "where atooerp_app_group_control.group in (select atooerp_group.id from atooerp_group,atooerp_user_module_group where atooerp_user_module_group.group=atooerp_group.id and atooerp_user_module_group.user= " + iduser + ");";
            if (await DbConnection.Connecter3())
            {
                ListInVisibleBtn = new List<string>();

                MySqlCommand cmd1 = new MySqlCommand(sqlCmd1, DbConnection.con);
                try
                {
                    MySqlDataReader reader1 = cmd1.ExecuteReader();
                    while (reader1.Read())
                    {
                        try
                        {
                            ListInVisibleBtn.Add(Convert.ToString(reader1["name"]));

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                    }
                    reader1.Close();
                }
                catch (Exception ex)
                {

                }


            }
        }
        public async static Task getInfo()
        {
            try
            {
                string sqlCmd = "select hr_employe.id as employe , count(hr_contract.Id ) as number , hr_contract.id as id_contract, atooerp_person.first_name as first_name\r\n, atooerp_person.last_name as last_name ,hr_exercice.name as name_exercice, hr_day_of_exercice.day_off_number ,hr_day_of_exercice.day_off_use\r\n, hr_day_of_exercice.pay_slip_day_off_number\r\nfrom hr_employe\r\nleft join atooerp_user on hr_employe.user = atooerp_user.id and atooerp_user.id=" + iduser + " \r\nleft join atooerp_person on atooerp_person.id=hr_employe.id\r\nleft join hr_contract on hr_contract.employe=hr_employe.id and hr_contract.validated = 1 and hr_contract.disabled = 0\r\nleft join hr_day_of_exercice on hr_day_of_exercice.contract = hr_contract.id\r\nleft join hr_exercice on hr_day_of_exercice.exercice = hr_exercice.id\r\nwhere  hr_employe.user=" + iduser + ";";




                if (await DbConnection.Connecter3())
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);


                    MySqlDataReader reader = cmd.ExecuteReader();



                    reader.Read();



                    try
                    {
                        id_employe = Convert.ToInt32(reader["employe"]);
                    }
                    catch (Exception ex)
                    {

                    }




                    if (Convert.ToString(reader["number"]) == "0")
                    {
                        HasNoContract = false;

                    }
                    else
                    {

                        if (Convert.ToString(reader["id_contract"]) == "")
                        {
                            contract = 0;

                        }
                        else
                        {
                            contract = Convert.ToInt32(reader["id_contract"]);

                        }
                        /**********************************************/


                        if (Convert.ToString(reader["day_off_number"]) == "")
                        {
                            day_off_number = 0;

                        }
                        else
                        {
                            day_off_number = Convert.ToDecimal(reader["day_off_number"]);

                        }
                        /********************************************/



                        if (Convert.ToString(reader["day_off_use"]) == "")
                        {
                            day_off_use = 0;

                        }
                        else
                        {
                            day_off_use = Convert.ToInt32(reader["day_off_use"]);

                        }
                        /****************************************************/

                        if (Convert.ToString(reader["pay_slip_day_off_number"]) == "")
                        {
                            pay_slip_day_off_number = 0;

                        }
                        else
                        {
                            pay_slip_day_off_number = Convert.ToInt32(reader["pay_slip_day_off_number"]);

                        }
                    }
                    /******************************************************/

                    nameuser = Convert.ToString(reader["first_name"]) + " " + Convert.ToString(reader["last_name"]);
                    name_exercice = Convert.ToString(reader["name_exercice"]);



                    day_off_rest = day_off_number - day_off_use;

                    reader.Close();

                }

            }
            catch (Exception ex)
            {

            }






        }
        public static async void valider_request_with_pay_slip()
        {
            await getActualPeriod();
            getInfo();

            string sqlCmd = " select count(hr_pay_slip.id) as number from hr_pay_slip where hr_pay_slip.contarct = " + user_contrat.contract + " and hr_pay_slip.period =" + user_contrat.id_actual_period + ";";

            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (Convert.ToInt32(reader["number"]) == 0)
            {
                cantDeposit = false;

            }
            else
            {
                cantDeposit = true;
            }



        }

        public static string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }





    }
}
