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
    public class BarSeriesModel : BaseViewModel
    {

        public int Id { get; set; } 
        public List<LandBarItem> ListOfItems { get; set; } = new List<LandBarItem>();
        public string Query_Name { get; set; } = "";
        public string BarSerie_Name { get; set; } = "";
        public string argument { get; set; } = "";
        public string value1 { get; set; } = "";
        public string value2 { get; set; } = "";
        public string value3 { get; set; } = "";
        public int NumberOfValues { get; set; } = 0;

        public bool Has_One_Value { get; set; } = false;
        public bool Has_Two_Value { get; set; } = false;
        public bool Has_Three_Value { get; set; } = false;

        public string TypeValue1 { get; set; } = "";
        public string TypeValue2 { get; set; } = "";
        public string TypeValue3 { get; set; } = "";





        private string selected_Arguemnt;
        public string Selected_Arguemnt { get => selected_Arguemnt; set => SetProperty(ref selected_Arguemnt, value); }


        private string selected_Value1;
        public string Selected_Value1 { get => selected_Value1; set => SetProperty(ref selected_Value1, value); }


        private string selected_Value2;
        public string Selected_Value2 { get => selected_Value2; set => SetProperty(ref selected_Value2, value); }

        private string selected_Value3;
        public string Selected_Value3 { get => selected_Value3; set => SetProperty(ref selected_Value3, value); }






        public static List<BarSeriesModel> GetAllBarItems(List<string> listOfStringCHarts,string Requette)
        {
            int j = 1;
            List < BarSeriesModel > Result = new List< BarSeriesModel >();
            foreach (string chart in listOfStringCHarts)
            {
                XDocument chartXml = XDocument.Parse(chart);
                XElement chartElement = chartXml.Element("Chart");

                BarSeriesModel barSeriesModel = new BarSeriesModel();
                barSeriesModel.Id = j;
                barSeriesModel.Query_Name = chartElement.Attribute("DataMember").Value;
                barSeriesModel.BarSerie_Name = chartElement.Attribute("Name").Value;

                barSeriesModel.argument = chartElement.Element("DataItems").Element("Dimension").Attribute("DataMember").Value;

                IEnumerable<XElement> measureElements = chartElement.Element("DataItems").Elements("Measure");

                int Number_Values = measureElements.ToList().Count();
                if (Number_Values > 3)
                {
                    Number_Values = 3;
                }

                for (int i = 0; i < Number_Values; i++)
                {
                    if (i == 0)
                    {
                        
                        try
                        {
                            barSeriesModel.value1 = GetValueFromMeasureElement(measureElements.ElementAtOrDefault(i));
                            barSeriesModel.TypeValue1 = GetTypeValueFromMeasureElement(measureElements.ElementAtOrDefault(i));
                        }
                        catch(Exception ex)
                        {

                        }
                        

                    }
                    else if (i == 1)
                    {
                        try
                        {
                            barSeriesModel.value2 = GetValueFromMeasureElement(measureElements.ElementAtOrDefault(i));
                            barSeriesModel.TypeValue2 = GetTypeValueFromMeasureElement(measureElements.ElementAtOrDefault(i));
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else if (i == 2)
                    {
                        try
                        {
                            barSeriesModel.value3 = GetValueFromMeasureElement(measureElements.ElementAtOrDefault(i));
                            barSeriesModel.TypeValue3 = GetTypeValueFromMeasureElement(measureElements.ElementAtOrDefault(i));
                        }
                        catch (Exception ex)
                        {

                        }
                    }


                }
              
                barSeriesModel.NumberOfValues = Number_Values;

                if (Number_Values==1)
                {
                    barSeriesModel.Has_One_Value= true;
                }
                else if (Number_Values == 2)
                {
                    barSeriesModel.Has_Two_Value = true;
                }
                else if (Number_Values == 3)
                {
                    barSeriesModel.Has_Three_Value = true;
                }

                //------------------------------------------

                DataTable AllDataForDashbord = GetGridListForDashbOARD(Requette);
                if (barSeriesModel.Has_One_Value)
                {
                    barSeriesModel.ListOfItems = AllDataForDashbord.AsEnumerable().Select(row => new LandBarItem(j,Convert.ToString(row.Field<Object>(barSeriesModel.argument)), Convert.ToDouble(row.Field<Object>(barSeriesModel.value1)))).ToList();
                    barSeriesModel.ListOfItems = barSeriesModel.ListOfItems.GroupBy(p => p.Name).Select(g =>
                    {
                        if(barSeriesModel.TypeValue1 == "Max")
                        {
                            return new LandBarItem(j, g.Key, g.Max(x => x.Number1));
                        }
                        else if (barSeriesModel.TypeValue1 == "Min")
                        {
                            return new LandBarItem(j, g.Key, g.Sum(x => x.Number1));
                        }
                        else if (barSeriesModel.TypeValue1 == "Count")
                        {
                            return new LandBarItem(j, g.Key, g.Count());
                        }
                        else if (barSeriesModel.TypeValue1 == "Average")
                        {
                            return new LandBarItem(j, g.Key, g.Average(x => x.Number1));
                        }
                        else
                        {
                            return new LandBarItem(j, g.Key, g.Sum(x => x.Number1));
                        }

                    }  ).ToList();
                }
                else if (barSeriesModel.Has_Two_Value)
                {
                    barSeriesModel.ListOfItems = AllDataForDashbord.AsEnumerable().Select(row => new LandBarItem(j,Convert.ToString(row.Field<Object>(barSeriesModel.argument)), Convert.ToDouble(row.Field<Object>(barSeriesModel.value1)), Convert.ToDouble(row.Field<Object>(barSeriesModel.value2)))).ToList();
                    barSeriesModel.ListOfItems = barSeriesModel.ListOfItems.GroupBy(p => p.Name).Select(g => {
                      LandBarItem item = new LandBarItem(j, g.Key);
                        if (barSeriesModel.TypeValue1 == "Max")
                        {
                            item.Number1 = g.Max(x => x.Number1);
                            
                        }
                        else if (barSeriesModel.TypeValue1 == "Min")
                        {
                            item.Number1 = g.Min(x => x.Number1);

                        }
                        else if (barSeriesModel.TypeValue1 == "Count")
                        {
                            item.Number1 = g.Count();

                        }
                        else if (barSeriesModel.TypeValue1 == "Average")
                        {
                            item.Number1 = g.Average(x => x.Number1);

                        }
                        else
                        {
                            item.Number1 = g.Sum(x => x.Number1);

                        }
                        //--------------------------------------------------

                        if (barSeriesModel.TypeValue2 == "Max")
                        {
                            item.Number2 = g.Max(x => x.Number2);

                        }
                        else if (barSeriesModel.TypeValue2 == "Min")
                        {
                            item.Number2 = g.Min(x => x.Number2);

                        }
                        else if (barSeriesModel.TypeValue2 == "Count")
                        {
                            item.Number2 = g.Count();

                        }
                        else if (barSeriesModel.TypeValue2 == "Average")
                        {
                            item.Number2 = g.Average(x => x.Number2);

                        }
                        else
                        {
                            item.Number2 = g.Sum(x => x.Number2);

                        }

                        return item;
                    }).ToList();

                }
                else if (barSeriesModel.Has_Three_Value)
                {
                    barSeriesModel.ListOfItems = AllDataForDashbord.AsEnumerable().Select(row => new LandBarItem(j,Convert.ToString(row.Field<Object>(barSeriesModel.argument)), Convert.ToDouble(row.Field<Object>(barSeriesModel.value1)), Convert.ToDouble(row.Field<Object>(barSeriesModel.value2)), Convert.ToDouble(row.Field<Object>(barSeriesModel.value3)))).ToList();
                    barSeriesModel.ListOfItems = barSeriesModel.ListOfItems.GroupBy(p => p.Name).Select(g => {
                        LandBarItem item = new LandBarItem(j, g.Key);
                        if (barSeriesModel.TypeValue1 == "Max")
                        {
                            item.Number1 = g.Max(x => x.Number1);

                        }
                        else if (barSeriesModel.TypeValue1 == "Min")
                        {
                            item.Number1 = g.Min(x => x.Number1);

                        }
                        else if (barSeriesModel.TypeValue1 == "Count")
                        {
                            item.Number1 = g.Count();

                        }
                        else if (barSeriesModel.TypeValue1 == "Average")
                        {
                            item.Number1 = g.Average(x => x.Number1);

                        }
                        else
                        {
                            item.Number1 = g.Sum(x => x.Number1);

                        }
                        //--------------------------------------------------

                        if (barSeriesModel.TypeValue2 == "Max")
                        {
                            item.Number2 = g.Max(x => x.Number2);

                        }
                        else if (barSeriesModel.TypeValue2 == "Min")
                        {
                            item.Number2 = g.Min(x => x.Number2);

                        }
                        else if (barSeriesModel.TypeValue2 == "Count")
                        {
                            item.Number2 = g.Count();

                        }
                        else if (barSeriesModel.TypeValue2 == "Average")
                        {
                            item.Number2 = g.Average(x => x.Number2);

                        }
                        else
                        {
                            item.Number2 = g.Sum(x => x.Number2);

                        }

                        //--------------------------------------------

                        if (barSeriesModel.TypeValue3 == "Max")
                        {
                            item.Number3 = g.Max(x => x.Number3);

                        }
                        else if (barSeriesModel.TypeValue3 == "Min")
                        {
                            item.Number3 = g.Min(x => x.Number3);

                        }
                        else if (barSeriesModel.TypeValue3 == "Count")
                        {
                            item.Number3 = g.Count();

                        }
                        else if (barSeriesModel.TypeValue3 == "Average")
                        {
                            item.Number3 = g.Average(x => x.Number3);

                        }
                        else
                        {
                            item.Number3 = g.Sum(x => x.Number3);

                        }

                        return item;
                    }).ToList();
                }










                //------------------------------------------
                j = j + 1;
                Result.Add(barSeriesModel);


            }


            return Result;


        }

        private static string GetTypeValueFromMeasureElement(XElement measureElement)
        {
            return measureElement != null ? measureElement.Attribute("SummaryType").Value : string.Empty;
        }
        private static string GetValueFromMeasureElement(XElement measureElement)
        {
            return measureElement != null ? measureElement.Attribute("DataMember").Value : string.Empty;
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
   
}
