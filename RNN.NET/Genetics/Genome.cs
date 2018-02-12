using Autrage.LEX.NET.Extensions;
using System.Collections.ObjectModel;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.RNN.NET
{
    internal class Genome : Collection<Chromosome>
    {
        public Genome(int complexity, int size, int sensors, int muscles, int order, int connectivity, int sensitivity, int proactivity)
        {
            for (int i = 0; i < complexity; i++)
            {
                Add(new Chromosome(Rnd.Int(size), Rnd.Int(sensors), Rnd.Int(muscles), Rnd.Int(order), Rnd.Int(connectivity), Rnd.Int(sensitivity), Rnd.Int(proactivity)));
            }
        }

        public Genome(Genome other, double fidelity = 1)
        {
            foreach (Chromosome chromosome in other)
            {
                Add(new Chromosome(chromosome, fidelity));
            }
        }

        private Genome()
        {
        }

        public static Genome Mate(Genome mother, Genome father, double fidelity = 1)
        {
            if (mother.Count != father.Count)
            {
                Log($"Could not mate genomes, due to their difference in complexity ({mother.Count}, {father.Count})!");
                return null;
            }

            Genome child = new Genome();
            for (int i = 0, complexity = mother.Count; i < complexity; i++)
            {
                if (Rnd.Choice())
                {
                    child.Add(new Chromosome(mother[i], fidelity));
                }
                else
                {
                    child.Add(new Chromosome(father[i], fidelity));
                }
            }

            return child;
        }

        public Phenotype Phenotype(NeuralNetwork network)
        {
            NetworkSkeleton skeleton = new NetworkSkeleton();
            foreach (Chromosome chromosome in this)
            {
                chromosome.Phenotype(network, skeleton);
            }

            return skeleton.ToPhenotype();
        }
    }
}