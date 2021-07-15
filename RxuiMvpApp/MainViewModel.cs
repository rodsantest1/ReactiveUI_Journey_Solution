using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace RxuiMvpApp
{
    public class MainViewModel : ReactiveObject
    {
        [Reactive] public double ZoomLevel { get; set; }
    }
}
