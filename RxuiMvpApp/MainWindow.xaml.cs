using ReactiveUI;
using System.Reactive.Disposables;

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;

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

            //Note: The way I wired up zoom buttons is not the ReactiveUI way
            //I just needed a Proof of Concept for zooming an ArcGIS map.
            ZoomInButton.Click += (s, e) =>
            {
                MainMapView.SetViewpointScaleAsync(MainMapView.MapScale / 2);
                //System.Diagnostics.Debug.WriteLine($"Rodney was here during in {esriMapView.MapScale}");
            };

            ZoomOutButton.Click += (s, e) =>
            {
                MainMapView.SetViewpointScaleAsync(MainMapView.MapScale * 2);
                //System.Diagnostics.Debug.WriteLine($"Rodney was here during out {esriMapView.MapScale}");
            };

            /* ArcGIS map init stuff  
             * 
             https://developers.arcgis.com/net/  
             https://developers.arcgis.com/net/maps-2d/tutorials/display-a-map/
             */
            MapPoint mapCenterPoint = new MapPoint(-118.805, 34.027, SpatialReferences.Wgs84);
            MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, 100000));

            //MainMapView.NavigationCompleted += (s, e) =>
            //{
            //    System.Diagnostics.Debug.WriteLine($"Rodney was here during out {MainMapView.MapScale}");
            //};

            MainMapView.ViewpointChanged += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"Rodney was here during wheel event {MainMapView.MapScale}");
                ViewModel.ZoomLevel = MainMapView.MapScale;
            };

            /* ReactiveUI stuff */


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
            });
        }
    }
}
