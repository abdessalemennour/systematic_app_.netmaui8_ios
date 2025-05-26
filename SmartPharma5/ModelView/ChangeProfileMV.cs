//using Acr.UserDialogs;
using Acr.UserDialogs;
using DevExpress.Maui.Editors;
using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.Model;
using SmartPharma5.View;

namespace SmartPharma5.ViewModel
{
    class ChangeProfileMV : BaseViewModel
    {

        private List<ProfileAttributes> listAttributes;
        public List<ProfileAttributes> ListAttributes { get => listAttributes; set => SetProperty(ref listAttributes, value); }

        public int id_instance_profile { get; set; }
        public int id_partner { get; set; }
        private Dictionary<ComboBoxEdit, object> comboBoxStates = new Dictionary<ComboBoxEdit, object>();

        private void ComboBox_Unfocused(object sender, FocusEventArgs e)
        {
            var comboBox = sender as ComboBoxEdit;
            if (comboBox != null)
            {
                // Sauvegarde de l'état de la ComboBox
                comboBoxStates[comboBox] = comboBox.SelectedItem;
            }
        }


        /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
        Avant :
                public static AsyncCommand UpdateList { get; set; }






                public AsyncCommand saveData { get; set; }

                public AsyncCommand DeleteInstance { get; set; }
                public ChangeProfileMV(int id,int id_instance ,int id_partner)
        Après :
                public static AsyncCommand UpdateList { get; set; }






                public AsyncCommand saveData { get; set; }

                public AsyncCommand DeleteInstance { get; set; }
                public ChangeProfileMV(int id,int id_instance, int id_partner)
        */
        public static AsyncCommand UpdateList { get; set; }






        public AsyncCommand saveData { get; set; }

        public AsyncCommand DeleteInstance { get; set; }
        public ChangeProfileMV(int id, int id_instance, int id_partner)
        {
            UpdateList = new AsyncCommand(UpdateListFonction);
            saveData = new AsyncCommand(SaveAttributes);
            DeleteInstance = new AsyncCommand(delete_instance);
            try
            {
                this.id_instance_profile = id_instance;
                this.id_partner = id_partner;
                ListAttributes = ProfileAttributes.GetAttributesProfile2(id).Result;

            }
            catch (Exception ex)
            {

            }


        }
        private async Task UpdateListFonction()
        {

        }
        private async Task delete_instance()
        {

            if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOY WANT TO DELETE TEMP INSTANCE", "YES", "NO"))
            {
                try
                {
                    await ProfileAttributes.deleteInstanceProfileTemp(id_instance_profile);
                    await App.Current.MainPage.Navigation.PopAsync();
                }
                catch (Exception ex)
                {

                }
            }

        }

        private async Task SaveAttributes()
        {

            if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOY WANT TO SAVE CHANGES", "YES", "NO"))
            {
                //Test IsNull
                foreach (ProfileAttributes attribute in ListAttributes)
                {
                    if (attribute.is_null == false)
                    {
                        if (attribute.HasString && attribute.String_value == null)
                        {
                            await App.Current.MainPage.DisplayAlert("Warnning", attribute.LabelName + " is Required", "OK");
                            return;

                        }
                        else if (attribute.HasBool && attribute.Bool_value == null)
                        {
                            await App.Current.MainPage.DisplayAlert("Warnning", attribute.LabelName + " is Required", "OK");
                            return;
                        }
                        else if (attribute.HasDate && attribute.Date_value == null)
                        {
                            await App.Current.MainPage.DisplayAlert("Warnning", attribute.LabelName + " is Required", "OK");
                            return;
                        }
                        else if (attribute.HasNumber && attribute.Number_value == null)
                        {
                            await App.Current.MainPage.DisplayAlert("Warnning", attribute.LabelName + " is Required", "OK");
                            return;
                        }
                        else if (attribute.HasDecimal && attribute.Number_value == null)
                        {
                            await App.Current.MainPage.DisplayAlert("Warnning", attribute.LabelName + " is Required", "OK");
                            return;
                        }
                        else if (attribute.HasMultiple && attribute.Selected_item == null)
                        {
                            await App.Current.MainPage.DisplayAlert("Warnning", attribute.LabelName + " is Required", "OK");
                            return;
                        }

                    }




                }


                foreach (ProfileAttributes attribute in ListAttributes)
                {




                    if (await DbConnection.Connecter3())
                    {
                        string sqlCmd = "";

                        if (attribute.HasString)
                        {
                            if(attribute.String_value != null)
                            {
                                sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,string_value) values (" + attribute.Id + "," + id_instance_profile + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + attribute.String_value + "');";

                            }
                          
                        }
                        else if (attribute.HasBool)
                        {
                            sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,boolean_value) values (" + attribute.Id + "," + id_instance_profile + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + attribute.Bool_value + ");";
                        }
                        else if (attribute.HasDate && attribute.Date_value.ToString() != "")
                        {
                            sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,date_value) values (" + attribute.Id + "," + id_instance_profile + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + attribute.Date_value?.ToString("yyyy-MM-dd HH:mm:ss") + "');";
                        }
                        else if (attribute.HasNumber && attribute.Number_value.ToString() != "")
                        {
                            sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,int_value) values (" + attribute.Id + "," + id_instance_profile + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + Convert.ToInt64(attribute.Number_value) + ");";
                        }
                        else if (attribute.HasDecimal && attribute.Number_value.ToString() != "")
                        {
                            sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,decimal_value) values (" + attribute.Id + "," + id_instance_profile + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + Convert.ToDecimal(attribute.Number_value) + ");";
                        }
                        else if (attribute.HasMultiple)
                        {
                            if(attribute.Selected_item != null)
                            {
                                sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,type) values (" + attribute.Id + "," + id_instance_profile + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + attribute.Selected_item.id + ");";


                            }
                           

                        }
                        else
                        {

                        }
                        try
                        {
                            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                            cmd.ExecuteScalar();

                        }
                        catch (Exception ex)
                        {



                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Warnning", "Failed Connection", "OK");
                        return;

                    }





                }
                await App.Current.MainPage.DisplayAlert("DONE", "PROFILE CHANGE SUCCEFULY!", "OK");
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                await App.Current.MainPage.Navigation.PopAsync();
                UserDialogs.Instance.HideLoading();
            }


        }
    }
}
