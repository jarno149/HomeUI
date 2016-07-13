using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TablettiTesti.Utils
{
    class MotionDetector
    {
        public VideoCaptureDevice CaptureDevice;
        private Bitmap first;
        private Bitmap second;
        private Bitmap third;
        private DateTime Timeout = DateTime.Now;

        public FilterInfoCollection GetVideoDevices()
        {
            return new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }

        public event EventHandler<EventArgs> IncomingImage;
        public event EventHandler<EventArgs> ImageValue;

        public void Init(FilterInfo captureDevice)
        {
            CaptureDevice = new VideoCaptureDevice(captureDevice.MonikerString);
            var capabs = CaptureDevice.VideoCapabilities;
            CaptureDevice.VideoResolution = capabs[2];
            CaptureDevice.NewFrame += CaptureDevice_NewFrame;
            CaptureDevice.Start();
        }

        void CaptureDevice_SnapshotFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            
        }

        public void AddTimeout(int seconds)
        {
            if (this.Timeout == null)
                return;
            this.Timeout = DateTime.Now + new TimeSpan(0,0,seconds);
        }

        public async void StartCapture()
        {
            while(true)
            {
                CaptureDevice.SimulateTrigger();
                Task.Delay(500);
            }
        }

        private int Counter = 15;
        private async void CaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
                if (Counter == 0)
                {
                    if (Timeout <= DateTime.Now)
                    {

                     if (IncomingImage != null)
                          IncomingImage.Invoke(eventArgs.Frame.Clone(), EventArgs.Empty);

                    if (second != null)
                        third = (Bitmap)second.Clone();
                    if (first != null)
                        second = (Bitmap)first.Clone();
                    first = (Bitmap)eventArgs.Frame.Clone();

                    if (first == null || second == null || third == null)
                    {
                        Counter = 15;
                        return;
                    }
                        

                    if (ImageComparer(first, second))
                    {
                        if (ImageComparer(first, third))
                        {
                            ScreenHandler.SetMonitorState(ScreenHandler.MonitorState.ON);
                          //  ScreenHandler.Wake();
                            AddTimeout(5);
                            Counter = 0;
                        }
                        else
                        {
                            ScreenHandler.SetMonitorState(ScreenHandler.MonitorState.OFF);
                        }
                    }
                    else
                    {
                        ScreenHandler.SetMonitorState(ScreenHandler.MonitorState.OFF);
                    }
                    Counter = 15;
                }
            }
                else
                {
                    Counter--;
                }
        }

        public void Start()
        {
            CaptureDevice.Start();
        }

        public void CaptureStill()
        {
            CaptureDevice.SimulateTrigger();
        }

        private int last = 0;
        private bool ImageComparer(Bitmap first, Bitmap second)
        {
            long pixelCounter = 0;
            for (int i = 0; i < first.Height; i+=10)
            {
                for (int j = 0; j < first.Width; j+=10)
                {
                    int diff = 0;
                    diff += Math.Abs(first.GetPixel(j, i).R - second.GetPixel(j,i).R);
                    diff += Math.Abs(first.GetPixel(j, i).G - second.GetPixel(j,i).G);
                    diff += Math.Abs(first.GetPixel(j, i).B - second.GetPixel(j,i).B);
                    diff += Math.Abs(first.GetPixel(j, i).A - second.GetPixel(j, i).A);
                    pixelCounter += diff;
                }
            }
            if (ImageValue != null)
                ImageValue.Invoke(pixelCounter, EventArgs.Empty);

            double result = pixelCounter / (first.Height * first.Width / 100);
            int threshold = 2;
            if (result > threshold + last || result < last - threshold)
            {
                Console.WriteLine("MOVEMENT!");
                Console.WriteLine(last + " " + (pixelCounter / (first.Height * first.Width / 100)));
                last = (int)pixelCounter / (first.Height * first.Width / 100);
                return true;
            }
            last = (int)pixelCounter / (first.Height * first.Width / 100);
            return false;
        }

        bool TestRange(int numberToCheck, int bottom, int top)
        {
            return (numberToCheck >= bottom && numberToCheck <= top);
        }

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }
    }
}
