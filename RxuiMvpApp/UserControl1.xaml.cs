using ReactiveUI;
using System;
using System.Collections.Generic;
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

namespace RxuiMvpApp
{
    // The class derives off ReactiveUserControl which contains the ViewModel property.
    // In our MainWindow when we register the ListBox with the collection of 
    // NugetDetailsViewModels if no ItemTemplate has been declared it will search for 
    // a class derived off IViewFor<NugetDetailsViewModel> and show that for the item.
    public partial class NugetDetailsView : ReactiveUserControl<MapViewModel>
    {
        public NugetDetailsView()
        {
            this.WhenActivated(disposableRegistration =>
            {
                // Our 4th parameter we convert from Url into a BitmapImage. 
                // This is an easy way of doing value conversion using ReactiveUI binding.
                //this.OneWayBind(ViewModel,
                //    viewModel => viewModel.IconUrl,
                //    view => view.iconImage.Source,
                //    url => url == null ? null : new BitmapImage(url))
                //    .DisposeWith(disposableRegistration);

                //this.OneWayBind(ViewModel,
                //    viewModel => viewModel.Title,
                //    view => view.titleRun.Text)
                //    .DisposeWith(disposableRegistration);

                //this.OneWayBind(ViewModel,
                //    viewModel => viewModel.Description,
                //    view => view.descriptionRun.Text)
                //    .DisposeWith(disposableRegistration);

                //this.BindCommand(ViewModel,
                //    viewModel => viewModel.OpenPage,
                //    view => view.openButton)
                //    .DisposeWith(disposableRegistration);
            });
        }
    }
}
