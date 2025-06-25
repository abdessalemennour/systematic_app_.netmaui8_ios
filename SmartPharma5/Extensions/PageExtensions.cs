using Microsoft.Maui.Controls;

namespace SmartPharma5.Extensions
{
    public static class PageExtensions
    {
        public static async Task ShowPopupAsync(this Page page, ContentView popup)
        {
            var grid = new Grid();
            grid.Children.Add(popup);
            
            var popupPage = new ContentPage
            {
                Content = grid,
                BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent
            };

            await page.Navigation.PushModalAsync(popupPage, false);
        }
    }
} 