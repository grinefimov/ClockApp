using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ClockApp.Annotations;
using MaterialDesignThemes.Wpf;
using ToastNotifications.Lifetime.Clear;

namespace ClockApp
{
    public class TimerModel : INotifyPropertyChanged
    {
        private TimerStatus _status = TimerStatus.Stopped;
        private bool _isAlarming = false;
        private bool _isStartPauseResumeButtonEnabled = false;
        private bool _isResetButtonEnabled = false;
        private int _number;
        private DateTime? _selectedTime = DateTime.MaxValue; // To change it in TimePicker_Initialized(object sender, EventArgs e),
        // so SelectedTimeChanged event raised and 0:00 changed to 0:00:00
        private ICommand _startPauseResumeCommand;
        private ICommand _resetCommand;
        private ICommand _removeTimerCommand;
        private DateTime Time { get; set; }
        private System.Windows.Threading.DispatcherTimer DispatcherTimer { get; set; } = new System.Windows.Threading.DispatcherTimer();
        private bool IsBackward { get; set; } = false;
        private TimerStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                switch (value)
                {
                    case TimerStatus.Stopped:
                        StartPauseResumeButtonText = "Start";
                        break;
                    case TimerStatus.Started:
                        StartPauseResumeButtonText = "Pause";
                        break;
                    case TimerStatus.Paused:
                        StartPauseResumeButtonText = "Resume";
                        break;
                }
                OnPropertyChanged(nameof(StartPauseResumeButtonText));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
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
                    MainWindow.Notifier.ShowCustomMessage("Timer " + Number + ": Time is up!!!");
                }

                if (!value)
                {
                    MainWindow.Player.Stop();
                }
                OnPropertyChanged(nameof(IsAlarming));
            }
        }

        public bool StopTimer { get; set; } = false;
        public string StartPauseResumeButtonText { get; set; } = "Start";
        public bool IsStartPauseResumeButtonEnabled
        {
            get => _isStartPauseResumeButtonEnabled;
            set
            {
                _isStartPauseResumeButtonEnabled = value;
                OnPropertyChanged(nameof(IsStartPauseResumeButtonEnabled));
            }
        }
        public bool IsResetButtonEnabled
        {
            get => _isResetButtonEnabled;
            set
            {
                _isResetButtonEnabled = value;
                OnPropertyChanged(nameof(IsResetButtonEnabled));
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
                DateTime time = value.GetValueOrDefault(DateTime.MinValue);
                if (time.Hour == 0 && time.Minute == 0 && time.Second == 0)
                {
                    IsStartPauseResumeButtonEnabled = false;
                }
                else
                {
                    if (!IsBackward)
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

        public TimerModel(int number)
        {
            Number = number;
        }

        #region StartPauseResumeCommand
        public ICommand StartPauseResumeCommand => _startPauseResumeCommand ??= new StartPauseResume();

        private class StartPauseResume : ICommand
        {
            private TimerModel _timer;
            public bool CanExecute(object parameter) => true;
            public void Execute(object parameter)
            {
                if (_timer == null)
                {
                    _timer = (TimerModel)parameter;
                    _timer.DispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                }
                if (_timer.SelectedTime > DateTime.MinValue)
                {
                    switch (_timer.Status)
                    {
                        case TimerStatus.Stopped:
                            _timer.Status = TimerStatus.Started;
                            _timer.StopTimer = false;
                            _timer.IsBackward = false;
                            _timer.IsResetButtonEnabled = true;
                            _timer.Time = _timer.SelectedTime.GetValueOrDefault(DateTime.MinValue);
                            _timer.DispatcherTimer.Tick -= DispatcherTimer_Tick;
                            _timer.DispatcherTimer.Tick += DispatcherTimer_Tick;
                            _timer.DispatcherTimer.Start();
                            break;
                        case TimerStatus.Started:
                            _timer.DispatcherTimer.Tick -= DispatcherTimer_Tick;
                            _timer.Status = TimerStatus.Paused;
                            break;
                        case TimerStatus.Paused:
                            _timer.DispatcherTimer.Tick += DispatcherTimer_Tick;
                            _timer.Status = TimerStatus.Started;
                            break;
                    }
                }
            }
            public event EventHandler CanExecuteChanged;

            private void DispatcherTimer_Tick(object sender, EventArgs e)
            {
                DateTime time = _timer.SelectedTime.GetValueOrDefault(DateTime.MinValue);
                if (_timer.StopTimer)
                {
                    _timer.StopTimer = false;
                    _timer.Status = TimerStatus.Stopped;
                    _timer.DispatcherTimer.Stop();
                }
                else if (time.Hour == 0 && time.Minute == 0 && time.Second == 1 && !_timer.IsBackward)
                {
                    _timer.IsBackward = true;
                    _timer.SelectedTime = DateTime.MinValue;
                    _timer.Status = TimerStatus.Stopped;
                    _timer.IsStartPauseResumeButtonEnabled = false;
                    _timer.IsResetButtonEnabled = true;
                    _timer.IsAlarming = true;
                }
                else
                {
                    _timer.SelectedTime = _timer.IsBackward ? time.AddSeconds(1) : time.AddSeconds(-1);
                }
            }
        }

        #endregion

        #region ResetCommand
        public ICommand ResetCommand => _resetCommand ??= new Reset();

        private class Reset : ICommand
        {
            private TimerModel _timer;
            public bool CanExecute(object parameter) => true;
            public void Execute(object parameter)
            {
                if (_timer == null)
                {
                    _timer = (TimerModel)parameter;
                }
                switch (_timer.Status)
                {
                    case TimerStatus.Stopped:
                        _timer.DispatcherTimer.Stop();
                        _timer.SelectedTime = _timer.Time;
                        _timer.IsAlarming = false;
                        _timer.IsStartPauseResumeButtonEnabled = true;
                        _timer.IsResetButtonEnabled = false;
                        MainWindow.Notifier.ClearMessages(new ClearByMessage("Timer " + _timer.Number + ": Time is up!!!"));
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
            }
            public event EventHandler CanExecuteChanged;
        }
        #endregion

        #region RemoveTimerCommand
        public ICommand RemoveTimerCommand => _removeTimerCommand ??= new RemoveTimer();

        private class RemoveTimer : ICommand
        {
            private TimerModel _timer;
            public bool CanExecute(object parameter) => true;
            public void Execute(object parameter)
            {
                if (_timer == null)
                {
                    _timer = (TimerModel)parameter;
                }
                if (_timer.IsAlarming)
                {
                    _timer.IsAlarming = false;
                    MainWindow.Notifier.ClearMessages(new ClearByMessage("Timer " + _timer.Number + ": Time is up!!!"));
                }
                TimerViewModel.Timers.RemoveAt(_timer.Number - 1);
                _timer.DispatcherTimer.Stop();
                int number = 1;
                foreach (var t in TimerViewModel.Timers)
                {
                    t.Number = number;
                    number++;
                }
                if (TimerViewModel.Timers.Count < 5)
                {
                    TimerView.Instance.ShowAddTimerButton();
                }
            }
            public event EventHandler CanExecuteChanged;
        }
        #endregion

        enum TimerStatus
        {
            Stopped,Started,Paused
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class TimerViewModel
    {
        public static ObservableCollection<TimerModel> Timers { get; } = new ObservableCollection<TimerModel>
        {
            new TimerModel(1)
        };
    }

    }