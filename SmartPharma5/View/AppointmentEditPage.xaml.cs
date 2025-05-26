using DevExpress.Maui.Scheduler;
using DevExpress.Maui.Scheduler.Internal;
using Microsoft.Maui.Controls;
using SmartPharma5.Model;

namespace SmartPharma5.View;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AppointmentEditPage : ContentPage, IDialogService
{
    const string SaveIconName = "check";
    const string LightThemePostfix = "_light";
    const string DarkThemePostfix = "_dark";
    const string FileResolution = ".png";
    public List<Contact_Partner> ListContacts;
    readonly ViewModel.AppointmentEditViewModel viewModel;
    readonly bool useThemeableToolbarIcons;

    bool inNavigation = false;
    public AppointmentEditPage()
    {
        InitializeComponent();
        //LoadTheme();
        //ThemeManager.AddThemeChangedHandler(this);
    }





    public AppointmentEditPage(DateTime startDate, DateTime endDate, bool allDay, SchedulerDataStorage storage, bool useThemeableToolbarIcons = false) : this()
    {
        BindingContext = viewModel = new ViewModel.AppointmentEditViewModel(startDate, endDate, allDay, storage);
        viewModel.DialogService = this;
        this.useThemeableToolbarIcons = useThemeableToolbarIcons;
       // UpdateToolbarItems();
    }

    public AppointmentEditPage(AppointmentItem appointment, SchedulerDataStorage storage, bool useThemeableToolbarIcons = false) : this()
    {
        BindingContext = viewModel = new ViewModel.AppointmentEditViewModel(appointment, storage);
        viewModel.DialogService = this;
        this.useThemeableToolbarIcons = useThemeableToolbarIcons;
        //UpdateToolbarItems();
    }

    public AppointmentEditPage(ViewModel.AppointmentEditViewModel viewModel, bool useThemeableToolbarIcons = false) : this()
    {
        BindingContext = this.viewModel = viewModel;
        viewModel.DialogService = this;
        this.useThemeableToolbarIcons = useThemeableToolbarIcons;
       // UpdateToolbarItems();
    }


    public virtual Page CreateColorItemSelectPage(ColorItemSelectViewModel viewModel) => new ColorItemSelectPage(viewModel);
    public virtual Page CreateRecurrenceEditPage(RecurrenceEditViewModel viewModel) => new RecurrenceEditPage(viewModel, useThemeableToolbarIcons);
    public virtual Page CreateTimeZoneSelectPage(TimeZoneSelectViewModel viewModel) => new TimeZoneSelectPage(viewModel);
    public virtual Page CreateReminderAddPage(ReminderAddViewModel viewModel) => new ReminderAddPage(viewModel, useThemeableToolbarIcons);

    protected override void OnAppearing()
    {
        base.OnAppearing();
        inNavigation = false;
       // ApplySafeInsets();
        this.SizeChanged += OnSizeChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        this.SizeChanged -= OnSizeChanged;
    }

    void OnSizeChanged(object sender, EventArgs e)
    {
        //ApplySafeInsets();
    }

    async void OnSaveTapped(object sender, EventArgs e)
    {

        TimeSpan begintime = begin_time.TimeSpan.Value;
        TimeSpan endtime = end_time.TimeSpan.Value;
        //TimeSpan begintime = begin_time
        DateTime date1 = (DateTime)begin_date.Date;
        int day1 = date1.Day;
        DateTime date2 = (DateTime)end_date.Date;
        int day2 = date2.Day;
        if (date1 >= date2)
        {
            await App.Current.MainPage.DisplayAlert("Warning", "begin date bigger then end date", "Ok");

            return;

        }


        if (await viewModel.SaveChanges(date1, date2, begintime, endtime))
            await Navigation.PopAsync();
    }

    void OnLabelTapped(object sender, EventArgs e)
    {
        if (inNavigation)
            return;
        inNavigation = true;
        ColorItemSelectViewModel selectorViewModel = viewModel.CreateLabelSelectViewModel();
        Page editPage = CreateColorItemSelectPage(selectorViewModel);
        Navigation.PushAsync(editPage);
    }

    void OnStatusTapped(object sender, EventArgs e)
    {
        if (inNavigation)
            return;
        inNavigation = true;
        ColorItemSelectViewModel selectorViewModel = viewModel.CreateStatusSelectViewModel();
        Page editPage = CreateColorItemSelectPage(selectorViewModel);
        Navigation.PushAsync(editPage);
    }

