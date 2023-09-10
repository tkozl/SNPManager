using SNPM.MVVM.Models.UiModels.Interfaces;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views.Interfaces;
using System.Windows.Controls;

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
        }

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

                //if (DataContext is IDirectoryViewModel viewModel)
                //{
                //    viewModel.RenameDirectoryCommand.Execute(null);
                //}
            }
        }
    }
}
