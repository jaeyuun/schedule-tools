using System;
using System.IO;
using System.Text.Json;
using GirderSchedule.App.Models;

namespace GirderSchedule.App.Services
{
    public sealed class AppSettingService
    {
        private readonly string _settingFolder;
        private readonly string _settingFilePath;

        public AppSettingService()
        {
            _settingFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GirderSchedule");
            _settingFilePath = Path.Combine(_settingFolder, "settings.json");
        }

        public AppSetting Load()
        {
            Directory.CreateDirectory(_settingFolder);

            if (!File.Exists(_settingFilePath))
            {
                var setting = CreateDefault();
                Save(setting);
                return setting;
            }

            var json = File.ReadAllText(_settingFilePath);
            var loaded = JsonSerializer.Deserialize<AppSetting>(json);

            if (loaded == null || string.IsNullOrWhiteSpace(loaded.DefaultProjectFolder))
            {
                var setting = CreateDefault();
                Save(setting);
                return setting;
            }

            Directory.CreateDirectory(loaded.DefaultProjectFolder);
            return loaded;
        }

        public void Save(AppSetting setting)
        {
            if (setting == null)
            {
                return;
            }

            Directory.CreateDirectory(_settingFolder);

            if (string.IsNullOrWhiteSpace(setting.DefaultProjectFolder))
            {
                setting.DefaultProjectFolder = GetDefaultProjectFolder();
            }

            Directory.CreateDirectory(setting.DefaultProjectFolder);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(setting, options);
            File.WriteAllText(_settingFilePath, json);
        }

        public string GetDefaultProjectFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GirderSchedule", "Projects");
        }

        private AppSetting CreateDefault()
        {
            return new AppSetting
            {
                DefaultProjectFolder = GetDefaultProjectFolder()
            };
        }
    }
}