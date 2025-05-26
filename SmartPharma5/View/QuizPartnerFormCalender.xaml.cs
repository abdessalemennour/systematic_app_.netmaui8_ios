using Acr.UserDialogs;
using DevExpress.Maui.Scheduler;
using DevExpress.Utils.About;
using Microsoft.Maui.Controls;
using MvvmHelpers;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View;

public partial class QuizPartnerFormCalender : ContentPage
{
    private double startScale = 1;
    private double currentScale = 1;
    public QuizPartnerFormCalender()
    {
        InitializeComponent();
        SetupPinchGesture();
    }
    public QuizPartnerFormCalender(ObservableRangeCollection<Partner_Form.Collection> partner_Forms)
    {
        InitializeComponent();
        BindingContext = new QuizPartnerFormCalenderViewModel(partner_Forms);
        SetupPinchGesture();
    }

    private async void ShowAppointmentEditPage(AppointmentItem appointment)
    {
        View.AppointmentEditPage appEditPage = new AppointmentEditPage(appointment, partnerformstorage);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(appEditPage));
        //Navigation.PushAsync(appEditPage);
    }

    private async void ShowNewAppointmentEditPage(IntervalInfo info)
    {
        //View.AppointmentEditPage appEditPage = new AppointmentEditPage(info.Start, info.End,info.AllDay, partnerformstorage);
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new AppointmentEditPage(info.Start, info.End, info.AllDay, partnerformstorage)));
    }

    private async void MonthView_LongPress(object sender, SchedulerGestureEventArgs e)
    {
        //if (e.AppointmentInfo == null)
        //{

        //    ShowNewAppointmentEditPage(e.IntervalInfo);
        //    return;
        //}






        
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        if (e.AppointmentInfo != null)
        {
            AppointmentItem appointment = e.AppointmentInfo.Appointment;
            var c = Cycle.GetCycleByPartnerFormId((int)e.AppointmentInfo.Appointment.Id);
            ShowAppointmentEditPage(appointment);

        }
        UserDialogs.Instance.HideLoading();
    }

    private async void MonthView_Tap(object sender, SchedulerGestureEventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        if (e.AppointmentInfo != null)
        {
            AppointmentItem appointment = e.AppointmentInfo.Appointment;
            //Partner_Form.Collection item = new Partner_Form.Collection((int)e.AppointmentInfo.Appointment.Id, e.AppointmentInfo.Appointment.Description, e.AppointmentInfo.Appointment.Subject, e.AppointmentInfo.Appointment.AllDay);
            await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new QuizQuestionView((int)appointment.Id)));
        }
        UserDialogs.Instance.HideLoading();
    }

    private async void SetupPinchGesture()
    {
        UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
        await Task.Delay(500);
        var pinchGestureRecognizer = new PinchGestureRecognizer();
        pinchGestureRecognizer.PinchUpdated += (sender, e) =>
        {
            if (e.Status == GestureStatus.Started)
            {
                startScale = Content.Scale;
                Content.AnchorX = 0;
                Content.AnchorY = 0;
            }

            if (e.Status == GestureStatus.Running)
            {
                double delta = 1 - (1 - e.Scale) * startScale;
                currentScale = Math.Max(1, delta * currentScale);
                Content.Scale = currentScale;
            }

            if (e.Status == GestureStatus.Completed)
            {
                startScale = Content.Scale;
            }
        };
        yourDayViewCell.GestureRecognizers.Add(pinchGestureRecognizer);
        UserDialogs.Instance.HideLoading();
    }

 
}