
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.Model;
using MySqlConnector;
using System;
Après :
using MySqlConnector;
using SmartPharma5.Model;
using System;
*/
using MySqlConnector;
using System.ComponentModel;

namespace SmartPharma5.Model
{
    public class Type
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public DateTime Create_Date { get; set; }
        public string Memo { get; set; }
        public bool System { get; set; }
        public uint Parent { get; set; }

        public Type() { }
        public Type(uint id, string name, DateTime create_date, string memo, bool system, uint parent)
        {
            Id = id;
            Name = name;
            Create_Date = create_date;
            Memo = memo;
            System = system;
            Parent = parent;
        }
        public Type(uint id, bool system, uint parent)
        {
            Id = id;
            System = system;
            Parent = parent;
        }

        public static async Task<BindingList<Type>> GetResponseTypes(int Id)
        {
            string sqlCmd = "SELECT * FROM atooerp.marketing_quiz_response_type where Id = '" + Id + "';";
            BindingList<Type> list = new BindingList<Type>();


            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {

                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    try
                    {
                        //list.Add(new Type(
                        //    Convert.ToUInt32(reader["Id"]),
                        //    reader["name"].ToString(),
                        //    Convert.ToDateTime(reader["create_Date"].ToString()),
                        //    reader["Memo"].ToString(),
                        //    reader["type_Name"].ToString()));



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

            return list;
        }
    }
}
