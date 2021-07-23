using Esri.ArcGISRuntime.Mapping;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
