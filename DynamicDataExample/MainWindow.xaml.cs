using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DynamicDataExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SourceList<MyClass> myClasses = new SourceList<MyClass>();

        public MainWindow()
        {
            InitializeComponent();

            ReadOnlyObservableCollection<MyClass> data;


            myClasses.Add(new MyClass() { Id = 1, Name = "Class1" });

            var myObservableMyClasses = myClasses.Connect();

            myObservableMyClasses
                .Bind(out data)
                .DisposeMany()
                .Subscribe();

            DataGrid1.ItemsSource = data;

            Loaded += MainWindow_Loaded;
        }   

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(5000);
            myClasses.Add(new MyClass() { Id = 2, Name = "Class2" });
        }
    }

    public class MyClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
