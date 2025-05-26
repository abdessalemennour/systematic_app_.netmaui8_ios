using MvvmHelpers;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.Model
{
    public class Contact_Partner :BaseViewModel
    {
        public int id {  get; set; }
        public int id_partner { get; set; }
        public string name_partner { get; set; }
        public string job_position { get; set; }
        private string firstName;
        public string FirstName { get => firstName; set => SetProperty(ref firstName, value); }

        private string lastName;
        public string LastName { get => lastName; set => SetProperty(ref lastName, value); }

        private int sex;
        public int Sex { get => sex; set => SetProperty(ref sex, value); }

        private int martal_status;
        public int Martal_status { get => martal_status; set => SetProperty(ref martal_status, value); }

        private DateTime birth_date;
        public DateTime Birth_date { get => birth_date; set => SetProperty(ref birth_date, value); }

        private string birth_place;
        public string Birth_place { get => birth_place; set => SetProperty(ref birth_place, value); }

        private int nationality;
        public int Nationality { get => nationality; set => SetProperty(ref nationality, value); }
        public string NationalityName { get; set; }
        /*  private string adress;
          public string Adress { get => adress; set => SetProperty(ref adress, value); }*/
        public int Adress { get; set; } // ID de l'adresse
        public string AddressName { get; set; } // Nom de l'adresse

        private int identity;
        public int Identity { get => identity; set => SetProperty(ref identity, value); }

        private int primary_occupation;
        public int Primary_occupation { get => primary_occupation; set => SetProperty(ref primary_occupation, value); }

        private bool handicap;
        public bool Handicap { get => handicap; set => SetProperty(ref handicap, value); }

        private string handicap_description;
        public string Handicap_description { get => handicap_description; set => SetProperty(ref handicap_description, value); }


        public Contact_Partner()
        {
            
        }
        public Contact_Partner(int id,int id_partner,string name_partner,string job_position,string firstName,string lastName)
        {
            this.id = id;
            this.id_partner = id_partner;
            this.name_partner = name_partner;
            this.job_position = job_position;
            this.firstName = firstName;
            this.lastName = lastName;

            
        }

        public Contact_Partner Clone()
        {
            return new Contact_Partner
            {
                id = this.id,
                id_partner = this.id_partner,
                name_partner = this.name_partner,
                job_position = this.job_position,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Sex = this.Sex,
                Martal_status = this.Martal_status,
                Birth_date = this.Birth_date,
                Birth_place = this.Birth_place,
                Nationality = this.Nationality,
                Adress = this.Adress,
                AddressName = this.AddressName,
                Identity = this.Identity,
                Primary_occupation = this.Primary_occupation,
                Handicap = this.Handicap,
                Handicap_description = this.Handicap_description
            };
        }
        /*  public async static Task<Contact_Partner> GetContact_PartnerById(int idContact)
          {
              string sqlCmd = "select * from commercial_partner_contact c\r\nleft join atooerp_person on atooerp_person.id = c.person\r\nwhere c.id = "+idContact+" ;";
              Contact_Partner partner = new Contact_Partner();

              if (await DbConnection.Connecter3())
              {
                  MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

                  MySqlDataReader reader = cmd.ExecuteReader();
                  while (reader.Read())
                  {
                      try
                      {
                          partner.id = Convert.ToInt32(reader["person"]);
                          partner.FirstName = Convert.ToString(reader["first_name"]);
                          partner.LastName = Convert.ToString(reader["last_name"]);
                          partner.Birth_date = Convert.ToDateTime(reader["birth_date"]);
                          partner.Birth_place = Convert.ToString(reader["birth_place"]);
                          partner.Sex = Convert.ToInt32(reader["sex"])  ;
                          partner.Martal_status = reader["marital_status"] != DBNull.Value ? Convert.ToInt32(reader["marital_status"]) : 0;
                          //partner.Nationality =  reader["nationality"] != DBNull.Value ? Convert.ToInt32(reader["nationality"]) : 0;
                          partner.Identity =  reader["identity"] != DBNull.Value ? Convert.ToInt32(reader["identity"]) : 0;
                          partner.Primary_occupation =  reader["primary_occupation"] != DBNull.Value ? Convert.ToInt32(reader["primary_occupation"]) : 0;
                          partner.Nationality = Convert.ToString(reader["nationality"]);
                          partner.Adress = Convert.ToString(reader["address"]);
                          partner.Handicap = Convert.ToBoolean(reader["handicap"]);
                          partner.Handicap_description = Convert.ToString(reader["handicap_description"]);


                      }

                      catch (Exception ex)
                      {

                      }
                  }
                  reader.Close();


              }
              return partner;

          }*/
        public async static Task<Contact_Partner> GetContact_PartnerById(int idContact)
        {
            string sqlCmd = @"
        SELECT c.*, 
               p.first_name, p.last_name, p.birth_date, p.birth_place, p.sex, 
               p.marital_status, p.nationality, p.identity, p.primary_occupation, 
               p.address AS address_id, p.handicap, p.handicap_description, 
               co.name_en_gb AS country_name,
               a.name AS address_name
        FROM commercial_partner_contact c
        LEFT JOIN atooerp_person p ON p.id = c.person
        LEFT JOIN atooerp_country co ON co.id = p.nationality
        LEFT JOIN atooerp_address a ON a.id = p.address
        WHERE c.id = @idContact";

            Contact_Partner partner = new Contact_Partner();

            if (await DbConnection.Connecter3())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                cmd.Parameters.AddWithValue("@idContact", idContact);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {
                        partner.id = Convert.ToInt32(reader["person"]);
                        partner.FirstName = Convert.ToString(reader["first_name"]);
                        partner.LastName = Convert.ToString(reader["last_name"]);
                        partner.Birth_date = Convert.ToDateTime(reader["birth_date"]);
                        partner.Birth_place = Convert.ToString(reader["birth_place"]);
                        partner.Sex = Convert.ToInt32(reader["sex"]);
                        partner.Martal_status = reader["marital_status"] != DBNull.Value ? Convert.ToInt32(reader["marital_status"]) : 0;
                        partner.Identity = reader["identity"] != DBNull.Value ? Convert.ToInt32(reader["identity"]) : 0;
                        partner.Primary_occupation = reader["primary_occupation"] != DBNull.Value ? Convert.ToInt32(reader["primary_occupation"]) : 0;
                        partner.Nationality = reader["nationality"] != DBNull.Value ? Convert.ToInt32(reader["nationality"]) : 0;
                        partner.NationalityName = reader["country_name"] != DBNull.Value ? Convert.ToString(reader["country_name"]) : "Unknown";
                        partner.Adress = reader["address_id"] != DBNull.Value ? Convert.ToInt32(reader["address_id"]) : 0; // ID de l'adresse
                        partner.AddressName = reader["address_name"] != DBNull.Value ? Convert.ToString(reader["address_name"]) : "Unknown"; // Nom de l'adresse
                        partner.Handicap = Convert.ToBoolean(reader["handicap"]);
                        partner.Handicap_description = Convert.ToString(reader["handicap_description"]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur : " + ex.Message);
                    }
                }
                reader.Close();
            }
            return partner;
        }


        public static async Task<string> GetCountryNameById(int countryId)
        {
            string sqlCmd = "SELECT name_en_gb FROM atooerp_country WHERE Id = @countryId";

            if (await DbConnection.Connecter3())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                cmd.Parameters.AddWithValue("@countryId", countryId);

                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "Unknown";
            }

            return "Unknown";
        }
        public async static Task<List<Item>> GetMaritalStatus()
        {
            string sqlCmd = "select * from atooerp_person_marital_status;";
            List<Item> Items = new List<Item>();

            if (await DbConnection.Connecter3())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {
                        Item item = new Item();
                        item.Id = Convert.ToInt32(reader["Id"]);
                        item.Name = Convert.ToString(reader["name"]);

                        Items.Add(item);

 

                    }
                    catch (Exception ex)
                    {


                    }
                }
                reader.Close();


            }
            return Items;

        }

        public static async Task<List<Item>> GetAllCountries()
        {
            List<Item> countries = new List<Item>();
            string sqlCmd = "SELECT Id, name_en_gb FROM atooerp_country";

            if (await DbConnection.Connecter3())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

                MySqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    countries.Add(new Item
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("name_en_gb")
                    });
                }
            }

            return countries;
        }
    }
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


}
