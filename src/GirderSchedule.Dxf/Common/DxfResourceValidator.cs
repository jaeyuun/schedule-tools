using System;
using netDxf;

namespace GirderSchedule.Dxf.Common
{
    public sealed class DxfResourceValidator
    {
        public void Validate(DxfDocument document)
        {
            RequireLayer(document, DxfLayers.FormLine);
            RequireLayer(document, DxfLayers.FormText);
            RequireLayer(document, DxfLayers.Text);
            RequireLayer(document, DxfLayers.Rebar);
            RequireLayer(document, DxfLayers.RcGirder);
            RequireLayer(document, DxfLayers.Dimension);

            RequireBlock(document, DxfBlocks.BaseForm);
            RequireBlock(document, DxfBlocks.ExtendForm);
            RequireBlock(document, DxfBlocks.RebarDot);
            RequireBlock(document, DxfBlocks.TitleForm);
        }

        private void RequireLayer(DxfDocument document, string name)
        {
            if (!document.Layers.Contains(name))
            {
                throw new InvalidOperationException("DXF Layer가 없습니다: " + name);
            }
        }

        private void RequireBlock(DxfDocument document, string name)
        {
            if (!document.Blocks.Contains(name))
            {
                throw new InvalidOperationException("DXF Block이 없습니다: " + name);
            }
        }
    }
}