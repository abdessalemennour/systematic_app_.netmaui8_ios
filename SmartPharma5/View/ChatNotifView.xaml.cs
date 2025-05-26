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
        await viewModel.LoadMessagesAsync();
        Device.BeginInvokeOnMainThread(async () =>
        {
            await viewModel.MarkMessagesAsRead();
            viewModel.ScrollToLastMessage();
        });
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
        MessagingCenter.Unsubscribe<ChatNotifViewModel>(this, "ScrollToLastMessage");
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
        //viewModel.ScrollToLastMessage = ScrollToLastMessage;

        // Les messages scrolleront automatiquement vers le bas après le chargement.
        MessagingCenter.Subscribe<ChatNotifViewModel>(this, "ScrollToLastMessage", (sender) =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (MessagesCollectionView.ItemsSource != null && MessagesCollectionView.ItemsSource.Cast<object>().Any())
                {
                    MessagesCollectionView.ScrollTo(MessagesCollectionView.ItemsSource.Cast<object>().Last(),
                                                    position: ScrollToPosition.End,
                                                    animate: true);
                }
            });
        });


    }








    /***************vu*****************/

}