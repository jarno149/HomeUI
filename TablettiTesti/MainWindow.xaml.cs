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
            detector.IncomingImage += detector_IncomingImage;
            detector.Init(devs[0]);
           // detector.Start();
            detector.StartCapture();
        }

        void detector_IncomingImage(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            { this.image.Source = MotionDetector.CreateBitmapSourceFromGdiBitmap((Bitmap)sender); }));
            
        }
    }
}
