using SNPM.MVVM.Models.UiModels.Interfaces;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views.Interfaces;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace SNPM.MVVM.Views
{
    /// <summary>
    /// Interaction logic for DirectoryView.xaml
    /// </summary>
    public partial class DirectoryView : UserControl, IDirectoryView
    {
        public DirectoryView()
        {
            InitializeComponent();

            //DataContextChanged += (sender, args) =>
            //{
            //    IDirectoryViewModel viewModel = (IDirectoryViewModel)DataContext;
            //    viewModel.PropertyChanged += ViewModel_PropertyChanged;
            //};
        }

        //private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "RootNodes" && sender is IUiDirectory uiDirectory)
        //    {
                
        //    }
        //}

        private void DirectoryTreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is IDirectoryViewModel viewModel)
            {
                viewModel.SelectedNode = e.NewValue as IUiDirectory;
            }
        }

        private void TextBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.IsReadOnly = false;
                textBox.SelectAll();
            }
        }

        private void TextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.IsReadOnly = true;
            }
        }
    }
}
