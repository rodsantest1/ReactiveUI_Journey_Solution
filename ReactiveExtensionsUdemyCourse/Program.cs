using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReactiveExtensionsUdemyCourse
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Example0();
            Example00();

            Console.ReadKey();
        }

        private static void Example00()
        {
            var market = new Market00();
            market.PriceAdded += (sender, f) =>
            {
                Console.WriteLine($"We got a price of {f}");
            };

            market.AddPrice(123);
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

    public class Market00
    {
        private List<float> prices = new List<float>();

        public event EventHandler<float> PriceAdded;

        public void AddPrice(float price)
        {
            prices.Add(price);
            PriceAdded?.Invoke(this, price);
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
