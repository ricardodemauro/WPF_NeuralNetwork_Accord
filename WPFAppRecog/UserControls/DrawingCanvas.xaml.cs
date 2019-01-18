using Accord.Imaging.Filters;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WPFAppRecog.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DrawingCanvas : UserControl
    {
        public double[] UserFeatures
        {
            get { return (double[])GetValue(UserFeaturesProperty); }
            set { SetValue(UserFeaturesProperty, value); }
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }


        public static readonly DependencyProperty UserFeaturesProperty =
          DependencyProperty.Register("UserFeatures", typeof(double[]), typeof(DrawingCanvas));

        public static readonly DependencyProperty IsActiveProperty =
          DependencyProperty.Register("IsActive", typeof(bool), typeof(DrawingCanvas));


        RenderTargetBitmap rtb;
        ResizeNearestNeighbor resize = new ResizeNearestNeighbor(32, 32);
        DispatcherTimer timer;

        public DrawingCanvas()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((bool)GetValue(IsActiveProperty))
                UserFeatures = GetFeatures();
        }

        private double[] GetFeatures()
        {
            rtb = new RenderTargetBitmap((int)InkCanvas1.ActualWidth, (int)InkCanvas1.ActualHeight,
                96d, 96d, PixelFormats.Default);

            rtb.Render(InkCanvas1);

            //save the ink to a memory stream
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            byte[] bitmapBytes;
            Bitmap bmp;
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                ms.Position = 0;
                bitmapBytes = ms.ToArray();

                ms.Seek(0, SeekOrigin.Begin);
                bmp = new Bitmap(ms);
            }

            bmp = resize.Apply(bmp);

            double[] features = ToFeatures(bmp);

            return features;
        }

        public void Clear()
        {
            InkCanvas1.Strokes.Clear();
        }

        public static double[] ToFeatures(Bitmap bmp)
        {
            double[] features = new double[32 * 32];
            for (int i = 0; i < 32; i++)
                for (int j = 0; j < 32; j++)
                    features[i * 32 + j] = (bmp.GetPixel(j, i).R > 0) ? 0 : 1;

            return features;
        }
    }
}
