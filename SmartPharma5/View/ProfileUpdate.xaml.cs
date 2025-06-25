using SmartPharma5.ViewModel;
using SmartPharma5.Model;
using Acr.UserDialogs;
using System.Globalization;
using Mapsui.Extensions;
using Mapsui;
using MPoint = Mapsui.MPoint;
using Mapsui.Layers;
using Mapsui.Styles;
using Brush = Mapsui.Styles.Brush;
using Color = Mapsui.Styles.Color;
using Mapsui.UI.Maui;

//using static Android.Renderscripts.FileA3D;

namespace SmartPharma5.View;
[XamlCompilation(XamlCompilationOptions.Compile)]

public partial class ProfileUpdate : ContentPage
{
    public Partner Partner { get; set; }

    public ProfileUpdate()
    {
        InitializeComponent();

        // Lier le contexte de donn�es ici si n�cessaire
        BindingContext = new DocumentViewModel();

    }
    public ProfileUpdate(uint a)
    {
        InitializeComponent();
        BindingContext = new UpdateProfileMV(a);
        this.Partner = new Partner(); 
        this.Partner.Id = a;

    }

    /*******************************/
    private bool isSwipeOpen = false;

    private void OnSwipeViewClicked(object sender, EventArgs e)
    {
        if (isSwipeOpen)
        {
            MainSwipeView.Close();
        }
        else
        {
            MainSwipeView.Open(OpenSwipeItem.RightItems);
        }

        isSwipeOpen = !isSwipeOpen;
    }
    private bool _refreshEnabled;
    private bool _firstAppear = true;

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Activer le rafra�chissement p�riodique
        _refreshEnabled = true;

        // Ex�cuter le premier rafra�chissement
        await RefreshMessages();

