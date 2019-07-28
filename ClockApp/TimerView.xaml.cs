using System;
using System.Windows;
using System.Windows.Controls;

namespace ClockApp
{
    /// <summary>
    /// Interaction logic for TimerView.xaml
    /// </summary>
    public partial class TimerView : UserControl
    {
        public const int MaxTimersNumber = 5;
        public static TimerView Instance { get; set; }
        public TimerView()
        {
            Instance = this;
            InitializeComponent();
            ItemsControl.ItemsSource = MainWindow.Setup.Timers;
        }
        private void TimePicker_Initialized(object sender, EventArgs e)
        {
            MaterialDesignThemes.Wpf.TimePicker timePicker = (MaterialDesignThemes.Wpf.TimePicker)sender;
            if (timePicker.SelectedTime == null)
            {
                timePicker.SelectedTime = DateTime.MinValue;
            }
            else if(timePicker.SelectedTime == DateTime.MinValue)
            {
                timePicker.SelectedTime = DateTime.MaxValue;
                timePicker.SelectedTime = DateTime.MinValue;
            }
            else
            {
                DateTime? temp = timePicker.SelectedTime;
                timePicker.SelectedTime = DateTime.MinValue;
                timePicker.SelectedTime = temp;
            }
        }
        private void AddTimer(object sender, RoutedEventArgs e)
        {
            MainWindow.Setup.Timers.Add(new TimerModel(MainWindow.Setup.Timers.Count + 1));
            if (MainWindow.Setup.Timers.Count > MaxTimersNumber - 1)
            {
                AddTimerButton.Visibility = Visibility.Hidden;
            }
        }

        public void ShowAddTimerButton()
        {
            AddTimerButton.Visibility = Visibility.Visible;
        }
    }
}
