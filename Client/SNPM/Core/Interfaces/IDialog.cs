using System.Threading.Tasks;

namespace SNPM.Core.Interfaces
{
    public interface IDialog
    {
        string MainMessage { get; }

        string SupportiveMessage { get; }

        string NegativeActionMessage { get; }

        string AffirmativeActionMessage { get; }

        Task<bool> Show();
    }
}
