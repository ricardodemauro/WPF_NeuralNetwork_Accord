using Accord.Neuro;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFAppRecog.UserControls
{
    /// <summary>
    /// Interaction logic for NetworkDiagram.xaml
    /// </summary>
    public partial class NetworkDiagram : UserControl
    {
        public ActivationNetwork Network
        {
            get { return (ActivationNetwork)GetValue(NetworkProperty); }
            set
            {
                SetValue(NetworkProperty, value);
                UpdateNetwork();
            }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set
            {
                SetValue(SelectedIndexProperty, value);
                UpdateIndex();
            }
        }


        public static readonly DependencyProperty NetworkProperty =
          DependencyProperty.Register(nameof(Network), typeof(ActivationNetwork), typeof(NetworkDiagram));

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(NetworkDiagram));

        public NetworkDiagram()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == nameof(Network))
            {
                UpdateNetwork();
            }
            else if (e.Property.Name == nameof(SelectedIndex))
            {
                UpdateIndex();
            }
        }

        private void UpdateIndex()
        {
            int selectedIndex = SelectedIndex;

            if (selectedIndex < 1 || selectedIndex > stackPanel1.Children.Count)
                return;

            if (selectedIndex >= stackPanel1.Children.Count)
                return;

            var element = stackPanel1.Children[selectedIndex];
            NetworkLayer current = (element as NetworkLayer);
            foreach (NetworkLayer layer in stackPanel1.Children)
            {
                layer.BorderBrush = Brushes.Transparent;
            }

            if (selectedIndex > 0)
            {
                current.BorderBrush = Brushes.Black;
            }
        }

        private void UpdateNetwork()
        {
            Network network = Network;

            stackPanel1.Children.Clear();

            if (network == null) return;

            // Add Input Layer
            NetworkLayer layer = new NetworkLayer();
            layer.InputOnly = true;
            layer.Inputs = Network.InputsCount;
            layer.Layer = null;
            layer.Name = "Input";
            layer.Index = 0;
            stackPanel1.Children.Add(layer);

            // Add Hidden Layers
            for (int i = 0; i < network.Layers.Length; i++)
            {
                layer = new NetworkLayer();
                layer.InputOnly = false;
                layer.Layer = network.Layers[i] as ActivationLayer;
                layer.Name = "Hidden" + i;
                layer.Index = i + 1;
                layer.MouseLeftButtonDown += new MouseButtonEventHandler(Layer_MouseLeftButtonDown);
                stackPanel1.Children.Add(layer);
            }

            SelectedIndex = 1;
        }

        void Layer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedIndex = (sender as NetworkLayer).Index;
        }
    }
}
