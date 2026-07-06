using System;
using System.IO;

namespace GirderSchedule.App.Constants
{
    public static class FileConstants
    {
        public const string ProjectExtension = ".gps";
        public const string ProjectFilter = "Girder Schedule Project (*.gps)|*.gps";
        public const string DefaultProjectName = "새 프로젝트";
        public const string DefaultProjectFileName = "새 프로젝트.gps";
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

        public const string ResourcesFolderName = "Resources";
        public const string TemplateFolderName = "Templates";
        public const string SettingFolderName = "Settings";
        public const string TemplateFileName = "GirderTemplate.dxf";

        public static string SettingPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingFolderName);
            }
        }

        public static string TemplatePath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TemplateFolderName);
            }
        }

        public static string DefaultTemplateFilePath
        {
            get
            {
#if DEBUG
                return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ResourcesFolderName, TemplateFileName));
#else
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ResourcesFolderName);
#endif
            }
        }
    }
}