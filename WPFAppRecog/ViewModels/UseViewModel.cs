using Accord.Neuro.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WPFAppRecog.Databases;
using WPFAppRecog.Extensions;
using Accord.Math;
using System.Windows.Input;

namespace WPFAppRecog.ViewModels
{
    public class UseViewModel : ViewModelBase
    {
        private double[] _userInput;
        public double[] UserInput { get { return _userInput; } set { Set(ref _userInput, value); } }

        public BitmapImage NetworkOutput { get; set; }

        private bool _isActive;
        public bool IsActive { get { return _isActive; } set { Set(ref _isActive, value); } }

        private int _classification;
        public int Classification { get { return _classification; } set { Set(ref _classification, value); } }

        public bool CanCompute { get { return UserInput != null && _mainViewModel.CanGenerate; } }

        public ICommand ClearUserInputCommand { get; private set; }

        private readonly MainViewModel _mainViewModel;

        public UseViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            IsActive = true;
            PropertyChanged += UseViewModel_PropertyChanged;
        }

        private void UseViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UserInput) && _mainViewModel.CanClassify)
                Compute();
        }

        public void Compute()
        {
            if (!CanCompute) return;

            double[] input = UserInput;
            DeepBeliefNetwork network = _mainViewModel.Network;
            IDatabase database = _mainViewModel.Database;

            database.Normalize(input);

            {
                double[] output = network.GenerateOutput(input);
                double[] reconstruction = network.Reconstruct(output);
                NetworkOutput = database.ToBitmap(reconstruction).ToBitmapImage();
            }

            if (_mainViewModel.CanClassify)
            {
                double[] output = network.Compute(input);
                //output.Max(int out imax);
                //Classification = imax;

                int imax; output.Max(out imax);
                Classification = imax;

            }
        }

        public void ClearUserInputHandler()
        {
            UserInput = new double[0];
        }
    }
}
