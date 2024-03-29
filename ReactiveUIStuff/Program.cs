﻿using ReactiveUI;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactiveUIStuff
{
    class Program
    {
        static void Main(string[] args)
        {
            //Examples from Commands in ReactiveUI handbook
            Console.WriteLine("Hello World!");

            //Example1();
            //Example2();
            //Example3();
            Example4();
        }

        private static void Example4()
        {
            // Creates a command with synchronous execution logic
            // which is always available for execution.
            var command = ReactiveCommand.CreateFromTask(
                () => Task.Run(() => Console.WriteLine("A reactive command is invoked!"))
            );

            command.Execute().Subscribe();
        }

        private static void Example3()
        {
            // Creates a command with synchronous execution logic
            // which is always available for execution.
            var command = ReactiveCommand.Create(
                () => Console.WriteLine("A reactive command is invoked!")
            );

            command.Execute().Subscribe();
        }

        private static void Example2()
        {
            // An asynchronous command created from IObservable<int> that 
            // waits 2 seconds and then returns 42 integer.
            var command = ReactiveCommand.CreateFromObservable<Unit, int>(
                _ => Observable.Return(42).Delay(TimeSpan.FromSeconds(2)));

            // Subscribing to the observable returned by `Execute()` will 
            // tick through the value `42` with a 2-second delay.
            command.Execute(Unit.Default)
                .Do(x => System.Diagnostics.Debug.WriteLine($"Hello World {x}"))
                .Subscribe();

            // We can also subscribe to _all_ values that a command
            // emits by using the `Subscribe()` method on the
            // ReactiveCommand itself.
            command.Subscribe(value => Console.WriteLine(value));

            Console.ReadKey();
        }

        private static void Example1()
        {
            // A synchronous command taking a parameter and returning nothing.
            // The Unit type is often used to denote the successfull completion
            // of a void-returning method (C#) or a sub procedure (VB).
            ReactiveCommand<int, Unit> command = ReactiveCommand.Create<int>(
                integer => Console.WriteLine(integer));

            // This outputs: 42
            command.Execute(42).Subscribe();
        }
    }
}
