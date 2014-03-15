using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
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
    /// Interaction logic for DeviceWindow.xaml
    /// </summary>
    public partial class DeviceWindow : MetroWindow
    {
        private string _adbDevices;
        private string _fastbootDevices;
        protected BackgroundWorker _bw;
        private readonly FastbootOperations _fastboot;
        private readonly AdbOperations _adb;
        private string _newAccentString = string.Empty;

        public DeviceWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _adb = new AdbOperations();
            _fastboot = new FastbootOperations();
            _bw = new BackgroundWorker();
            _bw.DoWork += _bw_DoWork;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;
            _bw.RunWorkerAsync();
        }

        private TextBox _adbDevicesBox;
        void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _adbDevicesBox = new TextBox()
                           {
                               Foreground = Brushes.Gray,
                               FontSize = 18
                           };
            _adbDevicesBox.Text = StringLinesRemover.ForgetLastLine(_adbDevices);
            AdbDevicesControl.Content = _adbDevicesBox;
            FastbootDevicesControl.Content = new TextBox().Text = StringLinesRemover.ForgetLastLine(_fastbootDevices);
        }
        void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            _adbDevices = StringLinesRemover.RemoveLine(_adb.ListAdbDevices(false), 5);
            _fastbootDevices = StringLinesRemover.RemoveLine(_fastboot.ListFastbootDevices(false), 4);
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_newAccentString))
            {
                ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == _newAccentString), Theme.Light);
            }
            else
            {
                ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Light);
            }
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_newAccentString))
            {
                ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == _newAccentString), Theme.Dark);
            }
            else
            {
                ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Dark);
            }
        }

        private void RefreshDevices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _bw = new BackgroundWorker();
                _bw.DoWork += _bw_DoWork;
                _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;
                _bw.RunWorkerAsync();
            }
            catch
            {
                MessageBox.Show("Wait.. Refreshing..");
            }

        }

        private void GetDeviceInfo_Click(object sender, RoutedEventArgs e)
        {
            _deviceID = DeviceID.Text;
            DeviceRoot.Visibility = Visibility.Collapsed;
            DeviceNoRoot.Visibility = Visibility.Collapsed;
            _infoWorker = new BackgroundWorker();
            _infoWorker.DoWork += _infoWorker_DoWork;
            _infoWorker.RunWorkerCompleted += _infoWorker_RunWorkerCompleted;
            _infoWorker.RunWorkerAsync();
        }

        void _infoWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetInfo();
        }

        protected void SetInfo()
        {
            DeviceName.Text = _deviceInfo.DeviceName;
            Style textblockStyle = Resources["TileTitle"] as Style;
            Style labelStyle = Resources["TileTitleLabel"] as Style;
            StackPanel panel = new StackPanel();
            panel.CanVerticallyScroll = true;
            panel.CanHorizontallyScroll = true;
            TextBlock infoBlock = new TextBlock()
            {
                Style = textblockStyle,
                TextWrapping = TextWrapping.Wrap
            };
            infoBlock.Text = StringLinesRemover.ForgetLastLine(_deviceInfo.OsVersion);
            infoBlock.Text = Regex.Replace(infoBlock.Text, @"^\s*$\n", string.Empty, RegexOptions.Multiline).TrimEnd();
            infoBlock.Text = infoBlock.Text + "\n" + _deviceInfo.OsVersionShort;
            infoBlock.Text = Regex.Replace(infoBlock.Text, @"^\s*$\n", string.Empty, RegexOptions.Multiline).TrimEnd();
            panel.Children.Add(infoBlock);
            DeviceOsContentControl.Content = panel;

            StackPanel panel2 = new StackPanel();
            panel2.CanVerticallyScroll = true;
            panel2.CanHorizontallyScroll = true;
            panel2.Children.Add(new Label() { Style = labelStyle, Content = "MANUFACTURER" });
            TextBlock infoBlock2 = new TextBlock()
            {
                Style = textblockStyle,
                TextWrapping = TextWrapping.Wrap
            };
            infoBlock2.Text = StringLinesRemover.ForgetLastLine(_deviceInfo.DeviceManufacturer);
            infoBlock2.Text = Regex.Replace(infoBlock2.Text, @"^\s*$\n", string.Empty, RegexOptions.Multiline).TrimEnd();
            panel2.Children.Add(infoBlock2);
            panel2.Children.Add(new Label() { Style = labelStyle, Content = "CODENAME" });
            TextBlock infoBlock3 = new TextBlock()
            {
                Style = textblockStyle,
                TextWrapping = TextWrapping.Wrap
            };
            infoBlock3.Text = StringLinesRemover.ForgetLastLine(_deviceInfo.DeviceCodename);
            panel2.Children.Add(infoBlock3);
            DeviceInfoContentControl.Content = panel2;

            Button hardwareBtn = new Button()
            {
                Width = 150,
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.Gray,
                Content = "VIEW HARDWARE"
            };
            hardwareBtn.Click += hardwareBtn_Click;
            DeviceHardwareContentControl.Content = hardwareBtn;
            if (!string.IsNullOrEmpty(_adbDevicesBox.Text))
            {
                if (_deviceInfo.IsRooted)
                {
                    DeviceRoot.Visibility = Visibility.Visible;
                    DeviceNoRoot.Visibility = Visibility.Collapsed;
                }
                else
                {
                    DeviceRoot.Visibility = Visibility.Collapsed;
                    DeviceNoRoot.Visibility = Visibility.Visible;
                }
            }

        }

        void hardwareBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectionChecker.IsConnectedToInternet)
            {
                Process.Start(string.Format("http://www.google.com/#q={0}", StringLinesRemover.ForgetLastLine(_deviceInfo.DeviceName)));
            }
        }

        void _infoWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _deviceInfo = _adb.DeviceDetailedInfo(false, _deviceID);
        }

        private string _deviceID;
        private DeviceInfo _deviceInfo;
        private BackgroundWorker _infoWorker;

        private void DownloadDrivers_OnClick(object sender, RoutedEventArgs e)
        {
            if (ConnectionChecker.IsConnectedToInternet)
            {
                WebClient downloader = new WebClient();
                downloader.DownloadProgressChanged += downloader_DownloadProgressChanged;
                downloader.DownloadFileCompleted += downloader_DownloadFileCompleted;
                DownloadProgress.Visibility = Visibility.Visible;
                downloader.DownloadFileAsync(new Uri("http://download.clockworkmod.com/test/UniversalAdbDriverSetup.msi"), "UniversalAdbDriverSetup.msi");
                this.ShowMessageAsync("DOWNLOADING", "Downloading drivers. Please wait");

            }
        }

        void downloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                DownloadProgress.Visibility = Visibility.Collapsed;
                Process.Start("UniversalAdbDriverSetup.msi");
                this.ShowMessageAsync("DRIVERS DOWNLOADED", null);
            }
            catch (Exception ex)
            {
                MetroMessageBoxHelper.ShowBox("ERROR", ex.ToString(), 400, 200);
            }

        }

        void downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress.Maximum = (int)e.TotalBytesToReceive / 100;
            DownloadProgress.Value = (int)e.BytesReceived / 100;
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
