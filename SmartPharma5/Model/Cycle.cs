using MySqlConnector;
using System.ComponentModel;

namespace SmartPharma5.Model
{
    public class Cycle
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string Memo { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public Cycle(uint id, string name, DateTime beginDate, DateTime endDate)
        {
            Id = id;
            Name = name;
            BeginDate = beginDate;
            EndDate = endDate;
        }
        public Cycle(DateTime beginDate, DateTime endDate)
        {
            BeginDate = beginDate;
            EndDate = endDate;
        }

        public Cycle()
        {
        }

        public static async Task<BindingList<Cycle>> GetAllCycle()
        {
            var list = new BindingList<Cycle>();
            string sqlCmd = "SELECT * FROM marketing_quiz_cycle;";



            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        list.Add(new Cycle(
                            Convert.ToUInt32(reader["Id"]),
                            reader["name"].ToString(),
                            Convert.ToDateTime(reader["begin_date"]),
                            Convert.ToDateTime(reader["end_date"])
                            ));

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
            return list;
        }

        internal static async Task<Cycle> GetCycleByPartnerFormId(int id)
        {
            string sqlCmd = "SELECT * FROM  marketing_quiz_partner_form where id = " + id + ";";
            Cycle cycle = null;


            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        cycle = new Cycle(
                            Convert.ToDateTime(reader["begin_date"]),
                            Convert.ToDateTime(reader["end_date"])
                            );

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
            return cycle;
        }
    }
}
