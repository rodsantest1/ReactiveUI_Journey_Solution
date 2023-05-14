using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace ReactiveExtensionsUdemyCourse
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Example0();
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
            //Example9();
            //Example10();

            /* Observable Sequences */

            //Example11();
            //Example12();
            //Example13();
            //Example14();
            //Example15(); //Observable.Create
            //Example16();
            //Example17();

            //Example18(); //Unexpected behavior
            //Example19(); //Expected behavior

            //Example20(); //Sequence factories
            //Example21();
            //Example22();
            //Example23();
            //Example24();
            //Example25();
            //Example26();
            //Example27();


            Console.ReadKey();
        }

        private static void Example27()
        {
            var item = new List<int> { 1, 2, 3 };
            var source = item.ToObservable();
            source.Inspect("observable");
        }

        private static void Example26()
        {
            var t = Task.Factory.StartNew(() => "Test");
            var source = t.ToObservable();
            source.Inspect("task");

            //There is already one in Main
            //Console.ReadLine();
        }

        private static void Example25()
        {
            var market = new Market25();
            var priceChanges = Observable.FromEventPattern<float>(
                h => market.PriceChanged += h,
                h => market.PriceChanged -= h
            );

            //Inspect will not work in this case
            //priceChanges.Inspect("price changes"); 
            priceChanges.Subscribe(
                x => Console.WriteLine($"{x.EventArgs}")
            );

            market.ChangePrice(1);
            market.ChangePrice(1.1f);
            market.ChangePrice(1.2f);
        }

        public class Market25
        {
            private float _price;

            //public float Price
            //{
            //    get { return _price; }
            //    set { _price = value; }
            //}

            public float Price
            {
                get => _price;
                set => _price = value;
            }

            public void ChangePrice(float price)
            {
                Price = price;
                PriceChanged?.Invoke(this, price);
            }

            public event EventHandler<float> PriceChanged;
        }

        private static void Example24()
        {
            //The difference between Start and Return is
            //Return is eager
            //Start is lazy
            //
            var start = Observable.Start(() =>
            {
                Console.WriteLine("Starting work...");
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(200);
                    Console.Write(".");
                }
            });

            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(200);
                Console.Write("-");
            }

            start.Inspect("start");
            Console.ReadKey();
        }

        private static void Example23()
        {
            var timer = Observable.Timer(TimeSpan.FromSeconds(2));
            timer.Inspect("timer");
            Console.ReadLine();
        }

        private static void Example22()
        {
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(500));
            using (interval.Inspect("interval"))
            {
                Console.ReadKey();
            }
        }

        private static void Example21()
        {
            var generated = Observable.Generate(
                1,
                value => value < 100,
                value => value * value + 1,
                value => $"[{value}]" //Like linq Select()
            );

            generated.Inspect("generated");
        }

        private static void Example20()
        {
            var tenToTwenty = Observable.Range(10, 11);
            tenToTwenty.Inspect("range");
        }

        private static void Example19()
        {
            var obs = Observable.Create<string>(o =>
            {
                var timer = new Timer(1000);
                timer.Elapsed += (s, e) => o.OnNext($"tick {e.SignalTime}");
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
                return () =>
                {
                    timer.Elapsed -= Timer_Elapsed;
                    timer.Dispose();
                };
            });

            var sub = obs.Inspect("timer");
            Console.ReadLine();

            sub.Dispose();
            Console.ReadLine();
        }

        private static void Example18()
        {
            var obs = Observable.Create<string>(o =>
            {
                var timer = new Timer(1000);
                timer.Elapsed += (s, e) => o.OnNext($"tick {e.SignalTime}");
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
                return Disposable.Empty;
            });

            var sub = obs.Inspect("timer");
            Console.ReadLine();

            sub.Dispose();
            Console.ReadLine();
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine($"tock {e.SignalTime}");
        }

        private static void Example17()
        {
            //var meaningOfLife = Observable.Return<int>(42); //OnNext(42), OnCompleted

            //same as

            var meaningOfLife2 = Return(42);
            meaningOfLife2.Inspect("meaningOfLife2");
        }

        public static IObservable<T> Return<T>(T value)
        {
            return Observable.Create<T>(x =>
            {
                x.OnNext(value);
                x.OnCompleted();
                return Disposable.Empty;
            });
        }

        private static void Example16()
        {
            NonBlocking().Inspect("nonblocking");
        }

        private static void Example15()
        {
            Blocking().Inspect("blocking");
        }

        private static IObservable<string> NonBlocking()
        {
            return Observable.Create<string>(observer =>
            {
                observer.OnNext("foo", "bar");
                observer.OnCompleted();
                Thread.Sleep(3000);
                return Disposable.Empty;
            });
        }

        private static IObservable<string> Blocking()
        {
            var subj = new ReplaySubject<string>();
            subj.OnNext("foo", "bar");
            subj.OnCompleted();
            Thread.Sleep(3000);
            return subj;
        }

        private static void Example14()
        {
            var obs = Observable.Throw<int>(new Exception("oops"));
            obs.Inspect("obs");
        }

        private static void Example13()
        {
            //does not produce the completion signal
            var obs = Observable.Never<int>();
            obs.Inspect("obs");
        }

        private static void Example12()
        {
            var obs = Observable.Empty<int>();
            obs.Inspect("obs");
        }

        private static void Example11()
        {
            var obs = Observable.Return(42); //ReplaySubject
            obs.Inspect("obs");
        }

        private static void Example10()
        {
            new Market10Observer();
        }

        public class Market10 : IObservable<float>
        {
            private ImmutableHashSet<IObserver<float>> observers =
                ImmutableHashSet<IObserver<float>>.Empty;

            public IDisposable Subscribe(IObserver<float> observer)
            {
                observers = observers.Add(observer);

                return Disposable.Create(() =>
                {
                    observers = observers.Remove(observer);
                });
            }

            public void Publish(float price)
            {
                foreach (var o in observers)
                {
                    o.OnNext(price);
                }
            }
        }

        public class Market10Observer
        {
            public Market10Observer()
            {
                var market = new Market10();
                var sub = market.Inspect("market");

                market.Publish(123.4f);
                sub.Dispose();
                market.Publish(567.8f);
            }
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
