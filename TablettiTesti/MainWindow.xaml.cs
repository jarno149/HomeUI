using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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

namespace TablettiTesti
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MotionDetector detector = new MotionDetector();
            var devs = detector.GetVideoDevices();
            label.Content = devs.Count.ToString();
            detector.Init(devs[0]);
            detector.ImageValue += Detector_ImageValue;
            detector.IncomingImage += Detector_IncomingImage;
            detector.Start();
        }

        private void Detector_IncomingImage(object sender, EventArgs e)
        {
            image.Source = MotionDetector.CreateBitmapSourceFromGdiBitmap((Bitmap)sender);
        }

        private void Detector_ImageValue(object sender, EventArgs e)
        {
            label.Content = (int)sender;
        }
    }
}
