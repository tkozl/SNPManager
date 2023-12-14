using SNPM.Core;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    public interface IDialogViewModel : IViewModel
    {
        bool InputRequired { get; set; }

        string MainMessage { get; set; }

        string SupportiveMessage { get; set; }

        string AffirmativeMessage { get; set; }

        string NegativeMessage { get; set; }

        ICommand AffirmativeCommand { get; }

        ICommand NegativeCommand { get; }

        object Result { get; }

        string FormText { get; set; }

        void Initialize(bool InputRequired, TaskCompletionSource<object> finished=null);
    }
}
