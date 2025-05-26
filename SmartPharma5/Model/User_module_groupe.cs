
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.Services;
using MySqlConnector;
using SQLite;
Après :
using MySqlConnector;
using SmartPharma5.Services;
using SQLite;
*/
using MySqlConnector;
using SmartPharma5.Services;
using System.
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
Après :
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;
using System.Threading.Tasks;
*/
Data;


namespace SmartPharma5.Model
{
    public partial class User_module_groupe
    {
        public int Id { get; set; }
        [SQLite.Column("IdUser")]
        public int IdUser { get; set; }
        [SQLite.Column("IdModule")]
        public int IdModule { get; set; }
        [SQLite.Column("IdGroup")]
        public int IdGroup { get; set; }
        public string nameModule { get; set; }

        public User_module_groupe()
        { }
        public User_module_groupe(int Id, int IdUser, int IdModule, int IdGroup)
        {
            this.Id = Id;
            this.IdUser = IdUser;
            this.IdModule = IdModule;
            this.IdGroup = IdGroup;
        }

     
        public async static void getListeByUser(int iduser)
        {

            /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
            Avant :
                        string sqlCmd = "SELECT Id ,atooerp_user_module_group.user, module,atooerp_user_module_group.group FROM atooerp_user_module_group WHERE(atooerp_user_module_group.user = "+iduser+");";

                        try
            Après :
                        string sqlCmd = "SELECT Id ,atooerp_user_module_group.user, module,atooerp_user_module_group.group FROM atooerp_user_module_group WHERE(atooerp_user_module_group.user = "+iduser+");";

                        try
            */
            string sqlCmd = "SELECT Id ,atooerp_user_module_group.user, module,atooerp_user_module_group.group FROM atooerp_user_module_group WHERE(atooerp_user_module_group.user = " + iduser + ");";

            try
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
                adapter.SelectCommand.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                try
                {
                    await User_Module_Groupe_Services.DeleteAll();
                }
                catch (Exception ex)
                {


                    /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                    Avant :
                                    }

                                    foreach (DataRow row in dt.Rows)
                    Après :
                                    }

                                    foreach (DataRow row in dt.Rows)
                    */
                }

                foreach (DataRow row in dt.Rows)
                {
                    await User_Module_Groupe_Services.




                            Adddb(new User_module_groupe(
                        Convert.ToInt32(row["Id"]),
                        Convert.ToInt32(row["user"]),
                        Convert.ToInt32(row["module"]),
                        Convert.ToInt32(row["group"])));
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Warning", ex.ToString(), "Ok");
            }
        }
    }
}
