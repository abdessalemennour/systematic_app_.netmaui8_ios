using MvvmHelpers;
using MySqlConnector;

namespace SmartPharma5.Model
{
    public class PartnerTempAttributesModel : BaseViewModel
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public int Id_Attribute { get; set; }

        public bool IsProflieAttribute { get; set; }


        //----------------------CHECH UPDATE -----------------------------------------

        private bool valid_attribute;
        public bool Valid_attribute { get => valid_attribute; set => SetProperty(ref valid_attribute, value); }


        /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
        Avant :
                //-----------------------VALUES ----------------------------------------------




                public string String_value { get; set; }
        Après :
                //-----------------------VALUES ----------------------------------------------




                public string String_value { get; set; }
        */
        //-----------------------VALUES ----------------------------------------------




        public string String_value { get; set; }
        public int? Int_value { get; set; } = null;

        public decimal? Decimal_value { get; set; } = null;
        public bool? Boolean_value { get; set; } = null;
        public DateTime? Date_value { get; set; } = null;

        public int? Type_value { get; set; } = null;
        public string String_Type_value { get; set; }

        public bool is_checked { get; set; }


        //---------------------------Check Values-----------------------------------------

        public bool HasString { get; set; }
        public bool HasInt { get; set; }
        public bool HasDecimal { get; set; }
        public bool HasBool { get; set; }
        public bool HasDate { get; set; }
        public bool HasType { get; set; }

        public PartnerTempAttributesModel()
        {
            Valid_attribute = true;

        }
        public static async Task<List<PartnerTempAttributesModel>> GetAllAttributesProfile(int id)
        {

            List<PartnerTempAttributesModel> list = new List<PartnerTempAttributesModel>();
            string sqlCmd = "select marketing_profile_attribut.is_null,marketing_profile_attribut_value_temp.* , marketing_profile_attribut.label , atooerp_type_element.name as type_name , commercial_partner_category.name as category_name from marketing_profile_attribut_value_temp\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=" + id + " left join commercial_partner_category on (commercial_partner_category.id=marketing_profile_attribut_value_temp.int_value and marketing_profile_attribut_value_temp.attribut_name=\"category\")\r\nleft join marketing_profile_attribut on marketing_profile_attribut.id = marketing_profile_attribut_value_temp.profile_attribute\r\nleft join atooerp_type_element on atooerp_type_element.id = marketing_profile_attribut_value_temp.type\r\nwhere (marketing_profile_attribut_value_temp.partner_temp=" + id + " or marketing_profile_attribut_value_temp.profile_instance_temp=marketing_profile_instance_temp.id);";
            if (await DbConnection.Connecter3())
            {


                MySqlDataReader reader;
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        PartnerTempAttributesModel attribute = new PartnerTempAttributesModel();
                        var p = reader["string_value"];
                        if (reader["partner_temp"].ToString() != "")
                        {
                            attribute.IsProflieAttribute = false;
                            if (reader["string_value"] != null && reader["int_value"].ToString() == "" && reader["boolean_value"].ToString() == "")
                            {
                                attribute.String_value = reader["string_value"].ToString();
                                attribute.HasString = true;
                            }
                            else if (reader["int_value"].ToString() != "")
                            {
                                attribute.Type_value = Convert.ToInt32(reader["int_value"]);
                                attribute.String_Type_value = reader["category_name"].ToString();
                                attribute.HasType = true;

                            }
                            else if (reader["boolean_value"].ToString() != "")
                            {
                                attribute.Boolean_value = Convert.ToBoolean(reader["boolean_value"]);
                                attribute.HasBool = true;
                            }


                            else
                            {

                            }

                            attribute.Label = reader["attribut_name"].ToString();
                        }
                        else
                        {
                            attribute.IsProflieAttribute = true;
                            if (reader["string_value"] != null && reader["int_value"].ToString() == "" && reader["decimal_value"].ToString() == "" && reader["boolean_value"].ToString() == "" && reader["date_value"].ToString() == "" && reader["type"].ToString() == "")
                            {
                                attribute.String_value = reader["string_value"].ToString();
                                attribute.HasString = true;
                            }
                            else if (reader["int_value"].ToString() != "")
                            {
                                attribute.Int_value = Convert.ToInt32(reader["int_value"]);
                                attribute.HasInt = true;

                            }
                            else if (reader["decimal_value"].ToString() != "")
                            {
                                attribute.Decimal_value = Convert.ToDecimal(reader["decimal_value"]);
                                attribute.HasDecimal = true;
                            }
                            else if (reader["boolean_value"].ToString() != "")
                            {
                                attribute.Boolean_value = Convert.ToBoolean(reader["boolean_value"]);
                                attribute.HasBool = true;
                            }
                            else if (reader["date_value"].ToString() != "")
                            {
                                attribute.Date_value = Convert.ToDateTime(reader["date_value"]);
                                attribute.HasDate = true;
                            }
                            else if (reader["type"].ToString() != "")
                            {
                                attribute.Type_value = Convert.ToInt32(reader["type"]);
                                attribute.String_Type_value = reader["type_name"].ToString();
                                attribute.HasType = true;
                            }
                            else
                            {

                            }
                            if (Convert.ToBoolean(reader["is_null"]) == false)
                            {
                                attribute.Label = reader["label"].ToString() + "*";
                            }
                            else
                            {
                                attribute.Label = reader["label"].ToString();
                            }

                            attribute.Id_Attribute = Convert.ToInt32(reader["profile_attribute"]);

                        }

                        list.Add(attribute);


                    }
                    reader.Close();
                    return list;


                }
                catch (Exception ex)
                {
                    return null;

                }
            }
            else
            {
                return null;
            }

        }


    }
}
