using GirderSchedule.Domain.Models;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class ScheduleSetCopyService
    {
        public ScheduleSet Clone(ScheduleSet source)
        {
            if (source == null)
            {
                return null;
            }

            var clone = new ScheduleSet();
            clone.MemberName = source.MemberName;
            clone.Left = CloneItem(source.Left);
            clone.Center = CloneItem(source.Center);
            clone.Right = CloneItem(source.Right);

            return clone;
        }

        public void CopyTo(ScheduleSet source, ScheduleSet target)
        {
            if (source == null || target == null)
            {
                return;
            }

            CopyItem(source.Left, target.Left);
            CopyItem(source.Center, target.Center);
            CopyItem(source.Right, target.Right);
        }

        private ScheduleItem CloneItem(ScheduleItem source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new ScheduleItem();
            CopyItem(source, target);

            return target;
        }

        private void CopyItem(ScheduleItem source, ScheduleItem target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Name = source.Name;
            target.Position = source.Position;
            target.IsSectionEnabled = source.IsSectionEnabled;

            CopySection(source.Section, target.Section);
            CopyRebarSet(source.TopRebar, target.TopRebar);
            CopyRebarSet(source.BottomRebar, target.BottomRebar);
            CopyStirrup(source.Stirrup, target.Stirrup);
            CopyMemberForce(source.MemberForce, target.MemberForce);
            CopySkinRebar(source.SkinRebar, target.SkinRebar);
        }

        private void CopySection(SectionData source, SectionData target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Width = source.Width;
            target.Height = source.Height;
        }

        private void CopyRebarSet(RebarSet source, RebarSet target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Diameter = source.Diameter;
            CopyRebarLayer(source.FirstLayer, target.FirstLayer);
            CopyRebarLayer(source.SecondLayer, target.SecondLayer);
        }

        private void CopyRebarLayer(RebarLayer source, RebarLayer target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Count = source.Count;
        }

        private void CopyStirrup(StirrupData source, StirrupData target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Legs = source.Legs;
            target.Diameter = source.Diameter;
            target.Spacing = source.Spacing;
            target.ExtraText = source.ExtraText;
        }

        private void CopyMemberForce(MemberForceData source, MemberForceData target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Moment = source.Moment;
            target.Shear = source.Shear;
        }

        private void CopySkinRebar(SkinRebarData source, SkinRebarData target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Diameter = source.Diameter;
            target.Spacing = source.Spacing;
            target.Note = source.Note;
        }
    }
}