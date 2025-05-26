
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using SmartPharma5.Model;
using MvvmHelpers;
Après :
using MvvmHelpers;
using MySqlConnector;
*/
using MvvmHelpers;
using MySqlConnector;
using SmartPharma5.Model;
using System.
/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System.ComponentModel;
Après :
using System.Collections.ObjectModel;
*/
ComponentModel;

/* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
Avant :
using System.Text;
using System.Threading.Tasks;
//using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
Après :
using System.Data;
//using Xamarin.Forms;
using Xamarin.Forms;
using System.Text;
using System.Threading.Tasks;
*/
//using System.Linq;
//using DevExpress.XamarinForms.CollectionView.Internal;

namespace SmartPharma5.ViewModel
{
    public class QuizQuestionViewModel : BaseViewModel
    {
        public Partner_Form.Collection partner_form { get; set; }
        public int question_number { get; set; }
        public BindingList<Question> QuestionList { get; set; } = new BindingList<Question>();
        public ObservableRangeCollection<Type_element> ElementList { get; set; } = new ObservableRangeCollection<Type_element>();
        public ObservableRangeCollection<Response> ResponseList { get; set; } = new ObservableRangeCollection<Response>();
        //private ObservableRangeCollection<Response> responseList;
        //public ObservableRangeCollection<Response> ResponseList { get => responseList; set => SetProperty(ref responseList, value); }


        private bool testConnection;
        public bool TestConnection { get => testConnection; set => SetProperty(ref testConnection, value); }

        private bool btnShow;
        public bool BtnShow { get => btnShow; set => SetProperty(ref btnShow, value); }

        private bool save_config;
        public bool Save_config { get => save_config; set => SetProperty(ref save_config, value); }
        
        public QuizQuestionViewModel()
        {
        }
        public QuizQuestionViewModel(int id_partner_form)
        {
            partner_form = Partner_Form.GetPartnerFormById(id_partner_form);
            BtnShow = false;
            LoadQuestionFromAsync();
        }
        public QuizQuestionViewModel(Partner_Form.Collection partner_Form)
        {
            partner_form = partner_Form;
            BtnShow = false;



            LoadQuestionFromAsync();
        }


        public async Task LoadQuestionFromAsync()
        {
            try
            {
                bool testCon = true;
                BtnShow = false;

                try
                {

                    QuestionList = await Question.GetPartnerFormQuestionByFormId((int)partner_form.Id);
                }
                catch (Exception ex)
                {

                    //TestConnection = true;
                    testCon = false;
                    //return;


                    /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                    Avant :
                                }



                                var finalQuery = QuestionList
                    Après :
                                }



                                var finalQuery = QuestionList
                    */
                }



                var finalQuery = QuestionList
                .GroupBy(category => category.System_Type_Id)
                .Select(grouping => new Question { System_Type_Id = grouping.Key, Id = 0 });

                BindingList<Question> Items = new BindingList<Question>(finalQuery.ToList());

                foreach (Question question in Items)
                    if (question.System_Type_Id > 6)
                    {
                        try
                        {
                            ElementList.AddRange(await Type_element.GetElementByTypeId(question.System_Type_Id));
                        }
                        catch (Exception ex)
                        {
                            //TestConnection = true;
                            testCon = false;
                            // return;
                        }

                    }
                try
                {
                    ResponseList.AddRange(await Response.GetresponseByFormAsync(partner_form));
                }
                catch (Exception ex)
                {
                    //TestConnection = true;
                    testCon = false;
                    //return;
                }
                if (testCon == false)
                {

                    /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                    Avant :
                                    TestConnection = true;

                                    QuestionList =new  BindingList<Question>();
                    Après :
                                    TestConnection = true;

                                    QuestionList =new  BindingList<Question>();
                    */
                    TestConnection = true;

                    QuestionList = new BindingList<Question>();
                    ResponseList = new ObservableRangeCollection<Response>();
                    ElementList = new ObservableRangeCollection<Type_element>();
                    return;


                }
                BtnShow = true;


            }
            catch (Exception ex)
            {

            }
          



        }



        public void Submit(string response, int formId, int questionId)

        {

            string sqlCmd = "INSERT INTO marketing_quiz_response (marketing_quiz_question_id, marketing_quiz_response) VALUES ('" + question_number + "');";

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            MySqlDataReader MyReader2;
            DbConnection.con.Open();
            MyReader2 = cmd.ExecuteReader();
            DbConnection.Deconnecter();


        }


    }
}
