using GirderSchedule.App.Common;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Models.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace GirderSchedule.App.Services.Project
{
    public sealed class ProjectFileService
    {
        private const string Header = "GIRDER_SCHEDULE_PROJECT_V1";
        private readonly DxfSettingService _dxfSettingService = new DxfSettingService();

        public void Save(string filePath, ScheduleProject project)
        {
            var json = AppJsonSerializer.Serialize(project);
            var jsonBytes = Encoding.UTF8.GetBytes(json);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fileStream, Encoding.UTF8))
            {
                writer.Write(Header);

                using (var gzipStream = new GZipStream(fileStream, CompressionLevel.Optimal, true))
                {
                    gzipStream.Write(jsonBytes, 0, jsonBytes.Length);
                }
            }
        }

        public ScheduleProject Load(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fileStream, Encoding.UTF8, true))
            {
                var header = reader.ReadString();

                if (header != Header)
                {
                    throw new InvalidOperationException("GirderSchedule 프로젝트 파일이 아닙니다.");
                }

                using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                using (var memoryStream = new MemoryStream())
                {
                    gzipStream.CopyTo(memoryStream);

                    var json = Encoding.UTF8.GetString(memoryStream.ToArray());
                    var project = AppJsonSerializer.Deserialize<ScheduleProject>(json);

                    if (project == null)
                    {
                        throw new InvalidOperationException("프로젝트 파일을 읽을 수 없습니다.");
                    }

                    EnsureProject(project);
                    return project;
                }
            }
        }

        private void EnsureProject(ScheduleProject project)
        {
            project.DxfSettingName = _dxfSettingService.NormalizeSettingName(project.DxfSettingName);

            if (project.Floors == null)
            {
                project.Floors = new List<ScheduleFloor>();
            }

            for (var i = 0; i < project.Floors.Count; i++)
            {
                var floor = project.Floors[i];

                if (floor.Setting == null)
                {
                    floor.Setting = new FloorSetting();
                }

                if (floor.Sets == null)
                {
                    floor.Sets = new List<ScheduleSet>();
                }
            }
        }
    }
}