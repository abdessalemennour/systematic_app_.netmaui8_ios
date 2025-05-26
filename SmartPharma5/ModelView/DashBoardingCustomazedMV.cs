using MvvmHelpers;
using MySqlConnector;
using SmartPharma5.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace SmartPharma5.ModelView
{
    public class DashBoardingCustomazedMV : BaseViewModel
    {
        private string xml_file;
        public string XML_FILE { get => xml_file; set => SetProperty(ref xml_file, value); }

        private string template_Requette;
        public string Template_Requette { get => template_Requette; set => SetProperty(ref template_Requette, value); }

        private string requette;
        public string Requette { get => requette; set => SetProperty(ref requette, value); }

        private List<ModelGridParametre> list_Paramettre_final;
        public List<ModelGridParametre> List_Paramettre_final { get => list_Paramettre_final; set => SetProperty(ref list_Paramettre_final, value); }

        public DashBoardingCustomazedMV(int id)
        {
            this.XML_FILE = GetXMLFILE(id);
           
            List<QueryModel> querrys = GetQueryContents(this.xml_file);
            Requette = ExtractSqlFromXml(querrys[0].Contenu).Replace("&lt;", "<").Replace("&gt;", ">");

            


            List<string> Params = ExtractParameters(XML_FILE);
            List<ModelGridParametre> ParametresList = new List<ModelGridParametre>();
            try
            {
                ParametresList= ParseParameters(Params[0]);

            }
            catch(Exception ex)
            {

            }
            

            string querry2 = AddSpacesToParameters(Requette);
            this.Template_Requette = querry2;
            string querry1 = ReplaceParameters(querry2, ParametresList);
            Requette = querry1;

            List<string> Paramettre_requette = GetParameterNames(querry2);
            List_Paramettre_final = FilterModelGridParametres(ParametresList, Paramettre_requette);

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
        static List<ModelGridParametre> FilterModelGridParametres(List<ModelGridParametre> inputList, List<string> allowedNames)
        {
            return inputList.Where(parametre => allowedNames.Contains(parametre.Name)).ToList();
        }

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
       public static string ReplaceParameters(string queryString, List<ModelGridParametre> parameters)
        {
            foreach (ModelGridParametre parameter in parameters)
            {
                string placeholder = " @" + parameter.Name + " ";

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

        static string AddSpacesToParameters(string input)
        {
            // Utiliser une expression régulière pour trouver les mots commençant par "@"
            Regex regex = new Regex(@"@(\w+)");
            string result = regex.Replace(input, " $& ");

            return result;
        }
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
        public static string GetXMLFILE(int id)
        {
            string Result = "";
            try
            {
                string sqlCmd1 = "SELECT * FROM atooerp_app_dashboard where Id=" + id + "";

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

    }
}
