using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    internal class NetworkSkeleton
    {
        #region Properties

        public List<Neuron> Neurons { get; } = new List<Neuron>();
        public List<Sensor> Stimulators { get; } = new List<Sensor>();
        public List<Muscle> Stimulands { get; } = new List<Muscle>();

        #endregion Properties

        #region Methods

        public Phenotype ToPhenotype()
        {
            IList<INeuralLayer> layers = new List<INeuralLayer>();
            layers.Insert(0, new StimulandLayer(Stimulands));

            List<Neuron> leftoverNodes = Neurons.ToList();
            List<Neuron> nextLayer =
                (from node in Neurons
                 from stimuland in Stimulands
                 from synapse in stimuland.Synapses
                 where node == synapse.Stimulator
                 select node)
                .ToList();

            while (nextLayer.Count > 0)
            {
                layers.Insert(0, new StimulatorLayer(nextLayer));
                layers.Insert(0, new StimulandLayer(nextLayer));

                leftoverNodes = leftoverNodes.Except(nextLayer).ToList();
                nextLayer =
                    (from node in Neurons
                     from stimuland in Stimulands
                     from synapse in stimuland.Synapses
                     where node == synapse.Stimulator
                     select node)
                    .ToList();
            }

            layers.Insert(0, new StimulatorLayer(Stimulators));

            return new Phenotype(layers);
        }

        #endregion Methods
    }
}