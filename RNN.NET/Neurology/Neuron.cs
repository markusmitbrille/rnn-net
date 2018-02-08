using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal abstract class Neuron : INeuron
    {
        #region Fields

        [DataMember]
        private double stimulus;

        #endregion Fields

        #region Properties

        [DataMember]
        public double State { get; private set; }

        [DataMember]
        public double Bias { get; set; }

        [DataMember]
        public IList<ISynapse> Synapses { get; private set; } = new List<ISynapse>();

        #endregion Properties

        #region Methods

        public void Activate() => State = ActivationFunction(stimulus);

        public void Stimulate() => stimulus = Bias + Synapses.Sum(synapse => synapse.Signal);

        protected abstract double ActivationFunction(double stimulus);

        #endregion Methods

        #region Classes

        internal class Serializer : ReferenceTypeSerializer
        {
            #region Methods

            public override bool CanHandle(Type type) => typeof(Neuron).IsAssignableFrom(type);

            protected override bool SerializePayload(Stream stream, object instance)
            {
                Neuron neuron = (Neuron)instance;

                stream.Write(neuron.stimulus);
                stream.Write(neuron.State);
                stream.Write(neuron.Bias);

                stream.Write(neuron.Synapses.Count);
                foreach (ISynapse synapse in neuron.Synapses)
                {
                    Marshaller.Serialize(stream, synapse);
                }

                return true;
            }

            protected override bool DeserializePayload(Stream stream, object instance)
            {
                Neuron neuron = (Neuron)instance;

                if (stream.ReadDouble() is double stimulus)
                {
                    neuron.stimulus = stimulus;
                }
                else
                {
                    Warning("Could not read neuron stimulus!");
                    return false;
                }

                if (stream.ReadDouble() is double state)
                {
                    neuron.State = state;
                }
                else
                {
                    Warning("Could not read neuron state!");
                    return false;
                }

                if (stream.ReadDouble() is double bias)
                {
                    neuron.Bias = bias;
                }
                else
                {
                    Warning("Could not read neuron bias!");
                    return false;
                }

                if (stream.ReadInt() is int synapseCount)
                {
                    DeserializeSynapses(stream, neuron, synapseCount);
                }
                else
                {
                    Warning("Could not read neuron synapse count!");
                    return false;
                }

                return true;
            }

            private void DeserializeSynapses(Stream stream, Neuron neuron, int synapseCount)
            {
                neuron.Synapses = new List<ISynapse>(synapseCount);
                for (int i = 0; i < synapseCount; i++)
                {
                    neuron.Synapses.Add(Marshaller.Deserialize<ISynapse>(stream));
                }
            }

            #endregion Methods
        }

        #endregion Classes
    }
}