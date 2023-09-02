using SNPM.Core.Interfaces;
using SNPM.MVVM.ViewModels;
using SNPM.MVVM.ViewModels.Interfaces;
using System.Threading.Tasks;

namespace SNPM.Core
{
    public class Dialog : IDialog
    {
        public string MainMessage { get; }

        public string SupportiveMessage { get; }

        public string NegativeActionMessage { get; }

        public string AffirmativeActionMessage { get; }

        private IDialogViewModel dialogViewModel;

        public Dialog(string MainMessage, string SupportiveMessage, string NegativeActionMessage, string AffirmativeActionMessage)
        {
            this.MainMessage = MainMessage;
            this.SupportiveMessage = SupportiveMessage;
            this.AffirmativeActionMessage = AffirmativeActionMessage;
            this.NegativeActionMessage = NegativeActionMessage;

            dialogViewModel = new DialogViewModel(MainMessage, SupportiveMessage, AffirmativeActionMessage, NegativeActionMessage);
        }
        public Task<bool> Show()
        {
            dialogViewModel.ShowView();

            return Task.FromResult(false);
        }
    }
}
