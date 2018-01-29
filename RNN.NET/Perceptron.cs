namespace Autrage.RNN.NET
{
    class Perceptron : Neuron
    {
        protected override double ActivationFunction(double stimulus) => stimulus > 0 ? 1 : -1;
    }
}
