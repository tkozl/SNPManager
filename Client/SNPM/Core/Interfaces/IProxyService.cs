using SNPM.MVVM.Models.UiModels.Interfaces;
using System.Threading.Tasks;

namespace SNPM.Core.Interfaces
{
    public interface IProxyService
    {
        Task CreateAccount(IUiAccount account);

        Task Login(IUiAccount uiAccount);
    }
}