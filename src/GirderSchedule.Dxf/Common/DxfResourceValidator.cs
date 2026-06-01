using System;
using netDxf;

namespace GirderSchedule.Dxf.Common
{
    public sealed class DxfResourceValidator
    {
        public void Ensure(DxfDocument document)
        {
            RequireLayer(document, DxfLayers.FormLine);
            RequireLayer(document, DxfLayers.FormText);
            RequireLayer(document, DxfLayers.Text);
            RequireLayer(document, DxfLayers.RcGir);
            RequireLayer(document, DxfLayers.Rebar);
            RequireLayer(document, DxfLayers.Dim);

            RequireTextStyle(document, DxfStyles.Text);
            RequireDimensionStyle(document, DxfStyles.Dimension);
            RequireLinetype(document, DxfStyles.RcGirderLine);

            RequireBlock(document, DxfBlocks.Form2);
            RequireBlock(document, DxfBlocks.BaseForm);
            RequireBlock(document, DxfBlocks.RebarD19);
            RequireBlock(document, DxfBlocks.DimensionDot);
        }

        private void RequireLayer(DxfDocument document, string name)
        {
            if (!document.Layers.Contains(name))
            {
                throw new InvalidOperationException("템플릿 DXF에 레이어가 없습니다: " + name);
            }
        }

        private void RequireTextStyle(DxfDocument document, string name)
        {
            if (!document.TextStyles.Contains(name))
            {
                throw new InvalidOperationException("템플릿 DXF에 문자 스타일이 없습니다: " + name);
            }
        }

        private void RequireDimensionStyle(DxfDocument document, string name)
        {
            if (!document.DimensionStyles.Contains(name))
            {
                throw new InvalidOperationException("템플릿 DXF에 치수 스타일이 없습니다: " + name);
            }
        }

        private void RequireLinetype(DxfDocument document, string name)
        {
            if (!document.Linetypes.Contains(name))
            {
                throw new InvalidOperationException("템플릿 DXF에 선종이 없습니다: " + name);
            }
        }

        private void RequireBlock(DxfDocument document, string name)
        {
            if (!document.Blocks.Contains(name))
            {
                throw new InvalidOperationException("템플릿 DXF에 블록이 없습니다: " + name);
            }
        }
    }
}