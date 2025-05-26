using MvvmHelpers;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using SmartPharma5.Model;

namespace SmartPharma5.ModelView
{
    public class ContactPartnerMV : BaseViewModel
    {
        public int id_partner;
        public int opp_id;

        private List<Contact_Partner> contacts;
        public List<Contact_Partner> Contacts { get => contacts; set => SetProperty(ref contacts, value); }

        public ContactPartnerMV()
        {
            
        }
        public ContactPartnerMV(int id)
        {
            this.id_partner = id;            
            this.Contacts = new List<Contact_Partner>();
            this.Contacts = getContactsOfPartner(id_partner).Result;
        }
        public ContactPartnerMV(int id, int opp)
        {
            this.id_partner = id;
            this.opp_id= opp;
            this.Contacts = new List<Contact_Partner>();
            this.Contacts = getContactsOfPartner(id_partner).Result;
        }
        public static async Task<List<Contact_Partner>> getContactsOfPartner(int id)
        {
            string sqlCmd = "select commercial_partner_contact.id as id , atooerp_person.id as id_person , commercial_partner.name as partner_name, commercial_partner_contact.partner as id_partner, commercial_job_position.name as job_position_name,atooerp_person.first_name,atooerp_person.last_name from commercial_partner_contact\r\nleft join commercial_job_position on commercial_job_position.id=commercial_partner_contact.job_position\r\nleft join atooerp_person on atooerp_person.id=commercial_partner_contact.person\r\nleft join commercial_partner on commercial_partner.id=commercial_partner_contact.partner\r\nWHERE commercial_partner_contact.partner=" + id+";";
            List<Contact_Partner> partner = new List<Contact_Partner>();
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    partner.Add(new Contact_Partner(Convert.ToInt32(reader["id"]), Convert.ToInt32(reader["id_partner"]), reader["partner_name"].ToString(), reader["job_position_name"].ToString(), reader["first_name"].ToString(), reader["last_name"].ToString()));
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
}
