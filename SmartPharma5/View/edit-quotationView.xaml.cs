using Acr.UserDialogs;
using DevExpress.Data.XtraReports.Native;
using Microsoft.Maui.Controls.Compatibility;
using MvvmHelpers;
using SmartPharma5.Model;
using SmartPharma5.ModelView;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SmartPharma5.View;

public partial class edit_quotationView : ContentPage
{
    public int id { get; set; }
    public int partner { get; set; }
    public edit_quotationView(int id)
	{
        this.id = id;
		InitializeComponent();
		BindingContext = new editQuotationMV(id);
	}
    public edit_quotationView(int id,List<SaleQuotationLine> lines)
    {
        this.id = id;
        InitializeComponent();
        BindingContext = new editQuotationMV(id,lines);
    }
    
     public edit_quotationView(SaleQuotation quotation, MvvmHelpers.ObservableRangeCollection<Product> productList, List<SaleQuotationLine> salesQuotationLines, List<int> ListDeleted, List<int> ListUpdated, List<SaleQuotationLine> ListNew)
    {
        this.id = id;
        InitializeComponent();
        BindingContext = new editQuotationMV( quotation, productList, salesQuotationLines, ListDeleted, ListUpdated, ListNew);
    }
    public edit_quotationView(int id, int partner)
    {
        this.id = id;
        this.partner = partner;
        InitializeComponent();
        //BindingContext = new editQuotationMV(id);
         BindingContext = new editQuotationMV(id, partner);
    }

    public edit_quotationView()
    {
        this.id = id;
        this.partner = partner;
        InitializeComponent();
        //BindingContext = new editQuotationMV(id);
        BindingContext = new editQuotationMV(id, partner);
    }






    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        var ovm = BindingContext as editQuotationMV;
        if (sender is ImageButton button)
        {
            // Navigate up the visual tree to find the parent Frame
            var frame = FindParent<Frame>(button);

            if (frame != null)
            {
                if(frame.BindingContext is SaleQuotationLine line)
                {
                    ovm.ListIdDeleted.Add(line.Id);
                    ovm.SelectedListQuotation.Remove(line);

                }

            }

        }
       
        
        
        //var ProductList = new ObservableRangeCollection<Product>(await C);
        UserDialogs.Instance.ShowLoading("DELETE LINE ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new edit_quotationView(this.id, ovm.SelectedListQuotation));
        UserDialogs.Instance.HideLoading();
    }

    private T FindParent<T>(Element child) where T : Element
    {
        Element parent = child.Parent;
        while (parent != null && !(parent is T))
        {
            parent = parent.Parent;
        }
        return parent as T;
    }

    private void AddButton_Clicked(object sender, EventArgs e)
    {
        var ovm = BindingContext as editQuotationMV;
        ovm.IsUpdated = false;
        ovm.Savebtn = true;
        foreach ( var item in ovm.SelectedListQuotation)
        {
            item.EnabledChange = true;

        }


    }

    private async void AddButton_Clicked_1(object sender, EventArgs e)
    {
        var C = Task.Run(() => Product.
             GetProduct(this.partner));
        var ProductList = new MvvmHelpers.ObservableRangeCollection<Product>(await C);
        var ovm = BindingContext as editQuotationMV;
        SaleQuotationLine line = new SaleQuotationLine();
        line.NameProduct = "NEW PRODUCT";
        ovm.SelectedListQuotation.Add(line);
        UserDialogs.Instance.ShowLoading("ADD NEW LINE ...");
        await Task.Delay(500);
        await App.Current.MainPage.Navigation.PushAsync(new ProductListView(ovm.Quotation,ProductList,ovm.SelectedListQuotation,ovm.ListIdDeleted,ovm.ListIdUpdated,ovm.ListLineAdded));
        UserDialogs.Instance.HideLoading();
      



    }

    private void ImageButton_Clicked_1(object sender, EventArgs e)
    {
        // Cast sender to ImageButton
        if (sender is ImageButton button)
        {
            // Find the parent Frame
            var parentFrame = FindParent<Frame>(button);

            // Check if found and remove it from its parent
            if (parentFrame != null)
            {
                var parent = parentFrame.Parent as Microsoft.Maui.Controls.Grid;
                if (parent != null)
                {
                    parent.Children.Remove(parentFrame);
                }
            }
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        var ovm = BindingContext as editQuotationMV;
        ovm.IsUpdated = true;
        ovm.Savebtn = false;


        foreach (var item in ovm.SelectedListQuotation)
        {
            item.EnabledChange = false;

        }
    }
}