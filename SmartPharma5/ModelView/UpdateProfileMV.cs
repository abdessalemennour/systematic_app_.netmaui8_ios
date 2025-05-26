using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.Model;
using SmartPharma5.View;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;

namespace SmartPharma5.ViewModel
{
    class UpdateProfileMV : BaseViewModel
    {

        private Partner partner;
        public Partner Partner { get => partner; set => SetProperty(ref partner, value); }

        

        private int? instance;
        public int? Instance { get => instance; set => SetProperty(ref instance, value); }

        private List<MyProfileAttrubutes> listMyAttributes;
        public List<MyProfileAttrubutes> ListMyAttributes { get => listMyAttributes; set => SetProperty(ref listMyAttributes, value); }



        private bool changeProfileBool;
        public bool ChangeProfileBool { get => changeProfileBool; set => SetProperty(ref changeProfileBool, value); }

        private bool isVisibleName;
        public bool IsVisibleName { get => isVisibleName; set => SetProperty(ref isVisibleName, value); }

        private bool isVisibleCategory;
        public bool IsVisibleCategory { get => isVisibleCategory; set => SetProperty(ref isVisibleCategory, value); }


        private bool isVisibleStreet;
        public bool IsVisibleStreet { get => isVisibleStreet; set => SetProperty(ref isVisibleStreet, value); }

        private bool isVisibleState;
        public bool IsVisibleState { get => isVisibleState; set => SetProperty(ref isVisibleState, value); }


        private bool isVisisbleCodePostal;
        public bool IsVisisbleCodePostal { get => isVisisbleCodePostal; set => SetProperty(ref isVisisbleCodePostal, value); }

        private bool isVisibleCountry;
        public bool IsVisibleCountry { get => isVisibleCountry; set => SetProperty(ref isVisibleCountry, value); }

        //------------------------------------------------------------

        private bool isVisibleNumber;
        public bool IsVisibleNumber { get => isVisibleNumber; set => SetProperty(ref isVisibleNumber, value); }

        private bool isVisibleFax;
        public bool IsVisibleFax { get => isVisibleFax; set => SetProperty(ref isVisibleFax, value); }

        private bool isVisibleVatCode;
        public bool IsVisibleVatCode { get => isVisibleVatCode; set => SetProperty(ref isVisibleVatCode, value); }





        public AsyncCommand UpdateName { get; }
        public AsyncCommand UpdateCategory { get; }
        public AsyncCommand UpdateStreet { get; }
        public AsyncCommand UpdateState { get; }
        public AsyncCommand UpdateCodePostale { get; }
        public AsyncCommand UpdateCountry { get; }




        public AsyncCommand UpdateNumber { get; }
        public AsyncCommand UpdateVatCode { get; }
        public AsyncCommand UpdateFax { get; }


        public AsyncCommand ChangeProfile { get; }

        public AsyncCommand SaveName { get; }
        public AsyncCommand SaveCategory { get; }
        public AsyncCommand SaveStreet { get; }
        public AsyncCommand SaveState { get; }
        public AsyncCommand SaveCodePostale { get; }
        public AsyncCommand SaveCountry { get; }

        public AsyncCommand SaveFax { get; }
        public AsyncCommand SaveNumber { get; }
        public AsyncCommand SaveVatCode { get; }




        public AsyncCommand CloseName { get; }
        public AsyncCommand CloseCategory { get; }
        public AsyncCommand CloseStreet { get; }
        public AsyncCommand CloseState { get; }
        public AsyncCommand CloseCodePostale { get; }
        public AsyncCommand CloseCountry { get; }

        public AsyncCommand CloseFax { get; }
        public AsyncCommand CloseNumber { get; }
        public AsyncCommand CloseVatCode { get; }



        private string inputName;
        public string InputName { get => inputName; set => SetProperty(ref inputName, value); }

        private int inputCategory = -1;
        public int InputCategory { get => inputCategory; set => SetProperty(ref inputCategory, value); }

        private Comercial_category selectedCategory;
        public Comercial_category SelectedCategory { get => selectedCategory; set => SetProperty(ref selectedCategory, value); }





        private string inputStreet;
        public string InputStreet { get => inputStreet; set => SetProperty(ref inputStreet, value); }

        private string inputState;
        public string InputState { get => inputState; set => SetProperty(ref inputState, value); }


        private string inputCodePostal;
        public string InputCodePostal { get => inputCodePostal; set => SetProperty(ref inputCodePostal, value); }

