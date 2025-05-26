using Acr.UserDialogs;
using CommunityToolkit.Maui.Views;
using SmartPharma5.Model;
using SmartPharma5.ModelView;

namespace SmartPharma5.View;

public partial class SaleQuotationView : ContentPage
{
	public SaleQuotationView(Opportunity opp)
	{
        BindingContext = new SaleQuotationMV(opp);
        InitializeComponent();
	}

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        UserDialogs.Instance.Toast("Sales Quotation Lines ...");
        await Task.Delay(200);
        var ovm = BindingContext as SaleQuotationMV;

        if (await DbConnection.Connecter3())
        {
            try
            {
                try
                {
                    Popup.IsOpen = true;
                    
                    await Task.Delay(300);
                    if (sender is Frame frame && frame.BindingContext is SaleQuotation tappedItem)
                    {
                        if (await DbConnection.Connecter3())
                        {
                            try
                            {
                                ovm.SelectedListQuotation = SaleQuotation.getSaleQuoatationLineById(tappedItem.Id).Result;
                                ovm.IsSelected = true;
                            }
                            catch (Exception ex)
                            { }


                        }
                        else
                        {
                            
                        }
                    }
                   
                }
                catch (Exception ex)
                {
                    Popup.IsOpen = false;

                }
            }



            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
                UserDialogs.Instance.HideLoading();
            }

        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Warning", "Connection Failed", "OK");

        }



        

    }

    private void SimpleButton_Clicked(object sender, EventArgs e)
    {
        Popup.IsOpen = false;

    }

  
}