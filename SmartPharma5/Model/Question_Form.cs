
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
    public class Question_Form
    {
        public uint Id { get; set; }
        public uint Form_Id { get; set; }
        public uint Question_Id { get; set; }
        public Question_Form(uint id, uint form_Id, uint question_Id)
        {
            Id = id;
            Form_Id = form_Id;
            Question_Id = question_Id;
        }
        public static async Task<BindingList<Question_Form>> GetQuestionForm(int id)
        {
            string sqlCmd = " SELECT * FROM atooerp.marketing_quiz_form_question where form_id = " + id + "; ";
            BindingList<Question_Form> list = new BindingList<Question_Form>();


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
                            list.Add(new Question_Form(
                                Convert.ToUInt32(reader["id"]),
                                Convert.ToUInt32(reader["form_Id"]),
                                Convert.ToUInt32(reader["question_Id"])));



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

        internal void GetQuestion()
        {
            throw new NotImplementedException();
        }
    }
}
