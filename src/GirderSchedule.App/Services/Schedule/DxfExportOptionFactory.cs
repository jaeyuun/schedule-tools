using GirderSchedule.Domain.Models.Settings;
using GirderSchedule.Dxf.Export;
using GirderSchedule.Dxf.Styles;
using System;
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

            return layerNames;
        }

        private static DxfStyleNameSet CreateStyleNames(DxfSettingProfile profile)
        {
            var styleNames = new DxfStyleNameSet();

            if (profile == null || profile.StyleSetting == null)
            {
                return styleNames;
            }

            styleNames.TextStyleName = profile.StyleSetting.TextStyleName;
            styleNames.DrawingBlockName = profile.StyleSetting.DrawingBlockName;
            styleNames.DimensionStyleName = profile.StyleSetting.DimensionStyleName;

            return styleNames;
        }

        private static string GetLayerName(DxfSettingProfile profile, DxfLayerRole role, string fallback)
        {
            var layer = profile.LayerSettings.FirstOrDefault(x => x.Role == role);
            return layer == null || string.IsNullOrWhiteSpace(layer.LayerName) ? fallback : layer.LayerName;
        }
    }
}