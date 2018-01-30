﻿namespace Autrage.RNN.NET
{
    internal class Synapse : ISynapse
    {
        #region Constructors

        public Synapse(IStimulator stimulator)
        {
            Stimulator = stimulator;
        }

        #endregion Constructors

        #region Properties

        public double Signal => Weight * Stimulator?.State ?? 0;
        public IStimulator Stimulator { get; }
        public double Weight { get; set; }

        #endregion Properties
    }
}