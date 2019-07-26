using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace ClockApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        public MainWindow()
        {
            try
            {
                Settings = Serializer.ReadFromXmlFile<SettingsModel>(Environment.GetFolderPath(
                                                                    Environment.SpecialFolder.LocalApplicationData) + "\\ClockApp\\settings");
            }
            catch (FileNotFoundException)
            {
                Settings = new SettingsModel();
            }
            catch (DirectoryNotFoundException)
            {
                Settings = new SettingsModel();
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ClockApp");
            }

            InitializeComponent();

            NotificationAreaIcon.Icon = new Icon(System.Windows.Application.GetResourceStream(
                new Uri("pack://application:,,,/Resources/alarm-colored-bg.ico")).Stream);
            NotificationAreaIcon.Visible = false;
            NotificationAreaIcon.MouseClick += OpenWindow;
            NotificationAreaIcon.MouseDown += OpenNotifierContextMenu;
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
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
                var menu = (System.Windows.Controls.ContextMenu)this.FindResource("NotifierContextMenu");
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
}
