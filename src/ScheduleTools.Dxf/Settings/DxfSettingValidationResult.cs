namespace ScheduleTools.Dxf.Settings
{
    public sealed class DxfSettingValidationResult
    {
        public string TemplateError { get; set; } = string.Empty;
        public List<string> InvalidStyles { get; } = new List<string>();
        public List<string> InvalidLayers { get; } = new List<string>();
        public bool IsValid => string.IsNullOrWhiteSpace(TemplateError) && InvalidStyles.Count == 0 && InvalidLayers.Count == 0;
    }
}
