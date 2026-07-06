using GirderSchedule.Domain.Models.Settings;
using GirderSchedule.Dxf.Geometry;
using GirderSchedule.Dxf.Styles;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;

namespace GirderSchedule.Dxf.Drawers.Common
{
    public sealed class DxfEntityDrawer
    {
        private readonly DxfDocument _document;
        private readonly DxfEntityStyler _styler;

        public DxfEntityDrawer(DxfDocument document, DxfEntityStyler styler)
        {
            _document = document;
            _styler = styler;
        }

        public Line AddLine(DxfLayerRole role, double x1, double y1, double x2, double y2)
        {
            var line = new Line(DxfPointConverter.ToVector3(x1, y1), DxfPointConverter.ToVector3(x2, y2));
            AddEntity(line, role);
            return line;
        }

        public Circle AddCircle(DxfLayerRole role, double centerX, double centerY, double radius)
        {
            var circle = new Circle(DxfPointConverter.ToVector3(centerX, centerY), radius);
            AddEntity(circle, role);
            return circle;
        }

        public Polyline2D AddPolyline(DxfLayerRole role, bool isClosed, params double[] values)
        {
            return AddPolylineCore(role, isClosed, 0.0, values);
        }

        private Polyline2D AddPolylineCore(DxfLayerRole role, bool isClosed, double width, params double[] values)
        {
            if (values == null || values.Length == 0 || values.Length % 2 != 0)
            {
                return null;
            }

            var polyline = new Polyline2D();

            for (var i = 0; i < values.Length; i += 2)
            {
                var vertex = new Polyline2DVertex(DxfPointConverter.ToVector2(values[i], values[i + 1]));
                vertex.StartWidth = width;
                vertex.EndWidth = width;
                polyline.Vertexes.Add(vertex);
            }

            polyline.IsClosed = isClosed;
            AddEntity(polyline, role);
            return polyline;
        }

        public Insert AddUnstyledInsert(Block block, double x, double y, double scale, double rotation)
        {
            var insert = CreateInsert(block, x, y, scale, rotation);

            if (insert != null)
            {
                _document.Entities.Add(insert);
            }

            return insert;
        }

        private Insert CreateInsert(Block block, double x, double y, double scale, double rotation)
        {
            if (block == null)
            {
                return null;
            }

            var insert = new Insert(block, DxfPointConverter.ToVector3(x, y));
            insert.Scale = new Vector3(scale, scale, scale);
            insert.Rotation = rotation;
            return insert;
        }

        private void AddEntity(EntityObject entity, DxfLayerRole role)
        {
            if (entity == null)
            {
                return;
            }

            _styler.ApplyLayer(entity, role);
            _document.Entities.Add(entity);
        }
    }
}
