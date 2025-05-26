using SmartPharma5.Model;
using System.Collections.ObjectModel;
using SmartPharma5.ViewModel;
using SmartPharma5.Models;

namespace SmartPharma5.View;

public partial class ManageConnectionsView : ContentPage
{
    public ObservableCollection<ServerSettings> Connections { get; set; } = new();

    public ManageConnectionsView()
	{
		InitializeComponent();
        LoadConnections();
        BindingContext = new HomeViewModel();
    }

    private async void LoadConnections()
    {
        var configService = new JsonConfigService();
        var history = await configService.LoadHistoryAsync();

        if (history?.Connections != null)
        {
            var sorted = history.Connections.OrderByDescending(c => c.ConnectionDate).ToList();
            foreach (var conn in sorted)
                Connections.Add(conn);

            ConnectionList.ItemsSource = Connections;
        }
        else
        {
            await DisplayAlert("Info", "Aucune connexion enregistrée.", "OK");
        }
    }




    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var button = (ImageButton)sender;
        var connection = (ServerSettings)button.BindingContext;

        if (BindingContext is HomeViewModel viewModel)
        {
            await viewModel.DeleteConnectionAsync(connection);
        }
    }
    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        var button = (ImageButton)sender;
        var connection = (ServerSettings)button.BindingContext;

        // Toggle entre édition et affichage normal
        connection.IsEditing = !connection.IsEditing;

        if (!connection.IsEditing)
        {
            //var configService = new JsonConfigService();
            //await configService.UpdateConnectionName(connection.ConnectionId.ToString(), connection.CustomName);
        }
    }
    private async void ValidateButton_Clicked(object sender, EventArgs e)
    {
        var button = (ImageButton)sender;
        var connection = (ServerSettings)button.BindingContext;
        var configService = new JsonConfigService();
        await configService.UpdateConnectionName(connection.ConnectionId.ToString(), connection.CustomName);
    }
    private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        // Récupérer le Frame tapé
        var frame = sender as Frame;
        if (frame == null) return;

        // Récupérer la connexion liée à ce Frame
        var selectedConnection = frame.BindingContext as ServerSettings;
        if (selectedConnection == null) return;

        // ? Ne rien faire si c'est la connexion courante
        if (!selectedConnection.IsNotCurrentConnection)
            return;

        // Récupérer le ViewModel lié à la page
        var vm = this.BindingContext as HomeViewModel;
        if (vm == null) return;

        // Vérifie que la commande peut être exécutée
        if (vm.SelectConnectionCommand.CanExecute(selectedConnection))
        {
            vm.SelectConnectionCommand.Execute(selectedConnection);
        }
    }


}