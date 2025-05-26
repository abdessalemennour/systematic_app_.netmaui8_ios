using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPharma5.Model
{
    /*  public class Currency
      {
          public string Name { get; set; }
      }
      */
    /* public class currency
     {
         public int Id { get; set; }
         public string Name { get; set; }

         public currency() { }

         public currency(int id, string name)
         {
             Id = id;
             Name = name;
         }

         public static async Task<List<currency>> GetAllCurrencies()
         {
             List<currency> currencies = new List<currency>();
             string sqlCmd = "SELECT * FROM commercial_currency;";

             MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
             adapter.SelectCommand.CommandType = CommandType.Text;
             DataTable dt = new DataTable();
             adapter.Fill(dt);

             try
             {
                 foreach (DataRow row in dt.Rows)
                 {
                     currencies.Add(new currency(
                         Convert.ToInt32(row["Id"]),
                         row["Name"].ToString()
                     ));
                 }
             }
             catch (Exception ex)
             {
                 await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
             }

             return currencies;
         }
     }

     */
/*    public class currency
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }*/


}


