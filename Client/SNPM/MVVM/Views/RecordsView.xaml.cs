using SNPM.MVVM.ViewModels.Interfaces;
using System.Windows.Controls;
using System.Windows.Input;

namespace SNPM.MVVM.Views
{
    /// <summary>
    /// Logika interakcji dla klasy RecordsView.xaml
    /// </summary>
    public partial class RecordsView : UserControl
    {
        public RecordsView()
        {
            InitializeComponent();
        }

        private void RecordsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext is IRecordsViewModel recordsViewModel && recordsViewModel.SelectedRecord != null)
            {
                recordsViewModel.ModifyRecordCommand.Execute(recordsViewModel.SelectedRecord);
            }
        }
    }
}
