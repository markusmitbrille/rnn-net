using Autrage.LEX.NET.Serialization;

namespace Autrage.RNN.NET
{
    [DataContract]
    public sealed class NeuralNetwork
    {
        [DataMember]
        private Genome genome;

        [DataMember]
        private Phenotype phenotype;

        public NeuralNetwork(int complexity, int size, int sensors, int muscles, int order, int connectivity, int sensitivity, int proactivity)
        {
            genome = new Genome(complexity, size, sensors, muscles, order, connectivity, sensitivity, proactivity);
            phenotype = genome.Phenotype(this);
        }

        public NeuralNetwork(NeuralNetwork other, double fidelity = 1)
        {
            genome = new Genome(other.genome, fidelity);
            phenotype = genome.Phenotype(this);
        }

        private NeuralNetwork()
        {
        }

        public void Pulse() => phenotype.Pulse();
    }
}