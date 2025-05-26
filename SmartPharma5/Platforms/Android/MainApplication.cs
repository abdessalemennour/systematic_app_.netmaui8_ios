using Acr.UserDialogs;
using Android.App;
using Android.Runtime;

namespace SmartPharma5
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }
        public override void OnCreate()
        {
            base.OnCreate();

            // Initialisez UserDialogs ici
            UserDialogs.Init(this);
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}