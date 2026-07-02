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

        public const string DxfExtension = ".dxf";
        public const string DxfFilter = "DXF Files (*.dxf)|*.dxf";
        public const string DxfFileTypeName = "DXF";
        public const string DxfSaveDialogTitle = "DXF 저장";

        public const string ExcelExtension = ".xlsx";
        public const string ExcelFilter = "Excel Files (*.xlsx)|*.xlsx";
        public const string ExcelFileTypeName = "Excel";
        public const string ExcelSaveDialogTitle = "Excel 저장";
        public const string ExcelSheetName = "보 일람표";

        public const string DefaultProjectNameForExport = "프로젝트";
        public const string DefaultScheduleTitleForExport = "보 일람표";

        public const string ResourcesFolderName = "Resources";
        public const string GirderTemplateFileName = "GirderTemplate.dxf";

        public static string GirderTemplatePath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ResourcesFolderName, GirderTemplateFileName);
            }
        }
    }
}