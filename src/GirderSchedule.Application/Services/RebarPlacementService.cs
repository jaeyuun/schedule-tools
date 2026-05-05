using System.Collections.Generic;

namespace GirderSchedule.Application.Services
{
	public static class RebarPlacementService
	{
		public static List<double> GetFirstLayerPositions(double minX, double maxX, int count)
		{
			return GetSymmetricPositions(minX, maxX, count);
		}

		public static List<double> GetSecondLayerPositions(double minX, double maxX, int firstLayerCount, int secondLayerCount)
		{
			var result = new List<double>();

			if (firstLayerCount <= 0 || secondLayerCount <= 0)
			{
				return result;
			}

			if (firstLayerCount == 1)
			{
				result.Add(minX);
				return result;
			}

			var spacing = (maxX - minX) / (firstLayerCount - 1);

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
				if (!drawn[i])
				{
					continue;
				}

				result.Add(minX + spacing * i);
			}

			return result;
		}

		public static List<double> GetSymmetricPositions(double minX, double maxX, int count)
		{
			var result = new List<double>();
			if (count <= 0)
			{
				return result;
			}

			if (count == 1)
			{
				result.Add((minX + maxX) / 2.0);
				return result;
			}

			var spacing = (maxX - minX) / (count - 1);
			for (var i = 0; i < count; i++)
			{
				result.Add(minX + spacing * i);
			}

			return result;
		}

		public static List<double> GetStirrupLegPositions(double minX, double maxX, int count)
		{
			var result = new List<double>();
			if (count <= 0)
			{
				return result;
			}

			if (count == 1)
			{
				result.Add(minX);
				return result;
			}

			if (count == 2)
			{
				result.Add(minX);
				result.Add(maxX);
				return result;
			}

			var spacing = (maxX - minX) / (count - 1);
			for (var i = 0; i < count; i++)
			{
				result.Add(minX + spacing * i);
			}

			return result;
		}

		public static double GetRebarRadius(int diameter)
		{
			return diameter / 2.0;
		}

		public static bool NeedSkinMarkX(double height)
		{
			return height > 900;
		}
	}
}