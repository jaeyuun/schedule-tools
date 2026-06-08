using GirderSchedule.Dxf.Common;
using GirderSchedule.Dxf.Common.Overrides;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;

namespace GirderSchedule.Dxf.Drawers
{
    public sealed class FormDrawer
    {
        private readonly DxfDocument _document;
        private readonly DxfPointConverter _pointConverter;
        private readonly DxfOverrideTemplateSet _overrides;

        public FormDrawer(DxfDocument document, DxfPointConverter pointConverter, TextDrawer textDrawer, DxfOverrideTemplateSet overrides)
        {
            _document = document;
            _pointConverter = pointConverter;
            _overrides = overrides;
        }

        public void Draw()
        {
            AddFormBlock(DxfBlocks.Form2, 0.0, 0.0);
        }

        private void AddFormBlock(string blockName, double x, double y)
        {
            if (!_document.Blocks.Contains(blockName))
            {
                return;
            }

            var insert = new Insert(_document.Blocks[blockName], _pointConverter.ToVector3(x, y));
            insert.Scale = new Vector3(1.0, 1.0, 1.0);
            insert.Rotation = 0.0;

            insert.Layer = _document.Layers["0"];
            insert.Color = AciColor.ByLayer;
            insert.Linetype = Linetype.ByLayer;
            insert.Lineweight = Lineweight.ByLayer;

            ApplyOverrides(insert);

            _document.Entities.Add(insert);
        }

        private void ApplyOverrides(EntityObject entity)
        {
            if (_overrides != null)
            {
                _overrides.Apply(entity);
            }
        }
    }
}