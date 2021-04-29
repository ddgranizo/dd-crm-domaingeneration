using ModelUI.Models;
using ModelUI.Viewmodels;
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
using System.Windows.Shapes;

namespace ModelUI.Views
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class DomainManager : Window
    {
        private DomainManagerViewmodel _viewModel;
        public DomainManager()
        {
            InitializeComponent();
            this._viewModel = LayoutRoot.Resources["viewModel"] as DomainManagerViewmodel;
            _viewModel.Initialize(this);
        }

        private void DomainsList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = DomainsScrollViewer;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
