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
        private System.Windows.Forms.NotifyIcon notifier = new System.Windows.Forms.NotifyIcon();
        public List<TimerModel> TimersList = new List<TimerModel>()
        {
            new TimerModel(1),new TimerModel(2),new TimerModel(3),new TimerModel(4),new TimerModel(5)
        };

        public MainWindow()
        {
            this.Title = "Clock";

            InitializeComponent();

            ItemsControl.ItemsSource = TimersList;

            notifier.Icon = new System.Drawing.Icon("alarm.ico"); // this.notifier.Icon = ForumProjects.Properties.Resources.A;
            notifier.Visible = false;
            notifier.MouseClick += OpenWindow;
            notifier.MouseDown += OpenNotifierContextMenu;
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
            notifier.Visible = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            notifier.Visible = false;
        }

        private void OpenWindow(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                Icon.Focus();
                notifier.Visible = false;
            }
            
        }

        private void TimePicker_Initialized(object sender, EventArgs e)
        {
            MaterialDesignThemes.Wpf.TimePicker timePicker = (MaterialDesignThemes.Wpf.TimePicker)sender;
            timePicker.SelectedTime = DateTime.MinValue;
        }

        private void TimerCloseButtonClick(object sender, RoutedEventArgs e)
        {
            TimersList.RemoveAt(0);
            ItemsControl.ApplyTemplate();
        }
    }
}
