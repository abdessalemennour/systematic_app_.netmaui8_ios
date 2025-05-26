//using Acr.UserDialogs;
using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.Model;
using SmartPharma5.View;

namespace SmartPharma5.ViewModel
{
    public class PartnerTempAttributesMV : BaseViewModel
    {

        public int Id_employe { get; set; }

        public int partner_temp { get; set; }
        public int id_profile { get; set; }

        private List<PartnerTempAttributesModel> listPartnerTempAttributes;
        public List<PartnerTempAttributesModel> ListPartnerTempAttributes { get => listPartnerTempAttributes; set => SetProperty(ref listPartnerTempAttributes, value); }

        public AsyncCommand ValidateTemp { get; set; }
        public AsyncCommand RefuseTemp { get; set; }

        public PartnerTempAttributesMV(int partner_temp, int id_profile,int id_employe)
        {
            Id_employe = id_employe;
            this.partner_temp = partner_temp;
            this.id_profile = id_profile;
            ValidateTemp = new AsyncCommand(validate);
            RefuseTemp = new AsyncCommand(refuse);
            try
            {
                ListPartnerTempAttributes = new List<PartnerTempAttributesModel>(PartnerTempAttributesModel.GetAllAttributesProfile(partner_temp).Result);

            }
            catch (Exception ex)
            {

            }

        }