    void OnRepeatTapped(object sender, EventArgs e)
    {
        if (inNavigation)
            return;
        inNavigation = true;
        RecurrenceEditViewModel editRecurrenceViewModel = viewModel.CreateRecurrenceEditViewModel();
        Page repeatEditPage = CreateRecurrenceEditPage(editRecurrenceViewModel);
        Navigation.PushAsync(repeatEditPage);
    }

    void OnTimeZoneTapped(object sender, EventArgs e)
    {
        if (inNavigation)
            return;
        inNavigation = true;
        TimeZoneSelectViewModel selectorViewModel = viewModel.CreateTimeZoneSelectViewModel();
        Page timeZoneEditPage = CreateTimeZoneSelectPage(selectorViewModel);
        Navigation.PushAsync(timeZoneEditPage);
    }

    void OnAddReminderTapped(object sender, EventArgs e)
    {
        if (inNavigation)
            return;
        inNavigation = true;
        ReminderAddViewModel addReminderViewModel = viewModel.CreateReminderAddViewModel();
        Page addReminderPage = CreateReminderAddPage(addReminderViewModel);
        Navigation.PushAsync(addReminderPage);
    }

    void OnCaptionTapped(object sender, EventArgs e)
    {
        eventNameEntry.Focus();
    }

    //void OnAllDayTapped(object sender, EventArgs e)
    //{
    //    allDaySwitch.IsToggled = !allDaySwitch.IsToggled;
    //}

    async void OnRemoveReminderTapped(object sender, System.EventArgs e)
    {
        if (!(sender is StackLayout btn))
            return;
        if (!(btn.Parent is QuizPartnerFormCalender item))
            return;
        if (!(item.BindingContext is ReminderViewModel reminder))
            return;

        viewModel.Reminders.Remove(reminder);
    }

   /* void IThemeChangingHandler.OnThemeChanged()
    {
        LoadTheme();
    }*/

    /*void LoadTheme()
    {
        if (ThemeManager.ThemeName == "Light")
            Xamarin.Forms.Application.Current.Resources.MergedDictionaries.Add(new EditorLightTheme());
        if (ThemeManager.ThemeName == "Dark")
            Xamarin.Forms.Application.Current.Resources.MergedDictionaries.Add(new EditorDarkTheme());
    }*/

    [Obsolete]
   /* void UpdateToolbarItems()
    {
        string actualPostfix = String.Empty;
        if (useThemeableToolbarIcons)
        {
            switch (ThemeManager.ThemeName)
            {
                case "Light":
                    actualPostfix = LightThemePostfix;
                    break;
                case "Dark":
                    actualPostfix = DarkThemePostfix;
                    break;
                default:
                    break;
            }
        }
        saveToolbarItem.Icon = new FileImageSource { File = SaveIconName + actualPostfix + FileResolution };
    }*/

    Task AnimateHeight(QuizPartnerFormCalender container, double oldValue, double newValue, uint length, Easing easing)
    {
        TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();
        Animation animation = new Animation(v => container.HeightRequest = v, oldValue, newValue);
        animation.Commit(container, "ContainerResize", length: length, easing: easing, finished: (d, b) => completionSource.SetResult(b));
        return completionSource.Task;
    }

    /*void ApplySafeInsets()
    {
        //var safeInsets = On<CommunityToolkit.Maui.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
        this.RecreateStyleWithHorizontalInsets("FormItemStyle", "FormItemStyleBase", typeof(Grid), safeInsets);
        this.RecreateStyleWithHorizontalInsets("FormDateTimeItemStyle", "FormDateTimeItemStyleBase", typeof(StackLayout), safeInsets);
        this.RecreateStyleWithHorizontalInsets("Wrapper", "WrapperBase", typeof(Frame), safeInsets);
        this.root.Margin = new Thickness(0, safeInsets.Top, 0, safeInsets.Bottom);
    }*/

    Task<bool> IDialogService.DisplayAlertMessage(string title, string message, string accept, string cancel) => DisplayAlert(title, message, accept, cancel);
    Task<string> IDialogService.DisplaySelectItemDialog(string title, string cancel, params string[] options) => DisplayActionSheet(title, cancel, null, options);
}