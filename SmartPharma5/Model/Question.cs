using MySqlConnector;
using System.ComponentModel;

namespace SmartPharma5.Model
{
    public class Question
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public DateTime Create_Date { get; set; }
        public string Memo { get; set; }
        public string By_ref { get; set; }
        public bool Is_Multi { get; set; }
        public bool Is_Null { get; set; }
        public bool Enable { get; set; }
        public string Question_Text { get; set; }
        public uint System_Type_Id { get; set; }
        public uint Question_Type_Id { get; set; }
        public uint Input_editor { get; set; }

        public bool Has_Req { get; set; }
        public Question()
        { }

        public Question(uint id, uint system_Type_Id)
        {
            Id = id;
            System_Type_Id = system_Type_Id;
        }
        public Question(uint id, bool is_Multi, bool is_Null, bool enable, string question_Text, uint question_type, uint system_Type_Id, uint input_editor, string by_ref)
        {
            Id = id;
            Is_Multi = is_Multi;
            Is_Null = is_Null;
            Enable = enable;
            Question_Text = question_Text;
            Question_Type_Id = question_type;
            System_Type_Id = system_Type_Id;
            Input_editor = input_editor;
            By_ref = by_ref;
        }
        public static BindingList<Question> GetQuestionById(int id)
        {
            string sqlCmd = "SELECT * FROM atooerp.marketing_quiz_question where Id = " + id + ";";
            BindingList<Question> list = new BindingList<Question>();


            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {

                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    try
                    {
                        list.Add(new Question(
                            Convert.ToUInt32(reader["idQuestion"]),
                            Convert.ToBoolean(reader["is_multi"]),
                            Convert.ToBoolean(reader["is_null"]),
                            Convert.ToBoolean(reader["enable"]),
                            reader["question_Text"].ToString(),
                            Convert.ToUInt32(reader["question_type_id"]),
                            Convert.ToUInt32(reader["system_type_id"]),
                            Convert.ToUInt32(reader["input_editors_id"]),
                            "test"

                            ));



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

            return list;
        }
        public async static Task<BindingList<Question>> GetPartnerFormQuestionByFormId(int id)
        {
            BindingList<Question> list = new BindingList<Question>();
            MySqlDataReader reader = null;
            string sqlCmd = " SELECT marketing_quiz_question.Id as idQuestion , is_multi, is_null, marketing_quiz_question.enable, question_text, question_type_id, system_type_id, input_editors_id , atooerp_type.by_request " +
                " FROM marketing_quiz_question " +
                " left outer join marketing_quiz_form_question On marketing_quiz_question.Id = marketing_quiz_form_question.question_id " +
                " left outer join marketing_quiz_partner_form On marketing_quiz_form_question.form_id = marketing_quiz_partner_form.form_id" +
                " left outer join marketing_quiz_form On marketing_quiz_partner_form.form_id = marketing_quiz_form.Id " +
                "  LEFT outer join atooerp_type On marketing_quiz_question.system_type_id = atooerp_type.Id" +
                " where marketing_quiz_partner_form.Id = " + id + " and marketing_quiz_question.enable= 1 order by question_rank ASC; ";

            // DbConnection.Deconnecter();
            if (await DbConnection.Connecter3())
            {

                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        list.Add(new Question(
                            Convert.ToUInt32(reader["idQuestion"]),
                            Convert.ToBoolean(reader["is_multi"]),
                            Convert.ToBoolean(reader["is_null"]),
                            Convert.ToBoolean(reader["enable"]),
                            reader["question_Text"].ToString(),
                            Convert.ToUInt32(reader["question_type_id"]),
                            Convert.ToUInt32(reader["system_type_id"]),
                            Convert.ToUInt32(reader["input_editors_id"]),
                            reader["by_request"].ToString()

                            ));



                    }

                }
                catch (Exception ex)
                {
                    reader.Close();
                    return null;
                }
                reader.Close();

            }
            else
            {
                return null;
            }

            return list;


        }
    }
}
