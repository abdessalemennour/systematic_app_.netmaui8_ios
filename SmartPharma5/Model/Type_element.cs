
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
    public class Type_element
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string Memo { get; set; }
        public uint Parent { get; set; }
        public uint Type_id { get; set; }

        public int QuestionId { get; set; }
        public Type_element(uint id, string name, uint typeId)
        {
            Id = id;
            Name = name;
            Type_id = typeId;
        }

        public async static Task<BindingList<Type_element>> GetElementByRequest(string req)
        {
            string sqlCmd = req;
            BindingList<Type_element> list = new BindingList<Type_element>();
            MySqlDataReader reader = null;
            DbConnection.Deconnecter();
            if (await DbConnection.Connecter3())
            {


                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {


                        list.Add(new Type_element(
                            reader.GetUInt32("Id"),
                            reader["name"].ToString(),
                            0
                            ));

                    }

                    reader.Close();
                    DbConnection.Deconnecter();


                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;
                }



            }
            else { return null; }
            return list;
        }



        public async static Task<BindingList<Type_element>> GetElementByTypeId(uint id)
        {
            string sqlCmd = " SELECT * FROM atooerp_type_element where type_id =" + id + " group by Id;";
            BindingList<Type_element> list = new BindingList<Type_element>();
            MySqlDataReader reader = null;
            DbConnection.Deconnecter();
            if (await DbConnection.Connecter3())
            {


                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {


                        list.Add(new Type_element(
                            reader.GetUInt32("Id"),
                            reader["name"].ToString(),
                            reader.GetUInt32("type_id")
                            ));

                    }

                    reader.Close();
                    DbConnection.Deconnecter();


                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;
                }



            }
            else { return null; }
            return list;
        }
    }
}
