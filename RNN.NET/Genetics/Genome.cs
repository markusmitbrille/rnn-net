using Autrage.LEX.NET;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Autrage.RNN.NET
{
    internal class Genome : Collection<Gene>
    {
        #region Constructors

        public Genome(int complexity)
        {
            for (int i = 0; i < complexity; i++)
            {
                Add(Gene.Next());
            }
        }

        public Genome(Genome other)
        {
            foreach (Gene gene in other)
            {
                Add(gene.Replicate());
            }
        }

        public Genome(Genome other,
            double mutationChance = 0,
            double complexificationChance = 0,
            double simplificationChance = 0,
            int maxMutations = 0,
            int maxComplexifications = 0,
            int maxSimplifications = 0)
        {
            foreach (Gene gene in other)
            {
                Add(gene.Replicate());
            }

            for (int i = 0; i < maxMutations; i++)
                if (Singleton<Random>.Instance.NextDouble() < mutationChance)
                    Mutate();
            for (int i = 0; i < maxComplexifications; i++)
                if (Singleton<Random>.Instance.NextDouble() < complexificationChance)
                    Complexify();
            for (int i = 0; i < maxSimplifications; i++)
                if (Singleton<Random>.Instance.NextDouble() < simplificationChance)
                    Simplify();
        }

        private Genome()
        {
        }

        #endregion Constructors

        #region Methods

        public NeuralNetwork Instantiate()
        {
            NetworkSkeleton skeleton = new NetworkSkeleton();
            foreach (Gene gene in this)
            {
                gene.ApplyTo(skeleton);
            }

            return new NeuralNetwork(this, skeleton.ToPhenotype());
        }

        private void Complexify() => Add(Gene.Next());

        private void Mutate() => this[Singleton<Random>.Instance.Next(Count)].Mutate();

        private void Simplify() => Remove(this.Last());

        #endregion Methods
    }
}