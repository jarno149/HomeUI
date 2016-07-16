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
        private VideoCapabilities[] CaptureModes;
        private VideoCapabilities SelectedCaptureMode;

        public MainWindow()
        {
            InitializeComponent();

            detector = new MotionDetector();
            detector.IncomingImage += detector_IncomingImage;
            var devs = detector.GetVideoDevices();
            foreach (FilterInfo dev in devs)
            {
                CaptureDevices.Add(dev);
            }
            CameraCb.ItemsSource = CaptureDevices;
            CameraCb.SelectionChanged += CameraCb_SelectionChanged;
            CaptureModeCb.SelectionChanged += CaptureModeCb_SelectionChanged;
        }

        void CaptureModeCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedCaptureMode = CaptureModes[CaptureModeCb.SelectedIndex];
        }

        void CameraCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CameraCb.SelectedItem != null)
            {
                SelectedDevice = (FilterInfo)CameraCb.SelectedItem;
                CaptureModes = detector.GetDevicesCaptureModes(SelectedDevice);
                CaptureModeCb.Items.Clear();
                foreach (VideoCapabilities cab in CaptureModes)
                {
                    CaptureModeCb.Items.Add(cab.FrameSize.Width + "x" + cab.FrameSize.Height + " - " + cab.FrameRate + "fps");
                }
            }
                
        }

        private MotionDetector detector;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            detector.Previewing = true;
            detector.Start(SelectedDevice, SelectedCaptureMode);
        }

        void detector_IncomingImage(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            { this.image.Source = MotionDetector.CreateBitmapSourceFromGdiBitmap((Bitmap)sender); }));
            
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

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (detector != null)
                detector.Threshold = (int)Slider.Value;
        }
    }
}
