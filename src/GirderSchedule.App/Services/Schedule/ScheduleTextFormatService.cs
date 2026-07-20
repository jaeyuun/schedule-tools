using GirderSchedule.Domain.Models;
using System.Globalization;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class ScheduleTextFormatService
    {
        public string FormatNullableValue(double? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            return FormatDiameter(value.Value);
        }

        public string FormatMomentText(double? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            return $"M = {FormatDiameter(value.Value)}";
        }

        public string FormatShearText(double? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            return $"V = {FormatDiameter(value.Value)}";
        }

        public string FormatFloorSettingDiameter(double diameter)
        {
            if (diameter <= 0.0)
            {
                return "-";
            }

            return $"HD{FormatDiameter(diameter)}";
        }

        public string FormatDiameterInput(double diameter)
        {
            if (diameter <= 0.0)
            {
                return string.Empty;
            }

            return FormatDiameter(diameter);
        }

        public double ParseDiameter(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0.0;
            }

            double result;

            if (double.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            if (double.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }

            return 0.0;
        }

        public string FormatMainRebarText(int count, double diameter)
        {
            if (count <= 0 || diameter <= 0.0)
            {
                return "-";
            }

            return $"{count} - HD {FormatDiameter(diameter)}";
        }

        public string FormatStirrupText(StirrupData stirrup)
        {
            if (stirrup == null || stirrup.Legs <= 0 || stirrup.Diameter <= 0.0 || stirrup.Spacing <= 0.0)
            {
                return "-";
            }

            return  $"{stirrup.Legs} - HD {FormatDiameter(stirrup.Diameter)} @ {stirrup.Spacing}";
        }

        public string FormatSkinRebarText(SkinRebarData skinRebar)
        {
            if (skinRebar == null || skinRebar.Diameter <= 0.0)
            {
                return "-";
            }

            if (skinRebar.Spacing <= 0)
            {
                return $"HD {FormatDiameter(skinRebar.Diameter)}";
            }

            return $"HD {FormatDiameter(skinRebar.Diameter)} @ {skinRebar.Spacing}";
        }

        private string FormatDiameter(double value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }
    }
}