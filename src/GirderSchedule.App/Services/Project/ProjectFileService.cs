using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Domain.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace GirderSchedule.App.Services.Project
{
    public sealed class ProjectFileService
    {
        private const string Header = "GIRDER_SCHEDULE_PROJECT_V1";

        public void Save(string filePath, ScheduleProject project)
        {
            var options = CreateJsonOptions();
            var json = JsonSerializer.Serialize(project, options);
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
                    var project = JsonSerializer.Deserialize<ScheduleProject>(json, CreateJsonOptions());

                    if (project == null)
                    {
                        throw new InvalidOperationException("프로젝트 파일을 읽을 수 없습니다.");
                    }

                    EnsureProject(project);
                    return project;
                }
            }
        }

        private JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.PropertyNameCaseInsensitive = true;
            return options;
        }

        private void EnsureProject(ScheduleProject project)
        {
            if (project.Floors == null)
            {
                project.Floors = new System.Collections.Generic.List<ScheduleFloor>();
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
                    floor.Sets = new System.Collections.Generic.List<ScheduleSet>();
                }
            }
        }
    }
}