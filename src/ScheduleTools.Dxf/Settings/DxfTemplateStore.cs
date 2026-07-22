using ScheduleTools.Dxf.Constants;

namespace ScheduleTools.Dxf.Settings
{
    public sealed class DxfTemplateStore
    {
        private readonly DxfStorageOptions _options;

        public string DefaultTemplateName => _options.DefaultTemplateName;

        public DxfTemplateStore(DxfStorageOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void EnsureDirectory()
        {
            Directory.CreateDirectory(_options.TemplatesDirectory);
        }

        public List<string> GetNames()
        {
            EnsureDirectory();

            var names = Directory.GetFiles(_options.TemplatesDirectory, "*" + DxfFileConstants.Extension)
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrWhiteSpace(name) && !IsDefaultName(name))
                .Select(name => name!)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (File.Exists(_options.DefaultTemplatePath)) names.Insert(0, DefaultTemplateName);
            return names;
        }

        public string Add(string sourceTemplatePath)
        {
            EnsureDirectory();
            if (string.IsNullOrWhiteSpace(sourceTemplatePath) || !File.Exists(sourceTemplatePath)) return string.Empty;

            var fileName = Path.GetFileName(sourceTemplatePath);
            File.Copy(sourceTemplatePath, Path.Combine(_options.TemplatesDirectory, fileName), true);
            return fileName;
        }

        public bool Delete(string? templateName)
        {
            if (IsDefaultName(templateName)) return false;

            var path = GetPath(templateName);
            if (!File.Exists(path)) return false;

            File.Delete(path);
            return true;
        }

        public string GetPath(string? templateName)
        {
            return IsDefaultName(templateName)
                ? _options.DefaultTemplatePath
                : Path.Combine(_options.TemplatesDirectory, templateName!);
        }

        public bool IsDefaultName(string? templateName)
        {
            return string.IsNullOrWhiteSpace(templateName) ||
                   string.Equals(templateName, DefaultTemplateName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
