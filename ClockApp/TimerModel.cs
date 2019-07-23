using System;
using System.Collections.Generic;
using System.Text;

namespace ClockApp
{
    public class TimerModel
    {
        public int Number { get; set; }
        public DateTime? SelectedTime { get; set; } = DateTime.MaxValue; // To change it in TimePicker_Initialized(object sender, EventArgs e),
        // so SelectedTimeChanged event raised and 0:00 changed to 0:00:00

        public TimerModel(int number)
        {
            Number = number;
        }
    }
}
