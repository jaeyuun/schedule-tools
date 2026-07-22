using GirderSchedule.Dxf.Constants;
using netDxf;
using ScheduleTools.Dxf.Validation;

namespace GirderSchedule.Dxf.Validation
{
    public sealed class DxfResourceValidator
    {
        private readonly DxfResourceRequirements _requirements = new DxfResourceRequirements
        {
            Layers = new[] { DxfLayers.FormLine, DxfLayers.FormText, DxfLayers.Text, DxfLayers.RcGir, DxfLayers.Rebar, DxfLayers.Dim, DxfLayers.Defpoint },
            DimensionStyles = new[] { DxfStyles.Dimension },
            Linetypes = new[] { DxfStyles.RcGirderLine },
            Blocks = new[] { DxfBlocks.RebarD19 }
        };

        public void Ensure(DxfDocument document)
        {
            _requirements.Ensure(document);
        }
    }
}
