using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WPFAppRecog.Models
{
    public class Sample
    {
        public BitmapImage Image { get; set; }

        public int Class { get; set; }

        public double[] Features { get; set; }
    }
}
