using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View.FloatingActionButton;

public partial class ChatView : ContentPage
{
    private ChatViewModel viewModel;
    private Memo _selectedMemo;
    private IDisposable _refreshTimer;


    public void Initialize(int entityId, string entityType, string entityActivityType)
    {
        entityId = CurrentData.CurrentModuleId;
        entityType = CurrentData.CurrentNoteModule;
        entityActivityType = CurrentData.CurrentActivityModule;
        viewModel = new ChatViewModel(entityId, entityType, entityActivityType);
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
            var viewModel = (ChatViewModel)BindingContext;
            viewModel.ToggleRecipient(user);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<ChatViewModel>(this, "ScrollToLastMessageWithoutAnimation");
        viewModel.StopTimer();
    }

    public ChatView()
    {
        InitializeComponent();
        viewModel = new ChatViewModel();
        BindingContext = viewModel;
        string currentnoteModule = CurrentData.CurrentNoteModule;
        string currentavtivityModule = CurrentData.CurrentActivityModule;
        int moduleId = CurrentData.CurrentModuleId;
        
        // Écouter le message pour scroller vers le dernier message sans animation
        MessagingCenter.Subscribe<ChatViewModel>(this, "ScrollToLastMessageWithoutAnimation", (sender) =>
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
            // Si l'utilisateur fait défiler vers le bas (vers les nouveaux messages)
            if (e.VerticalOffset > 0)
            {
                await viewModel.MarkMessagesAsRead();
            }
        };
    }

    private void OnTabClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.CommandParameter is string viewName)
        {
            NavigateToView(viewName);
        }
    }
    private async void NavigateToView(string viewName)
    {
        switch (viewName)
        {
            case "Memo":
                await Navigation.PushAsync(new MemoView());
                break;
            case "Activity":
                await Navigation.PushAsync(new ActivityView());
                break;
            case "Chat":
                await Navigation.PushAsync(new ChatView());
                break;
            default:
                await DisplayAlert("Erreur", "Vue non trouvée", "OK");
                break;
        }
    }
    private async void OnActionButtonClickedchat(object sender, EventArgs e)
    {
        var button = sender as ImageButton;


        if (button.Source.ToString().Contains("chat.png"))
        {
            await Navigation.PushAsync(new ChatView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }

    }


    private async void OnActionButtonClickedmemo(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        if (button.Source.ToString().Contains("memo.png"))
        {
            await Navigation.PushAsync(new MemoView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }

    }
    private async void OnActionButtonClickedactivity(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        if (button.Source.ToString().Contains("activity.png"))
        {
            await Navigation.PushAsync(new ActivityView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }

    }


    /***************vu*****************/

}