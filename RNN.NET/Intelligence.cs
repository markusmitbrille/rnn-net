using Autrage.LEX.NET;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Autrage.RNN.NET
{
    public abstract class Intelligence
    {
        private IGenome<NeuralNetwork> Genome { get; set; }
        private NeuralNetwork Brain { get; set; }

        public void GenerateGenome(int complexity)
        {
            if (Genome != null) return;

            Genome = new NeuralNetworkGenome(complexity);
        }

        public void ReplicateGenome(Intelligence other,
            double mutationChance = 0,
            double complexificationChance = 0,
            double simplificationChance = 0,
            int maxMutations = 0,
            int maxComplexifications = 0,
            int maxSimplifications = 0)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (Genome != null) return;

            Genome = other.Genome.Replicate();

            for (int i = 0; i < maxMutations; i++)
                if (Singleton<Random>.Instance.NextDouble() < mutationChance)
                    Genome.Mutate();
            for (int i = 0; i < maxComplexifications; i++)
                if (Singleton<Random>.Instance.NextDouble() < complexificationChance)
                    Genome.Complexify();
            for (int i = 0; i < maxSimplifications; i++)
                if (Singleton<Random>.Instance.NextDouble() < simplificationChance)
                    Genome.Simplify();
        }

        public void Initialize()
        {
            if (Brain != null) return;

            Brain = new NeuralNetwork();

            InitializeStimulators();
            InitializeStimulands();

            Genome.Phenotype(Brain);

            Brain.InferLayers();
        }

        protected abstract void InitializeStimulators();
        protected abstract void InitializeStimulands();

        public void Pulse() => Brain?.Pulse();
    }
}
