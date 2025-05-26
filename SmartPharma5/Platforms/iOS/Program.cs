using System;
using System.Diagnostics;
using UIKit;

namespace SmartPharma5
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Initialisation du logger avant tout
                InitLogger();

                Debug.WriteLine("Démarrage de l'application iOS...");

                UIApplication.Main(args, null, typeof(AppDelegate));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERREUR CRITIQUE AU DÉMARRAGE: {ex}");
                throw; // Conserve le crash pour le débogage
            }
        }

        static void InitLogger()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Debug.WriteLine($"EXCEPTION NON GÉRÉE: {e.ExceptionObject}");
            };
        }
    }
}