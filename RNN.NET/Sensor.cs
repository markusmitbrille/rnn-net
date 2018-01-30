using System;

namespace Autrage.RNN.NET
{
    public abstract class Sensor : IStimulator
    {
        public double State { get; private set; }

        public Sensor()
        {
        }

        public void Activate() => State = Fetch();

        protected abstract double Fetch();
    }
}
