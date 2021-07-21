using ReactiveUI;
using System.Reactive.Disposables;

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System;
using System.Reactive.Linq;
using ReactiveUI.Fody.Helpers;
using RxuiMvpApp.ViewModels;

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

            ViewModel.ZoomLevel = 100000;

            MapPoint mapCenterPoint = new MapPoint(-118.805, 34.027, SpatialReferences.Wgs84);
            MyUserControl1.MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, ViewModel.ZoomLevel));

            var zoomInButton = Observable.FromEventPattern(
                h => MyUserControl1.ZoomInButton.Click += new System.Windows.RoutedEventHandler(h),
                h => MyUserControl1.ZoomInButton.Click -= new System.Windows.RoutedEventHandler(h));

            var zoomOutButton = Observable.FromEventPattern(
                h => MyUserControl1.ZoomOutButton.Click += new System.Windows.RoutedEventHandler(h),
                h => MyUserControl1.ZoomOutButton.Click -= new System.Windows.RoutedEventHandler(h));

            var mapWheel = Observable.FromEventPattern(
                h => MyUserControl1.MainMapView.ViewpointChanged += h,
                h => MyUserControl1.MainMapView.ViewpointChanged -= h
            );

            var slider = Observable.FromEventPattern(
                h => MyUserControl1.SliderInput1.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(h),
                h => MyUserControl1.SliderInput1.ValueChanged -= new System.Windows.RoutedPropertyChangedEventHandler<double>(h)
            );

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel,
                    vm => vm.ZoomLevel,
                    v => v.MyUserControl1.Input1.Text)
                    .DisposeWith(disposables);

                this.Bind(ViewModel,
                    vm => vm.ZoomLevel,
                    v => v.MyUserControl1.SliderInput1.Value)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.ZoomLevel,
                    v => v.MyUserControl1.Label1.Text)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ViewModel.ZoomLevel)
                    .Subscribe(x =>
                    {
                        if (x > 0)
                        {
                            MyUserControl1.MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, x));
                        }
                    });

                Observable.Merge(
                    zoomInButton.Select(_ => ViewModel.ZoomLevel / 2),
                    zoomOutButton.Select(_ => ViewModel.ZoomLevel * 2),
                    slider.Select(_ => MyUserControl1.SliderInput1.Value),
                    mapWheel.Select(_ => MyUserControl1.MainMapView.MapScale)
                ).Subscribe(x => ViewModel.ZoomLevel = x);
            });
        }
    }
}