        private string inputCountry;
        public string InputCountry { get => inputCountry; set => SetProperty(ref inputCountry, value); }

        private string inputNumber;
        public string InputNumber { get => inputNumber; set => SetProperty(ref inputNumber, value); }
        private string inputFax;
        public string InputFax { get => inputFax; set => SetProperty(ref inputFax, value); }
        private string inputVatCode;
        public string InputVatCode { get => inputVatCode; set => SetProperty(ref inputVatCode, value); }




        private bool btnUpdateName;
        public bool BtnUpdateName { get => btnUpdateName; set => SetProperty(ref btnUpdateName, value); }

        private bool btnUpdateCattegory;
        public bool BtnUpdateCategory { get => btnUpdateCattegory; set => SetProperty(ref btnUpdateCattegory, value); }


        private bool btnUpdateStreet;
        public bool BtnUpdateStreet { get => btnUpdateStreet; set => SetProperty(ref btnUpdateStreet, value); }

        private bool btnUpdateState;
        public bool BtnUpdateState { get => btnUpdateState; set => SetProperty(ref btnUpdateState, value); }


        private bool btnUpdateCodePostal;
        public bool BtnUpdateCodePostal { get => btnUpdateCodePostal; set => SetProperty(ref btnUpdateCodePostal, value); }

        private bool btnUpdateCountry;
        public bool BtnUpdateCountry { get => btnUpdateCountry; set => SetProperty(ref btnUpdateCountry, value); }


        private bool btnUpdateFax;
        public bool BtnUpdateFax { get => btnUpdateFax; set => SetProperty(ref btnUpdateFax, value); }




        private bool btnUpdateNumber;
        public bool BtnUpdateNumber { get => btnUpdateNumber; set => SetProperty(ref btnUpdateNumber, value); }



        private bool btnUpdateVatCode;
        public bool BtnUpdateVatCode { get => btnUpdateVatCode; set => SetProperty(ref btnUpdateVatCode, value); }



        private List<Comercial_category> catgory_list;
        public List<Comercial_category> Catgory_list { get => catgory_list; set => SetProperty(ref catgory_list, value); }





        private string profileName = "NOT EXIST YET";
        public string ProfileName { get => profileName; set => SetProperty(ref profileName, value); }


        private List<Profile> profiles;
        public List<Profile> Profiles { get => profiles; set => SetProperty(ref profiles, value); }


        private Profile selected_profile;
        public Profile Selected_profile { get => selected_profile; set => SetProperty(ref selected_profile, value); }


        public AsyncCommand changeSearsh { get; }



        private List<ProfileAttributes> listAttributes;
        public List<ProfileAttributes> ListAttributes { get => listAttributes; set => SetProperty(ref listAttributes, value); }


        private List<MyProfileAttrubutes> restAttributes;
        public List<MyProfileAttrubutes> RestAttributes { get => restAttributes; set => SetProperty(ref restAttributes, value); }




        private List<tempValueName> nameTemValues;
        public List<tempValueName> NameTemValues { get => nameTemValues; set => SetProperty(ref nameTemValues, value); }


        private List<tempValueName> categoryTemValues;
        public List<tempValueName> CategoryTemValues { get => categoryTemValues; set => SetProperty(ref categoryTemValues, value); }



        private List<tempValueName> codePostaleTemValues;
        public List<tempValueName> CodePostaleTemValues { get => codePostaleTemValues; set => SetProperty(ref codePostaleTemValues, value); }



        private List<tempValueName> streetTemValues;
        public List<tempValueName> StreetTemValues { get => streetTemValues; set => SetProperty(ref streetTemValues, value); }




        private List<tempValueName> stateTemValues;
        public List<tempValueName> StateTemValues { get => stateTemValues; set => SetProperty(ref stateTemValues, value); }













