using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.Model
{
    public class HandwritingDrawable : IDrawable
    {
        public List<List<PointF>> DrawingHistory { get; set; } = new List<List<PointF>>();
        public List<PointF> CurrentStroke { get; set; } = new List<PointF>();

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 3;

            // Dessiner l'historique
            foreach (var stroke in DrawingHistory)
            {
                DrawStroke(canvas, stroke);
            }

            // Dessiner le trait en cours
            DrawStroke(canvas, CurrentStroke);
        }

        private void DrawStroke(ICanvas canvas, List<PointF> stroke)
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
    }
}
