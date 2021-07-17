using ReactiveUI;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System.Windows;
using System.Reactive.Linq;
using System.Windows.Input;
using System;
using System.Reactive.Disposables;
using System.Windows.Controls.Primitives;

namespace RxuiMvpApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IViewFor<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            MapPoint mapCenterPoint = new MapPoint(-118.805, 34.027, SpatialReferences.Wgs84);
            MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, 100000));
            SliderInput1.Value = 50;
            /* ReactiveUI stuff */
            //ViewModel = new MainViewModel();
            this.WhenActivated(disposables =>
            {
                ZoomInButton.Events().Click
                    .Do(_ => MainMapView.SetViewpointScaleAsync(MainMapView.MapScale / 2))
                    .Do(_ =>
                    {
                        //This doesn't account for calculation of accurate percentage baed on map max possible scale and is only to show the slider working
                        SliderInput1.Value /= 2;
                    })
                    .Do(_ => SetText())
                    .Subscribe()
                    .DisposeWith(disposables);
                ZoomOutButton.Events().Click
                    .Do(_ => MainMapView.SetViewpointScaleAsync(MainMapView.MapScale * 2))
                    .Do(_ =>
                    {
                        //This doesn't account for calculation of accurate percentage baed on map max possible scale and is only to show the slider working
                        SliderInput1.Value += (100 - SliderInput1.Value) / 2;
                    })
                    .Do(_ => SetText())
                    .Subscribe()
                    .DisposeWith(disposables);
            });
        }
        private void SetText()
        {
            Input1.Text = "Current Scale: " + MainMapView.MapScale;
        }
    }
}