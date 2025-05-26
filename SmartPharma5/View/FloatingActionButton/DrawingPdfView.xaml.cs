using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Acr.UserDialogs;

namespace SmartPharma5.View.FloatingActionButton;

public partial class DrawingPdfView : ContentPage
{
    private SKPath _currentPath = new();
    private readonly List<DrawingStroke> _strokes = new();

    private SKPaint _currentPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        Color = SKColors.Black,
        StrokeWidth = 10,
        StrokeCap = SKStrokeCap.Round,
        IsAntialias = true
    };

    public DrawingPdfView()
    {
        InitializeComponent();
        StrokeWidthSlider.Value = _currentPaint.StrokeWidth;
    }

    // Classe pour associer un chemin avec son style
    private class DrawingStroke
    {
        public SKPath Path { get; set; }
        public SKPaint Paint { get; set; }
    }

    private void OnCanvasPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        // Dessiner tous les traits avec leurs styles respectifs
        foreach (var stroke in _strokes)
        {
            canvas.DrawPath(stroke.Path, stroke.Paint);
        }

        // Dessiner le trait en cours avec le style actuel
        canvas.DrawPath(_currentPath, _currentPaint);
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
                // Créer une copie du style actuel pour ce trait
                var paintCopy = new SKPaint
                {
                    Style = _currentPaint.Style,
                    Color = _currentPaint.Color,
                    StrokeWidth = _currentPaint.StrokeWidth,
                    StrokeCap = _currentPaint.StrokeCap,
                    IsAntialias = _currentPaint.IsAntialias
                };

                _strokes.Add(new DrawingStroke { Path = _currentPath, Paint = paintCopy });
                _currentPath = new SKPath();
                break;
        }
        DrawingCanvas.InvalidateSurface();
        e.Handled = true;
    }

    // Gestion des couleurs
    private void OnColorBlackClicked(object sender, EventArgs e) => _currentPaint.Color = SKColors.Black;
    private void OnColorRedClicked(object sender, EventArgs e) => _currentPaint.Color = SKColors.Red;
    private void OnColorBlueClicked(object sender, EventArgs e) => _currentPaint.Color = SKColors.Blue;
    private void OnColorGreenClicked(object sender, EventArgs e) => _currentPaint.Color = SKColors.Green;

    private void OnStrokeWidthChanged(object sender, ValueChangedEventArgs e)
    {
        _currentPaint.StrokeWidth = (float)e.NewValue;
    }

    private void OnUndoClicked(object sender, EventArgs e)
    {
        if (_strokes.Count > 0)
        {
            _strokes.RemoveAt(_strokes.Count - 1);
            DrawingCanvas.InvalidateSurface();
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

        foreach (var stroke in _strokes)
        {
            canvas.DrawPath(stroke.Path, stroke.Paint);
        }

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        _strokes.Clear();
        _currentPath = new SKPath();
        DrawingCanvas.InvalidateSurface();
    }
    private async void OnContinueClicked(object sender, EventArgs e)
    {
        try
        {
            UserDialogs.Instance.ShowLoading("Conversion en cours...");

            // 1. Capturer le dessin comme image
            var imageData = await CaptureDrawingAsync();

            // 2. Créer un PDF avec cette image
            string fileName = $"Dessin_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            using (var stream = new MemoryStream(imageData))
            using (var document = SKDocument.CreatePdf(filePath))
            {
                var width = (int)DrawingCanvas.CanvasSize.Width;
                var height = (int)DrawingCanvas.CanvasSize.Height;

                using (var canvas = document.BeginPage(width, height))
                {
                    var image = SKImage.FromEncodedData(imageData);
                    canvas.DrawImage(image, 0, 0);
                }
                document.EndPage();
            }

            // 3. Retourner à la vue précédente avec les paramètres
            _strokes.Clear();
            _currentPath = new SKPath();
            DrawingCanvas.InvalidateSurface();

            // 4. Passer les données via MessagingCenter AVANT de retourner
            MessagingCenter.Send(this, "NewPdfCreated", new Tuple<string, string>(filePath, fileName));
            
            // Ajout d'un court délai pour s'assurer que le message est bien reçu
            await Task.Delay(200);
            
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Erreur lors de la conversion: {ex.Message}", "OK");
        }
        finally
        {
            UserDialogs.Instance.HideLoading();
        }
    }

    private void OnDraw(SKCanvas canvas)
    {
        foreach (var stroke in _strokes) // _paths doit contenir les traits dessinés
        {
        canvas.DrawPath(stroke.Path, stroke.Paint);
        }
    }
}