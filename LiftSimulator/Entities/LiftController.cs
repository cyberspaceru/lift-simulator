using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Exception = System.Exception;

namespace LiftSimulator.Entities
{
    public abstract class LiftController
    {
        private CancellationToken _mToken;
        private Dictionary<int, DateTime> _mStops;

        protected Queue<Action> Commands { get; private set; }
        protected Lift Lift { get; private set; }
        protected Lift.IChipset Chipset { get; private set; }
        
        protected List<int> Stops => (from pair in _mStops orderby pair.Value select pair.Key).ToList();

        private bool _mRecalculateRequired;

        private void Initilaze(CancellationToken token, Lift lift, Lift.IChipset chipset)
        {
            this.Commands = new Queue<Action>();
            this.Lift = lift;
            this.Chipset = chipset;

            this._mToken = token;
            this._mRecalculateRequired = false;
            this._mStops = new Dictionary<int, DateTime>();
        }

        public void Controll(CancellationToken token, Lift lift, Lift.IChipset chipset)
        {
            Initilaze(token, lift, chipset);
            DefineEventHandlers();
            Start();
            while (true)
            {
                if (this._mToken.IsCancellationRequested)
                {
                    End();
                    return;
                }
                if (_mRecalculateRequired)
                {
                    Commands.Clear();
                    Recalculate();
                    _mRecalculateRequired = false;
                }
                else
                {
                    Loop();
                }
            }
        }

        protected abstract void Start();
        protected abstract void Loop();
        protected abstract void Recalculate();
        protected abstract void End();

        private void DefineEventHandlers()
        {
            Lift.OnCallEvent += AddStop;
            Lift.OnDoorClosedEvent += (x) => _mStops.Remove(x);
        }

        private void AddStop(int level)
        {
            if (!_mStops.ContainsKey(level))
            {
                _mStops.Add(level, DateTime.Now);
                _mRecalculateRequired = true;
            }
        }
    }
}
