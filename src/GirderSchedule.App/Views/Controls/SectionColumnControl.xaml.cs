using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GirderSchedule.App.Views.Controls
{
	public partial class SectionColumnControl : UserControl
	{
		public SectionColumnControl()
		{
			InitializeComponent();
			Loaded += SectionColumnControl_Loaded;
			SizeChanged += SectionColumnControl_SizeChanged;
		}

		private void SectionColumnControl_Loaded(object sender, RoutedEventArgs e)
		{
			Redraw();
		}

		private void SectionColumnControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Redraw();
		}

		private static void OnPreviewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as SectionColumnControl;
			if (control == null)
			{
				return;
			}

			control.Redraw();
		}

		private void Redraw()
		{
			if (PART_PreviewCanvas == null)
			{
				return;
			}

			PART_PreviewCanvas.Children.Clear();

			if (!IsPreviewVisible)
			{
				return;
			}

			var widthValue = PreviewWidth;
			var heightValue = PreviewHeight;

			var topCount1 = TopCount1;
			var topCount2 = TopCount2;
			var bottomCount1 = BottomCount1;
			var bottomCount2 = BottomCount2;
			var stirrupLegs = StirrupLegs;

			var areaWidth = PART_PreviewCanvas.ActualWidth;
			var areaHeight = PART_PreviewCanvas.ActualHeight;

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
			PART_PreviewCanvas.Children.Add(outer);

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
			PART_PreviewCanvas.Children.Add(stirrup);

			var barStartX = left + stirrupInset + 6;
			var barEndX = left + drawWidth - stirrupInset - 6;

			var topBarY1 = top + 12;
			var topBarY2 = top + 24;
			var bottomBarY1 = top + drawHeight - 12;
			var bottomBarY2 = top + drawHeight - 24;

			DrawFirstLayerRebars(barStartX, barEndX, topBarY1, topCount1);
			DrawSecondLayerRebars(barStartX, barEndX, topBarY2, topCount1, topCount2);

			DrawFirstLayerRebars(barStartX, barEndX, bottomBarY1, bottomCount1);
			DrawSecondLayerRebars(barStartX, barEndX, bottomBarY2, bottomCount1, bottomCount2);

			DrawStirrups(barStartX, barEndX, top + stirrupInset, top + drawHeight - stirrupInset, stirrupLegs);

			DrawDimensionText(left + drawWidth / 2.0 - 12, top - 24, widthValue.ToString(CultureInfo.InvariantCulture));
			DrawDimensionText(left - 28, top + drawHeight / 2.0 - 8, heightValue.ToString(CultureInfo.InvariantCulture));
		}

		private void DrawFirstLayerRebars(double startX, double endX, double y, int count)
		{
			if (count <= 0)
			{
				return;
			}

			if (count == 1)
			{
				AddBar(startX, y);
				return;
			}

			var spacing = (endX - startX) / (count - 1);
			for (var i = 0; i < count; i++)
			{
				AddBar(startX + spacing * i, y);
			}
		}

		private void DrawSecondLayerRebars(double startX, double endX, double y, int firstLayerCount, int secondLayerCount)
		{
			if (firstLayerCount <= 0 || secondLayerCount <= 0)
			{
				return;
			}

			if (firstLayerCount == 1)
			{
				AddBar(startX, y);
				return;
			}

			var spacing = (endX - startX) / (firstLayerCount - 1);

			var leftCount = (secondLayerCount + 1) / 2;
			var rightCount = secondLayerCount / 2;

			var drawn = new bool[firstLayerCount];

			for (var i = 0; i < leftCount && i < firstLayerCount; i++)
			{
				drawn[i] = true;
			}

			for (var i = 0; i < rightCount && i < firstLayerCount; i++)
			{
				drawn[firstLayerCount - 1 - i] = true;
			}

			for (var i = 0; i < firstLayerCount; i++)
			{
				if (!drawn[i])
				{
					continue;
				}

				AddBar(startX + spacing * i, y);
			}
		}

		private void DrawRebars(double startX, double endX, double y, int count)
		{
			if (count <= 0)
			{
				return;
			}

			if (count % 2 == 0)
			{
				DrawEvenRebars(startX, endX, y, count);
				return;
			}

			var evenCount = count + 1;
			var spacing = (endX - startX) / (evenCount - 1);
			var removeIndex = evenCount - 2;

			for (var i = 0; i < evenCount; i++)
			{
				if (i == removeIndex)
				{
					continue;
				}

				AddBar(startX + spacing * i, y);
			}
		}

		private void DrawEvenRebars(double startX, double endX, double y, int count)
		{
			if (count <= 0)
			{
				return;
			}

			if (count == 1)
			{
				AddBar(startX, y);
				return;
			}

			var spacing = (endX - startX) / (count - 1);
			for (var i = 0; i < count; i++)
			{
				AddBar(startX + spacing * i, y);
			}
		}

		private void AddBar(double x, double y)
		{
			var ellipse = new Ellipse
			{
				Width = 6,
				Height = 6,
				Fill = new SolidColorBrush(Color.FromRgb(70, 70, 70))
			};

			Canvas.SetLeft(ellipse, x - 3);
			Canvas.SetTop(ellipse, y - 3);
			PART_PreviewCanvas.Children.Add(ellipse);
		}

		private void DrawStirrups(double startX, double endX, double topY, double bottomY, int count)
		{
			if (count <= 0)
			{
				return;
			}

			if (count == 1)
			{
				AddStirrupLine((startX + endX) / 2.0, topY, bottomY);
				return;
			}

			var spacing = (endX - startX) / (count - 1);
			for (var i = 0; i < count; i++)
			{
				AddStirrupLine(startX + spacing * i, topY, bottomY);
			}
		}

		private void AddStirrupLine(double x, double topY, double bottomY)
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
			PART_PreviewCanvas.Children.Add(line);
		}

		private void DrawDimensionText(double x, double y, string text)
		{
			var tb = new TextBlock
			{
				Text = text,
				FontSize = 12,
				Foreground = new SolidColorBrush(Color.FromRgb(60, 60, 60))
			};

			Canvas.SetLeft(tb, x);
			Canvas.SetTop(tb, y);
			PART_PreviewCanvas.Children.Add(tb);
		}

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register(nameof(Title), typeof(string), typeof(SectionColumnControl), new PropertyMetadata(string.Empty));

		public int MValue
		{
			get { return (int)GetValue(MValueProperty); }
			set { SetValue(MValueProperty, value); }
		}

		public static readonly DependencyProperty MValueProperty =
			DependencyProperty.Register(nameof(MValue), typeof(int), typeof(SectionColumnControl), new PropertyMetadata(0));

		public int VValue
		{
			get { return (int)GetValue(VValueProperty); }
			set { SetValue(VValueProperty, value); }
		}

		public static readonly DependencyProperty VValueProperty =
			DependencyProperty.Register(nameof(VValue), typeof(int), typeof(SectionColumnControl), new PropertyMetadata(0));

		public int TopCount1
		{
			get { return (int)GetValue(TopCount1Property); }
			set { SetValue(TopCount1Property, value); }
		}

		public static readonly DependencyProperty TopCount1Property =
			DependencyProperty.Register(nameof(TopCount1), typeof(int), typeof(SectionColumnControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPreviewPropertyChanged));

		public int TopCount2
		{
			get { return (int)GetValue(TopCount2Property); }
			set { SetValue(TopCount2Property, value); }
		}

		public static readonly DependencyProperty TopCount2Property =
			DependencyProperty.Register(nameof(TopCount2), typeof(int), typeof(SectionColumnControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPreviewPropertyChanged));

		public int TopDiameter
		{
			get { return (int)GetValue(TopDiameterProperty); }
			set { SetValue(TopDiameterProperty, value); }
		}

		public static readonly DependencyProperty TopDiameterProperty =
			DependencyProperty.Register(nameof(TopDiameter), typeof(int), typeof(SectionColumnControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public int BottomCount1
		{
			get { return (int)GetValue(BottomCount1Property); }
			set { SetValue(BottomCount1Property, value); }
		}

		public static readonly DependencyProperty BottomCount1Property =
			DependencyProperty.Register(nameof(BottomCount1), typeof(int), typeof(SectionColumnControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPreviewPropertyChanged));

		public int BottomCount2
		{
			get { return (int)GetValue(BottomCount2Property); }
			set { SetValue(BottomCount2Property, value); }
		}

		public static readonly DependencyProperty BottomCount2Property =
			DependencyProperty.Register(nameof(BottomCount2), typeof(int), typeof(SectionColumnControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPreviewPropertyChanged));

		public int BottomDiameter
		{
			get { return (int)GetValue(BottomDiameterProperty); }
			set { SetValue(BottomDiameterProperty, value); }
		}

		public static readonly DependencyProperty BottomDiameterProperty =
			DependencyProperty.Register(nameof(BottomDiameter), typeof(int), typeof(SectionColumnControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public int StirrupLegs
		{
			get { return (int)GetValue(StirrupLegsProperty); }
			set { SetValue(StirrupLegsProperty, value); }
		}

		public static readonly DependencyProperty StirrupLegsProperty =
			DependencyProperty.Register(nameof(StirrupLegs), typeof(int), typeof(SectionColumnControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPreviewPropertyChanged));

		public int StirrupDiameter
		{
			get { return (int)GetValue(StirrupDiameterProperty); }
			set { SetValue(StirrupDiameterProperty, value); }
		}

		public static readonly DependencyProperty StirrupDiameterProperty =
			DependencyProperty.Register(nameof(StirrupDiameter), typeof(int), typeof(SectionColumnControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public int StirrupSpacing
		{
			get { return (int)GetValue(StirrupSpacingProperty); }
			set { SetValue(StirrupSpacingProperty, value); }
		}

		public static readonly DependencyProperty StirrupSpacingProperty =
			DependencyProperty.Register(nameof(StirrupSpacing), typeof(int), typeof(SectionColumnControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public string SkinRebar
		{
			get { return (string)GetValue(SkinRebarProperty); }
			set { SetValue(SkinRebarProperty, value); }
		}

		public static readonly DependencyProperty SkinRebarProperty =
			DependencyProperty.Register(nameof(SkinRebar), typeof(string), typeof(SectionColumnControl), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public int PreviewWidth
		{
			get { return (int)GetValue(PreviewWidthProperty); }
			set { SetValue(PreviewWidthProperty, value); }
		}

		public static readonly DependencyProperty PreviewWidthProperty =
			DependencyProperty.Register(nameof(PreviewWidth), typeof(int), typeof(SectionColumnControl), new PropertyMetadata(0, OnPreviewPropertyChanged));

		public int PreviewHeight
		{
			get { return (int)GetValue(PreviewHeightProperty); }
			set { SetValue(PreviewHeightProperty, value); }
		}

		public static readonly DependencyProperty PreviewHeightProperty =
			DependencyProperty.Register(nameof(PreviewHeight), typeof(int), typeof(SectionColumnControl), new PropertyMetadata(0, OnPreviewPropertyChanged));

		public bool IsPreviewVisible
		{
			get { return (bool)GetValue(IsPreviewVisibleProperty); }
			set { SetValue(IsPreviewVisibleProperty, value); }
		}

		public static readonly DependencyProperty IsPreviewVisibleProperty =
			DependencyProperty.Register(nameof(IsPreviewVisible), typeof(bool), typeof(SectionColumnControl), new PropertyMetadata(true, OnPreviewPropertyChanged));
	}
}