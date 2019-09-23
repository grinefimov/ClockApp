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
        private readonly bool _firstLaunch = false;
        private readonly bool _foundSetup = false;
        private readonly NotifyIcon _notificationAreaIcon = new NotifyIcon();
        public static Window SettingsWindow { get; set; }
        public static Window AboutWindow { get; set; }
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
            Alarms = new ObservableCollection<AlarmModel> {new AlarmModel()}
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

            if (Settings.LaunchOnStartup == true)
            {
                _firstLaunch = true;
            }

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

            _notificationAreaIcon.Icon = new Icon(System.Windows.Application.GetResourceStream(
                new Uri("pack://application:,,,/Resources/alarm-colored-bg.ico")).Stream);
            _notificationAreaIcon.Visible = false;
            _notificationAreaIcon.MouseDoubleClick += ShowWindow;
            _notificationAreaIcon.MouseDown += OpenNotifierContextMenu;

            if (_firstLaunch == true)
            {
                _firstLaunch = false;
                ToNotificationArea(this, new RoutedEventArgs());
            }
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Window_Closed(this, EventArgs.Empty);
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

        private void ToNotificationArea(object sender, RoutedEventArgs e)
        {
            this.Hide();
            _notificationAreaIcon.Visible = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _notificationAreaIcon.Visible = false;
            if (AlarmsTab.IsSelected) Setup.SelectedTab = SetupModel.ClockAppTabs.Alarms;
            else if (StopwatchTab.IsSelected) Setup.SelectedTab = SetupModel.ClockAppTabs.Stopwatch;
            else Setup.SelectedTab = SetupModel.ClockAppTabs.Timer;
            Serializer.WriteToXmlFile(Environment.GetFolderPath(
                                          Environment.SpecialFolder.LocalApplicationData) + "\\ClockApp\\setup", Setup);
        }

        private void ShowWindow(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                Icon.Focus(); // To hide "To notification area" tooltip
                _notificationAreaIcon.Visible = false;
            }
        }

        private void ShowWindow(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            Icon.Focus(); // To hide "To notification area" tooltip
            _notificationAreaIcon.Visible = false;
        }

        private void OpenNotifierContextMenu(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var menu = (System.Windows.Controls.ContextMenu) this.FindResource("NotifierContextMenu");
                menu.IsOpen = true;
            }
        }

        private void OpenSettingsWindow(object sender, RoutedEventArgs e)
        {
            if (SettingsWindow == null)
            {
                SettingsWindow = new SettingsWindow();
                SettingsWindow.Show();
            }
            else SettingsWindow.Focus();
        }

        private void OpenAboutWindow(object sender, RoutedEventArgs e)
        {
            if (AboutWindow == null)
            {
                AboutWindow = new AboutWindow();
                AboutWindow.Show();
            }
            else AboutWindow.Focus();
        }

        public static void PlayAudio()
        {
            Player.Open(new Uri(Settings.AudioFilePath));
            Player.Volume = Settings.AlarmVolume / 100;
            Player.Play();
        }

        private void PinTopmost(object sender, RoutedEventArgs e)
        {
            this.Topmost = this.Topmost == false;
        }
    }

    public class Serializer
    {
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