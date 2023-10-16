using Microsoft.Extensions.DependencyInjection;
using SNPM.Core.Api.Interfaces;
using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.MVVM.Models.Interfaces;
using SNPM.MVVM.ViewModels;
using SNPM.MVVM.ViewModels.Interfaces;
using System;
using static SNPM.Core.BusinessLogic.Interfaces.IApplicationLogic;

namespace SNPM.Core.BusinessLogic
{
    public class ApplicationLogic : IApplicationLogic
    {
        private IMainViewModel mainViewModel;
        private ILoginViewModel loginViewModel;

        public event OptionChanged OnOptionChange;

        public ApplicationLogic(
            IGlobalVariables globalVariables,
            IServiceProvider serviceProvider,
            IKeySenderService keySenderService)
        {
            mainViewModel = serviceProvider.GetService<IMainViewModel>() ?? throw new Exception("ViewModel not registered");
            mainViewModel.SubscribeToPreferenceUpdate(OnPreferenceUpdate);

            loginViewModel = serviceProvider.GetService<ILoginViewModel>() ?? throw new Exception("LoginViewModel not registered");
            loginViewModel.LoginSuccessfulEvent += mainViewModel.ShowView;
            loginViewModel.LoginSuccessfulEvent += keySenderService.Initialize;
            loginViewModel.ShowView();

            OnExit = new Action(Shutdown);

            globalVariables.WindowHandle = mainViewModel.MainWindowHandle;
        }

        public Action OnExit { get; }

        private void Shutdown()
        {
            // TODO: (Przemek) Clear stuff before we exit
            System.Windows.Application.Current.Shutdown();
        }

        private void OnPreferenceUpdate(string PropertyName, object NewValue)
        {
            var option = Enum.Parse<ChangeableOption>(PropertyName);

            switch (option)
            {
                case ChangeableOption.TextSize:
                    OnOptionChange.Invoke(option, NewValue);
                    break;
                case ChangeableOption.DarkMode:

                    break;
                default:
                    throw new Exception("Option not handled");
            }
        }
    }
}