        private async Task validate()
        {
            bool ProfileExiste = false;
            int? id_instance = 0;
            int? id_new_partner = 0;
            string sqlCmdGlobal = "";
            string Partner_name = "";
            string Partner_Fax = "";
            string Partner_Number = "";
            string Partner_Street = "";
            string Partner_City = "";
            string Partner_State = ""; 
            string Partner_Country = "";
            string Partner_Postal = "";
            string Partner_Email = "";
            string Partner_Vat_Code = "";
            string Partner_Gps = "";
            bool? Partner_Customer = false;
            bool? Partner_Supplier = false;
            int? Partner_Category = 0;
            // Variable pour stocker l'ID de la devise
            int? currencyId = 0;

            if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO SAVE CHANGES", "YES", "NO"))
            {

                // Récupérer l'ID de currency principal
                if (await DbConnection.Connecter3())
                {
                    string sqlCurrency = "SELECT id FROM atooerp_currency WHERE principal = 1;";
                    MySqlCommand cmdCurrency = new MySqlCommand(sqlCurrency, DbConnection.con);
                    try
                    {
                        currencyId = int.Parse(cmdCurrency.ExecuteScalar().ToString());
                    }
                    catch (Exception ex)
                    {
                        await App.Current.MainPage.DisplayAlert("ERROR", "Failed to retrieve currency ID: " + ex.Message, "OK");
                        return;
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("ERROR", "Failed to connect to the database.", "OK");
                    return;
                }

                foreach (PartnerTempAttributesModel item in ListPartnerTempAttributes)
                {

                    if (item.Valid_attribute)
                    {
                        if (item.IsProflieAttribute == false)
                        {
                            if (item.Label.ToLower() == "name")
                            {
                                Partner_name = item.String_value;
                            }
                            else if (item.Label.ToLower() == "number")
                            {
                                Partner_Number = item.String_value;
                            }
                            else if (item.Label.ToLower() == "street")
                            {
                                Partner_Street = item.String_value;
                            }
                            else if (item.Label.ToLower() == "city")
                            {
                                Partner_City = item.String_value;
                            }
                            else if (item.Label.ToLower() == "country")
                            {
                                Partner_Country = item.String_value;
                            }
                            else if (item.Label.ToLower() == "postal_code")
                            {
                                Partner_Postal = item.String_value;
                            }
                            else if (item.Label.ToLower() == "email")
                            {
                                Partner_Email = item.String_value;
                            }
                            else if (item.Label.ToLower() == "state")
                            {
                                Partner_State = item.String_value;
                            }
                            else if (item.Label.ToLower() == "fax")
                            {
                                Partner_Fax = item.String_value;
                            }
                            else if (item.Label.ToLower() == "category")
                            {
                                Partner_Category = item.Type_value;
                            }
                            else if (item.Label.ToLower() == "customer")
                            {
                                Partner_Customer = item.Boolean_value;
                            }
                            else if (item.Label.ToLower() == "supplier")
                            {
                                Partner_Supplier = item.Boolean_value;
                            }
                            else if (item.Label.ToLower() == "vat_code")
                            {
                                Partner_Vat_Code = item.String_value;
                            }
                            else if (item.Label.ToLower() == "gps")
                            {
                                Partner_Gps = item.String_value;
                            }
                        }
                    }



                    //--------------------------------------------------------------------------------------------------------------------------------



                }

                id_new_partner = await Partner.InsertNewPartner(Partner_name, Partner_Street, Partner_City, Partner_State, Partner_Postal, Partner_Country, Partner_Email, Partner_Fax, Partner_Customer, Partner_Supplier, Partner_Category, Partner_Vat_Code, Partner_Gps, Id_employe);

                // Mettre à jour l'attribut currency dans la table commercial_partner
                if (await DbConnection.Connecter3())
                {
                    string sqlUpdateCurrency = "UPDATE commercial_partner SET currency = " + currencyId + " WHERE id = " + id_new_partner + ";";
                    MySqlCommand cmdUpdateCurrency = new MySqlCommand(sqlUpdateCurrency, DbConnection.con);
                    try
                    {
                        cmdUpdateCurrency.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        await App.Current.MainPage.DisplayAlert("ERROR", "Failed to update currency: " + ex.Message, "OK");
                        return;
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("ERROR", "Failed to connect to the database.", "OK");
                    return;
                }

                foreach (PartnerTempAttributesModel item in ListPartnerTempAttributes)
                {
                    if (item.Valid_attribute)
                    {
                        if (item.IsProflieAttribute)
                        {
                            if (ProfileExiste == false)
                            {
                                if (await DbConnection.Connecter3())
                                {
                                    string sqlCmd = "insert into marketing_profile_instances(create_date,partner,profil) values('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + id_new_partner + "," + id_profile + ");" +
                                        "select max(id) from marketing_profile_instances;";
                                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                                    try
                                    {
                                        id_instance = int.Parse(cmd.ExecuteScalar().ToString());
                                        ProfileExiste = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        await App.Current.MainPage.DisplayAlert("ERROR", "Failed to insert profile instance: " + ex.Message, "OK");
                                        return;
                                    }
                                }
                                else
                                {
                                    await App.Current.MainPage.DisplayAlert("ERROR", "Failed to connect to the database.", "OK");
                                    return;
                                }
                            }

                            if (item.HasString)
                            {
                                sqlCmdGlobal = sqlCmdGlobal + "insert into marketing_profile_attribut_value(create_date,profile_instance,attribut,string_value) values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + id_instance + "," + item.Id_Attribute + ",'" + item.String_value + "');";
                            }
                            else if (item.HasInt)
                            {
                                sqlCmdGlobal = sqlCmdGlobal + "insert into marketing_profile_attribut_value(create_date,profile_instance,attribut,int_value) values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + id_instance + "," + item.Id_Attribute + "," + item.Int_value + ");";
                            }
                            else if (item.HasDecimal)
                            {
                                sqlCmdGlobal = sqlCmdGlobal + "insert into marketing_profile_attribut_value(create_date,profile_instance,attribut,decimal_value) values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + id_instance + "," + item.Id_Attribute + "," + item.Decimal_value.ToString().Replace(',', '.') + ");";
                            }
                            else if (item.HasDate)
                            {
                                sqlCmdGlobal = sqlCmdGlobal + "insert into marketing_profile_attribut_value(create_date,profile_instance,attribut,date_value) values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + id_instance + "," + item.Id_Attribute + ",'" + item.Date_value?.ToString("yyyy-MM-dd HH:mm:ss") + "');";
                            }
                            else if (item.HasBool)
                            {
                                sqlCmdGlobal = sqlCmdGlobal + "insert into marketing_profile_attribut_value(create_date,profile_instance,attribut,boolean_value) values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + id_instance + "," + item.Id_Attribute + "," + item.Boolean_value + ");";
                            }
                            else if (item.HasType)
                            {
                                sqlCmdGlobal = sqlCmdGlobal + "insert into marketing_profile_attribut_value(create_date,profile_instance,attribut,type) values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + id_instance + "," + item.Id_Attribute + "," + item.Type_value + ");";
                            }
                        }
                    }
                }

                if (id_instance != 0)
                {
                    if (await DbConnection.Connecter3())
                    {
                        sqlCmdGlobal = sqlCmdGlobal + "update marketing_profile_instance_temp set instance = " + id_instance + ",state=2 where partner_temp=" + partner_temp + " ;";
                        MySqlCommand cmd = new MySqlCommand(sqlCmdGlobal, DbConnection.con);
                        try
                        {
                            cmd.ExecuteScalar();

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {

                    }

                }

                if (await DbConnection.Connecter3())
                {
                    string sql = "update commercial_partner_temp set state=2 where id =" + partner_temp + ";" +
                        "update commercial_partner_temp set partner=" + id_new_partner + "  where id =" + partner_temp + ";";
                    MySqlCommand cmd = new MySqlCommand(sql, DbConnection.con);
                    try
                    {
                        cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {

                }
                await App.Current.MainPage.DisplayAlert("DONE", "CHANGES SAVED SUCCEFULY", "OK");

                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                var tabbedPage = new TabPageValidationUpdates();

                // Accédez à l'onglet "PartnerTempView" (index 2)
                tabbedPage.CurrentPage = tabbedPage.Children[2];

                // Utilisez la navigation pour afficher la TabbedPage
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(tabbedPage));
                UserDialogs.Instance.HideLoading();

            }






        }

        private async Task refuse()
        {
            if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO REFUSE CHANGES", "YES", "NO"))
            {
                if (await DbConnection.Connecter3())
                {
                    string sql = "update commercial_partner_temp set state=3 where id =" + partner_temp + ";" +
                        "update marketing_profile_instance_temp set state=3 where partner_temp=" + partner_temp + " ;";
                    MySqlCommand cmd = new MySqlCommand(sql, DbConnection.con);
                    try
                    {
                        cmd.ExecuteScalar();

                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {

                }

                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                var tabbedPage = new TabPageValidationUpdates();

                // Accédez à l'onglet "PartnerTempView" (index 2)
                tabbedPage.CurrentPage = tabbedPage.Children[2];

                // Utilisez la navigation pour afficher la TabbedPage
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(tabbedPage));
                UserDialogs.Instance.HideLoading();
            }


        }
    }
}
