using MahApps.Metro.Controls;
using System.IO;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using AndroidToolkit.Infrastructure;
using System.Net;
using AndroidToolkit.Helpers;

namespace AndroidToolkit
{
    /// <summary>
    /// Interaction logic for DeviceConnection.xaml
    /// </summary>
    public partial class DeviceConnection : MetroWindow
    {
        public DeviceConnection()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _adb = new AdbOperations();
            _fastboot = new FastbootOperations();
        }
        private AdbOperations _adb;
        private FastbootOperations _fastboot;
        protected string _adbDevices;
        protected string _fastbootDevices;

        protected BackgroundWorker _bw;
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            _bw = new BackgroundWorker();
            _bw.DoWork += _bw_DoWork;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;
            _bw.RunWorkerAsync();

        }

        void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AdbDevices.Text = StringLinesRemover.ForgetLastLine(_adbDevices);
            FastbootDevices.Text = StringLinesRemover.ForgetLastLine(_fastbootDevices);
        }

        void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            _adbDevices = StringLinesRemover.RemoveLine(_adb.ListAdbDevices(false), 6);
            _fastbootDevices = StringLinesRemover.RemoveLine(_fastboot.ListFastbootDevices(false), 4);
        }

        private void DownloadDrivers_Click(object sender, RoutedEventArgs e)
        {
            bool isConn = ConnectionChecker.IsConnectedToInternet;
            if (isConn == true)
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri("http://download.clockworkmod.com/test/UniversalAdbDriverSetup.msi"), "UniversalAdbDriverSetup.msi");
                MessageBox.Show("Downloading drivers...");

            }
            else
            {
                MessageBox.Show("It seems like you are not connected to Internet. Contact your system admin or try again");
            }

        }
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress.Maximum = (int)e.TotalBytesToReceive / 100;
            DownloadProgress.Value = (int)e.BytesReceived / 100;
        }
        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                Process.Start("UniversalAdbDriverSetup.msi");
                MessageBox.Show("Drivers downloaded. Running setup ;)");
            }
            catch (Exception)
            {
                MessageBox.Show("An error has occured. Please try again :(");
            }


        }
        #region DumpException

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            DumpExceptionInfo(ex);
        }

        public static void DumpExceptionInfo(Exception ex)
        {
            string errorLog = string.Format("*** ERROR on {0} ***\n\n{1}\n\n*** ERROR ***\n\n*** INNER EXCEPTION ***\n\n{2}\n\n*** INNER EXCEPTION ***\n\n*** STACK TRACE ***\n\n{3}\n\n*** STACK TRACE ***\n\n",
            DateTime.Now, ex.Message, ex.InnerException, ex.StackTrace);
            FileStream stream = null;
            try
            {
                stream = new FileStream("ErrorLog.log", FileMode.Append, FileAccess.Write, FileShare.Read);
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    stream = null;
                    writer.Write(errorLog);
                }
            }
            finally
            {
                if (stream != null) stream.Dispose();
            }

        }
        #endregion
    }
}
