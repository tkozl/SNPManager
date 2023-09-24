using SNPM.Core;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views;
using System.Windows;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels
{
    public class DialogViewModel : IDialogViewModel
    {
        public Window DialogView { get; }
        public ICommand AffirmativeCommand { get; set; }
        public ICommand NegativeCommand { get; set; }
        public bool Result => result;

        public string MainMessage { get; set; }
        public string SupportiveMessage { get; set; }
        public string AffirmativeMessage { get; set; }
        public string NegativeMessage { get; set; }

        private bool result;
        
        public DialogViewModel()
        {
            AffirmativeCommand = new RelayCommand((_) =>
            {
                result = true;
                HideView();
            });

            NegativeCommand = new RelayCommand((_) =>
            {
                result = false;
                HideView();
            });

            DialogView = new DialogView()
            {
                DataContext = this,
            };
        }

        public void HideView()
        {
            DialogView.Hide();
        }

        public void ShowView()
        {
            DialogView.Show();
        }
    }
}
