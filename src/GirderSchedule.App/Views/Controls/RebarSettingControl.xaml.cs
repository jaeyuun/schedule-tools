using System.Windows;
using System.Windows.Controls;

namespace GirderSchedule.App.Views.Controls
{
	public partial class RebarSettingControl : UserControl
	{
		public RebarSettingControl()
		{
			InitializeComponent();
			UpdateTotalCount();
		}

		public int Count1
		{
			get { return (int)GetValue(Count1Property); }
			set { SetValue(Count1Property, value); }
		}

		public static readonly DependencyProperty Count1Property =
			DependencyProperty.Register(nameof(Count1), typeof(int), typeof(RebarSettingControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCountChanged));

		public int Count2
		{
			get { return (int)GetValue(Count2Property); }
			set { SetValue(Count2Property, value); }
		}

		public static readonly DependencyProperty Count2Property =
			DependencyProperty.Register(nameof(Count2), typeof(int), typeof(RebarSettingControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCountChanged));

		public int Diameter
		{
			get { return (int)GetValue(DiameterProperty); }
			set { SetValue(DiameterProperty, value); }
		}

		public static readonly DependencyProperty DiameterProperty =
			DependencyProperty.Register(nameof(Diameter), typeof(int), typeof(RebarSettingControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public int TotalCount
		{
			get { return (int)GetValue(TotalCountProperty); }
			private set { SetValue(TotalCountProperty, value); }
		}

		public static readonly DependencyProperty TotalCountProperty =
			DependencyProperty.Register(nameof(TotalCount), typeof(int), typeof(RebarSettingControl), new PropertyMetadata(0));

		private static void OnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as RebarSettingControl;
			if (control == null)
			{
				return;
			}

			control.UpdateTotalCount();
		}

		private void UpdateTotalCount()
		{
			TotalCount = Count1 + Count2;
		}
	}
}