using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ModelView;
using Microsoft.Maui.Media;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Maui.Alerts;
using System.Globalization;
using System.Diagnostics;
using Tesseract;
using Newtonsoft.Json.Linq;

namespace SmartPharma5.View.FloatingActionButton;

public partial class MemoView : ContentPage, IDrawable
{
    #region Fields and Properties
    private noteViewModel viewModel;
    private Memo _selectedMemo;
    private bool isDrawingMode = false;
    private List<PointF> drawingPoints = new();
    private List<List<PointF>> drawingHistory = new();
    private List<PointF> currentStroke = new();
    private CancellationTokenSource cancellationTokenSource = null;
    private HandwritingDrawable drawingDrawable = new HandwritingDrawable();

    #endregion
    private string _newMemoDescription;
    public string NewMemoDescription
    {
        get => _newMemoDescription;
        set
        {
            _newMemoDescription = value;
            OnPropertyChanged(); // ou RaisePropertyChanged si tu utilises MVVM Toolkit
        }
    }

    #region Constructor and Initialization
    public MemoView()
    {
        InitializeComponent();
        viewModel = new noteViewModel();
        BindingContext = viewModel;
        string currentnoteModule = CurrentData.CurrentNoteModule;
        string currentavtivityModule = CurrentData.CurrentActivityModule;
        int moduleId = CurrentData.CurrentModuleId;
        DrawingView.Drawable = drawingDrawable;
        DrawingView.StartInteraction += OnStartDrawing;
        DrawingView.DragInteraction += OnDrawing;
        DrawingView.EndInteraction += OnEndDrawing;
        //MessagingCenter.Subscribe<DrawingView, string>(this, "OcrCompleted", (sender, text) =>
        //{
        //    note.Text = text; // ⬅️ Affecte le texte à l'Editor
        //});

    }
    public Editor GetNoteEditor()
    {
        return note;
    }

    public void Initialize(int entityId, string entityType, string entityActivityType)
    {
        entityId = CurrentData.CurrentModuleId;
        entityType = CurrentData.CurrentNoteModule;
        entityActivityType = CurrentData.CurrentActivityModule;
        viewModel = new noteViewModel(entityId, entityType, entityActivityType);
        BindingContext = viewModel;
    }
    #endregion

    #region Drawing Methods
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 3;

        foreach (var stroke in drawingHistory)
        {
            if (stroke.Count > 1)
            {
                var path = new PathF();
                path.MoveTo(stroke[0]);
                for (int i = 1; i < stroke.Count; i++)
                {
                    path.LineTo(stroke[i]);
                }
                canvas.DrawPath(path);
            }
        }

