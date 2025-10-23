using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EpicGamesLauncher
{
    public partial class App : Application
    {
        public static Models.User CurrentUser { get; set; }

        protected void ApplicationStart(object sender, StartupEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ApplicationStart called");

            var authWindow = new AuthWindow();
            bool? result = authWindow.ShowDialog();

            System.Diagnostics.Debug.WriteLine($"Dialog result: {result}, User: {CurrentUser?.Username}");

            if (result == true && CurrentUser != null)
            {
                System.Diagnostics.Debug.WriteLine("Opening MainWindow...");

                var mainWindow = new MainWindow();
                this.MainWindow = mainWindow;
                mainWindow.Show();

                System.Diagnostics.Debug.WriteLine("MainWindow should be visible now");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Authentication failed - shutting down");
                Shutdown();
            }
        }
    }
}
