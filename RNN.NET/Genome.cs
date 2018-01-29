using System.Collections;
using System.Collections.Generic;

namespace Autrage.RNN.NET
{
    abstract class Genome<T> : IGenome<T>
    {
        protected IList<IGene<T>> Genes { get; } = new List<IGene<T>>();

        public IEnumerator<IGene<T>> GetEnumerator() => Genes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Genes.GetEnumerator();

        public abstract IGenome<T> Replicate();
        public abstract void Phenotype(T instance);
        public abstract void Mutate();
        public abstract void Complexify();
        public abstract void Simplify();
    }
}
