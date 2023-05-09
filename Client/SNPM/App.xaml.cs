﻿using SNPM.MVVM.Models;
using System;
using System.Windows;

namespace SNPM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var appLogic = new ApplicationLogic();
            appLogic.Initialize();
        }
    }
}
