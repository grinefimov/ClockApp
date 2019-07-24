using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace ClockApp
{
    /// <summary>
    /// Interaction logic for TimerView.xaml
    /// </summary>
    public partial class TimerView : UserControl
    {
        public static TimerView Instance { get; set; }

        public TimerView()
        {
            Instance = this;
            InitializeComponent();
            ItemsControl.ItemsSource = TimerViewModel.Timers;
        }
        private void TimePicker_Initialized(object sender, EventArgs e)
        {
            MaterialDesignThemes.Wpf.TimePicker timePicker = (MaterialDesignThemes.Wpf.TimePicker)sender;
            timePicker.SelectedTime = DateTime.MinValue;
        }
        private void AddTimer(object sender, RoutedEventArgs e)
        {
            TimerViewModel.Timers.Add(new TimerModel(TimerViewModel.Timers.Count + 1));
            if (TimerViewModel.Timers.Count > 4)
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
