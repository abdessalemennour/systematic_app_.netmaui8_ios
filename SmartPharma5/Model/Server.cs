using MySqlConnector;

namespace SmartPharma5.Models
{
    public class Server
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Data_base { get; set; }
        public int Port { get; set; }

        public Server() { }

        public Server(string name, string password, string address, string data_base, int port)
        {
            Name = name;
            Password = password;
            Address = address;
            Data_base = data_base;
            Port = port;
        }

        public async Task<bool> ServerConnectionIsTrue()
        {

            string ConnectionString = "Server=" + this.Address + ";database=" + Data_base + ";Uid=" + Name + "; Pwd=" + Password + ";Allow User Variables=true;Connect Timeout = 6;";
            MySqlConnection con = new MySqlConnection(ConnectionString);

            try
            {
                con.Open();
                con.Close();
                return true;
            }
            catch (Exception ex)
            { return false; }
        }


    }
}
