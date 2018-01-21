using System;
using LiftSimulator.Bootstrapper;
using LiftSimulator.Entities;

namespace LiftSimulator
{
    internal class SimulatorLauncher
    {
        public const int MinLevels = 5;
        public const int MaxLevels = 20;

        private static readonly Parameter<int> MaxLevelParameter = new Parameter<int>("Max Level", int.Parse, l => l >= MinLevels && l <= MaxLevels);
        private static readonly Parameter<double> LevelHeightParameter = new Parameter<double>("Level Height (m)", double.Parse, h => h > 0);
        private static readonly Parameter<double> LiftSpeedParameter = new Parameter<double>("Lift Speed (m/s)", double.Parse, s => s > 0);
        private static readonly Parameter<double> DoorTimeParameter = new Parameter<double>("Door Time (s)", double.Parse, t => t > 0);

        private static void Main(string[] args)
        {
            args = new[] { "20", "2.5", "0.5", "3.4" };
            if (!InitializeParametersByArgs(args))
            {
                Console.ReadLine();
                return;
            }
            var building = CreateBuilding();
            while (true)
            {
                var commandCode = GetNextCode();
                if (commandCode != 0)
                {
                    if (commandCode == -1)
                    {
                        building.Lift.ShutDown();
                        Console.WriteLine("You exited from the simulator.");
                        break;
                    }
                    else if (commandCode > 0)
                    {
                        building.Lift.CallTo(commandCode);
                    }
                }
            }
            Console.ReadLine();
        }

        private static int GetNextCode()
        {
            try
            {
                return int.Parse(Console.ReadLine());
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Can't read the command code: {exception.Message}");
            }
            return 0;
        }

        private static Building CreateBuilding()
        {
            var building = new Building(MaxLevelParameter.Value, LevelHeightParameter.Value);
            building.SetLift(new SimpleLiftController(), LiftSpeedParameter.Value, DoorTimeParameter.Value);
            SubscribeLiftToLog(building.Lift);
            return building;
        }

        private static void SubscribeLiftToLog(Lift lift)
        {
            lift.OnCallEvent += x => Console.WriteLine($"{DateTime.Now} - [USER ACTION] The lift was called to {x}.");
            lift.OnArrivedEvent += x => Console.WriteLine($"{DateTime.Now} - [LIFT MESSAGE] The lift has arrived. Level: {x}.");
            lift.OnDoorOpenedEvent += x => Console.WriteLine($"{DateTime.Now} - [LIFT MESSAGE] The lift door has opened. Level: {x}.");
            lift.OnDoorClosedEvent += x => Console.WriteLine($"{DateTime.Now} - [LIFT MESSAGE] The lift door has closed. Level: {x}.");
        }

        private static bool InitializeParametersByArgs(string[] args)
        {
            return MaxLevelParameter.Initilaze(args, 0) &
                   LevelHeightParameter.Initilaze(args, 1) &
                   LiftSpeedParameter.Initilaze(args, 2) &
                   DoorTimeParameter.Initilaze(args, 3);
        }
    }
}
