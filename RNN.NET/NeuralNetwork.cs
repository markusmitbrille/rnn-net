using Autrage.LEX.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    class NeuralNetwork
    {
        abstract class Gene
        {
            class Dud : Gene
            {
                public override Gene Replicate() => new Dud();

                public override void Mutate()
                {
                }

                public override void ApplyTo(NeuralNetwork instance)
                {
                }
            }

            class PerceptronCreator : Gene
            {
                private double bias = Singleton<Random>.Instance.NextDouble();

                public override Gene Replicate() => new PerceptronCreator() { bias = bias };
                public override void Mutate() => bias = Singleton<Random>.Instance.NextDouble();
                public override void ApplyTo(NeuralNetwork instance) => instance.Nodes.Add(new Perceptron() { Bias = bias });
            }

            class SigmonCreator : Gene
            {
                private double bias = Singleton<Random>.Instance.NextDouble();

                public override Gene Replicate() => new SigmonCreator() { bias = bias };
                public override void Mutate() => bias = Singleton<Random>.Instance.NextDouble();
                public override void ApplyTo(NeuralNetwork instance) => instance.Nodes.Add(new Sigmon() { Bias = bias });
            }

            class NodeLinker : Gene
            {
                private int stimulator = Singleton<Random>.Instance.Next();
                private int stimuland = Singleton<Random>.Instance.Next();
                private double weight = Singleton<Random>.Instance.NextDouble();

                public override Gene Replicate() => new NodeLinker() { stimulator = stimulator, stimuland = stimuland, weight = weight };
                public override void Mutate()
                {
                    stimulator = Singleton<Random>.Instance.Next();
                    stimuland = Singleton<Random>.Instance.Next();
                    weight = Singleton<Random>.Instance.NextDouble();
                }

                public override void ApplyTo(NeuralNetwork instance)
                    => instance.Nodes[stimuland % instance.Nodes.Count].Synapses.Add(new Synapse(instance.Nodes[stimulator % instance.Nodes.Count]) { Weight = weight });
            }

            class InLinker : Gene
            {
                private int stimulator = Singleton<Random>.Instance.Next();
                private int stimuland = Singleton<Random>.Instance.Next();
                private double weight = Singleton<Random>.Instance.NextDouble();

                public override Gene Replicate() => new InLinker() { stimulator = stimulator, stimuland = stimuland, weight = weight };
                public override void Mutate()
                {
                    stimulator = Singleton<Random>.Instance.Next();
                    stimuland = Singleton<Random>.Instance.Next();
                    weight = Singleton<Random>.Instance.NextDouble();
                }

                public override void ApplyTo(NeuralNetwork instance)
                    => instance.Nodes[stimuland % instance.Nodes.Count].Synapses.Add(new Synapse(instance.Stimulators[stimulator % instance.Stimulators.Count]) { Weight = weight });
            }

            class OutLinker : Gene
            {
                private int stimulator = Singleton<Random>.Instance.Next();
                private int stimuland = Singleton<Random>.Instance.Next();
                private double weight = Singleton<Random>.Instance.NextDouble();

                public override Gene Replicate() => new OutLinker() { stimulator = stimulator, stimuland = stimuland, weight = weight };
                public override void Mutate()
                {
                    stimulator = Singleton<Random>.Instance.Next();
                    stimuland = Singleton<Random>.Instance.Next();
                    weight = Singleton<Random>.Instance.NextDouble();
                }

                public override void ApplyTo(NeuralNetwork instance)
                    => instance.Stimulands[stimuland % instance.Stimulands.Count].Synapses.Add(new Synapse(instance.Nodes[stimulator % instance.Nodes.Count]) { Weight = weight });
            }

            public abstract Gene Replicate();
            public abstract void Mutate();
            public abstract void ApplyTo(NeuralNetwork instance);

            const int GeneTypeCount = 5;
            public static Gene Next()
            {
                switch (Singleton<Random>.Instance.Next(GeneTypeCount))
                {
                    case 0:
                        return new PerceptronCreator();
                    case 1:
                        return new SigmonCreator();
                    case 2:
                        return new NodeLinker();
                    case 3:
                        return new InLinker();
                    case 4:
                        return new OutLinker();
                    default:
                        return new Dud();
                }
            }
        }

        class Genome
        {
            IList<Gene> Genes { get; } = new List<Gene>();

            Genome()
            {
            }

            public Genome(int complexity)
            {
                for (int i = 0; i < complexity; i++)
                {
                    Genes.Add(Gene.Next());
                }
            }

            public Genome(Genome other)
            {
                foreach (Gene gene in other.Genes)
                {
                    Genes.Add(gene.Replicate());
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
                foreach (Gene gene in other.Genes)
                {
                    Genes.Add(gene.Replicate());
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

            public void ApplyTo(NeuralNetwork instance)
            {
                foreach (Gene gene in Genes)
                {
                    gene.ApplyTo(instance);
                }
            }

            void Mutate() => Genes[Singleton<Random>.Instance.Next(Genes.Count)].Mutate();
            void Complexify() => Genes.Add(Gene.Next());
            void Simplify() => Genes.Remove(Genes.Last());
        }

        Genome NetworkGenome { get; }

        IList<IStimulator> Stimulators { get; } = new List<IStimulator>();
        IList<IStimuland> Stimulands { get; } = new List<IStimuland>();
        IList<INeuralLayer> Layers { get; } = new List<INeuralLayer>();
        IList<INeuron> Nodes { get; } = new List<INeuron>();

        public NeuralNetwork(int complexity)
        {
            NetworkGenome = new Genome(complexity);
            NetworkGenome.ApplyTo(this);
            InferLayers();
        }

        public NeuralNetwork(NeuralNetwork other)
        {
            NetworkGenome = new Genome(other.NetworkGenome);
            NetworkGenome.ApplyTo(this);
            InferLayers();
        }

        public NeuralNetwork(NeuralNetwork other,
            double mutationChance = 0,
            double complexificationChance = 0,
            double simplificationChance = 0,
            int maxMutations = 0,
            int maxComplexifications = 0,
            int maxSimplifications = 0)
        {
            NetworkGenome = new Genome(other.NetworkGenome,
                mutationChance,
                complexificationChance,
                simplificationChance,
                maxMutations,
                maxComplexifications,
                maxSimplifications);

            NetworkGenome.ApplyTo(this);
            InferLayers();
        }

        void InferLayers()
        {
            Layers.Clear();

            IEnumerable<INeuron> nextLayer = Nodes.Where(node => node.Synapses.Any(synapse => Stimulators.Contains(synapse.Stimulator)));
            IEnumerable<INeuron> leftoverNodes = Nodes.Except(nextLayer);

            while (nextLayer.Count() > 0)
            {
                NeuralLayer layer = new NeuralLayer(nextLayer);
                Layers.Add(layer);

                nextLayer = leftoverNodes.Where(node => node.Synapses.Any(synapse => layer.Contains(synapse.Stimulator)));
                leftoverNodes = leftoverNodes.Except(layer);
            }
        }

        public void Pulse()
        {
            foreach (IStimulator stimulator in Stimulators)
            {
                stimulator.Activate();
            }

            foreach (INeuralLayer layer in Layers)
            {
                layer.Stimulate();
                layer.Activate();
            }

            foreach (IStimuland stimuland in Stimulands)
            {
                stimuland.Stimulate();
            }
        }
    }
}
