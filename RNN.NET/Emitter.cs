using System;

namespace Autrage.RNN.NET
{
    public sealed class Emitter : IStimulator
    {
        private double state;
        public double State => state;

        private Func<double> Fetch { get; }

        public Emitter(Func<double> fetch) => Fetch = fetch;

        public void Activate() => state = Fetch?.Invoke() ?? 0;
    }
}
