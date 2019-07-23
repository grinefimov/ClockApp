using System;
using System.Collections.Generic;
using System.Linq;
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
        private System.Windows.Forms.NotifyIcon Notifier { get; set; } = new System.Windows.Forms.NotifyIcon();

        public MainWindow()
        {
            this.Title = "Clock";

            InitializeComponent();

            Notifier.Icon = new System.Drawing.Icon("alarm.ico"); // this.notifier.Icon = ForumProjects.Properties.Resources.A;
            Notifier.Visible = false;
            Notifier.MouseClick += OpenWindow;
            Notifier.MouseDown += OpenNotifierContextMenu;
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
            Notifier.Visible = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Notifier.Visible = false;
        }

        private void OpenWindow(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                Icon.Focus();
                Notifier.Visible = false;
            }
        }
    }
}
