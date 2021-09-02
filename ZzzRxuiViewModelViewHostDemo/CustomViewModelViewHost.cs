using ReactiveUI;
using Splat;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ZzzRxuiViewModelViewHostDemo
{
    /// <summary>
    /// This content control will automatically load the View associated with
    /// the ViewModel property and display it. This control is very useful
    /// inside a DataTemplate to display the View associated with a ViewModel.
    /// </summary>
    [SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "Deliberate usage")]
    public class CustomViewModelViewHost : ContentControl, IViewFor, IEnableLogger, IDisposable
    {
        private static int InstanceCount;
     
        /// <summary>
        /// The default content dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultContentProperty =
            DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(CustomViewModelViewHost), new PropertyMetadata(null));

        /// <summary>
        /// The view model dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(object), typeof(CustomViewModelViewHost), new PropertyMetadata(null, SomethingChanged));

        /// <summary>
        /// The view contract observable dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewContractObservableProperty =
            DependencyProperty.Register(nameof(ViewContractObservable), typeof(IObservable<string>), typeof(CustomViewModelViewHost), new PropertyMetadata(Observable.Return(""), SomethingChanged));

        private readonly Subject<Unit> _updateViewModel = new Subject<Unit>();
        private string _viewContract;
        private bool _isDisposed;
        private readonly IDisposable disposable;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomViewModelViewHost"/> class.
        /// </summary>
        public CustomViewModelViewHost()
        {
            //Debug.WriteLine($"CustomViewModelViewHost ctor Instance Count: {Interlocked.Increment(ref InstanceCount)}");

            if (ModeDetector.InUnitTestRunner())
            {
                ViewContractObservable = Observable.Return("");

                // NB: InUnitTestRunner also returns true in Design Mode
                return;
            }

            //ViewContractObservable = Observable.FromEvent<SizeChangedEventHandler, string>(
            //    eventHandler =>
            //    {
            //        void Handler(object sender, SizeChangedEventArgs e) => eventHandler(platformGetter());
            //        return Handler;
            //    },
            //    x => SizeChanged += x,
            //    x => SizeChanged -= x)
            //    .Do(x => this.Log().Debug($"SizeChangedEventHandler"))
            //    .StartWith(platformGetter())
            //    .DistinctUntilChanged();

            //var contractChanged = _updateViewModel.Select(_ => ViewContractObservable).Switch();
            var viewModelChanged = _updateViewModel.Select(_ => ViewModel);

            //var vmAndContract = contractChanged.CombineLatest(viewModelChanged, (contract, vm) => new { ViewModel = vm, Contract = contract });
            disposable =
                viewModelChanged.Subscribe(x => ResolveViewForViewModel(x, ""));
            //contractChanged
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(x => _viewContract = x);
        }

        /// <summary>
        /// Gets or sets the view contract observable.
        /// </summary>
        public IObservable<string> ViewContractObservable
        {
            get => (IObservable<string>)GetValue(ViewContractObservableProperty);
            set => SetValue(ViewContractObservableProperty, value);
        }

        /// <summary>
        /// Gets or sets the content displayed by default when no content is set.
        /// </summary>
        public object DefaultContent
        {
            get => GetValue(DefaultContentProperty);
            set => SetValue(DefaultContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the ViewModel to display.
        /// </summary>
        public object ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// Gets or sets the view contract.
        /// </summary>
        public string ViewContract
        {
            get => _viewContract;
            set => ViewContractObservable = Observable.Return(value);
        }

        /// <summary>
        /// Gets or sets the view locator.
        /// </summary>
        public IViewLocator ViewLocator { get; set; }

        /// <inheritdoc />
#pragma warning disable CA1063 // Implement IDisposable Correctly
        public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            disposable.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
            //Debug.WriteLine($"CustomViewModelViewHost dispose Instance Count: {Interlocked.Decrement(ref InstanceCount)}");
        }

        /// <summary>
        /// Disposes of resources inside the class.
        /// </summary>
        /// <param name="isDisposing">If we are disposing managed resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                Debug.WriteLine($"Disposing of updateViewModel");
                _updateViewModel?.Dispose();
            }

            _isDisposed = true;
        }

        private static void SomethingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((CustomViewModelViewHost)dependencyObject)._updateViewModel.OnNext(Unit.Default);
        }

        private void ResolveViewForViewModel(object viewModel, string contract)
        {
            if (viewModel == null)
            {
                Content = DefaultContent;
                return;
            }

            var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
            var viewInstance = viewLocator.ResolveView(viewModel, contract) ?? viewLocator.ResolveView(viewModel, null);

            if (viewInstance == null)
            {
                Content = DefaultContent;
                this.Log().Warn($"The {nameof(CustomViewModelViewHost)} could not find a valid view for the view model of type {viewModel.GetType()} and value {viewModel}.");
                return;
            }

            viewInstance.ViewModel = viewModel;

            Content = viewInstance;
        }
    }
}