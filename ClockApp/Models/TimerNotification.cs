using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToastNotifications;
using ToastNotifications.Core;

namespace ClockApp.Models
{
    public class TimerNotification : NotificationBase, INotifyPropertyChanged
    {
        private TimerNotificationDisplayPart _displayPart;
        private string _message;

        public override ToastNotifications.Core.NotificationDisplayPart DisplayPart =>
            _displayPart ??= new TimerNotificationDisplayPart(this);

        public TimerModel Timer { get; }

        public new string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public TimerNotification(TimerModel timer, string message) : base(message, new MessageOptions())
        {
            Timer = timer;
            Message = message;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AlarmNotification : NotificationBase, INotifyPropertyChanged
    {
        private AlarmNotificationDisplayPart _displayPart;
        private string _message;

        public override ToastNotifications.Core.NotificationDisplayPart DisplayPart =>
            _displayPart ??= new AlarmNotificationDisplayPart(this);

        public AlarmModel Alarm { get; }

        public new string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public AlarmNotification(AlarmModel alarm, string message) : base(message, new MessageOptions())
        {
            Alarm = alarm;
            Message = message;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class NotificationMessageExtensions
    {
        public static void ShowTimerNotification(this Notifier notifier, TimerModel timer, string message)
        {
            notifier.Notify<TimerNotification>(() => new TimerNotification(timer, message));
        }

        public static void ShowAlarmNotification(this Notifier notifier, AlarmModel alarm, string message)
        {
            notifier.Notify<AlarmNotification>(() => new AlarmNotification(alarm, message));
        }
    }
}