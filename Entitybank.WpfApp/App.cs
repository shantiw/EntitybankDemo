using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using XData.Data.Diagnostics;
using XData.Client.Models;
using XData.Windows.Interactivity;
using XData.Windows.ViewModels;
using XData.Windows.Views;

namespace XData.Windows.Client
{
    public class App : Application
    {
        [STAThread]
        static void Main()
        {
            Window loginWindow = new LoginWindow();
            ViewModel loginViewModel = new LoginViewModel();
            loginWindow.DataContext = loginViewModel;
            loginViewModel.View = loginWindow;
            if (loginWindow.ShowDialog() != true)
            {
                return;
            }

            MainWindow mainWindow = new MainWindow();
            ViewModel viewModel = new MainViewModel();
            mainWindow.DataContext = viewModel;
            viewModel.View = mainWindow;

            App app = new App();
            app.Startup += app.App_Startup;
            app.DispatcherUnhandledException += app.App_DispatcherUnhandledException;
            app.Exit += app.App_Exit;

            app.Run(mainWindow);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            Log.Error(ex);
            ShowMessageBox.Show(ex.Message, this.MainWindow);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Log.Error(ex);
            ShowMessageBox.Show(ex.Message, this.MainWindow);

            e.Handled = true;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            AppSecurity.Logout();
        }


    }
}
