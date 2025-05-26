
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5;
Après :
using MySqlConnector;
using SmartPharma5;
*/
using
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using MySqlConnector;
using System;
Après :
using System;
*/
MySqlConnector;
using System.ComponentModel;

namespace SmartPharma5.Model
{
    public class Response
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public DateTime Create_Date { get; set; }
        public string Memo { get; set; }
        public string Response_String { get; set; }
        public int? Response_Int { get; set; }
        public decimal? Response_Decimal { get; set; }
        public byte[] Response_Blob { get; set; }
        public DateTime? Response_Date { get; set; }
        public bool? Response_Bool { get; set; }
        public uint QuestionId { get; set; }
        public uint? Type_Element_Id { get; set; }
        public uint Partner_Form_Id { get; set; }

        public Response() { }
        public Response(uint id, string name, DateTime create_Date, string memo)
        {
            Id = id;
            Name = name;
            Create_Date = create_Date;
            Memo = memo;

        }
        public Response(uint id, uint questionid, uint partner_form, string response_string, uint? type_element_id, int? response_int, decimal? response_decimal, DateTime? response_date, bool? response_bool)
        {
            Id = id;
            QuestionId = questionid;
            Partner_Form_Id = partner_form;
            Response_String = response_string;
            Type_Element_Id = type_element_id;
            Response_Int = response_int;
            Response_Decimal = response_decimal;
            Response_Date = response_date;
            Response_Bool = response_bool;
        }
        public Response(uint questionid, string response_string, uint? type_element_id, int? response_int, decimal? response_decimal, DateTime? response_date, bool? response_bool)
        {
            QuestionId = questionid;
            Response_String = response_string;
            Type_Element_Id = type_element_id;
            Response_Int = response_int;
            Response_Decimal = response_decimal;
            Response_Date = response_date;
            Response_Bool = response_bool;
        }
        public Response(string response_string, int response_int, decimal? response_decimal, DateTime? response_date, uint questionid)
        {
            Response_String = response_string;
            Response_Int = response_int;
            Response_Decimal = response_decimal;
            Response_Date = response_date;
            QuestionId = questionid;
        }
        public Response(string response_string)
        {

            Response_String = response_string;
        }
        public static async Task<BindingList<Response>> GetresponseByFormAsync(Partner_Form.Collection partner_Form)
        {
            BindingList<Response> list = new BindingList<Response>();

            string sqlCmd = "SELECT * FROM marketing_quiz_response where partner_form_id = " + partner_Form.Id + ";";
            MySqlDataReader reader = null;


            // DbConnection.Deconnecter();
            if (await DbConnection.Connecter3())
            {

                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {

                        list.Add(new Response(
                            Convert.ToUInt32(reader["Id"]),
                            Convert.ToUInt32(reader["question_id"]),
                            Convert.ToUInt32(reader["partner_form_id"]),
                            reader["response_value_string"].ToString(),
                            reader["type_element_id"] is uint ? Convert.ToUInt32(reader["type_element_id"]) : (uint?)null,
                            reader["response_value_int"] is int ? Convert.ToInt32(reader["response_value_int"]) : (int?)null,
                            reader["response_value_decimal"] is decimal ? Convert.ToDecimal(reader["response_value_decimal"]) : (decimal?)null,
                            reader["response_value_date"] is DateTime ? Convert.ToDateTime(reader["response_value_date"]) : (DateTime?)null,
                            reader["response_value_bool"] is bool ? Convert.ToBoolean(reader["response_value_bool"]) : (bool?)null));



                    }

                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;
                }



                reader.Close();
                DbConnection.Deconnecter();
            }
            else
            {
                return null;
            }
            return list;
        }
        public static void Insert(List<Response> list, Partner_Form.Collection partner_Form)
        {
            DbConnection.Connecter();
            string sqlCmd = "Delete from marketing_quiz_response where partner_form_id = " + partner_Form.Id + ";";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            try
            {
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            DbConnection.Deconnecter();
            foreach (var line in list)
            {

                try
                {
                    sqlCmd = "INSERT INTO marketing_quiz_response SET create_date= NOW(),question_id= " + line.QuestionId + ",partner_form_id=" + partner_Form.Id + ",response_value_string='" + line.Response_String + "',type_element_id=" + (line.Type_Element_Id is null ? "null" : line.Type_Element_Id.ToString()) + ",response_value_int=" + (line.Response_Int is null ? "null" : line.Response_Int.ToString()) + ",response_value_decimal=" + (line.Response_Decimal is null ? "null" : line.Response_Decimal.ToString()) + ",response_value_date=" + (line.Response_Date is null ? "null" : ("'" + Convert.ToDateTime(line.Response_Date.ToString()).ToString("yyyy-MM-dd hh:mm:ss") + "'")) + ",response_value_bool=" + (line.Response_Bool is null ? "null" : line.Response_Bool.ToString()) + ";";
                    cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    DbConnection.Connecter();
                    cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                DbConnection.Deconnecter();

            }

        }


        public static void Insert(List<Response> list, Partner_Form.Collection partner_Form, DateTime estilmated_date, TimeSpan estilmated_time, DateTime Start_date, TimeSpan Start_time, DateTime End_date, TimeSpan End_time)
        {
            DbConnection.Connecter();
            string sqlCmd = "Delete from marketing_quiz_response where partner_form_id = " + partner_Form.Id + ";";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            try
            {
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            DbConnection.Deconnecter();
            foreach (var line in list)
            {

                try
                {
                    //string sqlCmd1 = "Update marketing_quiz_partner_form SET estimated_date='" + start.ToString("yyyy-MM-dd hh:mm:ss") + "',begin_date='" + begin_date.ToString("yyyy-MM-dd") + " " + begin_time + "' ,end_date ='" + end_date.ToString("yyyy-MM-dd") + " " + end_time + "' where Id=" + id + ";";

                    sqlCmd = "INSERT INTO marketing_quiz_response SET create_date= NOW(),question_id= " + line.QuestionId + ",partner_form_id=" + partner_Form.Id + ",response_value_string='" + line.Response_String + "',type_element_id=" + (line.Type_Element_Id is null ? "null" : line.Type_Element_Id.ToString()) + ",response_value_int=" + (line.Response_Int is null ? "null" : line.Response_Int.ToString()) + ",response_value_decimal=" + (line.Response_Decimal is null ? "null" : line.Response_Decimal.ToString()) + ",response_value_date=" + (line.Response_Date is null ? "null" : ("'" + Convert.ToDateTime(line.Response_Date.ToString()).ToString("yyyy-MM-dd hh:mm:ss") + "'")) + ",response_value_bool=" + (line.Response_Bool is null ? "null" : line.Response_Bool.ToString()) + ";";

                    cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    DbConnection.Connecter();
                    cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                DbConnection.Deconnecter();

            }
            if (list.Count > 0)
            {
                try
                {
                    string sqlCmdUpdate = "Update marketing_quiz_partner_form SET end_date= '" + End_date.ToString("yyyy-MM-dd") + " " + End_time + "' ,begin_date= '" + Start_date.ToString("yyyy-MM-dd") + " " + Start_time + "'  ,estimated_date = '" + estilmated_date.ToString("yyyy-MM-dd") + " " + estilmated_time + "'  where Id=" + partner_Form.Id + ";";
                    cmd = new MySqlCommand(sqlCmdUpdate, DbConnection.con);
                    DbConnection.Connecter();
                    cmd.ExecuteScalar();
                }
                catch(Exception ex)
                {

                }
              

            }

        }
    }
}
