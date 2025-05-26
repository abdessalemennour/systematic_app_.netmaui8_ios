using DevExpress.Maui.DataGrid;

using MvvmHelpers;
using MySqlConnector;
using SmartPharma5.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization.DataContracts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SmartPharma5.ModelView
{
    public class GridMV : BaseViewModel
    {

        
       

        public string XML_FILE = "";
        public string REQUETTE = "";
        
        
        

        private List<ModelGridParametre> list_Paramettre_final;
        public List<ModelGridParametre> List_Paramettre_final { get => list_Paramettre_final; set => SetProperty(ref list_Paramettre_final, value); }


        public Task<DataTable> Result { get; set; }
        public DataTable GridList { get; set; }

        public List<string> Names;

        public List<GridElement> ContentList= new List<GridElement>();

        public List<GridColumnAttributeSymmary> SUMMURIES;

        public GridMV(List<ModelGridParametre> list_Paramettre_final,string xml_file)
        {
            XML_FILE = xml_file;
          
            List<GridModel> AllGrids = ExtractGridTags(XML_FILE);
            List<QueryModel> querrys = GetQueryContents(XML_FILE);
            List<string> Params = ExtractParameters(XML_FILE);
            List<ModelGridParametre> ParametresList = new List<ModelGridParametre>();
            try
            {
                ParametresList = ParseParameters(Params[0]);

            }
            catch (Exception ex)
            {

            }


            QueryModel Query = querrys.Where(p => p.Name == AllGrids[0].Query_Name).FirstOrDefault();
            if (Query != null)
            {
                REQUETTE = ExtractSqlFromXml(Query.Contenu).Replace("&lt;", "<").Replace("&gt;", ">");
                string querry2 = AddSpacesToParameters(REQUETTE,user_contrat.iduser);
                string querry1 = ReplaceParameters(querry2, list_Paramettre_final);
                this.REQUETTE = querry1;
                List<string> Paramettre_requette = GetParameterNames(querry2);
                List_Paramettre_final = list_Paramettre_final;
                Result = Task.Run(() => GetGridList());
                GridList = Result.Result;
                Names = GetDataMembersFromXml(GetDataItemsTagWithContent(XML_FILE));
                ContentList = GetFieldTypes(GetDataSetTagWithContent(XML_FILE), Names);
                List<GridColumn> gridColumnSummaries = new List<GridColumn>();
                gridColumnSummaries = ExtractGridColumnsFromXaml(AllGrids[0].Contenu);
                List<GridColumnSummary1> gridColumnSummaries1 = ExtractGridColumnSummariesFromXaml(AllGrids[0].Contenu);
                SUMMURIES = MapGridColumnsToSummaries(gridColumnSummaries, gridColumnSummaries1);

            }

        }

        public GridMV(int id)
        {
         
            XML_FILE=GetXMLFILE(id);
            List<GridModel> AllGrids = ExtractGridTags(XML_FILE);
            List<QueryModel> querrys = GetQueryContents(XML_FILE);
            List<string> Params = ExtractParameters(XML_FILE);
            List<ModelGridParametre> ParametresList = new List<ModelGridParametre>();
            try
            {
                ParametresList = ParseParameters(Params[0]);
            }
            catch(Exception ex)
            {

            }
            

            
                QueryModel Query = querrys.Where(p => p.Name== AllGrids[0].Query_Name).FirstOrDefault();
                if (Query != null)
                {
                    REQUETTE = ExtractSqlFromXml(Query.Contenu).Replace("&lt;", "<").Replace("&gt;", ">");
                    string querry2 = AddSpacesToParameters(REQUETTE,user_contrat.iduser);
                    string querry1 = ReplaceParameters(querry2, ParametresList);
                    this.REQUETTE = querry1;
                    List<string> Paramettre_requette = GetParameterNames(querry2);
                    List_Paramettre_final = FilterModelGridParametres(ParametresList, Paramettre_requette);
                    Result = Task.Run(() => GetGridList());
                    GridList = Result.Result;
                    Names = GetDataMembersFromXml(GetDataItemsTagWithContent(XML_FILE));
                    ContentList = GetFieldTypes(GetDataSetTagWithContent(XML_FILE), Names);
                    List<GridColumn> gridColumnSummaries = new List<GridColumn>();
                    gridColumnSummaries = ExtractGridColumnsFromXaml(AllGrids[0].Contenu);
                    List<GridColumnSummary1> gridColumnSummaries1 = ExtractGridColumnSummariesFromXaml(AllGrids[0].Contenu);
                    SUMMURIES = MapGridColumnsToSummaries(gridColumnSummaries, gridColumnSummaries1);

                }
              

            
            //--------------------------------------------


            //REQUETTE = ExtractSqlFromXml(querrys[0].Contenu).Replace("&lt;", "<").Replace("&gt;", ">");

            //List<string> Params = ExtractParameters(XML_FILE);
            //List<ModelGridParametre> ParametresList = ParseParameters(Params[0]);


            //string querry2 = AddSpacesToParameters(REQUETTE);

           // string querry1 = ReplaceParameters(querry2, ParametresList);

            //this.REQUETTE = querry1;

            //----------------------------------------------

            //List<string> Paramettre_requette = GetParameterNames(querry2);

            //List_Paramettre_final = FilterModelGridParametres(ParametresList, Paramettre_requette);
            //Result =  Task.Run(() => GetGridList());
           // GridList = Result.Result;
            //foreach(GridModel st in AllGrids)
            //{
            //    Names = GetDataMembersFromXml(GetDataItemsTagWithContent(XML_FILE));
            //    ContentList = GetFieldTypes(GetDataSetTagWithContent(XML_FILE), Names);
            //    List<GridColumn> gridColumnSummaries = new List<GridColumn>();
            //    gridColumnSummaries = ExtractGridColumnsFromXaml(st.Contenu);
            //    List<GridColumnSummary1> gridColumnSummaries1 = ExtractGridColumnSummariesFromXaml(st.Contenu);
            //    SUMMURIES = MapGridColumnsToSummaries(gridColumnSummaries, gridColumnSummaries1);
                

            //};
            
        }



        public DataTable GetGridList()
        {
            DataTable dt = new DataTable();
            string sqlCmd = REQUETTE ;

            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd, DbConnection.con);
            adapter.SelectCommand.CommandType = CommandType.Text;
            adapter.Fill(dt);
            DbConnection.Deconnecter();
            return dt;
        }


        #region XML --> (XML-NAMES)
        static string GetDataItemsTagWithContent(string xmlString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            XmlNode gridNode = xmlDoc.SelectSingleNode("//Grid")!;

            if (gridNode != null)
            {
                XmlNode dataItemsNode = gridNode.SelectSingleNode(".//DataItems")!;

                if (dataItemsNode != null)
                {
                    return dataItemsNode.OuterXml;
                }
            }

            return null;
        }
        #endregion


        #region (XML-TYPE) ,NAMES COLUMN --> LIST GRID ELEMENT

        static List<GridElement> GetFieldTypes(string xmlData, List<string> fieldNames)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            List<GridElement> ResultList = new List<GridElement>();
            Dictionary<string, string> fieldTypes = new Dictionary<string, string>();

            foreach (var fieldName in fieldNames)
            {
                XmlNode fieldNode = xmlDoc.SelectSingleNode($"//Field[@Name='{fieldName}']")!;

                if (fieldNode != null)
                {
                    string fieldType = fieldNode.Attributes!["Type"]!.Value;
                    fieldTypes[fieldName] = fieldType;
                }
                else
                {
                    fieldTypes[fieldName] = null;
                }
            }

            foreach (var fieldName in fieldNames)
            {
                if (fieldTypes.ContainsKey(fieldName))
                {
                    Console.WriteLine($"Le champ '{fieldName}' a le type : {fieldTypes[fieldName]}");
                    ResultList.Add(new GridElement(fieldName, fieldTypes[fieldName]));
                }
                else
                {
                    Console.WriteLine($"Le champ '{fieldName}' n'a pas été trouvé dans la définition XML.");
                }
            }
            return ResultList;


        }
        #endregion


        #region XML --> (XML-TYPE)


        static string GetDataSetTagWithContent(string xmlString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            XmlNode dataSetNode = xmlDoc.SelectSingleNode("//DataSet");

            if (dataSetNode != null)
            {
                return dataSetNode.OuterXml;
            }

            return null;
        }
      

        #endregion



        #region XML-NAMES --> LIST NAME GRID COLUMN
        static List<string> GetDataMembersFromXml(string xmlString)
        {
            List<string> dataMembers = new List<string>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            XmlNodeList dimensionNodes = xmlDoc.SelectNodes("//Dimension")!;
            XmlNodeList measureNodes = xmlDoc.SelectNodes("//Measure")!;

            foreach (XmlNode node in dimensionNodes)
            {
                string dataMember = node.Attributes!["DataMember"]!.Value;
                dataMembers.Add(dataMember);
            }

            foreach (XmlNode node in measureNodes)
            {
                string dataMember = node.Attributes!["DataMember"]!.Value;
                dataMembers.Add(dataMember);
            }

            return dataMembers;
        }
        #endregion


        #region XML --> REQUETTE
        //-------------------------Foction detecte le querry du xml file -----------------------------------/
        public static List<QueryModel> GetQueryContents(string xmlString)
        {
            List<QueryModel> queryList = new List<QueryModel>();
            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                xmlDoc.LoadXml(xmlString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de lecture XML : {ex.Message}");
                // Peut-être que vous voulez gérer l'exception d'une manière spécifique ici.
                // Pour l'exemple, la fonction renvoie une liste vide en cas d'erreur.
                return queryList;
            }

            XmlNodeList queryNodes = xmlDoc.SelectNodes("//Query[@Type='CustomSqlQuery']");

            foreach (XmlNode queryNode in queryNodes)
            {
                string name = queryNode.Attributes["Name"]?.Value;
                string contenu = queryNode.OuterXml;
                queryList.Add(new QueryModel(name, contenu));
            }

            return queryList;
        }
        static string RemoveNonVisibleCharacters(string input)
        {
            // Utiliser une expression régulière pour supprimer les caractères non visibles
            string pattern = @"[^\x20-\x7E]";
            string cleanedString = Regex.Replace(input, pattern, string.Empty);
            return cleanedString;
        }

        public static string ExtractSqlFromXml(string xml)
        {
            string pattern = @"<Sql>(.*?)<\/Sql>";
            Match match = Regex.Match(xml, pattern, RegexOptions.Singleline);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return "Balise <Sql> non trouvée dans la chaîne XML.";
        }

        #endregion


        #region GridComumns -> ToTalSummary
        static List<GridColumn> ExtractGridColumnsFromXaml(string xaml)
        {
            List<GridColumn> gridColumns = new List<GridColumn>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xaml);

            XmlNodeList dataItems = xmlDoc.SelectNodes("//DataItems/*");

            if (dataItems != null)
            {
                foreach (XmlNode child in dataItems)
                {
                    if (child.Name == "Measure" || child.Name == "Dimension")
                    {
                        // Extraire les attributs DataMember et DefaultId
                        string dataMember = child.Attributes["DataMember"].Value;
                        string defaultId = child.Attributes["DefaultId"].Value;

                        // Ajouter à la liste des colonnes
                        gridColumns.Add(new GridColumn(dataMember, defaultId));
                    }
                }
            }

            return gridColumns;
        }
        static List<GridColumnSummary1> ExtractGridColumnSummariesFromXaml(string xaml)
        {
            List<GridColumnSummary1> gridColumnSummaries = new List<GridColumnSummary1>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xaml);

            XmlNodeList gridColumnNodes = xmlDoc.SelectNodes("//GridColumns/*");

            if (gridColumnNodes != null)
            {
                foreach (XmlNode node in gridColumnNodes)
                {
                    if (node.Name == "GridDimensionColumn")
                    {
                        string defaultId = node.SelectSingleNode("Dimension")?.Attributes["DefaultId"]?.Value;

                        if (node.SelectSingleNode("Totals/Total") != null)
                        {
                            gridColumnSummaries.Add(new GridColumnSummary1(defaultId, "count"));
                        }
                        else
                        {
                            gridColumnSummaries.Add(new GridColumnSummary1(defaultId, "non"));
                        }
                    }
                    else if (node.Name == "GridMeasureColumn")
                    {
                        string defaultId = node.SelectSingleNode("Measure")?.Attributes["DefaultId"]?.Value;

                        XmlNodeList totalNodes = node.SelectNodes("Totals/Total");

                        if (totalNodes != null && totalNodes.Count > 0)
                        {
                            foreach (XmlNode totalNode in totalNodes)
                            {
                                string summaryType = totalNode.Attributes["Type"]?.Value;

                                if (summaryType == null)
                                {
                                    // If <Total /> is present, use "count" as SummaryType
                                    gridColumnSummaries.Add(new GridColumnSummary1(defaultId, "count"));
                                }
                                else
                                {
                                    gridColumnSummaries.Add(new GridColumnSummary1(defaultId, summaryType));
                                }
                            }
                        }
                        else
                        {
                            // If there are no Total nodes, add "non" and "count"
                            gridColumnSummaries.Add(new GridColumnSummary1(defaultId, "non"));
                            gridColumnSummaries.Add(new GridColumnSummary1(defaultId, "count"));
                        }
                    }
                }
            }

            return gridColumnSummaries;



        }

        static List<GridColumnAttributeSymmary> MapGridColumnsToSummaries(List<GridColumn> gridColumns, List<GridColumnSummary1> gridColumnSummaries)
        {
            List<GridColumnAttributeSymmary> result = new List<GridColumnAttributeSymmary>();

            foreach (var gridColumnSummary in gridColumnSummaries)
            {
                // Find the matching GridColumn based on DefaultId
                GridColumn matchingGridColumn = gridColumns.Find(gc => gc.DefaultId == gridColumnSummary.Attribute);

                if (matchingGridColumn != null)
                {
                    // Add the mapping to the result list
                    result.Add(new GridColumnAttributeSymmary
                    {
                        DataMember = matchingGridColumn.DataMember,
                        summaryType = gridColumnSummary.SummaryType
                    });
                }
            }

            return result;
        }

        #endregion


        #region XML - LISTGRIDS
        public static List<GridModel> ExtractGridTags(string xaml)
        {
            List<GridModel> gridModels = new List<GridModel>();

            // Utilisation d'une expression régulière pour trouver les balises <Grid></Grid> avec ou sans options
            string pattern = @"<Grid\b[^>]*>.*?</Grid>";
            Regex regex = new Regex(pattern, RegexOptions.Singleline);
            MatchCollection matches = regex.Matches(xaml);

            // Ajout des correspondances à la liste sous forme de GridModel
            foreach (Match match in matches)
            {
                string name = GetAttributeValue(match.Value, "Name");
                string queryName = GetAttributeValue(match.Value, "DataMember");

                gridModels.Add(new GridModel(name, match.Value, queryName));
            }

            return gridModels;
        }

        // Fonction utilitaire pour extraire la valeur d'un attribut d'une balise XML
        private static string GetAttributeValue(string xml, string attributeName)
        {
            string pattern = $@"{attributeName}=""([^""]*)""";
            Match match = Regex.Match(xml, pattern);
            return match.Success ? match.Groups[1].Value : null;
        }
        #endregion


        #region DataBase => XMLFILE
        public static string GetXMLFILE(int id)
        {
            string Result="";
            try
            {
                string sqlCmd1 = "SELECT * FROM atooerp_app_dashboard where Id="+id+"";

                DbConnection.Deconnecter();
                DbConnection.Connecter();

                MySqlCommand cmd = new MySqlCommand(sqlCmd1, DbConnection.con);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {
                        var name = reader["name"].ToString();
                        //Result = (reader["content"]).ToString();
                        int columnIndex = reader.GetOrdinal("content");
                        byte[] blobData = (byte[])reader.GetValue(columnIndex);
                        Result = Encoding.UTF8.GetString(blobData);

                        string _byteordermarkutf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                        if (Result.StartsWith(_byteordermarkutf8))
                        {
                            Result = Result.Remove(0, _byteordermarkutf8.Length);
                        }



                    }
                    catch (Exception ex)
                    {
                        return null;

                    }

                }
                reader.Close();
                DbConnection.Deconnecter();




            }
            catch (Exception e)
            {
                return null;

            }



            return Result;




        }
        #endregion


        #region XML - Parametres
        static List<string> ExtractParameters(string xmlString)
        {
            List<string> parametersContentList = new List<string>();

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);

                // Sélectionne tous les éléments "Parameters" dans le document XML qui ne sont pas des enfants de "DataSources"
                XmlNodeList parametersNodes = xmlDoc.SelectNodes("//Parameters[not(ancestor::DataSources)]");

                foreach (XmlNode node in parametersNodes)
                {
                    // Ajoute le contenu de chaque élément "Parameters" à la liste
                    parametersContentList.Add(node.OuterXml);
                }

                return parametersContentList;
            }
            catch (Exception ex)
            {
                // En cas d'erreur, retourne une liste vide et affiche un message d'erreur
                Console.WriteLine("Une erreur s'est produite : " + ex.Message);
                return new List<string>();
            }
        }
        #endregion



        #region Parameters - List<ModelGridParametre>
        static List<ModelGridParametre> ParseParameters(string xmlString)
        {
            List<ModelGridParametre> parametersList = new List<ModelGridParametre>();

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);

                // Sélectionne tous les éléments "Parameter" dans la balise "Parameters"
                XmlNodeList parameterNodes = xmlDoc.SelectNodes("//Parameters/Parameter");

                foreach (XmlNode node in parameterNodes)
                {
                    string paramName = node.Attributes["Name"].Value;
                    string paramType = node.Attributes["Type"]?.Value;
                    string paramValue = node.Attributes["Value"]?.Value;

                    ModelGridParametre parameter = CreateModelGridParametre(paramName, paramType, paramValue);
                    parametersList.Add(parameter);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite : " + ex.Message);
            }

            return parametersList;
        }

        static ModelGridParametre CreateModelGridParametre(string paramName, string paramType, string paramValue)
        {
            ModelGridParametre parameter = new ModelGridParametre(paramName, paramType);

            switch (paramType)
            {
                case "System.DateTime":
                    if (DateTime.TryParse(paramValue, out DateTime dateValue))
                    {
                        parameter.DateValue = dateValue;
                        parameter.IsDate = true;
                    }
                    break;

                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                    if (int.TryParse(paramValue, out int intValue))
                    {
                        parameter.IntValue = intValue;
                        parameter.IsInt = true;
                    }
                    break;

                case "System.Boolean":
                    if (bool.TryParse(paramValue, out bool boolValue))
                    {
                        parameter.BoolValue = boolValue;
                        parameter.IsBool = true;
                    }
                    else
                    {
                        parameter.BoolValue = false;
                        parameter.IsBool = true;
                    }
                    break;

                case "System.Decimal":
                    if (decimal.TryParse(paramValue, out decimal decimalValue))
                    {
                        parameter.DecimalValue = decimalValue;
                        parameter.IsDecimal = true;
                    }
                    break;

                

                default:
                    parameter.StringValue = paramValue;
                    parameter.IsString = true;
                    break;
            }

            return parameter;
        }

        /*static string GetValueAsString(ModelGridParametre parameter)
        {
            if (parameter.DateValue.HasValue)
            {
                return parameter.DateValue.ToString();
            }
            else if (parameter.IntValue.HasValue)
            {
                return parameter.IntValue.ToString();
            }
            else if (parameter.BoolValue.HasValue)
            {
                return parameter.BoolValue.ToString();
            }
            else if (parameter.DecimalValue.HasValue)
            {
                return parameter.DecimalValue.ToString();
            }
            else if (!string.IsNullOrEmpty(parameter.StringValue))
            {
                return parameter.StringValue;
            }
            else
            {
                return "Valeur non définie";
            }
        }*/
        #endregion


        #region cherche les parametre de finale de parametrage
        static List<ModelGridParametre> FilterModelGridParametres(List<ModelGridParametre> inputList, List<string> allowedNames)
        {
            return inputList.Where(parametre => allowedNames.Contains(parametre.Name)).ToList();
        }

        #endregion


        #region changer la requette avec les parametre
        static string ReplaceParameters(string queryString, List<ModelGridParametre> parameters)
        {
            foreach (ModelGridParametre parameter in parameters)
            {
                string placeholder = " @" + parameter.Name+" ";

                if (parameter.IsString is true)
                {
                    queryString = queryString.Replace(placeholder, $" '{parameter.StringValue}' ");
                }
                else if (parameter.IsDate is true)
                {
                    queryString = queryString.Replace(placeholder, $" '{parameter.DateValue.ToString("yyyy-M-d HH:mm:ss")}' ");
                }
                else if (parameter.IsInt is true)
                {
                    queryString = queryString.Replace(placeholder, parameter.IntValue.ToString());
                }
                else if (parameter.IsDecimal is true)
                {
                    queryString = queryString.Replace(placeholder, parameter.DecimalValue.ToString());
                }
                else if (parameter.IsBool is true)
                {
                    queryString = queryString.Replace(placeholder, parameter.BoolValue.ToString());
                }
                // Add additional conditions for other data types (e.g., IsBool)

            }

            return queryString;
        }

        public static string AddSpacesToParameters(string input,int nouveau_nom)
        {
           
            // Utiliser une expression régulière pour trouver les mots commençant par "@"
            Regex regex = new Regex(@"@(\w+)");
            string result = regex.Replace(input, " $& ");

            int index = result.IndexOf("@User", StringComparison.OrdinalIgnoreCase);

            // Vérification si "@User" a été trouvé
            if (index != -1)
            {
                // Remplacement de "@User" par le nouveau nom
                result = result.Substring(0, index) + nouveau_nom + result.Substring(index + "@User".Length);
            }

            return result;


            //return result;
        }
        #endregion 


        #region Requette => Parametres
        static List<string> GetParameterNames(string sqlQuery)
        {
            List<string> parameterNames = new List<string>();

            try
            {
                // Utilise une expression régulière pour trouver tous les noms de paramètres débutant par "@"
                Regex regex = new Regex(@"@\w+");
                MatchCollection matches = regex.Matches(sqlQuery);

                // Ajoute les noms de paramètres à la liste sans doublons et sans le caractère "@"
                foreach (Match match in matches)
                {
                    string paramName = match.Value.Substring(1); // Retire le "@" au début
                    if (!parameterNames.Contains(paramName))
                    {
                        parameterNames.Add(paramName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite : " + ex.Message);
            }

            return parameterNames;
        }
        #endregion

    }

    public class GridColumnAttributeSymmary
    {
        public string DataMember { get; set; }
        public string summaryType { get; set; }
    }

    public class GridColumnSummary1
    {
        public string Attribute { get; set; }
        public string SummaryType { get; set; }

        public GridColumnSummary1(string attribute, string summaryType)
        {
            Attribute = attribute;
            SummaryType = summaryType;
        }

        public override string ToString()
        {
            return $"GridColumnSummary(\"{Attribute}\", \"{SummaryType}\")";
        }
    }

    public class GridColumn
    {
        public string DataMember { get; set; }
        public string DefaultId { get; set; }

        public GridColumn(string dataMember, string defaultId)
        {
            DataMember = dataMember;
            DefaultId = defaultId;
        }
    }



}
