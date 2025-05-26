//using Acr.UserDialogs;
using Acr.UserDialogs;
using DevExpress.Data;
using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.Model;
using SmartPharma5.View;
using System.Collections.Generic;

namespace SmartPharma5.ViewModel
{
    class AddPartnerTempMV : BaseViewModel
    {
        private string inputName="";
        public string InputName { get => inputName; set => SetProperty(ref inputName, value); } 



        private List<string> listRequired;
        public List<string> ListRequired { get => listRequired; set => SetProperty(ref listRequired, value); }


        private List<string> listNotVisible;
        public List<string> ListNotVisible { get => listNotVisible; set => SetProperty(ref listNotVisible, value); }

        private bool actPop;
        public bool ActPop { get => actPop; set => SetProperty(ref actPop, value); } 

        private string inputEmail;
        public string InputEmail { get => inputEmail; set => SetProperty(ref inputEmail, value); }

        private string inputCountry;
        public string InputCountry { get => inputCountry; set => SetProperty(ref inputCountry, value); }

        private string inputCity;
        public string InputCity { get => inputCity; set => SetProperty(ref inputCity, value); }

        private string inputStreet;
        public string InputStreet { get => inputStreet; set => SetProperty(ref inputStreet, value); }

        private string inputNumber;
        public string InputNumber { get => inputNumber; set => SetProperty(ref inputNumber, value); }

        private string inputPostaleCode;
        public string InputPostaleCode { get => inputPostaleCode; set => SetProperty(ref inputPostaleCode, value); }

        private string inputFax;
        public string InputFax { get => inputFax; set => SetProperty(ref inputFax, value); }

        private int inputCategory;
        public int InputCategory { get => inputCategory; set => SetProperty(ref inputCategory, value); }

        private string inputState;
        public string InputState { get => inputState; set => SetProperty(ref inputState, value); }


        private bool inputCustomer;
        public bool InputCustomer { get => inputCustomer; set => SetProperty(ref inputCustomer, value); } 


        private bool inputSupplier;
        public bool InputSupplier { get => inputSupplier; set => SetProperty(ref inputSupplier, value); }

        private string vat_code;
        public string Vat_Code { get => vat_code; set => SetProperty(ref vat_code, value); }
        private string gps;
        public string Gps { get => gps; set => SetProperty(ref gps, value); }
        public AsyncCommand HomePage { get; }

        private List<Comercial_category> catgory_list;
        public List<Comercial_category> Catgory_list { get => catgory_list; set => SetProperty(ref catgory_list, value); }


        private Comercial_category selected_category;
        public Comercial_category Selected_category { get => selected_category; set => SetProperty(ref selected_category, value); }


        private List<Profile> profiles;
        public List<Profile> Profiles { get => profiles; set => SetProperty(ref profiles, value); }

        private bool hasSigleProfile;
        public bool HasSigleProfile { get => hasSigleProfile; set => SetProperty(ref hasSigleProfile, value); }

        private bool hasMultiProfile;
        public bool HasMultiProfile { get => hasMultiProfile; set => SetProperty(ref hasMultiProfile, value); }


        private Profile selected_profile;
        public Profile Selected_profile { get => selected_profile; set => SetProperty(ref selected_profile, value); }

        private List<ProfileAttributes> listAttributes;
        public List<ProfileAttributes> ListAttributes { get => listAttributes; set => SetProperty(ref listAttributes, value); }

        public AsyncCommand AddPartnerTemp { get; }
        public AsyncCommand ChangeProfile { get; }

        public static MySqlCommand cmd;

 


