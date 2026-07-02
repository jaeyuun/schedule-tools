using GirderSchedule.Domain.Models.Settings;
using GirderSchedule.Dxf.Constants;

namespace GirderSchedule.Dxf.Styles
{
    public sealed class DxfLayerNameSet
    {
        public string FormLine { get; set; }
        public string FormText { get; set; }
        public string MemberForce { get; set; }
        public string Girder { get; set; }
        public string Rebar { get; set; }
        public string Stirrup { get; set; }
        public string Text { get; set; }

        public DxfLayerNameSet()
        {
            FormLine = DxfLayers.FormLine;
            FormText = DxfLayers.FormText;
            MemberForce = DxfLayers.Defpoint;
            Girder = DxfLayers.RcGir;
            Rebar = DxfLayers.Rebar;
            Stirrup = DxfLayers.Rebar;
            Text = DxfLayers.Text;
        }

        public string GetLayerName(DxfLayerRole role)
        {
            switch (role)
            {
                case DxfLayerRole.FormLine:
                    return FormLine;
                case DxfLayerRole.FormText:
                    return FormText;
                case DxfLayerRole.MemberForce:
                    return MemberForce;
                case DxfLayerRole.Girder:
                    return Girder;
                case DxfLayerRole.Rebar:
                    return Rebar;
                case DxfLayerRole.Stirrup:
                    return Stirrup;
                case DxfLayerRole.Text:
                    return Text;
                default:
                    return DxfLayers.Text;
            }
        }

        public string GetTemplateLayerName(DxfLayerRole role)
        {
            switch (role)
            {
                case DxfLayerRole.FormLine:
                    return DxfLayers.FormLine;
                case DxfLayerRole.FormText:
                    return DxfLayers.FormText;
                case DxfLayerRole.MemberForce:
                    return DxfLayers.Defpoint;
                case DxfLayerRole.Girder:
                    return DxfLayers.RcGir;
                case DxfLayerRole.Rebar:
                case DxfLayerRole.Stirrup:
                    return DxfLayers.Rebar;
                case DxfLayerRole.Text:
                    return DxfLayers.Text;
                default:
                    return DxfLayers.Text;
            }
        }
    }
}