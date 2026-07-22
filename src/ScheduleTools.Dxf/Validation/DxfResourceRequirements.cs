using netDxf;

namespace ScheduleTools.Dxf.Validation
{
    public sealed class DxfResourceRequirements
    {
        public IReadOnlyCollection<string> Layers { get; init; } = Array.Empty<string>();
        public IReadOnlyCollection<string> DimensionStyles { get; init; } = Array.Empty<string>();
        public IReadOnlyCollection<string> Linetypes { get; init; } = Array.Empty<string>();
        public IReadOnlyCollection<string> Blocks { get; init; } = Array.Empty<string>();

        public void Ensure(DxfDocument document)
        {
            ArgumentNullException.ThrowIfNull(document);

            EnsureAll(Layers, document.Layers.Contains, "Layer");
            EnsureAll(DimensionStyles, document.DimensionStyles.Contains, "Dimension");
            EnsureAll(Linetypes, document.Linetypes.Contains, "Linetype");
            EnsureAll(Blocks, document.Blocks.Contains, "Block");
        }

        private static void EnsureAll(IEnumerable<string> names, Func<string, bool> contains, string resourceType)
        {
            foreach (var name in names)
            {
                if (!contains(name))
                {
                    throw new InvalidOperationException($"DXF 템플릿에 필요한 리소스가 없습니다. {resourceType}: {name}");
                }
            }
        }
    }
}
