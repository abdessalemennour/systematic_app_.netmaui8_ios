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
    public class ActivityViewModel : INotifyPropertyChanged, IDisposable
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
        private bool _isLateChecked;
        private bool _isTodayChecked;
        private bool _isFutureChecked;
        private bool _isComboBoxEnabled = true;
        private bool _showDoneMessage;
        private bool _isIndividualStateChange = false; // Variable pour distinguer les changements d'état individuels
        private bool _isLoadingFromFilter = false; // Variable pour désactiver l'événement pendant le chargement des filtres
        private string _parentObjectDisplay;
        private int? _parentObject;
        private string _parentObjectType;
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
                    
                    // Ne pas déclencher de chargement pendant l'initialisation
                    if (!_isLoadingFromFilter)
                    {
                        // Marquer que c'est un changement de filtre global (pas individuel)
                        _isLoadingFromFilter = true;
                        
                        // Appliquer le filtrage combiné seulement si "In Progress" est sélectionné ET que des switches sont actifs
                        if (value?.Name == "In Progress" && (IsLateChecked || IsTodayChecked || IsFutureChecked))
                        {
                            Task.Run(async () => 
                            {
                                await ApplyCombinedFilter();
                                _isLoadingFromFilter = false;
                            });
                        }
                        else if (value != null)
                        {
                            // Sinon, utiliser le filtre normal par état (ignorer les switches)
                            Task.Run(async () => 
                            {
                                await LoadFilteredActivities();
                                _isLoadingFromFilter = false;
                            });
                        }
                        else
                        {
                            _isLoadingFromFilter = false;
                        }
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
                                Task.Run(async () => await LoadFilteredActivities());
                            }
                        }
                        else
                        {
                            // Si ce n'est pas "In Progress", ignorer les switches et utiliser le filtre normal
                            Task.Run(async () => await LoadFilteredActivities());
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
                                Task.Run(async () => await LoadFilteredActivities());
                            }
                        }
                        else
                        {
                            // Si ce n'est pas "In Progress", ignorer les switches et utiliser le filtre normal
                            Task.Run(async () => await LoadFilteredActivities());
                        }
                    }
                }
            }
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
                                Task.Run(async () => await LoadFilteredActivities());
                            }
                        }
                        else
                        {
                            // Si ce n'est pas "In Progress", ignorer les switches et utiliser le filtre normal
                            Task.Run(async () => await LoadFilteredActivities());
                        }
                    }
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
            get => false; // Permettre toujours la sélection dans le ComboBox
        }

        // Nouvelle propriété pour l'état des switches
        public bool AreSwitchesEnabled
        {
            get => SelectedStateActivity?.Name == "In Progress"; // Activer les switches seulement si "In Progress" est sélectionné
        }

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
            set => SetProperty(ref _formDisplay, value);
        }

        // Propriété pour afficher le nom de l'employé assigné de l'activité précédente
        public string PreviousActivityEmployeeName
        {
            get => _previousActivityEmployeeName;
            set => SetProperty(ref _previousActivityEmployeeName, value);
        }

        // Événement pour notifier la fermeture du popup
        public event EventHandler ActivityAdded;

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
        public ActivityViewModel(int entityId, string entityType, string entityActivityType)
        {
            EntityId = entityId;
            EntityType = entityType;
            EntityActivityType = entityActivityType;
            EntityFormType = CurrentData.CurrentFormModule;
            AddActivityCommand = new Command(async () => await SaveActivityAsync());
            Activities = new ObservableCollection<Activity>();
            
            // Désactiver les événements pendant l'initialisation
            _isLoadingFromFilter = true;
            
            // Initialiser les switches
            IsLateChecked = true;
            IsTodayChecked = true;
            IsFutureChecked = false;
            
            // Définir l'état par défaut avant d'activer les événements
            SelectedStateActivity = Activity.ActivityState.selectStates.FirstOrDefault(s => s.Name == "In Progress");
            
            // Réactiver les événements
            _isLoadingFromFilter = false;
            
            // S'abonner à l'événement de changement d'état des activités
            Activity.ActivityStateChanged += OnActivityStateChanged;

            // Charger les activités initiales
            Task.Run(async () => 
            {
                await LoadActivityTypesAsync();
                await ApplyCombinedFilter();
            });
            
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
            
            // Désactiver les événements pendant l'initialisation
            _isLoadingFromFilter = true;
            
            // Initialiser les switches comme dans ActivityNotifViewModel
            IsLateChecked = true;
            IsTodayChecked = true;
            IsFutureChecked = false;
            
            // Définir l'état par défaut avant d'activer les événements
            SelectedStateActivity = Activity.ActivityState.selectStates.FirstOrDefault(s => s.Name == "In Progress");
            
            // Réactiver les événements
            _isLoadingFromFilter = false;
            
            // Notifier les changements après avoir réactivé les événements
            OnPropertyChanged(nameof(IsLateChecked));
            OnPropertyChanged(nameof(IsTodayChecked));
            OnPropertyChanged(nameof(IsFutureChecked));
            OnPropertyChanged(nameof(AreSwitchesEnabled)); // Notifier l'état des switches
            
            // S'abonner à l'événement de changement d'état des activités
            Activity.ActivityStateChanged += OnActivityStateChanged;

            // Charger les activités initiales
            Task.Run(async () => 
            {
                await LoadActivityTypesAsync();
                await ApplyCombinedFilter();
            });

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
                    IsActivityFormVisible = true;
                }
            }
        }

        private async void OnEditActivityClicked(int activityId)
        {
            SelectedActivity = Activities.FirstOrDefault(a => a.Id == activityId);
            LoadEmployeeForSelectedActivity();
            _isIndividualStateChange = true; // Marquer que les changements d'état suivants sont individuels
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
                    // Recharger les activités selon les filtres actifs
                    if (IsLateChecked || IsTodayChecked || IsFutureChecked)
                    {
                        await ApplyCombinedFilter();
                    }
                    else if (SelectedStateActivity != null)
                    {
                        await LoadFilteredActivities();
                    }
                    else
                    {
                        await LoadActivities();
                    }
                    
                    // Fermer le formulaire
                    IsActivityFormVisible = false;
                    ShowDoneMessage = false;
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
                _isLoadingFromFilter = true;
                UserDialogs.Instance.ShowLoading("Chargement...");

                if (SelectedStateActivity != null)
                {
                    switch (SelectedStateActivity.Name)
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
                    if (SelectedStateActivity.Name == "All")
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
            }
            catch (Exception ex)
            {
                TestLoad = true;
                Console.WriteLine($"Erreur lors du chargement des activités : {ex.Message}");

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur",
                        "Impossible de charger les activités", "OK");
                });
            }
            finally
            {
                _isLoadingFromFilter = false;
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
                    PreviousActivityEmployeeName = string.Empty;
                    
                    // Recharger la liste selon les paramètres de filtrage existants
                    await ReloadCurrentInterface();
                    
                    ActivityAdded?.Invoke(this, EventArgs.Empty);
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

        private async Task FilterLateActivities()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Chargement...");
                List<Activity> activities = await Activity.GetInProgressActivitiesOverdue(EntityId, EntityActivityType);

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
                UserDialogs.Instance.HideLoading();
            }
        }

        private async Task FilterTodayActivities()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Chargement...");
                List<Activity> activities = await Activity.GetInProgressActivitiesToday(EntityId, EntityActivityType);

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
                UserDialogs.Instance.HideLoading();
            }
        }

        private async Task FilterFutureActivities()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Chargement...");
                List<Activity> activities = await Activity.GetInProgressActivitiesFuture(EntityId, EntityActivityType);

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
                UserDialogs.Instance.ShowLoading("Chargement...");
                List<Activity> allActivities = new List<Activity>();

                if (IsLateChecked)
                {
                    var lateActivities = await Activity.GetInProgressActivitiesOverdue(EntityId, EntityActivityType);
                    allActivities.AddRange(lateActivities);
                }
                if (IsTodayChecked)
                {
                    var todayActivities = await Activity.GetInProgressActivitiesToday(EntityId, EntityActivityType);
                    allActivities.AddRange(todayActivities);
                }
                if (IsFutureChecked)
                {
                    var futureActivities = await Activity.GetInProgressActivitiesFuture(EntityId, EntityActivityType);
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
                UserDialogs.Instance.HideLoading();
            }
        }

        // Méthode pour gérer l'événement ActivityStateChanged du modèle Activity
        private void OnActivityStateChanged(object sender, Activity activity)
        {
            try
            {
                Console.WriteLine($"OnActivityStateChanged (événement): Activité ID={activity.Id}, État={activity.State}");
                Console.WriteLine($"OnActivityStateChanged (événement): ShowDoneMessage avant = {ShowDoneMessage}");
                Console.WriteLine($"OnActivityStateChanged (événement): _isLoadingFromFilter = {_isLoadingFromFilter}");
                
                // Ne pas déclencher l'affichage du formulaire si on charge depuis un filtre
                if (_isLoadingFromFilter)
                {
                    Console.WriteLine("OnActivityStateChanged (événement): Ignoré car _isLoadingFromFilter = true");
                    return;
                }
                
                // Marquer que c'est un changement individuel
                _isIndividualStateChange = true;
                
                // Définir l'activité sélectionnée
                SelectedActivity = activity;
                Console.WriteLine($"OnActivityStateChanged (événement): SelectedActivity défini = {SelectedActivity?.Id}");
                
                // Afficher le formulaire Done seulement si l'état est "Done"
                if (activity.State == 2) // État "Done"
                {
                    Console.WriteLine("OnActivityStateChanged (événement): Affichage du formulaire Done");
                    ShowDoneMessage = true;
                    IsActivityFormVisible = false; // Cacher l'autre formulaire
                    
                    Console.WriteLine($"OnActivityStateChanged (événement): ShowDoneMessage après = {ShowDoneMessage}");
                    Console.WriteLine($"OnActivityStateChanged (événement): IsActivityFormVisible = {IsActivityFormVisible}");
                }
                
                // Réinitialiser le flag après utilisation
                _isIndividualStateChange = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnActivityStateChanged (événement): Erreur - {ex.Message}");
                _isIndividualStateChange = false;
            }
        }

        public async Task PrefillNewActivityFromCompletedActivity(Activity completedActivity)
        {
            try
            {
                Console.WriteLine($"PrefillNewActivityFromCompletedActivity: Début - Activité ID={completedActivity.Id}");

                // Pré-remplir les informations de base
                ParentObject = EntityId = CurrentData.CurrentModuleId;
                ParentObjectType = CurrentData.CurrentActivityModule;
                ParentObjectDisplay = $"{completedActivity.PieceAcronym}/{completedActivity.PieceCode}";
                
                Console.WriteLine($"PrefillNewActivityFromCompletedActivity: Objet parent défini - Object={ParentObject}, Type={ParentObjectType}, Display={ParentObjectDisplay}");
                
                // Pré-remplir le formulaire
                FormDisplay = completedActivity.Form;
                EntityFormType = CurrentData.CurrentFormModule;
                
                Console.WriteLine($"PrefillNewActivityFromCompletedActivity: Formulaire défini - Form={FormDisplay}");
                
                // Pré-remplir l'employé assigné
                if (completedActivity.AssignedEmployee > 0 && Employees != null)
                {
                    SelectedEmployee = Employees.FirstOrDefault(e => e.Id == completedActivity.AssignedEmployee);
                    Console.WriteLine($"PrefillNewActivityFromCompletedActivity: Employé assigné - {SelectedEmployee?.NameEmployee}");
                }
                
                // Récupérer le nom de l'employé assigné de l'activité précédente
                PreviousActivityEmployeeName = completedActivity.AssignedEmployeeName ?? "Non assigné";
                
                Console.WriteLine($"PrefillNewActivityFromCompletedActivity: Nom employé activité précédente - {PreviousActivityEmployeeName}");
                
                // Réinitialiser les autres champs
                Summary = string.Empty;
                ActivityMemo = string.Empty;
                DueDate = DateTime.Now.AddDays(1); // Date d'échéance par défaut : demain

                Console.WriteLine($"PrefillNewActivityFromCompletedActivity: Champs réinitialisés");
                
                // Charger les types d'activité si pas encore fait
                if (ActivityTypes == null || ActivityTypes.Count == 0)
                {
                    await LoadActivityTypesAsync();
                }
                
                Console.WriteLine($"PrefillNewActivityFromCompletedActivity: Terminé avec succès");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PrefillNewActivityFromCompletedActivity: Erreur - {ex.Message}");
                throw;
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

        // Méthode pour se désabonner de l'événement
        public void Dispose()
        {
            Activity.ActivityStateChanged -= OnActivityStateChanged;
        }

        // Nouvelle méthode pour recharger l'interface selon les filtres actifs
        private async Task ReloadCurrentInterface()
        {
            try
            {
                // Vérifier les filtres actifs et recharger en conséquence
                // Appliquer le filtrage combiné seulement si "In Progress" est sélectionné ET que des switches sont actifs
                if (SelectedStateActivity?.Name == "In Progress" && (IsLateChecked || IsTodayChecked || IsFutureChecked))
                {
                    // Si "In Progress" est sélectionné et que des switches sont actifs, utiliser le filtre combiné
                    await ApplyCombinedFilter();
                }
                else if (SelectedStateActivity != null)
                {
                    // Si un état est sélectionné dans le ComboBox, utiliser le filtre par état (ignorer les switches)
                    await LoadFilteredActivities();
                }
                else
                {
                    // Sinon, recharger toutes les activités
                    await LoadActivities();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du rechargement de l'interface : {ex.Message}");
                // En cas d'erreur, essayer de recharger toutes les activités
                await LoadActivities();
            }
        }

        // Nouvelle méthode pour appliquer le filtrage combiné
        public async Task ApplyCombinedFilter()
        {
            try
            {
                // Vérifier que "In Progress" est sélectionné
                if (SelectedStateActivity?.Name != "In Progress")
                {
                    // Si ce n'est pas "In Progress", utiliser le filtre normal
                    await LoadFilteredActivities();
                    return;
                }

                _isLoadingFromFilter = true;
                UserDialogs.Instance.ShowLoading("Chargement...");
                int entityId = EntityId;
                string entityType = EntityActivityType;
                List<Activity> allActivities = new List<Activity>();
                // Déterminer l'état à filtrer (toujours "In Progress" pour le filtrage combiné)
                string targetState = "In Progress";

                // Appliquer les filtres de date selon les checkboxes cochées
                if (IsLateChecked)
                {
                    var lateActivities = await GetActivitiesByStateAndDate(entityId, entityType, targetState, "overdue");
                    allActivities.AddRange(lateActivities);
                }
                if (IsTodayChecked)
                {
                    var todayActivities = await GetActivitiesByStateAndDate(entityId, entityType, targetState, "today");
                    allActivities.AddRange(todayActivities);
                }
                if (IsFutureChecked)
                {
                    var futureActivities = await GetActivitiesByStateAndDate(entityId, entityType, targetState, "future");
                    allActivities.AddRange(futureActivities);
                }

                // Supprimer les doublons
                var combinedActivities = allActivities.Distinct(new ActivityComparer()).ToList();

                // Trier les activités par date d'échéance
                combinedActivities = combinedActivities
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
        private async Task<List<Activity>> GetActivitiesByStateAndDate(int entityId, string entityType, string state, string dateFilter)
        {
            // Pour le filtrage combiné, on ne gère que "In Progress"
            if (state != "In Progress")
            {
                return new List<Activity>();
            }

            switch (dateFilter)
            {
                case "overdue":
                    return await Activity.GetInProgressActivitiesOverdue(entityId, entityType);
                case "today":
                    return await Activity.GetInProgressActivitiesToday(entityId, entityType);
                case "future":
                    return await Activity.GetInProgressActivitiesFuture(entityId, entityType);
                default:
                    return await Activity.GetInProgressActivities(entityId, entityType);
            }
        }
    }

}
