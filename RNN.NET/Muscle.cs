using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    public abstract class Muscle : IStimuland
    {
        #region Properties

        public IList<ISynapse> Synapses { get; } = new List<ISynapse>();

        #endregion Properties

        #region Constructors

        public Muscle()
        {
        }

        #endregion Constructors

        #region Methods

        public void Stimulate() => Move(Synapses.Sum(synapse => synapse.Signal));

        protected abstract void Move(double stimulus);

        #endregion Methods
    }
}