using Microsoft.Extensions.DependencyInjection;
using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.MVVM.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SNPM.Core.BusinessLogic
{
    public class DialogService : IDialogService
    {
        private readonly IServiceProvider serviceProvider;

        public DialogService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<bool> CreateErrorDialog(string MainMessage, ICollection<string> ErrorMessages)
        {
            string supportiveMessage = string.Empty;
            foreach (var error in ErrorMessages)
            {
                supportiveMessage += $"{error}{Environment.NewLine}";
            }
            string affirmativeMessage = "OK";

            return await CreateDialogWindow(MainMessage, supportiveMessage, affirmativeMessage);
        }

        public async Task<bool> CreateDialogWindow(string MainMessage, string SupportiveMessage, string AffirmativeMessage)
        {
            return await CreateDialogWindow(MainMessage, SupportiveMessage, AffirmativeMessage, string.Empty);
        }

        public async Task<bool> CreateDialogWindow(string MainMessage, string SupportiveMessage, string AffirmativeMessage, string NegativeMessage)
        {
            var dialogViewModel = serviceProvider.GetService<IDialogViewModel>();

            if (dialogViewModel == null)
            {
                throw new Exception("DialogViewModel not registered in ServiceProvider");
            }

            dialogViewModel.MainMessage = MainMessage;
            dialogViewModel.SupportiveMessage = SupportiveMessage;
            dialogViewModel.AffirmativeMessage = AffirmativeMessage;
            dialogViewModel.NegativeMessage = NegativeMessage;

            dialogViewModel.Initialize(false);
            dialogViewModel.ShowView();

            return await Task.FromResult(false);
        }

        public async Task<string> CreateFormWindow(string MainMessage, string AffirmativeMessage, string NegativeMessage)
        {
            var dialogViewModel = serviceProvider.GetService<IDialogViewModel>();

            if (dialogViewModel == null)
            {
                throw new Exception("DialogViewModel not registered in ServiceProvider");
            }

            var finished = new TaskCompletionSource<object>();

            dialogViewModel.MainMessage = MainMessage;
            dialogViewModel.AffirmativeMessage = AffirmativeMessage;
            dialogViewModel.NegativeMessage = NegativeMessage;

            dialogViewModel.Initialize(true, finished);

            dialogViewModel.ShowView();

            var res = await finished.Task;

            return (string)res;
        }
    }
}
