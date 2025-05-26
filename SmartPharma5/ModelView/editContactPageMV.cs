using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.ModelView
{
    class editContactPageMV : BaseViewModel
    {
        private Contact_Partner contact;
        public Contact_Partner Contact { get => contact; set => SetProperty(ref contact, value); }

       // private Contact_Partner currentContact;
        public Contact_Partner CurrentContact { get; set; } = new Contact_Partner();


        private List<Item> martinal;
        public List<Item> Martinal { get => martinal; set => SetProperty(ref martinal, value); }
        private bool isMaleSelected;
        public bool IsMaleSelected
        {
            get => isMaleSelected;
            set
            {
                if (SetProperty(ref isMaleSelected, value) && value)
                { // 1 pour Homme
                    CurrentContact.Sex = 1;
                    IsFemaleSelected = !value;
                }
            }
        }

        private bool isFemaleSelected;
        public bool IsFemaleSelected
        {
            get => isFemaleSelected;
            set
            {
                if (SetProperty(ref isFemaleSelected, value) && value)
                {// 2 pour Femme
                    CurrentContact.Sex = 2; 
                    IsMaleSelected = !value;
                }
            }
        }
        private Item selectedMaritalStatus;
        public Item SelectedMaritalStatus
        {
            get => selectedMaritalStatus;
            set
            {
                if (SetProperty(ref selectedMaritalStatus, value))
                {
                    CurrentContact.Martal_status = value?.Id ?? 0;
                }
            }
        }
        private bool isHandicap;
        public bool IsHandicap
        {
            get => isHandicap;
            set
            {
                if (SetProperty(ref isHandicap, value))
                {
                    CurrentContact.Handicap = value;
                }
            }
        }
        private List<Item> countries;
        public List<Item> Countries
        {
            get => countries;
            set => SetProperty(ref countries, value);
        }
        private Item selectedCountry;
        public Item SelectedCountry
        {
            get => selectedCountry;
            set
            {
                if (SetProperty(ref selectedCountry, value))
                {
                    CurrentContact.Nationality = value?.Id ?? 0;
                }
            }
        }

        public AsyncCommand updateFirstNameCommand { get; set; }
        public AsyncCommand updateLastNameCommand { get; set; }
        public AsyncCommand updateSexCommand { get; set; }
        public AsyncCommand updateMaritalStatusCommand { get; set; }
        public AsyncCommand updateBirthDateCommand { get; set; }
        public AsyncCommand updateBirthPlaceCommand { get; set; }
        public AsyncCommand updateNationalityCommand { get; set; }
        public AsyncCommand updateAdressCommand { get; set; }
        public AsyncCommand updateIdentityCommand { get; set; }
        public AsyncCommand updateHandicapCommand { get; set; }


        public editContactPageMV(int id)
        {
            this.Contact = Contact_Partner.GetContact_PartnerById(id).Result;
            this.CurrentContact = this.Contact.Clone();
            this.Martinal = Contact_Partner.GetMaritalStatus().Result;
            IsMaleSelected = Contact.Sex == 1;
            IsFemaleSelected = Contact.Sex == 2;
            this.SelectedMaritalStatus = Martinal.FirstOrDefault(m => m.Id == this.CurrentContact.Martal_status);
            this.IsHandicap = this.CurrentContact.Handicap;
            this.CurrentContact.AddressName = this.Contact.AddressName;
            updateFirstNameCommand = new AsyncCommand(UpdateFirstName);
            updateLastNameCommand = new AsyncCommand(UpdateLastName);
            updateSexCommand = new AsyncCommand(UpdateSEX);
            updateMaritalStatusCommand = new AsyncCommand(UpdateMaritalStatus);
            updateBirthDateCommand = new AsyncCommand(UpdateBirthDate);
            updateBirthPlaceCommand = new AsyncCommand(UpdateBirthPlace);
            updateNationalityCommand = new AsyncCommand(UpdateNationality);
            updateAdressCommand = new AsyncCommand(UpdateAdress);
            updateIdentityCommand = new AsyncCommand(UpdateIdentity);
            updateHandicapCommand = new AsyncCommand(UpdateHandicap);
            LoadNationality();
            LoadCountries();
        }

        private async void LoadCountries()
        {
            Countries = await Contact_Partner.GetAllCountries();
            OnPropertyChanged(nameof(Countries));

            // Assurer que le pays par défaut est sélectionné dans la liste
            if (Countries != null && Countries.Any())
            {
                SelectedCountry = Countries.FirstOrDefault(c => c.Id == CurrentContact.Nationality);
            }
        }

        public void OnNationalitySelected(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var selectedCountry = picker.SelectedItem as Item;
            if (selectedCountry != null)
            {
                CurrentContact.Nationality = selectedCountry.Id;
            }
        }

        private async void LoadNationality()
        {
            this.CurrentContact.NationalityName = await Contact_Partner.GetCountryNameById(this.CurrentContact.Nationality);
            OnPropertyChanged(nameof(CurrentContact)); // Notifier l'UI du changement
        }
        private async Task UpdateContactField(string fieldName, object newValue)
            {
                if (await DbConnection.Connecter3())
                {
                    string sqlCmd = $"UPDATE atooerp_person SET {fieldName} = @newValue WHERE id = @id;";
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    cmd.Parameters.AddWithValue("@newValue", newValue);
                    cmd.Parameters.AddWithValue("@id", Contact.id);

                    try
                    {
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                          //  await App.Current.MainPage.DisplayAlert("Success", $"{fieldName} updated successfully", "OK");
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Error", "No rows were updated", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                    }
                    finally
                    {
                        DbConnection.Deconnecter();
                    }
                }
            }
        public async Task UpdateNationality()
        {
            if (this.CurrentContact.Nationality != this.Contact.Nationality)
            {
                await UpdateContactField("nationality", this.CurrentContact.Nationality);
                this.Contact.Nationality = this.CurrentContact.Nationality;
                this.CurrentContact.NationalityName = await Contact_Partner.GetCountryNameById(this.CurrentContact.Nationality);
            }
        }



        public async Task UpdateFirstName()
            {
                if (this.CurrentContact.FirstName != this.Contact.FirstName)
                {
                    await UpdateContactField("first_name", this.CurrentContact.FirstName);
                    this.Contact.FirstName = this.CurrentContact.FirstName; 
                }
            }
        public async Task UpdateIdentity()
        {
            if (this.CurrentContact.Identity != this.Contact.Identity)
            {
                await UpdateContactField("identity", this.CurrentContact.Identity);
                this.Contact.Identity = this.CurrentContact.Identity;
            }
        }


        public async Task UpdateLastName()
        {
            if (this.CurrentContact.LastName != this.Contact.LastName)
            {
                await UpdateContactField("last_name", this.CurrentContact.LastName);
                this.Contact.LastName = this.CurrentContact.LastName;
            }
           
        }
        private async Task UpdateSEX()
        {
            if (this.CurrentContact.Sex != this.Contact.Sex)
            {
                await UpdateContactField("sex", this.CurrentContact.Sex);
                this.Contact.Sex = this.CurrentContact.Sex;
            }
        }
        private async Task UpdateMaritalStatus()
        {
            if (this.CurrentContact.Martal_status != this.Contact.Martal_status)
            {
                await UpdateContactField("marital_status", this.CurrentContact.Martal_status);
                this.Contact.Martal_status = this.CurrentContact.Martal_status;
            }
        }
        public async Task UpdateBirthDate()
        {
            if (this.CurrentContact.Birth_date != this.Contact.Birth_date)
            {
                await UpdateContactField("birth_date", this.CurrentContact.Birth_date);
                this.Contact.Birth_date = this.CurrentContact.Birth_date;
            }
        }
        public async Task UpdateBirthPlace()
        {
            if (this.CurrentContact.Birth_place != this.Contact.Birth_place)
            {
                await UpdateContactField("birth_place", this.CurrentContact.Birth_place);
                this.Contact.Birth_place = this.CurrentContact.Birth_place;
            }

        }

        public async Task UpdateAdress()
        {
            if (this.CurrentContact.AddressName != this.Contact.AddressName)
            {
                // Mettre à jour l'adresse dans atooerp_address
                string updateAddressSql = @"
            UPDATE atooerp_address 
            SET name = @addressName 
            WHERE id = @addressId;";

                MySqlCommand updateAddressCmd = new MySqlCommand(updateAddressSql, DbConnection.con);
                updateAddressCmd.Parameters.AddWithValue("@addressName", this.CurrentContact.AddressName);
                updateAddressCmd.Parameters.AddWithValue("@addressId", this.CurrentContact.Adress);

                try
                {
                    await DbConnection.Connecter3();
                    int result = updateAddressCmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        // Mettre à jour l'ID de l'adresse dans atooerp_person si nécessaire
                        await UpdateContactField("address", this.CurrentContact.Adress);
                        this.Contact.AddressName = this.CurrentContact.AddressName;
                    }
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                }
                finally
                {
                    DbConnection.Deconnecter();
                }
            }
        }

        private async Task UpdateHandicap()
        {
            if (this.CurrentContact.Handicap != this.Contact.Handicap)
            {
                await UpdateContactField("handicap", this.CurrentContact.Handicap ? 1 : 0);
                this.Contact.Handicap = this.CurrentContact.Handicap;
            }
        }
        public async Task UpdateHandicapDescription()
        {
            if (this.CurrentContact.Handicap_description == this.Contact.Handicap_description)
            {

            }
            else
            {


            }

        }

    }
}
