using ScheduleTools.Core.Application.Contracts;
using System.Diagnostics;
using System.Reflection;

namespace ScheduleTools.Core.Application.Services
{
    public sealed class ApplicationInfoService : IApplicationInfoService
    {
        private readonly Assembly _assembly;

        public ApplicationInfoService(Assembly assembly)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        public string AssemblyName => _assembly.GetName().Name ?? string.Empty;

        public string ProductName
        {
            get
            {
                var product = _assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;

                return string.IsNullOrWhiteSpace(product)
                    ? AssemblyName
                    : product;
            }
        }

        public string CompanyName => _assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;

        public string Version => _assembly.GetName().Version?.ToString() ?? string.Empty;

        public string FileVersion
        {
            get
            {
                var fileVersion = GetFileVersionInfo()?.FileVersion;

                return string.IsNullOrWhiteSpace(fileVersion)
                    ? Version
                    : fileVersion;
            }
        }

        public string InformationalVersion
        {
            get
            {
                var informationalVersion = _assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion;

                if (string.IsNullOrWhiteSpace(informationalVersion))
                    return FileVersion;

                var metadataIndex = informationalVersion.IndexOf('+');

                return metadataIndex >= 0
                    ? informationalVersion[..metadataIndex]
                    : informationalVersion;
            }
        }

        public string ExecutablePath
        {
            get
            {
                var location = _assembly.Location;

                if (!string.IsNullOrWhiteSpace(location))
                    return location;

                return Environment.ProcessPath ?? string.Empty;
            }
        }

        public string ExecutableDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ExecutablePath))
                    return AppContext.BaseDirectory;

                return Path.GetDirectoryName(ExecutablePath) ?? AppContext.BaseDirectory;
            }
        }

        private FileVersionInfo? GetFileVersionInfo()
        {
            if (string.IsNullOrWhiteSpace(ExecutablePath) || !File.Exists(ExecutablePath))
                return null;

            return FileVersionInfo.GetVersionInfo(ExecutablePath);
        }
    }
}