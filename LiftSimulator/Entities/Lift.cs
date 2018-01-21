using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftSimulator.Entities
{
    public class Lift
    {
        private CancellationTokenSource _mTaskControllCancellationToken;

        #region Properties
        public Building Building { get; }
        public double LiftSpeed { get; }
        public double DoorTime { get; }
        public int CurrentLevel { get; private set; }
        #endregion

        #region Events
        public event Action<int> OnCallEvent = i => { };
        public event Action<int> OnArrivedEvent = i => { };
        public event Action<int> OnDoorOpenedEvent = i => { };
        public event Action<int> OnDoorClosedEvent = i => { };
        #endregion

        public Lift(Building building, double liftSpeed, double doorTime)
        {
            this.Building = building;
            this.LiftSpeed = liftSpeed;
            this.DoorTime = doorTime;
            this.CurrentLevel = 1;
        }

        public void ShutDown()
        {
            _mTaskControllCancellationToken?.Cancel();
            _mTaskControllCancellationToken?.Dispose();
        }

        public void Reset(LiftController controller)
        {
            ShutDown();
            _mTaskControllCancellationToken = new CancellationTokenSource();
            Task.Run(() => controller.Controll(_mTaskControllCancellationToken.Token, this, new Chipset(this)));
        }

        public void CallTo(int level)
        {
            if (level > 0 && level <= Building.MaxLevel)
            {
                OnCallEvent.Invoke(level);
            }
        }

        // Gives access to the lift controlling.
        public interface IChipset
        {
            void OpenDoor();
            void MoveUp();
            void MoveDown();
        }

        public override string ToString()
        {
            return $"{nameof(CurrentLevel)}: {CurrentLevel}";
        }

        private class Chipset : IChipset
        {
            private readonly Lift _mLift;
            private readonly int _mTimePerLevel;
            private const int TimeToOpenDoor = 1000;
            private const int TimeToCloseDoor = 1000;

            public Chipset(Lift lift)
            {
                this._mLift = lift;
                this._mTimePerLevel = (int)(_mLift.Building.LevelHeight / _mLift.LiftSpeed * 1000);
            }

            public void OpenDoor()
            {
                Thread.Sleep(TimeToOpenDoor);
                _mLift.OnDoorOpenedEvent.Invoke(_mLift.CurrentLevel);
                Thread.Sleep((int)(_mLift.DoorTime * 1000));
                Thread.Sleep(TimeToCloseDoor);
                _mLift.OnDoorClosedEvent.Invoke(_mLift.CurrentLevel);
            }

            public void MoveUp()
            {
                if (_mLift.CurrentLevel < _mLift.Building.MaxLevel)
                {
                    Thread.Sleep(_mTimePerLevel);
                    _mLift.CurrentLevel++;
                    _mLift.OnArrivedEvent.Invoke(_mLift.CurrentLevel);
                }
            }

            public void MoveDown()
            {
                if (_mLift.CurrentLevel > 1)
                {
                    Thread.Sleep(_mTimePerLevel);
                    _mLift.CurrentLevel--;
                    _mLift.OnArrivedEvent.Invoke(_mLift.CurrentLevel);
                }
            }
        }
    }
}
