using Autrage.LEX.NET.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class Phenotype : IEnumerable<INeuralLayer>, IEnumerable
    {
        #region Fields

        [DataMember]
        private IList<INeuralLayer> layers;

        [DataMember]
        private int currentLayer;

        #endregion Fields

        #region Constructors

        public Phenotype(IList<INeuralLayer> layers)
        {
            this.layers = layers ?? throw new ArgumentNullException(nameof(layers));
            AddLayerListeners();
        }

        #endregion Constructors

        #region Methods

        public IEnumerator<INeuralLayer> GetEnumerator()
        {
            return layers.GetEnumerator();
        }

        public void Pulse()
        {
            if (layers.Count == 0) return;
            if (currentLayer < layers.Count)
            {
                layers[currentLayer].Pulse();
            }
            else
            {
                currentLayer = 0;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return layers.GetEnumerator();
        }

        private void AddLayerListeners()
        {
            foreach (INeuralLayer layer in layers)
            {
                layer.Completed += OnLayerCompleted;
            }
        }

        private void OnLayerCompleted(object sender, EventArgs e) => currentLayer = ++currentLayer % layers.Count;

        #endregion Methods

        #region Classes

        internal class Serializer : ReferenceTypeSerializer
        {
            #region Methods

            public override bool CanHandle(Type type) => typeof(Phenotype).IsAssignableFrom(type);

            protected override bool SerializePayload(Stream stream, object instance)
            {
                Phenotype phenotype = (Phenotype)instance;
                Marshaller.Serialize(stream, phenotype.layers);
                Marshaller.Serialize(stream, phenotype.currentLayer);
                return true;
            }

            protected override bool DeserializePayload(Stream stream, object instance)
            {
                Phenotype phenotype = (Phenotype)instance;
                phenotype.layers = Marshaller.Deserialize<IList<INeuralLayer>>(stream);
                phenotype.currentLayer = Marshaller.Deserialize<int>(stream);
                phenotype.AddLayerListeners();
                return true;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}