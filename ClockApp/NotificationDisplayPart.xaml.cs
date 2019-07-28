using System;
using System.Windows;
using System.Windows.Threading;
using ClockApp.Models;

namespace ClockApp
{
    public partial class NotificationDisplayPart : ToastNotifications.Core.NotificationDisplayPart
    {
        private System.Windows.Threading.DispatcherTimer DispatcherTimer { get; set; }
        public ClockNotification ClockNotification { get; }

        public NotificationDisplayPart(ClockNotification clockNotification)
        {
            ClockNotification = clockNotification;

            InitializeComponent();

            Bind(clockNotification);

            DispatcherTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(137)};
            DispatcherTimer.Tick += (sender, args) =>
            {
                MuteButton.IsEnabled = false;
                DispatcherTimer.Stop();
            };
            DispatcherTimer.Start();
        }

        private void CloseNotification(object sender, RoutedEventArgs e)
        {
            ((NotificationDisplayPart) this).Notification.Close();
        }

        private void MuteAlarmMusic(object sender, RoutedEventArgs e)
        {
            MainWindow.Player.Stop();
            foreach (TimerModel timer in MainWindow.Setup.Timers)
            {
                timer.IsMuteButtonEnabled = false;
            }
        }

        private void ResetTimer(object sender, RoutedEventArgs e)
        {
            ClockNotification.Timer.ResetCommand.Execute(ClockNotification.Timer);
        }
    }
}