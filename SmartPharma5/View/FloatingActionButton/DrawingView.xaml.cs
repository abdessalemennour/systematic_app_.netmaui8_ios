using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Net.Http.Headers;
using System.Text;

namespace SmartPharma5.View.FloatingActionButton;

public partial class DrawingView : ContentPage
{
    private readonly SKPaint _paint = new()
    {
        Style = SKPaintStyle.Stroke,
        Color = SKColors.Black,
        StrokeWidth = 10,
        StrokeCap = SKStrokeCap.Round,
        IsAntialias = true
    };

    private SKPath _currentPath = new();
    private readonly List<SKPath> _paths = new();
    private readonly HttpClient _httpClient = new();
    public string RecognizedText { get; private set; }

    public DrawingView()
    {
        InitializeComponent();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private void OnCanvasPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        foreach (var path in _paths)
        {
            canvas.DrawPath(path, _paint);
        }
        canvas.DrawPath(_currentPath, _paint);
    }

    private void OnCanvasTouch(object sender, SKTouchEventArgs e)
    {
        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                _currentPath = new SKPath();
                _currentPath.MoveTo(e.Location);
                break;

            case SKTouchAction.Moved:
                _currentPath.LineTo(e.Location);
                break;

            case SKTouchAction.Released:
                _paths.Add(_currentPath);
                _currentPath = new SKPath();
                break;
        }
        DrawingCanvas.InvalidateSurface();
        e.Handled = true;
    }

    private async void OnOcrClicked(object sender, EventArgs e)
    {
        if (_paths.Count == 0)
        {
            await DisplayAlert("Attention", "Veuillez d'abord dessiner quelque chose", "OK");
            return;
        }

        try
        {
            // 1. Capturer le dessin
            var imageBytes = await CaptureDrawingAsync();

            // 2. Appeler l'API OCR
            var result = await CallOcrApiAsync(imageBytes);

            // 3. Afficher le r�sultat
            OcrResultLabel.Text = string.IsNullOrWhiteSpace(result)
                ? "Aucun texte d�tect�"
                : $"{result.Trim()}";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur OCR", ex.Message, "OK");
        }
    }
    private void OnUndoClicked(object sender, EventArgs e)
    {
        if (_paths.Count > 0)
        {
            _paths.RemoveAt(_paths.Count - 1);
            DrawingCanvas.InvalidateSurface(); // Redessiner sans le dernier chemin
        }
    }

    private async Task<byte[]> CaptureDrawingAsync()
    {
        var imageInfo = new SKImageInfo(
            (int)DrawingCanvas.CanvasSize.Width,
            (int)DrawingCanvas.CanvasSize.Height);

        using var surface = SKSurface.Create(imageInfo);
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);

        foreach (var path in _paths)
        {
            canvas.DrawPath(path, _paint);
        }

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
    private async void OnContinueClicked(object sender, EventArgs e)
    {
        // V�rifiez si le texte OCR est non vide
        if (!string.IsNullOrWhiteSpace(OcrResultLabel.Text))
        {
            // Obtenez la page parent (MemoView)
            var memoPage = Navigation.NavigationStack.FirstOrDefault(p => p is MemoView) as MemoView;

            if (memoPage != null)
            {
                // Utiliser la m�thode GetNoteEditor pour acc�der � l'Editor
                memoPage.GetNoteEditor().Text += " " + OcrResultLabel.Text;
            }
        }

        // Retourner � la page pr�c�dente (MemoView)
        await Navigation.PopAsync();
    }
    private async Task<string> CallOcrApiAsync(byte[] imageBytes)
    {
        try
        {
            var apiKey = "K82703345288957"; // Ne laisse pas "helloworld"

            var content = new MultipartFormDataContent
        {
            { new ByteArrayContent(imageBytes), "file", "drawing_test.png" },
            //{ new StringContent("eng"), "language" },
            { new StringContent("fre"), "language" },

            { new StringContent("true"), "isOverlayRequired" }
        };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);

            var apiUrl = "https://api.ocr.space/parse/image";

            var response = await _httpClient.PostAsync(apiUrl, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // Extraction simplifi�e (id�alement utiliser JSON parsing)
            var startIndex = json.IndexOf("\"ParsedText\":\"") + 14;
            var endIndex = json.IndexOf("\"", startIndex);
            var extractedText = json[startIndex..endIndex]
                .Replace("\\r\\n", "\n")
                .Replace("\\n", "\n");

            return extractedText;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur API OCR: {ex}");
            throw;
        }
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        _paths.Clear();
        _currentPath = new SKPath();
        DrawingCanvas.InvalidateSurface();
        OcrResultLabel.Text = "R�sultat OCR appara�tra ici...";
    }

    private async void OnCopyClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(OcrResultLabel.Text))
        {
            await Clipboard.Default.SetTextAsync(OcrResultLabel.Text);
            await DisplayAlert("Succ�s", "Texte copi� dans le presse-papiers", "OK");
        }
    }
}