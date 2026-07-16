using GirderSchedule.App.Common;
using GirderSchedule.App.Constants;
using GirderSchedule.App.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GirderSchedule.App.Services.Project
{
    public sealed class RecentProjectService
    {
        public List<RecentProjectInfo> Load()
        {
            Directory.CreateDirectory(FileConstants.RecentProjectPath);

            if (!File.Exists(FileConstants.RecentProjectFilePath))
            {
                return new List<RecentProjectInfo>();
            }

            var items = AppJsonSerializer.Read<List<RecentProjectInfo>>(FileConstants.RecentProjectFilePath);

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
            Directory.CreateDirectory(FileConstants.RecentProjectPath);

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

            AppJsonSerializer.Write(FileConstants.RecentProjectFilePath, ordered);
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