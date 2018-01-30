namespace Autrage.RNN.NET
{
    internal class Perceptron : Neuron
    {
        #region Methods

        protected override double ActivationFunction(double stimulus) => stimulus > 0 ? 1 : -1;

        #endregion Methods
    }
}