        public UpdateProfileMV(uint id)
        {
            BtnUpdateCategory = true;
            BtnUpdateName = true;
            BtnUpdateCodePostal = true;
            BtnUpdateState = true;
            BtnUpdateCountry = true;
            BtnUpdateStreet = true;
            BtnUpdateFax = true;
            BtnUpdateNumber = true;
            BtnUpdateVatCode = true;
            try
            {
                Profiles = Profile.getAllProfile().Result;
                Catgory_list = Comercial_category.GetCommercialCategory().Result;
                Partner = Partner.GetCommercialPartnerById(Convert.ToInt32(id)).Result;
                Instance = getInstance(Convert.ToInt32(Partner.Id)).Result;
                NameTemValues = Partner.GetCommercialPartnerTempName(Convert.ToInt32(Partner.Id)).Result;
                //CategoryTemValues=Partner.GetCommercialPartnerTempCategory(Convert.ToInt32(Partner.Id)).Result;
                //StateTemValues=Partner.GetCommercialPartnerTempState(Convert.ToInt32(Partner.Id)).Result;
                //ListMyAttributes = MyProfileAttrubutes.getMyAttributValuebyId(Convert.ToInt32(partner.Id));
                ListMyAttributes =  MyProfileAttrubutes.getMyAttributValuebyId(Convert.ToInt32(Partner.Id));
                RestAttributes = MyProfileAttrubutes.getMyAttributbyId(Convert.ToInt32(Partner.Id));
                RestAttributes = RestAttributes.Where(item2 => !ListMyAttributes.Any(item1 => item1.id_attribute == item2.id_attribute)).ToList();
               ListMyAttributes.AddRange(RestAttributes);
                if (ListMyAttributes.Count > 0)
                {
                    ProfileName = ListMyAttributes[0].profile_name;
                }
            }
            catch (Exception ex)
            {

            }
            UpdateCategory = new AsyncCommand(update_category);
            UpdateName = new AsyncCommand(update_name);
            UpdateState = new AsyncCommand(update_state);
            UpdateStreet = new AsyncCommand(update_street);
            UpdateCountry = new AsyncCommand(update_country);
            UpdateCodePostale = new AsyncCommand(update_code_postale);
            UpdateFax = new AsyncCommand(update_fax);
            UpdateNumber = new AsyncCommand(update_number);
            UpdateVatCode = new AsyncCommand(update_vat_code);
            ChangeProfile = new AsyncCommand(changeProfile1);
            SaveCategory = new AsyncCommand(save_category);
            SaveName = new AsyncCommand(save_name);
            SaveState = new AsyncCommand(save_state);
            SaveStreet = new AsyncCommand(save_street);
            SaveCountry = new AsyncCommand(save_country);
            SaveCodePostale = new AsyncCommand(save_code_postale);
            SaveVatCode = new AsyncCommand(save_vat_code);
            SaveNumber = new AsyncCommand(save_number);
            SaveFax = new AsyncCommand(save_fax);
            CloseCategory = new AsyncCommand(close_category);
            CloseName = new AsyncCommand(close_name);
            CloseState = new AsyncCommand(close_state);
            CloseStreet = new AsyncCommand(close_street);
            CloseCountry = new AsyncCommand(close_country);
            CloseCodePostale = new AsyncCommand(close_code_postale);
            CloseFax = new AsyncCommand(close_fax);
            CloseNumber = new AsyncCommand(close_number);
            CloseVatCode = new AsyncCommand(close_vat);
            changeSearsh = new AsyncCommand(change_profile);
        }
        private async Task<int?> getInstance(int partner_id)
        {
            string sqlCmd = "select max(id) as instance from marketing_profile_instances where partner="+partner_id+";";
            int Instance=0;
            DbConnection.Deconnecter();
            if (await DbConnection.Connecter3())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                MySqlDataReader reader = cmd.ExecuteReader();
               

                while (reader.Read())
                {
                    try
                    {
                        Instance =Convert.ToInt32(reader["instance"]); 
                    }
                    catch (Exception ex)
                    {
                        reader.Close();
                        return null;
                    }

                }
                reader.Close();
                DbConnection.Deconnecter();
            }
            else
            {

                return null;
            }

            return Instance ;


        }

        private async Task changeProfile1()
        {ChangeProfileBool = true;}



