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

        public static NeuralNetwork Mate(NeuralNetwork mother, NeuralNetwork father, double fidelity = 1)
        {
            NeuralNetwork child = new NeuralNetwork();
            Genome genome = Genome.Mate(mother.genome, father.genome, fidelity);
            Phenotype phenotype = genome.Phenotype(child);
            child.genome = genome;
            child.phenotype = genome.Phenotype(child);
            return child;
        }

        public void Pulse() => phenotype.Pulse();
    }
}