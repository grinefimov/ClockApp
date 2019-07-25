using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
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
                AudioName = Path.GetFileNameWithoutExtension(value);
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

    public class Serializer
    {
        /// <summary>
        /// Writes the given object instance to an XML file.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                writer?.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}
