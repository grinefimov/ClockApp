using System.Windows;
using ClockApp.Models;

namespace ClockApp
{
    public partial class TimerNotificationDisplayPart : ToastNotifications.Core.NotificationDisplayPart
    {
        public TimerNotification TimerNotification { get; }

        public TimerNotificationDisplayPart(TimerNotification timerNotification)
        {
            TimerNotification = timerNotification;

            InitializeComponent();

            Bind(timerNotification);
        }

        private void CloseNotification(object sender, RoutedEventArgs e)
        {
            ((TimerNotificationDisplayPart) this).Notification.Close();
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
            TimerNotification.Timer.ResetCommand.Execute(TimerNotification.Timer);
        }
    }
}