using System.Windows;
using System.Windows.Controls;
using ClockApp.Models;

namespace ClockApp
{
    /// <summary>
    /// Interaction logic for AlarmsView.xaml
    /// </summary>
    public partial class AlarmsView : UserControl
    {
        public const int MaxAlarmsNumber = 3;
        public static AlarmsView Instance { get; set; }

        public AlarmsView()
        {
            Instance = this;
            InitializeComponent();
            ItemsControl.ItemsSource = MainWindow.Setup.Alarms;
            if (MainWindow.Setup.Alarms.Count > MaxAlarmsNumber - 1)
            {
                AddTimerButton.Visibility = Visibility.Hidden;
            }
        }

        private void AddTimer(object sender, RoutedEventArgs e)
        {
            MainWindow.Setup.Alarms.Add(new AlarmModel());
            if (MainWindow.Setup.Alarms.Count > MaxAlarmsNumber - 1)
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