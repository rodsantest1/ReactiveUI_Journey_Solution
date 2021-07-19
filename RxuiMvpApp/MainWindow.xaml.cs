using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
                var zoomIn = Observable.FromEventPattern(
                    h => ZoomInButton.Click += new RoutedEventHandler(h),
                    h => ZoomInButton.Click -= new RoutedEventHandler(h));

                Observable.Merge(
                    zoomIn.Select(_ => 42)
                ).Subscribe(x =>
                {
                    System.Diagnostics.Debug.WriteLine($"hello {x}");
                    statusMsg.Text = $"The answer is {x}.";
                });
            });
        }
    }
}
