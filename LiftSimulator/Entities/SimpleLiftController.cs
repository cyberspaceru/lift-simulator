using System;

namespace LiftSimulator.Entities
{
    public class SimpleLiftController : LiftController
    {
        protected override void Start()
        {
            Console.WriteLine("Base Lift Controller has started.");
        }

        /// <summary>
        /// Called each time when we don't need to do the recalculation.
        /// </summary>
        protected override void Loop()
        {
            if (Commands.Count != 0)
            {
                Commands.Dequeue().Invoke();
            }
        }

        /// <summary>
        /// If the lift has received some actions from a user then commands need to be recalculated.
        /// </summary>
        protected override void Recalculate()
        {
            var stops = Stops;
            var cursor = Lift.CurrentLevel;
            // Setting "Open Door" command at first if needed.
            if (stops.Contains(cursor))
            {
                stops.Remove(cursor);
                Commands.Enqueue(Chipset.OpenDoor);
            }
            while (stops.Count != 0)
            {
                var level = stops[0];
                var negative = level < cursor;
                while (cursor != level)
                {
                    cursor += negative ? -1 : 1;
                    // Move the lift to down or up.
                    if (negative) Commands.Enqueue(Chipset.MoveDown);
                    else Commands.Enqueue(Chipset.MoveUp);
                    if (stops.Contains(cursor))
                    {
                        stops.Remove(cursor);
                        Commands.Enqueue(Chipset.OpenDoor);
                    }
                }
            }
        }

        protected override void End()
        {
            Console.WriteLine("Base Lift Controller has stopped.");
        }
    }
}
