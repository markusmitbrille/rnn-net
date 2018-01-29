using System.Collections;
using System.Collections.Generic;

namespace Autrage.RNN.NET
{
    public sealed class Simulation<T> : IEnumerable<T> where T : Intelligence, new()
    {
        private class ReferenceEqualityComparer : IEqualityComparer<T>
        {
            public bool Equals(T x, T y) => ReferenceEquals(x, y);

            public int GetHashCode(T obj) => obj?.GetHashCode() ?? 0;
        }

        private ISet<T> intelligences = new HashSet<T>(new ReferenceEqualityComparer());

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
                Generate(complexity);
            }
        }

        public bool Add(T intelligence) => intelligences.Add(intelligence);
        public bool Remove(T intelligence) => intelligences.Remove(intelligence);

        public void Generate(int complexity)
        {
            T intelligence = new T();
            intelligence.GenerateGenome(complexity);
            intelligence.Initialize();

            intelligences.Add(intelligence);
        }

        public void Replicate(T intelligence)
        {
            T replica = new T();
            replica.ReplicateGenome(intelligence,
                mutationChance,
                complexificationChance,
                simplificationChance,
                maxMutations,
                maxComplexifications,
                maxSimplifications);
            replica.Initialize();

            intelligences.Add(replica);
        }

        public void Pulse()
        {
            if (intelligences.Count == 0)
            {
                Genesis(order, complexity);
            }

            foreach (T intelligence in intelligences)
            {
                intelligence.Pulse();
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => intelligences.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => intelligences.GetEnumerator();
    }
}
