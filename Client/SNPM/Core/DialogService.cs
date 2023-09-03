using Microsoft.Extensions.DependencyInjection;
using SNPM.Core.Interfaces;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SNPM.Core
{
    public class DialogService : IDialogService
    {
        private readonly IServiceProvider serviceProvider;

        private IList<IDialog> activeDialogs;
        
        public DialogService(IServiceProvider serviceProvider)
        {
            activeDialogs = new List<IDialog>();
            this.serviceProvider = serviceProvider;
        }

        public async Task<bool> CreateErrorDialog(string MainMessage, ICollection<string> ErrorMessages)
        {
            string supportiveMessage = String.Empty;
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

        public Task<bool> CreateDialogWindow(string MainMessage, string SupportiveMessage, string AffirmativeMessage, string NegativeMessage)
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

            dialogViewModel.ShowView();

            return Task.FromResult(false);
        }
    }
}
