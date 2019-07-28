using System.Collections.ObjectModel;

namespace ClockApp.Models
{
    public class SetupModel
    {
        public ClockAppTabs SelectedTab { get; set; }
        public ObservableCollection<TimerModel> Timers { get; set; }
        public ObservableCollection<AlarmModel> Alarms { get; set; }

        public enum ClockAppTabs
        {
            Alarms,
            Stopwatch,
            Timer
        }
    }
}