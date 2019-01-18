using Accord.Neuro;
using Accord.Neuro.ActivationFunctions;
using Accord.Neuro.Networks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFAppRecog.Commands;
using WPFAppRecog.Databases;

namespace WPFAppRecog.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties
        private DeepBeliefNetwork _network;
        public DeepBeliefNetwork Network { get { return _network; } private set { Set(ref _network, value); } }

        private IDatabase _database;
        public IDatabase Database { get { return _database; } private set { Set(ref _database, value); } }

        private readonly IStochasticFunction _activationFunction = new BernoulliFunction();

        private int _newLayerNeurons;
        public int NewLayerNeurons { get { return _newLayerNeurons; } set { Set(ref _newLayerNeurons, value); } }

        public bool CanClassify { get { return Network != null && Database != null && Network.OutputCount == Database.Classes; } }
        public bool CanGenerate { get { return Network != null && Network.Layers.Length > 0; } }
        #endregion Properties

        #region ViewModels
        public LearnViewModel Learn { get; private set; }

        public UseViewModel Use { get; private set; }
        #endregion ViewModels

        private readonly string _title = "page title sample";

        public string Title { get { return _title; } }

        public ICommand ComputeCommand { get; private set; }

        public ICommand ProcessCommand { get; private set; }

        public ICommand StartLearnCommand { get; private set; }

        public ICommand PauseLearnCommand { get; private set; }

        public ICommand ResetLearnCommand { get; private set; }

        public MainViewModel()
        {
            Network = new DeepBeliefNetwork(_activationFunction, 1024, 50, 10);

            Database = new OptdigitsDatabase()
            {
                IsNormalized = false
            };

            new GaussianWeights(Network).Randomize();
            Network.UpdateVisibleWeights();
            NewLayerNeurons = 10;

            ProcessCommand = new RelayCommand(LearnHandler, () => Learn.CanStart);
            StartLearnCommand = new RelayCommand(StartLearnHandler);
            PauseLearnCommand = new RelayCommand(PauseLearnHandler);
            ResetLearnCommand = new RelayCommand(ResetLearnHandler);
            ComputeCommand = new RelayCommand(ComputHandler);

            Use = new UseViewModel(this);
            Learn = new LearnViewModel(this);
            Learn.OpenDatabase();
        }

        public void Save(string filename)
        {
            Network.Save(filename);
        }

        public void Load(string filename)
        {
            Network = DeepBeliefNetwork.Load(filename);
        }

        public void StackNewLayer()
        {
            if (Database.IsNormalized && Network.Layers.Length == 0)
            {
                Network.Push(NewLayerNeurons, visibleFunction: new GaussianFunction(), hiddenFunction: new BernoulliFunction());
            }
            else Network.Push(NewLayerNeurons, new BernoulliFunction());

            FireEventChanged(nameof(Network));
        }

        public void RemoveLastLayer()
        {
            Network.Pop();
            FireEventChanged(nameof(Network));
        }

        #region Handlers
        private void LearnHandler()
        {
            if (Learn.CanStart)
                Learn.Start();
        }

        private void StartLearnHandler()
        {
            //Learn
        }

        private void PauseLearnHandler()
        {
            //Learn
        }

        private void ResetLearnHandler()
        {
            //Learn
        }

        private void ComputHandler()
        {
            //Learn.comp
        }
        #endregion Handlers
    }
}
