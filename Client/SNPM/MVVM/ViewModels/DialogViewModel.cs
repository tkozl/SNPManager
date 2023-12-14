using SNPM.Core;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels
{
    public class DialogViewModel : IDialogViewModel
    {
        private object result;
        private TaskCompletionSource<object> finished;

        public bool InputRequired { get; set; }

        public Window DialogView { get; }

        public ICommand AffirmativeCommand { get; set; }

        public ICommand NegativeCommand { get; set; }

        public object Result { get => result; private set => result = value; }

        public string FormText { get; set; }

        public string MainMessage { get; set; }

        public string SupportiveMessage { get; set; }

        public string AffirmativeMessage { get; set; }

        public string NegativeMessage { get; set; }

        public DialogViewModel()
        {
            DialogView = new DialogView()
            {
                DataContext = this,
            };

            FormText = string.Empty;
        }

        public void HideView()
        {
            DialogView.Hide();
        }

        public void ShowView()
        {
            DialogView.Show();
        }

        public void Initialize(bool InputRequired, TaskCompletionSource<object> finished=null)
        {
            this.InputRequired = InputRequired;
            this.finished = finished;

            if (this.InputRequired)
            {
                InitializeForm();
            }
            else
            {
                InitalizeYesNoDialog();
            }
        }

        private void InitializeForm()
        {
            AffirmativeCommand = new RelayCommand((_) =>
            {
                finished.TrySetResult(FormText);
                HideView();
            }, (_) =>
            {
                return FormText.Length > 0;
            });

            NegativeCommand = new RelayCommand((_) =>
            {
                finished.TrySetResult(FormText);
                HideView();
            });
        }

        private void InitalizeYesNoDialog()
        {
            AffirmativeCommand = new RelayCommand((_) =>
            {
                Result = true;
                HideView();
            });

            NegativeCommand = new RelayCommand((_) =>
            {
                Result = false;
                HideView();
            });
        }
    }
}
