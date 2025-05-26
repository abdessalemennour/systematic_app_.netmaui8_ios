using Microsoft.Maui.Controls;
using SmartPharma5.Model;
using SmartPharma5.ModelView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SmartPharma5.View.FloatingActionButton
{
    public partial class LogsView : ContentPage
    {
        private readonly LogsViewModel _viewModel;

        public LogsView()
        {
            InitializeComponent();
            _viewModel = (LogsViewModel)BindingContext;
            LoadData();
        }

        private async void LoadData()
        {
            await _viewModel.LoadLogsAsync(CurrentData.CurrentModuleId, CurrentData.CurrentNoteModule);
        }
        private async void OnGpsClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is LogModel log && !string.IsNullOrWhiteSpace(log.GPS))
            {
                try
                {
                    var coords = log.GPS.Split(',');
                    if (coords.Length == 2 &&
                        double.TryParse(coords[0], CultureInfo.InvariantCulture, out double lat) &&
                        double.TryParse(coords[1], CultureInfo.InvariantCulture, out double lng))
                    {
                        string uri = $"https://www.google.com/maps/search/?api=1&query={lat.ToString("F6", CultureInfo.InvariantCulture)},{lng.ToString("F6", CultureInfo.InvariantCulture)}";
                        await Launcher.Default.OpenAsync(new Uri(uri));
                    }
                    else
                    {
                        await DisplayAlert("Erreur", "Coordonnées GPS invalides.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erreur", "Impossible d'ouvrir Google Maps : " + ex.Message, "OK");
                }
            }
        }


    }
}
