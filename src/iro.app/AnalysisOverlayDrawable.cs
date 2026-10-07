using Iro.Core.Analysis;
using Microsoft.Maui.Graphics;

namespace Iro.App;

// Presentation only: the original image and the analysis pixels are never modified.
public sealed class AnalysisOverlayDrawable(AnalysisPresentation presentation) : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (presentation.ImageWidth <= 0 || presentation.ImageHeight <= 0) return;
        float scale = Math.Min(dirtyRect.Width / presentation.ImageWidth, dirtyRect.Height / presentation.ImageHeight);
        float left = dirtyRect.X + (dirtyRect.Width - presentation.ImageWidth * scale) / 2;
        float top = dirtyRect.Y + (dirtyRect.Height - presentation.ImageHeight * scale) / 2;
        canvas.SaveState();
        foreach (var marker in presentation.Markers)
        {
            var color = marker.Kind == MarkerKind.Measured ? Colors.LimeGreen
                : marker.Kind == MarkerKind.Uncertain ? Colors.DarkOrange : Colors.Gray;
            canvas.StrokeColor = color;
            canvas.StrokeSize = marker.IsNearest ? 4 : 2;
            canvas.StrokeDashPattern = marker.Kind == MarkerKind.Uncertain ? new float[] { 5, 3 } : null;
            var r = marker.Bounds;
            if (marker.Polygon is { Count: > 2 } polygon)
            {
                var path = new PathF();
                path.MoveTo(left + (float)polygon[0].X * scale, top + (float)polygon[0].Y * scale);
                foreach (var point in polygon.Skip(1))
                    path.LineTo(left + (float)point.X * scale, top + (float)point.Y * scale);
                path.Close();
                canvas.DrawPath(path);
            }
            else canvas.DrawRectangle(left + r.X * scale, top + r.Y * scale, r.Width * scale, r.Height * scale);
            // Compact identity badges avoid covering adjacent fields in the fitted preview.
            // The same identity labels the row containing the value and its release state.
            string text = marker.Kind == MarkerKind.Uncertain ? "?" : marker.Label.Replace("Feld ", "");
            float labelWidth = Math.Min(dirtyRect.Width, 24);
            float x = Math.Clamp(left + r.X * scale, dirtyRect.X, dirtyRect.Right - labelWidth);
            float y = Math.Clamp(top + r.Y * scale - 22, dirtyRect.Y, Math.Max(dirtyRect.Y, dirtyRect.Bottom - 22));
            canvas.FillColor = Colors.Black.WithAlpha(.8f);
            canvas.FillRectangle(x, y, labelWidth, 22);
            canvas.FontColor = color; canvas.FontSize = 12;
            canvas.DrawString(text, x + 3, y, labelWidth - 6, 22, HorizontalAlignment.Left, VerticalAlignment.Center);
        }
        canvas.RestoreState();
    }
}
