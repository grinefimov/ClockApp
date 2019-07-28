using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToastNotifications;
using ToastNotifications.Core;

namespace ClockApp.Models
{
    public class ClockNotification : NotificationBase, INotifyPropertyChanged
    {
        private NotificationDisplayPart _displayPart;
        private string _message;

        public override ToastNotifications.Core.NotificationDisplayPart DisplayPart =>
            _displayPart ??= new NotificationDisplayPart(this);

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

        public ClockNotification(TimerModel timer, string message) : base(message, new MessageOptions())
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

    public static class NotificationMessageExtension
    {
        public static void ShowNotification(this Notifier notifier, TimerModel timer, string message)
        {
            notifier.Notify<ClockNotification>(() => new ClockNotification(timer, message));
        }
    }
}