using MvvmHelpers.Commands;
using MySqlConnector;
using System.ComponentModel;

namespace SmartPharma5.Model
{
    public class Form
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string Memo { get; set; }
        public bool Enable { get; set; }
        public bool IsGpsEnable { get; set; }

        public AsyncCommand CreateForm { get; }

        public uint FormTypeId { get; set; }
        public Form(uint id, string name, bool enable, bool isGpsEnable, uint formTypeId)
        {
            Id = id;
            Name = name;
            Enable = enable;
            IsGpsEnable = isGpsEnable;
            FormTypeId = formTypeId;
            CreateForm = new AsyncCommand(createFormFonc);
        }
        private async Task createFormFonc()
        {

        }
        public static async Task<BindingList<Form>> GetAllForms()
        {

            string sqlCmd = "SELECT * From marketing_quiz_form; ";

            BindingList<Form> list = new BindingList<Form>();


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
                            list.Add(new Form(
                                Convert.ToUInt32(reader["Id"]),
                                reader["name"].ToString(),
                                Convert.ToBoolean(reader["enable"]),
                                Convert.ToBoolean(reader["is_gps_enable"]),
                                Convert.ToUInt32(reader["form_type_id"])));
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
                {
                    
                }
            }
            return list;
        }

        public static async Task<BindingList<Form>> GetFormsByPartnerCategory(uint Partner_Category)
        {
            string sqlCmd = "SELECT * From marketing_quiz_form left join marketing_quiz_form_partner_category on marketing_quiz_form.Id=marketing_quiz_form_partner_category.form_id" +
                " where marketing_quiz_form_partner_category.partner_category_id = " + Partner_Category + "  and marketing_quiz_form.enable=1; ";

            BindingList<Form> list = new BindingList<Form>();


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
                            list.Add(new Form(
                                Convert.ToUInt32(reader["Id"]),
                                reader["name"].ToString(),
                                Convert.ToBoolean(reader["enable"]),
                                Convert.ToBoolean(reader["is_gps_enable"]),
                                Convert.ToUInt32(reader["form_type_id"])));
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

    }
}