        if (currentStroke.Count > 1)
        {
            var currentPath = new PathF();
            currentPath.MoveTo(currentStroke[0]);
            for (int i = 1; i < currentStroke.Count; i++)
            {
                currentPath.LineTo(currentStroke[i]);
            }
            canvas.DrawPath(currentPath);
        }
    }

    private void OnStartDrawing(object sender, TouchEventArgs e)
    {
        if (!isDrawingMode) return;
        drawingDrawable.CurrentStroke = new List<PointF>();
        drawingDrawable.CurrentStroke.Add(e.Touches[0]);
    }

    private void OnDrawing(object sender, TouchEventArgs e)
    {
        if (!isDrawingMode) return;
        drawingDrawable.CurrentStroke.Add(e.Touches[0]);
        DrawingView.Invalidate();
    }

    private void OnEndDrawing(object sender, TouchEventArgs e)
    {
        if (!isDrawingMode || drawingDrawable.CurrentStroke.Count == 0) return;
        drawingDrawable.CurrentStroke.Add(e.Touches[0]);
        drawingDrawable.DrawingHistory.Add(new List<PointF>(drawingDrawable.CurrentStroke));
        drawingDrawable.CurrentStroke.Clear();
        DrawingView.Invalidate();
    }
    #endregion

    #region Drawing Control Methods
    private async void OnDrawingClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DrawingView());
    }

    //private async void OnDrawingClicked(object sender, EventArgs e)
    //{
    //    isDrawingMode = !isDrawingMode;

    //    if (isDrawingMode)
    //    {
    //        await EnableHandwritingMode();
    //        DrawingOverlay.IsVisible = true;
    //        drawingPoints.Clear();
    //        DrawingView.Invalidate();
    //    }
    //    else
    //    {
    //        await DisableHandwritingMode();
    //        DrawingOverlay.IsVisible = false;
    //    }
    //}

    private async Task EnableHandwritingMode()
    {
        MicButton.IsEnabled = false;
        DrawingButton.BackgroundColor = Colors.LightGray;
        await Toast.Make("Mode écriture activé - Dessinez dans la zone").Show();
    }

    private async Task DisableHandwritingMode()
    {
        MicButton.IsEnabled = true;
        DrawingButton.BackgroundColor = Colors.Transparent;
    }

    private void OnClearDrawing(object sender, EventArgs e)
    {
        drawingDrawable.DrawingHistory.Clear();
        drawingDrawable.CurrentStroke.Clear();
        DrawingView.Invalidate();
    }

    private void OnUndoClicked(object sender, EventArgs e)
    {
        if (drawingDrawable.DrawingHistory.Count > 0)
        {
            drawingDrawable.DrawingHistory.RemoveAt(drawingDrawable.DrawingHistory.Count - 1);
            DrawingView.Invalidate();
        }
    }
    #endregion

    #region Handwriting Recognition Methods
    private async void OnValidateDrawing(object sender, EventArgs e)
    {
        //bool hasValidDrawing = drawingHistory.Any(stroke => stroke.Count >= 1);
        bool hasValidDrawing = drawingDrawable.DrawingHistory.Any(stroke => stroke.Count >= 1);
        if (!hasValidDrawing)
        {
            await Toast.Make("Dessin trop court - tracez quelque chose").Show();
            return;
        }

        try
        {
            var screenshot = await DrawingView.CaptureAsync();
            using var stream = await screenshot.OpenReadAsync();
            string recognizedText = await RecognizeHandwriting(stream);
            note.Text += " " + recognizedText;

            isDrawingMode = false;
            DrawingOverlay.IsVisible = false;
            drawingHistory.Clear();
            currentStroke.Clear();
            await DisableHandwritingMode();
        }
        catch (Exception ex)
        {
            await Toast.Make($"Erreur: {ex.Message}").Show();
        }
    }

    private async Task<string> RecognizeHandwriting(Stream imageStream)
    {
        var filePath = Path.Combine(FileSystem.CacheDirectory, "drawing.png");
        using (var fileStream = File.Create(filePath))
        {
            await imageStream.CopyToAsync(fileStream);
        }

        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            try
            {
                var tessDataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");
                if (!Directory.Exists(tessDataPath))
                    Directory.CreateDirectory(tessDataPath);

                if (!File.Exists(Path.Combine(tessDataPath, "fra.traineddata")))
                {
                    using var client = new HttpClient();
                    var trainedData = await client.GetByteArrayAsync("https://github.com/tesseract-ocr/tessdata/raw/main/fra.traineddata");
                    await File.WriteAllBytesAsync(Path.Combine(tessDataPath, "fra.traineddata"), trainedData);
                }

                using var engine = new TesseractEngine(tessDataPath, "fra", EngineMode.Default);
                using var img = Pix.LoadFromFile(filePath);
                using var page = engine.Process(img);
                return page.GetText().Trim();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Windows OCR Error: {ex.Message}");
            }
        }

        return await UseCloudOCRAlternative(imageStream);
    }

    private async Task<string> UseCloudOCRAlternative(Stream imageStream)
    {
        try
        {
            var apiKey = "helloworld";
            var apiUrl = "https://api.ocr.space/parse/image";

            using var client = new HttpClient();
            using var form = new MultipartFormDataContent();

            using var ms = new MemoryStream();
            await imageStream.CopyToAsync(ms);
            var imageBytes = ms.ToArray();

            form.Add(new ByteArrayContent(imageBytes), "file", "drawing.png");
            form.Add(new StringContent(apiKey), "apikey");
            form.Add(new StringContent("fre"), "language");
            form.Add(new StringContent("2"), "OCREngine");

            var response = await client.PostAsync(apiUrl, form);
            var result = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(result);
            if (json["ParsedResults"]?[0]?["ParsedText"] != null)
            {
                return json["ParsedResults"][0]["ParsedText"].ToString().Trim();
            }

            return "[Reconnaissance échouée]";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Cloud OCR Error: {ex.Message}");
            return "[Service temporairement indisponible]";
        }
    }
    #endregion

    #region Speech Recognition Methods
    private async Task Listen(CancellationToken cancellationToken)
    {
        var speechToText = SpeechToText.Default;

        var isGranted = await speechToText.RequestPermissions(cancellationToken);
        if (!isGranted)
        {
            await Toast.Make("Permission microphone non accordée").Show(CancellationToken.None);
            return;
        }

        string language = "fr-FR";
        //string language = "ang-ANG";
        string currentText = note.Text ?? "";

        var animationTask = SoundWaveAnimation(cancellationToken);

        try
        {
            var recognitionResult = await speechToText.ListenAsync(
                CultureInfo.GetCultureInfo(language),
                new Progress<string>(partialText =>
                {
                    note.Text = currentText + " " + partialText;
                }),
                cancellationToken);

            if (recognitionResult.IsSuccessful)
            {
                note.Text = currentText + " " + recognitionResult.Text;
            }
            else
            {
                await Toast.Make(recognitionResult.Exception?.Message ?? "Reconnaissance vocale échouée")
                           .Show(CancellationToken.None);
            }
        }
        finally
        {
            cancellationTokenSource?.Cancel();
            await animationTask;
        }
    }

    private async void OnListenClicked(object sender, EventArgs e)
    {
        if (cancellationTokenSource is { IsCancellationRequested: false })
        {
            await Toast.Make("Reconnaissance déjà en cours...").Show(CancellationToken.None);
            return;
        }

        cancellationTokenSource = new CancellationTokenSource();
        await Listen(cancellationTokenSource.Token);
    }

    private async Task SoundWaveAnimation(CancellationToken token)
    {
        try
        {
            RecordingIndicator.IsVisible = true;
            var rnd = new Random();

            while (!token.IsCancellationRequested)
            {
                var scale = 1 + rnd.NextDouble() * 0.5;
                var opacity = 0.3 + rnd.NextDouble() * 0.7;

                await Task.WhenAll(
                    RecordingIndicator.ScaleTo(scale, 100, Easing.Linear),
                    RecordingIndicator.FadeTo(opacity, 50, Easing.Linear)
                );

                await Task.Delay(50, token);
            }
        }
        catch (TaskCanceledException)
        {
            // Ignore, normal lors de l'annulation
        }
        finally
        {
            await RecordingIndicator.ScaleTo(1, 100);
            await RecordingIndicator.FadeTo(0, 150);
            RecordingIndicator.IsVisible = false;
        }
    }
    #endregion

    #region Memo Management Methods
    private async void OnDeleteMemoClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Memo memo)
        {
            bool confirmDelete = await App.Current.MainPage.DisplayAlert(
                "Confirm deletion\r\n",
                "Are you sure you want to delete this memo ?",
                "Yes",
                "No"
            );

            if (confirmDelete)
            {
                bool isDeleted = await Memo.DeleteMemoFromDatabase(memo.Id);
                if (isDeleted && BindingContext is noteViewModel viewModel)
                {
                    viewModel.Memos.Remove(memo);
                }
            }
        }
    }

    private void OnEditMemoClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Memo memo)
        {
            if (BindingContext is noteViewModel viewModel)
            {
                viewModel.SelectedMemo = memo;
                editMemoLayout.IsVisible = true;
                editDescriptionEntry.Text = memo.Description;
            }
        }
    }

    private void OnCancelEditClicked(object sender, EventArgs e)
    {
        editMemoLayout.IsVisible = false;
        if (BindingContext is noteViewModel viewModel)
        {
            viewModel.SelectedMemo = null;
        }
    }

    private void OnSaveEditClicked(object sender, EventArgs e)
    {
        if (BindingContext is noteViewModel viewModel)
        {
            if (viewModel.SelectedMemo != null)
            {
                viewModel.SelectedMemo.Description = editDescriptionEntry.Text;
            }
            viewModel.SaveEditCommand.Execute(null);
        }
    }
    #endregion

    #region Navigation Methods
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

    private async void OnActionButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        if (button != null)
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }
    }
    #endregion
}