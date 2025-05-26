using MvvmHelpers;
using MvvmHelpers.Commands;
using MySqlConnector;
using SmartPharma5.Model;
using SmartPharma5.View;
using System.ComponentModel;

namespace SmartPharma5.ViewModel
{
    public class TabMV : BaseViewModel
    {
        private string number_Info_change;
        public string Number_Info_change { get => number_Info_change; set => SetProperty(ref number_Info_change, value); }

        private int number_profile_change;
        public int Number_profile_change { get => number_profile_change; set => SetProperty(ref number_profile_change, value); }

        private int number_partner_add;
        public int Number_partner_add { get => number_partner_add; set => SetProperty(ref number_partner_add, value); }

        public string num = "test";
        public AsyncCommand HomePage { get; }
        public TabMV()
        {
            Number_Info_change = "8";
            HomePage = new AsyncCommand(homePage);
            LoadNumbers().Wait();
        }

        private async Task homePage()
        {

            try
            {

                await App.Current.MainPage.Navigation.PushAsync(new HomeView());
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }

        }

        public async Task LoadNumbers()
        {

            string sqlCmd = "SELECT  \"1\" as id, count(marketing_profile_attribut_value_temp.id) as count1\r\nfrom marketing_profile_attribut_value_temp\r\nleft join atooerp_user on   atooerp_user.Id = marketing_profile_attribut_value_temp.user\r\nleft join  marketing_profile_attribut_value on marketing_profile_attribut_value_temp.attribut_value =marketing_profile_attribut_value.Id\r\nleft join marketing_profile_attribut on marketing_profile_attribut_value.attribut= marketing_profile_attribut.Id\r\nleft join marketing_profile_instances on marketing_profile_attribut_value.profile_instance=marketing_profile_instances.Id\r\nleft join marketing_profile on marketing_profile_instances.profil = marketing_profile.Id\r\nleft join commercial_partner on marketing_profile_instances.partner=commercial_partner.Id\r\nleft join commercial_partner as com2 on marketing_profile_attribut_value_temp.partner=com2.Id\r\nleft join commercial_partner_category as current_category on com2.category=current_category.id\r\nleft join atooerp_type_element as element1 on marketing_profile_attribut_value_temp.type = element1.id\r\nleft join atooerp_type_element as element2 on marketing_profile_attribut_value.type = element2.id\r\nleft join atooerp_person as person on person.id=33\r\n\r\nleft join commercial_partner_category as temp_category on marketing_profile_attribut_value_temp.int_value=temp_category.id\r\nwhere marketing_profile_attribut_value_temp.profile_instance_temp is null and  marketing_profile_attribut_value_temp.partner_temp is null\r\nand marketing_profile_attribut_value_temp.state = 1\r\nand (marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance  group by instance.partner) or marketing_profile_attribut_value_temp.attribut_name is not null)\r\nunion\r\nselect \"2\" as id ,count(m.id) as count1\r\nFROM marketing_profile_instance_temp m\r\nleft join commercial_partner partner on partner.id = m.partner\r\nleft join marketing_profile on marketing_profile.id=m.profile\r\nleft join marketing_profile_instances on marketing_profile_instances.partner=m.partner and marketing_profile_instances.Id in (select max(instance.Id)from marketing_profile_instances instance group by instance.partner)\r\nleft join marketing_profile profile on profile.id=marketing_profile_instances.profil\r\nleft join hr_employe employe on employe.user=m.user\r\nleft join atooerp_person on atooerp_person.id=employe.id\r\nwhere m.partner_temp is null and m.instance is null and m.state=1\r\n\r\n\r\nunion\r\nSELECT \"3\" as id , count(c.id) as count1\r\nFROM commercial_partner_temp c\r\nleft join atooerp_person on  atooerp_person.id = c.employe\r\nleft join  marketing_profile_attribut_value_temp on marketing_profile_attribut_value_temp.partner_temp=c.id and marketing_profile_attribut_value_temp.attribut_name=\"name\"\r\nleft join marketing_profile_instance_temp on marketing_profile_instance_temp.partner_temp=c.id\r\nWHERE c.partner is null and c.state=1\r\n\r\n;";
            BindingList<Partner> list = new BindingList<Partner>();



            if (await DbConnection.Connecter3())
            {

                MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    try
                    {
                        if (reader["id"].ToString() == "1")
                        {
                            Number_Info_change = Convert.ToString(reader["count1"]);


                        }
                        else if (reader["id"].ToString() == "2")
                        {
                            Number_profile_change = Convert.ToInt32(reader["count1"]);
                        }
                        else if (reader["id"].ToString() == "3")
                        {
                            Number_partner_add = Convert.ToInt32(reader["count1"]);
                        }






                    }
                    catch (Exception ex)
                    {
                        reader.Close();


                    }

                }

                reader.Close();
                DbConnection.Deconnecter();
            }
            else
            {

            }



        }
    }
}
