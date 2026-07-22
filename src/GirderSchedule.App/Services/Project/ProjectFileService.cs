using GirderSchedule.App.Services.Schedule;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Models.Settings;
using ScheduleTools.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace GirderSchedule.App.Services.Project
{
    public sealed class ProjectFileService
    {
        private const string Header = "GIRDER_SCHEDULE_PROJECT_V1";

        private readonly DxfSettingService _dxfSettingService;
        private readonly VersionedCompressedJsonStore<ScheduleProject> _store =
            new VersionedCompressedJsonStore<ScheduleProject>(Header, () => new ScheduleProject());

        public ProjectFileService(DxfSettingService dxfSettingService)
        {
            _dxfSettingService = dxfSettingService ?? throw new ArgumentNullException(nameof(dxfSettingService));
        }

        public void Save(string filePath, ScheduleProject project)
        {
            _store.Save(filePath, project);
        }

        public ScheduleProject Load(string filePath)
        {
            try
            {
                var project = _store.Load(filePath);
                EnsureProject(project);
                return project;
            }
            catch (InvalidDataException ex)
            {
                throw new InvalidOperationException("GirderSchedule 프로젝트 파일이 아닙니다.", ex);
            }
        }

        private void EnsureProject(ScheduleProject project)
        {
            project.DxfSettingName = _dxfSettingService.NormalizeSettingName(project.DxfSettingName);
            project.Floors ??= new List<ScheduleFloor>();

            for (var i = 0; i < project.Floors.Count; i++)
            {
                var floor = project.Floors[i];
                floor.Setting ??= new FloorSetting();
                floor.Sets ??= new List<ScheduleSet>();
            }
        }
    }
}
