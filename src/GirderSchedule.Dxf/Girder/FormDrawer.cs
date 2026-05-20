using GirderSchedule.Dxf.Common;
using GirderSchedule.Domain.Girder.Models;
using netDxf;
using netDxf.Entities;

namespace GirderSchedule.Dxf.Girder
{
    public sealed class FormDrawer
    {
        private readonly DxfDocument _document;
        private readonly DxfPointConverter _pointConverter;

        public FormDrawer(DxfDocument document, DxfPointConverter pointConverter)
        {
            _document = document;
            _pointConverter = pointConverter;
        }

        public void Draw(ScheduleSheet sheet)
        {
            DrawTitleForm();

            for (var i = 0; i < sheet.Rows.Count; i++)
            {
                DrawBaseForm(i);
            }
        }

        private void DrawTitleForm()
        {
            InsertBlock(DxfBlocks.TitleForm, 2014.8381, 0.0, 65.0, 65.0, 0.0);
        }

        private void DrawBaseForm(int rowIndex)
        {
            var x = 3009.8381;
            var y = 12152.5003 - 5000.0 * rowIndex;

            InsertBlock(DxfBlocks.BaseForm, x, y, 1.0, 1.0, 0.0);
        }

        private void InsertBlock(string blockName, double x, double y, double scaleX, double scaleY, double rotation)
        {
            var block = _document.Blocks[blockName];
            var insert = new Insert(block, _pointConverter.ToVector3(x, y));
            insert.Scale = new Vector3(scaleX, scaleY, 1.0);
            insert.Rotation = rotation;
            insert.Layer = _document.Layers["0"];

            _document.Entities.Add(insert);
        }
    }
}