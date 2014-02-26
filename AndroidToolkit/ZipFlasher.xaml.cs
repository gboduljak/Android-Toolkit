using System.ComponentModel;
using System.IO;
using System.Reflection;
using AndroidToolkit.Helpers;
using AndroidToolkit.Infrastructure;
using MahApps.Metro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;

namespace AndroidToolkit
{
    /// <summary>
    /// Interaction logic for ZipFlasher.xaml
    /// </summary>
    public partial class ZipFlasher : MetroWindow
    {
        private readonly FastbootOperations _fastboot;
        public ZipFlasher()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _fastboot = new FastbootOperations();
        }


        private void Flash_Click(object sender, RoutedEventArgs e)
        {
            if (_flashZip.Length != 0)
            {
                _flasher = new BackgroundWorker();
                _flasher.DoWork += _flasher_DoWork;
                _flasher.RunWorkerCompleted += _flasher_RunWorkerCompleted;
                _flasher.RunWorkerAsync();
            }
            else
            {
                this.ShowMessageAsync("ERROR", "You must select .zip to flash");
            }

        }

        void _flasher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.ShowMessageAsync("FINISHED", null);
            _flasher.Dispose();
        }

        void _flasher_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.FlashZips(_createWindow, _flashZip);
        }

        private void Choose_Click(object sender, RoutedEventArgs e)
        {
            _flashZip = Dialoger.ShowSingleChooseDialog(filter: "Android Zip file (.zip)|*.zip");
        }

        private BackgroundWorker _flasher;
        private string _flashZip = string.Empty;
        private bool _createWindow = true;
        private void Reboot_OnClick(object sender, RoutedEventArgs e)
        {
            _rebootWorker = new BackgroundWorker();
            _rebootWorker.DoWork += _rebootWorker_DoWork;
            _rebootWorker.RunWorkerAsync();
        }
        void _rebootWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _fastboot.Reboot(_createWindow);
        }

        private BackgroundWorker _rebootWorker;

        private void ShowCMD_OnChecked(object sender, RoutedEventArgs e)
        {
            _createWindow = false;
        }

        private void ShowCMD_OnUnchecked(object sender, RoutedEventArgs e)
        {
            _createWindow = true;
        }

        private void ChangeTheme_OnChecked(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Light);
        }

        private void ChangeTheme_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Dark);
        }

        private void Settings_OnClick(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)Flyouts.Items[0];
            flyout.IsOpen = !flyout.IsOpen;
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
