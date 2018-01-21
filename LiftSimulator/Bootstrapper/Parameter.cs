using System;

namespace LiftSimulator.Bootstrapper
{
    public class Parameter<T> where T : struct
    {
        public string Name { get; }
        public Func<string, T> Cast{ get; }
        public Predicate<T> Test { get; }

        public T Value { get; private set; }

        public Parameter(string name, Func<string, T> cast, Predicate<T> test)
        {
            this.Name = name;
            this.Cast = cast;
            this.Test = test;
        }

        public bool Initilaze(string[] args, int index)
        {
            string input = null;
            try
            {
                input = args[index];
            }
            catch
            {
                Console.WriteLine($"{Name} is undefined.");
                return false;
            }
            try
            {
                this.Value = Cast.Invoke(input);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Can't cast \"{input}\": {exception.Message}" );
                return false;
            }
            var testResult = Test.Invoke(this.Value);
            Console.WriteLine($"{Name}: {Value} (Approved: {testResult})");
            return testResult;
        }

    }
}
