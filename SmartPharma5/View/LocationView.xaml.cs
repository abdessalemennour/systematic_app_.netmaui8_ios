using System.Globalization;
using Mapsui.UI.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using System.Diagnostics;


namespace SmartPharma5.View
{
    public partial class LocationView : ContentPage
    {
        private double latitude1;
        private double longitude1;
        private double latitude2;
        private double longitude2;
        // private Mapsui.UI.Maui.Pin _currentPin;
        private Mapsui.UI.Maui.Pin _defaultPin;

        public LocationView(string gps1, string gps2)
        {
            InitializeComponent();
            DisplayCoordinatesFromGps(gps1, gps2);
            GetLocationAndUpdateMap();
        }

        public LocationView()
        {
            InitializeComponent();
            GetLocationAndUpdateMap();
            // AdjustMapHeight();

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            GetLocationAndUpdateMap(); // Appeler la méthode après que la vue soit apparue
        }

        private void DisplayCoordinatesFromGps(string gps1, string gps2)
        {
            try
            {
                if (!string.IsNullOrEmpty(gps1) && gps1.Contains(","))
                {
                    var coords1 = gps1.Split(',');
                    latitude1 = Convert.ToDouble(coords1[0].Trim(), CultureInfo.InvariantCulture);
                    longitude1 = Convert.ToDouble(coords1[1].Trim(), CultureInfo.InvariantCulture);

                    LatitudeLabel1.Text = $"Latitude 1: {latitude1}";
                    LongitudeLabel1.Text = $"Longitude 1: {longitude1}";
                }
                else
                {
                    LatitudeLabel1.Text = "Latitude 1: Non valide";
                    LongitudeLabel1.Text = "Longitude 1: Non valide";
                }

                if (!string.IsNullOrEmpty(gps2) && gps2.Contains(","))
                {
                    var coords2 = gps2.Split(',');
                    latitude2 = Convert.ToDouble(coords2[0].Trim(), CultureInfo.InvariantCulture);
                    longitude2 = Convert.ToDouble(coords2[1].Trim(), CultureInfo.InvariantCulture);

                    LatitudeLabel2.Text = $"Latitude 2: {latitude2}";
                    LongitudeLabel2.Text = $"Longitude 2: {longitude2}";

                    // Calculer la distance et l'afficher
                    double distance = CalculateDistance(latitude1, longitude1, latitude2, longitude2);
                    DistanceLabel.Text = $"Distance: {distance:F2} km";

                }
                else
                {
                    LatitudeLabel2.Text = "Latitude 2: Non valide";
                    LongitudeLabel2.Text = "Longitude 2: Non valide";
                }
            }
            catch (Exception ex)
            {
                LatitudeLabel1.Text = "Erreur lors de l'affichage";
                LongitudeLabel1.Text = ex.Message;
                LatitudeLabel2.Text = "Erreur lors de l'affichage";
                LongitudeLabel2.Text = ex.Message;
            }
        }

        private async void GetLocationAndUpdateMap()
        {
            try
            {
                var map = mapView.Map;

                // Ajouter la couche de tuiles OpenStreetMap
                map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

                // Supprimer la couche MyLocationLayer (pour éviter le marqueur bleu)
                if (mapView.MyLocationLayer != null && map.Layers.Contains(mapView.MyLocationLayer))
                {
                    map.Layers.Remove(mapView.MyLocationLayer);
                }
                // Effacer les marqueurs existants
                mapView.Pins.Clear();

                // latitude1, longitude1
                if (latitude1 != 0.0 && longitude1 != 0.0)
                {
                    var pin1 = new Mapsui.UI.Maui.Pin
                    {
                        Position = new Mapsui.UI.Maui.Position(latitude1, longitude1),
                        Label = "Localisation 1",
                        Address = $"Lat: {latitude1}, Lon: {longitude1}",
                        Color = Colors.Green
                    };
                    mapView.Pins.Add(pin1);
                }

                // latitude2, longitude2 (si différent de 0.0)
                if (latitude2 != 0.0 && longitude2 != 0.0)
                {
                    var pin2 = new Mapsui.UI.Maui.Pin
                    {
                        Position = new Mapsui.UI.Maui.Position(latitude2, longitude2),
                        Label = "Localisation 2",
                        Address = $"Lat: {latitude2}, Lon: {longitude2}",
                        Color = Colors.Red
                    };
                    mapView.Pins.Add(pin2);
                }

                // Zoom automatique sur les deux points
                if (latitude1 != 0.0 && longitude1 != 0.0 && latitude2 != 0.0 && longitude2 != 0.0)
                {
                    var mercatorLocation1 = Mapsui.Projections.SphericalMercator.FromLonLat(longitude1, latitude1);
                    var mercatorLocation2 = Mapsui.Projections.SphericalMercator.FromLonLat(longitude2, latitude2);
                    double minX = Math.Min(mercatorLocation1.Item1, mercatorLocation2.Item1);
                    double maxX = Math.Max(mercatorLocation1.Item1, mercatorLocation2.Item1);
                    double minY = Math.Min(mercatorLocation1.Item2, mercatorLocation2.Item2);
                    double maxY = Math.Max(mercatorLocation1.Item2, mercatorLocation2.Item2);

                    var bounds = new Mapsui.MRect(minX, minY, maxX, maxY).Grow(500); // Agrandir de 500 mètres

                    await Task.Delay(500); // Attendre que la carte soit complètement initialisée
                    map.Navigator.ZoomToBox(bounds);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
            }
        }




        private void OnCoordinateTapped(object sender, EventArgs e)
        {
            try
            {
                // Définir les coordonnées selon le label sélectionné
                double latitude = 0.0;
                double longitude = 0.0;

                var label = sender as Label;

                if (label == null)
                    return;

                if (label == LatitudeLabel1 || label == LongitudeLabel1)
                {
                    double.TryParse(LatitudeLabel1.Text.Split(':')[1].Trim(), out latitude);
                    double.TryParse(LongitudeLabel1.Text.Split(':')[1].Trim(), out longitude);
                }
                else if (label == LatitudeLabel2 || label == LongitudeLabel2)
                {
                    double.TryParse(LatitudeLabel2.Text.Split(':')[1].Trim(), out latitude);
                    double.TryParse(LongitudeLabel2.Text.Split(':')[1].Trim(), out longitude);
                }

                if (latitude == 0.0 || longitude == 0.0)
                {
                    DisplayAlert("Erreur", "Les coordonnées sont invalides", "OK");
                    return;
                }

                var mercatorLocation = Mapsui.Projections.SphericalMercator.FromLonLat(longitude, latitude);

                var bounds = new Mapsui.MRect(mercatorLocation.Item1 - 500, mercatorLocation.Item2 - 500, mercatorLocation.Item1 + 500, mercatorLocation.Item2 + 500);
                // Appliquer le zoom sur le point sélectionné
                mapView.Map?.Navigator.ZoomToBox(bounds);
            }
            catch (Exception ex)
            {
                DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
            }
        }


        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadius = 6371.0; // Rayon de la Terre en kilomètres

            double lat1Rad = lat1 * Math.PI / 180.0;
            double lon1Rad = lon1 * Math.PI / 180.0;
            double lat2Rad = lat2 * Math.PI / 180.0;
            double lon2Rad = lon2 * Math.PI / 180.0;

            double dLat = lat2Rad - lat1Rad;
            double dLon = lon2Rad - lon1Rad;

            // Calcul de la distance avec la formule de Haversine
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            // Distance en kilomètres
            double distance = EarthRadius * c;
            return distance;
        }


