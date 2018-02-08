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
    public abstract class Muscle : IStimuland
    {
        #region Properties

        [DataMember]
        public IList<ISynapse> Synapses { get; private set; } = new List<ISynapse>();

        #endregion Properties

        #region Methods

        public void Stimulate() => Move(Synapses.Sum(synapse => synapse.Signal));

        protected abstract void Move(double stimulus);

        #endregion Methods

        #region Classes

        internal class Serializer : ReferenceTypeSerializer
        {
            #region Methods

            public override bool CanHandle(Type type) => typeof(Muscle).IsAssignableFrom(type);

            protected override bool SerializePayload(Stream stream, object instance)
            {
                Muscle muscle = (Muscle)instance;

                stream.Write(muscle.Synapses.Count);
                foreach (ISynapse synapse in muscle.Synapses)
                {
                    Marshaller.Serialize(stream, synapse);
                }

                return true;
            }

            protected override bool DeserializePayload(Stream stream, object instance)
            {
                Muscle muscle = (Muscle)instance;

                if (stream.ReadInt() is int synapseCount)
                {
                    muscle.Synapses = new List<ISynapse>();
                    for (int i = 0; i < synapseCount; i++)
                    {
                        muscle.Synapses.Add(Marshaller.Deserialize<ISynapse>(stream));
                    }
                }
                else
                {
                    Warning($"Could not read muscle synapse count!");
                    return false;
                }

                return true;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}