        private async Task change_profile()
        {
            try
            {

                /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                Avant :
                                int p = Profile.InsertNewInstanceProfileTemp(Selected_profile.Id, Convert.ToInt32(Partner.Id)).Result;

                                await App.Current.MainPage.Navigation.PushAsync(new ChangeProfile(Selected_profile.Id, Selected_profile.Name,p, Convert.ToInt32(Partner.Id)));
                Après :
                                int p = Profile.InsertNewInstanceProfileTemp(Selected_profile.Id, Convert.ToInt32(Partner.Id)).Result;

                                await App.Current.MainPage.Navigation.PushAsync(new ChangeProfile(Selected_profile.Id, Selected_profile.Name,p, Convert.ToInt32(Partner.Id)));
                */

                var a = Selected_profile;
                if(a == null)
                {
                    await   App.Current.MainPage.DisplayAlert("Warning", "Please select profile first!", "OK");
                }
                else
                {
                    UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                    await Task.Delay(500);
                    int p = Profile.InsertNewInstanceProfileTemp(Selected_profile.Id, Convert.ToInt32(Partner.Id)).Result;

                    await App.Current.MainPage.Navigation.PushAsync(new ChangeProfile(Selected_profile.Id, Selected_profile.Name, p, Convert.ToInt32(Partner.Id)));
                    ListAttributes = ProfileAttributes.GetAttributesProfile(Selected_profile.Id).Result;
                    UserDialogs.Instance.HideLoading();
                }

                

            }
            catch (Exception ex)
            {

            }
            ChangeProfileBool = false;



        }

        public async Task RestaureInput()
        {
           

        }

        private async Task update_category()

