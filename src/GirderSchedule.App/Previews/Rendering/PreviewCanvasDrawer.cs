using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GirderSchedule.App.Previews.Rendering
{
    public sealed class PreviewCanvasDrawer
    {
        public void DrawLine(Canvas canvas, double x1, double y1, double x2, double y2, Brush stroke, double thickness)
        {
            var line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = stroke,
                StrokeThickness = thickness
            };

            canvas.Children.Add(line);
        }

        public void DrawCircle(Canvas canvas, double centerX, double centerY, double radius, Brush stroke, double strokeThickness, Brush fill)
        {
            var circle = new Ellipse
            {
                Width = radius * 2.0,
                Height = radius * 2.0,
                Stroke = stroke,
                StrokeThickness = strokeThickness,
                Fill = fill
            };

            Canvas.SetLeft(circle, centerX - radius);
            Canvas.SetTop(circle, centerY - radius);
            canvas.Children.Add(circle);
        }

        public void DrawRectangle(Canvas canvas, double x, double y, double width, double height, Brush stroke, double thickness, Brush fill)
        {
            var rectangle = new Rectangle
            {
                Width = width,
                Height = height,
                Stroke = stroke,
                StrokeThickness = thickness,
                Fill = fill
            };

            Canvas.SetLeft(rectangle, x);
            Canvas.SetTop(rectangle, y);
            canvas.Children.Add(rectangle);
        }

        public void DrawText(Canvas canvas, string text, double x, double y, double fontSize, Brush foreground)
        {
            var block = new TextBlock
            {
                Text = text ?? string.Empty,
                FontSize = fontSize,
                Foreground = foreground
            };

            Canvas.SetLeft(block, x);
            Canvas.SetTop(block, y);
            canvas.Children.Add(block);
        }

        public void DrawTextBox(Canvas canvas, string text, double x, double y, double width, double height, double fontSize, Brush foreground)
        {
            var block = new TextBlock
            {
                Text = text ?? string.Empty,
                FontSize = fontSize,
                Foreground = foreground,
                TextAlignment = TextAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
                TextWrapping = TextWrapping.NoWrap,
                Width = width,
                LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                LineHeight = fontSize
            };

            block.Measure(new Size(width, double.PositiveInfinity));

            var top = y + (height - block.DesiredSize.Height) / 2.0;

            Canvas.SetLeft(block, x);
            Canvas.SetTop(block, top);
            canvas.Children.Add(block);
        }

        public void DrawCenteredText(Canvas canvas, double centerX, double centerY, string text, double fontSize, double angle, Brush foreground)
        {
            var textBlock = new TextBlock
            {
                Text = text ?? string.Empty,
                FontSize = fontSize,
                Foreground = foreground,
                FontWeight = FontWeights.SemiBold,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            Canvas.SetLeft(textBlock, centerX - textBlock.DesiredSize.Width / 2.0);
            Canvas.SetTop(textBlock, centerY - textBlock.DesiredSize.Height / 2.0);

            if (Math.Abs(angle) > 0.001)
            {
                textBlock.RenderTransform = new RotateTransform(angle);
            }

            canvas.Children.Add(textBlock);
        }
    }
}
