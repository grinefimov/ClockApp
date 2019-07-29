using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml.Serialization;
using ClockApp.Annotations;
using ToastNotifications.Lifetime.Clear;

namespace ClockApp.Models
{
    public class TimerModel : INotifyPropertyChanged
    {
        private TimerStatus _status = TimerStatus.Stopped;
        private bool _isAlarming = false;
        private bool _isStartPauseResumeButtonEnabled = false;
        private bool _isResetButtonEnabled = false;
        private bool _canSaveTime = true;
        private bool _isTimePickerEnabled = true;
        private int _number;
        private DateTime? _selectedTime;
        private bool _isBackward = false;
        private bool _isMuteButtonEnabled = true;

        private readonly System.Windows.Threading.DispatcherTimer _dispatcherTimer =
            new System.Windows.Threading.DispatcherTimer() {Interval = new TimeSpan(0, 0, 1)};

        private TimerStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                switch (value)
                {
                    case TimerStatus.Stopped:
                        if (!_isBackward)
                        {
                            IsTimePickerEnabled = true;
                        }

                        StartPauseResumeButtonText = "Start";
                        break;
                    case TimerStatus.Started:
                        StartPauseResumeButtonText = "Pause";
                        IsTimePickerEnabled = false;
                        break;
                    case TimerStatus.Paused:
                        StartPauseResumeButtonText = "Resume";
                        break;
                }

