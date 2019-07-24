using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace ClockApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon NotificationAreaIcon { get; set; } = new System.Windows.Forms.NotifyIcon();

        public static MediaPlayer Player { get; set; } = new MediaPlayer();
        public MainWindow()
        {
            this.Title = "Clock";

            InitializeComponent();

            NotificationAreaIcon.Icon = new System.Drawing.Icon("alarm.ico"); // this.notifier.Icon = ForumProjects.Properties.Resources.A;
            NotificationAreaIcon.Visible = false;
            NotificationAreaIcon.MouseClick += OpenWindow;
            NotificationAreaIcon.MouseDown += OpenNotifierContextMenu;

            Player.Open(new Uri("audio.mp3", UriKind.Relative));
            Player.Volume = 0.4;
        }

        private void OpenNotifierContextMenu(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                System.Windows.Controls.ContextMenu menu = (System.Windows.Controls.ContextMenu)this.FindResource("NotifierContextMenu");
                menu.IsOpen = true;
            }
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
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
                Icon.Focus();
                NotificationAreaIcon.Visible = false;
            }
        }
    }
}
