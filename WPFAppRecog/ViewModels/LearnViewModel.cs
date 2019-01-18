using Accord.Neuro.Learning;
using Accord.Statistics.Analysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WPFAppRecog.Commands;
using WPFAppRecog.Extensions;
using WPFAppRecog.Models;

namespace WPFAppRecog.ViewModels
{
    public class LearnViewModel : ViewModelBase
    {
        public MainViewModel Main { get; set; }

        private IEnumerable<Sample> _display;
        public IEnumerable<Sample> Display { get { return _display; } set { Set(ref _display, value); } }

        private GeneralConfusionMatrix _confusionMatrix;
        public GeneralConfusionMatrix ConfusionMatrix { get { return _confusionMatrix; } set { Set(ref _confusionMatrix, value); } }

        /// <summary>Indicates whether a background thread is busy loading data.</summary>
        public bool IsDataLoading { get; set; }

        /// <summary>Indicates whether training data has already finished loading.</summary>
        public bool HasDataLoaded { get; private set; }

        /// <summary>Indicates the learning procedure is starting.</summary>
        public bool IsStarting { get; set; }

        private bool _isLearning;
        /// <summary>Indicates the learning procedure is currently being run.</summary>
        public bool IsLearning { get { return _isLearning; } set { Set(ref _isLearning, value); } }

        private bool _hasLearned;
        public bool HasLearned { get { return _hasLearned; } set { Set(ref _hasLearned, value); } }

        public bool CanPause { get { return IsLearning; } }
        public bool CanReset { get { return Main.CanGenerate; } }
        public bool CanTest { get { return HasDataLoaded && Main.CanClassify && !IsLearning; } }

        private int _currentEpoch;
        public int CurrentEpoch { get { return _currentEpoch; } set { Set(ref _currentEpoch, value); } }

        private double _currentEpochError;
        public double CurrentEpochError { get { return _currentEpochError; } set { Set(ref _currentEpochError, value); } }

        private bool _shouldLayerBeSupervised;
        public bool ShouldLayerBeSupervised { get { return _shouldLayerBeSupervised; } set { Set(ref _shouldLayerBeSupervised, value); } }

        private double _learningRate;
        public double LearningRate { get { return _learningRate; } set { Set(ref _learningRate, value); } }

        private double _momentum;
        public double Momentum { get { return _momentum; } set { Set(ref _momentum, value); } }

        private double _weightDecay;
        public double WeightDecay { get { return _weightDecay; } set { Set(ref _weightDecay, value); } }

        private int _epochs;
        public int Epochs { get { return _epochs; } set { Set(ref _epochs, value); } }

        private int _batchSize;
        public int BatchSize { get { return _batchSize; } set { Set(ref _batchSize, value); } }

        private DataType _currentSet;
        public DataType CurrentSet { get { return _currentSet; } set { Set(ref _currentSet, value); } }

        private DataType[] _sets;
        public DataType[] Sets { get { return _sets; } set { Set(ref _sets, value); } }

        public bool CanStart { get { return (HasDataLoaded && !IsLearning); } }

        private bool _shouldStop;

        public LearnViewModel(MainViewModel mainViewModel)
        {
            Main = mainViewModel;

            LearningRate = 0.1;
            WeightDecay = 0.001;
            Momentum = 0.9;
            Epochs = 50;
            BatchSize = 100;

            if (IsDesignTime)
                HasLearned = true;

            Sets = new DataType[] { DataType.Testing, DataType.Training };
            CurrentSet = DataType.Training;

            PropertyChanged += LearnViewModel_PropertyChanged;
        }

        private void LearnViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentSet))
                UpdateDisplay();
        }

        public void OpenDatabase()
        {
            IsDataLoading = true;
            if (!IsDesignTime)
                Main.Database.Load();

            IsDataLoading = false;
            HasDataLoaded = true;
            UpdateDisplay();
        }

        public void Start()
        {
            if (!CanStart) return;

            _shouldStop = false;

            IsLearning = true;
            IsStarting = true;
            //shouldStop = false;

            //ErrorPoints.Points.Clear();
            CurrentEpoch = 0;
            CurrentEpochError = 0;

            //if (ShouldLearnEntireNetwork)
            LearnNetworkSupervised();

            //else if (ShouldLayerBeSupervised)
            //    learnLayerSupervised();

            //else learnLayerUnsupervised();
        }

        private void UpdateDisplay()
        {
            if (HasDataLoaded)
            {
                IEnumerable<Sample> dataset = CurrentSet == DataType.Training ? Main.Database.Training : Main.Database.Testing;
                Display = dataset;
            }
        }

        private void UpdateError(int epoch, double error)
        {
            IsStarting = false;
            CurrentEpoch = epoch;
            CurrentEpochError = error;
            //ErrorPoints.Points.Add(error);
            HasLearned = true;
        }

        private void LearnNetworkSupervised()
        {
            if (!Main.CanClassify) return;
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            new Task(() =>
            {
                var teacher = new BackPropagationLearning(Main.Network)
                {
                    LearningRate = LearningRate,
                    Momentum = Momentum
                };

                double[][] inputs, outputs;
                Main.Database.Training.GetInstances(Main.Database.Classes, out inputs, out outputs);

                // Start running the learning procedure
                for (int i = 0; i < Epochs && !_shouldStop; i++)
                {
                    double error = teacher.RunEpoch(inputs, outputs);

                    dispatcher.BeginInvoke((Action<int, double>)UpdateError, DispatcherPriority.ContextIdle, i + 1, error);
                }

                Main.Network.UpdateVisibleWeights();
                IsLearning = false;

            }).Start();
        }
    }
}
