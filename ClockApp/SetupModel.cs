using System.Collections.ObjectModel;

namespace ClockApp
{
    public class SetupModel
    {
        public ObservableCollection<TimerModel> Timers { get; set; }
    }
}
