using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;

namespace ZzzRxuiViewModelViewHostDemo
{
    public class MapViewModel : ReactiveObject
    {
        [Reactive] public Map Map { get; set; }
        [Reactive] public double ZoomLevel { get; set; }

        public MapViewModel()
        {
            ZoomLevel = 100000;
            SetupMap();
        }

        private void SetupMap()
        {
            // Create a new map with a 'topographic vector' basemap.
            Map = new Map(BasemapStyle.ArcGISTopographic);
        }

    }
}