        // D�marrer le timer pour les rafra�chissements p�riodiques
        Device.StartTimer(TimeSpan.FromSeconds(3), () =>
        {
            if (!_refreshEnabled) return false;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await RefreshMessages();
            });
            return _refreshEnabled;
        });

        // Jouer l'animation seulement au premier affichage
        if (_firstAppear)
        {
            _firstAppear = false;
            await AnimateSwipeHint();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // D�sactiver le rafra�chissement p�riodique
        _refreshEnabled = false;
    }

    private async Task RefreshMessages()
    {
        try
        {
            int userId = Preferences.Get("iduser", 0);
            int unreadCount = await UserModel.GetUnreadMessagesCountAsync(
                userId,
                CurrentData.CurrentModuleId,
                CurrentData.CurrentNoteModule);

            Device.BeginInvokeOnMainThread(() =>
            {
                UnreadBadgeLabel.Text = unreadCount.ToString();
                UnreadBadgeFrame.IsVisible = unreadCount > 0;
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du rafra�chissement: {ex.Message}");
        }
    }

    private async Task AnimateSwipeHint()
    {
        // Faire appara�tre l'indicateur
        await swipeHint.FadeTo(0.8, 300);

        // Animation de balayage (2 cycles)
        for (int i = 0; i < 2; i++)
        {
            await swipeHint.TranslateTo(-20, 0, 300, Easing.SinInOut);
            await swipeHint.TranslateTo(0, 0, 300, Easing.SinInOut);
        }

        // Rendre l'indicateur plus discret
        await swipeHint.FadeTo(0.3, 300);
    }
    /*******************************/
    private async void OnButtonClicked(object sender, EventArgs e)
    {
        // Afficher une action sheet avec les deux options
        var action = await DisplayActionSheet("Choisir une option de localisation", "Annuler", null,
            "Utiliser ma position actuelle", "Entrer une localisation manuelle");

        if (action == "Utiliser ma position actuelle")
        {
            await GetCurrentLocation();
        }
        else if (action == "Entrer une localisation manuelle")
        {
            await EnterManualLocation();
        }
    }

    private async Task GetCurrentLocation()
    {
        try
        {
            UserDialogs.Instance.ShowLoading("Chargement...");
            await Task.Delay(200);

            // V�rifier les permissions (garder votre code existant)
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    bool openSettings = await DisplayAlert("Permission refus�e",
                        "L'acc�s � la localisation est n�cessaire pour cette fonctionnalit�. Voulez-vous l'activer dans les param�tres?",
                        "Ouvrir les param�tres", "Annuler");

                    if (openSettings)
                    {
                        AppInfo.ShowSettingsUI();
                    }

                    UserDialogs.Instance.HideLoading();
                    return;
                }
            }

            // V�rifier si la localisation est activ�e
            var locationEnabled = false;
            try
            {
                var quickRequest = new GeolocationRequest(GeolocationAccuracy.Lowest, TimeSpan.FromSeconds(2));
                var quickLocation = await Geolocation.GetLocationAsync(quickRequest);
                locationEnabled = quickLocation != null;
            }
            catch (FeatureNotEnabledException)
            {
                locationEnabled = false;
            }
            catch (Exception)
            {
                locationEnabled = false;
            }

            if (!locationEnabled)
            {
                bool activateLocation = await DisplayAlert("Localisation d�sactiv�e",
                    "Le service de localisation est d�sactiv� sur votre appareil. Voulez-vous l'activer maintenant?",
                    "Activer", "Annuler");

                if (activateLocation)
                {
                    AppInfo.ShowSettingsUI();
                    await DisplayAlert("Instructions",
                        "Apr�s avoir activ� la localisation, revenez � l'application et appuyez � nouveau sur le bouton.",
                        "OK");

                    UserDialogs.Instance.HideLoading();
                    return;
                }
                else
                {
                    await DisplayAlert("Information",
                        "Cette fonctionnalit� n�cessite l'acc�s � votre localisation.",
                        "OK");

                    UserDialogs.Instance.HideLoading();
                    return;
                }
            }

            // Obtenir la localisation
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.GetLocationAsync(request);

            if (location != null)
            {
                //await ProcessLocation(location.Latitude, location.Longitude);
                await ProcessLocation(location.Latitude, location.Longitude, false);
            }
            else
            {
                await DisplayAlert("Erreur", "Impossible de r�cup�rer votre position GPS.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
        }
        finally
        {
            UserDialogs.Instance.HideLoading();
        }
    }
        private async Task EnterManualLocation()
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

                // R�cup�rer les coordonn�es existantes
                var viewModel = BindingContext as UpdateProfileMV;
                double currentLat = 0, currentLon = 0;
                if (viewModel?.Partner != null)
                {
                    var existingGps = await Partner.GetExistingGpsCoordinates(viewModel.Partner.Id);
                    if (!string.IsNullOrEmpty(existingGps))
                    {
                        var coords = existingGps.Split(',');
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
                }

                // Cr�ation du layout principal avec proportions pr�cises
                var mainLayout = new Grid
                {
                    RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(0.92, GridUnitType.Star) }, // 92% pour la carte
                    new RowDefinition { Height = new GridLength(0.06, GridUnitType.Star) }    // 6% pour les boutons
                },
                    ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
                };

                // Ajout de la carte (prend toute la largeur et 92% de hauteur)
                mainLayout.Children.Add(mapView);
                Grid.SetRow(mapView, 0);
                Grid.SetColumnSpan(mapView, 2);

                // Bouton Fermer
                var closeButton = new Button
                {
                    Text = "Fermer",
                    BackgroundColor = Colors.Red,
                    TextColor = Colors.White,
                    CornerRadius = 8,
                    Margin = new Thickness(10, 5),
                    HeightRequest = 40
                };

                // Bouton Google Maps
                var googleMapsButton = new Button
                {
                    Text = "Google Maps",
                    BackgroundColor = Colors.Blue,
                    TextColor = Colors.White,
                    CornerRadius = 8,
                    Margin = new Thickness(10, 5),
                    HeightRequest = 40
                };

                // Ajout des boutons directement dans le mainLayout
                mainLayout.Children.Add(closeButton);
                Grid.SetRow(closeButton, 1);
                Grid.SetColumn(closeButton, 0);

                mainLayout.Children.Add(googleMapsButton);
                Grid.SetRow(googleMapsButton, 1);
                Grid.SetColumn(googleMapsButton, 1);

                // Gestion des �v�nements
                closeButton.Clicked += async (s, e) => await Navigation.PopModalAsync();
                googleMapsButton.Clicked += async (s, e) =>
                {
                    if (currentLat != 0 && currentLon != 0)
                    {
                        // Utilisation de CultureInfo.InvariantCulture pour garantir le point comme s�parateur d�cimal
                        var latStr = currentLat.ToString(CultureInfo.InvariantCulture);
                        var lonStr = currentLon.ToString(CultureInfo.InvariantCulture);
                        var uri = $"https://www.google.com/maps/search/?api=1&query={latStr},{lonStr}";
                        await Launcher.OpenAsync(new Uri(uri));
                    }
                    else
                    {
                        await DisplayAlert("Erreur", "Position invalide", "OK");
                    }
                };

                var page = new ContentPage
                {
                    Title = "Carte",
                    Content = mainLayout
                };

                mapView.LongTap += async (sender, e) =>
                {
                    var worldPosition = ConvertScreenToWorld(mapView, e.ScreenPosition);
                    currentLat = Mapsui.Projections.SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y).lat;
                    currentLon = Mapsui.Projections.SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y).lon;
                    await HandleLongTap(mapView, worldPosition);
                };

                await Navigation.PushModalAsync(page);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
            }
        }
    private void AddMarker(MapView mapView, double x, double y)
    {
        var layer = new MemoryLayer
        {
            Name = "Marker Layer",
            Features = new List<IFeature>
        {
            new PointFeature(new MPoint(x, y))
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
            }
        }
        };

        // Supprimer l'ancien marqueur s'il existe
        var existingLayer = mapView.Map.Layers.FirstOrDefault(l => l.Name == "Marker Layer");
        if (existingLayer != null)
        {
            mapView.Map.Layers.Remove(existingLayer);
        }

        mapView.Map.Layers.Add(layer);
    }

    private async Task HandleLongTap(MapView mapView, MPoint worldPosition)
    {
        // Convertir en lat/lon
        var (lon, lat) = Mapsui.Projections.SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);

        // Demander confirmation
        bool confirm = await DisplayAlert("Confirmation",
            $"Voulez-vous d�finir cette position?\nLat: {lat:F6}\nLon: {lon:F6}",
            "Oui", "Non");

        if (confirm)
        {
            // Ajouter/mettre � jour le marqueur
            AddMarker(mapView, worldPosition.X, worldPosition.Y);
            // Mettre � jour la base de donn�es
            var viewModel = BindingContext as UpdateProfileMV;
            if (viewModel?.Partner != null)
            {
                await Partner.UpdateManualGpsCoordinates(viewModel.Partner.Id, lat, lon);
                await DisplayAlert("Succ�s", "Position mise � jour avec succ�s", "OK");
            }
        }

    }
    private Mapsui.MPoint ConvertScreenToWorld(Mapsui.UI.Maui.MapView mapView, MPoint screenPosition)
    {
        // Solution alternative pour la conversion des coordonn�es
        if (mapView.Map?.Navigator != null)
        {
            return mapView.Map.Navigator.Viewport.ScreenToWorld(screenPosition);
        }
        throw new InvalidOperationException("Map or Navigator is not initialized");
    }

    private async Task ProcessLocation(double latitude, double longitude, bool isManual = false)
    {
        try
        {
            UserDialogs.Instance.ShowLoading("Enregistrement...");

            string formattedLatitude = latitude.ToString("F6", CultureInfo.InvariantCulture);
            string formattedLongitude = longitude.ToString("F6", CultureInfo.InvariantCulture);
            string gpsCoordinates = $"{formattedLatitude},{formattedLongitude}";

            // Mettre � jour la base de donn�es
            var viewModel = BindingContext as UpdateProfileMV;
            if (viewModel != null && viewModel.Partner != null)
            {
                if (isManual)
                {
                    // Appel pour la localisation manuelle
                    await Partner.UpdateManualGpsCoordinates(viewModel.Partner.Id, latitude, longitude);
                }
                else
                {
                    // Appel pour la localisation automatique
                    await Partner.UpdateGpsCoordinates(viewModel.Partner.Id);
                    string uri = $"https://www.google.com/maps/search/?api=1&query={formattedLatitude},{formattedLongitude}";
                    await Launcher.OpenAsync(new Uri(uri));
                }

                // Afficher une confirmation
                await DisplayAlert("Succ�s", "Les coordonn�es GPS ont �t� mises � jour.", "OK");

                // Ouvrir Google Maps

            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur s'est produite lors de l'enregistrement : {ex.Message}", "OK");
        }
        finally
        {
            UserDialogs.Instance.HideLoading();
        }
    }
    /****************/

    //private async void OnButtonClicked(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        UserDialogs.Instance.ShowLoading("Loading...");
    //        await Task.Delay(200);

    //        // V�rifier si l'application a la permission d'acc�der � la localisation
    //        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

    //        if (status != PermissionStatus.Granted)
    //        {
    //            // Demander la permission � l'utilisateur
    //            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

    //            if (status != PermissionStatus.Granted)
    //            {
    //                // Si l'utilisateur refuse, lui proposer d'aller dans les param�tres
    //                bool openSettings = await DisplayAlert("Permission refus�e",
    //                    "L'acc�s � la localisation est n�cessaire pour cette fonctionnalit�. Voulez-vous l'activer dans les param�tres?",
    //                    "Ouvrir les param�tres", "Annuler");

    //                if (openSettings)
    //                {
    //                    // Ouvrir les param�tres de l'application o� l'utilisateur peut activer les permissions
    //                    AppInfo.ShowSettingsUI();
    //                }

    //                UserDialogs.Instance.HideLoading();
    //                return;
    //            }
    //        }

    //        // Essayer d'obtenir la localisation rapidement pour v�rifier si le service est activ�
    //        var locationEnabled = false;
    //        try
    //        {
    //            var quickRequest = new GeolocationRequest(GeolocationAccuracy.Lowest, TimeSpan.FromSeconds(2));
    //            var quickLocation = await Geolocation.GetLocationAsync(quickRequest);
    //            locationEnabled = quickLocation != null;
    //        }
    //        catch (FeatureNotEnabledException)
    //        {
    //            locationEnabled = false;
    //        }
    //        catch (Exception)
    //        {
    //            locationEnabled = false;
    //        }

    //        // Si la localisation est d�sactiv�e, inviter l'utilisateur � l'activer
    //        if (!locationEnabled)
    //        {
    //            bool activateLocation = await DisplayAlert("Localisation d�sactiv�e",
    //                "Le service de localisation est d�sactiv� sur votre appareil. Voulez-vous l'activer maintenant?",
    //                "Activer", "Annuler");

    //            if (activateLocation)
    //            {
    //                // Sur Android et iOS, cela ouvre les param�tres de localisation du syst�me
    //                AppInfo.ShowSettingsUI();

    //                // Message indiquant � l'utilisateur de revenir � l'application apr�s avoir activ� la localisation
    //                await DisplayAlert("Instructions",
    //                    "Apr�s avoir activ� la localisation, revenez � l'application et appuyez � nouveau sur le bouton.",
    //                    "OK");

    //                UserDialogs.Instance.HideLoading();
    //                return;
    //            }
    //            else
    //            {
    //                // L'utilisateur a refus� d'activer la localisation
    //                await DisplayAlert("Information",
    //                    "Cette fonctionnalit� n�cessite l'acc�s � votre localisation.",
    //                    "OK");

    //                UserDialogs.Instance.HideLoading();
    //                return;
    //            }
    //        }

    //        // Si nous arrivons ici, la localisation devrait �tre disponible
    //        // Obtenir la localisation avec une meilleure pr�cision
    //        var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
    //        var location = await Geolocation.GetLocationAsync(request);

    //        if (location != null)
    //        {
    //            // Formater les coordonn�es GPS
    //            string formattedLatitude = location.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);
    //            string formattedLongitude = location.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);

    //            // Construire les coordonn�es GPS sous forme de cha�ne
    //            string gpsCoordinates = $"{formattedLatitude},{formattedLongitude}";

    //            // Mettre � jour la base de donn�es avec les coordonn�es GPS
    //            var viewModel = BindingContext as UpdateProfileMV;
    //            if (viewModel != null && viewModel.Partner != null)
    //            {
    //                // Appeler la m�thode pour mettre � jour la colonne DeliveryNumber dans la base de donn�es
    //                await Partner.UpdateGpsCoordinates(viewModel.Partner.Id);



    //                // Afficher une confirmation que les coordonn�es ont �t� mises � jour
    //                await DisplayAlert("Succ�s", "Les coordonn�es GPS ont �t� mises � jour.", "OK");

    //                // Ouvrir Google Maps pour afficher la position actuelle
    //                string uri = $"https://www.google.com/maps/search/?api=1&query={formattedLatitude},{formattedLongitude}";
    //                await Launcher.OpenAsync(new Uri(uri));
    //            }
    //        }
    //        else
    //        {
    //            await DisplayAlert("Erreur", "Impossible de r�cup�rer votre position GPS.", "OK");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // G�rer les erreurs qui peuvent survenir
    //        await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
    //    }
    //    finally
    //    {
    //        UserDialogs.Instance.HideLoading();
    //    }
    //}
    //private async void OnButtonClicked(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        UserDialogs.Instance.ShowLoading("Loading...");
    //        await Task.Delay(200);
    //        // R�cup�rer la localisation de l'utilisateur
    //        var location = await Geolocation.GetLocationAsync();

    //        if (location != null)
    //        {
    //            // Formater les coordonn�es GPS
    //            string formattedLatitude = location.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);
    //            string formattedLongitude = location.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);

    //            // Construire les coordonn�es GPS sous forme de cha�ne
    //            string gpsCoordinates = $"{formattedLatitude},{formattedLongitude}";

    //            // Afficher une confirmation que la position a �t� r�cup�r�e
    //            //await DisplayAlert("Succ�s", "Votre position a �t� r�cup�r�e avec succ�s.", "OK");

    //            // Mettre � jour la base de donn�es avec les coordonn�es GPS
    //            var viewModel = BindingContext as UpdateProfileMV;
    //            if (viewModel != null && viewModel.Partner != null)
    //            {
    //                // Appeler la m�thode pour mettre � jour la colonne DeliveryNumber dans la base de donn�es
    //                await Partner.UpdateGpsCoordinates(viewModel.Partner.Id);

    //                // Afficher une confirmation que les coordonn�es ont �t� mises � jour
    //                await DisplayAlert("Success", "The GPS coordinates have been updated.", "OK");

    //                // Ouvrir Google Maps pour afficher la position actuelle
    //                string uri = $"https://www.google.com/maps/search/?api=1&query={formattedLatitude},{formattedLongitude}";
    //                await Launcher.OpenAsync(new Uri(uri));
    //            }
    //        }
    //        else
    //        {
    //            await DisplayAlert("Erreur", "Impossible de r�cup�rer votre position GPS.", "OK");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // G�rer les erreurs qui peuvent survenir
    //        await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
    //    }
    //    UserDialogs.Instance.HideLoading();
    //}
    //private async void OnDocumentButtonClicked(object sender, EventArgs e)
    //{
    //    if (this.Partner != null)
    //    {
    //       // var partnerId = this.Partner.Id;
    //        var partnerId = (int)this.Partner.Id;


    //        // Passer l'ID du partenaire � ProfileUpdateFileSelectionView
    //        //await Navigation.PushAsync(new ProfileUpdateFileSelectionView(partnerId));
    //        await Navigation.PushAsync(new FileSelectionView(partnerId, EntityType.Partner));

    //    }
    //    else
    //    {
    //        await DisplayAlert("Erreur", "Aucun partenaire trouv�.", "OK");
    //    }
    //}


    private async void OnActionButtonClickedmemo(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        UserDialogs.Instance.ShowLoading("Loading...");
        await Task.Delay(200);
        if (button.Source.ToString().Contains("note3.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.MemoView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqu� sur {buttonName}", "OK");
        }
        UserDialogs.Instance.HideLoading();

    }
    private async void OnActionButtonClickedactivity(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        //UserDialogs.Instance.ShowLoading("Loading...");
        //await Task.Delay(200);
        if (button.Source.ToString().Contains("activity.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.ActivityView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqu� sur {buttonName}", "OK");
        }
        //UserDialogs.Instance.HideLoading();

    }
    private async void OnActionButtonClickedreport(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        UserDialogs.Instance.ShowLoading("Loading...");
        await Task.Delay(200);
        if (button.Source.ToString().Contains("rapport.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.ReportView());
        }
        else
        {
            /* string buttonName = button.Source.ToString().Replace("File: ", "");
             await DisplayAlert("Action", $"Vous avez cliqu� sur {buttonName}", "OK");*/
        }
        UserDialogs.Instance.HideLoading();

    }
    private async void OnActionButtonClickedchat(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        UserDialogs.Instance.ShowLoading("Loading...");
        await Task.Delay(200);
        if (button.Source.ToString().Contains("chat.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.ChatView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqu� sur {buttonName}", "OK");
        }
        UserDialogs.Instance.HideLoading();

    }
    private async void OnActionButtonClickeddocument(object sender, EventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading...");

        try
        {
            var partnerId = (int)this.Partner.Id;

            // Appeler la m�me navigation que l'ancien bouton
            await Navigation.PushAsync(new FileSelectionView(partnerId, EntityType.Partner));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Une erreur est survenue: {ex.Message}", "OK");
        }
        finally
        {
            UserDialogs.Instance.HideLoading();
        }
    }
    private async void OnActionButtonClickedlogs(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        if (button.Source.ToString().Contains("logs.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.LogsView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqu� sur {buttonName}", "OK");
        }
    }

    /* private async void OnDocumentButtonClicked(object sender, EventArgs e)
     {
         // V�rifie si le partenaire existe
         if (this.Partner != null)
         {
             // Si le partenaire existe, r�cup�rer son ID
             var partnerId = this.Partner.Id;

             // Naviguer vers une nouvelle page en passant l'ID du partenaire
             await Navigation.PushAsync(new PartnerDetailView(partnerId));
         }
         else
         {
             // Si aucun partenaire trouv�, afficher un message d'erreur
             await DisplayAlert("Erreur", "Aucun partenaire trouv�.", "OK");
         }
     }
    */




}