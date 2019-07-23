using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ClockApp
{
    public class TimerModel
    {
        public int Number { get; set; }
        public DateTime? SelectedTime { get; set; } = DateTime.MinValue;

        public TimerModel(int number)
        {
            Number = number;
        }
    }
    public class TimerViewModel
    {
        public static ObservableCollection<TimerModel> Timers { get; } = new ObservableCollection<TimerModel>
        {
            new TimerModel(1),new TimerModel(2),new TimerModel(3),new TimerModel(4),new TimerModel(5)
        };
    }
}