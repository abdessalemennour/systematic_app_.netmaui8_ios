using MvvmHelpers;
using MySqlConnector;
//using SmartPharma5.Model;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
//using Xamarin.Essentials;
//using static SQLite.SQLite3;

namespace SmartPharma5.Model
{
    public class User : ObservableObject
    {
        public int Id { get; set; }
        public DateTime create_date { get; set; }
        string login;
        public string Login
        {
            get { return login; }
            set
            {
                SetProperty(ref login, value);

            }
        }
        string password;
        public string Password
        {
            get { return password; }
            set
            {
                SetProperty(ref password, value);

            }
        }
        public string barcode { get; set; }
        public BindingList<User_module_groupe> UMGList { get; set; }
        public DataTable permessionList { get; set; }
        public string skins { get; set; }
        public int user_type { get; set; }
        public byte[] stamp { get; set; }
        public string email { get; set; }
        public string email_smtpserver { get; set; }
        public string email_smtpserver_port { get; set; }
        public bool email_smtpserver_enable_ssl { get; set; }
        public string email_password { get; set; }
        public string email_signature { get; set; }
        public byte[] email_signature_picture { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string job_position { get; set; }
        public int sex { get; set; }
        public string key_dallas { get; set; }
        public bool? actif { get; set; } = false;
        public uint employe { get; set; }
        public User()
        { }

        //public User(string login, string password)
        //{
        //    // Utilisez des paramètres pour éviter l'injection SQL
        //    string sqlCmd = @"SELECT atooerp_user.*, hr_employe.Id as employe, hr_employe.actif 
        //             FROM atooerp_user 
        //             LEFT JOIN hr_employe ON hr_employe.user = atooerp_user.Id 
        //             WHERE atooerp_user.login = @login AND atooerp_user.password = @password";

        //    try
        //    {
        //        using (var connection = new MySqlConnection(DbConnection.con.ConnectionString))
        //        {
        //            var command = new MySqlCommand(sqlCmd, connection);
        //            command.Parameters.AddWithValue("@login", login);
        //            command.Parameters.AddWithValue("@password", password); // Assurez-vous que le mot de passe est hashé

        //            var adapter = new MySqlDataAdapter(command);
        //            var dt = new DataTable();
        //            adapter.Fill(dt);

        //            if (dt.Rows.Count == 0)
        //                throw new Exception("Utilisateur non trouvé ou mot de passe incorrect");

        //            // Remplissage des propriétés
        //            this.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
        //            this.create_date = Convert.ToDateTime(dt.Rows[0]["create_date"]);
        //            this.login = dt.Rows[0]["login"].ToString();
        //            this.password = dt.Rows[0]["password"].ToString();
        //            this.barcode = dt.Rows[0]["barcode"].ToString();
        //            this.skins = dt.Rows[0]["skins"].ToString();
        //            this.user_type = Convert.ToInt32(dt.Rows[0]["type"]);
        //            this.stamp = dt.Rows[0]["stamp"] is byte[]? (byte[])dt.Rows[0]["stamp"] : (byte[])null;
        //            this.email = dt.Rows[0]["email"].ToString();
        //            this.email_smtpserver = dt.Rows[0]["email_smtpserver"].ToString();
        //            this.email_smtpserver_port = dt.Rows[0]["email_smtpserver_port"].ToString();
        //            this.email_smtpserver_enable_ssl = Convert.ToBoolean(dt.Rows[0]["email_smtpserver_enable_ssl"]);
        //            this.email_password = dt.Rows[0]["email_password"].ToString();
        //            this.email_signature = dt.Rows[0]["email_signature"].ToString();
        //            this.email_signature_picture = dt.Rows[0]["email_signature_picture"] is byte[]? (byte[])dt.Rows[0]["email_signature_picture"] : (byte[])null;
        //            this.first_name = dt.Rows[0]["first_name"].ToString();
        //            this.last_name = dt.Rows[0]["last_name"].ToString();
        //            this.job_position = dt.Rows[0]["job_position"].ToString();
        //            this.sex = Convert.ToInt32(dt.Rows[0]["sex"]);
        //            this.key_dallas = dt.Rows[0]["key_dallas"].ToString();
        //            this.actif = Convert.ToBoolean(dt.Rows[0]["actif"]);
        //            this.employe = Convert.ToUInt32(dt.Rows[0]["employe"]);
        //        }
        //    }
        //    catch (MySqlException ex)
        //    {
        //        // Journalisez l'erreur proprement
        //        Debug.WriteLine($"Erreur MySQL: {ex.Message}");
        //        throw new Exception("Erreur de connexion à la base de données", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Erreur: {ex.Message}");
        //        throw;
        //    }
        //}

        public User(string login, string password)
        {
            string sqlCmd = " SELECT atooerp_user.*,hr_employe.Id as employe,hr_employe.actif FROM atooerp_user LEFT join " +
                "hr_employe on hr_employe.user= atooerp_user.Id " +
                "where atooerp_user.login = " + login + " and atooerp_password =" + password + ";";
            try
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con.ConnectionString);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                Avant :
                                BindingList<User_module_groupe> gridDataList = new BindingList<User_module_groupe>();

                                this.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                Après :
                                BindingList<User_module_groupe> gridDataList = new BindingList<User_module_groupe>();

                                this.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                */
                BindingList<User_module_groupe> gridDataList = new BindingList<User_module_groupe>();

                this.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                this.create_date = Convert.ToDateTime(dt.Rows[0]["create_date"]);
                this.login = dt.Rows[0]["login"].ToString();
                this.password = dt.Rows[0]["password"].ToString();
                this.barcode = dt.Rows[0]["barcode"].ToString();
                this.skins = dt.Rows[0]["skins"].ToString();
                this.user_type = Convert.ToInt32(dt.Rows[0]["type"]);
                this.stamp = dt.Rows[0]["stamp"] is byte[]? (byte[])dt.Rows[0]["stamp"] : (byte[])null;
                this.email = dt.Rows[0]["email"].ToString();
                this.email_smtpserver = dt.Rows[0]["email_smtpserver"].ToString();
                this.email_smtpserver_port = dt.Rows[0]["email_smtpserver_port"].ToString();
                this.email_smtpserver_enable_ssl = Convert.ToBoolean(dt.Rows[0]["email_smtpserver_enable_ssl"]);
                this.email_password = dt.Rows[0]["email_password"].ToString();
                this.email_signature = dt.Rows[0]["email_signature"].ToString();
                this.email_signature_picture = dt.Rows[0]["email_signature_picture"] is byte[]? (byte[])dt.Rows[0]["email_signature_picture"] : (byte[])null;
                this.first_name = dt.Rows[0]["first_name"].ToString();
                this.last_name = dt.Rows[0]["last_name"].ToString();
                this.job_position = dt.Rows[0]["job_position"].ToString();
                this.sex = Convert.ToInt32(dt.Rows[0]["sex"]);
                this.key_dallas = dt.Rows[0]["key_dallas"].ToString();
                this.actif = Convert.ToBoolean(dt.Rows[0]["actif"]);
                this.employe = Convert.ToUInt32(dt.Rows[0]["employe"]);

                //User_module_groupe.getListeByUser(this);
            }
            catch
            { }
        }
        public async Task<bool> LoginTrue(string login1,string password1)
        {
            bool result = false;
            string sqlCmd = " SELECT atooerp_user.*,hr_employe.Id as employe,hr_employe.actif FROM atooerp_user LEFT join " +
                "hr_employe on hr_employe.user= atooerp_user.Id " +
                "where atooerp_user.login = '" + login1 + "' and atooerp_user.password ='" + password1 + "';";
            try
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    this.actif = (bool?)Convert.ToBoolean(dt.Rows[0]["actif"]);
                    if (this.actif == null)
                        result = false;
                    else
                        result = (bool)this.actif;

                    this.employe = Convert.ToUInt32(dt.Rows[0]["employe"]);
                    this.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                }
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Warning", "Sorry you are not allowed to use this App", "Ok");
            }
            if (actif == true)
            {

            //    Preferences.Set("idagent", employe);
            //    Preferences.Set("iduser", Id);
            //     User_module_groupe.getListeByUser(this.Id) ;
            }
            return result;
        }
        public bool LoginTrue()
        {
            bool result = false;
            string sqlCmd = " SELECT atooerp_user.*,hr_employe.Id as employe,hr_employe.actif FROM atooerp_user LEFT join " +
                "hr_employe on hr_employe.user= atooerp_user.Id " +
                "where atooerp_user.login = '" + login + "' and atooerp_user.password ='" + password + "';";
            try
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    this.actif = (bool?)Convert.ToBoolean(dt.Rows[0]["actif"]);
                    if (this.actif == null)
                        result = false;
                    else
                        result = (bool)this.actif;

                    this.employe = Convert.ToUInt32(dt.Rows[0]["employe"]);
                    this.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                }
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Warning", "Sorry you are not allowed to use this App", "Ok");
            }
            if (actif == true)
            {

                Preferences.Set("idagent", employe);
                Preferences.Set("iduser", Id);
                User_module_groupe.getListeByUser(this.Id);
            }
            return result;
        }

        public static bool? UserIsActif(uint idEmploye)
        {
            bool actif = false;
            if (DbConnection.Connecter())
            {
                string sqlCmd = " SELECT *  FROM hr_employe WHERE Id = " + idEmploye + ";";
                try
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        actif = dt.Rows[0]["actif"] is bool ? Convert.ToBoolean(dt.Rows[0]["actif"]) : false;

                    }
                }
                catch (Exception ex)
                {
                    App.Current.MainPage.DisplayAlert("Warning", "Sorry you are not allowed to use this App", "Ok");
                }
            }
            else
            {
                return null;
            }
            return actif;
        }
    }
}
