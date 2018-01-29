using System;
using System.Collections;
using System.Collections.Generic;

namespace Autrage.RNN.NET
{
    class NeuralLayer : INeuralLayer
    {
        private IList<INeuron> neurons = new List<INeuron>();

        public INeuron this[int index] { get => neurons[index]; set => neurons[index] = value ?? throw new ArgumentNullException(nameof(value)); }

        public int Count => neurons.Count;
        public bool IsReadOnly => neurons.IsReadOnly;

        public NeuralLayer()
        {
        }

        public NeuralLayer(IEnumerable<INeuron> neurons)
        {
            this.neurons = new List<INeuron>(neurons);
        }

        public void Activate()
        {
            foreach (INeuron neuron in neurons)
            {
                neuron.Activate();
            }
        }

        public void Stimulate()
        {
            foreach (INeuron neuron in neurons)
            {
                neuron.Stimulate();
            }
        }
        public void Add(INeuron item) => neurons.Add(item ?? throw new ArgumentNullException(nameof(item)));
        void IList<INeuron>.Insert(int index, INeuron item) => neurons.Insert(index, item ?? throw new ArgumentNullException(nameof(item)));

        public bool Remove(INeuron item) => neurons.Remove(item);
        void IList<INeuron>.RemoveAt(int index) => neurons.RemoveAt(index);

        public void Clear() => neurons.Clear();

        public bool Contains(INeuron item) => neurons.Contains(item);

        void ICollection<INeuron>.CopyTo(INeuron[] array, int arrayIndex) => neurons.CopyTo(array, arrayIndex);

        int IList<INeuron>.IndexOf(INeuron item) => neurons.IndexOf(item);

        IEnumerator<INeuron> IEnumerable<INeuron>.GetEnumerator() => neurons.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => neurons.GetEnumerator();
    }
}
