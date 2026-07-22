namespace ScheduleTools.Core.Application.Contracts
{
    public interface IApplicationInfoService
    {
        string AssemblyName { get; }
        string ProductName { get; }
        string CompanyName { get; }
        string Version { get; }
        string FileVersion { get; }
        string InformationalVersion { get; }
        string ExecutablePath { get; }
        string ExecutableDirectory { get; }
    }
}