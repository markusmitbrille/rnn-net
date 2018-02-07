using Autrage.LEX.NET.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    public class Simulation : List<NeuralNetwork>
    {
        #region Fields

        public Func<NeuralNetwork, double> Fitness;
        private double cutoffPercentile;

        #endregion Fields

        #region Properties

        public int Order { get; set; }
        public int Complexity { get; set; }

        public double CutoffPercentile
        {
            get => cutoffPercentile;
            set => cutoffPercentile = value.Clamp01();
        }

        public int CutoffCount => (int)(Count * cutoffPercentile);

        #endregion Properties

        #region Methods

        public void Genesis()
        {
            for (int i = Count; i < Order; i++)
            {
                Add(new NeuralNetwork(Complexity));
            }
        }

        public void Armageddon()
        {
            if (Fitness == null)
            {
                return;
            }

            foreach (NeuralNetwork network in this.OrderBy(Fitness).Take(CutoffCount).ToList())
            {
                Remove(network);
            }
        }

        public void Pulse()
        {
            foreach (NeuralNetwork network in this)
            {
                network.Pulse();
            }
        }

        #endregion Methods
    }
}