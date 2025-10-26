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
            var authWindow = new AuthWindow();
            bool? result = authWindow.ShowDialog();
            if (result == true && CurrentUser != null)
            {
                var mainWindow = new MainWindow();
                this.MainWindow = mainWindow;
                mainWindow.Show();
            }
            else
            {
                Shutdown();
            }
        }
    }
}
