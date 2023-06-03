using SNPM.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Logika interakcji dla klasy PreferencesView.xaml
    /// </summary>
    public partial class PreferencesView : Window
    {
        public PreferencesView()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            if (DataContext is PreferencesViewModel vm)
            {
                vm.CloseAction();
            }
            else
            {
                throw new Exception("ViewModel is not PreferencesViewModel");
            }
        }
    }
}