        /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
        Avant :
                {

                    BtnUpdateCategory= false;
        Après :
                {

                    BtnUpdateCategory= false;
        */
        {

            BtnUpdateCategory = false;
            BtnUpdateCodePostal = true;
            BtnUpdateCountry = true;
            BtnUpdateName = true;
            BtnUpdateState = true;
            BtnUpdateStreet = true;
            BtnUpdateVatCode = true;
            BtnUpdateNumber = true;
            BtnUpdateFax = true;



            IsVisibleCategory = true;
            IsVisibleState = false;
            IsVisibleStreet = false;
            IsVisisbleCodePostal = false;
            IsVisibleName = false;
            IsVisibleFax = false;
            IsVisibleNumber = false;
            IsVisibleVatCode = false;
            IsVisibleCountry = false;


        }
        private async Task update_name()
        {
            BtnUpdateStreet = true;
            BtnUpdateCategory = true;
            BtnUpdateCodePostal = true;
            BtnUpdateCountry = true;
            BtnUpdateName = false;
            BtnUpdateState = true;
            BtnUpdateVatCode = true;
            BtnUpdateNumber = true;
            BtnUpdateFax = true;

            IsVisibleCategory = false;
            IsVisibleState = false;
            IsVisibleStreet = false;
            IsVisisbleCodePostal = false;
            IsVisibleName = true;
            IsVisibleFax = false;
            IsVisibleNumber = false;
            IsVisibleVatCode = false;
            IsVisibleCountry = false;

        }
        private async Task update_state()
        {
            BtnUpdateStreet = true;
            BtnUpdateCategory = true;
            BtnUpdateCodePostal = true;
            BtnUpdateCountry = true;
            BtnUpdateName = true;
            BtnUpdateState = false;
            BtnUpdateVatCode = true;
            BtnUpdateNumber = true;
            BtnUpdateFax = true;

            IsVisibleCategory = false;
            IsVisibleState = true;
            IsVisibleStreet = false;
            IsVisisbleCodePostal = false;
            IsVisibleName = false;
            IsVisibleFax = false;
            IsVisibleNumber = false;
            IsVisibleVatCode = false;
            IsVisibleCountry = false;

        }
        private async Task update_street()
        {
            BtnUpdateStreet = false;
            BtnUpdateCategory = true;
            BtnUpdateCodePostal = true;
            BtnUpdateCountry = true;
            BtnUpdateName = true;
            BtnUpdateState = true;
            BtnUpdateVatCode = true;
            BtnUpdateNumber = true;
            BtnUpdateFax = true;

            IsVisibleCategory = false;
            IsVisibleState = false;
            IsVisibleStreet = true;
            IsVisisbleCodePostal = false;
            IsVisibleName = false;
            IsVisibleFax = false;
            IsVisibleNumber = false;
            IsVisibleVatCode = false;
            IsVisibleCountry = false;

        }
        private async Task update_country()
        {
            BtnUpdateStreet = true;
            BtnUpdateCategory = true;
            BtnUpdateCodePostal = true;
            BtnUpdateCountry = true;
            BtnUpdateName = true;
            BtnUpdateState = true;
            BtnUpdateCountry = false;
            BtnUpdateVatCode = true;
            BtnUpdateNumber = true;
            BtnUpdateFax = true;

            IsVisibleCategory = false;
            IsVisibleState = false;
            IsVisibleStreet = false;
            IsVisisbleCodePostal = false;
            IsVisibleName = false;
            IsVisibleFax = false;
            IsVisibleNumber = false;
            IsVisibleVatCode = false;
            IsVisibleCountry = true;

        }
        private async Task update_code_postale()
        {
            BtnUpdateStreet = true;
            BtnUpdateCategory = true;
            BtnUpdateCodePostal = false;
            BtnUpdateCountry = true;
            BtnUpdateName = true;
            BtnUpdateState = true;
            BtnUpdateCodePostal = false;
            BtnUpdateVatCode = true;
            BtnUpdateNumber = true;
            BtnUpdateFax = true;

            IsVisibleCategory = false;
            IsVisibleState = false;
            IsVisibleStreet = false;
            IsVisisbleCodePostal = true;
            IsVisibleName = false;
            IsVisibleFax = false;
            IsVisibleNumber = false;
            IsVisibleVatCode = false;
            IsVisibleCountry = false;

        }

        //---------------------------------------
        private async Task update_fax()
        {
            BtnUpdateCategory = true;
            BtnUpdateCodePostal = true;
            BtnUpdateCountry = true;
            BtnUpdateName = true;
            BtnUpdateState = true;
            BtnUpdateStreet = true;
            BtnUpdateVatCode = true;
            BtnUpdateNumber = true;
            BtnUpdateFax = false;



            IsVisibleCategory = false;
            IsVisibleState = false;
            IsVisibleStreet = false;
            IsVisisbleCodePostal = false;
            IsVisibleName = false;
            IsVisibleFax = true;
            IsVisibleNumber = false;
            IsVisibleVatCode = false;
            IsVisibleCountry = false;
        }
        private async Task update_vat_code()
        {
            BtnUpdateCategory = true;
            BtnUpdateCodePostal = true;
            BtnUpdateCountry = true;
            BtnUpdateName = true;
            BtnUpdateState = true;
            BtnUpdateStreet = true;
            BtnUpdateVatCode = false;
            BtnUpdateNumber = true;
            BtnUpdateFax = true;



            IsVisibleCategory = false;
            IsVisibleState = false;
            IsVisibleStreet = false;
            IsVisisbleCodePostal = false;
            IsVisibleName = false;
            IsVisibleFax = false;
            IsVisibleNumber = false;
            IsVisibleVatCode = true;
            IsVisibleCountry = false;
        }
        private async Task update_number()
        {
            BtnUpdateCategory = true;
            BtnUpdateCodePostal = true;
            BtnUpdateCountry = true;
            BtnUpdateName = true;
            BtnUpdateState = true;
            BtnUpdateStreet = true;
            BtnUpdateVatCode = true;
            BtnUpdateNumber = false;
            BtnUpdateFax = true;



            IsVisibleCategory = false;
            IsVisibleState = false;
            IsVisibleStreet = false;
            IsVisisbleCodePostal = false;
            IsVisibleName = false;
            IsVisibleFax = false;
            IsVisibleNumber = true;
            IsVisibleVatCode = false;
            IsVisibleCountry = false;
        }

        private async Task close_category()
        {
            IsVisibleCategory = false;
            BtnUpdateCategory = true;


        }
        private async Task close_name()
        {
            IsVisibleName = false;
            BtnUpdateName = true;

        }
        private async Task close_state()
        {
            IsVisibleState = false;
            BtnUpdateState = true;

        }
        private async Task close_street()
        {
            IsVisibleStreet = false;
            BtnUpdateStreet = true;

        }
        private async Task close_country()
        {
            IsVisibleCountry = false;
            BtnUpdateCountry = true;

        }
        private async Task close_code_postale()
        {
            IsVisisbleCodePostal = false;
            BtnUpdateCodePostal = true;

        }

        //----------------------------------

        private async Task close_number()
        {
            IsVisibleNumber = false;
            BtnUpdateNumber = true;
        }

        private async Task close_fax()
        {
            IsVisibleFax = false;
            BtnUpdateFax = true;
        }

        private async Task close_vat()
        {
            IsVisibleVatCode = false;
            BtnUpdateVatCode = true;
        }





        //-------------------------------------------------

        private async Task save_category()
        {

            try
            {
                if (InputCategory == -1)
                {

                    await App.Current.MainPage.DisplayAlert("Warnning", "Pick Category Please", "Ok");
                    return;
                }
                if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO SAVE THIS CHANGES!", "Yes", "No"))
                {
                    await Partner.updateCategoryPartner(Convert.ToInt32(Partner.Id), SelectedCategory.id);
                }
                await App.Current.MainPage.DisplayAlert("Done", "Request Saved Succefuly", "Ok");



            }
            catch (Exception ex)
            {

            }
            IsVisibleCategory = false;
            BtnUpdateCategory = true;


        }
        private async Task save_name()
        {

            try
            {

                if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO SAVE THIS CHANGES!", "Yes", "No"))
                {
                    await Partner.updateNamePartner(Convert.ToInt32(Partner.Id), InputName);

                }
                InputName = "";
                await App.Current.MainPage.DisplayAlert("Done", "Request Saved Succefuly", "Ok");


            }
            catch (Exception ex)
            {

            }
            IsVisibleName = false;
            BtnUpdateName = true;


        }
        private async Task save_state()
        {

            try
            {
                if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO SAVE THIS CHANGES!", "Yes", "No"))
                {
                    await Partner.updateStatePartner(Convert.ToInt32(Partner.Id), InputState);
                }
                InputState = "";
                await App.Current.MainPage.DisplayAlert("Done", "Request Saved Succefuly", "Ok");

            }
            catch (Exception ex)
            {

            }
            IsVisibleState = false;
            BtnUpdateState = true;

        }
        private async Task save_street()
        {

            try
            {
                if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO SAVE THIS CHANGES!", "Yes", "No"))
                {
                    await Partner.updateStreetPartner(Convert.ToInt32(Partner.Id), InputStreet);
                }
                InputStreet = "";
                await App.Current.MainPage.DisplayAlert("Done", "Request Saved Succefuly", "Ok");


            }
            catch (Exception ex)
            {

            }
            IsVisibleStreet = false;
            BtnUpdateStreet = true;

        }
        private async Task save_country()
        {

            try
            {
                if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO SAVE THIS CHANGES!", "Yes", "No"))
                {
                    await Partner.updateCountryPartner(Convert.ToInt32(Partner.Id), InputCountry);
                }
                InputCountry = "";
                await App.Current.MainPage.DisplayAlert("Done", "Request Saved Succefuly", "Ok");


            }
            catch (Exception ex)
            {

            }
            IsVisibleCountry = false;
            BtnUpdateCountry = true;

        }
        private async Task save_code_postale()
        {

            try
            {
                if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO SAVE THIS CHANGES!", "Yes", "No"))
                {
                    await Partner.updateCodePostalePartner(Convert.ToInt32(Partner.Id), InputCodePostal);
                }

                InputCodePostal = "";
                await App.Current.MainPage.DisplayAlert("Done", "Request Saved Succefuly", "Ok");


            }
            catch (Exception ex)
            {

            }


            IsVisisbleCodePostal = false;
            BtnUpdateCodePostal = true;

        }

        private async Task save_fax()
        {

            try
            {

                if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO SAVE THIS CHANGES!", "Yes", "No"))
                {
                    await Partner.updateFaxPartner(Convert.ToInt32(Partner.Id), InputFax);
                }
                InputFax = "";
                await App.Current.MainPage.DisplayAlert("Done", "Request Saved Succefuly", "Ok");



            }
            catch (Exception ex)
            {

            }
            IsVisibleFax = false;
            BtnUpdateFax = true;


        }
        private async Task save_number()
        {

            try
            {

                if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO SAVE THIS CHANGES!", "Yes", "No"))
                {
                    await Partner.updateNumberePartner(Convert.ToInt32(Partner.Id), InputNumber);
                }
                InputNumber = "";

                await App.Current.MainPage.DisplayAlert("Done", "Request Saved Succefuly", "Ok");



            }
            catch (Exception ex)
            {

            }
            IsVisibleNumber = false;
            BtnUpdateNumber = true;


        }


        public static MySqlCommand cmd;


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

        private async Task save_vat_code()
        {

            try
            {
                if(await ExistVatCode(InputVatCode.ToString()))
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Vat code is already exist", "Ok");
                    return;
                }


                if (await App.Current.MainPage.DisplayAlert("INFO", "DO YOU WANT TO SAVE THIS CHANGES!", "Yes", "No"))
                {
                    await Partner.updateVatCodePartner(Convert.ToInt32(Partner.Id), InputVatCode);
                }
                await App.Current.MainPage.DisplayAlert("Done", "Request Saved Succefuly", "Ok");


            }
            catch (Exception ex)
            {

            }
            IsVisibleVatCode = false;
            BtnUpdateVatCode = true;


        }

    }
}
