using GirderSchedule.Dxf.Constants;

namespace GirderSchedule.Dxf.Styles
{
    public sealed class DxfLayerNameSet
    {
        public string Rebar { get; set; }
        public string Stirrup { get; set; }
        public string Form { get; set; }
        public string Girder { get; set; }
        public string Dimension { get; set; }
        public string TitleText { get; set; }
        public string ContentText { get; set; }
        public string Defpoint { get; set; }

        public DxfLayerNameSet()
        {
            Rebar = DxfLayers.Rebar;
            Stirrup = DxfLayers.Rebar;
            Form = DxfLayers.FormLine;
            Girder = DxfLayers.RcGir;
            Dimension = DxfLayers.Dim;
            TitleText = DxfLayers.FormText;
            ContentText = DxfLayers.Text;
            Defpoint = DxfLayers.Defpoint;
        }

        public string GetLayerName(DxfStyleRole role)
        {
            switch (role)
            {
                case DxfStyleRole.Rebar:
                    return Rebar;
                case DxfStyleRole.Stirrup:
                    return Stirrup;
                case DxfStyleRole.Form:
                    return Form;
                case DxfStyleRole.Girder:
                    return Girder;
                case DxfStyleRole.Dimension:
                    return Dimension;
                case DxfStyleRole.TitleText:
                    return TitleText;
                case DxfStyleRole.ContentText:
                    return ContentText;
                case DxfStyleRole.Defpoint:
                    return Defpoint;
                default:
                    return DxfLayers.Text;
            }
        }

        public string GetTemplateLayerName(DxfStyleRole role)
        {
            switch (role)
            {
                case DxfStyleRole.Rebar:
                case DxfStyleRole.Stirrup:
                    return DxfLayers.Rebar;
                case DxfStyleRole.Form:
                    return DxfLayers.FormLine;
                case DxfStyleRole.Girder:
                    return DxfLayers.RcGir;
                case DxfStyleRole.Dimension:
                    return DxfLayers.Dim;
                case DxfStyleRole.TitleText:
                    return DxfLayers.FormText;
                case DxfStyleRole.ContentText:
                    return DxfLayers.Text;
                case DxfStyleRole.Defpoint:
                    return DxfLayers.Defpoint;
                default:
                    return DxfLayers.Text;
            }
        }
    }
}
