using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System;
using System.IO;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class Synapse : ISynapse
    {
        #region Properties

        public double Signal => Weight * Stimulator?.State ?? 0;

        [DataMember]
        public IStimulator Stimulator { get; private set; }

        [DataMember]
        public double Weight { get; set; }

        #endregion Properties

        #region Constructors

        public Synapse(IStimulator stimulator)
        {
            Stimulator = stimulator;
        }

        private Synapse()
        {
        }

        #endregion Constructors

        #region Classes

        internal class Serializer : ReferenceTypeSerializer
        {
            #region Methods

            public override bool CanHandle(Type type) => typeof(Synapse).IsAssignableFrom(type);

            protected override bool SerializePayload(Stream stream, object instance)
            {
                Synapse synapse = (Synapse)instance;

                Marshaller.Serialize(stream, synapse.Stimulator);

                stream.Write(synapse.Weight);

                return true;
            }

            protected override bool DeserializePayload(Stream stream, object instance)
            {
                Synapse synapse = (Synapse)instance;

                synapse.Stimulator = Marshaller.Deserialize<IStimulator>(stream);

                if (stream.ReadDouble() is double weight)
                {
                    synapse.Weight = weight;
                }
                else
                {
                    Warning("Could not read synapse weight!");
                    return false;
                }

                return true;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}