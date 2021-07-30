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
                ZoomLevel = 50
            };

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

                this.ViewModel.ZoomInteraction.RegisterHandler(interaction =>
                {
                    Observable.Start(() =>
                    {

                    }).Subscribe(x=>
                    {
                        var zoomLevel = this.ViewModel.ZoomLevel;

                        if (zoomLevel > 0)
                        {
                            Envelope extent = (Envelope)MainMapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry)?.TargetGeometry;

                            if (extent != null)
                            {
                                this.ViewModel.MapPointCenter = extent.GetCenter();
                        
                                //ViewModel.ZoomLevel = displayValue;
                            }
                        }
                    });

                    interaction.SetOutput(this.SliderInput1.Value);    
                });

                

                this.WhenAnyValue(x => x.ViewModel.ZoomLevel)
                    .Subscribe(x =>
                    {
                     
                    });

                //Observable.Merge(
                //    zoomInButton.Select(_ => ViewModel.ZoomLevel / 2),
                //    zoomOutButton.Select(_ => ViewModel.ZoomLevel * 2),
                //    slider.Throttle(TimeSpan.FromMilliseconds(75), RxApp.MainThreadScheduler).Select(_ => SliderInput1.Value),
                //    mapWheel.Throttle(TimeSpan.FromMilliseconds(75), RxApp.MainThreadScheduler).Select(_ => MainMapView.MapScale)
                //).Subscribe(x => ViewModel.ZoomLevel = x);
            });

        }


    }
}

namespace RxuiMvpApp.ViewModels
{

    public class MapViewModel : ReactiveObject
    {
        [Reactive] public double ZoomLevel { get; set; }
        ReactiveCommand<Unit, Double> ZoomCommand { get; }
        [Reactive] public double DisplayValue { get; set; }
        [Reactive] public MapPoint MapPointCenter { get; set; }

        private readonly Interaction<Unit, Double> _zoomInteraction;
        public Interaction<Unit, Double> ZoomInteraction => this._zoomInteraction;

        const double A = -26110;
        const double B = 26111;
        const double C = 0.04317;

        public MapViewModel()
        {
            SetupMap();

            ZoomCommand = ReactiveCommand.CreateFromObservable<Unit, Double>(this.ZoomSlider);
            _zoomInteraction = new Interaction<Unit, double>();
        }

        public IObservable<Double> ZoomSlider(Unit unit)
        {

            return Observable.StartAsync(async () =>
            {
                var currentZoomLevel = await this._zoomInteraction.Handle(unit);
                var displayValue = GetDisplayValue(currentZoomLevel);
                var updatedViewpoint = new Viewpoint(MapPointCenter, displayValue);
                MainMapView.SetViewpoint(updatedViewpoint);
                return currentZoomLevel;
            });
        }

        [Reactive] public Map Map { get; set; }

        private void SetupMap()
        {
            // Create a new map with a 'topographic vector' basemap.
            Map = new Map(BasemapStyle.ArcGISTopographic);
        }

        private double GetDisplayValue(double sliderValue)
        {
            var displayValue = A + (B * Math.Exp(C * sliderValue));

            return displayValue;
        }
        private double GetSliderValue(double displayValue)
        {
            var sliderValue = Math.Log((displayValue - A) / B) / C;

            return sliderValue;
        }
    }
}

