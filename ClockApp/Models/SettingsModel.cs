using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using ClockApp.Annotations;
using Microsoft.Win32;

namespace ClockApp.Models
{
    [Serializable]
    public class SettingsModel : INotifyPropertyChanged
    {
        private double _alarmVolume = 50;
        private string _audioFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\alarm.mp3";
        private string _audioName;
        private int _snoozeLength = 10;
        private bool _launchOnStartup = false;

        private readonly RegistryKey _registryKey = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        public event PropertyChangedEventHandler PropertyChanged;

        public double AlarmVolume
        {
            get => _alarmVolume;
            set => _alarmVolume = value;
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

        public int SnoozeLength
        {
            get => _snoozeLength;
            set
            {
                _snoozeLength = value == 0 ? 1 : value;
                OnPropertyChanged(nameof(SnoozeLength));
            }
        }

        public bool LaunchOnStartup
        {
            get => _launchOnStartup;
            set
            {
                _launchOnStartup = value;
                if (_launchOnStartup)
                    _registryKey.SetValue("ClockApp",
                        Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\ClockApp.exe");
                else
                    _registryKey.DeleteValue("ClockApp", false);
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