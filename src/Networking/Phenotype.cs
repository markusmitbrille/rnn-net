using Autrage.LEX.NET;
using Autrage.LEX.NET.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class Phenotype : IEnumerable<INeuralLayer>, IEnumerable
    {
        [DataMember]
        private IList<INeuralLayer> layers;

        [DataMember]
        private int currentLayer;

        public Phenotype(IList<INeuralLayer> layers)
        {
            layers.AssertNotNull();

            this.layers = layers;
            AddLayerListeners();
        }

        private Phenotype()
        {
        }

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
    }
}