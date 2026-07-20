using netDxf;
using netDxf.Entities;

namespace GirderSchedule.Dxf.Styles.Overrides
{
    public sealed class DxfBlockInsertOverride
    {
        private readonly string _blockName;
        private readonly Vector3 _scale;
        private readonly double _rotation;

        public DxfBlockInsertOverride(Insert source)
        {
            _blockName = source == null || source.Block == null ? string.Empty : source.Block.Name;
            _scale = source == null ? new Vector3(1.0, 1.0, 1.0) : new Vector3(source.Scale.X, source.Scale.Y, source.Scale.Z);
            _rotation = source == null ? 0.0 : source.Rotation;
        }

        public bool IsMatch(Insert target)
        {
            if (target == null || target.Block == null)
            {
                return false;
            }

            return string.Equals(_blockName, target.Block.Name, StringComparison.OrdinalIgnoreCase);
        }

        public void Apply(Insert target)
        {
            if (target == null)
            {
                return;
            }

            target.Scale = new Vector3(_scale.X, _scale.Y, _scale.Z);
            target.Rotation = _rotation;
        }

        public static void ApplyDefault(Insert target, double scale, double rotation)
        {
            if (target == null)
            {
                return;
            }

            target.Scale = new Vector3(scale, scale, scale);
            target.Rotation = rotation;
        }
    }
}