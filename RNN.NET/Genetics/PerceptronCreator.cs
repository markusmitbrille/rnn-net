using Autrage.LEX.NET;
using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System;
using System.IO;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class PerceptronCreator : Gene
    {
        #region Fields

        [DataMember]
        private double bias = Singleton<Random>.Instance.NextDouble();

        #endregion Fields

        #region Methods

        public override void ApplyTo(NetworkSkeleton network) => network.Nodes.Add(new Perceptron() { Bias = bias });

        public override void Mutate() => bias = Singleton<Random>.Instance.NextDouble();

        public override Gene Replicate() => new PerceptronCreator() { bias = bias };

        #endregion Methods

        #region Classes

        internal class Serializer : ReferenceTypeSerializer
        {
            #region Methods

            public override bool CanHandle(Type type) => typeof(PerceptronCreator).IsAssignableFrom(type);

            protected override bool SerializePayload(Stream stream, object instance)
            {
                PerceptronCreator gene = (PerceptronCreator)instance;

                stream.Write(gene.bias);

                return true;
            }

            protected override bool DeserializePayload(Stream stream, object instance)
            {
                PerceptronCreator gene = (PerceptronCreator)instance;

                if (stream.ReadDouble() is double bias)
                {
                    gene.bias = bias;
                }
                else
                {
                    Warning("Could not read perceptron creator bias!");
                    return false;
                }

                return true;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}