using Acr.UserDialogs;
//using Android.Provider;
using MySqlConnector;
using SmartPharma5.Model;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace SmartPharma5.View;

public partial class AddNewContact : ContentPage
{
    private List<Country> countries;

    public int partner_id { get; set; }
    public AddNewContact(int id)
	{
		InitializeComponent();
        this.partner_id = id;
        maritalStatusPicker.ItemsSource = GetListMaritalStatus().Result;
        jobPositionPicker.ItemsSource = GetListJobPosition().Result;
        //LoadNationalitiesAsync().GetAwaiter();

    }
    public AddNewContact()
    {
        InitializeComponent();
        maritalStatusPicker.ItemsSource = GetListMaritalStatus().Result;
        jobPositionPicker.ItemsSource = GetListJobPosition().Result;
       // LoadNationalitiesAsync().GetAwaiter();
    }
    private void OnHandicapToggled(object sender, ToggledEventArgs e)
    {
        handicapDescriptionStack.IsVisible = e.Value;
    }

   /* private void OnNationalityChanged(object sender, EventArgs e)
    {
        if (nationalityComboBox.SelectedIndex == -1)
        {
            //citiesPicker.ItemsSource = null;
            return;
        }

        var selectedCountry = nationalityComboBox.SelectedItem.ToString();
        var country = countries.Find(c => c.Name == selectedCountry);


        if (country != null)
        {
            //citiesPicker.ItemsSource = country.Cities;
        }
        else
        {
            //citiesPicker.ItemsSource = null;
        }
    }
   */
   /* public async Task LoadNationalitiesAsync()
    {
        try
        {
            if (await DbConnection.Connecter3())
            {
                string sqlCmd = "SELECT Id,name_en_gb FROM atooerp_country"; 
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                MySqlDataReader reader = cmd.ExecuteReader();

                List<Country> countryList = new List<Country>();

                while (reader.Read())
                {
                    countryList.Add(new Country
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name_en_gb")
                    });
                }
                reader.Close();

                countries = countryList;
                Device.BeginInvokeOnMainThread(() =>
                {
                    nationalityComboBox.ItemsSource = countries;
                });
            }
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Database Error", ex.Message, "OK");
        }
    }*/

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        if (jobPositionPicker.SelectedItem == null)
        {
            await DisplayAlert("Error", "Please select a job position before adding a contact.", "OK");
            return;
        }

        Item item1 = jobPositionPicker.SelectedItem as Item;
        int job_position = item1?.id ?? 0;

        Item item2 = maritalStatusPicker.SelectedItem as Item;
        int? maritalStatus = item2?.id;

        string firstName = firstNameEntry.Text;
        string lastName = lastNameEntry.Text;
        int sex = hommeRadioButton.IsChecked ? 1 : 2;

        DateTime? birthDateNullable = birthDatePicker.Date;
        DateTime birthDate = birthDateNullable ?? DateTime.MinValue; string birthPlace = birthPlaceEntry.Text;

        // Extraire uniquement le nom du pays de l'objet sélectionné
       // string nationality = (nationalityComboBox.SelectedItem as Country)?.Name;

        // Rendre "nationality" facultatif
       /* if (string.IsNullOrEmpty(nationality))
        {
            nationality = null;  // Laisser null si non sélectionné
        }*/

       // string address = addressEntry.Text;
        bool handicap = handicapSwitch.IsToggled;
        string handicapDescription = handicap ? handicapDescriptionEditor.Text : null;

        // Rendre "address" facultatif
      /*  if (string.IsNullOrEmpty(address))
        {
            address = null;  // Laisser null si non rempli
        }*/

        // Rendre "identity" facultatif
       /* int? identity = null;
        if (!string.IsNullOrEmpty(identityEntry.Text) && int.TryParse(identityEntry.Text, out int parsedIdentity))
        {
            identity = parsedIdentity;  // Garder la valeur si elle est valide
        }*/

        UserDialogs.Instance.Loading("Adding New Contact, please wait...");

