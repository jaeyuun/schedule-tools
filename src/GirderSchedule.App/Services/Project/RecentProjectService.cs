using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using GirderSchedule.App.Models;

namespace GirderSchedule.App.Services.Project
{
    public sealed class RecentProjectService
    {
        private readonly string _folderPath;
        private readonly string _filePath;

        public RecentProjectService()
        {
            _folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GirderSchedule");
            _filePath = Path.Combine(_folderPath, "recent-projects.json");
        }

        public List<RecentProjectInfo> Load()
        {
            Directory.CreateDirectory(_folderPath);

            if (!File.Exists(_filePath))
            {
                return new List<RecentProjectInfo>();
            }

            var json = File.ReadAllText(_filePath);
            var items = JsonSerializer.Deserialize<List<RecentProjectInfo>>(json);

            if (items == null)
            {
                return new List<RecentProjectInfo>();
            }

            return items
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.FilePath))
                .OrderByDescending(x => x.LastOpenedAt)
                .ToList();
        }

        public void Save(List<RecentProjectInfo> items)
        {
            Directory.CreateDirectory(_folderPath);

            if (items == null)
            {
                items = new List<RecentProjectInfo>();
            }

            var ordered = items
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.FilePath))
                .GroupBy(x => x.FilePath, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.OrderByDescending(y => y.LastOpenedAt).First())
                .OrderByDescending(x => x.LastOpenedAt)
                .Take(20)
                .ToList();

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(ordered, options);
            File.WriteAllText(_filePath, json);
        }

        public void AddOrUpdate(string filePath, string projectName)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            var items = Load();
            var item = items.FirstOrDefault(x => string.Equals(x.FilePath, filePath, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                item = new RecentProjectInfo();
                items.Add(item);
            }

            item.FilePath = filePath;
            item.ProjectName = string.IsNullOrWhiteSpace(projectName) ? Path.GetFileNameWithoutExtension(filePath) : projectName;
            item.LastOpenedAt = DateTime.Now;

            Save(items);
        }

        public void Remove(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            var items = Load();
            items.RemoveAll(x => string.Equals(x.FilePath, filePath, StringComparison.OrdinalIgnoreCase));
            Save(items);
        }

        public void RemoveMissingFiles()
        {
            var items = Load();
            items.RemoveAll(x => !File.Exists(x.FilePath));
            Save(items);
        }
    }
}