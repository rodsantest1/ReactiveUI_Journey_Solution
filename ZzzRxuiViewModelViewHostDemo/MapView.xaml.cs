using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        }
    }
}
