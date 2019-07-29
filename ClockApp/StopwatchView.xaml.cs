using System.Windows;
using System.Windows.Controls;

namespace ClockApp
{
    public partial class StopwatchView : UserControl
    {
        public StopwatchView()
        {
            this.DataContext = MainWindow.Setup.Stopwatch;
            InitializeComponent();
        }

        private void StartPauseResume(object sender, RoutedEventArgs e)
        {
            MainWindow.Setup.Stopwatch.StartPauseResume();
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            MainWindow.Setup.Stopwatch.Reset();
        }
    }
}