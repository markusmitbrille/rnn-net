using System;
using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    class NeuralNetwork : INeuralNetwork
    {
        public IList<IStimulator> Stimulators { get; } = new List<IStimulator>();
        public IList<IStimuland> Stimulands { get; } = new List<IStimuland>();
        public IList<INeuralLayer> Layers { get; } = new List<INeuralLayer>();
        public IList<INeuron> Nodes { get; } = new List<INeuron>();

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

        public void InferLayers()
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
    }
}
