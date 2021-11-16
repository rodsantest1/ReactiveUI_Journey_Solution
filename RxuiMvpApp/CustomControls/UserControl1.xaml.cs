using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using RxuiMvpApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private int _scaleIndex = 0;
        private List<double> _scales = new List<double>();

        private void SetScales()
        {
            var counter = 0;
            for (var scale = _maxScale; scale <= _minScale; scale *= 2, counter++)
            {
                _scales.Add(scale);
                if (scale == _startScale)
                {
                    SliderInput1.Value = counter;
                }
            }

            SliderInput1.Minimum = 0;
            SliderInput1.Maximum = _scales.Count - 1;
        }

        public UserControl1()
        {
            InitializeComponent();

            SetScales();
            ViewModel = new MapViewModel();

            MapPoint mapCenterPoint = new MapPoint(-118.805, 34.027, SpatialReferences.Wgs84);
            MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, _startScale));

            ZoomInButton.Click += ZoomInButton_Click;
            ZoomOutButton.Click += ZoomOutButton_Click;
            SliderInput1.ValueChanged += SliderInput1_ValueChanged;
            MainMapView.NavigationCompleted += MainMapView_NavigationCompleted;
        }

        private void ZoomInButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_scaleIndex - 1 >= 0)
            {
                SliderInput1.Value = --_scaleIndex;
            }
        }

        private void ZoomOutButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_scaleIndex + 1 < _scales.Count)
            {
                SliderInput1.Value = ++_scaleIndex;
            }
        }

        private void SliderInput1_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            _scaleIndex = (int)e.NewValue;
            SetZoomForNonMouseEvent(_scaleIndex);
        }

        private void SetZoomForNonMouseEvent(int scaleIndex)
        {
            MainMapView.NavigationCompleted -= MainMapView_NavigationCompleted;

            var env = (Envelope)MainMapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry).TargetGeometry;
            var scale = _scales[scaleIndex];
            MainMapView.SetViewpoint(new Viewpoint(env.GetCenter(), scale));

            MainMapView.NavigationCompleted += MainMapView_NavigationCompleted;
        }

        private void MainMapView_NavigationCompleted(object sender, EventArgs e)
        {
            if (MainMapView.MapScale != _lastScale)
            {
                var isZoomIn = MainMapView.MapScale < _lastScale;

                _lastScale = !isZoomIn ?
                    _scales.Where(x => x > _lastScale).FirstOrDefault() :
                    _scales.Where(x => x < _lastScale).LastOrDefault();

                if (_lastScale > 0)
                {
                    var scaleToUse = -1;
                    var minDiff = double.MaxValue;
                    foreach (var scale in _scales)
                    {
                        var diff = MainMapView.MapScale - scale;
                        diff = diff < 0 ? diff * -1 : diff;
                        if (diff <= minDiff)
                        {
                            minDiff = diff;
                            scaleToUse = _scales.IndexOf(scale);
                        }
                    }

                    if (scaleToUse >= 0)
                    {
                        SliderInput1.ValueChanged -= SliderInput1_ValueChanged;
                        SliderInput1.Value = scaleToUse;
                        _scaleIndex = scaleToUse;
                        SliderInput1.ValueChanged += SliderInput1_ValueChanged;
                    }
                }
            }
        }
    }
}

namespace RxuiMvpApp.ViewModels
{

    public class MapViewModel : ReactiveObject
    {
        public MapViewModel()
        {
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

