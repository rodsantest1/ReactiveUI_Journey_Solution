using Esri.ArcGISRuntime.Mapping;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace RxuiMvpApp
{
    public class MainViewModel : ReactiveObject
    {
        [Reactive] public Map Map { get; set; }
        [Reactive] public double ZoomLevel { get; set; }

        public MainViewModel()
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
