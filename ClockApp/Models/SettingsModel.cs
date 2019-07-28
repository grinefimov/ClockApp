using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using ClockApp.Annotations;

namespace ClockApp.Models
{
    [Serializable]
    public class SettingsModel : INotifyPropertyChanged
    {
        private double _alarmVolume = 50;
        private string _audioFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\alarm.mp3";
        private string _audioName;

        public event PropertyChangedEventHandler PropertyChanged;

        public double AlarmVolume
        {
            get => _alarmVolume;
            set
            {
                _alarmVolume = value;
                OnPropertyChanged(nameof(AlarmVolume));
            }
        }

        public string AudioFilePath
        {
            get => _audioFilePath;
            set
            {
                _audioFilePath = value;
                AudioName = Path.GetFileName(value);
            }
        }

        public string AudioName
        {
            get => _audioName;
            set
            {
                _audioName = value;
                OnPropertyChanged(nameof(AudioName));
            }
        }

        public SettingsModel()
        {
            AudioName = !File.Exists(AudioFilePath) ? "Not Found" : Path.GetFileName(AudioFilePath);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}