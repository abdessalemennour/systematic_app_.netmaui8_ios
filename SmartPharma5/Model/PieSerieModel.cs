using MvvmHelpers;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartPharma5.Model
{
    public class PieSerieModel : BaseViewModel
    {
        public int Id { get; set; }
        public List<LandPieItem> ListOfItems { get; set; } = new List<LandPieItem>();
        public string Query_Name { get; set; } = "";
        public string Pie_Name { get; set; } = "";
        public string argument { get; set; } = "";
        public string value { get; set; } = "";
   
     




        private string selected_Arguemnt;
        public string Selected_Arguemnt { get => selected_Arguemnt; set => SetProperty(ref selected_Arguemnt, value); }


        private string selected_Value;
        public string Selected_Value1 { get => selected_Value; set => SetProperty(ref selected_Value, value); }


        public static List<PieSerieModel> GetAllPieItems(List<string> listOfStringCHarts, string Requette)
        {
            int j = 1;
            DataTable AllDataForDashbord = GetGridListForDashbOARD(Requette); 
            List<PieSerieModel> Result = new List<PieSerieModel>();
            foreach(string chart in listOfStringCHarts)
            {
                XDocument chartXml = XDocument.Parse(chart);
                XElement chartElement = chartXml.Element("Pie");

                PieSerieModel barSeriesModel = new PieSerieModel();
                barSeriesModel.Id = j;
                barSeriesModel.Query_Name = chartElement.Attribute("DataMember").Value;
                barSeriesModel.Pie_Name = chartElement.Attribute("Name").Value;
                barSeriesModel.argument = chartElement.Element("DataItems").Element("Dimension").Attribute("DataMember").Value;
                barSeriesModel.value = chartElement.Element("DataItems").Element("Measure").Attribute("DataMember").Value;

                
                barSeriesModel.ListOfItems = AllDataForDashbord.AsEnumerable().Select(row => new LandPieItem(Convert.ToString(row.Field<Object>(barSeriesModel.argument)),Convert.ToDouble(row.Field<Object>(barSeriesModel.value)), j)).ToList();
                barSeriesModel.ListOfItems = barSeriesModel.ListOfItems.GroupBy(p => p.Name).Select(g => new LandPieItem(g.Key, g.Sum(x => x.Number), j)).ToList();

                Result.Add(barSeriesModel);
            }
            return Result;


        }
        public static DataTable GetGridListForDashbOARD(string Requette)
        {
            DataTable dt = new DataTable();
            string sqlCmd = Requette;

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            adapter.Fill(dt);
            DbConnection.Deconnecter();
            return dt;
        }



    }

    public class LandPieItem{
        public LandPieItem()
        {
        }

        public LandPieItem(string name, double number, int id_Pie)
        {
            Name = name;
            Number = number;
            this.id_Pie = id_Pie;
        }

        public string Name { get; set; }
        public double Number { get; set; }
        public int id_Pie { get; set; }

        
}

}
