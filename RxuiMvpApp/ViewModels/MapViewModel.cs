using Esri.ArcGISRuntime.Mapping;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace RxuiMvpApp.ViewModels
{
    public class MapViewModel : ReactiveObject
    {
        [Reactive] public Map Map { get; set; }
        [Reactive] public double ZoomLevel { get; set; }

        public MapViewModel()
        {
            SetupMap();
        }

        private void SetupMap()
        {
            // Create a new map with a 'topographic vector' basemap.
            Map = new Map(BasemapStyle.ArcGISTopographic);
        }
    }
}
