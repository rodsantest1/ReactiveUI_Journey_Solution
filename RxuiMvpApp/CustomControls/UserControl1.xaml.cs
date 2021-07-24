using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using ReactiveUI;
using RxuiMvpApp.ViewModels;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RxuiMvpApp.CustomControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : ReactiveUserControl<MapViewModel>
    {
        public UserControl1()
        {
            InitializeComponent();

            ViewModel = new MapViewModel();

            ViewModel.ZoomLevel = 100000;

            MapPoint mapCenterPoint = new MapPoint(-118.805, 34.027, SpatialReferences.Wgs84);
            MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, ViewModel.ZoomLevel));

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
                    mapWheel.Throttle(TimeSpan.FromMilliseconds(50), RxApp.MainThreadScheduler).Select(_ => MainMapView.MapScale)
                ).Subscribe(x => ViewModel.ZoomLevel = x);
            });

        }
    }
}
