using Acr.UserDialogs;
using DevExpress.Maui.Editors;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;
using System.Globalization;
using Color = Mapsui.Styles.Color;
using Brush = Mapsui.Styles.Brush;

namespace SmartPharma5.View;

public partial class AddPartnerTemp : ContentPage
{
    public bool isUserInteraction = false;
    public AddPartnerTemp()
    {
        try
        {
            InitializeComponent();
            BindingContext = new AddPartnerTempMV();
            var OVM = BindingContext as AddPartnerTempMV;

            foreach (var item in OVM.ListNotVisible)
            {
                if (item == "vat_code")
                {
                    vat_code.IsVisible = false;
                }
                else if (item == "name")
                {
                    name.IsVisible = false;
                }
                else if (item == "country")
                {
                    country.IsVisible = false;
                }
                else if (item == "state")
                {
                    state.IsVisible = false;
                }
                else if (item == "postale_code")
                {
                    postale_code.IsVisible = false;
                }
                else if (item == "email")
                {
                    email.IsVisible = false;
                }
                else if (item == "fax")
                {
                    fax.IsVisible = false;
                }
                else if (item == "iscustomer")
                {
                    isCustomer.IsVisible = false;
                }
                else if (item == "issupplier")
                {
                    isSupplier.IsVisible = false;
                }
                else if (item == "category")
                {
                    //name.LabelText += "*";
                    Category.IsVisible = false;
                }
            }
            foreach (var item in OVM.ListRequired)
            {
                if (item == "vat_code")
                {
                    vat_code.LabelText += "*";
                }
                else if (item == "name")
                {
                    name.LabelText += "*";
                }
                else if (item == "country")
                {
                    country.LabelText += "*";
                }
                else if (item == "state")
                {
                    state.LabelText += "*";
                }
                else if (item == "postale_code")
                {
                    postale_code.LabelText += "*";
                }
                else if (item == "email")
                {
                    email.LabelText += "*";
                }
                else if (item == "fax")
                {
                    fax.LabelText += "*";
                }
                else if (item == "iscustomer")
                {
                    isCustomer.Text += "*";
                }
                else if (item == "issupplier")
                {
                    isSupplier.Text += "*";
                }
                else if (item == "category")
                {
                    //name.LabelText += "*";
                }
            }
        }
        catch(Exception ex)
        {

        }
        
    }
    /***************************************************/

    private async void OnLocationButtonClicked(object sender, EventArgs e)
    {
        // Afficher les options
        var action = await DisplayActionSheet(
            "Choisir le mode de localisation",
            "Annuler",
            null,
            "Position actuelle",
            "Sélection manuelle sur carte");

        if (action == "Position actuelle")
        {
            await GetCurrentLocation();
        }
        else if (action == "Sélection manuelle sur carte")
        {
            await OpenManualMapSelection();
        }
    }

    // Méthode existante modifiée
    private async Task GetCurrentLocation()
    {
        try
        {
            UserDialogs.Instance.ShowLoading("Obtention de la position...");

            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Erreur", "Permission de localisation refusée", "OK");
                    return;
                }
            }

            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            var location = await Geolocation.GetLocationAsync(request);

            if (location != null)
            {
                locationTextEdit.Text = $"{location.Latitude.ToString(CultureInfo.InvariantCulture)},{location.Longitude.ToString(CultureInfo.InvariantCulture)}";

                // Stockez aussi dans le ViewModel si nécessaire
                if (BindingContext is AddPartnerTempMV vm)
                {
                    vm.Gps = locationTextEdit.Text;
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
        finally
        {
            UserDialogs.Instance.HideLoading();
        }
    }

    // Nouvelle méthode pour la carte manuelle
    private async Task OpenManualMapSelection()
    {
        try
        {
            var mapView = new Mapsui.UI.Maui.MapView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            mapView.Map = new Mapsui.Map();
            mapView.Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

            double currentLat = 0, currentLon = 0;

            // Initialiser la carte avec les coordonnées existantes si disponibles
            if (!string.IsNullOrEmpty(locationTextEdit.Text))
            {
                var coords = locationTextEdit.Text.Split(',');
                if (coords.Length == 2 &&
                    double.TryParse(coords[0], NumberStyles.Any, CultureInfo.InvariantCulture, out currentLat) &&
                    double.TryParse(coords[1], NumberStyles.Any, CultureInfo.InvariantCulture, out currentLon))
                {
                    var worldPos = Mapsui.Projections.SphericalMercator.FromLonLat(currentLon, currentLat);
                    mapView.Map.Home = n => n.CenterOnAndZoomTo(
                        new MPoint(worldPos.x, worldPos.y),
                        n.Resolutions[9]);
                    AddMarker(mapView, worldPos.x, worldPos.y);
                }
            }

            var mainLayout = new Grid
            {
                RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(0.92, GridUnitType.Star) },
                new RowDefinition { Height = new GridLength(0.06, GridUnitType.Star) }
            },
                ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            }
            };

