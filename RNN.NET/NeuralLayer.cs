﻿using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class NeuralLayer : INeuralLayer
    {
        #region Fields

        [DataMember]
        private IList<INeuron> neurons = new List<INeuron>();

        #endregion Fields

        #region Indexers

        public INeuron this[int index] { get => neurons[index]; set => neurons[index] = value ?? throw new ArgumentNullException(nameof(value)); }

        #endregion Indexers

        #region Properties

        public int Count => neurons.Count;

        public bool IsReadOnly => neurons.IsReadOnly;

        #endregion Properties

        #region Constructors

        public NeuralLayer()
        {
        }

        public NeuralLayer(IEnumerable<INeuron> neurons)
        {
            this.neurons = new List<INeuron>(neurons);
        }

        #endregion Constructors

        #region Methods

        public void Activate()
        {
            foreach (INeuron neuron in neurons)
            {
                neuron.Activate();
            }
        }

        public void Add(INeuron item) => neurons.Add(item ?? throw new ArgumentNullException(nameof(item)));

        public void Clear() => neurons.Clear();

        public bool Contains(INeuron item) => neurons.Contains(item);

        public bool Remove(INeuron item) => neurons.Remove(item);

        public void Stimulate()
        {
            foreach (INeuron neuron in neurons)
            {
                neuron.Stimulate();
            }
        }

        void ICollection<INeuron>.CopyTo(INeuron[] array, int arrayIndex) => neurons.CopyTo(array, arrayIndex);

        IEnumerator<INeuron> IEnumerable<INeuron>.GetEnumerator() => neurons.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => neurons.GetEnumerator();

        int IList<INeuron>.IndexOf(INeuron item) => neurons.IndexOf(item);

        void IList<INeuron>.Insert(int index, INeuron item) => neurons.Insert(index, item ?? throw new ArgumentNullException(nameof(item)));

        void IList<INeuron>.RemoveAt(int index) => neurons.RemoveAt(index);

        #endregion Methods

        #region Classes

        internal class Serializer : ReferenceTypeSerializer
        {
            #region Methods

            public override bool CanHandle(Type type)
            {
                return typeof(NeuralLayer).IsAssignableFrom(type);
            }

            protected override bool SerializePayload(Stream stream, object instance)
            {
                NeuralLayer layer = (NeuralLayer)instance;

                stream.Write(layer.neurons.Count);
                foreach (INeuron neuron in layer.neurons)
                {
                    Marshaller.Serialize(stream, neuron);
                }

                return true;
            }

            protected override bool DeserializePayload(Stream stream, object instance)
            {
                NeuralLayer layer = (NeuralLayer)instance;

                if (stream.ReadInt() is int neuronCount)
                {
                    layer.neurons = new List<INeuron>(neuronCount);
                    for (int i = 0; i < neuronCount; i++)
                    {
                        layer.neurons.Add(Marshaller.Deserialize<INeuron>(stream));
                    }
                }
                else
                {
                    Warning("Could not read genome gene count!");
                    return false;
                }

                return true;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}