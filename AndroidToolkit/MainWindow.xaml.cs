using AndroidToolkit.Helpers;
using AndroidToolkit.Infrastructure;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using MahApps.Metro.Controls.Dialogs;

namespace AndroidToolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _adb = new AdbOperations();
            _fastboot = new FastbootOperations();
     
        }



        #region DevicesBackgroundWorker
        protected BackgroundWorker _bw;
        private string _adbDevices;
        private string _fastbootDevices;
        private readonly AdbOperations _adb;
        private readonly FastbootOperations _fastboot;
        void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AdbDevices.Content = new TextBox().Text = StringLinesRemover.ForgetLastLine(_adbDevices);
            FastbootDevices.Content = new TextBox().Text = StringLinesRemover.ForgetLastLine(_fastbootDevices);
        }
        void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            _adbDevices = StringLinesRemover.RemoveLine(_adb.ListAdbDevices(false), 4);
            _fastbootDevices = StringLinesRemover.RemoveLine(_fastboot.ListFastbootDevices(false), 4);
        }
        #endregion

        #region Tasks

        private async Task<string> Changelog()
        {
            return await Task.Run(() => File.ReadAllText("changelog.log"));
        }
        private async Task<string> ErrorLog()
        {
            if (File.Exists("ErrorLog.log"))
            {
                return await Task.Run(() => File.ReadAllText("ErrorLog.log"));
            }
            return null;
        }

        #endregion

        protected void ShowFlyout(int index)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;
        }

        private void RefreshDevices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _bw.RunWorkerAsync();
            }
            catch
            {
                MessageBox.Show("Wait.. Refreshing..");
            }

        }

        private async void Disclaimer_OnClick(object sender, RoutedEventArgs e)
        {
            await
                this.ShowMessageAsync("DISCLAIMER",
                    "I am not responsible for any damage to your device. Everything you do is on your own risk. Some features can void warranty.");
        }

        private async void About_Click(object sender, RoutedEventArgs e)
        {
            ChangelogText.Text = await Changelog();
            ShowFlyout(0);
        }

        private void CheckForUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateCheck();
        }

        private async void UpdateCheck()
        {
            if (ConnectionChecker.IsConnectedToInternet)
            {
                ShowFlyout(1);
                bool isUpdateAvaliable = await UpdateManager.IsUpdateAvailable();
                ShowFlyout(1);
                if (isUpdateAvaliable)
                {

                    ShowFlyout(3);
                    bool success = await UpdateManager.Update();
                    if (success)
                    {
                        await this.ShowMessageAsync("UPDATE SUCCESSFUL", null);
                        ShowFlyout(3);
                    }
                    else
                    {
                        await this.ShowMessageAsync("AN ERROR HAS OCCURED", null);
                        ShowFlyout(3);
                    }
                }
                await this.ShowMessageAsync("APP DB IS ON LATEST VERSION", null);
            }
            else
            {
                await this.ShowMessageAsync("UPDATE DB CHECK FAIL", "YOU ARE NOT CONNECTED TO INTERNET");
            }



        }

        private async void ShowErrorLog_OnClick(object sender, RoutedEventArgs e)
        {
            string errorLog = await ErrorLog();
            if (errorLog.Length != 0)
            {
                MetroMessageBoxHelper.ShowBox("ERROR LOG", errorLog, 500, 250);
            }
            else
            {
                MetroMessageBoxHelper.ShowBox("EXCELLENT", "This app doesn't have errors. :)", 250, 125);
            }
        }

        #region Open
        private void OpenADB(object sender, RoutedEventArgs e)
        {
            _AdbTools = new ADBTools();
            _AdbTools.Show();
        }

        protected ADBTools _AdbTools;

        private void OpenFastboot(object sender, RoutedEventArgs e)
        {
            _FastbootTools = new FastbootTools();
            _FastbootTools.Show();
        }

        protected FastbootTools _FastbootTools;

        private void OpenAdvancedClick(object sender, RoutedEventArgs e)
        {
            _AdvancedTools = new AdvancedTools();
            _AdvancedTools.Show();
        }
        protected AdvancedTools _AdvancedTools;

        private void OpenAdmin(object sender, RoutedEventArgs e)
        {
            new AdminLoger().Show();
        }
        private void OpenDeviceConnection(object sender, RoutedEventArgs e)
        {
            new DeviceWindow().Show();
        }

        private void ReportBugTile_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectionChecker.IsConnectedToInternet)
            {
                new ReportBugInterop().ShowDialog();
            }
            else
            {
                MetroMessageBoxHelper.ShowBox("ERROR", "You are not connected to internet", 400, 150);
            }

        }

        private void OpenRecoveryMaster_Click(object sender, RoutedEventArgs e)
        {
            new RecoveryMaster().Show();
        }

        private void OpenDeviceRestore_Click(object sender, RoutedEventArgs e)
        {
            new DeviceRestore().Show();
        }

        private void Help_OnClick(object sender, RoutedEventArgs e)
        {
            new Help().Show();
        }
        #endregion

        #region Intro
        private void ShowIntro()
        {
            var flyout = Flyouts.Items[2] as Flyout;
            if (flyout != null) flyout.IsOpen = !flyout.IsOpen;
            File.WriteAllText("ShowNoIntro.shownointro", "True");
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _bw = new BackgroundWorker();
            _bw.DoWork += _bw_DoWork;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;
            _bw.RunWorkerAsync();
            if (!File.Exists("ShowNoIntro.shownointro"))
            {
                ShowIntro();
            }
        }

        #endregion

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
