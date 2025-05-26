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
        public class CustomNavigationDrawerViewModel : INotifyPropertyChanged
    {
            private readonly IDispatcherTimer _timer;
            int userId = Preferences.Get("iduser", 0);
            public ICommand EnableSelectionModeCommand => new Command<UserModel>((user) =>
            {
                IsSelectionMode = true;
                user.IsSelected = true; // cocher l'élément appuyé
            });
            protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
            {
                if (EqualityComparer<T>.Default.Equals(backingField, value))
                    return false;

                backingField = value;
                OnPropertyChanged(propertyName);
                return true;
            }

            private bool testLoad;
            public bool TestLoad { get => testLoad; set => SetProperty(ref testLoad, value); }
            public bool actpopup = true;
            public bool ActPopup { get => actpopup; set => SetProperty(ref actpopup, value); }
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

        /*********memo*********/
            private ObservableCollection<Memo> _memos = new ObservableCollection<Memo>(); // Liste des mémos
            public ObservableCollection<Memo> Memos
            {
                get => _memos;
                set
                {
                    _memos = value;
                    OnPropertyChanged(nameof(Memos));
                }
            }

            private string _newMemoName;
            public string NewMemoName
            {
                get => _newMemoName;
                set
                {
                    _newMemoName = value;
                    OnPropertyChanged(nameof(NewMemoName));
                }
            }

            private string _newMemoDescription;
            public string NewMemoDescription
            {
                get => _newMemoDescription;
                set
                {
                    _newMemoDescription = value;
                    OnPropertyChanged(nameof(NewMemoDescription));
                }
            }

            private int _currentViewIndex = 0; // 0 -> Memo, 1 -> Activity, 2 -> Chat
            private string _headerText = "Memo";

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
          private List<UserModel> _user;
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
        /************chat***********/
        private bool _isSelectionMode;
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


        private ObservableCollection<UserchatModel> _users;
            public ObservableCollection<UserchatModel> Users
            {
                get => _users;
                set
                {
                    _users = value;
                    OnPropertyChanged();
                }
            }
        private ObservableCollection<UserModel> _recipients = new ObservableCollection<UserModel>();
        public ObservableCollection<UserModel> Recipients
        {
            get => _recipients;
            set
            {
                _recipients = value;
                OnPropertyChanged();
            }
        }
        private string _searchText;
            public string SearchText
            {
                get => _searchText;
                set
                {
                    if (_searchText != value)
                    {
                        _searchText = value;
                        OnPropertyChanged(nameof(SearchText));
                        FilterUsers(); // Applique le filtre chaque fois que le texte change
                    }
                }
            }

            private ObservableCollection<UserModel> _filteredUsers;
            public ObservableCollection<UserModel> FilteredUsers
            {
                get => _filteredUsers;
                set
                {
                    _filteredUsers = value;
                    OnPropertyChanged(nameof(FilteredUsers));
                }
            }

        private bool _showCheckboxes;
        public bool ShowCheckboxes
        {
            get => _showCheckboxes;
            set
            {
                _showCheckboxes = value;
                OnPropertyChanged();
            }
        }
        public ICommand ToggleCheckboxesCommand { get; }

        private UserModel _selectedUser;
            public UserModel SelectedUser
            {
                get => _selectedUser;
                set
                {
                    //🔁 Désélectionner tous les utilisateurs
                    foreach (var user in FilteredUsers.Where(u => u.IsSelected))
                    {
                        user.IsSelected = false;
                    }
                   _selectedUser = value;
                    OnPropertyChanged();  
                    if (_selectedUser != null)
                    {
                         _selectedUser.IsSelected = true; //✅ Cocher la checkbox liée à cet utilisateur
                        LoadMessagesAsync();  
                    }
                }
            }

            private bool _isRead;
            public bool IsRead
            {
                get => _isRead;
                set
                {
                    if (_isRead != value)
                    {
                        _isRead = value;
                        OnPropertyChanged(nameof(IsRead));
                    }
                }
            }

            private DateTime _readDate;
            public DateTime ReadDate
            {
                get => _readDate;
                set
                {
                    if (_readDate != value)
                    {
                        _readDate = value;
                        OnPropertyChanged(nameof(ReadDate));
                    }
                }
            }
            private string _messageText;
            public string MessageText
            {
                get => _messageText;
                set
                {
                    _messageText = value;
                    OnPropertyChanged(nameof(MessageText));
                }
            }

             private Timer _messageCheckTimer;

        // public Action ScrollToLastMessage { get; set; } // Définir l'action pour faire défiler


        private ObservableCollection<MessageModel> _messages = new ObservableCollection<MessageModel>();
        public ObservableCollection<MessageModel> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        /*********************/
        /*********activity*******/
        private SmartPharma5.Model.Activity.ActivityState _selectedStateActivity = Activity.selectStates.First();
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

        private ObservableCollection<ActivityType> _activityTypes;
        public ObservableCollection<ActivityType> ActivityTypes
        {
            get => _activityTypes;
            set
            {
                _activityTypes = value;
                OnPropertyChanged(nameof(ActivityTypes));
            }
        }

        private ActivityType _selectedActivityType;
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
                        Summary = value.Summary; // Ceci mettra à jour l'Entry via le binding
                        OnPropertyChanged(nameof(Summary));
                    }
                }
            }
        }

        //private ActivityType _selectedActivityType;
        //public ActivityType SelectedActivityType
        //{
        //    get => _selectedActivityType;
        //    set
        //    {
        //        _selectedActivityType = value;
        //        OnPropertyChanged(nameof(SelectedActivityType));

        //        // Mettre à jour l'ID du type d'activité dans votre modèle si nécessaire
        //        if (value != null && SelectedActivity != null)
        //        {
        //            SelectedActivity.Type = value.Id;
        //        }
        //    }
        //}
        private string _summary;
        private DateTime _dueDate;
        private string _activityMemo;

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

        //private ActivityType _selectedActivityType;
        //public ActivityType SelectedActivityType
        //{
        //    get => _selectedActivityType;
        //    set
        //    {
        //        _selectedActivityType = value;
        //        OnPropertyChanged(nameof(SelectedActivityType));
        //    }
        //}
        private Activity _selectedActivity;
        public Activity SelectedActivity
        {
            get => _selectedActivity;
            set
            {
                _selectedActivity = value;
                OnPropertyChanged(nameof(SelectedActivity));
            }
        }


        private ObservableCollection<Activity> _activities;
            public ObservableCollection<Activity> Activities
            {
                get => _activities;
                set
                {
                    _activities = value;
                    OnPropertyChanged(nameof(Activities));
                }
            }

            private string _newActivityDescription;
            public string NewActivityDescription
            {
                get => _newActivityDescription;
                set
                {
                    _newActivityDescription = value;
                    OnPropertyChanged(nameof(NewActivityDescription));
                }
            }
            private ObservableCollection<EmployeeDto> _employees;
            public ObservableCollection<EmployeeDto> Employees
            {
                get => _employees;
                set
                {
                    _employees = value;
                    OnPropertyChanged(nameof(Employees));
                }
            }
        private EmployeeDto _selectedEmployee;
        public EmployeeDto SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
                // Mettez à jour l'employé assigné dans l'activité
                //if (CurrentActivity != null && value != null)
                //{
                //    CurrentActivity.AssignedEmployee = value.Id;
                //}
            }
        }
        private Activity _currentActivity;
        public Activity CurrentActivity
        {
            get => _currentActivity;
            set
            {
                _currentActivity = value;
                OnPropertyChanged(nameof(CurrentActivity));
                // Mettez à jour l'employé sélectionné si nécessaire
                if (value != null && Employees != null)
                {
                    SelectedEmployee = Employees.FirstOrDefault(e => e.Id == value.AssignedEmployee);
                }
            }
        }
        private EmployeeDto _selectedActivityEmployee;
        public EmployeeDto SelectedActivityEmployee
        {
            get => _selectedActivityEmployee;
            set
            {
                _selectedActivityEmployee = value;
                OnPropertyChanged(nameof(SelectedActivityEmployee));

                // Mettre à jour l'employé assigné dans SelectedActivity
                if (SelectedActivity != null && value != null)
                {
                    SelectedActivity.AssignedEmployee = value.Id;
                    SelectedActivity.AssignedEmployeeName = value.NameEmployee;

                }
            }
        }


        /***********************/






        private bool _isMemoFormVisible;
            public bool IsMemoFormVisible
            {
                get => _isMemoFormVisible;
                set
                {
                    _isMemoFormVisible = value;
                    OnPropertyChanged(nameof(IsMemoFormVisible));
                }
            }
            private bool _isActivityFormVisible;
            public bool IsActivityFormVisible
            {
                get => _isActivityFormVisible;
                set
                {
                    _isActivityFormVisible = value;
                    OnPropertyChanged(nameof(IsActivityFormVisible));
                }
            }
            private Memo _selectedMemo;
            public Memo SelectedMemo
            {
                get => _selectedMemo;
                set
                {
                    _selectedMemo = value;
                    OnPropertyChanged(nameof(SelectedMemo));
                }
            }

        private string _selectedView = "Memo"; // Définir Memo comme actif au démarrage
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

        private string _selectedState;
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

        public CustomNavigationDrawerViewModel(int entityId, string entityType, string entityActivityType)
             {
                 LoadEmployeesAsync();
                 // Sélectionnez l'employé par défaut si nécessaire
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
                 // Initialiser avec l'état "All" (premier élément de la liste)
                 SelectedStateActivity = Activity.selectStates.First(s => s.Name == "All");
                 // Charger les activités initiales
                 Task.Run(async () => await LoadFilteredActivities());
                 _messageCheckTimer = new Timer(CheckForNewMessages, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
                 SwipeRightCommand = new Command(OnSwipeRight);
                 SwipeLeftCommand = new Command(OnSwipeLeft);
                 AddMemoCommand = new Command<object>(async (linkedObject) => await OnAddMemo(linkedObject));
                 Task.Run(async () => await LoadMemos());
                // Task.Run(async () => await LoadActivities()); // Charger les activités
                 //NavigateToMemoCommand = new Command(async () => await NavigateToMemoAsync());
                 //NavigateToActivityCommand = new Command(() => CurrentViewIndex = 1);
                 NavigateToChatCommand = new Command(() => CurrentViewIndex = 2);
                 States = new ObservableCollection<string> { "All", "In Progress", "Done", "Cancelled" };
                 SaveActivityEditCommand = new Command(OnSaveActivityEdit);
                 FilteredActivities = new ObservableCollection<Activity>(Activities);
                 EditActivityCommand = new Command<int>(OnEditActivityClicked);
                 CancelEditCommand = new Command(OnCancelEdit);
                 LoadUsers();
                _timer = Dispatcher.GetForCurrentThread().CreateTimer();
                _timer.Interval = TimeSpan.FromSeconds(5); // Met à jour toutes les 2 secondes
                _timer.Tick += async (s, e) => await RefreshUnreadMessages();
                _timer.Start();
                NavigateToMemoCommand = new Command(() => LoadMemoView());
                NavigateToActivityCommand = new Command(() => LoadActivityView());
                // juste pour l'afficher de checkbox
                ToggleCheckboxesCommand = new Command(() =>
                {
                    ShowCheckboxes = !ShowCheckboxes;
                });
            LoadActivityTypesAsync();
            // SmartPharma5.Model.Activity.ActivityState.InitializeStates();
        }


            public CustomNavigationDrawerViewModel()
            {
                 LoadEmployeesAsync();

                // Sélectionnez l'employé par défaut si nécessaire
                if (CurrentActivity?.AssignedEmployee > 0 && Employees != null)
                {
                    SelectedEmployee = Employees.FirstOrDefault(e => e.Id == CurrentActivity.AssignedEmployee);
                }
                EntityId = CurrentData.CurrentModuleId;
                EntityType =CurrentData.CurrentNoteModule;
                EntityActivityType = CurrentData.CurrentActivityModule;
                EntityFormType = CurrentData.CurrentFormModule;

                AddActivityCommand = new Command(async () => await SaveActivityAsync());
                Activities = new ObservableCollection<Activity>();
                SmartPharma5.Model.Activity.ActivityState
                // Initialiser avec l'état "All" (premier élément de la liste)
                SelectedStateActivity = Activity.selectStates.First(s => s.Name == "All");
                // Charger les activités initiales
                Task.Run(async () => await LoadFilteredActivities());
                NavigateToMemoCommand = new Command(() => LoadMemoView());
                NavigateToActivityCommand = new Command(() => LoadActivityView());
                SendMessageCommand = new Command(async () => await SendMessageAsync());
                _messageCheckTimer = new Timer(CheckForNewMessages, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
                DueDate = DateTime.Now; 
                // Task.Run(async () => await LoadActivities()); 
                 ActivityTypes = new ObservableCollection<ActivityType>();
                SwipeRightCommand = new Command(OnSwipeRight);
                SwipeLeftCommand = new Command(OnSwipeLeft);
                AddMemoCommand = new Command<object>(async (linkedObject) => await OnAddMemo(linkedObject));
                Task.Run(async () => await LoadMemos());
                //NavigateToMemoCommand = new Command(async () => await NavigateToMemoAsync());
                //NavigateToActivityCommand = new Command(() => CurrentViewIndex = 1);
                NavigateToChatCommand = new Command(() => CurrentViewIndex = 2);
                SaveEditCommand = new Command(OnSaveEdit);
                SaveActivityEditCommand = new Command(OnSaveActivityEdit);
                States = new ObservableCollection<string> { "All", "In Progress", "Done", "Cancelled" };
                FilteredActivities = new ObservableCollection<Activity>(Activities);
                EditActivityCommand = new Command<int>(OnEditActivityClicked);
                CancelEditCommand = new Command(OnCancelEdit);
                LoadUsers();
                _timer = Dispatcher.GetForCurrentThread().CreateTimer();
                _timer.Interval = TimeSpan.FromSeconds(5); // Met à jour toutes les 2 secondes
                _timer.Tick += async (s, e) => await RefreshUnreadMessages();
                _timer.Start();
                ToggleCheckboxesCommand = new Command(() =>
                {
                    ShowCheckboxes = !ShowCheckboxes;
                });
           LoadActivityTypesAsync();
            // SmartPharma5.Model.Activity.ActivityState.InitializeStates();

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

        private void LoadMemoView()
        {
            //Activities?.Clear(); // Vider la liste des activités si besoin
            //Messages?.Clear();   // Vider la liste des messages si besoin
            Task.Run(async () => await LoadMemos());
            //CurrentViewIndex = 0; // Affiche la vue Memo
        }

        private void LoadActivityView()
        {
            //Messages?.Clear();   // Vider la liste des messages si besoin
            //Memos?.Clear();      // Vider la liste des mémos si besoin
            Task.Run(async () => await LoadActivities());
            //CurrentViewIndex = 1; // Affiche la vue Activité
        }


        /******************chat***************/
        private async Task RefreshUnreadMessages()
        {
            //var updatedUsers = await UserModel.LoadUsersAsync(userId);
            var updatedUsers = await UserModel.LoadUsersAsync(
            userId,
            CurrentData.CurrentModuleId,
            CurrentData.CurrentNoteModule);
            int totalUnread = 0;

            // Mise à jour des valeurs sans recréer la liste
            foreach (var updatedUser in updatedUsers)
            {
                var existingUser = FilteredUsers.FirstOrDefault(u => u.Id == updatedUser.Id);
                if (existingUser != null)
                {
                    existingUser.UnreadMessagesCount = updatedUser.UnreadMessagesCount;
                    existingUser.HasUnreadMessages = updatedUser.UnreadMessagesCount > 0;
                }
            }
            App.NotificationVM.TotalUnreadMessages = totalUnread;

            // Rafraîchir complètement la liste
            var tempList = new ObservableCollection<UserModel>(FilteredUsers);
            FilteredUsers.Clear();
            foreach (var user in tempList)
            {
                FilteredUsers.Add(user);
            }
        }


        private async void LoadUsers()
        {
            //User = await UserModel.LoadUsersAsync(userId);
            User = await UserModel.LoadUsersAsync(
            userId,
            CurrentData.CurrentModuleId,
            CurrentData.CurrentNoteModule);
            FilteredUsers = new ObservableCollection<UserModel>(User); // Initialiser avec tous les utilisateurs

        }
        private async void CheckForNewMessages(object state)
        {
            await LoadMessagesAsync();

        }
        public void AddNewMessage(MessageModel newMessage)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Messages.Add(newMessage); // Ajouter le nouveau message
               // ScrollToLastMessage?.Invoke(); // Déclencher le défilement automatique
            });
        }
        public async void ScrollToLastMessage()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Envoyer le message pour le scroll
                MessagingCenter.Send(this, "ScrollToLastMessage");

                // Marquer les messages comme lus
                await MarkMessagesAsRead();
            });
        }

        private async Task MarkLastMessageAsRead()
        {
            if (Messages == null || !Messages.Any()) return;

            var lastMessage = Messages.Last();
            var currentUserId = Preferences.Get("iduser", 0);

            // Vérifier que le dernier message n'est pas déjà lu et qu'il vient de l'autre utilisateur
            if (!lastMessage.IsRead && lastMessage.Sender != currentUserId)
            {
                bool success = await MessageModel.UpdateMessageAsReadAsync(lastMessage.Id);
                if (success)
                {
                    lastMessage.IsRead = true;
                    lastMessage.ReadDate = DateTime.Now;
                    // Rafraîchir l'affichage
                    OnPropertyChanged(nameof(Messages));
                }
            }
        }
        public async Task MarkMessagesAsRead()
        {
            if (Messages == null || !Messages.Any()) return;

            var currentUserId = Preferences.Get("iduser", 0);
            var unreadMessages = Messages
                .Where(m => !m.IsRead && m.Sender != currentUserId)
                .ToList();

            if (!unreadMessages.Any()) return;

            foreach (var message in unreadMessages)
            {
                bool success = await MessageModel.UpdateMessageAsReadAsync(message.Id, currentUserId);
                if (success)
                {
                    message.IsRead = true;  // Met à jour la propriété Bindable
                    message.ReadDate = DateTime.Now;
                    message.OnPropertyChanged(nameof(message.ImageSource));

                }
            }
        }

        public void StopTimer()
        {
            _messageCheckTimer?.Change(Timeout.Infinite, 0);
        }
        private void FilterUsers()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                FilteredUsers = new ObservableCollection<UserModel>(User);
            }
            else
            {
                var filteredList = User.Where(u => u.Login.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                FilteredUsers = new ObservableCollection<UserModel>(filteredList);
            }
        }

        //public async Task SendMessageAsync()
        //{
        //    if (!string.IsNullOrEmpty(MessageText) && SelectedUser != null)
        //    {

        //        // Réinitialiser IsLastSentMessage pour tous les messages précédents
        //        foreach (var msg in Messages.Where(m => m.Sender == userId))
        //        {
        //            msg.IsLastSentMessage = false;
        //        }

        //        bool success = await MessageModel.InsertMessageAsync(
        //            CurrentData.CurrentModuleId,
        //            CurrentData.CurrentNoteModule,
        //            MessageText,
        //            userId,
        //            SelectedUser.Id); // Utiliser l'ID de l'utilisateur sélectionné

        //        if (success)
        //        {
        //            Messages.Add(new MessageModel
        //            {
        //                Text = $"{MessageText}",
        //                BackgroundColor = Color.FromArgb("#D0E8FF"),
        //                Alignment = LayoutOptions.End,
        //                CreateDate = DateTime.Now,
        //                IsLastSentMessage = true // Marquer comme dernier message envoyé

        //            });

        //            MessageText = string.Empty;
        //            ScrollToLastMessage();
        //            MessagingCenter.Send(this, "ScrollToLastMessage");

        //        }
        //        else
        //        {
        //            await UserDialogs.Instance.AlertAsync("Erreur lors de l'envoi du message.", "Erreur", "OK");
        //        }
        //    }
        //}
        // Ajoutez cette propriété


        // Modifiez SendMessageAsync
        public async Task SendMessageAsync()
        {
            if (string.IsNullOrEmpty(MessageText))
            {
                await UserDialogs.Instance.AlertAsync("Veuillez saisir un message", "Erreur", "OK");
                return;
            }

            // Ajouter l'utilisateur courant s'il n'est pas déjà dans la liste
            if (SelectedUser != null && !Recipients.Any(r => r.Id == SelectedUser.Id))
            {
                Recipients.Add(SelectedUser);
            }

            if (!Recipients.Any())
            {
                await UserDialogs.Instance.AlertAsync("Aucun destinataire sélectionné", "Erreur", "OK");
                return;
            }

            bool allSucceeded = true;

            foreach (var recipient in Recipients.ToList()) // ToList pour éviter les modifications pendant l'itération
            {
                bool success = await MessageModel.InsertMessageAsync(
                    CurrentData.CurrentModuleId,
                    CurrentData.CurrentNoteModule,
                    MessageText,
                    userId,
                    recipient.Id);

                if (!success)
                {
                    allSucceeded = false;
                    await UserDialogs.Instance.AlertAsync(
                        $"Échec d'envoi à {recipient.Login}",
                        "Erreur", "OK");
                }
                else
                {
                    // Ajouter le message dans la conversation actuelle si c'est le destinataire actif
                    if (SelectedUser != null && recipient.Id == SelectedUser.Id)
                    {
                        Messages.Add(new MessageModel
                        {
                            Text = MessageText,
                            BackgroundColor = Color.FromArgb("#D0E8FF"),
                            Alignment = LayoutOptions.End,
                            CreateDate = DateTime.Now,
                            IsLastSentMessage = true
                        });
                    }
                }
            }

            if (allSucceeded)
            {
                MessageText = string.Empty;
                ScrollToLastMessage();

                // Optionnel : vider la liste après envoi ou la garder pour le prochain message
                // Recipients.Clear(); 
            }
        }
        public void ToggleRecipient(UserModel user)
        {
            if (user == null) return;

            if (user.IsSelected)
            {
                if (!Recipients.Any(r => r.Id == user.Id))
                {
                    Recipients.Add(user);
                }
            }
            else
            {
                var toRemove = Recipients.FirstOrDefault(r => r.Id == user.Id);
                if (toRemove != null)
                {
                    Recipients.Remove(toRemove);
                }
            }
        }
        public async Task LoadMessagesAsync()
        {
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
            try
            {
                if (SelectedUser == null) return;

                var messages = await MessageModel.GetMessagesAsync(
                    CurrentData.CurrentModuleId,
                    CurrentData.CurrentNoteModule,
                    SelectedUser.Id,
                    userId

                );

                var sortedMessages = messages.OrderBy(m => m.CreateDate).ToList();

                // Trouver le dernier message envoyé par l'utilisateur actuel
                var lastSentMessage = sortedMessages
                    .Where(m => m.Sender == userId)
                    .OrderByDescending(m => m.CreateDate)
                    .FirstOrDefault();

                if (lastSentMessage != null)
                {
                    // Désactiver l'indicateur pour tous les messages
                    foreach (var msg in sortedMessages.Where(m => m.Sender == userId))
                    {
                        msg.IsLastSentMessage = false;
                    }

                    // Activer seulement pour le dernier message
                    lastSentMessage.IsLastSentMessage = true;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    Messages.Clear();
                    foreach (var message in sortedMessages)
                    {
                        Messages.Add(message);
                    }
                    ScrollToLastMessage();
                });
            }catch (Exception ex)
            {
                TestLoad = true;
            }
            ActPopup = false;

        }
        /****************************************/
        /****************activity***************/

        //public async Task LoadActivityTypesAsync()
        //{
        //    var activityTypes = await ActivityType.GetActivityTypes();
        //    ActivityTypes = new ObservableCollection<ActivityType>(activityTypes);

        //    // Sélectionner un type par défaut si nécessaire
        //    if (SelectedActivity != null && SelectedActivity.Type > 0)
        //    {
        //        SelectedActivityType = ActivityTypes.FirstOrDefault(at => at.Id == SelectedActivity.Type);
        //    }
        //}
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


        public async Task LoadFilteredActivities()
        {
            int entityId = EntityId;
            string entityType = EntityActivityType;
            List<Activity> activities;
            try
            {
                UserDialogs.Instance.ShowLoading("Chargement...");
                await Task.Delay(400);
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
            int employeeId = await Activity.GetEmployeeIdByUserId(userId);

            var activity = new Activity
            {
                CreateDate = DateTime.Now,
                Summary = Summary,
                DueDate = DueDate,
                Memo = ActivityMemo,
                Type = SelectedActivityType.Id,
                AssignedEmployee = SelectedEmployee.Id, // ID de l'employé sélectionné
                Author = employeeId, // Ou votre logique actuelle
                State = 1,
                ObjectType = EntityActivityType,
                Object = EntityId,
                Date = DateTime.Now,
                Form =  EntityFormType 
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

        /***********************************/
        /*****************memo**************/
        public void RefreshMemos()
        {
            LoadMemos();
        }
        private async void OnSaveEdit()
        {
            if (SelectedMemo == null)
            {
                Console.WriteLine("SelectedMemo est null.");
                await Application.Current.MainPage.DisplayAlert("Error", "Aucun mémo sélectionné.", "OK");
                return;
            }

            // Mettre à jour les propriétés de modification
            SelectedMemo.ModifyDate = DateTime.Now;
            SelectedMemo.Modify_user = userId; // Remplacez par l'utilisateur actuel

            // Sauvegarder les modifications dans la base de données
            bool isUpdated = await Memo.UpdateMemoInDatabase(SelectedMemo);

            if (isUpdated)
            {
                // Rafraîchir la liste des mémoires
                RefreshMemos();
            }
            else
            {
                // Afficher une erreur
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to update memo.", "OK");
            }
        }
        //public async Task LoadMemos()
        //{
        //    try
        //                {
        //        var memos = await Memo.GetAllMemosForDrawerControl(EntityId, EntityType);
        //        Device.BeginInvokeOnMainThread(() =>
        //        {
        //            Memos.Clear();
        //            foreach (var memo in memos)
        //            { 
        //                Memos.Add(memo);
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Erreur lors du chargement des mémos : {ex.Message}");
        //    }
        //}
        public async Task LoadMemos()
        {
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

            try
            {

                var memos = await Memo.GetAllMemosForDrawerControl(EntityId, EntityType);

                if (memos == null || memos.Count == 0)
                {
                    Console.WriteLine("Aucun mémo trouvé.");
                    return; // Évite d'effacer les mémos si aucun nouveau mémo n'est chargé.
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    Memos.Clear();
                    foreach (var memo in memos)
                    {
                        Memos.Add(memo);
                    }
                });
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des mémos : {ex.Message}");
                TestLoad = true;
            }
            //finally
            //{
            //    UserDialogs.Instance.HideLoading();
            //}
            ActPopup = false;

        }

        private async void OnDeleteMemoClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.CommandParameter is int memoId)
            {
                bool isDeleted = await Memo.DeleteMemoFromDatabase(memoId);
                if (isDeleted)
                {
                    var memoToRemove = Memos.FirstOrDefault(m => m.Id == memoId);
                    if (memoToRemove != null)
                    {
                        Memos.Remove(memoToRemove); // Met à jour l'UI automatiquement si Memos est une ObservableCollection
                    }
                }
                else
                {   
                    Console.WriteLine("Erreur lors de la suppression du mémo.");
                }
            }
        }

        //private void OnSendMessage()
        //    {
        //        if (!string.IsNullOrEmpty(MessageText))
        //        {
        //            Messages.Add(new MessageModel
        //            {
        //                Text = $"You: {MessageText}",
        //                BackgroundColor = Color.FromArgb("#D0E8FF"), 
        //                Alignment = LayoutOptions.End 
        //            });
        //        MessageText = string.Empty; 
        //        }
        //    }

        private async Task OnAddMemo(object linkedObject)
            {
                //if (string.IsNullOrWhiteSpace(NewMemoName) /*|| string.IsNullOrWhiteSpace(NewMemoDescription)*/)
                //{
                //    return;
                //}

                int userId = Preferences.Get("iduser", 0);
                if (userId == 0)
                {
                    // Gérer le cas où l'ID de l'utilisateur n'est pas trouvé (ex: utilisateur non connecté)
                    return;
                }

                // Récupération dynamique de l'ID (Piece) et du type (PieceType) de l'objet lié
                int? pieceId = null;
                string pieceType = null;

                // Création de l'objet Memo en utilisant les valeurs récupérées
                var memo = new Memo(NewMemoName, NewMemoDescription, userId)
                {
                    // Piece = EntityId,
                    Piece = CurrentData.CurrentModuleId,
                    // PieceType = EntityType.ToString(),
                    PieceType = CurrentData.CurrentNoteModule,

                    // ModifyDate = DateTime.Now,
                    Modify_user = null
                };

                bool isSaved = await Memo.SaveToDatabase(memo);
                if (isSaved)
                {
                    Memos.Insert(0, memo);

                    NewMemoName = string.Empty;
                    NewMemoDescription = string.Empty;

                    // Informer l'UI qu'il y a une mise à jour
                    OnPropertyChanged(nameof(Memos));
                    RefreshMemos();

                }
                else
                    {
                        Console.WriteLine("Erreur lors de l'enregistrement du mémo.");
                    }
                }

            // Méthode pour charger tous les mémos
         
            // Méthode pour gérer le swipe à droite
            private void OnSwipeRight()
            {
                if (CurrentViewIndex > 0)
                {
                    CurrentViewIndex--;
                }
            }

            // Méthode pour gérer le swipe à gauche
            private void OnSwipeLeft()
            {
                if (CurrentViewIndex < 2)
                {
                    CurrentViewIndex++;
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

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public enum EntityType
        {
            Opportunity,
            Partner,
            Payment
        }
    }


/* public async Task LoadActivitiesAsync()
 {
     var activities = await Activity.GetActivitiesFromDatabaseAsync();
     if (activities != null)
     {
         Activities = new ObservableCollection<Activity>(activities);
     }
 }*/

/*  private void AddActivity()
  {
      if (!string.IsNullOrEmpty(Summary))
      {
          Activities.Add(new Activity
          {
              CreateDate = DateTime.Now, // Date de création = maintenant
              Summary = Summary,
              DueDate = DueDate,
              ActivityMemo = ActivityMemo, // Utilisation de la nouvelle propriété
              DoneDate = null // Par défaut, la date de fin est nulle
          });

          // Réinitialiser les champs du formulaire
          Summary = string.Empty;
          DueDate = DateTime.Now;
          ActivityMemo = string.Empty; // Réinitialiser le champ mémo
      }
  }*/
/* private async void AddActivity()
 {
     if (!string.IsNullOrEmpty(Summary))
     {


         var activity = new Activity
         {
             CreateDate = DateTime.Now,
             Summary = Summary,
             DueDate = DueDate,
             Memo = ActivityMemo,
             DoneDate = null,
             Author = userId,
             ObjectType = EntityType.ToString(),
             Object = EntityId, 
             Date = DateTime.Now,
             Form = "AtooERP_Standard.CRM_Opportunity.Opportunity_update"
         };

         bool isSaved = await Activity.SaveactivityToDatabase(activity, SelectedActivityType.Id);
         if (isSaved)
         {
             Activities.Add(activity);

             // Réinitialiser les champs du formulaire
             Summary = string.Empty;
             DueDate = DateTime.Now;
             ActivityMemo = string.Empty;
         }
         else
         {
             Console.WriteLine("Erreur lors de l'enregistrement de l'activité.");
         }
     }
 }  */      // Méthode pour envoyer un message
/*private async void OnSaveEditActivity()
{
    if (SelectedActivity != null)
    {

        bool isUpdated = await Activity.UpdateActivityInDatabase(SelectedActivity);

        if (isUpdated)
        {
            LoadActivities();
            IsActivityFormVisible = false;
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Failed to update activity.", "OK");
        }
    }
}*/
/*
 
            switch (SelectedStateActivity?.Name)
            {
                case "In Progress":
                    UserDialogs.Instance.ShowLoading("Loading...");
                    try
                    {
                       activities = await Activity.GetInProgressActivities(entityId, entityType);

                    }
                    catch (Exception ex)
                    {
                    }
                    UserDialogs.Instance.HideLoading();
                    break;
                case "Done":
                    UserDialogs.Instance.ShowLoading("Loading...");
                    try
                    {
                        activities = await Activity.GetDoneActivities(entityId, entityType);

                    }
                    catch (Exception ex)
                    {
                    }
                    UserDialogs.Instance.HideLoading();
                    break;
                case "Cancelled":
                    UserDialogs.Instance.ShowLoading("Loading...");
                        try
                        {
                            activities = await Activity.GetCancelledActivities(entityId, entityType);

                        }
                        catch (Exception ex)
                        {
                        }
                    UserDialogs.Instance.HideLoading();
                    break;
                default: // "All" ou autre
                    UserDialogs.Instance.ShowLoading("Loading...");
                    try
                    {
                        activities = await Activity.GetAllActivitiesForDrawerControl(entityId, entityType);

                    }
                    catch (Exception ex)
                    {
                    }
                    UserDialogs.Instance.HideLoading();
                    break;
            }*/