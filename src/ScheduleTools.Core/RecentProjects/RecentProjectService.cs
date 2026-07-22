using ScheduleTools.Core.Serialization;

namespace ScheduleTools.Core.RecentProjects
{
    public sealed class RecentProjectService
    {
        private readonly string _folderPath;
        private readonly int _maximumCount;
        private readonly string _recentProjectFilePath;

        public RecentProjectService(string folderPath, int maximumCount = 20)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentException("최근 프로젝트 저장 경로가 필요합니다.", nameof(folderPath));

            if (maximumCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumCount));

            _folderPath = folderPath;
            _maximumCount = maximumCount;
            _recentProjectFilePath = Path.Combine(_folderPath, "recent-projects.json");
        }

        public IReadOnlyList<RecentProjectInfo> Load()
        {
            Directory.CreateDirectory(_folderPath);

            var items = JsonFileSerializer.Load(
                _recentProjectFilePath,
                () => new List<RecentProjectInfo>());

            return items
                .Where(IsValid)
                .GroupBy(x => x.FilePath, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.OrderByDescending(y => y.LastOpenedAt).First())
                .OrderByDescending(x => x.LastOpenedAt)
                .Take(_maximumCount)
                .ToList();
        }

        public void Save(IEnumerable<RecentProjectInfo> items)
        {
            Directory.CreateDirectory(_folderPath);

            var normalizedItems = (items ?? Enumerable.Empty<RecentProjectInfo>())
                .Where(IsValid)
                .GroupBy(x => x.FilePath, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.OrderByDescending(y => y.LastOpenedAt).First())
                .OrderByDescending(x => x.LastOpenedAt)
                .Take(_maximumCount)
                .ToList();

            JsonFileSerializer.Save(_recentProjectFilePath, normalizedItems);
        }

        public void AddOrUpdate(string filePath, string projectName)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            var items = Load().ToList();
            var item = items.FirstOrDefault(x => string.Equals(x.FilePath, filePath, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                item = new RecentProjectInfo();
                items.Add(item);
            }

            item.FilePath = filePath;
            item.ProjectName = string.IsNullOrWhiteSpace(projectName)
                ? Path.GetFileNameWithoutExtension(filePath)
                : projectName;
            item.LastOpenedAt = DateTime.Now;

            Save(items);
        }

        public void Remove(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            var items = Load()
                .Where(x => !string.Equals(x.FilePath, filePath, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Save(items);
        }

        public int RemoveMissingFiles()
        {
            var items = Load().ToList();
            var existingItems = items.Where(x => File.Exists(x.FilePath)).ToList();
            var removedCount = items.Count - existingItems.Count;

            if (removedCount > 0)
                Save(existingItems);

            return removedCount;
        }

        private static bool IsValid(RecentProjectInfo item)
        {
            return item != null && !string.IsNullOrWhiteSpace(item.FilePath);
        }
    }
}