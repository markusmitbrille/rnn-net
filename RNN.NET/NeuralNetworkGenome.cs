using Autrage.LEX.NET;
using System;
using System.Linq;

namespace Autrage.RNN.NET
{
    class NeuralNetworkGenome : Genome<NeuralNetwork>
    {
        private NeuralNetworkGenome()
        {
        }

        public NeuralNetworkGenome(int complexity)
        {
            for (int i = 0; i < complexity; i++)
            {
                Genes.Add(NeuralNetworkGene.Next());
            }
        }

        public override IGenome<NeuralNetwork> Replicate()
        {
            NeuralNetworkGenome replica = new NeuralNetworkGenome();
            foreach (IGene<NeuralNetwork> gene in Genes)
            {
                replica.Genes.Add(gene.Replicate());
            }

            return replica;
        }

        public override void Phenotype(NeuralNetwork instance)
        {
            foreach (IGene<NeuralNetwork> gene in Genes)
            {
                gene.Phenotype(instance);
            }
        }

        public override void Mutate() => Genes[Singleton<Random>.Instance.Next(Genes.Count)].Mutate();
        public override void Complexify() => Genes.Add(NeuralNetworkGene.Next());
        public override void Simplify() => Genes.Remove(Genes.Last());
    }
}
