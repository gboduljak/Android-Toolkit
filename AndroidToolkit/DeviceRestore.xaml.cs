using System.IO;
using AndroidToolkit.Helpers;
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
using AndroidToolkit.Infrastructure;
using System.ComponentModel;
using MahApps.Metro;

namespace AndroidToolkit
{
    /// <summary>
    /// Interaction logic for DeviceRestore.xaml
    /// </summary>
    public partial class DeviceRestore : MetroWindow
    {
        public DeviceRestore()
        {
            InitializeComponent();
            _fastboot = new FastbootOperations();
            LightThemeSwitch.Checked += delegate
            {
                ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Light);
            };
            LightThemeSwitch.Unchecked += delegate
            {
                ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Dark);
            };
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        private bool _createWindow = true;
        private string _system;
        private string _userdata;
        private string _boot;
        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            _createWindow = false;
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            _createWindow = true;
        }

        private void LoadBoot_Click(object sender, RoutedEventArgs e)
        {
            _boot = Dialoger.ShowSingleChooseDialog(filter: "Android Image File (.img)|*.img");
        }
        private void LoadSystem_Click(object sender, RoutedEventArgs e)
        {
            _system = Dialoger.ShowSingleChooseDialog(filter: "Android Image File (.img)|*.img");
        }

        private void LoadUserdata_Click(object sender, RoutedEventArgs e)
        {
            _userdata = Dialoger.ShowSingleChooseDialog(filter: "Android Image File (.img)|*.img");
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_boot) && !string.IsNullOrEmpty(_system) && !string.IsNullOrEmpty(_userdata))
            {
                RestoreTask();
            }
            else
            {
                ErrorBlock.Text = "You must select all images for full restore. For Recovery use Recovery Master. Make sure all files are valid to avoid damage to device. Fingers crossed ;)";
                ShowErrorFlyout();
            }
        }

        private void ShowErrorFlyout()
        {
            var flyout = Flyouts.Items[0] as Flyout;
            flyout.IsOpen = !flyout.IsOpen;
        }

        private void RunFlyout(int index)
        {
            var flyout = (Flyout)Flyouts.Items[index];
            if (flyout != null)
            {
                flyout.IsOpen = !flyout.IsOpen;
            }
        }

        #region Tasks
        private void RestoreTask()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += async (s, e) =>
                {
                    FlashBoot();
                    FlashSystem();
                    FlashUserData();
                    await this.Dispatcher.InvokeAsync((Action)(() =>
                          {
                              RunFlyout(1);
                              RunFlyout(2);
                              RunFlyout(2);
                              RunFlyout(3);
                          }));
                    _fastboot.Reboot(_createWindow);
                    await this.Dispatcher.InvokeAsync((Action)(() =>
                    {
                        RunFlyout(3);
                        RunFlyout(4);
                    }));
                };
            RunFlyout(1);
            worker.RunWorkerAsync();

        }
     
        private void FlashBoot()
        {
            _fastboot.FlashBoot(_boot, _createWindow);
        }
        private void FlashSystem()
        {
            _fastboot.FlashSystem(_system, _createWindow);
        }
        private void FlashUserData()
        {
            _fastboot.FlashBoot(_userdata, _createWindow);
        }
        #endregion
        private readonly FastbootOperations _fastboot;
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
