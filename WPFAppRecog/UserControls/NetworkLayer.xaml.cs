using Accord.Neuro;
using System;
using System.Collections.Generic;
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

namespace WPFAppRecog.UserControls
{
    /// <summary>
    /// Interaction logic for NetworkLayer.xaml
    /// </summary>
    public partial class NetworkLayer : UserControl
    {
        private ActivationLayer layer;

        private bool inputOnly;

        public bool InputOnly
        {
            get { return inputOnly; }
            set
            {
                inputOnly = value;
                update();
            }
        }

        public ActivationLayer Layer
        {
            get { return layer; }
            set
            {
                layer = value;
                update();
            }
        }

        public int Maximum { get; set; }

        public bool Selected { get; set; }

        public int Index { get; set; }

        public int Inputs { get; set; }

        private void update()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            stackPanel1.Children.Clear();

            if (layer == null)
            {
                int count = Inputs;
                if (count > Maximum) count = Maximum;

                for (int i = 0; i < count; i++)
                    stackPanel1.Children.Add(new Polygon());

                lbCount.Content = count;
            }

            else
            {
                int count = layer.Neurons.Length;
                if (count > Maximum) count = Maximum;

                for (int i = 0; i < count; i++)
                    stackPanel1.Children.Add(new Ellipse());

                lbCount.Content = count;
            }
        }

        public NetworkLayer()
        {
            InitializeComponent();

            Maximum = 1024;
        }
    }
}
