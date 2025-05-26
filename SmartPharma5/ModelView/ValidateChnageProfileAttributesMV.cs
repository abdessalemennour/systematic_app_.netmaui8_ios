//using Acr.UserDialogs;
using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.Model;
using SmartPharma5.View;

namespace SmartPharma5.ViewModel
{
    class ValidateChnageProfileAttributesMV : BaseViewModel
    {

        public int Id { get; set; }
        public int Id_Partner { get; set; }
        public int Id_Profile { get; set; }
        private List<ValidateAttributeProfile> listAttributes;
        public List<ValidateAttributeProfile> ListAttributes { get => listAttributes; set => SetProperty(ref listAttributes, value); }

        public AsyncCommand ValidateAttrubutes { get; }
        public AsyncCommand RefuseAttrubutes { get; }

        public ValidateChnageProfileAttributesMV(int id, int id_partner, int profile)
        {
            this.Id = id;
            this.Id_Partner = id_partner;
            this.Id_Profile = profile;
            ValidateAttrubutes = new AsyncCommand(validateAttrubutes);
            RefuseAttrubutes = new AsyncCommand(refuseAttrubutes);

            try
            {
                ListAttributes = new List<ValidateAttributeProfile>(ValidateAttributeProfile.GetAllAttributesProfile(this.Id).Result);

            }
            catch (Exception ex)
            {

            }
        }

        private async Task refuseAttrubutes()
        {
            if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO REFUSE THIS CHANGES!", "Yes", "No"))
            {
                try
                {

                    string sqlCmd = "update marketing_profile_instance_temp set state=3  where id=" + this.Id + ";";
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    cmd.ExecuteScalar();

                }
                catch (Exception ex)
                {

                }

                await App.Current.MainPage.DisplayAlert("DONE", "PROFILE CHANGED REFUSED", "OK");
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                var tabbedPage = new TabPageValidationUpdates();

                // Accédez à l'onglet "PartnerTempView" (index 2)
                tabbedPage.CurrentPage = tabbedPage.Children[1];

                // Utilisez la navigation pour afficher la TabbedPage
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(tabbedPage));
                UserDialogs.Instance.HideLoading();


            }
        }

        private async Task validateAttrubutes()
        {

            if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO SAVE THIS CHANGES!", "Yes", "No"))
            {
                try
                {
                    int New_Instance = 0;
                    string sqlCmd = "insert into marketing_profile_instances(create_date,partner,profil) values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + Id_Partner + "," + Id_Profile + ");select max(id) from marketing_profile_instances;";
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    try
                    {
                        New_Instance = int.Parse(cmd.ExecuteScalar().ToString());

                    }
                    catch (Exception ex)
                    {

                    }

                    sqlCmd = "update marketing_profile_instance_temp set instance=" + New_Instance + " where id=" + Id + " ;";
                    foreach (ValidateAttributeProfile attribute in ListAttributes)
                    {

                        if (attribute.Valid_attribute)
                        {
                            if (attribute.HasString)
                            {
                                sqlCmd = sqlCmd + "insert into marketing_profile_attribut_value(create_date,profile_instance,attribut,string_value) values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + New_Instance + "," + attribute.Id_Attribute + ",'" + attribute.String_value + "');";

                            }
                            else if (attribute.HasInt)
                            {
                                sqlCmd = sqlCmd + "insert into marketing_profile_attribut_value(create_date,profile_instance,attribut,int_value) values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + New_Instance + "," + attribute.Id_Attribute + "," + attribute.Int_value + ");";

                            }
                            else if (attribute.HasDecimal)
                            {

                                sqlCmd = sqlCmd + "insert into marketing_profile_attribut_value(create_date,profile_instance,attribut,decimal_value) values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + New_Instance + "," + attribute.Id_Attribute + "," + attribute.Decimal_value.ToString().Replace(',', '.') + ");";


                            }
                            else if (attribute.HasDate)
                            {
                                sqlCmd = sqlCmd + "insert into marketing_profile_attribut_value(create_date,profile_instance,attribut,date_value) values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + New_Instance + "," + attribute.Id_Attribute + ",'" + attribute.Date_value?.ToString("yyyy-MM-dd HH:mm:ss") + "');";


                            }
                            else if (attribute.HasBool)
                            {
                                sqlCmd = sqlCmd + "insert into marketing_profile_attribut_value(create_date,profile_instance,attribut,boolean_value) values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + New_Instance + "," + attribute.Id_Attribute + "," + attribute.Boolean_value + ");";


                            }
                            else if (attribute.HasType)
                            {
                                sqlCmd = sqlCmd + "insert into marketing_profile_attribut_value(create_date,profile_instance,attribut,type) values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + New_Instance + "," + attribute.Id_Attribute + "," + attribute.Type_value + ");";
                            }
                            else
                            {

                            }
                           
                        }

                    }
                    sqlCmd = sqlCmd + "update marketing_profile_instance_temp set state=2  where id=" + this.Id + ";";
                    cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    try
                    {
                        cmd.ExecuteScalar();

                    }
                    catch (Exception ex)
                    {

                    }

                }
                catch (Exception ex)
                {

                }
                await App.Current.MainPage.DisplayAlert("DONE", "PROFILE CHANGED SUCCEFULY", "OK");

                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                var tabbedPage = new TabPageValidationUpdates();

                // Accédez à l'onglet "PartnerTempView" (index 2)
                tabbedPage.CurrentPage = tabbedPage.Children[1];

                // Utilisez la navigation pour afficher la TabbedPage
                await App.Current.MainPage.Navigation.PushAsync( new NavigationPage(tabbedPage));
                UserDialogs.Instance.HideLoading();


            }

        }



    }
}
