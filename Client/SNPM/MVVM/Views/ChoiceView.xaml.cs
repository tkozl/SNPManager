using SNPM.MVVM.ViewModels.Interfaces;
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

namespace SNPM.MVVM.Views
{
    /// <summary>
    /// Interaction logic for ChoiceView.xaml
    /// </summary>
    public partial class ChoiceView : Window
    {
        public ChoiceView()
        {
            InitializeComponent();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is IChoiceViewModel vm)
            {
                vm.MakeChoiceCommand.Execute(null);
            }
        }
    }
}
