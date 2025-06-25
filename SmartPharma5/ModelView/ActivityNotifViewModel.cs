using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Acr.UserDialogs;
using MvvmHelpers;
using SmartPharma5.Model;
using static SmartPharma5.Model.Activity;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace SmartPharma5.ModelView
{
    public class ActivityNotifViewModel : INotifyPropertyChanged
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
        private SmartPharma5.Model.Activity.ActivityState _selectedStateActivity;
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
        private bool _isLateChecked;
        private bool _isTodayChecked;
        private bool _isTodayFilterActive;
        private bool _shouldUseTotalActivities;
        private bool _isComboBoxEnabled = true;
        private bool _isFutureChecked;
        private bool _isDoneFormVisible;
        private bool _showDoneMessage;
        private string _parentObjectDisplay;
        private int? _parentObject;
        private string _parentObjectType;
        private bool _isIndividualStateChange = false; // Variable pour distinguer les changements d'état individuels
        private bool _isLoadingFromFilter = false; // Variable pour désactiver l'événement pendant le chargement des filtres
        private string _entityFormType;
        private string _formDisplay; // Nouvelle propriété pour l'affichage du formulaire
        private string _previousActivityEmployeeName;

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
        public ICommand FilterTodayCommand { get; }


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
                    OnPropertyChanged(nameof(AreSwitchesEnabled)); // Notifier le changement de l'état des switches

                    // Appliquer le filtrage combiné seulement si "In Progress" est sélectionné ET que des switches sont actifs
                    if (value?.Name == "In Progress" && (IsLateChecked || IsTodayChecked || IsFutureChecked))
                    {
                        Task.Run(async () => await ApplyCombinedFilter());
                    }
                    else if (value != null)
                    {
                        // Sinon, utiliser le filtre normal par état (ignorer les switches)
                        Task.Run(async () => await LoadFilteredAllActivities());
                    }
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
        public string EntityFormType
        {
            get => _entityFormType;
            set
            {
                _entityFormType = value;
                OnPropertyChanged(nameof(EntityFormType));
            }
        }

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
                if (_selectedState != value)
                {
                    _selectedState = value;
                    OnPropertyChanged(nameof(SelectedState));

                    // Si l'état est changé vers "Done" ET que c'est un changement individuel, afficher le formulaire Done
                    if (value == "Done" && _isIndividualStateChange)
                    {
                        ShowDoneMessage = true; // Afficher le formulaire Done
                        IsActivityFormVisible = false; // Cacher l'autre formulaire
                        if (SelectedActivity != null)
                        {
                            SelectedActivity.State = 2; // État "Done"
                        }
                    }
                    else
                    {
                        //IsDoneFormVisible = false;
                        if (SelectedActivity != null)
                        {
                            SelectedActivity.State = value == "In Progress" ? 1 :
                                                   value == "Cancelled" ? 3 : 1;
                        }
                    }
                    
                    // Réinitialiser le flag après utilisation
                    _isIndividualStateChange = false;
                }
            }
        }

        public bool IsComboBoxEnabled
        {
            get => _isComboBoxEnabled;
            set
            {
                if (_isComboBoxEnabled != value)
                {
                    _isComboBoxEnabled = value;
                    OnPropertyChanged(nameof(IsComboBoxEnabled));
                    if (!value)
                    {
                        SelectedStateActivity = null;
                    }
                }
            }
        }
        
        public bool IsComboBoxReadOnly
        {
            get => false; // Le ComboBox doit toujours être activé
        }
        
        // Nouvelle propriété pour l'état des switches
        public bool AreSwitchesEnabled
        {
            get => SelectedStateActivity?.Name == "In Progress"; // Activer les switches seulement si "In Progress" est sélectionné
        }

        public bool IsTodayFilterActive
        {
            get => _isTodayFilterActive;
            set => SetProperty(ref _isTodayFilterActive, value);
        }

        public bool ShouldUseTotalActivities
        {
            get => _shouldUseTotalActivities;
            set => SetProperty(ref _shouldUseTotalActivities, value);
        }

        public bool IsFutureChecked
        {
            get => _isFutureChecked;
            set
            {
                if (_isFutureChecked != value)
                {
                    _isFutureChecked = value;
                    OnPropertyChanged(nameof(IsFutureChecked));
                    OnPropertyChanged(nameof(AreSwitchesEnabled));
                    
                    // Ne pas déclencher de chargement pendant l'initialisation
                    if (!_isLoadingFromFilter)
                    {
                        // Appliquer le filtrage combiné seulement si "In Progress" est sélectionné
                        if (SelectedStateActivity?.Name == "In Progress")
                        {
                            if (value)
                            {
                                Task.Run(async () => await ApplyCombinedFilter());
                            }
                            else if (IsLateChecked || IsTodayChecked)
                            {
                                Task.Run(async () => await ApplyCombinedFilter());
                            }
                            else
                            {
                                // Si aucune checkbox n'est cochée, utiliser le filtre par état
                                Task.Run(async () => await LoadFilteredAllActivities());
                            }
                        }
                        else
                        {
                            // Si ce n'est pas "In Progress", ignorer les switches et utiliser le filtre normal
                            Task.Run(async () => await LoadFilteredAllActivities());
                        }
                    }
                }
            }
        }

        //public bool IsDoneFormVisible
        //{
        //    get => _isDoneFormVisible;
        //    set
        //    {
        //        if (_isDoneFormVisible != value)
        //        {
        //            _isDoneFormVisible = value;
        //            OnPropertyChanged(nameof(IsDoneFormVisible));
        //        }
        //    }
        //}

        public bool ShowDoneMessage
        {
            get => _showDoneMessage;
            set
            {
                _showDoneMessage = value;
                OnPropertyChanged(nameof(ShowDoneMessage));
            }
        }

        public string ParentObjectDisplay
        {
            get => _parentObjectDisplay;
            set
            {
                _parentObjectDisplay = value;
                OnPropertyChanged();
            }
        }

        public int? ParentObject
        {
            get => _parentObject;
            set
            {
                _parentObject = value;
                OnPropertyChanged();
            }
        }

        public string ParentObjectType
        {
            get => _parentObjectType;
            set
            {
                _parentObjectType = value;
                OnPropertyChanged();
            }
        }

        public string FormDisplay
        {
            get => _formDisplay;
            set
            {
                _formDisplay = value;
                OnPropertyChanged();
            }
        }

        public string PreviousActivityEmployeeName
        {
            get => _previousActivityEmployeeName;
            set => SetProperty(ref _previousActivityEmployeeName, value);
        }

        // Propriété pour réinitialiser le flag de changement d'état individuel
        public void ResetIndividualStateChangeFlag()
        {
            _isIndividualStateChange = false;
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
        public ActivityNotifViewModel(int entityId, string entityType, string entityActivityType)
        {
            LoadEmployeesAsync();
            if (CurrentActivity?.AssignedEmployee > 0 && Employees != null)
            {
                SelectedEmployee = Employees.FirstOrDefault(e => e.Id == CurrentActivity.AssignedEmployee);
            }
            EntityId = entityId;
            EntityType = entityType;
            EntityActivityType = entityActivityType;
            // EntityFormType sera défini par PrefillNewActivityFromCompletedActivity
            AddActivityCommand = new Command(async () => await SaveActivityAsync());
            ActivityTypes = new ObservableCollection<ActivityType>();
            Activities = new ObservableCollection<Activity>();
            SelectedStateActivity = null;
            _isTodayChecked = true;
            OnPropertyChanged(nameof(IsTodayChecked));
            Task.Run(async () => await FilterTodayActivities());
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
            // S'abonner à l'événement
            Activity.ActivityStateChanged += OnActivityStateChanged;
        }


        public ActivityNotifViewModel()
        {
            LoadEmployeesAsync();
            if (CurrentActivity?.AssignedEmployee > 0 && Employees != null)
            {
                SelectedEmployee = Employees.FirstOrDefault(e => e.Id == CurrentActivity.AssignedEmployee);
            }
            EntityId = CurrentData.CurrentModuleId;
            EntityType = CurrentData.CurrentNoteModule;
            EntityActivityType = CurrentData.CurrentActivityModule;
            // EntityFormType sera défini par PrefillNewActivityFromCompletedActivity
            AddActivityCommand = new Command(async () => await SaveActivityAsync());
            Activities = new ObservableCollection<Activity>();
            
            // Désactiver les événements pendant l'initialisation
            _isLoadingFromFilter = true;
            
            // Initialiser les switches
            _isLateChecked = true;
            _isTodayChecked = true;
            _isFutureChecked = false;
            
            // Définir l'état par défaut avant d'activer les événements
            SelectedStateActivity = Activity.ActivityState.selectStates.FirstOrDefault(s => s.Name == "In Progress");
            
            // Réactiver les événements
            _isLoadingFromFilter = false;
            
            // Notifier les changements après avoir réactivé les événements
            OnPropertyChanged(nameof(IsLateChecked));
            OnPropertyChanged(nameof(IsTodayChecked));
            OnPropertyChanged(nameof(IsFutureChecked));
            OnPropertyChanged(nameof(AreSwitchesEnabled)); // Notifier l'état des switches
            
            // Charger les activités initiales
            Task.Run(async () => 
            {
                await LoadActivityTypesAsync();
                await ApplyCombinedFilter();
            });
            
            NavigateToActivityCommand = new Command(() => AllLoadActivityView());
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
            FilterTodayCommand = new Command(async () =>
            {
                try
                {
                    IsTodayFilterActive = !IsTodayFilterActive;
                    await LoadFilteredAllActivities();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in FilterTodayCommand: {ex.Message}");
                    await Application.Current.MainPage.DisplayAlert("Erreur",
                        "Impossible de filtrer les activités", "OK");
                }
            });
            // S'abonner à l'événement
            Activity.ActivityStateChanged += OnActivityStateChanged;
        }
        private async Task FilterTodayActivities()
        {
            try
            {
                _isLoadingFromFilter = true; // Désactiver l'événement pendant le chargement
                UserDialogs.Instance.ShowLoading("Chargement...");
                int userId = Preferences.Get("iduser", 0);
                List<Activity> activities;

                if (IsTodayChecked)
                {
                    activities = await Activity.GetInProgressAllActivitiesToday(userId);
                }
                else
                {
                    // Recharger les activités normales
                    await LoadFilteredAllActivities();
                    return;
                }

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    Activities.Clear();
                    foreach (var activity in activities)
                    {
                        Activities.Add(activity);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du filtrage des activités d'aujourd'hui : {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Erreur",
                    "Impossible de filtrer les activités d'aujourd'hui", "OK");
            }
            finally
            {
                _isLoadingFromFilter = false; // Réactiver l'événement après le chargement
                UserDialogs.Instance.HideLoading();
            }
        }
        public async Task LoadActivityTypesAsync()
        {
            var types = await ActivityType.GetActivityTypes(); 
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
        private void AllLoadActivityView()
        {

            Task.Run(async () => await LoadAllActivities());
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
            _isIndividualStateChange = false; // Réinitialiser le flag
            //IsDoneFormVisible = false;
            // Réinitialiser l'état à "In Progress" si on annule
            if (SelectedActivity != null)
            {
                SelectedActivity.State = 1;
                OnPropertyChanged(nameof(SelectedActivity));
            }
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
        private void OnEditActivityClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            if (button != null && button.CommandParameter is int activityId)
            {
                SelectedActivity = Activities.FirstOrDefault(a => a.Id == activityId);
                if (SelectedActivity != null)
                {
                    LoadEmployeeForSelectedActivity(); // Charge l'employé actuel
                    _isIndividualStateChange = true; // Marquer que les changements d'état suivants sont individuels
                }
            }
        }
        
        private async void OnEditActivityClicked(int activityId)
        {
            SelectedActivity = Activities.FirstOrDefault(a => a.Id == activityId);
            LoadEmployeeForSelectedActivity();
            _isIndividualStateChange = true; // Marquer que les changements d'état suivants sont individuels
            //IsDoneFormVisible = true;
            IsActivityFormVisible = true;
        }
        
        //private async Task NavigateToMemoAsync()
        //{
        //    await LoadMemos();
        //    CurrentViewIndex = 0;
        //}

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

                // Si l'état est "Done", mettre à jour la date de fin
                if (SelectedActivity.State == 2)
                {
                    SelectedActivity.DoneDate = DateTime.Now;
                }

                bool isUpdated = await Activity.UpdateAllActivity(SelectedActivity);

                if (isUpdated)
                {
                    IsActivityFormVisible = false;
                    _isIndividualStateChange = false; // Réinitialiser le flag
                    //IsDoneFormVisible = false;
                    // Rafraîchir la liste selon les filtres actifs
                    await ReloadCurrentInterface();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to update activity.", "OK");
                }
            }
            UserDialogs.Instance.HideLoading();
        }

        private void LoadEmployeeForSelectedActivity()
        {
            if (SelectedActivity != null && Employees != null)
            {
                SelectedActivityEmployee = Employees.FirstOrDefault(e => e.Id == SelectedActivity.AssignedEmployee);
            }
        }

        public async Task LoadAllActivities()
        {
            int userId = Preferences.Get("iduser", 0);
            var activities = await Activity.GetAllActivities(userId);
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
                _isLoadingFromFilter = true;
                UserDialogs.Instance.ShowLoading("Chargement...");

                if (activities != null && activities.Any())
                {
                    // Trier par état (In Progress, Done, Cancelled) puis par date
                    var sortedActivities = activities
                        .OrderBy(a => a.State) // 1=In Progress, 2=Done, 3=Cancelled
                        .ThenBy(a => a.DueDate)
                        .ToList();

                    // Mettre à jour la collection sur le thread UI
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Activities.Clear();
                        foreach (var activity in sortedActivities)
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
                _isLoadingFromFilter = false;
                UserDialogs.Instance.HideLoading();
            }
            ActPopup = false;

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
                _isLoadingFromFilter = true;
                UserDialogs.Instance.ShowLoading("Chargement...");

                if (activities != null && activities.Any())
                {
                    // Trier par état (In Progress, Done, Cancelled) puis par date
                    var sortedActivities = activities
                        .OrderBy(a => a.State) // 1=In Progress, 2=Done, 3=Cancelled
                        .ThenBy(a => a.DueDate)
                        .ToList();

                    // Mettre à jour la collection sur le thread UI
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Activities.Clear();
                        foreach (var activity in sortedActivities)
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
                _isLoadingFromFilter = false;
                UserDialogs.Instance.HideLoading();
            }
            ActPopup = false;

        }


        public async Task LoadFilteredAllActivities()
        {
            int userId = Preferences.Get("iduser", 0);
            List<Activity> activities;

            try
            {
                _isLoadingFromFilter = true; // Désactiver l'événement pendant le chargement
                UserDialogs.Instance.ShowLoading("Chargement...");

                if (IsTodayFilterActive)
                {
                    activities = await Activity.GetInProgressAllActivitiesToday(userId);
                }
                else
                {
                    // Sinon, on utilise le filtre normal basé sur l'état
                    switch (SelectedStateActivity?.Name)
                    {
                        case "In Progress":
                            activities = await Activity.GetInProgressAllActivities(userId);
                            break;
                        case "Done":
                            activities = await Activity.GetDoneAllActivities(userId);
                            break;
                        case "Cancelled":
                            activities = await Activity.GetCancelledAllActivities(userId);
                            break;
                        default: // "All" ou autre
                            activities = await Activity.GetAllActivities(userId);
                            break;
                    }
                }

                // Trier les activités selon l'état sélectionné
                if (SelectedStateActivity?.Name == "All")
                {
                    // Pour "All", trier par état (In Progress, Done, Cancelled) puis par date
                    activities = activities
                        .OrderBy(a => a.State) // 1=In Progress, 2=Done, 3=Cancelled
                        .ThenBy(a => a.DueDate)
                        .ToList();
                }
                else
                {
                    // Pour les autres états, trier seulement par date d'échéance
                    activities = activities
                        .OrderBy(a => a.DueDate)
                        .ToList();
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
                        Console.WriteLine($"Error updating UI: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                TestLoad = true;
                Console.WriteLine($"Error loading activities: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Erreur",
                    "Impossible de charger les activités", "OK");
            }
            finally
            {
                _isLoadingFromFilter = false; // Réactiver l'événement après le chargement
                UserDialogs.Instance.HideLoading();
            }
        }
        public async Task LoadFilteredActivities()
        {
            int entityId = EntityId;
            string entityType = EntityActivityType;
            List<Activity> activities;
            try
            {
                _isLoadingFromFilter = true;
                UserDialogs.Instance.ShowLoading("Chargement...");
                //await Task.Delay(400);
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

                // Trier les activités selon l'état sélectionné
                if (SelectedStateActivity?.Name == "All")
                {
                    // Pour "All", trier par état (In Progress, Done, Cancelled) puis par date
                    activities = activities
                        .OrderBy(a => a.State) // 1=In Progress, 2=Done, 3=Cancelled
                        .ThenBy(a => a.DueDate)
                        .ToList();
                }
                else
                {
                    // Pour les autres états, trier seulement par date d'échéance
                    activities = activities
                        .OrderBy(a => a.DueDate)
                        .ToList();
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    Activities.Clear();
                    foreach (var activity in activities)
                    {
                        Activities.Add(activity);
                    }
                });

            }
            catch
            {
                TestLoad = true;

            }
            finally
            {
                _isLoadingFromFilter = false;
                UserDialogs.Instance.HideLoading();
            }
        }
        private async Task SaveActivityAsync()
        {
            // Vérification des champs obligatoires
            if (SelectedEmployee == null)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Veuillez sélectionner un employé", "OK");
                return;
            }

            if (SelectedActivityType == null)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Veuillez sélectionner un type d'activité", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Summary))
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Veuillez entrer un résumé", "OK");
                return;
            }

            // Vérification que EntityFormType est défini (doit venir de l'activité terminée)
            if (string.IsNullOrEmpty(EntityFormType))
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Le type de formulaire n'est pas défini. Veuillez d'abord terminer une activité.", "OK");
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
                if (employeeId <= 0)
                {
                    throw new Exception("Impossible de récupérer l'ID de l'employé");
                }

                var activity = new Activity
                {
                    CreateDate = DateTime.Now,
                    Summary = Summary?.Trim(),
                    DueDate = DueDate,
                    Memo = ActivityMemo?.Trim(),
                    Type = SelectedActivityType?.Id ?? 0,
                    AssignedEmployee = SelectedEmployee?.Id ?? 0,
                    Author = employeeId,
                    State = 1,
                    ObjectType = ParentObjectType,
                    Object = ParentObject,
                    Date = DateTime.Now,
                    Form = EntityFormType,
                    Gps = gpsCoordinates
                };

                // Vérification finale des données avant sauvegarde
                if (activity.Type <= 0 || activity.AssignedEmployee <= 0)
                {
                    throw new Exception("Données d'activité invalides");
                }

                // Vérification que Form n'est pas null
                if (string.IsNullOrEmpty(activity.Form))
                {
                    throw new Exception("Le type de formulaire ne peut pas être vide");
                }

                Console.WriteLine($"Sauvegarde de l'activité avec Form: {activity.Form}");

                bool isSaved = await Activity.SaveAllactivityToDatabase(activity);

                UserDialogs.Instance.HideLoading();

                if (isSaved)
                {
                    // Reset du formulaire
                    Summary = string.Empty;
                    ActivityMemo = string.Empty;
                    DueDate = DateTime.Now;
                    SelectedEmployee = null;
                    SelectedActivityType = null;
                    ParentObjectDisplay = string.Empty;
                    ParentObject = null;
                    ParentObjectType = null;
                    EntityFormType = string.Empty;
                    FormDisplay = string.Empty;

                    // Fermer automatiquement le popup
                    IsActivityFormVisible = false;
                    ShowDoneMessage = false;

                    // Recharger l'interface selon les filtres actifs
                    await ReloadCurrentInterface();

                    // Afficher un message de confirmation
                    await Application.Current.MainPage.DisplayAlert("Succès", "Activité enregistrée avec succès !", "OK");
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

        // Nouvelle méthode pour recharger l'interface selon les filtres actifs
        private async Task ReloadCurrentInterface()
        {
            try
            {
                // Vérifier les filtres actifs et recharger en conséquence
                if (IsLateChecked || IsTodayChecked || IsFutureChecked)
                {
                    // Si des checkboxes de filtres sont cochées, utiliser le filtre combiné
                    await ApplyCombinedFilter();
                }
                else if (SelectedStateActivity != null)
                {
                    // Si un état est sélectionné dans le ComboBox, utiliser le filtre par état
                    await LoadFilteredAllActivities();
                }
                else
                {
                    // Sinon, recharger toutes les activités
                    await LoadAllActivities();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du rechargement de l'interface : {ex.Message}");
                // En cas d'erreur, essayer de recharger toutes les activités
                await LoadAllActivities();
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

        private async Task FilterLateActivities()
        {
            try
            {
                _isLoadingFromFilter = true; // Désactiver l'événement pendant le chargement
                UserDialogs.Instance.ShowLoading("Chargement...");
                int userId = Preferences.Get("iduser", 0);
                List<Activity> activities;

                if (IsLateChecked)
                {
                    activities = await Activity.GetInProgressAllActivitiesOverDue(userId);
                }
                else
                {
                    // Recharger les activités normales
                    await LoadFilteredAllActivities();
                    return;
                }

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    Activities.Clear();
                    foreach (var activity in activities)
                    {
                        Activities.Add(activity);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du filtrage des activités en retard : {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Erreur",
                    "Impossible de filtrer les activités en retard", "OK");
            }
            finally
            {
                _isLoadingFromFilter = false; // Réactiver l'événement après le chargement
                UserDialogs.Instance.HideLoading();
            }
        }

        private async Task FilterBothActivities()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Chargement...");
                int userId = Preferences.Get("iduser", 0);
                List<Activity> lateActivities = await Activity.GetInProgressAllActivitiesOverDue(userId);
                List<Activity> todayActivities = await Activity.GetInProgressAllActivitiesToday(userId);

                // Combiner les deux listes et supprimer les doublons (si une activité est à la fois en retard et aujourd'hui)
                var combinedActivities = lateActivities.Union(todayActivities, new ActivityComparer())
                                                     .OrderBy(a => a.DueDate)
                                                     .ToList();

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    Activities.Clear();
                    foreach (var activity in combinedActivities)
                    {
                        Activities.Add(activity);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du filtrage des activités : {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Erreur",
                    "Impossible de filtrer les activités", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async Task FilterFutureActivities()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Chargement...");
                int userId = Preferences.Get("iduser", 0);
                List<Activity> activities = await Activity.GetInProgressAllActivitiesFuture(userId);

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    Activities.Clear();
                    foreach (var activity in activities)
                    {
                        Activities.Add(activity);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du filtrage des activités futures : {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Erreur",
                    "Impossible de filtrer les activités futures", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        public async Task FilterAllActivities()
        {
            try
            {
                _isLoadingFromFilter = true; // Désactiver l'événement pendant le chargement
                UserDialogs.Instance.ShowLoading("Chargement...");
                int userId = Preferences.Get("iduser", 0);
                List<Activity> allActivities = new List<Activity>();

                if (IsLateChecked)
                {
                    var lateActivities = await Activity.GetInProgressAllActivitiesOverDue(userId);
                    allActivities.AddRange(lateActivities);
                }
                if (IsTodayChecked)
                {
                    var todayActivities = await Activity.GetInProgressAllActivitiesToday(userId);
                    allActivities.AddRange(todayActivities);
                }
                if (IsFutureChecked)
                {
                    var futureActivities = await Activity.GetInProgressAllActivitiesFuture(userId);
                    allActivities.AddRange(futureActivities);
                }

                // Supprimer les doublons et trier par date d'échéance
                var combinedActivities = allActivities.Distinct(new ActivityComparer())
                                                    .OrderBy(a => a.DueDate)
                                                    .ToList();

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    Activities.Clear();
                    foreach (var activity in combinedActivities)
                    {
                        Activities.Add(activity);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du filtrage des activités : {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Erreur",
                    "Impossible de filtrer les activités", "OK");
            }
            finally
            {
                _isLoadingFromFilter = false; // Réactiver l'événement après le chargement
                UserDialogs.Instance.HideLoading();
            }
        }

        // Classe pour comparer les activités et supprimer les doublons
        private class ActivityComparer : IEqualityComparer<Activity>
        {
            public bool Equals(Activity x, Activity y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(Activity obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        public bool IsLateChecked
        {
            get => _isLateChecked;
            set
            {
                if (_isLateChecked != value)
                {
                    _isLateChecked = value;
                    OnPropertyChanged(nameof(IsLateChecked));
                    OnPropertyChanged(nameof(AreSwitchesEnabled));
                    
                    // Ne pas déclencher de chargement pendant l'initialisation
                    if (!_isLoadingFromFilter)
                    {
                        // Appliquer le filtrage combiné seulement si "In Progress" est sélectionné
                        if (SelectedStateActivity?.Name == "In Progress")
                        {
                            if (value)
                            {
                                Task.Run(async () => await ApplyCombinedFilter());
                            }
                            else if (IsTodayChecked || IsFutureChecked)
                            {
                                Task.Run(async () => await ApplyCombinedFilter());
                            }
                            else
                            {
                                // Si aucune checkbox n'est cochée, utiliser le filtre par état
                                Task.Run(async () => await LoadFilteredAllActivities());
                            }
                        }
                        else
                        {
                            // Si ce n'est pas "In Progress", ignorer les switches et utiliser le filtre normal
                            Task.Run(async () => await LoadFilteredAllActivities());
                        }
                    }
                }
            }
        }

        public bool IsTodayChecked
        {
            get => _isTodayChecked;
            set
            {
                if (_isTodayChecked != value)
                {
                    _isTodayChecked = value;
                    OnPropertyChanged(nameof(IsTodayChecked));
                    OnPropertyChanged(nameof(AreSwitchesEnabled));
                    
                    // Ne pas déclencher de chargement pendant l'initialisation
                    if (!_isLoadingFromFilter)
                    {
                        // Appliquer le filtrage combiné seulement si "In Progress" est sélectionné
                        if (SelectedStateActivity?.Name == "In Progress")
                        {
                            if (value)
                            {
                                Task.Run(async () => await ApplyCombinedFilter());
                            }
                            else if (IsLateChecked || IsFutureChecked)
                            {
                                Task.Run(async () => await ApplyCombinedFilter());
                            }
                            else
                            {
                                // Si aucune checkbox n'est cochée, utiliser le filtre par état
                                Task.Run(async () => await LoadFilteredAllActivities());
                            }
                        }
                        else
                        {
                            // Si ce n'est pas "In Progress", ignorer les switches et utiliser le filtre normal
                            Task.Run(async () => await LoadFilteredAllActivities());
                        }
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void OnSaveDoneActivityClicked()
        {
            UserDialogs.Instance.ShowLoading("Loading...");

            if (SelectedActivity != null)
            {
                // Mettre à jour l'activité
                SelectedActivity.State = 2; // État "Done"
                SelectedActivity.DoneDate = DateTime.Now;

                bool isUpdated = await Activity.UpdateAllActivity(SelectedActivity);

                if (isUpdated)
                {
                    //IsDoneFormVisible = false;
                    // Rafraîchir la liste selon les filtres actifs
                    await ReloadCurrentInterface();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to update activity.", "OK");
                }
            }
            UserDialogs.Instance.HideLoading();
        }

        private void OnCancelDoneActivityClicked()
        {
            //IsDoneFormVisible = false;
            // Réinitialiser l'état à "In Progress"
            if (SelectedActivity != null)
            {
                SelectedActivity.State = 1;
                OnPropertyChanged(nameof(SelectedActivity));
            }
        }

        private void OnActivityStateChanged(object sender, Activity activity)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Ne pas déclencher l'affichage du formulaire si on charge depuis un filtre
                if (_isLoadingFromFilter)
                    return;

                // Vérifier si l'activité est actuellement sélectionnée (c'est-à-dire qu'on a cliqué sur le bouton d'édition)
                // et que l'état est "Done"
                if (activity.State == 2)
                {
                    // Définir l'activité comme sélectionnée si elle ne l'est pas déjà
                    if (SelectedActivity == null || SelectedActivity.Id != activity.Id)
                    {
                        SelectedActivity = activity;
                    }
                    ShowDoneMessage = true;
                }
            });
        }

        public void SetParentObject(Activity parentActivity)
        {
            if (parentActivity != null)
            {
                ParentObject = parentActivity.Object;
                ParentObjectType = parentActivity.ObjectType;
                ParentObjectDisplay = $"{parentActivity.PieceAcronym}/{parentActivity.PieceCode}";
                FormDisplay = parentActivity.Form; // Ajouter l'affichage du formulaire
            }
        }

        public async Task PrefillNewActivityFromCompletedActivity(Activity completedActivity)
        {
            try
            {
                // Pré-remplir les informations de base
                ParentObject = completedActivity.Object;
                ParentObjectType = completedActivity.ObjectType;
                ParentObjectDisplay = $"{completedActivity.PieceAcronym}/{completedActivity.PieceCode}";
                
                // Utiliser directement le Form de l'activité terminée
                EntityFormType = completedActivity.Form;
                FormDisplay = completedActivity.Form; // Affichage du formulaire
                Console.WriteLine($"EntityFormType défini depuis l'activité : {EntityFormType}");

                // Pré-remplir l'employé assigné
                if (completedActivity.AssignedEmployee > 0 && Employees != null)
                {
                    SelectedEmployee = Employees.FirstOrDefault(e => e.Id == completedActivity.AssignedEmployee);
                }

                // Pré-remplir le type d'activité (optionnel - tu peux le laisser vide pour que l'utilisateur choisisse)
                // Si tu veux pré-remplir le type d'activité, décommente la ligne suivante :
                // SelectedActivityType = ActivityTypes.FirstOrDefault(t => t.Id == completedActivity.Type);

                // Réinitialiser les champs de saisie avec des valeurs par défaut
                Summary = string.Empty;
                ActivityMemo = string.Empty;
                DueDate = DateTime.Now.AddDays(1); // Date d'échéance par défaut : demain

                // Notifier tous les changements pour que l'UI se mette à jour
                OnPropertyChanged(nameof(ParentObject));
                OnPropertyChanged(nameof(ParentObjectType));
                OnPropertyChanged(nameof(ParentObjectDisplay));
                OnPropertyChanged(nameof(EntityFormType));
                OnPropertyChanged(nameof(FormDisplay));
                OnPropertyChanged(nameof(SelectedEmployee));
                OnPropertyChanged(nameof(SelectedActivityType));
                OnPropertyChanged(nameof(Summary));
                OnPropertyChanged(nameof(ActivityMemo));
                OnPropertyChanged(nameof(DueDate));

                Console.WriteLine($"Pré-remplissage terminé : Objet={ParentObject}, Type={ParentObjectType}, Form={EntityFormType}, Employé={SelectedEmployee?.NameEmployee}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du pré-remplissage : {ex.Message}");
            }
        }

        // Nouvelle méthode pour appliquer le filtrage combiné
        private async Task ApplyCombinedFilter()
        {
            try
            {
                _isLoadingFromFilter = true;
                UserDialogs.Instance.ShowLoading("Chargement...");
                int userId = Preferences.Get("iduser", 0);
                List<Activity> allActivities = new List<Activity>();

                // Déterminer l'état à filtrer
                string targetState = SelectedStateActivity?.Name ?? "In Progress";

                // Appliquer les filtres de date selon les checkboxes cochées
                if (IsLateChecked)
                {
                    var lateActivities = await GetActivitiesByStateAndDate(userId, targetState, "overdue");
                    allActivities.AddRange(lateActivities);
                }
                if (IsTodayChecked)
                {
                    var todayActivities = await GetActivitiesByStateAndDate(userId, targetState, "today");
                    allActivities.AddRange(todayActivities);
                }
                if (IsFutureChecked)
                {
                    var futureActivities = await GetActivitiesByStateAndDate(userId, targetState, "future");
                    allActivities.AddRange(futureActivities);
                }

                // Supprimer les doublons
                var combinedActivities = allActivities.Distinct(new ActivityComparer()).ToList();

                // Trier les activités
                if (targetState == "All")
                {
                    // Pour "All", trier par état (In Progress, Done, Cancelled) puis par date
                    combinedActivities = combinedActivities
                        .OrderBy(a => a.State) // 1=In Progress, 2=Done, 3=Cancelled
                        .ThenBy(a => a.DueDate)
                        .ToList();
                }
                else
                {
                    // Pour les autres états, trier seulement par date d'échéance
                    combinedActivities = combinedActivities
                        .OrderBy(a => a.DueDate)
                        .ToList();
                }

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    Activities.Clear();
                    foreach (var activity in combinedActivities)
                    {
                        Activities.Add(activity);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du filtrage combiné : {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Erreur",
                    "Impossible de filtrer les activités", "OK");
            }
            finally
            {
                _isLoadingFromFilter = false;
                UserDialogs.Instance.HideLoading();
            }
        }

        // Méthode helper pour obtenir les activités par état et période
        private async Task<List<Activity>> GetActivitiesByStateAndDate(int userId, string state, string dateFilter)
        {
            switch (state)
            {
                case "In Progress":
                    switch (dateFilter)
                    {
                        case "overdue":
                            return await Activity.GetInProgressAllActivitiesOverDue(userId);
                        case "today":
                            return await Activity.GetInProgressAllActivitiesToday(userId);
                        case "future":
                            return await Activity.GetInProgressAllActivitiesFuture(userId);
                        default:
                            return await Activity.GetInProgressAllActivities(userId);
                    }
                case "Done":
                    switch (dateFilter)
                    {
                        case "overdue":
                            return await Activity.GetDoneAllActivities(userId).ContinueWith(t => 
                                t.Result.Where(a => a.DueDate < DateTime.Today).ToList());
                        case "today":
                            return await Activity.GetDoneAllActivities(userId).ContinueWith(t => 
                                t.Result.Where(a => a.DueDate.Date == DateTime.Today).ToList());
                        case "future":
                            return await Activity.GetDoneAllActivities(userId).ContinueWith(t => 
                                t.Result.Where(a => a.DueDate > DateTime.Today).ToList());
                        default:
                            return await Activity.GetDoneAllActivities(userId);
                    }
                case "Cancelled":
                    switch (dateFilter)
                    {
                        case "overdue":
                            return await Activity.GetCancelledAllActivities(userId).ContinueWith(t => 
                                t.Result.Where(a => a.DueDate < DateTime.Today).ToList());
                        case "today":
                            return await Activity.GetCancelledAllActivities(userId).ContinueWith(t => 
                                t.Result.Where(a => a.DueDate.Date == DateTime.Today).ToList());
                        case "future":
                            return await Activity.GetCancelledAllActivities(userId).ContinueWith(t => 
                                t.Result.Where(a => a.DueDate > DateTime.Today).ToList());
                        default:
                            return await Activity.GetCancelledAllActivities(userId);
                    }
                default: // "All"
                    switch (dateFilter)
                    {
                        case "overdue":
                            return await Activity.GetAllActivities(userId).ContinueWith(t => 
                                t.Result.Where(a => a.DueDate < DateTime.Today).ToList());
                        case "today":
                            return await Activity.GetAllActivities(userId).ContinueWith(t => 
                                t.Result.Where(a => a.DueDate.Date == DateTime.Today).ToList());
                        case "future":
                            return await Activity.GetAllActivities(userId).ContinueWith(t => 
                                t.Result.Where(a => a.DueDate > DateTime.Today).ToList());
                        default:
                            return await Activity.GetAllActivities(userId);
                    }
            }
        }
    }

}
