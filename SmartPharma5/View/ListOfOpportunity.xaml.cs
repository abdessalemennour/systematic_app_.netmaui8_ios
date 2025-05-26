using DevExpress.Maui.Editors;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class ListOfOpportunity : ContentPage
{
    bool TestLoad { get; set; } = true;
    public ListOfOpportunity()
	{
        InitializeComponent();

        var OVM = BindingContext as OpportunityListViewModel;

        BindingContext = new OpportunityListViewModel();

        try
        {
            OVM.RefreshOnApp().GetAwaiter();
            Task.Run(() => loadConnectionState());
        }
        catch
        {

        }
    }

    
   
    public ListOfOpportunity(uint idAgent)
    {
        InitializeComponent();

        BindingContext = new OpportunityListViewModel(idAgent);

        var OVM = BindingContext as OpportunityListViewModel;
        try
        {
            OVM.RefreshOnApp().GetAwaiter();
            Task.Run(() => loadConnectionState());
        }
        catch
        {

        }


    }

    private void AutoCompleteEdit_TextChanged(object sender, AutoCompleteEditTextChangedEventArgs e)
    {
        AutoCompleteEdit edit = sender as AutoCompleteEdit;
        var search = edit.Text.ToLowerInvariant().ToString();
        var OVM = BindingContext as OpportunityListViewModel;


        if (string.IsNullOrWhiteSpace(search))
        {
            OpportunitytCollectionView.ItemsSource = OVM.OpportunityList.ToList();
        }
        else
        {
            OpportunitytCollectionView.ItemsSource = OVM.OpportunityList.Where(i => (i.code.ToLowerInvariant().Contains(search)) || (i.agentName.ToLowerInvariant().Contains(search)) || (i.parentCode.ToLowerInvariant().Contains(search)) || (i.wholesalerName.ToLowerInvariant().Contains(search)) || (i.partnaireName.ToLowerInvariant().Contains(search)) || (i.stateName.ToLowerInvariant().Contains(search)) || (i.create_date.ToString().ToLowerInvariant().Contains(search)) || (i.delivredState.ToString().ToLowerInvariant().Contains(search))).ToList();
        }

    }

    protected override async void OnAppearing()
    {

        base.OnAppearing();














    }
    public async Task loadConnectionState()
    {
        var OVM = BindingContext as OpportunityListViewModel;
        /* while (OpportunityListViewModel.TestLoad)
         {
             await  OVM.ChangeConnexionState();

         }*/

    }
    protected override void OnDisappearing()
    {
        this.TestLoad = false;
        base.OnDisappearing();


    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        //await Shell.Current.GoToAsync(nameof(OpportunityView));

        try
        {
            if (sender is Frame frame && frame.BindingContext is SmartPharma5.Model.Opportunity.Collection collection)
            {
                var OVM = BindingContext as OpportunityListViewModel;
                OVM.TestLoad = false;
                OVM.Loading = true;
                await Task.Delay(500);
                if (collection != null)
                {
                    if (await DbConnection.Connecter3())
                    {


                        OVM.Opportunity = new Opportunity(collection);
                        // Opportunity = new Opportunity();
                        CurrentData.CurrentNoteModule = "CRM.Opportunity";
                        CurrentData.CurrentActivityModule = "CRM.Opportunity, CRM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                        CurrentData.CurrentFormModule = "AtooERP_Standard.CRM_Opportunity.Opportunity_update, AtooERP_Standard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

                        CurrentData.CurrentModuleId = collection.Id; 

                        // await Opportunity.GetOpportunityLine((int)o.Id); ;

                        await App.Current.MainPage.Navigation.PushAsync(new OpportunityView(OVM.Opportunity));

                    }
                    else
                    {
                        OVM.TestConnection = true;
                    }
                }
                OVM.Loading = false;

            }

        }
        catch (Exception ex)
        {

        }

    }
}