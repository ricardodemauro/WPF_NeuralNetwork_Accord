using Accord.Math;
using Accord.Statistics;
using WPFAppRecog.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAppRecog.Models;

namespace WPFAppRecog.Databases
{
    public class OptdigitsDatabase : INotifyPropertyChanged, IDatabase
    {
        public int Classes { get { return 10; } }

        public IEnumerable<Sample> Training { get; private set; }

        public IEnumerable<Sample> Testing { get; private set; }

        public bool IsNormalized { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private double[] _mean;

        private double[] _dev;

        public OptdigitsDatabase()
        {
            Training = new List<Sample>();
            Testing = new List<Sample>();

            _mean = new double[1024];
            _dev = new double[1024];
            for (int i = 0; i < _dev.Length; i++)
            {
                _dev[i] = 1;
            }
        }

        public void Load()
        {
            List<Sample> _trainning = new List<Sample>();
            List<Sample> _testing = new List<Sample>();

            string input = Properties.Resources.optdigits_tra;

            // Load optdigits dataset into the DataGridView
            StringReader reader = new StringReader(input);

            char[] buffer = new char[(32 + 1) * 32]; // 32 chars + \n
            int count = 0;

            while (true)
            {
                int read = reader.ReadBlock(buffer, 0, buffer.Length);
                string label = reader.ReadLine();

                if (read < buffer.Length || label == null)
                    break;

                var sample = ConvertToSample(buffer, label);
                if (count > 1000)
                    _trainning.Add(sample);
                else
                    _testing.Add(sample);

                count++;
            }
            Testing = _testing;
            Training = _trainning;

            if (IsNormalized)
            {
                double[][] training = Training.GetInstances();

                _mean = training.Mean(dimension: 0);
                _dev = training.StandardDeviation();

                double[][] testing = Testing.GetInstances();

                Normalize(training);
                Normalize(testing);
            }
        }

        public void Normalize(double[][] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs[i].Length; j++)
                {
                    inputs[i][j] -= _mean[j];

                    if (_dev[j] != 0)
                        inputs[i][j] /= _dev[j];
                }
            }
        }

        public void Normalize(double[] inputs)
        {
            for (int j = 0; j < inputs.Length; j++)
            {
                inputs[j] -= _mean[j];

                if (_dev[j] != 0)
                    inputs[j] /= _dev[j];

            }
        }

        public Bitmap ToBitmap(double[] features)
        {
            if (features.Length != 1024)
                throw new Exception();

            Bitmap bitmap = new Bitmap(32, 32, PixelFormat.Format32bppRgb);

            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    int c = i * 32 + j;
                    double v = (features[c] * _dev[c]) + _mean[c];
                    v = v.Scale(0, 1, 255.0, 0.0);
                    bitmap.SetPixel(j, i, Color.FromArgb((int)v, (int)v, (int)v));
                }
            }

            return bitmap;
        }

        public double[] ToFeatures(Bitmap bitmap)
        {
            return bitmap.ToFeatures();
        }

        #region Private Methods
        private Sample ConvertToSample(char[] buffer, string label)
        {
            Bitmap bitmap = Extract(new string(buffer));
            var image = bitmap.ToBitmapImage();

            double[] features = ToFeatures(bitmap);
            int classLabel = int.Parse(label);

            return new Sample()
            {
                Image = image,
                Class = classLabel,
                Features = features
            };
        }

        private static Bitmap Extract(string text)
        {
            Bitmap bitmap = new Bitmap(32, 32, PixelFormat.Format32bppRgb);
            string[] lines = text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if (lines[i][j] == '0')
                        bitmap.SetPixel(j, i, Color.White);
                    else bitmap.SetPixel(j, i, Color.Black);
                }
            }
            return bitmap;
        }
        #endregion Private Methods
    }
}
