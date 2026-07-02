using GirderSchedule.App.Common;
using GirderSchedule.App.Models;
using System;
using System.IO;

namespace GirderSchedule.App.Services.Settings
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

            var loaded = AppJsonSerializer.Read<AppSetting>(_settingFilePath);

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
            AppJsonSerializer.Write(_settingFilePath, setting);
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