        // Vérifier si identity est null, et si oui, passer 0 (ou toute autre valeur par défaut)
        //await InsertNewContact(lastName, firstName, sex, maritalStatus, birthDate, birthPlace, handicap, handicapDescription, job_position, nationality, address, identity ?? 0);
        await InsertNewContact(lastName, firstName, sex, maritalStatus, birthDate, birthPlace, handicap, handicapDescription, job_position);

        await App.Current.MainPage.DisplayAlert("INFO", "CONTACT SAVED SUCCESSFULLY", "OK");
        await App.Current.MainPage.Navigation.PopAsync();

        UserDialogs.Instance.HideLoading();

    }


    public async Task InsertNewContact(string lastName, string firstName, int sex, int? maritalStatus, DateTime birthDate, string birthPlace, bool handicap, string? handicapDescription, int job_position)
    {
        UserDialogs.Instance.ShowLoading("Loading Please wait ...");
        await Task.Delay(500);

        if (await DbConnection.Connecter3())
        {
            // Étape 1 : Insérer l'adresse dans la table atooerp_address
          /*  string insertAddressSql = @"
            INSERT INTO atooerp_address (name) 
            VALUES (@address);
            SELECT LAST_INSERT_ID();";

            MySqlCommand insertAddressCmd = new MySqlCommand(insertAddressSql, DbConnection.con);
            insertAddressCmd.Parameters.AddWithValue("@address", address);

            int addressId = 0;

            try
            {
                // Exécuter la commande pour insérer l'adresse et récupérer l'ID
                addressId = Convert.ToInt32(insertAddressCmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Database Error", ex.Message, "OK");
                Console.WriteLine($"SQL Error: {ex.Message}");
                UserDialogs.Instance.HideLoading();
                return; // Arrêter l'exécution en cas d'erreur
            }*/
            // Étape 2 : Insérer la personne dans la table atooerp_person avec l'ID de l'adresse
            string insertPersonSql = @"
            INSERT INTO atooerp_person (create_date, first_name, last_name, sex, marital_status, birth_date, birth_place, address, handicap, handicap_description, nationality, identity) 
            VALUES (@create_date, @first_name, @last_name, @sex, @marital_status, @birth_date, @birth_place, @address, @handicap, @handicap_description, @nationality, @identity);
            SELECT LAST_INSERT_ID();";

            MySqlCommand insertPersonCmd = new MySqlCommand(insertPersonSql, DbConnection.con);
            insertPersonCmd.Parameters.AddWithValue("@create_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            insertPersonCmd.Parameters.AddWithValue("@first_name", firstName);
            insertPersonCmd.Parameters.AddWithValue("@last_name", lastName);
            insertPersonCmd.Parameters.AddWithValue("@sex", sex);
            insertPersonCmd.Parameters.AddWithValue("@marital_status", maritalStatus == 0 ? (object)DBNull.Value : maritalStatus);
            insertPersonCmd.Parameters.AddWithValue("@birth_date", birthDate.ToString("yyyy-MM-dd HH:mm:ss"));
            insertPersonCmd.Parameters.AddWithValue("@birth_place", birthPlace);
           // insertPersonCmd.Parameters.AddWithValue("@address", addressId); 
            insertPersonCmd.Parameters.AddWithValue("@handicap", handicap);
            insertPersonCmd.Parameters.AddWithValue("@handicap_description", string.IsNullOrEmpty(handicapDescription) ? (object)DBNull.Value : handicapDescription);
           // insertPersonCmd.Parameters.AddWithValue("@nationality", (nationalityComboBox.SelectedItem as Country)?.Id ?? (object)DBNull.Value);
            //insertPersonCmd.Parameters.AddWithValue("@identity", identity);

            int personId = 0;

            try
            {
                // Exécuter la commande pour insérer la personne et récupérer l'ID
                personId = Convert.ToInt32(insertPersonCmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Database Error", ex.Message, "OK");
                Console.WriteLine($"SQL Error: {ex.Message}");
                UserDialogs.Instance.HideLoading();
                return; // Arrêter l'exécution en cas d'erreur
            }

            // Étape 3 : Insérer la relation dans la table commercial_partner_contact
            if (personId != 0)
            {
                string insertContactSql = @"
                INSERT INTO commercial_partner_contact (partner, person, job_position) 
                VALUES (@partner, @person, @job_position);";

                MySqlCommand insertContactCmd = new MySqlCommand(insertContactSql, DbConnection.con);
                insertContactCmd.Parameters.AddWithValue("@partner", this.partner_id);
                insertContactCmd.Parameters.AddWithValue("@person", personId);
                insertContactCmd.Parameters.AddWithValue("@job_position", job_position);

                try
                {
                    insertContactCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Database Error", ex.Message, "OK");
                    Console.WriteLine($"SQL Error: {ex.Message}");
                }
            }
        }

        UserDialogs.Instance.HideLoading();
    }





    public async static Task<List<Item>> GetListMaritalStatus()
    {

        string sqlCmd = "SELECT * from atooerp_person_marital_status ;";


        List<Item> partner = new List<Item>();
        DbConnection.Deconnecter();
        DbConnection.Connecter();

        MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            try
            {
                partner.Add(new Item(Convert.ToInt32(reader["id"]), Convert.ToString(reader["name"])));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        DbConnection.Deconnecter();

        return partner;

    }
    public async static Task<List<Item>> GetListJobPosition()
    {

        string sqlCmd = "SELECT * from commercial_job_position ;";


        List<Item> partner = new List<Item>();
        DbConnection.Deconnecter();
        DbConnection.Connecter();

        MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            try
            {
                partner.Add(new Item(Convert.ToInt32(reader["id"]), Convert.ToString(reader["name"])));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        DbConnection.Deconnecter();

        return partner;

    }

}

public class Item
{
   public int id { get; set; }
    public string name { get; set; }

    public Item(int id,string name)
    {
        this.id = id;
        this.name = name;      
    }

}

public class CountriesResponse
{
    [JsonPropertyName("error")]
    public bool Error { get; set; }

    [JsonPropertyName("msg")]
    public string Msg { get; set; }

    [JsonPropertyName("data")]
    public List<Country> Data { get; set; }
}

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; }
}

/*public class Country
{
    [JsonPropertyName("country")]
    public string country { get; set; }

    [JsonPropertyName("cities")]
    public List<string> Cities { get; set; }
}*/

//private async void OnSubmitClicked(object sender, EventArgs e)
//{
//    // Handle form submission here
//    UserDialogs.Instance.Loading("Adding New Contact, please wait...");
//    Item item1 = maritalStatusPicker.SelectedItem as Item;
//    int job_position = item1.id;
//    string firstName = firstNameEntry.Text;
//    string lastName = lastNameEntry.Text;
//    //string memo = memoEditor.Text;
//    int sex = hommeRadioButton.IsChecked ? 1 : 2;
//    /* Item item = maritalStatusPicker.SelectedItem as Item;
//     int maritalStatus = item.id;*/
//    int maritalStatus = 0; // Valeur par défaut
//    if (maritalStatusPicker.SelectedItem != null)
//    {
//        Item item = maritalStatusPicker.SelectedItem as Item;
//        maritalStatus = item?.id ?? 0; // Si item est null, maritalStatus sera 0
//    }
//    DateTime birthDate = birthDatePicker.Date;
//    string birthPlace = birthPlaceEntry.Text;
//    string nationality = nationalityPicker.SelectedItem?.ToString();
//    //string city = citiesPicker.SelectedItem?.ToString();
//    string address = addressEntry.Text;
//    string identity = identityEntry.Text;
//    bool handicap = handicapSwitch.IsToggled;
//    string handicapDescription = handicap ? handicapDescriptionEditor.Text : null;
//    await InsertNewContact(lastName,firstName,sex,maritalStatus,birthDate,birthPlace,handicap,handicapDescription,job_position);
//    await App.Current.MainPage.DisplayAlert("INFO","CONTACT SAVED SUCCEFULY","OK") ;
//    await App.Current.MainPage.Navigation.PopAsync();
//    await App.Current.MainPage.Navigation.PopAsync();
//    UserDialogs.Instance.HideLoading();
//}