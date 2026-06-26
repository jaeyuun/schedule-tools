using GirderSchedule.Domain.Models;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class SectionItemTextService
    {
        public string FormatNullableValue(double? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            return value.Value.ToString("0");
        }

        public string FormatMomentText(double? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            return "M = " + value.Value.ToString("0");
        }

        public string FormatShearText(double? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            return "V = " + value.Value.ToString("0");
        }

        public string FormatLayerText(int count, double diameter)
        {
            if (count <= 0 || diameter <= 0)
            {
                return "-";
            }

            return count + " - HD " + diameter;
        }

        public string FormatStirrupText(StirrupData stirrup)
        {
            if (stirrup == null || stirrup.Legs <= 0 || stirrup.Diameter <= 0 || stirrup.Spacing <= 0)
            {
                return "-";
            }

            return stirrup.Legs + " - HD " + stirrup.Diameter + " @ " + stirrup.Spacing;
        }

        public string FormatSkinRebarText(SkinRebarData skinRebar)
        {
            if (skinRebar == null || skinRebar.Diameter <= 0 || skinRebar.Spacing <= 0)
            {
                return "-";
            }

            return "HD " + skinRebar.Diameter + " @ " + skinRebar.Spacing;
        }
    }
}