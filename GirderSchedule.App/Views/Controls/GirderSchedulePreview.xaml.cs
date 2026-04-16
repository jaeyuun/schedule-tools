using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GirderSchedule.App.ViewModels;

namespace GirderSchedule.App.Views.Controls
{
	public partial class GirderSchedulePreview : UserControl
	{
		private INotifyPropertyChanged _notify;

		public GirderSchedulePreview()
		{
			InitializeComponent();
			Loaded += GirderSchedulePreview_Loaded;
			DataContextChanged += GirderSchedulePreview_DataContextChanged;
			SizeChanged += GirderSchedulePreview_SizeChanged;
		}

		private void GirderSchedulePreview_Loaded(object sender, RoutedEventArgs e)
		{
			Redraw();
		}

		private void GirderSchedulePreview_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Redraw();
		}

		private void GirderSchedulePreview_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (_notify != null)
			{
				_notify.PropertyChanged -= Notify_PropertyChanged;
				_notify = null;
			}

			_notify = e.NewValue as INotifyPropertyChanged;
			if (_notify != null)
			{
				_notify.PropertyChanged += Notify_PropertyChanged;
			}

			Redraw();
		}

		private void Notify_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Redraw();
		}

		private void Redraw()
		{
			var vm = DataContext as MainViewModel;
			if (vm == null)
			{
				return;
			}

			DrawSection(PART_LeftCanvas, vm.WidthValue, vm.HeightValue, vm.LeftTopCount, vm.LeftBottomCount, vm.LeftStirrupLegs, vm.ShowLeft);
			DrawSection(PART_CenterCanvas, vm.WidthValue, vm.HeightValue, vm.CenterTopCount, vm.CenterBottomCount, vm.CenterStirrupLegs, vm.ShowCenter);
			DrawSection(PART_RightCanvas, vm.WidthValue, vm.HeightValue, vm.RightTopCount, vm.RightBottomCount, vm.RightStirrupLegs, vm.ShowRight);
		}

		private void DrawSection(Canvas canvas, int widthValue, int heightValue, int topCount, int bottomCount, int stirrupLegs, bool isVisible)
		{
			canvas.Children.Clear();

			if (!isVisible)
			{
				return;
			}

			var areaWidth = canvas.ActualWidth;
			var areaHeight = canvas.ActualHeight;

			if (areaWidth <= 0 || areaHeight <= 0)
			{
				areaWidth = 200;
				areaHeight = 220;
			}

			var drawWidth = areaWidth * 0.38;
			var drawHeight = areaHeight * 0.55;

			if (widthValue > 0 && heightValue > 0)
			{
				var ratio = (double)widthValue / heightValue;
				if (ratio > 1.0)
				{
					drawHeight = drawWidth / ratio;
				}
				else
				{
					drawWidth = drawHeight * ratio;
				}
			}

			var left = (areaWidth - drawWidth) / 2.0;
			var top = (areaHeight - drawHeight) / 2.0 + 8.0;

			var outer = new Rectangle
			{
				Width = drawWidth,
				Height = drawHeight,
				Stroke = new SolidColorBrush(Color.FromRgb(70, 70, 70)),
				StrokeThickness = 2
			};
			Canvas.SetLeft(outer, left);
			Canvas.SetTop(outer, top);
			canvas.Children.Add(outer);

			var stirrupInset = 10.0;
			var stirrup = new Rectangle
			{
				Width = Math.Max(10, drawWidth - stirrupInset * 2),
				Height = Math.Max(10, drawHeight - stirrupInset * 2),
				Stroke = new SolidColorBrush(Color.FromRgb(90, 90, 90)),
				StrokeThickness = 1.5
			};
			Canvas.SetLeft(stirrup, left + stirrupInset);
			Canvas.SetTop(stirrup, top + stirrupInset);
			canvas.Children.Add(stirrup);

			DrawRebars(canvas, left + stirrupInset + 6, left + drawWidth - stirrupInset - 6, top + 10, topCount, true);
			DrawRebars(canvas, left + stirrupInset + 6, left + drawWidth - stirrupInset - 6, top + drawHeight - 10, bottomCount, false);
			DrawStirrups(canvas, left + stirrupInset + 6, left + drawWidth - stirrupInset - 6, top + stirrupInset, top + drawHeight - stirrupInset, stirrupLegs);

			DrawDimensionText(canvas, left + drawWidth / 2.0 - 12, top - 24, widthValue.ToString(CultureInfo.InvariantCulture));
			DrawDimensionText(canvas, left - 28, top + drawHeight / 2.0 - 8, heightValue.ToString(CultureInfo.InvariantCulture));
		}

		private void DrawRebars(Canvas canvas, double startX, double endX, double y, int count, bool isTop)
		{
			if (count <= 0)
			{
				return;
			}

			if (count == 1)
			{
				AddBar(canvas, (startX + endX) / 2.0, y);
				return;
			}

			var spacing = (endX - startX) / (count - 1);
			for (var i = 0; i < count; i++)
			{
				AddBar(canvas, startX + spacing * i, y);
			}
		}

		private void AddBar(Canvas canvas, double x, double y)
		{
			var ellipse = new Ellipse
			{
				Width = 5,
				Height = 5,
				Fill = new SolidColorBrush(Color.FromRgb(70, 70, 70))
			};

			Canvas.SetLeft(ellipse, x - 2.5);
			Canvas.SetTop(ellipse, y - 2.5);
			canvas.Children.Add(ellipse);
		}

		private void DrawStirrups(Canvas canvas, double startX, double endX, double topY, double bottomY, int count)
		{
			if (count <= 0)
			{
				return;
			}

			if (count == 1)
			{
				AddStirrupLine(canvas, startX, topY, bottomY);
				return;
			}

			var spacing = (endX - startX) / (count - 1);
			for (var i = 0; i < count; i++)
			{
				AddStirrupLine(canvas, startX + spacing * i, topY, bottomY);
			}
		}

		private void AddStirrupLine(Canvas canvas, double x, double topY, double bottomY)
		{
			var line = new Line
			{
				X1 = x,
				Y1 = topY,
				X2 = x,
				Y2 = bottomY,
				Stroke = new SolidColorBrush(Color.FromRgb(110, 110, 110)),
				StrokeThickness = 1
			};
			canvas.Children.Add(line);
		}

		private void DrawDimensionText(Canvas canvas, double x, double y, string text)
		{
			var tb = new TextBlock
			{
				Text = text,
				FontSize = 12,
				Foreground = new SolidColorBrush(Color.FromRgb(60, 60, 60))
			};

			Canvas.SetLeft(tb, x);
			Canvas.SetTop(tb, y);
			canvas.Children.Add(tb);
		}
	}
}