            mainLayout.Children.Add(mapView);
            Grid.SetRow(mapView, 0);
            Grid.SetColumnSpan(mapView, 2);

            var closeButton = new Button
            {
                Text = "Fermer",
                BackgroundColor = Colors.Red,
                TextColor = Colors.White,
                CornerRadius = 8,
                Margin = new Thickness(10, 5),
                HeightRequest = 40
            };

            var validateButton = new Button
            {
                Text = "Valider",
                BackgroundColor = Colors.Green,
                TextColor = Colors.White,
                CornerRadius = 8,
                Margin = new Thickness(10, 5),
                HeightRequest = 40
            };

            mainLayout.Children.Add(closeButton);
            Grid.SetRow(closeButton, 1);
            Grid.SetColumn(closeButton, 0);

            mainLayout.Children.Add(validateButton);
            Grid.SetRow(validateButton, 1);
            Grid.SetColumn(validateButton, 1);

            closeButton.Clicked += async (s, e) => await Navigation.PopModalAsync();

            validateButton.Clicked += async (s, e) =>
            {
                if (currentLat != 0 && currentLon != 0)
                {
                    locationTextEdit.Text = $"{currentLat.ToString(CultureInfo.InvariantCulture)},{currentLon.ToString(CultureInfo.InvariantCulture)}";
                    await Navigation.PopModalAsync();
                }
                else
                {
                    await DisplayAlert("Erreur", "Veuillez sélectionner une position sur la carte", "OK");
                }
            };

            mapView.LongTap += (sender, e) =>
            {
                var worldPosition = mapView.Map.Navigator.Viewport.ScreenToWorld(e.ScreenPosition);
                var lonLat = Mapsui.Projections.SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);
                currentLat = lonLat.lat;
                currentLon = lonLat.lon;

