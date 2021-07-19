using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Windows.Input;

namespace RxuiMvpApp
{
    public class MainViewModel : ReactiveObject
    {
        public ICommand TestCommand { get; }
        [Reactive] public double ZoomLevel { get; set; }

        public MainViewModel()
        {
            //TestCommand = ReactiveCommand.Create(() =>
            //{
            //    System.Diagnostics.Debug.WriteLine("Command executed.");
            //});
        }
    }
}
