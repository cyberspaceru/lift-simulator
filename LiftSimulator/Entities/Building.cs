using System;

namespace LiftSimulator.Entities
{
    public class Building
    {
        public Lift Lift { get; private set; }
        public int MaxLevel { get; }
        public double LevelHeight { get; }

        public Building(int maxLevel, double levelHeight)
        {
            this.MaxLevel = maxLevel;
            this.LevelHeight = levelHeight;
        }

        public void SetLift(LiftController controller, double liftSpeed, double doorTime)
        {
            if (Lift != null)
            {
                throw new Exception("The lift already exists in this building.");
            }
            Lift = new Lift(this, liftSpeed, doorTime);
            Lift.Reset(controller);
        }
    }
}
