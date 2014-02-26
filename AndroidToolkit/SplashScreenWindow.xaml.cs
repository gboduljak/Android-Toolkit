using AndroidToolkit.Infrastructure;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AndroidToolkit
{
    /// <summary>
    /// Interaction logic for SplashScreenWindow.xaml
    /// </summary>
    public partial class SplashScreenWindow : MetroWindow
    {
        public SplashScreenWindow()
        {
            InitializeComponent();
            BackgroundWorker loader = new BackgroundWorker();
            loader.DoWork += ((sender, args) =>
            {
                new AdbOperations().ListAdbDevices(false);
                new AdbOperations().ListAdbDevices(false);
                new FastbootOperations().ListFastbootDevices(false);
                Thread.Sleep(2000);
            });
            loader.RunWorkerCompleted += ((sender, args) =>
            {
                new MainWindow().Show();
                this.Close();
            });
            loader.RunWorkerAsync();
        }
    }
}
