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
		private const double BarRadius = 3.0;
		private const double StirrupTouchOffset = BarRadius;

		public SectionColumnControl()
		{
			InitializeComponent();
			Loaded += SectionColumnControl_Loaded;
			SizeChanged += SectionColumnControl_SizeChanged;

			UpdateInputEnabled();
		}

		private void SectionColumnControl_Loaded(object sender, RoutedEventArgs e)
		{
			UpdateInputEnabled();
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

		private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as SectionColumnControl;
			if (control == null)
			{
				return;
			}

			control.UpdateInputEnabled();
			control.Redraw();
		}

		private void UpdateInputEnabled()
		{
			IsInputEnabled = !ShowNoneCheckBox || !IsNone;
		}

		private bool ShouldDrawNone()
		{
			return ShowNoneCheckBox && IsNone;
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

			if (ShouldDrawNone())
			{
				//DrawNoneSlash();
				return;
			}

			var layout = CreatePreviewLayout();

			DrawGirderOutline(layout);
			DrawMainRebars(layout);
			DrawStirrups(
				layout.BarStartX,
				layout.BarEndX,
				layout.SectionTop,
				layout.SectionBottom,
				TopCount1,
				BottomCount1,
				StirrupLegs,
				StirrupTouchOffset);

			DrawPreviewDimensions(
				layout.OuterLeft,
				layout.OuterRight,
				layout.OuterTop,
				layout.OuterBottom,
				layout.SideWing,
				PreviewWidth,
				PreviewHeight,
				IsWidthOver,
				IsHeightOver,
				layout.DimensionScale);
		}

		private PreviewLayout CreatePreviewLayout()
		{
			var widthValue = PreviewWidth;
			var heightValue = PreviewHeight;

			var areaWidth = PART_PreviewCanvas.ActualWidth;
			var areaHeight = PART_PreviewCanvas.ActualHeight;

			if (areaWidth <= 0 || areaHeight <= 0)
			{
				areaWidth = 200;
				areaHeight = 220;
			}

			var sectionUnitWidth = widthValue > 0 ? (double)widthValue : 600.0;
			var sectionUnitHeight = heightValue > 0 ? (double)heightValue : 900.0;

			const double topGapUnit = 30.0;
			const double sideDropUnit = 150.0;
			const double sideWingUnit = 175.0;
			const double sideOffsetUnit = 25.0;
			const double bottomOffsetUnit = 30.0;
			const double rebarCoverUnit = 25.0;
			const double secondLayerGapUnit = 40.0;

			const double dimensionTopSpaceUnit = 95.0;
			const double dimensionLeftSpaceUnit = 95.0;
			const double dimensionRightSpaceUnit = 35.0;
			const double dimensionBottomSpaceUnit = 25.0;

			var outerUnitWidth = sectionUnitWidth + sideOffsetUnit * 2.0;
			var outerUnitHeight = topGapUnit + sectionUnitHeight + bottomOffsetUnit;

			var modelWidth =
				dimensionLeftSpaceUnit +
				sideWingUnit +
				outerUnitWidth +
				sideWingUnit +
				dimensionRightSpaceUnit;

			var modelHeight =
				dimensionTopSpaceUnit +
				outerUnitHeight +
				dimensionBottomSpaceUnit;

			var scaleX = areaWidth * 0.9 / modelWidth;
			var scaleY = areaHeight * 0.9 / modelHeight;
			var scale = Math.Min(scaleX, scaleY);

			if (scale <= 0)
			{
				scale = 1.0;
			}

			const double shapeScaleFactor = 0.82;

			var shapeScale = scale * shapeScaleFactor;
			var dimensionScale = scale;

			var originX = (areaWidth - modelWidth * scale) / 2.0;
			var originY = (areaHeight - modelHeight * scale) / 2.0;

			var topGap = topGapUnit * shapeScale;
			var sideDrop = sideDropUnit * shapeScale;
			var sideWing = sideWingUnit * shapeScale;
			var sideOffset = sideOffsetUnit * shapeScale;
			var bottomOffset = bottomOffsetUnit * shapeScale;
			var rebarCover = Math.Max(4.0, rebarCoverUnit * shapeScale);
			var secondLayerGap = Math.Max(7.0, secondLayerGapUnit * shapeScale);

			var drawWidth = sectionUnitWidth * shapeScale;
			var drawHeight = sectionUnitHeight * shapeScale;

			var dimensionLeftSpace = dimensionLeftSpaceUnit * dimensionScale;
			var dimensionRightSpace = dimensionRightSpaceUnit * dimensionScale;
			var dimensionTopSpace = dimensionTopSpaceUnit * dimensionScale;
			var dimensionBottomSpace = dimensionBottomSpaceUnit * dimensionScale;

			var shapeModelWidth =
				sideWingUnit * shapeScale +
				sideOffsetUnit * shapeScale +
				sectionUnitWidth * shapeScale +
				sideOffsetUnit * shapeScale +
				sideWingUnit * shapeScale;

			var shapeModelHeight =
				topGapUnit * shapeScale +
				sectionUnitHeight * shapeScale +
				bottomOffsetUnit * shapeScale;

			var availableShapeWidth =
				modelWidth * scale -
				dimensionLeftSpace -
				dimensionRightSpace;

			var availableShapeHeight =
				modelHeight * scale -
				dimensionTopSpace -
				dimensionBottomSpace;

			var shapeStartX =
				originX +
				dimensionLeftSpace +
				(availableShapeWidth - shapeModelWidth) / 2.0;

			var shapeStartY =
				originY +
				dimensionTopSpace +
				(availableShapeHeight - shapeModelHeight) / 2.0;

			var outerLeft = shapeStartX + sideWing;
			var outerTop = shapeStartY;

			var outerRight = outerLeft + drawWidth + sideOffset * 2.0;
			var outerShelfY = outerTop + sideDrop;
			var outerBottom = outerTop + topGap + drawHeight + bottomOffset;

			var sectionLeft = outerLeft + sideOffset;
			var sectionTop = outerTop + topGap;
			var sectionBottom = sectionTop + drawHeight;

			var barStartX = sectionLeft + rebarCover;
			var barEndX = sectionLeft + drawWidth - rebarCover;

			var topBarY1 = sectionTop + rebarCover;
			var topBarY2 = topBarY1 + secondLayerGap;

			var bottomBarY1 = sectionTop + drawHeight - rebarCover;
			var bottomBarY2 = bottomBarY1 - secondLayerGap;

			return new PreviewLayout
			{
				AreaWidth = areaWidth,
				AreaHeight = areaHeight,
				ShapeScale = shapeScale,
				DimensionScale = dimensionScale,
				OuterLeft = outerLeft,
				OuterRight = outerRight,
				OuterTop = outerTop,
				OuterShelfY = outerShelfY,
				OuterBottom = outerBottom,
				SectionLeft = sectionLeft,
				SectionTop = sectionTop,
				SectionBottom = sectionBottom,
				DrawWidth = drawWidth,
				DrawHeight = drawHeight,
				SideWing = sideWing,
				RebarCover = rebarCover,
				SecondLayerGap = secondLayerGap,
				BarStartX = barStartX,
				BarEndX = barEndX,
				TopBarY1 = topBarY1,
				TopBarY2 = topBarY2,
				BottomBarY1 = bottomBarY1,
				BottomBarY2 = bottomBarY2
			};
		}

		private void DrawGirderOutline(PreviewLayout layout)
		{
			var lineBrush = new SolidColorBrush(Color.FromRgb(25, 25, 25));

			var outer = new Path
			{
				Stroke = lineBrush,
				StrokeThickness = 1.5,
				Fill = Brushes.Transparent,
				Data = CreateGirderOuterGeometry(
					layout.OuterLeft,
					layout.OuterRight,
					layout.OuterTop,
					layout.OuterShelfY,
					layout.OuterBottom,
					layout.SideWing)
			};

			PART_PreviewCanvas.Children.Add(outer);

			var stirrup = new Rectangle
			{
				Width = layout.DrawWidth,
				Height = layout.DrawHeight,
				Stroke = lineBrush,
				StrokeThickness = 1.0,
				Fill = Brushes.Transparent
			};

			Canvas.SetLeft(stirrup, layout.SectionLeft);
			Canvas.SetTop(stirrup, layout.SectionTop);
			PART_PreviewCanvas.Children.Add(stirrup);
		}

		private void DrawMainRebars(PreviewLayout layout)
		{
			DrawFirstLayerRebars(layout.BarStartX, layout.BarEndX, layout.TopBarY1, TopCount1);
			DrawSecondLayerRebars(layout.BarStartX, layout.BarEndX, layout.TopBarY2, TopCount1, TopCount2);

			DrawFirstLayerRebars(layout.BarStartX, layout.BarEndX, layout.BottomBarY1, BottomCount1);
			DrawSecondLayerRebars(layout.BarStartX, layout.BarEndX, layout.BottomBarY2, BottomCount1, BottomCount2);
		}

		private void DrawNoneSlash()
		{
			var areaWidth = PART_PreviewCanvas.ActualWidth;
			var areaHeight = PART_PreviewCanvas.ActualHeight;

			if (areaWidth <= 0 || areaHeight <= 0)
			{
				areaWidth = 200;
				areaHeight = 220;
			}

			var margin = 18.0;

			var line = new Line
			{
				X1 = margin,
				Y1 = areaHeight - margin,
				X2 = areaWidth - margin,
				Y2 = margin,
				Stroke = new SolidColorBrush(Color.FromRgb(25, 25, 25)),
				StrokeThickness = 1.5
			};

			PART_PreviewCanvas.Children.Add(line);
		}

		private static Geometry CreateGirderOuterGeometry(
			double outerLeft,
			double outerRight,
			double outerTop,
			double outerShelfY,
			double outerBottom,
			double sideWing)
		{
			var geometry = new PathGeometry();

			var topLine = new PathFigure
			{
				StartPoint = new Point(outerLeft - sideWing, outerTop),
				IsClosed = false
			};

			topLine.Segments.Add(new LineSegment(new Point(outerRight + sideWing, outerTop), true));

			var lowerShape = new PathFigure
			{
				StartPoint = new Point(outerLeft - sideWing, outerShelfY),
				IsClosed = false
			};

			lowerShape.Segments.Add(new LineSegment(new Point(outerLeft, outerShelfY), true));
			lowerShape.Segments.Add(new LineSegment(new Point(outerLeft, outerBottom), true));
			lowerShape.Segments.Add(new LineSegment(new Point(outerRight, outerBottom), true));
			lowerShape.Segments.Add(new LineSegment(new Point(outerRight, outerShelfY), true));
			lowerShape.Segments.Add(new LineSegment(new Point(outerRight + sideWing, outerShelfY), true));

			geometry.Figures.Add(topLine);
			geometry.Figures.Add(lowerShape);

			return geometry;
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

		private void AddBar(double x, double y)
		{
			var ellipse = new Ellipse
			{
				Width = BarRadius * 2.0,
				Height = BarRadius * 2.0,
				Fill = new SolidColorBrush(Color.FromRgb(70, 70, 70))
			};

			Canvas.SetLeft(ellipse, x - BarRadius);
			Canvas.SetTop(ellipse, y - BarRadius);
			PART_PreviewCanvas.Children.Add(ellipse);
		}

		private void DrawStirrups(
			double barStartX,
			double barEndX,
			double topY,
			double bottomY,
			int topFirstLayerCount,
			int bottomFirstLayerCount,
			int stirrupLegs,
			double sideOffset)
		{
			if (stirrupLegs <= 2)
			{
				return;
			}

			if (topFirstLayerCount <= 2 || bottomFirstLayerCount <= 2)
			{
				return;
			}

			var availableLegs = Math.Min(stirrupLegs, bottomFirstLayerCount);
			var innerCount = availableLegs - 2;

			if (innerCount <= 0)
			{
				return;
			}

			var topXs = GetLayerRebarXs(barStartX, barEndX, topFirstLayerCount);
			var bottomXs = GetLayerRebarXs(barStartX, barEndX, bottomFirstLayerCount);
			var usedBottom = new bool[bottomFirstLayerCount];

			for (var i = 1; i <= innerCount; i++)
			{
				var rawIndex = (topFirstLayerCount - 1) * i / (double)(availableLegs - 1);
				var topIndex = (int)Math.Floor(rawIndex + 0.5 - 0.000001);

				if (topIndex <= 0)
				{
					topIndex = 1;
				}

				if (topIndex >= topFirstLayerCount - 1)
				{
					topIndex = topFirstLayerCount - 2;
				}

				var topX = topXs[topIndex];
				var direction = topIndex < topFirstLayerCount / 2.0 ? -1.0 : 1.0;

				var bottomIndex = FindNearestIndex(bottomXs, topX, usedBottom);

				if (bottomIndex < 0)
				{
					continue;
				}

				usedBottom[bottomIndex] = true;

				var bottomX = bottomXs[bottomIndex];

				var topLineX = topX + direction * sideOffset;
				var bottomLineX = bottomX + direction * sideOffset;

				topLineX = Clamp(topLineX, barStartX, barEndX);
				bottomLineX = Clamp(bottomLineX, barStartX, barEndX);

				AddStirrupLine(topLineX, bottomLineX, topY, bottomY);
			}
		}

		private void AddStirrupLine(double topX, double bottomX, double topY, double bottomY)
		{
			var line = new Line
			{
				X1 = topX,
				Y1 = topY,
				X2 = bottomX,
				Y2 = bottomY,
				Stroke = new SolidColorBrush(Color.FromRgb(25, 25, 25)),
				StrokeThickness = 1.0
			};

			PART_PreviewCanvas.Children.Add(line);
		}

		private double[] GetLayerRebarXs(double startX, double endX, int count)
		{
			if (count <= 0)
			{
				return new double[0];
			}

			if (count == 1)
			{
				return new[] { startX };
			}

			var values = new double[count];
			var spacing = (endX - startX) / (count - 1);

			for (var i = 0; i < count; i++)
			{
				values[i] = startX + spacing * i;
			}

			return values;
		}

		private static int FindNearestIndex(double[] values, double targetX, bool[] used)
		{
			var index = -1;
			var distance = double.MaxValue;

			for (var i = 0; i < values.Length; i++)
			{
				if (used != null && i < used.Length && used[i])
				{
					continue;
				}

				var currentDistance = Math.Abs(values[i] - targetX);

				if (currentDistance >= distance)
				{
					continue;
				}

				index = i;
				distance = currentDistance;
			}

			return index;
		}

		private static double Clamp(double value, double min, double max)
		{
			if (value < min)
			{
				return min;
			}

			if (value > max)
			{
				return max;
			}

			return value;
		}

		private void DrawPreviewDimensions(
			double outerLeft,
			double outerRight,
			double outerTop,
			double outerBottom,
			double sideWing,
			double widthValue,
			double heightValue,
			bool isWidthOver,
			bool isHeightOver,
			double scale)
		{
			var dimensionBrush = new SolidColorBrush(Color.FromRgb(25, 25, 25));

			const double auxGap = 12.0;

			var dimensionGap = Math.Max(24.0, 34.0 * scale);
			var extensionOver = Math.Max(8.0, 12.0 * scale);
			var tick = Math.Max(4.0, 8.0 * scale);
			var circleRadius = Math.Max(2.0, 3.0 * scale);
			var textFontSize = Math.Max(10.0, 16.0 * scale);

			var widthText = FormatDimensionText(widthValue, isWidthOver);
			var heightText = FormatDimensionText(heightValue, isHeightOver);

			var outerVisualLeft = outerLeft - sideWing;

			var topDimY = outerTop - dimensionGap;
			var topExtTop = topDimY - extensionOver;
			var topExtBottom = outerTop - auxGap;

			DrawLine(outerLeft, topDimY, outerRight, topDimY, dimensionBrush, 1.0);
			DrawLine(outerLeft, topExtTop, outerLeft, topExtBottom, dimensionBrush, 1.0);
			DrawLine(outerRight, topExtTop, outerRight, topExtBottom, dimensionBrush, 1.0);

			DrawDimensionMark(outerLeft, topDimY, circleRadius, tick, dimensionBrush);
			DrawDimensionMark(outerRight, topDimY, circleRadius, tick, dimensionBrush);

			DrawDimensionText(
				outerLeft + (outerRight - outerLeft) / 2.0,
				topDimY - textFontSize * 0.9,
				widthText,
				textFontSize,
				0.0,
				dimensionBrush);

			var leftDimX = outerVisualLeft - dimensionGap;
			var leftExtLeft = leftDimX - extensionOver;
			var leftExtRight = outerVisualLeft - auxGap;

			DrawLine(leftDimX, outerTop, leftDimX, outerBottom, dimensionBrush, 1.0);
			DrawLine(leftExtLeft, outerTop, leftExtRight, outerTop, dimensionBrush, 1.0);
			DrawLine(leftExtLeft, outerBottom, leftExtRight, outerBottom, dimensionBrush, 1.0);

			DrawDimensionMark(leftDimX, outerTop, circleRadius, tick, dimensionBrush);
			DrawDimensionMark(leftDimX, outerBottom, circleRadius, tick, dimensionBrush);

			DrawDimensionText(
				leftDimX - textFontSize * 0.9,
				outerTop + (outerBottom - outerTop) / 2.0,
				heightText,
				textFontSize,
				-90.0,
				dimensionBrush);
		}

		private string FormatDimensionText(double value, bool isOver)
		{
			var text = value.ToString(CultureInfo.InvariantCulture);

			if (!isOver)
			{
				return text;
			}

			return text + " 이상";
		}

		private void DrawLine(double x1, double y1, double x2, double y2, Brush stroke, double thickness)
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

			PART_PreviewCanvas.Children.Add(line);
		}

		private void DrawDimensionMark(
			double x,
			double y,
			double radius,
			double tick,
			Brush stroke)
		{
			var circle = new Ellipse
			{
				Width = radius * 2.0,
				Height = radius * 2.0,
				Stroke = stroke,
				StrokeThickness = 1.0,
				Fill = Brushes.Transparent
			};

			Canvas.SetLeft(circle, x - radius);
			Canvas.SetTop(circle, y - radius);
			PART_PreviewCanvas.Children.Add(circle);

			DrawLine(x - tick, y, x + tick, y, stroke, 1.0);
			DrawLine(x, y - tick, x, y + tick, stroke, 1.0);
		}

		private void DrawDimensionText(
			double centerX,
			double centerY,
			string text,
			double fontSize,
			double angle,
			Brush foreground)
		{
			var textBlock = new TextBlock
			{
				Text = text,
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

			PART_PreviewCanvas.Children.Add(textBlock);
		}

		private class PreviewLayout
		{
			public double AreaWidth { get; set; }
			public double AreaHeight { get; set; }

			public double ShapeScale { get; set; }
			public double DimensionScale { get; set; }

			public double OuterLeft { get; set; }
			public double OuterRight { get; set; }
			public double OuterTop { get; set; }
			public double OuterShelfY { get; set; }
			public double OuterBottom { get; set; }

			public double SectionLeft { get; set; }
			public double SectionTop { get; set; }
			public double SectionBottom { get; set; }

			public double DrawWidth { get; set; }
			public double DrawHeight { get; set; }

			public double SideWing { get; set; }
			public double RebarCover { get; set; }
			public double SecondLayerGap { get; set; }

			public double BarStartX { get; set; }
			public double BarEndX { get; set; }

			public double TopBarY1 { get; set; }
			public double TopBarY2 { get; set; }
			public double BottomBarY1 { get; set; }
			public double BottomBarY2 { get; set; }
		}

		public bool IsNone
		{
			get { return (bool)GetValue(IsNoneProperty); }
			set { SetValue(IsNoneProperty, value); }
		}

		public static readonly DependencyProperty IsNoneProperty =
			DependencyProperty.Register(nameof(IsNone), typeof(bool), typeof(SectionColumnControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnStateChanged));

		public bool ShowNoneCheckBox
		{
			get { return (bool)GetValue(ShowNoneCheckBoxProperty); }
			set { SetValue(ShowNoneCheckBoxProperty, value); }
		}

		public static readonly DependencyProperty ShowNoneCheckBoxProperty =
			DependencyProperty.Register(nameof(ShowNoneCheckBox), typeof(bool), typeof(SectionColumnControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnStateChanged));

		public bool IsInputEnabled
		{
			get { return (bool)GetValue(IsInputEnabledProperty); }
			private set { SetValue(IsInputEnabledProperty, value); }
		}

		public static readonly DependencyProperty IsInputEnabledProperty =
			DependencyProperty.Register(nameof(IsInputEnabled), typeof(bool), typeof(SectionColumnControl), new PropertyMetadata(true));

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

		public double PreviewWidth
		{
			get { return (double)GetValue(PreviewWidthProperty); }
			set { SetValue(PreviewWidthProperty, value); }
		}

		public static readonly DependencyProperty PreviewWidthProperty =
			DependencyProperty.Register(nameof(PreviewWidth), typeof(double), typeof(SectionColumnControl), new PropertyMetadata(0.0, OnPreviewPropertyChanged));

		public double PreviewHeight
		{
			get { return (double)GetValue(PreviewHeightProperty); }
			set { SetValue(PreviewHeightProperty, value); }
		}

		public static readonly DependencyProperty PreviewHeightProperty =
			DependencyProperty.Register(nameof(PreviewHeight), typeof(double), typeof(SectionColumnControl), new PropertyMetadata(0.0, OnPreviewPropertyChanged));

		public bool IsWidthOver
		{
			get { return (bool)GetValue(IsWidthOverProperty); }
			set { SetValue(IsWidthOverProperty, value); }
		}

		public static readonly DependencyProperty IsWidthOverProperty =
			DependencyProperty.Register(nameof(IsWidthOver), typeof(bool), typeof(SectionColumnControl), new PropertyMetadata(false, OnPreviewPropertyChanged));

		public bool IsHeightOver
		{
			get { return (bool)GetValue(IsHeightOverProperty); }
			set { SetValue(IsHeightOverProperty, value); }
		}

		public static readonly DependencyProperty IsHeightOverProperty =
			DependencyProperty.Register(nameof(IsHeightOver), typeof(bool), typeof(SectionColumnControl), new PropertyMetadata(false, OnPreviewPropertyChanged));

		public bool IsPreviewVisible
		{
			get { return (bool)GetValue(IsPreviewVisibleProperty); }
			set { SetValue(IsPreviewVisibleProperty, value); }
		}

		public static readonly DependencyProperty IsPreviewVisibleProperty =
			DependencyProperty.Register(nameof(IsPreviewVisible), typeof(bool), typeof(SectionColumnControl), new PropertyMetadata(true, OnPreviewPropertyChanged));
	}
}