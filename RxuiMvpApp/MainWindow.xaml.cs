using ReactiveUI;
using System.Reactive.Disposables;

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System;
using System.Reactive.Linq;
using ReactiveUI.Fody.Helpers;

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

            MapPoint mapCenterPoint = new MapPoint(-118.805, 34.027, SpatialReferences.Wgs84);
            MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, 100000));
            ViewModel.ZoomLevel = 100000;

            var zoomInButton = Observable.FromEventPattern(
                h => ZoomInButton.Click += new System.Windows.RoutedEventHandler(h),
                h => ZoomInButton.Click -= new System.Windows.RoutedEventHandler(h));

            var zoomOutButton = Observable.FromEventPattern(
                h => ZoomOutButton.Click += new System.Windows.RoutedEventHandler(h),
                h => ZoomOutButton.Click -= new System.Windows.RoutedEventHandler(h));

            var mapWheel = Observable.FromEventPattern(
                h => MainMapView.ViewpointChanged += h,
                h => MainMapView.ViewpointChanged -= h
            );

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
                            MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, x));
                        }
                    });

                Observable.Merge(
                    zoomInButton.Select(_ => ViewModel.ZoomLevel / 2),
                    zoomOutButton.Select(_ => ViewModel.ZoomLevel * 2),
                    slider.Select(_ => SliderInput1.Value),
                    mapWheel.Select(_ => MainMapView.MapScale)
                ).Subscribe(x => ViewModel.ZoomLevel = x);
            });
        }

        /// <summary>
        /// Scale a linear range between 0.0-1.0 to an exponential scale using the equation returnValue = A + B * Math.Exp(C * inputValue);
        /// </summary>
        /// <param name="inoutValue">The value to scale</param>
        /// <param name="midValue">The value returned for input value of 0.5</param>
        /// <param name="maxValue">The value to be returned for input value of 1.0</param>
        /// <returns></returns>
        private double ExpScale(double inputValue, double midValue, double maxValue)
        {
            double returnValue = 0;
            //if (inputValue < 0 || inputValue > 1) throw new ArgumentOutOfRangeException("Input value must be between 0 and 1.0");
            //if (midValue <= 0 || midValue >= maxValue) throw new ArgumentOutOfRangeException("MidValue must be greater than 0 and less than MaxValue");
            // returnValue = A + B * Math.Exp(C * inputValue);
            double M = maxValue / midValue;
            double C = Math.Log(Math.Pow(M - 1, 2));
            double B = maxValue / (Math.Exp(C) - 1);
            double A = -1 * B;
            returnValue = A + B * Math.Exp(C * inputValue);
            return returnValue;
        }
    }
}
