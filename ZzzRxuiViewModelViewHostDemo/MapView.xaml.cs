using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ZzzRxuiViewModelViewHostDemo
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : ReactiveUserControl<MapViewModel>
    {
        public MapView()
        {
            InitializeComponent();

            ViewModel = new MapViewModel
            {
                ZoomLevel = 100000
            };

            MapPoint mapCenterPoint = new MapPoint(-118.805, 34.027, SpatialReferences.Wgs84);
            this.MapControl.esriMapView.SetViewpoint(new Viewpoint(mapCenterPoint, ViewModel.ZoomLevel));

            var zoomInButton = Observable.FromEventPattern(
                h => ZoomInButton.Click += new System.Windows.RoutedEventHandler(h),
                h => ZoomInButton.Click -= new System.Windows.RoutedEventHandler(h));

            var zoomOutButton = Observable.FromEventPattern(
                h => ZoomOutButton.Click += new System.Windows.RoutedEventHandler(h),
                h => ZoomOutButton.Click -= new System.Windows.RoutedEventHandler(h));

            var mapWheel = Observable.FromEventPattern(
                h => MapControl.esriMapView.ViewpointChanged += h,
                h => MapControl.esriMapView.ViewpointChanged -= h
            );

            var slider = Observable.FromEventPattern(
                h => SliderInput1.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(h),
                h => SliderInput1.ValueChanged -= new System.Windows.RoutedPropertyChangedEventHandler<double>(h)
            );

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel,
             vm => vm.ZoomLevel,
             v => v.MapControl.Input1.Text)
             .DisposeWith(disposables);

                this.Bind(ViewModel,
                    vm => vm.ZoomLevel,
                    v => v.SliderInput1.Value)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.ZoomLevel,
                    v => v.MapControl.Label1.Text)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ViewModel.ZoomLevel)
                    .Subscribe(x =>
                    {
                        if (x > 0)
                        {
                            Envelope extent = (Envelope)MapControl.esriMapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry)?.TargetGeometry;

                            if (extent != null)
                            {
                                var updatedViewpoint = new Viewpoint(extent.GetCenter(), x);
                                MapControl.esriMapView.SetViewpoint(updatedViewpoint);
                                ViewModel.ZoomLevel = x;
                            }
                        }
                    });

                Observable.Merge(
                    zoomInButton.Select(_ => ViewModel.ZoomLevel / 2),
                    zoomOutButton.Select(_ => ViewModel.ZoomLevel * 2),
                    slider.Throttle(TimeSpan.FromMilliseconds(75), RxApp.MainThreadScheduler).Select(_ => SliderInput1.Value),
                    mapWheel.Throttle(TimeSpan.FromMilliseconds(75), RxApp.MainThreadScheduler).Select(_ => MapControl.esriMapView.MapScale)
                ).Subscribe(x => ViewModel.ZoomLevel = x);
            });

        }
    }
}