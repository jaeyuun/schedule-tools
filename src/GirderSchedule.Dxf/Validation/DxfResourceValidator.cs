using GirderSchedule.Dxf.Constants;
using netDxf;

namespace GirderSchedule.Dxf.Validation
{
    public sealed class DxfResourceValidator
    {
        public void Ensure(DxfDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            RequireLayers(document, DxfLayers.FormLine, DxfLayers.FormText, DxfLayers.Text, DxfLayers.RcGir, DxfLayers.Rebar, DxfLayers.Dim, DxfLayers.Defpoint);
            RequireDimensionStyles(document, DxfStyles.Dimension);
            RequireLinetypes(document, DxfStyles.RcGirderLine);
            RequireBlocks(document, DxfBlocks.RebarD19);
        }

        private void RequireLayers(DxfDocument document, params string[] names)
        {
            for (var i = 0; i < names.Length; i++)
            {
                if (!document.Layers.Contains(names[i]))
                {
                    throw new InvalidOperationException("DXF 템플릿에 필요한 레이어가 없습니다. Layer: " + names[i]);
                }
            }
        }

        private void RequireDimensionStyles(DxfDocument document, params string[] names)
        {
            for (var i = 0; i < names.Length; i++)
            {
                if (!document.DimensionStyles.Contains(names[i]))
                {
                    throw new InvalidOperationException("DXF 템플릿에 필요한 치수 스타일이 없습니다. Dimension: " + names[i]);
                }
            }
        }

        private void RequireLinetypes(DxfDocument document, params string[] names)
        {
            for (var i = 0; i < names.Length; i++)
            {
                if (!document.Linetypes.Contains(names[i]))
                {
                    throw new InvalidOperationException("DXF 템플릿에 필요한 선 타입이 없습니다. Linetype: " + names[i]);
                }
            }
        }

        private void RequireBlocks(DxfDocument document, params string[] names)
        {
            for (var i = 0; i < names.Length; i++)
            {
                if (!document.Blocks.Contains(names[i]))
                {
                    throw new InvalidOperationException("DXF 템플릿에 필요한 블록이 없습니다. Block: " + names[i]);
                }
            }
        }
    }
}