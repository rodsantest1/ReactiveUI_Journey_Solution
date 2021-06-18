using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveExtensionsUdemyCourse
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Example0();
            //Example00();
            //Example000();
            //Example0000(); //Not runnable,
            // just shows the setup for IObserver and IObservable

            //Example1(); //Introduction to Subject
            //Example2();
            //Example3();
            //Example4(); //ReplaySubject
            //Example5();
            //Example6();
            //Example7(); //BehaviorSubject

            //Example8();
            Example9();

            Console.ReadKey();
        }

        private static void Example9()
        {
            var sensor = new AsyncSubject<double>();
            sensor.Inspect("async");

            sensor.OnNext(1.0);
            sensor.OnNext(2.0);
            sensor.OnNext(3.0);
            sensor.OnCompleted();

            sensor.OnNext(123); //You aren't going to get this.
        }

        private static void Example8()
        {
            Task<int> t = Task<int>.Factory.StartNew(() => 42);
            int value = t.Result;

            Console.WriteLine(value);
        }

        private static void Example7()
        {
            var sensorReading = new BehaviorSubject<double>(-1.0);

            sensorReading.Inspect("sensor");

            sensorReading.OnNext(0.99);

            sensorReading.OnCompleted();
        }

        private static void Example6()
        {
            var bufferSize = 1;
            var market = new ReplaySubject<float>(bufferSize);

            market.OnNext(123);
            market.OnNext(456);
            market.OnNext(789);

            market.Subscribe(x => Console.WriteLine($"Got the price {x}"));
        }

        private static void Example5()
        {
            var timeWindow = TimeSpan.FromMilliseconds(500);
            var market = new ReplaySubject<float>(timeWindow);

            market.OnNext(123);
            Thread.Sleep(200);
            market.OnNext(456);
            Thread.Sleep(200);
            market.OnNext(789);
            Thread.Sleep(200);

            market.Subscribe(x => Console.WriteLine($"Got the price {x}"));
        }

        private static void Example4()
        {
            var market = new ReplaySubject<float>();

            market.OnNext(123);

            market.Subscribe(x => Console.WriteLine($"Got the price {x}"));
        }

        private static void Example3()
        {
            var market = new Subject<float>();
            var marketConsumber = new Subject<float>();

            market.Subscribe(marketConsumber);

            marketConsumber.Inspect("market consumer");

            market.OnNext(1, 2, 3, 4);

            market.OnCompleted();
        }

        private static void Example2()
        {
            var sensor = new Subject<float>();

            using (sensor.Subscribe(Console.WriteLine))
            {
                sensor.OnNext(1);
            }

            sensor.OnNext(2);
        }

        private static void Example1()
        {
            //Subject combines both implementations of IObserver and IOservable
            var market = new Subject<float>();
            market.Subscribe(
                f => Console.WriteLine($"Price is {f}"),
                e => Console.WriteLine($"Error was {e.Message}"),
                () => Console.WriteLine("Sequence is complete"));
            //market.Subscribe(marketObserver);

            market.OnNext(7.24f);
            //market.OnError(new Exception("oops"));
            market.OnCompleted();
        }

        private static void Example0000()
        {
            //As we'll see later Rx encapsulates 
            // manually implementing IObserver and IObservable
            // OnNext* --> (OnError | OnCompleted)?

            var market = new Market0000();

            market.Subscribe(new MarketObserver0000());
        }

        public class MarketObserver0000 : IObserver<float>
        {
            public void OnNext(float value)
            {
                Console.WriteLine($"Market gave us {value}");
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            public void OnCompleted()
            {
                throw new NotImplementedException();
            }
        }

        public class Market0000 : IObservable<float>
        {
            public IDisposable Subscribe(IObserver<float> observer)
            {
                throw new NotImplementedException();
            }
        }

        private static void Example000()
        {
            var market = new Market000();
            market.Prices.ListChanged += (sender, args) =>
            {
                if (args.ListChangedType == ListChangedType.ItemAdded)
                {
                    float price = ((BindingList<float>)sender)[args.NewIndex];
                    Console.WriteLine($"Binding list got a price of {price}");
                }
            };

            market.AddPrice(123);
        }

        public class Market000
        {
            public BindingList<float> Prices = new BindingList<float>();

            public void AddPrice(float price)
            {
                Prices.Add(price);
            }
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

}
