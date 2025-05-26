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
    public class noteViewModel : INotifyPropertyChanged
    {
        int userId = Preferences.Get("iduser", 0);

        // Commandes pour les mémos
        public ICommand AddMemoCommand { get; }
        public ICommand LoadMemosCommand { get; }
        public ICommand SaveEditCommand { get; }
        public Command NavigateToMemoCommand { get; }
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
        public int EntityId { get; private set; }
        public string EntityType { get; private set; }
        public string EntityActivityType { get; private set; }
        public string EntityFormType { get; private set; }
        /********* Propriétés pour Memo *********/
        /*********memo*********/
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

        private ObservableCollection<Memo> _memos = new ObservableCollection<Memo>();
        public ObservableCollection<Memo> Memos
        {
            get => _memos;
            set
            {
                _memos = value;
                OnPropertyChanged(nameof(Memos));
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

        public noteViewModel()
        {
            EntityId = CurrentData.CurrentModuleId;
            EntityType = CurrentData.CurrentNoteModule;
            EntityActivityType = CurrentData.CurrentActivityModule;
            AddMemoCommand = new Command<object>(async (linkedObject) => await OnAddMemo(linkedObject));
            LoadMemosCommand = new Command(async () => await LoadMemos());
            SaveEditCommand = new Command(OnSaveEdit);
            NavigateToMemoCommand = new Command(() => LoadMemoView());

            // Charger les mémos au démarrage
            Task.Run(async () => await LoadMemos());
        }
        public noteViewModel(int entityId, string entityType, string entityActivityType)
        {
            EntityId = entityId;
            EntityType = entityType;
            EntityActivityType = entityActivityType;
            AddMemoCommand = new Command<object>(async (linkedObject) => await OnAddMemo(linkedObject));
            LoadMemosCommand = new Command(async () => await LoadMemos());
            SaveEditCommand = new Command(OnSaveEdit);
            NavigateToMemoCommand = new Command(() => LoadMemoView());

            // Charger les mémos au démarrage
            Task.Run(async () => await LoadMemos());
        }

        /***************** Méthodes pour Memo ***************/

        private void LoadMemoView()
        {
            Task.Run(async () => await LoadMemos());
        }

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
                var memos = await Memo.GetAllMemosForDrawerControl(CurrentData.CurrentModuleId, CurrentData.CurrentNoteModule);

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
        public void RefreshMemos()
        {
            LoadMemos();
        }
        private async void OnSaveEdit()
        {
            if (SelectedMemo == null) return;

            SelectedMemo.ModifyDate = DateTime.Now;
            SelectedMemo.Modify_user = userId;

            bool isUpdated = await Memo.UpdateMemoInDatabase(SelectedMemo);
            if (isUpdated)
            {
                await LoadMemos();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
