using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Windows;

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

            this.WhenActivated(disposables =>
            {
                this.BindCommand(ViewModel, vm => vm.TestCommand, v => v.SliderInput1, nameof(SliderInput1.ValueChanged));

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
