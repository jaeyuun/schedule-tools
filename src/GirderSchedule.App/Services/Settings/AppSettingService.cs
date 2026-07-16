using GirderSchedule.App.Constants;
using GirderSchedule.App.Models;
using GirderSchedule.App.Utils;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace GirderSchedule.App.Services.Settings
{
    public sealed class AppSettingService
    {
        public const double DefaultFontSize = 12;
        public const double MinFontSize = 9;
        public const double MaxFontSize = 16;

        public AppSetting Load()
        {
            try
            {
                if (!File.Exists(FileConstants.AppSettingFilePath))
                {
                    return CreateDefaultSetting();
                }

                var json = File.ReadAllText(FileConstants.AppSettingFilePath);
                var setting = JsonSerializer.Deserialize<AppSetting>(json) ?? CreateDefaultSetting();

                Normalize(setting);

                return setting;
            }
            catch
            {
                return CreateDefaultSetting();
            }
        }

        public void Save(AppSetting setting)
        {
            if (setting == null)
            {
                return;
            }

            Normalize(setting);
            Directory.CreateDirectory(FileConstants.SettingPath);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(setting, options);
            File.WriteAllText(FileConstants.AppSettingFilePath, json);
        }

        public static void ApplyFontSize(double fontSize)
        {
            if (Application.Current == null)
            {
                return;
            }

            var normalizeFontSize = NormalizeFontSize(fontSize);
            Application.Current.Resources["DefaultFontSize"] = normalizeFontSize;
            Application.Current.Resources["SectionFontSize"] = normalizeFontSize + 2;
            Application.Current.Resources["TitleFontSize"] = normalizeFontSize + 6;
            Application.Current.Resources["IconFontSize"] = normalizeFontSize + 12;
        }

        public static double NormalizeFontSize(double fontSize)
        {
            if (double.IsNaN(fontSize) || double.IsInfinity(fontSize) || fontSize <= 0)
            {
                return DefaultFontSize;
            }

            return Math.Clamp(fontSize, MinFontSize, MaxFontSize);
        }

        public static string GetDefaultProjectFolder()
        {
            return PathUtil.GetDefaultProjectDirectory();
        }

        private static AppSetting CreateDefaultSetting()
        {
            return new AppSetting
            {
                DefaultProjectFolder = GetDefaultProjectFolder(),
                DefaultFontSize = DefaultFontSize
            };
        }

        private static void Normalize(AppSetting setting)
        {
            if (string.IsNullOrWhiteSpace(setting.DefaultProjectFolder))
            {
                setting.DefaultProjectFolder = GetDefaultProjectFolder();
            }

            setting.DefaultFontSize = NormalizeFontSize(setting.DefaultFontSize);
        }
    }
}