using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using ClockApp.Models;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace ClockApp
{
    public partial class MainWindow : Window
    {
        private readonly bool _foundSetup = false;
        private NotifyIcon NotificationAreaIcon { get; set; } = new NotifyIcon();

        public static MediaPlayer Player { get; set; } = new MediaPlayer();

        public static Notifier Notifier { get; set; } = new Notifier(cfg =>
        {
            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                TimeSpan.FromDays(365),
                MaximumNotificationCount.FromCount(10)
            );
            cfg.PositionProvider = new PrimaryScreenPositionProvider(Corner.BottomRight, 10, 10);
            cfg.DisplayOptions.Width = 325;
        });

        public static SettingsModel Settings { get; set; }

        public static SetupModel Setup { get; private set; } = new SetupModel()
        {
            Timers = new ObservableCollection<TimerModel> {new TimerModel(1)},
            Alarms = new ObservableCollection<AlarmModel> {new AlarmModel(1)}
        };

        public MainWindow()
        {
            #region Loading Settings

            try
            {
                Settings = Serializer.ReadFromXmlFile<SettingsModel>(Environment.GetFolderPath(
                                                                         Environment.SpecialFolder
                                                                             .LocalApplicationData) +
                                                                     "\\ClockApp\\settings");
            }
            catch (FileNotFoundException)
            {
                Settings = new SettingsModel();
            }
            catch (DirectoryNotFoundException)
            {
                Settings = new SettingsModel();
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                          "\\ClockApp");
            }

            #endregion

            #region Loading Setup

            try
            {
                Setup = Serializer.ReadFromXmlFile<SetupModel>(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ClockApp\\setup");
                foreach (var t in Setup.Timers)
                {
                    t.SelectedTime = t.Time;
                }

                _foundSetup = true;
            }
            catch (FileNotFoundException)
            {
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                          "\\ClockApp");
            }

            #endregion

            InitializeComponent();

            if (_foundSetup)
            {
                switch (Setup.SelectedTab)
                {
                    case SetupModel.ClockAppTabs.Alarms:
                        AlarmsTab.IsSelected = true;
                        break;
                    case SetupModel.ClockAppTabs.Stopwatch:
                        StopwatchTab.IsSelected = true;
                        break;
                    case SetupModel.ClockAppTabs.Timer:
                        TimerTab.IsSelected = true;
                        break;
                }
            }

            NotificationAreaIcon.Icon = new Icon(System.Windows.Application.GetResourceStream(
                new Uri("pack://application:,,,/Resources/alarm-colored-bg.ico")).Stream);
            NotificationAreaIcon.Visible = false;
            NotificationAreaIcon.MouseClick += OpenWindow;
            NotificationAreaIcon.MouseDown += OpenNotifierContextMenu;
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            if (AlarmsTab.IsSelected) Setup.SelectedTab = SetupModel.ClockAppTabs.Alarms;
            else if (StopwatchTab.IsSelected) Setup.SelectedTab = SetupModel.ClockAppTabs.Stopwatch;
            else Setup.SelectedTab = SetupModel.ClockAppTabs.Timer;
            Serializer.WriteToXmlFile(Environment.GetFolderPath(
                                          Environment.SpecialFolder.LocalApplicationData) + "\\ClockApp\\setup", Setup);
            System.Windows.Application.Current.Shutdown();
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void MinimizeApp(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void HideWindow(object sender, RoutedEventArgs e)
        {
            this.Hide();
            NotificationAreaIcon.Visible = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            NotificationAreaIcon.Visible = false;
        }

        private void OpenWindow(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                Icon.Focus(); // To hide "To notification area" tooltip
                NotificationAreaIcon.Visible = false;
            }
        }

        private void OpenNotifierContextMenu(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var menu = (System.Windows.Controls.ContextMenu) this.FindResource("NotifierContextMenu");
                menu.IsOpen = true;
            }
        }

        private void OpenSettingsWindow(object sender, MouseButtonEventArgs e)
        {
            Window settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void OpenAboutWindow(object sender, MouseButtonEventArgs e)
        {
            Window aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }

        public static void PlayAudio()
        {
            Player.Open(new Uri(Settings.AudioFilePath));
            Player.Volume = Settings.AlarmVolume / 100;
            Player.Play();
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
                return (T) serializer.Deserialize(reader);
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}