using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace ClockApp
{
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
            MainWindow.SettingsWindow = null;
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            Serializer.WriteToXmlFile(Environment.GetFolderPath(
                                          Environment.SpecialFolder.LocalApplicationData) + "\\ClockApp\\settings",
                MainWindow.Settings);
            this.Close();
            MainWindow.SettingsWindow = null;
        }

        private void SetDefaultSettings(object sender, RoutedEventArgs e)
        {
            MainWindow.Settings.AlarmVolume = 50;
            MainWindow.Settings.AudioFilePath =
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\alarm.mp3";
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
}