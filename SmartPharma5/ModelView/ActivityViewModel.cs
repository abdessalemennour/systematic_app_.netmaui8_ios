using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Acr.UserDialogs;
using MvvmHelpers;
using SmartPharma5.Model;
using static SmartPharma5.Model.Activity;

namespace SmartPharma5.ModelView
{
    public class ActivityViewModel : INotifyPropertyChanged
    {
        // Variables et propriétés privées
        private int userId = Preferences.Get("iduser", 0);
        private bool testLoad;
        private bool actpopup = true;
        private int _currentViewIndex = 0; // 0 -> Memo, 1 -> Activity, 2 -> Chat
        private string _headerText = "Memo";
        private List<UserModel> _user;
        private string _activityMemo;
        private bool _isSelectionMode;
        private ObservableCollection<UserModel> _recipients = new ObservableCollection<UserModel>();
        private ObservableCollection<UserModel> _filteredUsers;
        private bool _showCheckboxes;
        private ObservableCollection<MessageModel> _messages = new ObservableCollection<MessageModel>();
        private SmartPharma5.Model.Activity.ActivityState _selectedStateActivity = Activity.selectStates.First();
        private ObservableCollection<ActivityType> _activityTypes;
        private ActivityType _selectedActivityType;
        private string _summary;
        private DateTime _dueDate;
        private Activity _selectedActivity;
        private ObservableCollection<Activity> _activities;
        private string _newActivityDescription;
        private ObservableCollection<EmployeeDto> _employees;
        private EmployeeDto _selectedEmployee;
        private Activity _currentActivity;
        private EmployeeDto _selectedActivityEmployee;
        private bool _isActivityFormVisible;
        private string _selectedView = "Memo";
        private string _selectedState;

        // Commandes
        public ICommand EnableSelectionModeCommand => new Command<UserModel>((user) =>
        {
            IsSelectionMode = true;
            user.IsSelected = true; // cocher l'élément appuyé
        });

        public ICommand SendMessageCommand { get; }
        public ICommand AddMemoCommand { get; }
        public ICommand LoadMemosCommand { get; }
        public ICommand AddActivityCommand { get; }
        public ICommand SwipeRightCommand { get; }
        public ICommand SwipeLeftCommand { get; }
        public ICommand MarkAsDoneCommand { get; }
        public ICommand SaveEditCommand { get; }
        public ICommand SaveActivityEditCommand { get; }
        public ICommand SaveEditActivityCommand { get; }
        public ICommand CancelEditCommand { get; }
        public Command NavigateToMemoCommand { get; }
        public Command NavigateToActivityCommand { get; }
        public Command NavigateToChatCommand { get; }
        public Command<int> EditActivityCommand { get; }
        public ICommand ToggleCheckboxesCommand { get; }

        // Propriétés publiques
        public bool TestLoad { get => testLoad; set => SetProperty(ref testLoad, value); }
        public bool ActPopup { get => actpopup; set => SetProperty(ref actpopup, value); }

        public int CurrentViewIndex
        {
            get => _currentViewIndex;
            set
            {
                _currentViewIndex = value;
                OnPropertyChanged();
                UpdateHeaderText();
            }
        }

        public string HeaderText
        {
            get => _headerText;
            set
            {
                _headerText = value;
                OnPropertyChanged();
            }
        }

        public List<UserModel> User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }

