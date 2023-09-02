using SNPM.Core;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    public interface IDialogViewModel : IViewModel
    {
        string MainMessage { get; set; }
        string SupportiveMessage { get; set; }
        string AffirmativeMessage { get; set; }
        string NegativeMessage { get; set; }
        ICommand AffirmativeCommand { get; }
        ICommand NegativeCommand { get; }
        bool Result { get; }
    }
}
