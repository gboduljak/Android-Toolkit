using System.Data.Entity;
using AndroidToolkit.Helpers;
using AndroidToolkit.Infrastructure;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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
using MahApps.Metro;


namespace AndroidToolkit
{
    /// <summary>
    /// Interaction logic for RecoveryMaster.xaml
    /// </summary>
    public partial class RecoveryMaster : MetroWindow
    {
        public RecoveryMaster()
        {
            InitializeComponent();
            _db = new AndroidToolkitDB();
            _fastboot = new FastbootOperations();
            BackgroundWorker loader = new BackgroundWorker();
            loader.DoWork += ((sender, args) =>
            {
                Load();
                //this.View.Dispatcher.BeginInvoke((Action)(() => Load()));
            });
            loader.RunWorkerCompleted += ((sender, args) =>
            {
                View.ItemsSource = _tempDevices;
                RunFlyout(0);
                RunFlyout(1);
            });
            RunFlyout(0);
            loader.RunWorkerAsync();

            View.SelectionChanged += ((sender, args) =>
            {
                FlashRecoveriesComboBox.Items.Clear();
                _device = (Device)View.SelectedItem;
                if (!string.IsNullOrEmpty(_device.CWMRecovery))
                {
                    FlashRecoveriesComboBox.Items.Add("CWM");
                }
                if (!string.IsNullOrEmpty(_device.CWMTouchRecovery))
                {
                    FlashRecoveriesComboBox.Items.Add("CWM Touch");
                }
                if (!string.IsNullOrEmpty(_device.TWRPRecovery))
                {
                    FlashRecoveriesComboBox.Items.Add("TWRP");
                }
            });

            LightThemeSwitch.Checked += delegate
            {
                ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Light);
            };
            LightThemeSwitch.Unchecked += delegate
            {
                ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Dark);
            };
        }


        AndroidToolkitDB _db;
        Device _device;
        readonly FastbootOperations _fastboot;
        string _currentRecovery;
        bool _createWindow = true;

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            _createWindow = false;
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            _createWindow = true;
        }


        private async void Flash_Click(object sender, RoutedEventArgs e)
        {
            _currentRecovery = (string)FlashRecoveriesComboBox.SelectedItem;
            if (!string.IsNullOrEmpty(_currentRecovery))
            {
                if (_currentRecovery == "CWM")
                {
                    bool isConn = ConnectionChecker.IsConnectedToInternet;
                    if (isConn == true)
                    {

                        WebClient cwmDownloader = new WebClient();
                        cwmDownloader.DownloadProgressChanged += cwmDownloader_DownloadProgressChanged;
                        cwmDownloader.DownloadFileCompleted += cwmDownloader_DownloadFileCompleted;
                        cwmDownloader.DownloadFileAsync(new Uri(_device.CWMRecovery), "recovery.img");
                        RunFlyout(2);
                    }
                    else
                    {
                        MetroMessageBoxHelper.ShowBox("Error", "It seems like you are not connected to Internet.\n Contact your system admin or try again", 225, 150);
                    }
                }
                else if (_currentRecovery == "CWM Touch")
                {

                    bool isConn = ConnectionChecker.IsConnectedToInternet;
                    if (isConn == true)
                    {

                        WebClient cwmTouchDownloader = new WebClient();
                        cwmTouchDownloader.DownloadProgressChanged += cwmtouchDownloader_DownloadProgressChanged;
                        cwmTouchDownloader.DownloadFileCompleted += cwmtouchDownloader_DownloadFileCompleted;
                        cwmTouchDownloader.DownloadFileAsync(new Uri(_device.CWMTouchRecovery), "recovery.img");
                        RunFlyout(2);
                    }
                    else
                    {
                        MetroMessageBoxHelper.ShowBox("Error", "It seems like you are not connected to Internet.\n Contact your system admin or try again", 225, 150);
                    }
                }
                else if (_currentRecovery == "TWRP")
                {
                    bool isConn = ConnectionChecker.IsConnectedToInternet;
                    if (isConn == true)
                    {

                        WebClient twrpDownloader = new WebClient();
                        twrpDownloader.DownloadProgressChanged += twrpDownloader_DownloadProgressChanged;
                        twrpDownloader.DownloadFileCompleted += twrpDownloader_DownloadFileCompleted;
                        twrpDownloader.DownloadFileAsync(new Uri(_device.TWRPRecovery), "recovery.img");
                        RunFlyout(2);
                    }
                    else
                    {
                        MetroMessageBoxHelper.ShowBox("Error", "It seems like you are not connected to Internet.\n Contact your system admin or try again", 225, 150);
                    }
                }
                else
                {
                    MessageBox.Show("Unknown error");
                }
            }
            else
            {
                await this.ShowMessageAsync("YOU MUST SELECT RECOVERY", null);
            }

        }

        // CWM WORKER
        void cwmDownloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            RunFlyout(2);
            RunFlyout(3);
            RunFlyout(3);
            RunFlyout(4);
            _flashRecoveryWorker = new BackgroundWorker();
            _flashRecoveryWorker.DoWork += _flashRecoveryWorker_DoWork;
            _flashRecoveryWorker.RunWorkerCompleted += _flashRecoveryWorker_RunWorkerCompleted;
            _flashRecoveryWorker.RunWorkerAsync();


        }
        void cwmDownloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress.Maximum = (int)e.TotalBytesToReceive / 100;
            DownloadProgress.Value = (int)e.BytesReceived / 100;
            ProgressText.Text = string.Format("{0} %", (int)e.ProgressPercentage);
        }
        private BackgroundWorker _flashRecoveryWorker;
        // END CWM WORKER

        // CWM TOUCH WORKER
        void cwmtouchDownloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            RunFlyout(2);
            RunFlyout(3);
            RunFlyout(3);
            RunFlyout(4);
            _flashRecoveryWorker = new BackgroundWorker();
            _flashRecoveryWorker.DoWork += _flashRecoveryWorker_DoWork;
            _flashRecoveryWorker.RunWorkerCompleted += _flashRecoveryWorker_RunWorkerCompleted;
            _flashRecoveryWorker.RunWorkerAsync();
        }
        void cwmtouchDownloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress.Maximum = (int)e.TotalBytesToReceive / 100;
            DownloadProgress.Value = (int)e.BytesReceived / 100;
            ProgressText.Text = string.Format("{0} %", (int)e.ProgressPercentage);
        }
        // END CWM TOUCH WORKER

        // FLASH WORKER

        // TWRP WORKER
        void twrpDownloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            RunFlyout(2);
            RunFlyout(3);
            RunFlyout(3);
            RunFlyout(4);
            _flashRecoveryWorker = new BackgroundWorker();
            _flashRecoveryWorker.DoWork += _flashRecoveryWorker_DoWork;
            _flashRecoveryWorker.RunWorkerCompleted += _flashRecoveryWorker_RunWorkerCompleted;
            _flashRecoveryWorker.RunWorkerAsync();
        }
        void twrpDownloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress.Maximum = (int)e.TotalBytesToReceive / 100;
            DownloadProgress.Value = (int)e.BytesReceived / 100;
            ProgressText.Text = string.Format("{0} %", (int)e.ProgressPercentage);
        }
        // END TWRP WORKER
        async void _flashRecoveryWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RunFlyout(4);
            RunFlyout(5);
            File.Delete("recovery.img");
            await Task.Factory.StartNew(() => _fastboot.RebootRecovery(_createWindow));
            RunFlyout(5);
            _flashRecoveryWorker.Dispose();
        }
        void _flashRecoveryWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _fastboot.FlashRecovery("recovery.img", _createWindow);
        }
        // END FLASH WORKER

        #region Reboot
        private void RebootBootloader_Click(object sender, RoutedEventArgs e)
        {
            _rebootBootloader = new BackgroundWorker();
            _rebootBootloader.DoWork += _rebootBootloader_DoWork;
            _rebootBootloader.RunWorkerCompleted += _rebootBootloader_RunWorkerCompleted;
            _rebootBootloader.RunWorkerAsync();
        }

        void _rebootBootloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _rebootBootloader.Dispose();
        }

        void _rebootBootloader_DoWork(object sender, DoWorkEventArgs e)
        {
            _fastboot.RebootBootloader(_createWindow);
        }
        private BackgroundWorker _rebootBootloader;
        private void RebootRecovery_Click(object sender, RoutedEventArgs e)
        {
            _rebootRecovery = new BackgroundWorker();
            _rebootRecovery.DoWork += _rebootRecovery_DoWork;
            _rebootRecovery.RunWorkerCompleted += _rebootRecovery_RunWorkerCompleted;
            _rebootRecovery.RunWorkerAsync();
        }

        void _rebootRecovery_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _rebootRecovery.Dispose();
        }

        void _rebootRecovery_DoWork(object sender, DoWorkEventArgs e)
        {
            _fastboot.RebootRecovery(_createWindow);
        }
        private BackgroundWorker _rebootRecovery;
        #endregion


        #region Tasks

        private async void Load()
        {
            _tempDevices = await _db.Devices.ToListAsync();
        }

        private void RunFlyout(int index)
        {
            var flyout = (Flyout)Flyouts.Items[index];
            if (flyout != null)
            {
                flyout.IsOpen = !flyout.IsOpen;
            }
        }
        private IEnumerable<Device> _tempDevices;
        #endregion;

    }
}
