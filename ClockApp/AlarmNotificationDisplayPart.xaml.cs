using System.Windows;
using ClockApp.Models;

namespace ClockApp
{
    public partial class AlarmNotificationDisplayPart : ToastNotifications.Core.NotificationDisplayPart
    {
        public AlarmNotification AlarmNotification { get; }

        public AlarmNotificationDisplayPart(AlarmNotification alarmNotification)
        {
            AlarmNotification = alarmNotification;

            InitializeComponent();

            Bind(alarmNotification);
        }

        private void CloseNotification(object sender, RoutedEventArgs e)
        {
            ((AlarmNotificationDisplayPart) this).Notification.Close();
        }

        private void Snooze(object sender, RoutedEventArgs e)
        {
            AlarmNotification.Alarm.Snooze();
            ((AlarmNotificationDisplayPart) this).Notification.Close();
        }

        private void DismissAlarm(object sender, RoutedEventArgs e)
        {
            AlarmNotification.Alarm.IsAlarming = false;
            ((AlarmNotificationDisplayPart) this).Notification.Close();
        }
    }
}