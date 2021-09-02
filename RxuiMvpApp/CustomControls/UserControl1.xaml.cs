using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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
                    .Where(x => x > 0)
                    .Subscribe(async x => await MainMapView.SetViewpointScaleAsync(x));

                Observable.Merge(
                    zoomInButton.Select(_ => ViewModel.ZoomLevel / 2),
                    zoomOutButton.Select(_ => ViewModel.ZoomLevel * 2),
                    slider.Throttle(TimeSpan.FromMilliseconds(75), RxApp.MainThreadScheduler).Select(_ => SliderInput1.Value),
                    mapWheel.Throttle(TimeSpan.FromMilliseconds(75), RxApp.MainThreadScheduler).Select(_ => MainMapView.MapScale)
                ).Subscribe(x => ViewModel.ZoomLevel = x);
            });

        }
    }
}

namespace RxuiMvpApp.ViewModels
{

    public class MapViewModel : ReactiveObject
    {
        [Reactive] public double ZoomLevel { get; set; }

        public MapViewModel()
        {
            ZoomLevel = 100000;
            SetupMap();
        }

        [Reactive] public Map Map { get; set; }

        private void SetupMap()
        {
            // Create a new map with a 'topographic vector' basemap.
            Map = new Map(BasemapStyle.ArcGISTopographic);
        }
    }
}

