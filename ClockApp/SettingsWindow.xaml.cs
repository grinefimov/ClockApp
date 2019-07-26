using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using ClockApp.Annotations;

namespace ClockApp
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            this.DataContext = MainWindow.Settings;

            InitializeComponent();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            Serializer.WriteToXmlFile(Environment.GetFolderPath(
                                          Environment.SpecialFolder.LocalApplicationData) + "\\ClockApp\\settings", MainWindow.Settings);
            this.Close();
        }

        private void SetDefaultSettings(object sender, RoutedEventArgs e)
        {
            MainWindow.Settings.AlarmVolume = 50;
            MainWindow.Settings.AudioFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\alarm.mp3";
            MainWindow.Settings.AudioName = "alarm.mp3";

        }

        private void OpenAudioFile(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".mp3", Filter = "MP3 Files (*.mp3)|*.mp3|WAV Files (*.wav)|*.wav"
            };
            bool? result = dlg.ShowDialog();
            if (result != true) return;
            string filename = dlg.FileName;
            MainWindow.Settings.AudioFilePath = filename;
        }
    }

    [Serializable]
    public class SettingsModel : INotifyPropertyChanged
    {
        private double _alarmVolume = 50;
        private string _audioFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\alarm.mp3";
        private string _audioName = "alarm.mp3";

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
