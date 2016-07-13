using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TablettiTesti.Utils;
using System.ComponentModel;
using AForge.Video.DirectShow;

namespace TablettiTesti
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<FilterInfo> CaptureDevices = new List<FilterInfo>();
        private FilterInfo SelectedDevice;
        public MainWindow()
        {
            InitializeComponent();

            detector = new MotionDetector();
            var devs = detector.GetVideoDevices();
            foreach (FilterInfo dev in devs)
            {
                CaptureDevices.Add(dev);
            }
            CameraCb.ItemsSource = CaptureDevices;
        }

        private MotionDetector detector;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            detector = new MotionDetector();
            var devs = detector.GetVideoDevices();
            label.Content = devs.Count.ToString();
            detector.IncomingImage += detector_IncomingImage;
            detector.Init(devs[0]);
            detector.Start();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                ScreenHandler.SetMonitorState(ScreenHandler.MonitorState.OFF);
                Thread.Sleep(4000);
               // ScreenHandler.SetMonitorState(ScreenHandler.MonitorState.ON);
                ScreenHandler.SetMonitorState(ScreenHandler.MonitorState.ON);
                Thread.Sleep(4000);
            }
        }

        void detector_IncomingImage(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            { this.image.Source = MotionDetector.CreateBitmapSourceFromGdiBitmap((Bitmap)sender); }));
            
        }

        private void kuva_Click(object sender, RoutedEventArgs e)
        {
            detector.CaptureStill();
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (detector != null)
                detector.AddTimeout(5);
        }

        private void Window_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (detector != null)
                detector.AddTimeout(5);
        }
    }
}
