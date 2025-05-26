using MySqlConnector;

namespace SmartPharma5.Model
{
    public class deposit_line
    {
        #region attribut
        public int id { get; set; }

        public int deposit { get; set; }

        public int period { get; set; }

        public string period_name { get; set; }

        public DateTime start_period { get; set; }

        public DateTime end_period { get; set; }

        public decimal amount { get; set; }

        #endregion attribut


        #region constructor

        public deposit_line()
        {
        }
        public deposit_line(int id, int deposit, int period, decimal amount, string period_name, DateTime start_period, DateTime end_period)
        {
            this.id = id;
            this.deposit = deposit;
            this.period = period;
            this.amount = amount;
            this.period_name = period_name;
            this.start_period = start_period;
            this.end_period = end_period;

        }

        #endregion constructor



        public static List<deposit_line> getAllDepositLineByDeposit(int deposit)
        {

            string sqlCmd = "SELECT hr_deposit_line.* , hr_period.name as name_period, hr_period.end_date as end_period , hr_period.beginning_date as start_period from hr_deposit_line , hr_period  where" +
                " hr_deposit_line.deposit = " + deposit + "  and  hr_period.id = hr_deposit_line.period order by id desc ;";
            Console.WriteLine(sqlCmd);
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            List<deposit_line> list_deposit = new List<deposit_line>();

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list_deposit.Add(new deposit_line(Convert.ToInt32(reader["id"]),
                    Convert.ToInt32(reader["deposit"]),
                    Convert.ToInt32(reader["period"]),
                    Convert.ToDecimal(reader["amount"]),
                    Convert.ToString(reader["name_period"]),
                    Convert.ToDateTime(reader["start_period"]),
                    Convert.ToDateTime(reader["end_period"])
                          ));
            }


            reader.Close();
            DbConnection.Deconnecter();
            Console.WriteLine("______________gggg_");
            Console.WriteLine(user_contrat.iduser);
            Console.WriteLine("______________gggg_");


            return list_deposit;

        }
        public static decimal SumDepositLine(int deposit)
        {

            string sqlCmd = "SELECT hr_deposit_line.id,sum(hr_deposit_line.amount) as sum  from hr_deposit_line , hr_period  where " +
                " hr_deposit_line.deposit = " + deposit + "  and  hr_period.id = hr_deposit_line.period order by id desc ;";
            Console.WriteLine(sqlCmd);
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);

            List<deposit_line> list_deposit = new List<deposit_line>();

            MySqlDataReader reader = cmd.ExecuteReader();
            decimal total_amount;
            reader.Read();
            total_amount = Convert.ToDecimal(reader["sum"]);




            reader.Close();
            DbConnection.Deconnecter();

            return total_amount;

        }
        public async static Task<int> CheckExistDepositLine(int deposit)
        {

            string sqlCmd = "SELECT hr_deposit_line.id , count( hr_deposit_line.id) as number  from hr_deposit_line , hr_period  where " +
                " hr_deposit_line.deposit = " + deposit + "  and  hr_period.id = hr_deposit_line.period order by id desc ;";
            Console.WriteLine(sqlCmd);
            DbConnection.Deconnecter();
            DbConnection.Connecter();

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);



            MySqlDataReader reader = cmd.ExecuteReader();
            int total_amount;
            reader.Read();
            total_amount = Convert.ToInt32(reader["number"]);




            reader.Close();
            DbConnection.Deconnecter();

            return total_amount;

        }


    }
}
