using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReactiveExtensionsUdemyCourse
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Example0();

            Console.ReadKey();
        }

        private static void Example0()
        {
            // Example of INotifyPropertyChanged implementation
            // A bit of plumbing for a single property
            var market = new Market0();

            market.PropertyChanged += (sender, eventArgs) =>
            {
                Console.WriteLine("Inside handler");

                if (eventArgs.PropertyName == "Volatility")
                {
                    Console.WriteLine(sender.GetType().Name);
                }
            };

            market.Volatility = 3F;
        }
    }

    public class Market0 : INotifyPropertyChanged
    {
        private float volatility;

        public float Volatility
        {
            get => volatility;
            set
            {
                if (value.Equals(volatility))
                {
                    return;
                }

                volatility = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
