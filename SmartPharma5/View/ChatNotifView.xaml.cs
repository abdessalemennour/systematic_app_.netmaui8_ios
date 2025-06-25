using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class ChatNotifView : ContentPage
{
    private ChatNotifViewModel viewModel;
    private Memo _selectedMemo;
    private IDisposable _refreshTimer;


    public void Initialize()
    {

        viewModel = new ChatNotifViewModel();
        BindingContext = viewModel;

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Ne pas charger automatiquement les messages au démarrage
        // Les messages seront chargés uniquement quand un utilisateur sera sélectionné
    }

    private void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is UserModel user)
        {
            var viewModel = (ChatNotifViewModel)BindingContext;
            viewModel.ToggleRecipient(user);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<ChatNotifViewModel>(this, "ScrollToLastMessageWithoutAnimation");
        viewModel.StopTimer();
    }

    public ChatNotifView()
    {
        InitializeComponent();
        viewModel = new ChatNotifViewModel();
        BindingContext = viewModel;
        string currentnoteModule = CurrentData.CurrentNoteModule;
        string currentavtivityModule = CurrentData.CurrentActivityModule;
        int moduleId = CurrentData.CurrentModuleId;
        
        // Écouter le message pour scroller vers le dernier message sans animation
        MessagingCenter.Subscribe<ChatNotifViewModel>(this, "ScrollToLastMessageWithoutAnimation", (sender) =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (MessagesCollectionView.ItemsSource != null && MessagesCollectionView.ItemsSource.Cast<object>().Any())
                {
                    MessagesCollectionView.ScrollTo(MessagesCollectionView.ItemsSource.Cast<object>().Last(),
                                                    position: ScrollToPosition.End,
                                                    animate: false);
                }
            });
        });
        
        // Marquer les messages comme lus quand l'utilisateur fait défiler
        MessagesCollectionView.Scrolled += async (sender, e) =>
        {
            // Indiquer que l'utilisateur fait défiler manuellement
            viewModel.IsUserScrolling = true;
            
            // Si l'utilisateur fait défiler vers le bas (vers les nouveaux messages)
            if (e.VerticalOffset > 0)
            {
                await viewModel.MarkMessagesAsRead();
            }
            
            // Réinitialiser le flag après un délai pour permettre le scroll automatique pour les nouveaux messages
            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    viewModel.IsUserScrolling = false;
                });
                return false; // Arrêter le timer après une seule exécution
            });
        };
        
        // Ajouter un événement pour détecter quand l'utilisateur atteint le bas de la liste
        MessagesCollectionView.SizeChanged += (sender, e) =>
        {
            // Si l'utilisateur est en bas de la liste, permettre le scroll automatique
            if (MessagesCollectionView.ItemsSource != null && MessagesCollectionView.ItemsSource.Cast<object>().Any())
            {
                var lastItem = MessagesCollectionView.ItemsSource.Cast<object>().Last();
                MessagesCollectionView.ScrollTo(lastItem, position: ScrollToPosition.MakeVisible, animate: false);
            }
        };
    }

    // Méthode pour détecter si l'utilisateur est en bas de la liste
    private bool IsUserAtBottom()
    {
        if (MessagesCollectionView.ItemsSource == null || !MessagesCollectionView.ItemsSource.Cast<object>().Any())
            return true;
            
        // Cette logique peut être améliorée selon les besoins
        // Pour l'instant, on considère que l'utilisateur est en bas s'il n'a pas fait défiler récemment
        return !viewModel.IsUserScrolling;
    }








    /***************vu*****************/

}