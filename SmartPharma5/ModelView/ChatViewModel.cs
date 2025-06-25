using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Acr.UserDialogs;
using MvvmHelpers;
using SmartPharma5.Model;
using static SmartPharma5.Model.Activity;

namespace SmartPharma5.ViewModel
{
    public class ChatViewModel : INotifyPropertyChanged
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
        //public ICommand SendMessageCommand { get; }
        private ICommand sendMessageCommand;
        public ICommand SendMessageCommand
        {
            get => sendMessageCommand;
            set => sendMessageCommand = value;
        }

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
                    IsLoadingToLastMessage = true; // Activer l'indicateur quand on sélectionne un utilisateur
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

        private bool _isLoadingToLastMessage;
        public bool IsLoadingToLastMessage
        {
            get => _isLoadingToLastMessage;
            set
            {
                _isLoadingToLastMessage = value;
                OnPropertyChanged(nameof(IsLoadingToLastMessage));
            }
        }

        private Timer _refreshMessagesTimer; // Timer pour rafraîchir les messages toutes les 2 secondes
        private Timer _markAsReadTimer; // Timer pour marquer comme lu

        public ChatViewModel(int entityId, string entityType, string entityActivityType)
        {
            LoadEmployeesAsync();
            if (CurrentActivity?.AssignedEmployee > 0 && Employees != null)
            {
                SelectedEmployee = Employees.FirstOrDefault(e => e.Id == CurrentActivity.AssignedEmployee);
            }
            EntityId = entityId;
            EntityType = entityType;
            EntityActivityType = entityActivityType;
            ActivityTypes = new ObservableCollection<ActivityType>();
            Activities = new ObservableCollection<Activity>();
            SmartPharma5.Model.Activity.ActivityState
            SelectedStateActivity = Activity.selectStates.First(s => s.Name == "All");
            _messageCheckTimer = new Timer(CheckForNewMessages, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            _markAsReadTimer = new Timer(MarkAsReadPeriodically, null, TimeSpan.Zero, TimeSpan.FromSeconds(3)); // Marquer comme lu toutes les 3 secondes
            _refreshMessagesTimer = new Timer(RefreshMessagesStatus, null, TimeSpan.Zero, TimeSpan.FromSeconds(2)); // Rafraîchir le statut toutes les 2 secondes
            NavigateToChatCommand = new Command(() => CurrentViewIndex = 2);
            States = new ObservableCollection<string> { "All", "In Progress", "Done", "Cancelled" };
            FilteredActivities = new ObservableCollection<Activity>(Activities);
            LoadUsers();
            _timer = Dispatcher.GetForCurrentThread().CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(5); // Met à jour toutes les 2 secondes
            _timer.Tick += async (s, e) => await RefreshUnreadMessages();
            _timer.Start();
            // juste pour l'afficher de checkbox
            ToggleCheckboxesCommand = new Command(() =>
            {
                ShowCheckboxes = !ShowCheckboxes;
            });
            // SmartPharma5.Model.Activity.ActivityState.InitializeStates();
            SendMessageCommand = new Command(async () =>
            {
                await SendMessageAsync();
                LoadUsers(); // ou await RefreshUnreadMessages();
            });
            
            // S'assurer que l'indicateur est désactivé au démarrage
            IsLoadingToLastMessage = false;
        }


        public ChatViewModel()
        {
            LoadEmployeesAsync();

            // Sélectionnez l'employé par défaut si nécessaire
            if (CurrentActivity?.AssignedEmployee > 0 && Employees != null)
            {
                SelectedEmployee = Employees.FirstOrDefault(e => e.Id == CurrentActivity.AssignedEmployee);
            }
            EntityId = CurrentData.CurrentModuleId;
            EntityType = CurrentData.CurrentNoteModule;
            EntityActivityType = CurrentData.CurrentActivityModule;
            EntityFormType = CurrentData.CurrentFormModule;
            Activities = new ObservableCollection<Activity>();
            SmartPharma5.Model.Activity.ActivityState
            SelectedStateActivity = Activity.selectStates.First(s => s.Name == "All");
            //SendMessageCommand = new Command(async () => await SendMessageAsync());
            SendMessageCommand = new Command(async () =>
            {
                await SendMessageAsync();
                LoadUsers(); // ou await RefreshUnreadMessages();
            });

            _messageCheckTimer = new Timer(CheckForNewMessages, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            _markAsReadTimer = new Timer(MarkAsReadPeriodically, null, TimeSpan.Zero, TimeSpan.FromSeconds(3)); // Marquer comme lu toutes les 3 secondes
            _refreshMessagesTimer = new Timer(RefreshMessagesStatus, null, TimeSpan.Zero, TimeSpan.FromSeconds(2)); // Rafraîchir le statut toutes les 2 secondes
            DueDate = DateTime.Now;
            ActivityTypes = new ObservableCollection<ActivityType>();
            NavigateToChatCommand = new Command(() => CurrentViewIndex = 2);
            States = new ObservableCollection<string> { "All", "In Progress", "Done", "Cancelled" };
            FilteredActivities = new ObservableCollection<Activity>(Activities);
            LoadUsers();
            _timer = Dispatcher.GetForCurrentThread().CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(5); // Met à jour toutes les 2 secondes
            _timer.Tick += async (s, e) => await RefreshUnreadMessages();
            _timer.Start();
            ToggleCheckboxesCommand = new Command(() =>
            {
                ShowCheckboxes = !ShowCheckboxes;
            });
            
            // S'assurer que l'indicateur est désactivé au démarrage
            IsLoadingToLastMessage = false;
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
        // Appelé lorsqu'un nouveau message est reçu


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
                // Marquer les messages comme lus
                await MarkMessagesAsRead();
                
                // Envoyer un message pour indiquer qu'il faut scroller vers le dernier message
                MessagingCenter.Send(this, "ScrollToLastMessageWithoutAnimation");
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
            _markAsReadTimer?.Change(Timeout.Infinite, 0);
            _refreshMessagesTimer?.Change(Timeout.Infinite, 0);
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
            // IsLoadingToLastMessage est déjà activé par SelectedUser, pas besoin de le réactiver ici
            var P = Task.Run(() => DbConnection.Connecter3());
            Testcon = await P;
            if (Testcon == false)
            {
                TestLoad = true;
                IsLoadingToLastMessage = false; // Désactiver l'indicateur en cas d'erreur
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

                Device.BeginInvokeOnMainThread(async () =>
                {
                    Messages.Clear();
                    foreach (var message in sortedMessages)
                    {
                        Messages.Add(message);
                    }
                    
                    // Envoyer un message pour indiquer qu'il faut scroller vers le dernier message
                    MessagingCenter.Send(this, "ScrollToLastMessageWithoutAnimation");
                    
                    // Marquer automatiquement les messages comme lus après le chargement
                    await MarkMessagesAsRead();
                    
                    IsLoadingToLastMessage = false; // Désactiver l'indicateur après le chargement
                });
            }
            catch (Exception ex)
            {
                TestLoad = true;
                IsLoadingToLastMessage = false; // Désactiver l'indicateur en cas d'erreur
            }
            ActPopup = false;
        }

        /****************************************/
        /****************activity***************/


        public async Task LoadEmployeesAsync()
        {
            var employees = await Activity.GetEmployeesWithFullNamesAsync();
            Employees = new ObservableCollection<EmployeeDto>(employees);
        }


        private void LoadEmployeeForSelectedActivity()
        {
            if (SelectedActivity != null && Employees != null)
            {
                SelectedActivityEmployee = Employees.FirstOrDefault(e => e.Id == SelectedActivity.AssignedEmployee);
            }
        }

        /***********************************/
        /*****************memo**************/


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

        private async void MarkAsReadPeriodically(object state)
        {
            // Marquer les messages comme lus seulement si l'utilisateur est dans une conversation active
            if (SelectedUser != null && Messages.Any())
            {
                await MarkMessagesAsRead();
            }
        }

        private async void RefreshMessagesStatus(object state)
        {
            // Rafraîchir le statut des messages seulement si l'utilisateur est dans une conversation active
            if (SelectedUser != null)
            {
                try
                {
                    // Récupérer les messages depuis la base de données (même principe que LoadMessagesAsync)
                    var messages = await MessageModel.GetMessagesAsync(
                        CurrentData.CurrentModuleId,
                        CurrentData.CurrentNoteModule,
                        SelectedUser.Id,
                        userId);

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

                    // Mettre à jour l'interface avec les nouveaux statuts (même principe que LoadMessagesAsync)
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Messages.Clear();
                        foreach (var message in sortedMessages)
                        {
                            Messages.Add(message);
                        }
                        
                        // Scroller vers le dernier message
                        MessagingCenter.Send(this, "ScrollToLastMessageWithoutAnimation");
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du rafraîchissement du statut : {ex.Message}");
                }
            }
        }
    }
}
