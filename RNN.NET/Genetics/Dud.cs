using Autrage.LEX.NET.Serialization;
using System;
using System.IO;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class Dud : Gene
    {
        #region Methods

        public override void ApplyTo(NetworkSkeleton network)
        {
        }

        public override void Mutate()
        {
        }

        public override Gene Replicate() => new Dud();

        #endregion Methods

        #region Classes

        internal class Serializer : ReferenceTypeSerializer
        {
            #region Methods

            public override bool CanHandle(Type type) => typeof(Dud).IsAssignableFrom(type);

            protected override bool SerializePayload(Stream stream, object instance) => true;

            protected override bool DeserializePayload(Stream stream, object instance) => true;

            #endregion Methods
        }

        #endregion Classes
    }
}