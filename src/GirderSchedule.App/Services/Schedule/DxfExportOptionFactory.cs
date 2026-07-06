using GirderSchedule.Domain.Models.Settings;
using GirderSchedule.Dxf.Export;
using GirderSchedule.Dxf.Styles;
using System.Linq;

namespace GirderSchedule.App.Services.Schedule
{
    public static class DxfExportOptionFactory
    {
        public static DxfExportOptions Create(DxfSettingProfile profile, string templatePath, int formColumnCount)
        {
            var options = new DxfExportOptions();
            options.TemplatePath = templatePath;
            options.FormColumnCount = formColumnCount;
            options.LayerNames = CreateLayerNames(profile);
            options.StyleNames = CreateStyleNames(profile);
            return options;
        }

        private static DxfLayerNameSet CreateLayerNames(DxfSettingProfile profile)
        {
            var layerNames = new DxfLayerNameSet();

            if (profile == null || profile.LayerSettings == null)
            {
                return layerNames;
            }

            layerNames.FormLine = GetLayerName(profile, DxfLayerRole.FormLine, layerNames.FormLine);
            layerNames.FormText = GetLayerName(profile, DxfLayerRole.FormText, layerNames.FormText);
            layerNames.MemberForce = GetLayerName(profile, DxfLayerRole.MemberForce, layerNames.MemberForce);
            layerNames.Girder = GetLayerName(profile, DxfLayerRole.Girder, layerNames.Girder);
            layerNames.Rebar = GetLayerName(profile, DxfLayerRole.Rebar, layerNames.Rebar);
            layerNames.Stirrup = GetLayerName(profile, DxfLayerRole.Stirrup, layerNames.Stirrup);
            layerNames.Text = GetLayerName(profile, DxfLayerRole.Text, layerNames.Text);
            layerNames.Dimension = GetLayerName(profile, DxfLayerRole.Dimension, layerNames.Dimension);

            return layerNames;
        }

        private static DxfStyleNameSet CreateStyleNames(DxfSettingProfile profile)
        {
            var styleNames = new DxfStyleNameSet();

            if (profile == null || profile.StyleSettings == null)
            {
                return styleNames;
            }

            styleNames.Text = GetStyleName(profile, DxfStyleRole.Text, styleNames.Text);
            styleNames.Block = GetStyleName(profile, DxfStyleRole.Block, styleNames.Block);
            styleNames.Dimension = GetStyleName(profile, DxfStyleRole.Dimension, styleNames.Dimension);

            return styleNames;
        }

        private static string GetLayerName(DxfSettingProfile profile, DxfLayerRole role, string fallback)
        {
            var layer = profile.LayerSettings.FirstOrDefault(x => x.Role == role);
            return layer == null || string.IsNullOrWhiteSpace(layer.LayerName) ? fallback : layer.LayerName;
        }

        private static string GetStyleName(DxfSettingProfile profile, DxfStyleRole role, string fallback)
        {
            var style = profile.StyleSettings.FirstOrDefault(x => x.Role == role);
            return style == null || string.IsNullOrWhiteSpace(style.StyleName) ? fallback : style.StyleName;
        }
    }
}