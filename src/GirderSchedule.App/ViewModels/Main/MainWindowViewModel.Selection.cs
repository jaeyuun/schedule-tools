using GirderSchedule.App.ViewModels.Schedule;

namespace GirderSchedule.App.ViewModels.Main
{
    public sealed partial class MainWindowViewModel
    {
        private void ClearTreeSelection()
        {
            for (var i = 0; i < Floors.Count; i++)
            {
                Floors[i].IsSelected = false;
                Floors[i].IsChildSelected = false;

                for (var j = 0; j < Floors[i].Sets.Count; j++)
                {
                    Floors[i].Sets[j].IsSelected = false;
                }
            }
        }

        private FloorNodeViewModel? FindFloorBySet(ScheduleSetNodeViewModel? set)
        {
            if (set == null)
            {
                return null;
            }

            for (var i = 0; i < Floors.Count; i++)
            {
                if (Floors[i].Sets.Contains(set))
                {
                    return Floors[i];
                }
            }

            return null;
        }
    }
}