                OnPropertyChanged(nameof(StartPauseResumeButtonText));
            }
        }

        [XmlIgnore]
        public bool IsAlarming
        {
            get => _isAlarming;
            set
            {
                _isAlarming = value;
                if (value)
                {
                    MainWindow.Player.Stop();
                    MainWindow.PlayAudio();
                    MainWindow.Notifier.ShowTimerNotification(this, "Timer " + Number + ": Time is up!!!");
                }

                if (!value)
                {
                    MainWindow.Player.Stop();
                }

                OnPropertyChanged(nameof(IsAlarming));
            }
        }

        [XmlIgnore] public bool StopTimer { get; set; } = false;
        [XmlIgnore] public string StartPauseResumeButtonText { get; set; } = "Start";

        [XmlIgnore]
        public bool IsStartPauseResumeButtonEnabled
        {
            get => _isStartPauseResumeButtonEnabled;
            set
            {
                _isStartPauseResumeButtonEnabled = value;
                OnPropertyChanged(nameof(IsStartPauseResumeButtonEnabled));
            }
        }

        [XmlIgnore]
        public bool IsResetButtonEnabled
        {
            get => _isResetButtonEnabled;
            set
            {
                _isResetButtonEnabled = value;
                OnPropertyChanged(nameof(IsResetButtonEnabled));
            }
        }

        [XmlIgnore]
        public bool IsTimePickerEnabled
        {
            get => _isTimePickerEnabled;
            set
            {
                _isTimePickerEnabled = value;
                OnPropertyChanged(nameof(IsTimePickerEnabled));
            }
        }

        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                OnPropertyChanged(nameof(Number));
            }
        }

        public DateTime Time { get; set; }

        [XmlIgnore]
        public DateTime? SelectedTime
        {
            get => _selectedTime;
            set
            {
                _selectedTime = value;
                if (Status == TimerStatus.Paused)
                {
                    Status = TimerStatus.Stopped;
                }

                if (_canSaveTime)
                {
                    Time = value.GetValueOrDefault();
                }

                DateTime time = value.GetValueOrDefault();
                if (time.Hour == 0 && time.Minute == 0 && time.Second == 0)
                {
                    IsStartPauseResumeButtonEnabled = false;
                }
                else
                {
                    if (!_isBackward)
                    {
                        IsStartPauseResumeButtonEnabled = true;
                        if (Status == TimerStatus.Stopped)
                        {
                            IsResetButtonEnabled = false;
                        }
                    }
                }

                OnPropertyChanged(nameof(SelectedTime));
            }
        }

        [XmlIgnore]
        public bool IsMuteButtonEnabled
        {
            get => _isMuteButtonEnabled;
            set
            {
                _isMuteButtonEnabled = value;
                OnPropertyChanged(nameof(IsMuteButtonEnabled));
            }
        }

        public TimerModel()
        {
        }

        public TimerModel(int number)
        {
            Number = number;
        }

        #region StartPauseResumeCommand

        public ICommand StartPauseResumeCommand { get; } = new StartPauseResume();

        private class StartPauseResume : ICommand
        {
            private TimerModel _timer;
            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                if (_timer == null)
                {
                    _timer = (TimerModel) parameter;
                }

                if (_timer.SelectedTime > DateTime.MinValue)
                {
                    switch (_timer.Status)
                    {
                        case TimerStatus.Stopped:
                            _timer.Status = TimerStatus.Started;
                            _timer._canSaveTime = false;
                            _timer.StopTimer = false;
                            _timer._isBackward = false;
                            _timer.IsResetButtonEnabled = true;
                            _timer.Time = _timer.SelectedTime.GetValueOrDefault(DateTime.MinValue);
                            _timer._dispatcherTimer.Tick -= DispatcherTimer_Tick;
                            _timer._dispatcherTimer.Tick += DispatcherTimer_Tick;
                            _timer._dispatcherTimer.Start();
                            break;
                        case TimerStatus.Started:
                            _timer._dispatcherTimer.Tick -= DispatcherTimer_Tick;
                            _timer.Status = TimerStatus.Paused;
                            break;
                        case TimerStatus.Paused:
                            _timer._dispatcherTimer.Tick += DispatcherTimer_Tick;
                            _timer.Status = TimerStatus.Started;
                            break;
                    }
                }
            }

            public event EventHandler CanExecuteChanged;

            private void DispatcherTimer_Tick(object sender, EventArgs e)
            {
                DateTime time = _timer.SelectedTime.GetValueOrDefault();
                if (_timer.StopTimer)
                {
                    _timer.StopTimer = false;
                    _timer.Status = TimerStatus.Stopped;
                    _timer._dispatcherTimer.Stop();
                }
                else if (time.Hour == 0 && time.Minute == 0 && time.Second == 1 && !_timer._isBackward)
                {
                    _timer._isBackward = true;
                    _timer.SelectedTime = new DateTime();
                    _timer.Status = TimerStatus.Stopped;
                    _timer.IsStartPauseResumeButtonEnabled = false;
                    _timer.IsResetButtonEnabled = true;
                    _timer.IsAlarming = true;
                }
                else
                {
                    _timer.SelectedTime = _timer._isBackward ? time.AddSeconds(1) : time.AddSeconds(-1);
                }
            }
        }

        #endregion

        #region ResetCommand

        public ICommand ResetCommand { get; } = new Reset();

        private class Reset : ICommand
        {
            private TimerModel _timer;
            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                if (_timer == null)
                {
                    _timer = (TimerModel) parameter;
                }

                switch (_timer.Status)
                {
                    case TimerStatus.Stopped:
                        _timer._dispatcherTimer.Stop();
                        _timer.SelectedTime = _timer.Time;
                        _timer.IsAlarming = false;
                        if (_timer._isBackward)
                        {
                            _timer._isBackward = false;
                        }

                        _timer.IsStartPauseResumeButtonEnabled = true;
                        _timer.IsResetButtonEnabled = false;
                        _timer.IsTimePickerEnabled = true;
                        MainWindow.Notifier.ClearMessages(
                            new ClearByMessage("Timer " + _timer.Number + ": Time is up!!!"));
                        _timer.IsMuteButtonEnabled = true;
                        break;
                    case TimerStatus.Started:
                        _timer.StopTimer = true;
                        _timer.SelectedTime = _timer.Time;
                        _timer.Status = TimerStatus.Stopped;
                        _timer.IsStartPauseResumeButtonEnabled = true;
                        _timer.IsResetButtonEnabled = false;
                        break;
                    case TimerStatus.Paused:
                        _timer.SelectedTime = _timer.Time;
                        _timer.Status = TimerStatus.Stopped;
                        _timer.IsStartPauseResumeButtonEnabled = true;
                        _timer.IsResetButtonEnabled = false;
                        break;
                }

                _timer._canSaveTime = true;
            }

            public event EventHandler CanExecuteChanged;
        }

        #endregion

        #region RemoveTimerCommand

        public ICommand RemoveTimerCommand { get; } = new RemoveTimer();

        private class RemoveTimer : ICommand
        {
            private TimerModel _timer;
            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                if (_timer == null)
                {
                    _timer = (TimerModel) parameter;
                }

                if (_timer.IsAlarming)
                {
                    _timer.IsAlarming = false;
                    MainWindow.Notifier.ClearMessages(new ClearByMessage("Timer " + _timer.Number + ": Time is up!!!"));
                }

                MainWindow.Setup.Timers.Remove(_timer);
                _timer._dispatcherTimer.Stop();
                int number = 1;
                foreach (var t in MainWindow.Setup.Timers)
                {
                    t.Number = number;
                    number++;
                }

                if (MainWindow.Setup.Timers.Count < TimerView.MaxTimersNumber)
                {
                    TimerView.Instance.ShowAddTimerButton();
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

        private enum TimerStatus
        {
            Stopped,
            Started,
            Paused
        }
    }
}