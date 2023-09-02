using System.Threading.Tasks;

namespace SNPM.Core.Interfaces
{
    public interface IDialogService
    {
        Task<bool> CreateDialogWindow(string MainMessage, string SupportiveMessage, string AffirmativeActionMessage, string NegativeActionMessage);

        Task<bool> CreateDialogWindow(string MainMessage, string SupportiveMessage, string AffirmativeActionMessage);
    }
}