                AddMarker(mapView, worldPosition.X, worldPosition.Y);
            };

            await Navigation.PushModalAsync(new ContentPage
            {
                Title = "Sélection manuelle",
                Content = mainLayout
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private void AddMarker(MapView mapView, double x, double y)
    {
        // Créer un nouveau marqueur
        var newMarker = new PointFeature(new MPoint(x, y))
        {
            Styles = new List<IStyle>
        {
            new SymbolStyle
            {
                SymbolType = SymbolType.Ellipse,
                Fill = new Brush(Color.Red),
                SymbolScale = 0.5,
                Outline = new Pen(Color.Black, 2)
            }
        }
        };

        // Trouver la couche de marqueur existante
        var markerLayer = mapView.Map.Layers.OfType<MemoryLayer>().FirstOrDefault(l => l.Name == "MarkerLayer");

        if (markerLayer == null)
        {
            // Créer une nouvelle couche si elle n'existe pas
            markerLayer = new MemoryLayer
            {
                Name = "MarkerLayer",
                Features = new List<IFeature> { newMarker } // Utilisation de List<IFeature> au lieu de IEnumerable
            };
            mapView.Map.Layers.Add(markerLayer);
        }
        else
        {
            // Convertir Features en List pour pouvoir utiliser Clear et Add
            if (markerLayer.Features is List<IFeature> featureList)
            {
                featureList.Clear();
                featureList.Add(newMarker);
            }
            else
            {
                // Si Features n'est pas une List, créer une nouvelle List
                markerLayer.Features = new List<IFeature> { newMarker };
            }
        }

        // Forcer le rafraîchissement de la carte
        mapView.Refresh();
    }
    private MapView CreateInteractiveMap()
    {
        var mapView = new MapView
        {
            VerticalOptions = LayoutOptions.FillAndExpand,
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        // Configuration de base de la carte
        mapView.Map = new Mapsui.Map();
        mapView.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

        // Gestion du clic long
        mapView.LongTap += async (sender, e) =>
        {
            var worldPosition = mapView.Map.Navigator.Viewport.ScreenToWorld(e.ScreenPosition);
            var (longitude, latitude) = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);

            bool confirm = await DisplayAlert(
                "Confirmation",
                $"Utiliser cette position?\nLat: {latitude:F6}\nLon: {longitude:F6}",
                "Oui", "Non");

            if (confirm)
            {
                locationTextEdit.Text = $"{latitude.ToString(CultureInfo.InvariantCulture)},{longitude.ToString(CultureInfo.InvariantCulture)}";

                // Mise à jour du ViewModel
                if (BindingContext is AddPartnerTempMV vm)
                {
                    vm.Gps = locationTextEdit.Text;
                }

                await Navigation.PopModalAsync();
            }
        };

        return mapView;
    }

    //private async void OnLocationButtonClicked(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        UserDialogs.Instance.ShowLoading("Récupération de la position...");

    //        // Vérification des permissions
    //        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
    //        if (status != PermissionStatus.Granted)
    //        {
    //            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
    //            if (status != PermissionStatus.Granted)
    //            {
    //                await DisplayAlert("Erreur", "Permission de localisation refusée", "OK");
    //                return;
    //            }
    //        }

    //        // Vérifier si le GPS est activé
    //        var isGpsEnabled = await CheckIfGpsEnabled();
    //        if (!isGpsEnabled)
    //        {
    //            await DisplayAlert("GPS désactivé", "Veuillez activer le GPS pour continuer", "OK");
    //            return;
    //        }

    //        // Récupération de la position
    //        var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
    //        var location = await Geolocation.GetLocationAsync(request);

    //        if (location != null)
    //        {
    //            locationTextEdit.Text = $"{location.Latitude.ToString(CultureInfo.InvariantCulture)}, {location.Longitude.ToString(CultureInfo.InvariantCulture)}";

    //            // Optionnel: centrer la carte sur cette position si vous avez une carte
    //            // await CenterMapOnLocation(location.Latitude, location.Longitude);
    //        }
    //        else
    //        {
    //            await DisplayAlert("Erreur", "Impossible de récupérer la position", "OK");
    //        }
    //    }
    //    catch (FeatureNotEnabledException)
    //    {
    //        await DisplayAlert("Erreur", "Le service de localisation n'est pas activé", "OK");
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert("Erreur", $"Une erreur est survenue: {ex.Message}", "OK");
    //    }
    //    finally
    //    {
    //        UserDialogs.Instance.HideLoading();
    //    }
    //}

    //// Méthode pour vérifier si le GPS est activé
    //private async Task<bool> CheckIfGpsEnabled()
    //{
    //    try
    //    {
    //        var quickRequest = new GeolocationRequest(GeolocationAccuracy.Lowest, TimeSpan.FromSeconds(2));
    //        var quickLocation = await Geolocation.GetLocationAsync(quickRequest);
    //        return quickLocation != null;
    //    }
    //    catch (FeatureNotEnabledException)
    //    {
    //        return false;
    //    }
    //    catch (Exception)
    //    {
    //        return false;
    //    }
    //}   
    /***************************************************/
    private void comboBox_TouchUp(object sender, EventArgs e)
    {
        isUserInteraction = true;

    }

    private void picker_SelectedIndexChanged(object sender, EventArgs e)
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
                var ovm = BindingContext as AddPartnerTempMV;

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


                            attribute.Selected_item = null;
                            attribute.Multiple_value = 0;
                            attribute.List_item = attribute.List_item_fixe;

                            attribute.List_item = attribute.List_item.Where(p => p.parent == test10.id || p.parent == null).ToList();
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





    private void ComboBoxEdit_Tap(object sender, System.ComponentModel.HandledEventArgs e)
    {
        isUserInteraction = true;

    }
}