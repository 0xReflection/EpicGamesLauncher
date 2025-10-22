using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EpicGamesLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected void ApplicationStart(object sender, StartupEventArgs e)
        {
            var authUser = new AuthWindow();
            authUser.Show();
            authUser.IsVisibleChanged += (s, ev) =>
            {
                if (authUser.IsVisible == false && authUser.IsLoaded)
                {
                    var mainWin= new MainWindow();
                    mainWin.Show();
                    authUser.Close();
                }
            };
        }
    }
}
