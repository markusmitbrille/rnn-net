using Autrage.LEX.NET;
using Autrage.LEX.NET.Serialization;
using System;

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
    }
}