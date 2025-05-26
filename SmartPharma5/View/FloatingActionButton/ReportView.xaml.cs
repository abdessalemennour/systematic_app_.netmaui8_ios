using SmartPharma5.ModelView;
using CommunityToolkit.Maui.Alerts;
using System.Globalization;
using System.Threading;
using CommunityToolkit.Maui.Media;
namespace SmartPharma5.View.FloatingActionButton
{
    public partial class ReportView : ContentPage
    {
        public ReportView()
        {
            InitializeComponent();

            // Création et assignation du ViewModel
            this.BindingContext = new ReportViewModel();
        }
        CancellationTokenSource cancellationTokenSource = new();

        async Task Listen(CancellationToken cancellationToken)
        {
            var speechToText = SpeechToText.Default;

            var isGranted = await speechToText.RequestPermissions(cancellationToken);
            if (!isGranted)
            {
                await Toast.Make("Permission not granted").Show(CancellationToken.None);
                return;
            }

            string language = "fr-FR"; // ou "en-US"
            string RecognitionText = "";

            var recognitionResult = await speechToText.ListenAsync(
                CultureInfo.GetCultureInfo(language),
                new Progress<string>(partialText =>
                {
                    // Texte partiel en direct
                    RecognitionText += partialText + " ";
                }), cancellationToken);

            if (recognitionResult.IsSuccessful)
            {
                RecognitionText = recognitionResult.Text;
                await Toast.Make($"Reconnu : {RecognitionText}").Show(CancellationToken.None);
            }
            else
            {
                await Toast.Make(recognitionResult.Exception?.Message ?? "Unable to recognize speech").Show(CancellationToken.None);
            }
        }
        private async void OnListenClicked(object sender, EventArgs e)
        {
            await Listen(cancellationTokenSource.Token);
        }

    }
}