        public string ActivityMemo
        {
            get => _activityMemo;
            set
            {
                _activityMemo = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelectionMode
        {
            get => _isSelectionMode;
            set
            {
                if (_isSelectionMode != value)
                {
                    _isSelectionMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<UserModel> Recipients
        {
            get => _recipients;
            set
            {
                _recipients = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UserModel> FilteredUsers
        {
            get => _filteredUsers;
            set
            {
                _filteredUsers = value;
                OnPropertyChanged(nameof(FilteredUsers));
            }
        }

        public bool ShowCheckboxes
        {
            get => _showCheckboxes;
            set
            {
                _showCheckboxes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MessageModel> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public SmartPharma5.Model.Activity.ActivityState SelectedStateActivity
        {
            get => _selectedStateActivity;
            set
            {
                if (_selectedStateActivity != value)
                {
                    _selectedStateActivity = value;
                    OnPropertyChanged(nameof(SelectedStateActivity));
                    LoadFilteredActivities(); // Charger les activités quand l'état change
                }
            }
        }

        public ObservableCollection<ActivityType> ActivityTypes
        {
            get => _activityTypes;
            set
            {
                _activityTypes = value;
                OnPropertyChanged(nameof(ActivityTypes));
            }
        }

        public ActivityType SelectedActivityType
        {
            get => _selectedActivityType;
            set
            {
                if (_selectedActivityType != value)
                {
                    _selectedActivityType = value;
                    OnPropertyChanged(nameof(SelectedActivityType));

                    // Mettre à jour l'activité sélectionnée
                    if (SelectedActivity != null && value != null)
                    {
                        SelectedActivity.Type = value.Id;
                    }

                    // Mettre à jour le summary automatiquement
                    if (value != null && !string.IsNullOrEmpty(value.Summary))
                    {
                        Summary = value.Summary;
                        OnPropertyChanged(nameof(Summary));
                    }
                }
            }
        }

        public string Summary
        {
            get => _summary;
            set
            {
                _summary = value;
                OnPropertyChanged();
            }
        }

        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                OnPropertyChanged();
            }
        }

        public int EntityId { get; private set; }
        public string EntityType { get; private set; }
        public string EntityActivityType { get; private set; }
        public string EntityFormType { get; private set; }

        public Activity SelectedActivity
        {
            get => _selectedActivity;
            set
            {
                _selectedActivity = value;
                OnPropertyChanged(nameof(SelectedActivity));
            }
        }

        public ObservableCollection<Activity> Activities
        {
            get => _activities;
            set
            {
                _activities = value;
                OnPropertyChanged(nameof(Activities));
            }
        }

        public string NewActivityDescription
        {
            get => _newActivityDescription;
            set
            {
                _newActivityDescription = value;
                OnPropertyChanged(nameof(NewActivityDescription));
            }
        }

        public ObservableCollection<EmployeeDto> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
                OnPropertyChanged(nameof(Employees));
            }
        }

        public EmployeeDto SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
            }
        }

        public Activity CurrentActivity
        {
            get => _currentActivity;
            set
            {
                _currentActivity = value;
                OnPropertyChanged(nameof(CurrentActivity));
                if (value != null && Employees != null)
                {
                    SelectedEmployee = Employees.FirstOrDefault(e => e.Id == value.AssignedEmployee);
                }
            }
        }

        public EmployeeDto SelectedActivityEmployee
        {
            get => _selectedActivityEmployee;
            set
            {
                _selectedActivityEmployee = value;
                OnPropertyChanged(nameof(SelectedActivityEmployee));

                if (SelectedActivity != null && value != null)
                {
                    SelectedActivity.AssignedEmployee = value.Id;
                    SelectedActivity.AssignedEmployeeName = value.NameEmployee;
                }
            }
        }

        public bool IsActivityFormVisible
        {
            get => _isActivityFormVisible;
            set
            {
                _isActivityFormVisible = value;
                OnPropertyChanged(nameof(IsActivityFormVisible));
            }
        }

        public string SelectedView
        {
            get => _selectedView;
            set
            {
                _selectedView = value;
                OnPropertyChanged(nameof(SelectedView));
            }
        }

        public ObservableCollection<string> States { get; set; }
        public ObservableCollection<Activity> FilteredActivities { get; set; }

        public string SelectedState
        {
            get => _selectedState;
            set
            {
                _selectedState = value;
                OnPropertyChanged(nameof(SelectedState));
                FilterActivities();
            }
        }

        // Méthode utilitaire
        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        public ActivityViewModel(int entityId, string entityType, string entityActivityType)
        {
            LoadEmployeesAsync();
            if (CurrentActivity?.AssignedEmployee > 0 && Employees != null)
            {
                SelectedEmployee = Employees.FirstOrDefault(e => e.Id == CurrentActivity.AssignedEmployee);
            }
            EntityId = entityId;
            EntityType = entityType;
            EntityActivityType = entityActivityType;
            AddActivityCommand = new Command(async () => await SaveActivityAsync());
            ActivityTypes = new ObservableCollection<ActivityType>();
            Activities = new ObservableCollection<Activity>();
            SmartPharma5.Model.Activity.ActivityState
            SelectedStateActivity = Activity.selectStates.First(s => s.Name == "All");
            Task.Run(async () => await LoadFilteredActivities());
            NavigateToChatCommand = new Command(() => CurrentViewIndex = 2);
            States = new ObservableCollection<string> { "All", "In Progress", "Done", "Cancelled" };
            SaveActivityEditCommand = new Command(OnSaveActivityEdit);
            FilteredActivities = new ObservableCollection<Activity>(Activities);
            EditActivityCommand = new Command<int>(OnEditActivityClicked);
            CancelEditCommand = new Command(OnCancelEdit);
            NavigateToActivityCommand = new Command(() => LoadActivityView());
            ToggleCheckboxesCommand = new Command(() =>
            {
                ShowCheckboxes = !ShowCheckboxes;
            });
            LoadActivityTypesAsync();
        }


        public ActivityViewModel()
        {
            LoadEmployeesAsync();
            if (CurrentActivity?.AssignedEmployee > 0 && Employees != null)
            {
                SelectedEmployee = Employees.FirstOrDefault(e => e.Id == CurrentActivity.AssignedEmployee);
            }
            EntityId = CurrentData.CurrentModuleId;
            EntityType = CurrentData.CurrentNoteModule;
            EntityActivityType = CurrentData.CurrentActivityModule;
            EntityFormType = CurrentData.CurrentFormModule;
            AddActivityCommand = new Command(async () => await SaveActivityAsync());
            Activities = new ObservableCollection<Activity>();
            SmartPharma5.Model.Activity.ActivityState
            SelectedStateActivity = Activity.selectStates.First(s => s.Name == "All");
            Task.Run(async () => await LoadFilteredActivities());
            NavigateToActivityCommand = new Command(() => LoadActivityView());
            DueDate = DateTime.Now;
            ActivityTypes = new ObservableCollection<ActivityType>();
            NavigateToChatCommand = new Command(() => CurrentViewIndex = 2);
            SaveActivityEditCommand = new Command(OnSaveActivityEdit);
            States = new ObservableCollection<string> { "All", "In Progress", "Done", "Cancelled" };
            FilteredActivities = new ObservableCollection<Activity>(Activities);
            EditActivityCommand = new Command<int>(OnEditActivityClicked);
            CancelEditCommand = new Command(OnCancelEdit);
            ToggleCheckboxesCommand = new Command(() =>
            {
                ShowCheckboxes = !ShowCheckboxes;
            });
            LoadActivityTypesAsync();

        }

        public async Task LoadActivityTypesAsync()
        {
            var types = await ActivityType.GetActivityTypes(); // récupère depuis la BDD
            ActivityTypes = new ObservableCollection<ActivityType>(types);

            // Synchroniser avec l'activité en cours
            if (SelectedActivity != null)
            {
                SelectedActivityType = ActivityTypes.FirstOrDefault(t => t.Id == SelectedActivity.Type);
            }
        }

        private void LoadActivityView()
        {

            Task.Run(async () => await LoadActivities());
        }


        /****************activity***************/


        public async Task LoadEmployeesAsync()
        {
            var employees = await Activity.GetEmployeesWithFullNamesAsync();
            Employees = new ObservableCollection<EmployeeDto>(employees);
        }
        private void OnCancelEdit()
        {
            IsActivityFormVisible = false;
        }

        private int GetStateValue(string state)
        {
            switch (state)
            {
                case "In Progress":
                    return 1;
                case "Done":
                    return 2;
                case "Cancelled":
                    return 3;
                default:
                    return -1; // Pour "All"
            }
        }
        private async void OnEditActivityClicked(int activityId)
        {
            SelectedActivity = Activities.FirstOrDefault(a => a.Id == activityId);
            LoadEmployeeForSelectedActivity();

            IsActivityFormVisible = true;
        }

        private void FilterActivities()
        {
            if (SelectedState == "All")
            {
                FilteredActivities = new ObservableCollection<Activity>(Activities);
            }
            else
            {
                int stateValue = GetStateValue(SelectedState);
                FilteredActivities = new ObservableCollection<Activity>(Activities.Where(a => a.State == stateValue));
            }
            OnPropertyChanged(nameof(FilteredActivities));
        }

        private async void OnSaveActivityEdit()
        {
            UserDialogs.Instance.ShowLoading("Loading...");

            if (SelectedActivity != null)
            {
                // Mettre à jour l'employé assigné depuis le ComboBox
                if (SelectedActivityEmployee != null)
                {
                    SelectedActivity.AssignedEmployee = SelectedActivityEmployee.Id;
                }

                bool isUpdated = await Activity.UpdateActivity(SelectedActivity);

                if (isUpdated)
                {
                    SmartPharma5.Model.Activity.ActivityState
                    // Initialiser avec l'état "All" (premier élément de la liste)
                    SelectedState = Activity.selectStates.FirstOrDefault();

                    // Charger les activités initiales
                    Task.Run(async () => await LoadFilteredActivities());
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to update activity.", "OK");
                }
            }
            UserDialogs.Instance.HideLoading();

        }

        private void OnEditActivityClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            if (button != null && button.CommandParameter is int activityId)
            {
                SelectedActivity = Activities.FirstOrDefault(a => a.Id == activityId);
                if (SelectedActivity != null)
                {
                    LoadEmployeeForSelectedActivity(); // Charge l'employé actuel

                }
            }
        }
        //private async Task NavigateToMemoAsync()
        //{
        //    await LoadMemos();
        //    CurrentViewIndex = 0;
        //}
        private void LoadEmployeeForSelectedActivity()
        {
            if (SelectedActivity != null && Employees != null)
            {
                SelectedActivityEmployee = Employees.FirstOrDefault(e => e.Id == SelectedActivity.AssignedEmployee);
            }
        }


        public async Task LoadActivities()
        {
            int entityId = EntityId; // Remplacez par l'ID de l'entité
            string entityType = EntityActivityType; // Remplacez par le type d'entité
            var activities = await Activity.GetAllActivitiesForDrawerControl(entityId, entityType);
            /****failed connectio******/
            bool Testcon = false;
            ActPopup = true;
            var P = Task.Run(() => DbConnection.Connecter3());
            Testcon = await P;
            if (Testcon == false)
            {


                TestLoad = true;
                //IsBusy = false;

                return;
            }
            TestLoad = false;
            /*********************/
            try
            {
                UserDialogs.Instance.ShowLoading("Chargement...");

                if (activities != null && activities.Any())
                {
                    // Mettre à jour la collection sur le thread UI
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Activities.Clear();
                        foreach (var activity in activities)
                        {
                            Activities.Add(activity);
                        }
                    });
                }
                else
                {
                    Console.WriteLine("Aucune activité trouvée.");
                }
            }
            catch (Exception ex)
            {
                TestLoad = true;
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
            ActPopup = false;

        }


        //public async Task LoadFilteredActivities()
        //{
        //    int entityId = EntityId;
        //    string entityType = EntityActivityType;
        //    List<Activity> activities;
        //    try
        //    {
        //        UserDialogs.Instance.ShowLoading("Chargement...");
        //        await Task.Delay(400);
        //        switch (SelectedStateActivity?.Name)
        //        {
        //            case "In Progress":
        //                activities = await Activity.GetInProgressActivities(entityId, entityType);
        //                break;
        //            case "Done":
        //                activities = await Activity.GetDoneActivities(entityId, entityType);
        //                break;
        //            case "Cancelled":
        //                activities = await Activity.GetCancelledActivities(entityId, entityType);
        //                break;
        //            default: // "All" ou autre
        //                activities = await Activity.GetAllActivitiesForDrawerControl(entityId, entityType);
        //                break;
        //        }
        //        Device.BeginInvokeOnMainThread(() =>
        //        {
        //            Activities.Clear();
        //            foreach (var activity in activities)
        //            {
        //                Activities.Add(activity);
        //            }
        //        });

        //    }
        //    catch
        //    {
        //        TestLoad = true;

        //    }
        //    finally
        //    {
        //        UserDialogs.Instance.HideLoading();
        //    }
        //}
        public async Task LoadFilteredActivities()
        {
            int entityId = EntityId;
            string entityType = EntityActivityType;
            List<Activity> activities;

            try
            {
                // Afficher immédiatement l'indicateur de chargement
                UserDialogs.Instance.ShowLoading("Chargement...");

                // Charger les activités selon l'état sélectionné
                switch (SelectedStateActivity?.Name)
                {
                    case "In Progress":
                        activities = await Activity.GetInProgressActivities(entityId, entityType);
                        break;
                    case "Done":
                        activities = await Activity.GetDoneActivities(entityId, entityType);
                        break;
                    case "Cancelled":
                        activities = await Activity.GetCancelledActivities(entityId, entityType);
                        break;
                    default: // "All" ou autre
                        activities = await Activity.GetAllActivitiesForDrawerControl(entityId, entityType);
                        break;
                }

                // Mettre à jour l'UI sur le thread principal
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    try
                    {
                        Activities.Clear();
                        foreach (var activity in activities)
                        {
                            Activities.Add(activity);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur lors de la mise à jour de l'interface : {ex.Message}");
                        TestLoad = true;
                    }
                });
            }
            catch (Exception ex)  // Gestion spécifique des erreurs
            {
                TestLoad = true;
                Console.WriteLine($"Erreur lors du chargement des activités : {ex.Message}");

                // Afficher une alerte à l'utilisateur
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur",
                        "Impossible de charger les activités", "OK");
                });
            }
            finally
            {
                // Masquer l'indicateur de chargement dans tous les cas
                UserDialogs.Instance.HideLoading();
            }
        }
        private async Task SaveActivityAsync()
        {
            if (SelectedEmployee == null)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Veuillez sélectionner un employé", "OK");
                return;
            }

            UserDialogs.Instance.ShowLoading("Enregistrement...");

            try
            {
                // Obtenir la position GPS actuelle
                var location = await GetCurrentLocation();
                string gpsCoordinates = null;

                if (location != null)
                {
                    // Format avec point comme séparateur décimal
                    gpsCoordinates = FormattableString.Invariant($"{location.Latitude},{location.Longitude}");
                }
                int employeeId = await Activity.GetEmployeeIdByUserId(userId);

                var activity = new Activity
                {
                    CreateDate = DateTime.Now,
                    Summary = Summary,
                    DueDate = DueDate,
                    Memo = ActivityMemo,
                    Type = SelectedActivityType.Id,
                    AssignedEmployee = SelectedEmployee.Id,
                    Author = employeeId,
                    State = 1,
                    ObjectType = EntityActivityType,
                    Object = EntityId,
                    Date = DateTime.Now,
                    Form = EntityFormType,
                    Gps = gpsCoordinates // Ajout des coordonnées GPS
                };

                bool isSaved = await Activity.SaveactivityToDatabase(activity);

                UserDialogs.Instance.HideLoading();

                if (isSaved)
                {
                    // Reset du formulaire
                    Summary = string.Empty;
                    ActivityMemo = string.Empty;
                    DueDate = DateTime.Now;
                    SelectedEmployee = null;
                    SelectedActivityType = null;
                    IsActivityFormVisible = false;
                    await LoadActivities();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur", "Échec de l'enregistrement", "OK");
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                await Application.Current.MainPage.DisplayAlert("Erreur", $"Erreur lors de l'enregistrement: {ex.Message}", "OK");
            }
        }

        private async Task<Location> GetCurrentLocation()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        return null;
                    }
                }

                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

                return location;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GPS: {ex.Message}");
                return null;
            }
        }
        /***********************************/

        // Mise à jour du texte de l'en-tête selon l'index de la vue
        private void UpdateHeaderText()
        {
            HeaderText = CurrentViewIndex switch
            {
                0 => "Memo",
                1 => "Activity",
                2 => "Chat",
                _ => "Memo"
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}