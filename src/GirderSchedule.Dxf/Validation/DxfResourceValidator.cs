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
            RequireTextStyles(document, DxfStyles.Text);
            RequireDimensionStyles(document, DxfStyles.Dimension);
            RequireLinetypes(document, DxfStyles.RcGirderLine);
            RequireBlocks(document, DxfBlocks.TableForm, DxfBlocks.BaseForm, DxfBlocks.RebarD19, DxfBlocks.DimensionDot, DxfBlocks.TitleBlock);
        }

        private void RequireLayers(DxfDocument document, params string[] names)
        {
            for (var i = 0; i < names.Length; i++)
            {
                if (!document.Layers.Contains(names[i]))
                {
                    throw new InvalidOperationException("템플릿 DXF에 레이어가 없습니다: " + names[i]);
                }
            }
        }

        private void RequireTextStyles(DxfDocument document, params string[] names)
        {
            for (var i = 0; i < names.Length; i++)
            {
                if (!document.TextStyles.Contains(names[i]))
                {
                    throw new InvalidOperationException("템플릿 DXF에 문자 스타일이 없습니다: " + names[i]);
                }
            }
        }

        private void RequireDimensionStyles(DxfDocument document, params string[] names)
        {
            for (var i = 0; i < names.Length; i++)
            {
                if (!document.DimensionStyles.Contains(names[i]))
                {
                    throw new InvalidOperationException("템플릿 DXF에 치수 스타일이 없습니다: " + names[i]);
                }
            }
        }

        private void RequireLinetypes(DxfDocument document, params string[] names)
        {
            for (var i = 0; i < names.Length; i++)
            {
                if (!document.Linetypes.Contains(names[i]))
                {
                    throw new InvalidOperationException("템플릿 DXF에 선종이 없습니다: " + names[i]);
                }
            }
        }

        private void RequireBlocks(DxfDocument document, params string[] names)
        {
            for (var i = 0; i < names.Length; i++)
            {
                if (!document.Blocks.Contains(names[i]))
                {
                    throw new InvalidOperationException("템플릿 DXF에 블록이 없습니다: " + names[i]);
                }
            }
        }
    }
}
