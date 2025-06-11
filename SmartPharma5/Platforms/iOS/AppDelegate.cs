using Foundation;
using UIKit;
using System;
using Microsoft.Maui;

namespace SmartPharma5
{
    [Register("AppDelegate")]

    public class AppDelegate : MauiUIApplicationDelegate
    {

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            try
            {
                Console.WriteLine("Initialisation de l'application...");
                return base.FinishedLaunching(application, launchOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERREUR D'INITIALISATION: {ex}");
                throw;
            }
        }

        protected override MauiApp CreateMauiApp()
        {
            try
            {
                Console.WriteLine("Création de l'application MAUI...");
                var mauiApp = MauiProgram.CreateMauiApp();
                Console.WriteLine("Application MAUI créée avec succès");
                return mauiApp;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERREUR DE CRÉATION MAUI: {ex}");
                throw;
            }
        }
    }
}