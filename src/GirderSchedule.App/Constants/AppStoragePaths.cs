using ScheduleTools.Core.IO;
using ScheduleTools.Dxf.Constants;
using ScheduleTools.Dxf.Settings;
using System;
using System.IO;

namespace GirderSchedule.App.Constants
{
    public static class AppStoragePaths
    {
        public const string CompanyFolderName = "ScheduleTools";
        public const string ApplicationFolderName = "GirderSchedule";
        public const string ResourcesFolderName = "Resources";
        public const string AppSettingFileName = "AppSetting.json";
        public const string DxfSettingExtension = ".gds";
        public const string DxfSettingFileHeader = "GIRDER_SCHEDULE_DXF_SETTING_V1";
        public const string DefaultTemplateFileName = "GirderTemplate.dxf";

        private static readonly ApplicationStoragePaths Paths = new ApplicationStoragePaths(CompanyFolderName, ApplicationFolderName);

        public static string ApplicationDataDirectory => Paths.ApplicationDataDirectory;
        public static string DocumentsDirectory => Paths.DocumentsDirectory;
        public static string SettingsDirectory => Paths.SettingsDirectory;
        public static string TemplatesDirectory => Paths.TemplatesDirectory;
        public static string AppSettingFilePath => Path.Combine(SettingsDirectory, AppSettingFileName);
        public static string DefaultTemplateFilePath => Path.Combine(AppContext.BaseDirectory, ResourcesFolderName, DefaultTemplateFileName);

        public static DxfStorageOptions CreateDxfStorageOptions()
        {
            return new DxfStorageOptions(
                SettingsDirectory,
                TemplatesDirectory,
                DxfSettingExtension,
                DxfSettingFileHeader,
                DxfFileConstants.DefaultSettingName,
                DefaultTemplateFileName,
                DefaultTemplateFilePath);
        }
    }
}
