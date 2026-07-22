using System;
using System.IO;

namespace GirderSchedule.App.Constants
{
    public static class FileConstants
    {
        public const string ProjectExtension = ".gsp";
        public const string ProjectFilter = "Girder Schedule Project (*.gsp)|*.gsp";
        public const string DefaultProjectName = "새 프로젝트";
        public const string DefaultProjectFileName = "새 프로젝트.gsp";
        public const string ProjectDialogTitle = "프로젝트 열기";

        public const string DxfSettingExtension = ".gds";
        public const string DxfSettingName = "Default";

        public const string DxfExtension = ".dxf";
        public const string DxfFilter = "DXF Files (*.dxf)|*.dxf";
        public const string DxfSaveDialogTitle = "DXF 저장";
        public const string DxfOpenDialogTitle = "DXF 템플릿 선택";

        public const string ExcelExtension = ".xlsx";
        public const string ExcelFilter = "Excel Files (*.xlsx)|*.xlsx";
        public const string ExcelSaveDialogTitle = "Excel 저장";
        public const string ExcelSheetName = "보 일람표";

        public const string DefaultProjectNameForExport = "프로젝트";
        public const string DefaultScheduleTitleForExport = "보 일람표";

        public const string CompanyFolderName = "ScheduleTools";
        public const string ApplicationFolderName = "GirderSchedule";
        public const string ProjectFolderName = "Projects";

        public const string ResourcesFolderName = "Resources";
        public const string SettingFolderName = "Settings";
        public const string TemplateFolderName = "Templates";
        public const string RecentProjectFolderName = "RecentProjects";

        public const string AppSettingFileName = "AppSetting.json";
        public const string RecentProjectFileName = "recent-projects.json";
        public const string TemplateFileName = "GirderTemplate.dxf";

        public static string AppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CompanyFolderName, ApplicationFolderName);

        public static string SettingPath => Path.Combine(AppDataPath, SettingFolderName);
        public static string TemplatePath => Path.Combine(AppDataPath, TemplateFolderName);
        public static string RecentProjectPath => Path.Combine(AppDataPath, RecentProjectFolderName);

        public static string AppSettingFilePath => Path.Combine(SettingPath, AppSettingFileName);

        public static string DefaultTemplateFilePath
        {
            get
            {
#if DEBUG
                return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ResourcesFolderName, TemplateFileName));
#else
                return Path.Combine(InstalledResourcePath, TemplateFileName);
#endif
            }
        }
    }
}