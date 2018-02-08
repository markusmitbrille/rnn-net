using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Autrage.RNN.NET
{
    internal class StimulandLayer : Collection<IStimuland>, INeuralLayer
    {
        #region Fields

        private int current;

        #endregion Fields

        #region Events

        public event EventHandler Completed;

        #endregion Events

        #region Constructors

        public StimulandLayer()
        {
        }

        public StimulandLayer(IList<IStimuland> list) : base(list)
        {
        }

        public StimulandLayer(IEnumerable<IStimuland> enumerable) : base(enumerable.ToList())
        {
        }

        #endregion Constructors

        #region Methods

        public void Pulse()
        {
            if (Count == 0) return;
            if (current < Count)
            {
                this[current].Stimulate();
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