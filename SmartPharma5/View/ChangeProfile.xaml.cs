using DevExpress.Maui.Editors;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class ChangeProfile : ContentPage
{
    public bool isUserInteraction = false;
    public ChangeProfile(int id, string name, int id_instance_profile, int id_partner)
    {
        InitializeComponent();
        BindingContext = new ChangeProfileMV(id, id_instance_profile, id_partner);
        //Title_page.Text = name + " attributes";


    }
    private void EventToCommandBehavior_BindingContextChanged(object sender, EventArgs e)
    {

    }

    private void comboBox_TouchUp(object sender, EventArgs e)
    {
        isUserInteraction = true;
    }

    private void picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (isUserInteraction)
            {
                try
                {
                    ComboBoxEdit picker = (ComboBoxEdit)sender;
                    int selectedIndex = picker.SelectedIndex;
                    bool find_fils = false;
                    atooerp_element test10 = picker.SelectedItem as atooerp_element;
                    //atooerp_element item = picker.ItemsSource as atooerp_element;
                    var ovm = BindingContext as ChangeProfileMV;

                    foreach (ProfileAttributes attribute in ovm.ListAttributes)
                    {

                        if (attribute.HasMultiple)
                        {

                            var test1 = attribute.HasMultiple;
                            var test2 = attribute.type_parent_multi_value;
                            var test3 = test10.id_type;

                            if (attribute.type_parent_multi_value == test10.id_type && attribute.Rank > test10.Rank)
                            {

                                find_fils = true;
                                try
                                {
                                    attribute.Selected_item = null;
                                    attribute.Multiple_value = 0;
                                    attribute.List_item = attribute.List_item_fixe;
                                    attribute.List_item = attribute.List_item.Where(p => p.parent == test10.id || p.parent == null).ToList();

                                }
                                catch (Exception ex)
                                {

                                }


                            }

                            if (find_fils == true && attribute.type_parent_multi_value != test10.id_type)
                            {
                                return;
                            }

                        }
                    }


                }
                catch (Exception ex)
                {

                }

                isUserInteraction = false;
            }

        }
        catch (Exception ex)
        {

        }


    }





    private void ComboBoxEdit_Tap(object sender, System.ComponentModel.HandledEventArgs e)
    {
        isUserInteraction = true;

    }

    private void ComboBoxEdit_BindingContextChanged(object sender, EventArgs e)
    {
        try
        {
            // Votre code susceptible de générer une erreur ici
        }
        catch (Exception ex)
        {
            // Gérez l'erreur ici, par exemple, affichez un message d'erreur ou effectuez d'autres actions nécessaires
            Console.WriteLine("Erreur dans le code XAML : " + ex.Message);
        }

    }
}