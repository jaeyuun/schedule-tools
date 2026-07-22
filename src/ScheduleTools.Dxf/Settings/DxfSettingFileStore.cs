using ScheduleTools.Core.Serialization;

namespace ScheduleTools.Dxf.Settings
{
    public sealed class DxfSettingFileStore<TProfile> where TProfile : class
    {
        private readonly DxfStorageOptions _options;
        private readonly Func<TProfile> _defaultFactory;
        private readonly VersionedCompressedJsonStore<TProfile> _fileStore;

        public string DefaultSettingName => _options.DefaultSettingName;

        public DxfSettingFileStore(DxfStorageOptions options, Func<TProfile> defaultFactory)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _defaultFactory = defaultFactory ?? throw new ArgumentNullException(nameof(defaultFactory));
            _fileStore = new VersionedCompressedJsonStore<TProfile>(_options.SettingFileHeader, _defaultFactory);
        }

        public void EnsureDirectory()
        {
            Directory.CreateDirectory(_options.SettingsDirectory);
        }

        public List<string> GetNames()
        {
            EnsureDirectory();

            return Directory.GetFiles(_options.SettingsDirectory, "*" + _options.SettingExtension)
                .Select(Path.GetFileNameWithoutExtension)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name!)
                .OrderBy(name => IsDefaultName(name) ? 0 : 1)
                .ThenBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public bool Exists(string? settingName)
        {
            return File.Exists(GetPath(settingName));
        }

        public string NormalizeName(string? settingName)
        {
            return string.IsNullOrWhiteSpace(settingName) || !Exists(settingName) ? DefaultSettingName : settingName;
        }

        public TProfile Load(string? settingName)
        {
            var path = GetPath(settingName);
            if (!File.Exists(path)) return _defaultFactory();

            try
            {
                return _fileStore.Load(path);
            }
            catch (InvalidDataException)
            {
                return _defaultFactory();
            }
        }

        public void Save(string? settingName, TProfile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);
            EnsureDirectory();
            _fileStore.Save(GetPath(settingName), profile);
        }

        public bool Delete(string? settingName)
        {
            if (IsDefaultName(settingName)) return false;

            var path = GetPath(settingName);
            if (!File.Exists(path)) return false;

            File.Delete(path);
            return true;
        }

        public string GetPath(string? settingName)
        {
            var normalizedName = string.IsNullOrWhiteSpace(settingName) ? DefaultSettingName : settingName;
            return Path.Combine(_options.SettingsDirectory, normalizedName + _options.SettingExtension);
        }

        public bool IsDefaultName(string? settingName)
        {
            return string.Equals(settingName, DefaultSettingName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
