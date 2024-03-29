﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using Splat;

namespace ZzzRxuiViewModelViewHostDemo
{
    
    public  class CustomRoutedViewHost : ContentControl, IActivatableView, IEnableLogger
    {
        /// <summary>
        /// The router dependency property.
        /// </summary>
        public static readonly DependencyProperty RouterProperty =
            DependencyProperty.Register("Router", typeof(RoutingState), typeof(RoutedViewHost), new PropertyMetadata(null));

        /// <summary>
        /// The default content property.
        /// </summary>
        public static readonly DependencyProperty DefaultContentProperty =
            DependencyProperty.Register("DefaultContent", typeof(object), typeof(RoutedViewHost), new PropertyMetadata(null));

        /// <summary>
        /// The view contract observable property.
        /// </summary>
        public static readonly DependencyProperty ViewContractObservableProperty =
            DependencyProperty.Register("ViewContractObservable", typeof(IObservable<string>), typeof(RoutedViewHost), new PropertyMetadata(Observable.Return("")));

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutedViewHost"/> class.
        /// </summary>
        public CustomRoutedViewHost()
        {

            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
            //if (ModeDetector.InUnitTestRunner())
            //{
            //    ViewContractObservable = Observable<string>.Never;
            //    return;
            //}

            //var platform = Locator.Current.GetService<IPlatformOperations>();
            //Func<string?> platformGetter = () => default!;

            //if (platform == null)
            //{
            //    // NB: This used to be an error but WPF design mode can't read
            //    // good or do other stuff good.
            //    this.Log().Error("Couldn't find an IPlatformOperations implementation. Please make sure you have installed the latest version of the ReactiveUI packages for your platform. See https://reactiveui.net/docs/getting-started/installation for guidance.");
            //}
            //else
            //{
            //    platformGetter = () => platform.GetOrientation();
            //}

            //ViewContractObservable = Observable.FromEvent<SizeChangedEventHandler, string?>(
            //        eventHandler =>
            //        {
            //            void Handler(object sender, SizeChangedEventArgs e) => eventHandler(platformGetter());
            //            return Handler;
            //        },
            //        x => SizeChanged += x,
            //        x => SizeChanged -= x)
            //    .DistinctUntilChanged()
            //    .StartWith(platformGetter())
            //    .Select(x => x);

            //var vmAndContract = Observable.CombineLatest(
            //    this.WhenAnyObservable(x => x.Router.CurrentViewModel!),
            //        (viewModel, contract) => (viewModel, contract)); ;
            var viewModel = this.WhenAnyObservable(x => x.Router.CurrentViewModel!);
            this.WhenActivated(d =>
            {
                // NB: The DistinctUntilChanged is useful because most views in
                // WinRT will end up getting here twice - once for configuring
                // the RoutedViewHost's ViewModel, and once on load via SizeChanged
                d(viewModel.DistinctUntilChanged().Subscribe(
                    x =>
                    {
                        if (x == null)
                        {
                            Content = DefaultContent;
                            return;
                        }

                        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                        var view = viewLocator.ResolveView(x, "") ?? viewLocator.ResolveView(x, null);

                        if (view == null)
                        {
                            throw new Exception($"Couldn't find view for '{x}'.");
                        }

                        view.ViewModel = x;
                        Content = view;
                    }, ex => RxApp.DefaultExceptionHandler?.OnNext(ex)));
            });
        }
        private readonly IDisposable disposable;
        /// <summary>
        /// Gets or sets the <see cref="RoutingState"/> of the view model stack.
        /// </summary>
        public RoutingState Router
        {
            get => (RoutingState)GetValue(RouterProperty);
            set => SetValue(RouterProperty, value);
        }

        /// <summary>
        /// Gets or sets the content displayed whenever there is no page currently
        /// routed.
        /// </summary>
        public object DefaultContent
        {
            get => (object)GetValue(DefaultContentProperty);
            set => SetValue(DefaultContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the view contract observable.
        /// </summary>
        /// <value>
        /// The view contract observable.
        /// </value>
        public IObservable<string?> ViewContractObservable
        {
            get => (IObservable<string?>)GetValue(ViewContractObservableProperty);
            set => SetValue(ViewContractObservableProperty, value);
        }

        /// <summary>
        /// Gets or sets the view locator.
        /// </summary>
        /// <value>
        /// The view locator.
        /// </value>
        public IViewLocator? ViewLocator { get; set; }
    }
}
