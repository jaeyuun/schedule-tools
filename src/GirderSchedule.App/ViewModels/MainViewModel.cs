using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GirderSchedule.App.ViewModels.Export;
using GirderSchedule.App.ViewModels.Preview;
using GirderSchedule.App.Views;
using GirderSchedule.Application.Preview;
using GirderSchedule.Domain.Models;
using GirderSchedule.Infrastructure.Dxf;
using GirderSchedule.Infrastructure.Export;

namespace GirderSchedule.App.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		private string _memberName = "-1WG10";
		private int _width = 600;
		private int _height = 900;

		private bool _showLeft = true;
		private bool _showCenter = true;
		private bool _showRight = true;

		private string _leftTitle = "END (G2측)";
		private string _centerTitle = "CENTER";
		private string _rightTitle = "END (EXT.)";

		private int _leftTopCount = 4;
		private int _leftTopDiameter = 19;
		private int _leftBottomCount = 4;
		private int _leftBottomDiameter = 19;
		private int _leftStirrupLegs = 2;
		private int _leftStirrupDiameter = 10;
		private int _leftStirrupSpacing = 250;

		private int _centerTopCount = 6;
		private int _centerTopDiameter = 19;
		private int _centerBottomCount = 4;
		private int _centerBottomDiameter = 19;
		private int _centerStirrupLegs = 3;
		private int _centerStirrupDiameter = 10;
		private int _centerStirrupSpacing = 250;

		private int _rightTopCount = 11;
		private int _rightTopDiameter = 19;
		private int _rightBottomCount = 7;
		private int _rightBottomDiameter = 19;
		private int _rightStirrupLegs = 4;
		private int _rightStirrupDiameter = 10;
		private int _rightStirrupSpacing = 250;

		private string _leftM = string.Empty;
		private string _leftV = string.Empty;
		private string _centerM = string.Empty;
		private string _centerV = string.Empty;
		private string _rightM = "1458";
		private string _rightV = "1458";

		private bool _showForce = true;

		public GirderSchedulePreviewViewModel Preview { get; } = new GirderSchedulePreviewViewModel();
		public ObservableCollection<SectionColumnViewModel> Columns { get; private set; }

		public ICommand RefreshCommand { get; private set; }
		public ICommand OpenExportCommand { get; private set; }

		public MainViewModel()
		{
			RefreshCommand = new RelayCommand(RefreshPreview);
			OpenExportCommand = new RelayCommand(OpenExport);
			Columns = new ObservableCollection<SectionColumnViewModel>
			{
				new SectionColumnViewModel(),
				new SectionColumnViewModel(),
				new SectionColumnViewModel()
			};
			RefreshPreview();
		}

		public string MemberName
		{
			get { return _memberName; }
			set
			{
				SetProperty(ref _memberName, value);
				RefreshPreview();
			}
		}

		public int WidthValue
		{
			get { return _width; }
			set
			{
				SetProperty(ref _width, value);
				RefreshPreview();
			}
		}

		public int HeightValue
		{
			get { return _height; }
			set
			{
				SetProperty(ref _height, value);
				RefreshPreview();
			}
		}

		public bool ShowLeft
		{
			get { return _showLeft; }
			set
			{
				SetProperty(ref _showLeft, value);
				RefreshPreview();
			}
		}

		public bool ShowCenter
		{
			get { return _showCenter; }
			set
			{
				SetProperty(ref _showCenter, value);
				RefreshPreview();
			}
		}

		public bool ShowRight
		{
			get { return _showRight; }
			set
			{
				SetProperty(ref _showRight, value);
				RefreshPreview();
			}
		}

		public string LeftTitle
		{
			get { return _leftTitle; }
			set
			{
				SetProperty(ref _leftTitle, value);
				RefreshPreview();
			}
		}

		public string CenterTitle
		{
			get { return _centerTitle; }
			set
			{
				SetProperty(ref _centerTitle, value);
				RefreshPreview();
			}
		}

		public string RightTitle
		{
			get { return _rightTitle; }
			set
			{
				SetProperty(ref _rightTitle, value);
				RefreshPreview();
			}
		}

		public int LeftTopCount
		{
			get { return _leftTopCount; }
			set
			{
				SetProperty(ref _leftTopCount, value);
				RefreshPreview();
			}
		}

		public int LeftTopDiameter
		{
			get { return _leftTopDiameter; }
			set
			{
				SetProperty(ref _leftTopDiameter, value);
				RefreshPreview();
			}
		}

		public int LeftBottomCount
		{
			get { return _leftBottomCount; }
			set
			{
				SetProperty(ref _leftBottomCount, value);
				RefreshPreview();
			}
		}

		public int LeftBottomDiameter
		{
			get { return _leftBottomDiameter; }
			set
			{
				SetProperty(ref _leftBottomDiameter, value);
				RefreshPreview();
			}
		}

		public int LeftStirrupLegs
		{
			get { return _leftStirrupLegs; }
			set
			{
				SetProperty(ref _leftStirrupLegs, value);
				RefreshPreview();
			}
		}

		public int LeftStirrupDiameter
		{
			get { return _leftStirrupDiameter; }
			set
			{
				SetProperty(ref _leftStirrupDiameter, value);
				RefreshPreview();
			}
		}

		public int LeftStirrupSpacing
		{
			get { return _leftStirrupSpacing; }
			set
			{
				SetProperty(ref _leftStirrupSpacing, value);
				RefreshPreview();
			}
		}

		public int CenterTopCount
		{
			get { return _centerTopCount; }
			set
			{
				SetProperty(ref _centerTopCount, value);
				RefreshPreview();
			}
		}

		public int CenterTopDiameter
		{
			get { return _centerTopDiameter; }
			set
			{
				SetProperty(ref _centerTopDiameter, value);
				RefreshPreview();
			}
		}

		public int CenterBottomCount
		{
			get { return _centerBottomCount; }
			set
			{
				SetProperty(ref _centerBottomCount, value);
				RefreshPreview();
			}
		}

		public int CenterBottomDiameter
		{
			get { return _centerBottomDiameter; }
			set
			{
				SetProperty(ref _centerBottomDiameter, value);
				RefreshPreview();
			}
		}

		public int CenterStirrupLegs
		{
			get { return _centerStirrupLegs; }
			set
			{
				SetProperty(ref _centerStirrupLegs, value);
				RefreshPreview();
			}
		}

		public int CenterStirrupDiameter
		{
			get { return _centerStirrupDiameter; }
			set
			{
				SetProperty(ref _centerStirrupDiameter, value);
				RefreshPreview();
			}
		}

		public int CenterStirrupSpacing
		{
			get { return _centerStirrupSpacing; }
			set
			{
				SetProperty(ref _centerStirrupSpacing, value);
				RefreshPreview();
			}
		}

		public int RightTopCount
		{
			get { return _rightTopCount; }
			set
			{
				SetProperty(ref _rightTopCount, value);
				RefreshPreview();
			}
		}

		public int RightTopDiameter
		{
			get { return _rightTopDiameter; }
			set
			{
				SetProperty(ref _rightTopDiameter, value);
				RefreshPreview();
			}
		}

		public int RightBottomCount
		{
			get { return _rightBottomCount; }
			set
			{
				SetProperty(ref _rightBottomCount, value);
				RefreshPreview();
			}
		}

		public int RightBottomDiameter
		{
			get { return _rightBottomDiameter; }
			set
			{
				SetProperty(ref _rightBottomDiameter, value);
				RefreshPreview();
			}
		}

		public int RightStirrupLegs
		{
			get { return _rightStirrupLegs; }
			set
			{
				SetProperty(ref _rightStirrupLegs, value);
				RefreshPreview();
			}
		}

		public int RightStirrupDiameter
		{
			get { return _rightStirrupDiameter; }
			set
			{
				SetProperty(ref _rightStirrupDiameter, value);
				RefreshPreview();
			}
		}

		public int RightStirrupSpacing
		{
			get { return _rightStirrupSpacing; }
			set
			{
				SetProperty(ref _rightStirrupSpacing, value);
				RefreshPreview();
			}
		}

		public string LeftM
		{
			get { return _leftM; }
			set
			{
				SetProperty(ref _leftM, value);
				RefreshPreview();
			}
		}

		public string LeftV
		{
			get { return _leftV; }
			set
			{
				SetProperty(ref _leftV, value);
				RefreshPreview();
			}
		}

		public string CenterM
		{
			get { return _centerM; }
			set
			{
				SetProperty(ref _centerM, value);
				RefreshPreview();
			}
		}

		public string CenterV
		{
			get { return _centerV; }
			set
			{
				SetProperty(ref _centerV, value);
				RefreshPreview();
			}
		}

		public string RightM
		{
			get { return _rightM; }
			set
			{
				SetProperty(ref _rightM, value);
				RefreshPreview();
			}
		}

		public string RightV
		{
			get { return _rightV; }
			set
			{
				SetProperty(ref _rightV, value);
				RefreshPreview();
			}
		}

		public bool ShowForce
		{
			get { return _showForce; }
			set
			{
				SetProperty(ref _showForce, value);
				Preview.ShowForce = value;
				RefreshPreview();
			}
		}

		private void RefreshPreview()
		{
			var set = BuildCurrentSetModel();
			Preview.ShowForce = ShowForce;
			Preview.CurrentSet = set;
			Preview.Refresh();
		}

		private void OpenExport()
		{
			var document = BuildDocumentForExport();

			var dxfExporter = new GirderDxfExporter();
			var dxfService = new DxfExportService(dxfExporter);
			var pdfService = new PdfExportService();
			var jpgService = new JpgExportService();

			var vm = new ExportWindowViewModel(dxfService, pdfService, jpgService);
			vm.Load(document);

			var window = new ExportWindow();
			window.Owner = System.Windows.Application.Current.MainWindow;
			window.DataContext = vm;
			window.ShowDialog();
		}

		private GirderScheduleDocumentModel BuildDocumentForExport()
		{
			var sets = new List<GirderSetModel>();
			sets.Add(BuildCurrentSetModel());

			var service = new BuildExportPreviewPagesService();
			return service.Build(sets);
		}

		private GirderSetModel BuildCurrentSetModel()
		{
			var set = new GirderSetModel();
			set.MemberName = MemberName;
			set.Width = WidthValue;
			set.Height = HeightValue;

			set.Left.IsVisible = ShowLeft;
			set.Left.Title = LeftTitle;
			set.Left.Top1.Count = LeftTopCount;
			set.Left.Top1.Diameter = LeftTopDiameter;
			set.Left.Bottom1.Count = LeftBottomCount;
			set.Left.Bottom1.Diameter = LeftBottomDiameter;
			set.Left.Stirrup.Legs = LeftStirrupLegs;
			set.Left.Stirrup.Diameter = LeftStirrupDiameter;
			set.Left.Stirrup.Spacing = LeftStirrupSpacing;
			set.Left.Force.Visible = true;
			set.Left.Force.M = LeftM;
			set.Left.Force.V = LeftV;
			set.Left.SkinRebarText = HeightValue > 900 ? "X" : "-";

			set.Center.IsVisible = ShowCenter;
			set.Center.Title = CenterTitle;
			set.Center.Top1.Count = CenterTopCount;
			set.Center.Top1.Diameter = CenterTopDiameter;
			set.Center.Bottom1.Count = CenterBottomCount;
			set.Center.Bottom1.Diameter = CenterBottomDiameter;
			set.Center.Stirrup.Legs = CenterStirrupLegs;
			set.Center.Stirrup.Diameter = CenterStirrupDiameter;
			set.Center.Stirrup.Spacing = CenterStirrupSpacing;
			set.Center.Force.Visible = true;
			set.Center.Force.M = CenterM;
			set.Center.Force.V = CenterV;
			set.Center.SkinRebarText = HeightValue > 900 ? "X" : "-";

			set.Right.IsVisible = ShowRight;
			set.Right.Title = RightTitle;
			set.Right.Top1.Count = RightTopCount;
			set.Right.Top1.Diameter = RightTopDiameter;
			set.Right.Bottom1.Count = RightBottomCount;
			set.Right.Bottom1.Diameter = RightBottomDiameter;
			set.Right.Stirrup.Legs = RightStirrupLegs;
			set.Right.Stirrup.Diameter = RightStirrupDiameter;
			set.Right.Stirrup.Spacing = RightStirrupSpacing;
			set.Right.Force.Visible = true;
			set.Right.Force.M = RightM;
			set.Right.Force.V = RightV;
			set.Right.SkinRebarText = HeightValue > 900 ? "X" : "-";

			return set;
		}
	}
}