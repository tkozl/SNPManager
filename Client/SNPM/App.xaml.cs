﻿using Microsoft.Extensions.DependencyInjection;
using SNPM.Core.Api;
using SNPM.Core.Api.Interfaces;
using SNPM.Core.BusinessLogic;
using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.Core.Helpers;
using SNPM.Core.Helpers.Interfaces;
using SNPM.MVVM.Models;
using SNPM.MVVM.Models.Interfaces;
using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.ViewModels;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views;
using SNPM.MVVM.Views.Interfaces;
using System;
using System.Net;
using System.Windows;

namespace SNPM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IApplicationLogic? ApplicationLogic;
        private IServiceProvider ServiceProvider;

        public double TextSize;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            TextSize = 14;

            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            ConfigureViewModels(services);
            ConfigureViews(services);
            ServiceProvider = services.BuildServiceProvider();
            services.AddSingleton(ServiceProvider);

            ApplicationLogic = ServiceProvider.GetService<IApplicationLogic>();
            if (ApplicationLogic != null)
            {
                ApplicationLogic.OnOptionChange += HandleOptionChange;
            }
            else
            {
                throw new Exception("ApplicationLogic is not registered");
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IApplicationLogic, ApplicationLogic>();
            services.AddSingleton<IPasswordVerifierService, PasswordVerifierService>();
            services.AddSingleton<IApiService, ApiService>();
            services.AddSingleton<IProxyService, ProxyService>();
            services.AddSingleton<IAccountBlService, AccountBlService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IDirectoryBlService, DirectoryBlService>();
            services.AddSingleton<IRecordBlService, RecordBlService>();
            services.AddSingleton<IHotkeyService, HotkeyService>();
            services.AddSingleton<IKeySenderService, KeySenderService>();

            services.AddSingleton<IGlobalVariables, GlobalVariables>();
            services.AddTransient<IJsonHelper, JsonHelper>();
            services.AddSingleton<IChoiceItem, ChoiceItem>();
            services.AddSingleton<IAccountActivity, AccountActivity>();
            services.AddSingleton<IToken, Token>();
        }

        private void ConfigureViewModels(IServiceCollection services)
        {
            services.AddSingleton<IMainViewModel, MainViewModel>();
            services.AddSingleton<ILoginViewModel, LoginViewModel>();
            services.AddSingleton<IDialogViewModel, DialogViewModel>();
            services.AddSingleton<IRecordsViewModel, RecordsViewModel>();
            services.AddSingleton<IRecordFormViewModel, RecordFormViewModel>();
            services.AddSingleton<IPreferencesViewModel, PreferencesViewModel>();
            services.AddSingleton<IDirectoryViewModel, DirectoryViewModel>();
            services.AddSingleton<IChoiceViewModel, ChoiceViewModel>();
            services.AddSingleton<IActivityViewModel, ActivityViewModel>();
        }

        private void ConfigureViews(IServiceCollection services)
        {
            services.AddSingleton<IMainView, MainView>();
            services.AddSingleton<ILoginView, LoginView>();
            services.AddSingleton<IDialogView, DialogView>();
            services.AddSingleton<IDirectoryView, DirectoryView>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (ApplicationLogic != null)
            { 
                ApplicationLogic.OnExit();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void HandleOptionChange(ChangeableOption option, object value)
        {
            switch (option)
            {
                case ChangeableOption.TextSize:
                    ChangeTextSize(Convert.ToDouble(value));
                    break;
            }
        }

        private void ChangeTextSize(double newValue)
        {
            App.Current.Resources[ChangeableOption.TextSize.ToString()] = newValue;
        }
    }
}
