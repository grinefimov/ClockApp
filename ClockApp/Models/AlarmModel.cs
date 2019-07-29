using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Serialization;
using ClockApp.Annotations;
using ToastNotifications.Lifetime.Clear;

namespace ClockApp.Models
{
    public class AlarmModel : INotifyPropertyChanged
    {
        private int _number;
        private bool _isOn;
        private bool _isAlarming = false;
        private bool _isSnoozing = false;

        private readonly System.Windows.Threading.DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        private System.Windows.Threading.DispatcherTimer _snoozeTimer;


        private DateTime? _selectedTime = DateTime.MinValue;

        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                OnPropertyChanged(nameof(Number));
            }
        }

        public string Title { get; set; }

        public DateTime? SelectedTime
        {
            get => _selectedTime;
            set
            {
                _selectedTime = value;
                SetDispatcherTimerInterval();
            }
        }

        public bool IsOn
        {
            get => _isOn;
            set
            {
                _isOn = value;
                if (value)
                {
                    SetDispatcherTimerInterval();
                    _dispatcherTimer.Tick -= DispatcherTimerOnTick;
                    _dispatcherTimer.Tick += DispatcherTimerOnTick;
                    _dispatcherTimer.Start();
                }
                else
                {
                    _dispatcherTimer.Stop();
                }

                OnPropertyChanged(nameof(IsOn));
            }
        }

        private void SetDispatcherTimerInterval()
        {
            DateTime time = DateTime.Today.AddHours(SelectedTime.GetValueOrDefault().Hour);
            time = time.AddMinutes(SelectedTime.GetValueOrDefault().Minute);
            if (DateTime.Now > time)
            {
                time = time.AddDays(1.0);
            }

            TimeSpan timeLeft = time - DateTime.Now;
            _dispatcherTimer.Interval = timeLeft;
        }

        private void DispatcherTimerOnTick(object sender, EventArgs e)
        {
            IsAlarming = true;
            if (!IsRepeat) _dispatcherTimer.Stop();
        }

        public bool IsRepeat { get; set; } = true;

        [XmlIgnore]
        public bool IsAlarming
        {
            get => _isAlarming;
            set
            {
                _isAlarming = value;
                if (value)
                {
                    if (!_isSnoozing)
                    {
                        if (IsRepeat)
                        {
                            _dispatcherTimer.Interval = TimeSpan.FromDays(1);
                        }
                        else
                        {
                            IsOn = false;
                        }
                    }
                    else
                    {
                        _isSnoozing = false;
                        _snoozeTimer.Stop();
                    }


                    MainWindow.Player.Stop();
                    MainWindow.PlayAudio();
                    MainWindow.Notifier.ShowAlarmNotification(this, Title);
                }
                else
                {
                    MainWindow.Player.Stop();
                }

                OnPropertyChanged(nameof(IsAlarming));
            }
        }

        public AlarmModel()
        {
        }

        public AlarmModel(int number)
        {
            Number = number;
            Title = "Alarm " + number.ToString();
        }

        public void Snooze()
        {
            IsAlarming = false;
            if (_snoozeTimer == null) _snoozeTimer = new DispatcherTimer();
            _isSnoozing = true;
            _snoozeTimer.Interval = TimeSpan.FromMinutes(MainWindow.Settings.SnoozeLength);
            _snoozeTimer.Tick -= OnSnoozeTimerTick;
            _snoozeTimer.Tick += OnSnoozeTimerTick;
            _snoozeTimer.Start();
        }

        private void OnSnoozeTimerTick(object sender, EventArgs e)
        {
            IsAlarming = true;
            _snoozeTimer.Stop();
        }

        #region RemoveTimerCommand

        public ICommand RemoveAlarmCommand { get; } = new RemoveAlarm();

        private class RemoveAlarm : ICommand
        {
            private AlarmModel _alarm;
            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                if (_alarm == null)
                {
                    _alarm = (AlarmModel) parameter;
                }

                if (_alarm.IsAlarming)
                {
                    _alarm.IsAlarming = false;
                    MainWindow.Notifier.ClearMessages(new ClearByMessage("Timer " + _alarm.Title + ": Time is up!!!"));
                }

                MainWindow.Setup.Alarms.Remove(_alarm);
                int number = 1;
                foreach (var t in MainWindow.Setup.Alarms)
                {
                    t.Number = number;
                    number++;
                }

                if (MainWindow.Setup.Alarms.Count < AlarmsView.MaxAlarmsNumber)
                {
                    AlarmsView.Instance.ShowAddTimerButton();
                }
            }

            public event EventHandler CanExecuteChanged;
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}