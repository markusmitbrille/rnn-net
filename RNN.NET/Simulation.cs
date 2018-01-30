using System.Collections;
using System.Collections.Generic;

namespace Autrage.RNN.NET
{
    public sealed class Simulation
    {
        private class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public new bool Equals(object x, object y) => ReferenceEquals(x, y);

            public int GetHashCode(object obj) => obj?.GetHashCode() ?? 0;
        }

        private ISet<NeuralNetwork> brains = new HashSet<NeuralNetwork>(new ReferenceEqualityComparer());

        private int order;
        private int complexity;

        private double mutationChance;
        private double complexificationChance;
        private double simplificationChance;

        private int maxMutations;
        private int maxComplexifications;
        private int maxSimplifications;

        public Simulation(int order,
            int complexity,
            double mutationChance = 0.05,
            double complexificationChance = 0.02,
            double simplificationChance = 0.02,
            int maxMutations = 10,
            int maxComplexifications = 3,
            int maxSimplifications = 3)
        {
            this.order = order;
            this.complexity = complexity;

            this.mutationChance = mutationChance;
            this.complexificationChance = complexificationChance;
            this.simplificationChance = simplificationChance;

            this.maxMutations = maxMutations;
            this.maxComplexifications = maxComplexifications;
            this.maxSimplifications = maxSimplifications;

            Genesis(order, complexity);
        }

        private void Genesis(int order, int complexity)
        {
            for (int i = 0; i < order; i++)
            {
                brains.Add(new NeuralNetwork(complexity));
            }
        }

        public void Pulse()
        {
            if (brains.Count == 0)
            {
                Genesis(order, complexity);
            }

            foreach (NeuralNetwork brain in brains)
            {
                brain.Pulse();
            }
        }
    }
}