        private static async Task<List<string>?> getRequiredAttributes()
        {
            List<string> list = new List<string>();
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "SELECT * FROM atooerp_app_commercial_partner_temp_configuration c WHERE c.is_null=0;";
                MySqlDataReader reader = null;
                try
                {
                    cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(Convert.ToString(reader["attribute_name"])); 
                    }
                    
                    reader.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    reader.Close();
                    return list;
                }
                DbConnection.Deconnecter();
                reader.Close();
            }
            return list;
        }

        private static async Task<List<string>> getIsNotVisibleAttributes()
        {
            List<string> list = new List<string>();
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "SELECT * FROM atooerp_app_commercial_partner_temp_configuration c WHERE c.is_visible=0;";
                MySqlDataReader reader = null;
                try
                {
                    cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(Convert.ToString(reader["attribute_name"]));
                    }

                    reader.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    reader.Close();
                    return list;
                }
                DbConnection.Deconnecter();
                reader.Close();
            }
            return list;
        }

        private async Task<Boolean> ExistVatCode(string vat_code)
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "select count(id) as num from commercial_partner where vat_code= '" + vat_code + "';";
                MySqlDataReader reader = null;
                try
                {
                    cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (Convert.ToInt16(reader["num"]) != 0)
                        {
                            return true;

                        }
                        else
                        
                        {
                            return false;
                        }


                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    //await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "Ok");
                    reader.Close();

                }


                DbConnection.Deconnecter();
                reader.Close();
            }
            return false;

        }



        public  AddPartnerTempMV()
        {
            InputCustomer = true;
            ActPop = false;
            HomePage = new AsyncCommand(homePage);
            try
            {
                ChangeProfile = new AsyncCommand(changeProfileMV);
                AddPartnerTemp = new AsyncCommand(InsertPartnerTemp);
                ListRequired = getRequiredAttributes().Result;
                ListNotVisible = getIsNotVisibleAttributes().Result;

            }
            catch (Exception ex)
            {

            }


            try
            {
                Catgory_list = Comercial_category.GetCommercialCategory().Result;
                Profiles = Profile.getAllProfile().Result;
                
                if (Profiles.Count == 1)
                {
                    HasMultiProfile = false;
                    HasSigleProfile = true;
                    this.Selected_profile = Profiles[0];
                 
                    try
                    {

                        ListAttributes = new List<ProfileAttributes>();
                        ListAttributes =  ProfileAttributes.GetAttributesProfile2(Convert.ToInt32(this.Selected_profile.Id)).Result;
                        ActPop = false;


                    }
                    catch (Exception ex)
                    {

                    }

                }
                else if(Profiles.Count > 1)
                {
                    HasMultiProfile = true;
                    HasSigleProfile = false;
                }
                else
                {
                    HasMultiProfile = false;
                    HasSigleProfile = false;
                }

                

            }
            catch (Exception ex)
            {

            }

        }
        private async Task homePage()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);

            try
            {

                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }

            UserDialogs.Instance.HideLoading();

        }

     
        

        private async Task InsertPartnerTemp()
        {

            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            try
            {

                try
                {

                    try
                    {
                        if(selected_category == null)
                        {
                            await App.Current.MainPage.DisplayAlert("Warning", "Must Choose Category first", "Ok");
                            UserDialogs.Instance.HideLoading();
                            return;
                        }
                        foreach (var item in ListRequired)
                        {
                            if (item == "vat_code" && string.IsNullOrEmpty(Vat_Code?.ToString()))
                            {
                                await App.Current.MainPage.DisplayAlert("Warning", "Must Write the vat code first", "Ok");
                                UserDialogs.Instance.HideLoading();
                                return;
                            }
                            else if (item == "name" && (InputName.ToString() == "" | InputName == null))
                            {
                                await App.Current.MainPage.DisplayAlert("Warning", "Must Write the name first", "Ok");
                                UserDialogs.Instance.HideLoading();
                                return;
                            }
                            else if (item == "country" && (InputCountry.ToString() == "" | InputCountry == null))
                            {
                                await App.Current.MainPage.DisplayAlert("Warning", "Must Write the country first", "Ok");
                                UserDialogs.Instance.HideLoading();
                                return;
                            }
                            else if (item == "state" && (InputState.ToString() == "" | InputState == null))
                            {
                                await App.Current.MainPage.DisplayAlert("Warning", "Must Write the state first", "Ok");
                                UserDialogs.Instance.HideLoading();
                                return;
                            }
                            else if (item == "postale_code" && (InputPostaleCode.ToString() == "" | InputPostaleCode == null))
                            {
                                await App.Current.MainPage.DisplayAlert("Warning", "Must Write the postale code first", "Ok");
                                UserDialogs.Instance.HideLoading();
                                return;
                            }
                            else if (item == "email" && (InputEmail.ToString() == "" | InputEmail == null))
                            {
                                await App.Current.MainPage.DisplayAlert("Warning", "Must Write the email first", "Ok");
                                UserDialogs.Instance.HideLoading();
                                return;
                            }
                            else if (item == "fax" && (InputFax.ToString() == "" | InputFax == null))
                            {
                                await App.Current.MainPage.DisplayAlert("Warning", "Must Write fax first", "Ok");
                                UserDialogs.Instance.HideLoading();
                                return;
                            }
                            else if (item == "gps" && (InputFax.ToString() == "" | InputFax == null))
                            {
                                await App.Current.MainPage.DisplayAlert("Warning", "Must Write fax first", "Ok");
                                UserDialogs.Instance.HideLoading();
                                return;
                            }

                            /*   else if (item == "category" && Selected_category == null)
                               {
                                   await App.Current.MainPage.DisplayAlert("Warning", "Must choose a category first", "Ok");
                                   UserDialogs.Instance.HideLoading();
                                   return;
                               }*/
                            else
                            {
                                UserDialogs.Instance.HideLoading();

                            }
                        }

                    }
                    catch (Exception ex )
                    {
                        
                    }


                    if (Selected_profile != null )
                    {
                        foreach (ProfileAttributes attribute in listAttributes)
                        {

                            if (attribute.is_null == false)
                            {
                                if (attribute.HasString && attribute.String_value == null)
                                {
                                    await App.Current.MainPage.DisplayAlert("Warnning", attribute.LabelName + " is Required", "OK");
                                    UserDialogs.Instance.HideLoading();

                                    return;

                                }
                                else if (attribute.HasBool && attribute.Bool_value == null)
                                {
                                    await App.Current.MainPage.DisplayAlert("Warnning", attribute.LabelName + " is Required", "OK");
                                    UserDialogs.Instance.HideLoading();

                                    return;
                                }
                                else if (attribute.HasDate && attribute.Date_value == null)
                                {
                                    await App.Current.MainPage.DisplayAlert("Warnning", attribute.LabelName + " is Required", "OK");
                                    UserDialogs.Instance.HideLoading();

                                    return;
                                }
                                else if (attribute.HasNumber && attribute.Number_value == null)
                                {
                                    await App.Current.MainPage.DisplayAlert("Warnning", attribute.LabelName + " is Required", "OK");
                                    UserDialogs.Instance.HideLoading();

                                    return;
                                }
                                else if (attribute.HasDecimal && attribute.Number_value == null)
                                {
                                    await App.Current.MainPage.DisplayAlert("Warnning", attribute.LabelName + " is Required", "OK");
                                    UserDialogs.Instance.HideLoading();

                                    return;
                                }
                                else if (attribute.HasMultiple && attribute.selected_item == null)
                                {
                                    await App.Current.MainPage.DisplayAlert("Warnning", attribute.LabelName + " is Required", "OK");
                                    UserDialogs.Instance.HideLoading();

                                    return;
                                }

                            }
                        }

                        int? p = await ProfileAttributes.InsertTempPartner(InputName, InputCountry, InputStreet, InputCity, InputState, InputPostaleCode, InputEmail, InputFax, Selected_category.id, InputCustomer, InputSupplier, Vat_Code, Gps);
                        int? id_instance = await ProfileAttributes.InsertInstanceWithTempPartner(p, Selected_profile.Id);
                        foreach (ProfileAttributes attribute in listAttributes)
                        {
                            string sqlCmd = "";
                            if (attribute.HasString && attribute.String_value != "")
                            {
                                sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,string_value) values (" + attribute.Id + "," + id_instance + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + attribute.String_value + "');";
                            }
                            else if (attribute.HasBool)
                            {
                                sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,boolean_value) values (" + attribute.Id + "," + id_instance + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + attribute.Bool_value + ");";
                            }
                            else if (attribute.HasDate && attribute.Date_value.ToString() != "")
                            {
                                sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,date_value) values (" + attribute.Id + "," + id_instance + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + attribute.Date_value?.ToString("yyyy-MM-dd HH:mm:ss") + "');";
                            }
                            else if (attribute.HasNumber && attribute.Number_value.ToString() != "")
                            {
                                sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,int_value) values (" + attribute.Id + "," + id_instance + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + Convert.ToInt32(attribute.Number_value) + ");";
                            }
                            else if (attribute.HasDecimal && attribute.Number_value.ToString() != "")
                            {
                                sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,decimal_value) values (" + attribute.Id + "," + id_instance + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + attribute.Number_value + ");";
                            }
                            else if (attribute.HasMultiple && attribute.Multiple_value.ToString() != "" && attribute.Selected_item is not null)
                            {


                                sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,type) values (" + attribute.Id + "," + id_instance + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + attribute.Selected_item.id + ");";
                            }
                            try
                            {
                                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                                cmd.ExecuteScalar();

                            }
                            catch (Exception ex)
                            {
                                //await App.Current.MainPage.DisplayAlert("Warnning", "Failed Connection", "OK");
                                //return;


                            }

                        }

                    }else if (Profiles.Count == 1)
                    {

                        int? p = await ProfileAttributes.InsertTempPartner(InputName, InputCountry, InputStreet, InputCity, InputState, InputPostaleCode, InputEmail, InputFax, Selected_category.id, InputCustomer, InputSupplier, Vat_Code, Gps);
                        int? id_instance = await ProfileAttributes.InsertInstanceWithTempPartner(p, Profiles[0].Id ) ;
                        foreach (ProfileAttributes attribute in listAttributes)
                        {
                            string sqlCmd = "";
                            if (attribute.HasString && attribute.String_value != "")
                            {
                                sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,string_value) values (" + attribute.Id + "," + id_instance + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + attribute.String_value + "');";
                            }
                            else if (attribute.HasBool)
                            {
                                sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,boolean_value) values (" + attribute.Id + "," + id_instance + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + attribute.Bool_value + ");";
                            }
                            else if (attribute.HasDate && attribute.Date_value.ToString() != "")
                            {
                                sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,date_value) values (" + attribute.Id + "," + id_instance + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + attribute.Date_value?.ToString("yyyy-MM-dd HH:mm:ss") + "');";
                            }
                            else if (attribute.HasNumber && attribute.Number_value.ToString() != "")
                            {
                                sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,int_value) values (" + attribute.Id + "," + id_instance + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + Convert.ToInt32(attribute.Number_value) + ");";
                            }
                            else if (attribute.HasDecimal && attribute.Number_value.ToString() != "")
                            {
                                sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,decimal_value) values (" + attribute.Id + "," + id_instance + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + attribute.Number_value + ");";
                            }
                            else if (attribute.HasMultiple && attribute.Multiple_value.ToString() != "" && attribute.Selected_item is not null)
                            {


                                sqlCmd = "insert into marketing_profile_attribut_value_temp(profile_attribute,profile_instance_temp,user,employe,state,create_date,type) values (" + attribute.Id + "," + id_instance + "," + user_contrat.iduser + "," + user_contrat.id_employe + ",1,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + attribute.Selected_item.id + ");";
                            }
                            try
                            {
                                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                                cmd.ExecuteScalar();

                            }
                            catch (Exception ex)
                            {
                                //await App.Current.MainPage.DisplayAlert("Warnning", "Failed Connection", "OK");
                                //return;


                            }

                        }
                    }
                    else
                    {
                        int? p = await ProfileAttributes.InsertTempPartner(InputName, InputCountry, InputStreet, InputCity, InputState, InputPostaleCode, InputEmail, InputFax, Selected_category.id, InputCustomer, InputSupplier, Vat_Code, Gps);

                    }

                }
                catch (Exception ex)
                {
                    

                }
               
            }
            catch(Exception ex)
            {

            }

           
            await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomeView()));
            //await App.Current.MainPage.Navigation.PopAsync();
            UserDialogs.Instance.HideLoading();
            await App.Current.MainPage.DisplayAlert("Info", "Partner Request Added Succefuly!", "Done");
        }

        private async Task changeProfileMV()
        {
            ActPop = true;
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            try
            {

                ListAttributes = new List<ProfileAttributes>();
                ListAttributes = await ProfileAttributes.GetAttributesProfile2(Convert.ToInt32(this.Selected_profile.Id));
                ActPop = false; 


            }
            catch (Exception ex)
            { 
                
            }
            UserDialogs.Instance.HideLoading();
        }
    }
}
