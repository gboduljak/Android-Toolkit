using AndroidToolkit.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AndroidToolkit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Startup
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _prepare = new BackgroundWorker();
            _prepare.DoWork += _prepare_DoWork;
            _prepare.RunWorkerAsync();
        }

        void _prepare_DoWork(object sender, DoWorkEventArgs e)
        {
            new AdbOperations().ListAdbDevices(false);
            new FastbootOperations().ListFastbootDevices(false);
        }
        private BackgroundWorker _prepare;
        #endregion
    }
}
