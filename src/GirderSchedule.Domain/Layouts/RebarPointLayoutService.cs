using System.Collections.Generic;

namespace GirderSchedule.Domain.Girder.Layout
{
    public sealed class RebarPointLayoutService
    {
        public List<double> GetFirstLayerXs(double startX, double endX, int count)
        {
            var result = new List<double>();

            if (count <= 0)
            {
                return result;
            }

            if (count == 1)
            {
                result.Add(startX);
                return result;
            }

            var spacing = (endX - startX) / (count - 1);

            for (var i = 0; i < count; i++)
            {
                result.Add(startX + spacing * i);
            }

            return result;
        }

        public List<double> GetSecondLayerXs(double startX, double endX, int firstLayerCount, int secondLayerCount)
        {
            var result = new List<double>();

            if (firstLayerCount <= 0 || secondLayerCount <= 0)
            {
                return result;
            }

            if (firstLayerCount == 1)
            {
                result.Add(startX);
                return result;
            }

            var spacing = (endX - startX) / (firstLayerCount - 1);
            var leftCount = (secondLayerCount + 1) / 2;
            var rightCount = secondLayerCount / 2;
            var drawn = new bool[firstLayerCount];

            for (var i = 0; i < leftCount && i < firstLayerCount; i++)
            {
                drawn[i] = true;
            }

            for (var i = 0; i < rightCount && i < firstLayerCount; i++)
            {
                drawn[firstLayerCount - 1 - i] = true;
            }

            for (var i = 0; i < firstLayerCount; i++)
            {
                if (drawn[i])
                {
                    result.Add(startX + spacing * i);
                }
            }

            return result;
        }
    }
}