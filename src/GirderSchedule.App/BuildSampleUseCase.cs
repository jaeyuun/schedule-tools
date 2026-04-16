using GirderSchedule.Application.Layout;
using GirderSchedule.Domain.Models;
using GirderSchedule.Infrastructure.Dxf;
using System.Collections.Generic;

namespace GirderSchedule.App
{
	public static class BuildSampleUseCase
	{
		public static void Execute(string path)
		{
			var sets = new List<GirderSetModel>();

			var set = new GirderSetModel();
			set.MemberName = "-1WG10";
			set.Width = 600;
			set.Height = 900;

			set.Left.IsVisible = true;
			set.Left.Title = "END (G2측)";
			set.Left.Type = SectionCellType.End;
			set.Left.Top1.Count = 4;
			set.Left.Top1.Diameter = 19;
			set.Left.Bottom1.Count = 4;
			set.Left.Bottom1.Diameter = 19;
			set.Left.Stirrup.Legs = 2;
			set.Left.Stirrup.Diameter = 10;
			set.Left.Stirrup.Spacing = 250;
			set.Left.SkinRebarText = "-";
			set.Left.Force.Visible = true;
			set.Left.Force.M = "";
			set.Left.Force.V = "";

			set.Center.IsVisible = true;
			set.Center.Title = "CENTER";
			set.Center.Type = SectionCellType.Center;
			set.Center.Top1.Count = 6;
			set.Center.Top1.Diameter = 19;
			set.Center.Bottom1.Count = 4;
			set.Center.Bottom1.Diameter = 19;
			set.Center.Stirrup.Legs = 3;
			set.Center.Stirrup.Diameter = 10;
			set.Center.Stirrup.Spacing = 250;
			set.Center.SkinRebarText = "-";
			set.Center.Force.Visible = true;
			set.Center.Force.M = "";
			set.Center.Force.V = "";

			set.Right.IsVisible = true;
			set.Right.Title = "END (EXT.)";
			set.Right.Type = SectionCellType.End;
			set.Right.Top1.Count = 11;
			set.Right.Top1.Diameter = 19;
			set.Right.Bottom1.Count = 7;
			set.Right.Bottom1.Diameter = 19;
			set.Right.Stirrup.Legs = 4;
			set.Right.Stirrup.Diameter = 10;
			set.Right.Stirrup.Spacing = 250;
			set.Right.SkinRebarText = "-";
			set.Right.Force.Visible = true;
			set.Right.Force.M = "1458";
			set.Right.Force.V = "1458";

			sets.Add(set);

			var pages = GirderPaginator.Paginate(sets);

			var exporter = new GirderDxfExporter();
			exporter.Save(path, pages);
		}
	}
}