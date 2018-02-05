﻿using Autrage.LEX.NET.Serialization;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class Synapse : ISynapse
    {
        #region Properties

        public double Signal => Weight * Stimulator?.State ?? 0;

        [DataMember]
        public IStimulator Stimulator { get; }

        [DataMember]
        public double Weight { get; set; }

        #endregion Properties

        #region Constructors

        public Synapse(IStimulator stimulator)
        {
            Stimulator = stimulator;
        }

        private Synapse()
        {
        }

        #endregion Constructors
    }
}