using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Acr.UserDialogs;
using SmartPharma5.Model;

namespace SmartPharma5.ModelView
{
    public class MemoViewModel : INotifyPropertyChanged
    {
        private readonly IDispatcherTimer _timer;
        int userId = Preferences.Get("iduser", 0);
        public ICommand AddMemoCommand { get; }
        public ICommand LoadMemosCommand { get; }
        public ICommand SwipeRightCommand { get; }
        public ICommand SwipeLeftCommand { get; }
        public ICommand MarkAsDoneCommand { get; }
        public ICommand SaveEditCommand { get; }
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
        private string _activityMemo;

        public string ActivityMemo
        {
            get => _activityMemo;
            set
            {
                _activityMemo = value;
                OnPropertyChanged();
            }
        }

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
        public int EntityId { get; private set; }
        public string EntityType { get; private set; }
        public string EntityActivityType { get; private set; }
        public MemoViewModel(int entityId, string entityType, string entityActivityType)
        {
            EntityId = entityId;
            EntityType = entityType;
            EntityActivityType = entityActivityType;
            AddMemoCommand = new Command<object>(async (linkedObject) => await OnAddMemo(linkedObject));
            Task.Run(async () => await LoadMemos());
            NavigateToMemoCommand = new Command(async () => await NavigateToMemoAsync());
            NavigateToActivityCommand = new Command(() => CurrentViewIndex = 1);
            NavigateToChatCommand = new Command(() => CurrentViewIndex = 2);
            // Messages.Add(new MessageModel { Text = "User1: Hello!", Alignment = LayoutOptions.Start, BackgroundColor = Color.FromArgb("#88e388") });
            /*  Users = new ObservableCollection<UserchatModel>
               {
              new UserchatModel { ImageSource = "avatar1.png", Name = "John" },
              new UserchatModel { ImageSource = "avatar2.png", Name = "Alice" },
              new UserchatModel { ImageSource = "avatar3.png", Name = "Bob" },
              new UserchatModel { ImageSource = "avatar4.png", Name = "Emma" },
              new UserchatModel { ImageSource = "avatar5.png", Name = "Mike" },
              new UserchatModel { ImageSource = "avatar6.png", Name = "Sarah" },
              new UserchatModel { ImageSource = "avatar1.png", Name = "mika" },
              new UserchatModel { ImageSource = "avatar2.png", Name = "Alicon" }
               };
              FilteredUsers = new ObservableCollection<UserchatModel>(Users);*/

        }
        public MemoViewModel()
        {
            EntityId = CurrentData.CurrentModuleId;
            EntityType = CurrentData.CurrentNoteModule;
            EntityActivityType = CurrentData.CurrentActivityModule;
            AddMemoCommand = new Command<object>(async (linkedObject) => await OnAddMemo(linkedObject));
            Task.Run(async () => await LoadMemos());
            NavigateToMemoCommand = new Command(async () => await NavigateToMemoAsync());
            NavigateToActivityCommand = new Command(() => CurrentViewIndex = 1);
            NavigateToChatCommand = new Command(() => CurrentViewIndex = 2);
            // Messages.Add(new MessageModel { Text = "User1: Hello!", Alignment = LayoutOptions.Start, BackgroundColor = Color.FromArgb("#88e388") });
            /*  Users = new ObservableCollection<UserchatModel>
               {
              new UserchatModel { ImageSource = "avatar1.png", Name = "John" },
              new UserchatModel { ImageSource = "avatar2.png", Name = "Alice" },
              new UserchatModel { ImageSource = "avatar3.png", Name = "Bob" },
              new UserchatModel { ImageSource = "avatar4.png", Name = "Emma" },
              new UserchatModel { ImageSource = "avatar5.png", Name = "Mike" },
              new UserchatModel { ImageSource = "avatar6.png", Name = "Sarah" },
              new UserchatModel { ImageSource = "avatar1.png", Name = "John" },
              new UserchatModel { ImageSource = "avatar2.png", Name = "Alice" }
               };
              FilteredUsers = new ObservableCollection<UserchatModel>(Users);*/

        }
        private async Task NavigateToMemoAsync()
        {
            await LoadMemos();
            CurrentViewIndex = 0;
        }
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
        public async Task LoadMemos()
        {
            try
            {
                var memos = await Memo.GetAllMemosForDrawerControl(EntityId, EntityType);
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
            }
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
            if (string.IsNullOrWhiteSpace(NewMemoName) || string.IsNullOrWhiteSpace(NewMemoDescription))
            {
                return;
            }

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

}
