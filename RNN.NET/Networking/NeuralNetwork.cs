using Autrage.LEX.NET.Serialization;
using System;
using System.Linq;

namespace Autrage.RNN.NET
{
    [DataContract]
    public sealed class NeuralNetwork
    {
        #region Fields

        [DataMember]
        private Genome genome;

        [DataMember]
        private Phenotype phenotype;

        #endregion Fields

        #region Constructors

        internal NeuralNetwork(Genome genome, Phenotype phenotype)
        {
            this.genome = genome ?? throw new ArgumentNullException(nameof(genome));
            this.phenotype = phenotype ?? throw new ArgumentNullException(nameof(phenotype));

            SetBackReferences();
        }

        private NeuralNetwork()
        {
        }

        #endregion Constructors

        #region Methods

        public static NeuralNetwork Create(int complexity) => new Genome(complexity).Instantiate();

        public NeuralNetwork Replicate() => new Genome(genome).Instantiate();

        public NeuralNetwork Replicate(double mutationChance = 0, double complexificationChance = 0, double simplificationChance = 0, int maxMutations = 0, int maxComplexifications = 0, int maxSimplifications = 0)
            => new Genome(genome, mutationChance, complexificationChance, simplificationChance, maxMutations, maxComplexifications, maxSimplifications).Instantiate();

        public void Pulse() => phenotype.Pulse();

        private void SetBackReferences()
        {
            foreach (INeuralLayer layer in phenotype)
            {
                foreach (Muscle muscle in layer.OfType<Muscle>())
                {
                    muscle.Network = this;
                }
                foreach (Sensor sensor in layer.OfType<Sensor>())
                {
                    sensor.Network = this;
                }
            }
        }

        #endregion Methods
    }
}