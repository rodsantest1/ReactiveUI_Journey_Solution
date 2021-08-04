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

            ViewModel = new MapViewModel
            {
                ZoomLevel = 100000
            };

            SliderInput1.Value = 50;

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
                this.WhenAnyValue(x => x.SliderInput1.Value)
                                  .Subscribe(x => Label1.Text = x.ToString());

                this.WhenAnyValue(x => x.ViewModel.ZoomLevel)
                    .Subscribe(x =>
                    {
                        if (x > 0)
                        {
                            Envelope extent = (Envelope)MainMapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry)?.TargetGeometry;

                            if (extent != null)
                            {
                                var updatedViewpoint = new Viewpoint(extent.GetCenter(), x);
                                MainMapView.SetViewpoint(updatedViewpoint);
                                ViewModel.ZoomLevel = x;
                                SliderInput1.Value = x / 2000;
                                Input1.Text = $"{x}";
                            }
                        }
                    });

                Observable.Merge(
                    zoomInButton.Select(_ => ViewModel.ZoomLevel / 2),
                    zoomOutButton.Select(_ => ViewModel.ZoomLevel * 2),
                    slider.Throttle(TimeSpan.FromMilliseconds(75), RxApp.MainThreadScheduler).Select(_ => SliderInput1.Value * 2000)                    
                    //mapWheel.Throttle(TimeSpan.FromMilliseconds(75), RxApp.MainThreadScheduler).Select(_ => MainMapView.MapScale)
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

