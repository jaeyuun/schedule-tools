using GirderSchedule.Domain.Models;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class ScheduleProjectFactory
    {
        private readonly ScheduleSetFactory _setFactory;

        public ScheduleProjectFactory()
            : this(new ScheduleSetFactory())
        {
        }

        public ScheduleProjectFactory(ScheduleSetFactory setFactory)
        {
            _setFactory = setFactory;
        }

        public ScheduleProject Create(string projectName)
        {
            var project = new ScheduleProject();
            project.ProjectName = string.IsNullOrWhiteSpace(projectName) ? "새 프로젝트" : projectName;
            project.ScheduleTitle = "보 일람표";

            var floor = CreateDefaultFloor();
            floor.Sets.Add(_setFactory.Create("1G1", floor.Setting));
            project.Floors.Add(floor);

            return project;
        }

        private ScheduleFloor CreateDefaultFloor()
        {
            var floor = new ScheduleFloor();
            floor.Setting.FloorPrefix = "1";
            floor.Name = "지상 1층";
            floor.Setting.FloorName = floor.Name;
            floor.Setting.MainRebarDiameter = 19;
            floor.Setting.StirrupDiameter = 10;
            floor.Setting.SkinRebarDiameter = 10;

            return floor;
        }
    }
}
