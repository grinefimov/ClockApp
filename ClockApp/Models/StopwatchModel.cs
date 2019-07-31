using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ClockApp.Annotations;

namespace ClockApp.Models
{
    public class MainStopwatch : StopwatchModel
    {
        private readonly System.Windows.Threading.DispatcherTimer _dispatcherTimer =
            new System.Windows.Threading.DispatcherTimer() {Interval = new TimeSpan(0, 0, 0, 0, 1)};

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly Stopwatch _lapStopwatch = new Stopwatch();
        private StopwatchStatus _status = StopwatchStatus.Stopped;
        private bool _needStop = false;
        private bool _isResetButtonEnabled = false;
        private bool _isLapButtonEnabled = false;


        private StopwatchStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                switch (value)
                {
                    case StopwatchStatus.Stopped:
                        StartPauseResumeButtonText = "Start";
                        break;
                    case StopwatchStatus.Started:
                        StartPauseResumeButtonText = "Pause";
                        break;
                    case StopwatchStatus.Paused:
                        StartPauseResumeButtonText = "Resume";
                        break;
                }

                OnPropertyChanged(nameof(StartPauseResumeButtonText));
            }
        }

        public ObservableCollection<StopwatchModel> Laps { get; set; } = new ObservableCollection<StopwatchModel>();
        public string StartPauseResumeButtonText { get; set; } = "Start";

        public bool IsResetButtonEnabled
        {
            get => _isResetButtonEnabled;
            set
            {
                _isResetButtonEnabled = value;
                OnPropertyChanged(nameof(IsResetButtonEnabled));
            }
        }

        public bool IsLapButtonEnabled
        {
            get => _isLapButtonEnabled;
            set
            {
                _isLapButtonEnabled = value;
                OnPropertyChanged(nameof(IsLapButtonEnabled));
            }
        }

        public void StartPauseResume()
        {
            if (Status == StopwatchStatus.Stopped || Status == StopwatchStatus.Paused)
            {
                Status = StopwatchStatus.Started;
                _stopwatch.Start();
                _lapStopwatch.Start();
                _dispatcherTimer.Tick -= DispatcherTimerOnTick;
                _dispatcherTimer.Tick += DispatcherTimerOnTick;
                _dispatcherTimer.Start();
            }
            else
            {
                Status = StopwatchStatus.Paused;
                _stopwatch.Stop();
                _lapStopwatch.Stop();
                _needStop = true;
            }

            IsResetButtonEnabled = true;
            IsLapButtonEnabled = true;
        }

        public void Reset()
        {
            Status = StopwatchStatus.Stopped;
            _dispatcherTimer.Stop();
            _stopwatch.Reset();
            _lapStopwatch.Reset();
            TotalTime = new TimeSpan();
            LapTime = new TimeSpan();
            IsResetButtonEnabled = false;
            IsLapButtonEnabled = false;
            Laps.Clear();
        }

        public void AddLap()
        {
            Laps.Insert(0, new StopwatchModel(Laps.Count + 1, TotalTime, LapTime));
            if (Status == StopwatchStatus.Started)
            {
                _lapStopwatch.Restart();
            }
            else
            {
                _lapStopwatch.Reset();
                LapTime = TimeSpan.Zero;
            }
        }

        private void DispatcherTimerOnTick(object sender, EventArgs e)
        {
            TotalTime = _stopwatch.Elapsed;
            LapTime = _lapStopwatch.Elapsed;
            if (_needStop)
            {
                _dispatcherTimer.Stop();
                _needStop = false;
            }
        }

        private enum StopwatchStatus
        {
            Stopped,
            Started,
            Paused
        }
    }

    public class StopwatchModel : INotifyPropertyChanged
    {
        private TimeSpan _totalTime;
        private string _totalTimeString;
        private TimeSpan _lapTime;
        private string _lapTimeString;

        public int Number { get; set; }

        public TimeSpan TotalTime
        {
            get => _totalTime;
            set
            {
                _totalTime = value;
                TotalTimeString = value.ToStopwatchString();
            }
        }

        public string TotalTimeString
        {
            get => _totalTimeString;
            set
            {
                _totalTimeString = value;
                OnPropertyChanged(nameof(TotalTimeString));
            }
        }

        public TimeSpan LapTime
        {
            get => _lapTime;
            set
            {
                _lapTime = value;
                LapTimeString = value.ToStopwatchString();
            }
        }

        public string LapTimeString
        {
            get => _lapTimeString;
            set
            {
                _lapTimeString = value;
                OnPropertyChanged(nameof(LapTimeString));
            }
        }

        public StopwatchModel()
        {
            TotalTime = new TimeSpan();
            LapTime = new TimeSpan();
        }

        public StopwatchModel(int number, TimeSpan totalTime, TimeSpan lapTime)
        {
            Number = number;
            TotalTime = totalTime;
            LapTime = lapTime;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class StopwatchDateTimeExtensions
    {
        public static string ToStopwatchString(this TimeSpan time) =>
            $"{time.Minutes:00}" + ":" + $"{time.Seconds:00}" + ":" + $"{time.Milliseconds:000}";
    }
}