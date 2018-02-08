using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Autrage.RNN.NET
{
    internal class StimulatorLayer : Collection<IStimulator>, INeuralLayer
    {
        #region Fields

        private int current;

        #endregion Fields

        #region Events

        public event EventHandler Completed;

        #endregion Events

        #region Constructors

        public StimulatorLayer()
        {
        }

        public StimulatorLayer(IList<IStimulator> list) : base(list)
        {
        }

        public StimulatorLayer(IEnumerable<IStimulator> enumerable) : base(enumerable.ToList())
        {
        }

        #endregion Constructors

        #region Methods

        public void Pulse()
        {
            if (Count == 0) return;
            if (current < Count)
            {
                this[current].Activate();
            }
            else
            {
                current = 0;
                Completed?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion Methods
    }
}