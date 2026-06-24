using System.Collections.Generic;

namespace GirderSchedule.Domain.Layouts
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

            var firstLayerXs = GetFirstLayerXs(startX, endX, firstLayerCount);
            var drawn = GetSecondLayerIndexFlags(firstLayerCount, secondLayerCount);

            for (var i = 0; i < firstLayerXs.Count; i++)
            {
                if (drawn[i])
                {
                    result.Add(firstLayerXs[i]);
                }
            }

            return result;
        }

        public bool[] GetSecondLayerIndexFlags(int firstLayerCount, int secondLayerCount)
        {
            var result = new bool[firstLayerCount <= 0 ? 0 : firstLayerCount];

            if (firstLayerCount <= 0 || secondLayerCount <= 0)
            {
                return result;
            }

            secondLayerCount = ClampCount(secondLayerCount, firstLayerCount);

            var leftCount = (secondLayerCount + 1) / 2;
            var rightCount = secondLayerCount / 2;

            for (var i = 0; i < leftCount && i < firstLayerCount; i++)
            {
                result[i] = true;
            }

            for (var i = 0; i < rightCount && i < firstLayerCount; i++)
            {
                result[firstLayerCount - 1 - i] = true;
            }

            return result;
        }

        public int ClampCount(int count, int max)
        {
            if (count <= 0 || max <= 0)
            {
                return 0;
            }

            return count > max ? max : count;
        }
    }
}