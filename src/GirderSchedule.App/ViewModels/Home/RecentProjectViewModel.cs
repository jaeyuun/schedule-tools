using System;
using System.IO;
using GirderSchedule.App.Models;
using GirderSchedule.App.ViewModels;

namespace GirderSchedule.App.ViewModels.Home
{
    public sealed class RecentProjectViewModel : ViewModelBase
    {
        private readonly RecentProjectInfo _model;

        public RecentProjectInfo Model
        {
            get { return _model; }
        }

        public string ProjectName
        {
            get { return string.IsNullOrWhiteSpace(_model.ProjectName) ? Path.GetFileNameWithoutExtension(_model.FilePath) : _model.ProjectName; }
        }

        public string FileName
        {
            get { return Path.GetFileName(_model.FilePath); }
        }

        public string FilePath
        {
            get { return _model.FilePath; }
        }

        public DateTime LastOpenedAt
        {
            get { return _model.LastOpenedAt; }
        }

        public string DisplayTime
        {
            get { return _model.LastOpenedAt.ToString("yyyy.MM.dd tt hh:mm"); }
        }

        public RecentProjectViewModel(RecentProjectInfo model)
        {
            _model = model;
        }
    }
}