using Microsoft.Extensions.DependencyInjection;
using SNPM.Core.Api;
using SNPM.Core.Interfaces;
using SNPM.Core.Interfaces.Api;
using SNPM.MVVM.Models;
using SNPM.MVVM.ViewModels;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views;
using SNPM.MVVM.Views.Interfaces;
using System;
using System.Windows;

namespace SNPM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IApplicationLogic? ApplicationLogic;
        private ServiceProvider ServiceProvider;

        public double TextSize;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            TextSize = 14;

            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            ApplicationLogic = ServiceProvider.GetService<IApplicationLogic>();
            if (ApplicationLogic != null)
            {
                ApplicationLogic.Initialize();
                ApplicationLogic.OnOptionChange += HandleOptionChange;
            }
            else
            {
                throw new Exception("ApplicationLogic is not registered");
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPasswordVerifier, PasswordVerifier>();
            services.AddSingleton<IMainView, MainView>();
            services.AddSingleton<IApplicationLogic, ApplicationLogic>();
            services.AddSingleton<IApiService, PlaceholderApiService>();
        }

        private void ConfigureViewModels(IServiceCollection services)
        {
            services.AddSingleton<IMainView, MainView>();
            services.AddSingleton<IMainViewModel, MainViewModel>();
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
