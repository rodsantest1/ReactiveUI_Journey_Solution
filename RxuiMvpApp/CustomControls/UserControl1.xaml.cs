using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using RxuiMvpApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;

namespace RxuiMvpApp.CustomControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : ReactiveUserControl<MapViewModel>
    {
        private const int _maxScale = 1250;
        private const int _minScale = 81920000;
        private const int _startScale = 10000;
        private double _lastScale = _startScale;
        private MapPoint _mouseMapPoint;
        private List<double> _scales = new List<double>();

        private void SetScales()
        {
            for (var scale = _maxScale; scale <= _minScale; scale *= 2)
            {
                _scales.Add(scale);
            }
        }

        public UserControl1()
        {
            InitializeComponent();

            SetScales();
            ViewModel = new MapViewModel();

            //MainMapView.InteractionOptions = new Esri.ArcGISRuntime.UI.MapViewInteractionOptions()
            //{
            //    IsZoomEnabled = false
            //};

            MapPoint mapCenterPoint = new MapPoint(-118.805, 34.027, SpatialReferences.Wgs84);
            MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, ViewModel.ZoomLevel));

            SliderInput1.ValueChanged += SliderInput1_ValueChanged;
            MainMapView.NavigationCompleted += MainMapView_NavigationCompleted;
            MainMapView.MouseMove += MainMapView_MouseMove;
            //MainMapView.MouseWheel += MainMapView_MouseWheel;

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel,
                    vm => vm.ZoomLevel,
                    v => v.Input1.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.ZoomLevel,
                    v => v.Label1.Text)
                    .DisposeWith(disposables);
            });
        }

        private void MainMapView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _mouseMapPoint = MainMapView.ScreenToLocation(e.GetPosition(MainMapView));
        }

        //private void MainMapView_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        //{
        //if (e.Delta > 0)
        //{
        //    ViewModel.ZoomLevel /= 2;
        //}
        //else
        //{
        //    ViewModel.ZoomLevel *= 2;
        //}

        //SliderInput1.ValueChanged -= SliderInput1_ValueChanged;
        //var position = MainMapView.ScreenToLocation(e.GetPosition(MainMapView));
        //MainMapView.SetViewpoint(new Viewpoint(position, ViewModel.ZoomLevel));
        //SliderInput1.Value = ViewModel.ZoomLevel;
        //SliderInput1.ValueChanged += SliderInput1_ValueChanged;
        //}

        private void MainMapView_NavigationCompleted(object sender, EventArgs e)
        {
            // mapscale = 15854.65465123
            // last = 10000
            if (MainMapView.MapScale > _lastScale)
            {
                // zoom out
                var t = _scales.Where(x => x > _lastScale).FirstOrDefault();
                if (t > 0)
                {
                    _lastScale = t;
                    MainMapView.SetViewpoint(new Viewpoint(_mouseMapPoint, t));
                    ViewModel.ZoomLevel = MainMapView.MapScale;
                }
            }
            else if (MainMapView.MapScale < _lastScale)
            {
                // zoom in
                var t = _scales.Where(x => x < _lastScale).LastOrDefault();
                if (t > 0)
                {
                    _lastScale = t;
                    MainMapView.SetViewpoint(new Viewpoint(_mouseMapPoint, t));
                    ViewModel.ZoomLevel = MainMapView.MapScale;
                }
            }

            if (_scales.Contains(MainMapView.MapScale))
            {
                SliderInput1.ValueChanged -= SliderInput1_ValueChanged;
                //SliderInput1.Value = MainMapView.MapScale;
                SliderInput1.Value = _scales.IndexOf(MainMapView.MapScale);
                SliderInput1.ValueChanged += SliderInput1_ValueChanged;
            }
        }

        private void SliderInput1_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            //MainMapView.NavigationCompleted -= MainMapView_NavigationCompleted;

            //ViewModel.ZoomLevel = e.NewValue;
            var scale = _scales[(int)e.NewValue];
            ViewModel.ZoomLevel = scale;
            var env = (Envelope)MainMapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry).TargetGeometry;
            MainMapView.SetViewpoint(new Viewpoint(env.GetCenter(), scale));
            
            //MainMapView.NavigationCompleted += MainMapView_NavigationCompleted;
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
            ZoomLevel = 10000;
            SetupMap();
        }

        [Reactive] public Map Map { get; set; }

        private void SetupMap()
        {
            // Create a new map with a 'topographic vector' basemap.
            Map = new Map(BasemapStyle.ArcGISTopographic)
            {
                MaxScale = 1250,
                MinScale = 81920000
            };
        }
    }
}

