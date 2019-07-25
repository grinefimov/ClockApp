using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Core;

namespace ClockApp
{
    public partial class CustomDisplayPart : NotificationDisplayPart
    {
        private System.Windows.Threading.DispatcherTimer DispatcherTimer { get; set; }
        public CustomDisplayPart(CustomNotification customNotification)
        {
            InitializeComponent();
            Bind(customNotification);

            DispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(137) };
            DispatcherTimer.Tick += (sender, args) =>
            {
                TurnOffButton.IsEnabled = false;
                DispatcherTimer.Stop();
            };
            DispatcherTimer.Start();
        }

        private void CloseNotification(object sender, RoutedEventArgs e)
        {
            ((CustomDisplayPart)this).Notification.Close();
        }

        private void TurnOffAlarmMusic(object sender, RoutedEventArgs e)
        {
            MainWindow.Player.Stop();
            Button button = (Button)sender;
            button.IsEnabled = false;
        }
    }

    public class CustomNotification : NotificationBase, INotifyPropertyChanged
    {
        private CustomDisplayPart _displayPart;
        public override NotificationDisplayPart DisplayPart => _displayPart ??= new CustomDisplayPart(this);
        public CustomNotification(string message) : base(message, new MessageOptions())
        {
            Message = message;
        }
        private string _message;
        public new string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public static class CustomMessageExtensions
    {
        public static void ShowCustomMessage(this Notifier notifier, string message)
        {
            notifier.Notify<CustomNotification>(() => new CustomNotification(message));
        }
    }
}
