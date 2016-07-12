using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
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

        public FilterInfoCollection GetVideoDevices()
        {
            return new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }

        public event EventHandler<EventArgs> IncomingImage;
        public event EventHandler<EventArgs> ImageValue;

        public void Init(FilterInfo captureDevice)
        {
            CaptureDevice = new VideoCaptureDevice(captureDevice.MonikerString);
            CaptureDevice.NewFrame += CaptureDevice_NewFrame;
        }

        private async void CaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            third = second;
            second = first;
            first = eventArgs.Frame;

            // Compare images
            if(ImageComparer(first, second))
            {
                if(ImageComparer(first, third))
                {
                    ScreenHandler.SetMonitorState(ScreenHandler.MonitorState.OFF);
                }
            }

            CaptureDevice.Stop();
            await Task.Delay(500);
            CaptureDevice.Start();
        }

        public void Start()
        {
            CaptureDevice.Start();
        }

        private bool ImageComparer(Bitmap first, Bitmap second)
        {
            long pixelCounter = 0;
            for (int i = 0; i < first.Height; i++)
            {
                for (int j = 0; j < first.Width; j++)
                {
                    int diff = 0;
                    diff += Math.Abs(first.GetPixel(i, j).R - second.GetPixel(i, j).R);
                    diff += Math.Abs(first.GetPixel(i, j).G - second.GetPixel(i, j).G);
                    diff += Math.Abs(first.GetPixel(i, j).B - second.GetPixel(i, j).B);
                    pixelCounter += diff;
                }
            }
            if (ImageValue != null)
                ImageValue.Invoke(pixelCounter, EventArgs.Empty);

            if (pixelCounter / (first.Height * first.Width / 100) > 0.01)
            {
                return true;
            }
            return false;
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
