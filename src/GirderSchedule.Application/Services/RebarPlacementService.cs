using System.Collections.Generic;

namespace GirderSchedule.Application.Services
{
	public static class RebarPlacementService
	{
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

		public static bool NeedSkinMarkX(int height)
		{
			return height > 900;
		}
	}
}