using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RxuiMvpApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();

            var slider = Observable.FromEventPattern(
                h => SliderInput1.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(h),
                h => SliderInput1.ValueChanged -= new System.Windows.RoutedPropertyChangedEventHandler<double>(h)
            );

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel,
                    vm => vm.ZoomLevel,
                    v => v.Input1.Text)
                    .DisposeWith(disposables);

                this.Bind(ViewModel,
                    vm => vm.ZoomLevel,
                    v => v.SliderInput1.Value)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.ZoomLevel,
                    v => v.Label1.Text)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ViewModel.ZoomLevel)
                .Subscribe(x =>
                {
                    if (x > 0)
                    {
                        var displayValue = Math.Exp(0.04 * x);
                        Label1.Text = $"{displayValue}";
                        ViewModel.ZoomLevel = x;
                    }
                });

                Observable.Merge(
                    slider.Throttle(TimeSpan.FromMilliseconds(75), RxApp.MainThreadScheduler).Select(_ => SliderInput1.Value)
                ).Subscribe(x => ViewModel.ZoomLevel = x);

            });
        }
    }
}
