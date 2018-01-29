using Autrage.LEX.NET;
using System;

namespace Autrage.RNN.NET
{
    abstract class NeuralNetworkGene : IGene<NeuralNetwork>
    {
        private class Dud : NeuralNetworkGene
        {
            public override IGene<NeuralNetwork> Replicate() => new Dud();

            public override void Mutate()
            {
            }

            public override void Phenotype(NeuralNetwork instance)
            {
            }
        }

        private class PerceptronCreator : NeuralNetworkGene
        {
            private double bias = Singleton<Random>.Instance.NextDouble();

            public override IGene<NeuralNetwork> Replicate() => new PerceptronCreator() { bias = bias };
            public override void Mutate() => bias = Singleton<Random>.Instance.NextDouble();
            public override void Phenotype(NeuralNetwork instance) => instance.Nodes.Add(new Perceptron() { Bias = bias });
        }

        private class SigmonCreator : NeuralNetworkGene
        {
            private double bias = Singleton<Random>.Instance.NextDouble();

            public override IGene<NeuralNetwork> Replicate() => new SigmonCreator() { bias = bias };
            public override void Mutate() => bias = Singleton<Random>.Instance.NextDouble();
            public override void Phenotype(NeuralNetwork instance) => instance.Nodes.Add(new Sigmon() { Bias = bias });
        }

        private class NodeLinker : NeuralNetworkGene
        {
            private int stimulator = Singleton<Random>.Instance.Next();
            private int stimuland = Singleton<Random>.Instance.Next();
            private double weight = Singleton<Random>.Instance.NextDouble();

            public override IGene<NeuralNetwork> Replicate() => new NodeLinker() { stimulator = stimulator, stimuland = stimuland, weight = weight };
            public override void Mutate()
            {
                stimulator = Singleton<Random>.Instance.Next();
                stimuland = Singleton<Random>.Instance.Next();
                weight = Singleton<Random>.Instance.NextDouble();
            }

            public override void Phenotype(NeuralNetwork instance)
                => instance.Nodes[stimuland % instance.Nodes.Count].Synapses.Add(new Synapse(instance.Nodes[stimulator % instance.Nodes.Count]) { Weight = weight });
        }

        private class InLinker : NeuralNetworkGene
        {
            private int stimulator = Singleton<Random>.Instance.Next();
            private int stimuland = Singleton<Random>.Instance.Next();
            private double weight = Singleton<Random>.Instance.NextDouble();

            public override IGene<NeuralNetwork> Replicate() => new InLinker() { stimulator = stimulator, stimuland = stimuland, weight = weight };
            public override void Mutate()
            {
                stimulator = Singleton<Random>.Instance.Next();
                stimuland = Singleton<Random>.Instance.Next();
                weight = Singleton<Random>.Instance.NextDouble();
            }

            public override void Phenotype(NeuralNetwork instance)
                => instance.Nodes[stimuland % instance.Nodes.Count].Synapses.Add(new Synapse(instance.Stimulators[stimulator % instance.Stimulators.Count]) { Weight = weight });
        }

        private class OutLinker : NeuralNetworkGene
        {
            private int stimulator = Singleton<Random>.Instance.Next();
            private int stimuland = Singleton<Random>.Instance.Next();
            private double weight = Singleton<Random>.Instance.NextDouble();

            public override IGene<NeuralNetwork> Replicate() => new OutLinker() { stimulator = stimulator, stimuland = stimuland, weight = weight };
            public override void Mutate()
            {
                stimulator = Singleton<Random>.Instance.Next();
                stimuland = Singleton<Random>.Instance.Next();
                weight = Singleton<Random>.Instance.NextDouble();
            }

            public override void Phenotype(NeuralNetwork instance)
                => instance.Stimulands[stimuland % instance.Stimulands.Count].Synapses.Add(new Synapse(instance.Nodes[stimulator % instance.Nodes.Count]) { Weight = weight });
        }

        public abstract IGene<NeuralNetwork> Replicate();
        public abstract void Mutate();
        public abstract void Phenotype(NeuralNetwork instance);

        private const int GeneTypeCount = 5;
        public static NeuralNetworkGene Next()
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
}
