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
    }
}