        private async void OpenGoogleMaps(object sender, EventArgs e)
        {
            try
            {
                string uri = "";

                if (latitude1 != 0 && longitude1 != 0 && latitude2 == 0 && longitude2 == 0)
                {
                    string formattedLatitude1 = latitude1.ToString("F6", CultureInfo.InvariantCulture);
                    string formattedLongitude1 = longitude1.ToString("F6", CultureInfo.InvariantCulture);
                    uri = $"https://www.google.com/maps/search/?api=1&query={formattedLatitude1},{formattedLongitude1}";
                }
                else if (latitude1 != 0 && longitude1 != 0 && latitude2 != 0 && longitude2 != 0)
                {
                    string formattedLatitude1 = latitude1.ToString("F6", CultureInfo.InvariantCulture);
                    string formattedLongitude1 = longitude1.ToString("F6", CultureInfo.InvariantCulture);
                    string formattedLatitude2 = latitude2.ToString("F6", CultureInfo.InvariantCulture);
                    string formattedLongitude2 = longitude2.ToString("F6", CultureInfo.InvariantCulture);
                    uri = $"https://www.google.com/maps/dir/{formattedLatitude1},{formattedLongitude1}/{formattedLatitude2},{formattedLongitude2}";
                }
                else
                {
                    await DisplayAlert("Erreur", "Aucune coordonnée valide trouvée", "OK");
                    return;
                }

                await Launcher.OpenAsync(new Uri(uri));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Impossible d'ouvrir Google Maps : {ex.Message}", "OK");
            }
        }
        private void ZoomToBoundsButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Vérifier si les deux points sont valides
                if (latitude1 == 0.0 || longitude1 == 0.0 || latitude2 == 0.0 || longitude2 == 0.0)
                {
                    DisplayAlert("Erreur", "Les coordonnées sont invalides", "OK");
                    return;
                }

                var mercatorLocation1 = Mapsui.Projections.SphericalMercator.FromLonLat(longitude1, latitude1);
                var mercatorLocation2 = Mapsui.Projections.SphericalMercator.FromLonLat(longitude2, latitude2);
                double minX = Math.Min(mercatorLocation1.Item1, mercatorLocation2.Item1);
                double maxX = Math.Max(mercatorLocation1.Item1, mercatorLocation2.Item1);
                double minY = Math.Min(mercatorLocation1.Item2, mercatorLocation2.Item2);
                double maxY = Math.Max(mercatorLocation1.Item2, mercatorLocation2.Item2);

                var bounds = new Mapsui.MRect(minX, minY, maxX, maxY).Grow(500); // Agrandir de 500 mètres

                mapView.Map?.Navigator.ZoomToBox(bounds);
            }
            catch (Exception ex)
            {
                DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
            }
        }

        /*   protected override void OnSizeAllocated(double width, double height)
           {
               base.OnSizeAllocated(width, height);
               AdjustMapHeight();
           }*/

        /* private void AdjustMapHeight()
         {
             // Adapter la taille de la carte en fonction de la taille de l'écran
             if (Width > Height) // Mode paysage (tablette ou écran plus large)
             {
                 Resources["MapHeight"] = 500; // Hauteur de la carte pour les tablettes
             }
             else // Mode portrait (smartphone ou écran plus petit)
             {
                 Resources["MapHeight"] = 300; // Hauteur réduite pour les smartphones
             }
         }*/
    }
}
