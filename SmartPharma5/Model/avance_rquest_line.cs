using MySqlConnector;

namespace SmartPharma5.Model
{
    public class avance_rquest_line
    {
        public int id { get; set; }
        public int deposit_request { get; set; }
        public decimal amount { get; set; }
        public int period { get; set; }
        public string name_periode { get; set; }

        public avance_rquest_line(int id, int deposit_request, decimal amount, int period, string name_periode)
        {
            this.id = id;
            this.deposit_request = deposit_request;
            this.amount = amount;
            this.period = period;
            this.name_periode = name_periode;
        }

        public async static Task insertRequestLine(int deposit_request, decimal amount, int period)
        {
            string _Amount = amount.ToString().Replace(',', '.');
            string sqlCmd = "INSERT INTO hr_deposit_request_line (deposit_request , amount , period) values ( " + deposit_request + " , " + _Amount + " , " + period + " ) ;";
            DbConnection.Connecter();
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);


            try
            {
                cmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error YYttY", ex.Message, "Ok");

            }

            DbConnection.Deconnecter();

        }
        public static avance_rquest_line getByDepositRequest(int id)
        {
            string sqlCmd = " select hr_deposit_request_line.* ,hr_period.name as name_period  from hr_deposit_request_line , hr_period where id = +" + id + " and hr_deposit_request_line.period=hr_period.id;";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();

            MySqlDataReader reader = cmd.ExecuteReader();

            reader.Read();
            avance_rquest_line avance_request_line = new avance_rquest_line(Convert.ToInt32(reader["id"]),
                        Convert.ToInt32(reader["deposit_request"]),
                       Convert.ToDecimal(reader["amount"]),
                       Convert.ToInt32(reader["period"]),
                       Convert.ToString(reader["name_period"])

                       );

            reader.Close();
            DbConnection.Deconnecter();


            return avance_request_line;

        }
        public async static Task update(int id, decimal amount)
        {
            string _Amount = amount.ToString().Replace(',', '.');
            string sqlCmd = "update hr_deposit_request_line set amount = " + _Amount + " where hr_deposit_request_line.deposit_request = " + id + " ;";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {
                cmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");

            }

            DbConnection.Deconnecter();

        }

        public async static Task delete(int id)
        {
            string sqlCmd = "delete from hr_deposit_request_line where hr_deposit_request_line.deposit_request = " + id + " ;";
            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();
            try
            {
                cmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");

            }

            DbConnection.Deconnecter();
        }






    }
}
