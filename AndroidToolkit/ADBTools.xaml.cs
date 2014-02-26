using MahApps.Metro.Controls;
using MahApps.Metro;
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
using System.ComponentModel;
using AndroidToolkit.Infrastructure;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using AndroidToolkit.Helpers;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;
using System.Threading;
namespace AndroidToolkit
{
    /// <summary>
    /// Interaction logic for ADBTools.xaml
    /// </summary>
    public partial class ADBTools : MetroWindow
    {
        public ADBTools()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _adb = new AdbOperations();
            _pushFiles = new List<string>();
            _pullFiles = new List<string>();
            LoadAccents();
            RemoteADBHelp.Click += ((sender, args) =>
                {
                    new Help().Show();
                });
            RefreshIPButton.Click += delegate
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += delegate
                {
                    LoadIPs();
                };
                worker.RunWorkerAsync();
            };
            BackgroundWorker worker2 = new BackgroundWorker();
            worker2.DoWork += delegate
            {
                LoadIPs();
            };
            worker2.RunWorkerAsync();
            DeleteIPsButton.Click += async delegate
            {
                await DeleteIPs();
                LoadIPs();
            };
        }

        private bool createWindow = true;
        private string _target = string.Empty;
        private string _targetDeviceIP = string.Empty;
        private string _targetDevicePort = string.Empty;
        private AdbOperations _adb;

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

            //var brush = (Brush)(new BrushConverter().ConvertFrom("#FF41B1E1"));
            //DeviceMenu.Background = brush;
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
            //var brush = (Brush)(new BrushConverter().ConvertFrom("#FF363636"));
            //DeviceMenu.Background = brush;
        }

        private void CreateWindowCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            createWindow = false;
        }

        private void CreateWindowCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            createWindow = true;
        }




        #region Reboot and Execute
        private void RebootButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker reboot = new BackgroundWorker();
            reboot.DoWork += reboot_DoWork;
            reboot.RunWorkerCompleted += reboot_RunWorkerCompleted;
            reboot.RunWorkerAsync();
        }
        void reboot_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }
        void reboot_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.Reboot(createWindow, _target);
        }

        private void RebootRecoveryButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker rebootRecovery = new BackgroundWorker();
            rebootRecovery.DoWork += rebootRecovery_DoWork;
            rebootRecovery.RunWorkerCompleted += rebootRecovery_RunWorkerCompleted;
            rebootRecovery.RunWorkerAsync();
        }
        void rebootRecovery_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }
        void rebootRecovery_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.RebootRecovery(createWindow, _target);
        }

        private void RebootBootloaderButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker rebootBootloader = new BackgroundWorker();
            rebootBootloader.DoWork += rebootBootloader_DoWork;
            rebootBootloader.RunWorkerCompleted += rebootBootloader_RunWorkerCompleted;
            rebootBootloader.RunWorkerAsync();
        }
        void rebootBootloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }
        void rebootBootloader_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.RebootBootloader(createWindow, _target);
        }

        private string _executeCommand = null;
        private void ExecuteOneCommand_Click(object sender, RoutedEventArgs e)
        {
            _executeCommand = ExecuteOneCommandText.Text;
            if (!string.IsNullOrEmpty(_executeCommand))
            {
                BackgroundWorker executer = new BackgroundWorker();
                executer.DoWork += executer_DoWork;
                executer.RunWorkerCompleted += executer_RunWorkerCompleted;
                executer.RunWorkerAsync();
            }
            else
            {
                ADBFlyout.Header = "Error";
                FlyoutText.Text = "You must enter command you want to execute";
                ShowADBFlyout();
            }

        }
        void executer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }
        void executer_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!string.IsNullOrEmpty(_executeCommand))
            {
                e.Result = _adb.ExecuteSingleCommand(_executeCommand, createWindow, _target);
            }
            else
            {
                e.Result = "Error while ADB Command execution";
            }
        }
        #endregion

        #region Push and Pull
        private List<string> _pushFiles;
        private string _location;
        private void PushChooseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == true)
            {
                _pushFiles = new List<string>();
                foreach (string file in dialog.FileNames)
                {
                    _pushFiles.Add(file.Trim());
                }
            }
        }
        private void PushButton_Click(object sender, RoutedEventArgs e)
        {
            _location = PushLocation.Text;
            if (_pushFiles.Count != 0 && !string.IsNullOrEmpty(_location))
            {
                BackgroundWorker pushFiles = new BackgroundWorker();
                pushFiles.DoWork += pushFiles_DoWork;
                pushFiles.RunWorkerCompleted += pushFiles_RunWorkerCompleted;
                pushFiles.RunWorkerAsync();
            }
            else
            {
                ADBFlyout.Header = "Error";
                FlyoutText.Text = "You select files you want to push and \n enter valid location";
                ShowADBFlyout();
            }


        }
        void pushFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }
        void pushFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_pushFiles.Count != 0 && !string.IsNullOrEmpty(_location))
            {
                string locationPath = string.Format(@"""{0}""", _location);
                e.Result = _adb.Push(_pushFiles, locationPath, createWindow, _target);
            }
            else
            {
                e.Result = "Execute push command error";
            }
        }

        private List<string> _pullFiles;
        private string _pullLocation;
        string file1;
        string file2;
        string file3;
        string file4;
        string file5;
        private void PullButton_Click(object sender, RoutedEventArgs e)
        {
            file1 = PullFile1.Text;
            file2 = PullFile2.Text;
            file3 = PullFile3.Text;
            file4 = PullFile4.Text;
            file5 = PullFile5.Text;

            if (!string.IsNullOrEmpty(file1))
            {
                _pullFiles.Add(file1);
            }
            if (!string.IsNullOrEmpty(file2))
            {
                _pullFiles.Add(file2);
            }
            if (!string.IsNullOrEmpty(file3))
            {
                _pullFiles.Add(file3);
            }
            if (!string.IsNullOrEmpty(file4))
            {
                _pullFiles.Add(file4);
            }
            if (!string.IsNullOrEmpty(file5))
            {
                _pullFiles.Add(file5);
            }
            if (_pullFiles.Count > 0 && !string.IsNullOrEmpty(_pullLocation))
            {
                BackgroundWorker pullFiles = new BackgroundWorker();
                pullFiles.DoWork += pullFiles_DoWork;
                pullFiles.RunWorkerCompleted += pullFiles_RunWorkerCompleted;
                pullFiles.RunWorkerAsync();
            }
            else
            {
                ADBFlyout.Header = "Error";
                FlyoutText.Text = "You must enter name/s files \n you want to push and \n enter choose location";
                ShowADBFlyout();
            }





        }

        void pullFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }
        void pullFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = _adb.Pull(_pullFiles, _pullLocation, createWindow, _target);
            }

            catch (Exception ex)
            {
                e.Result = "Execute pull command error \n " + ex.ToString();
            }

        }
        private void PullChooseLocation(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            _pullLocation = dialog.SelectedPath;
        }
        #endregion

        #region Install and Uninstall
        private string _installApk;
        private bool _systemApp = false;
        private void InstallChooseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = ".apk file you want to install";
            dlg.DefaultExt = ".apk";
            dlg.Filter = "APK Android Package File (.apk)|*.apk";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                _installApk = dlg.FileName.Trim();
            }
        }
        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_installApk))
            {
                BackgroundWorker install = new BackgroundWorker();
                install.DoWork += install_DoWork;
                install.RunWorkerCompleted += install_RunWorkerCompleted;
                install.RunWorkerAsync();
            }
            else
            {
                ADBFlyout.Header = "Error";
                FlyoutText.Text = "You must choose app to install";
                ShowADBFlyout();
            }


        }

        void install_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }
        void install_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.InstallApk(_installApk, _systemApp, createWindow, _target);
        }
        private void InstallSystemApp_Checked(object sender, RoutedEventArgs e)
        {
            _systemApp = true;
        }
        private void InstallSystemApp_Unchecked(object sender, RoutedEventArgs e)
        {
            _systemApp = false;
        }

        private string _uninstallApp;
        private bool _systemApp2 = false;
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _uninstallApp = DeleteAppName.Text;
            if (!string.IsNullOrEmpty(_uninstallApp))
            {
                BackgroundWorker delete = new BackgroundWorker();
                delete.DoWork += delete_DoWork;
                delete.RunWorkerCompleted += delete_RunWorkerCompleted;
                delete.RunWorkerAsync();
            }
            else
            {
                ADBFlyout.Header = "Error";
                FlyoutText.Text = "You must enter valid app package name to uninstall";
                ShowADBFlyout();
            }


        }
        void delete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }
        void delete_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.UninstallApp(_uninstallApp, _systemApp2, createWindow, _target);
        }
        private void UninstallSystemApp_Checked(object sender, RoutedEventArgs e)
        {
            _systemApp2 = true;
        }
        private void UninstallSystemApp_Unchecked(object sender, RoutedEventArgs e)
        {
            _systemApp2 = false;
        }
        #endregion

        #region Sideload
        private string _sideloadFile;
        private void ChooseSideloadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = ".zip you want to sideload";
            dlg.DefaultExt = ".zip";
            dlg.Filter = "ZIP files (.zip)|*.zip";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                _sideloadFile = dlg.FileName.Trim();
            }
        }

        private void SideloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_sideloadFile))
            {
                BackgroundWorker sideloadWorker = new BackgroundWorker();
                sideloadWorker.DoWork += sideloadWorker_DoWork;
                sideloadWorker.RunWorkerCompleted += sideloadWorker_RunWorkerCompleted;
                sideloadWorker.RunWorkerAsync();
            }
            else
            {
                ADBFlyout.Header = "Error";
                FlyoutText.Text = "You must select .zip to sideload";
                ShowADBFlyout();
            }
        }

        void sideloadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }

        void sideloadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.Sideload(_sideloadFile, createWindow, _target);
        }
        #endregion

        #region Execute
        private string _cmd1;
        private string _cmd2;
        private string _cmd3;
        private string _cmd4;
        private string _cmd5;
        private string _cmd6;
        private string _cmd7;
        private string _cmd8;
        private string _cmd9;
        private string _cmd10;
        List<string> Commands = new List<string>();
        private void ExecuteCommands_Click(object sender, RoutedEventArgs e)
        {
            _cmd1 = Command1.Text;
            _cmd2 = Command2.Text;
            _cmd3 = Command3.Text;
            _cmd4 = Command4.Text;
            _cmd5 = Command5.Text;
            _cmd6 = Command6.Text;
            _cmd7 = Command7.Text;
            _cmd8 = Command8.Text;
            _cmd9 = Command9.Text;
            _cmd10 = Command10.Text;

            if (!string.IsNullOrEmpty(_cmd1))
            {
                Commands.Add(_cmd1);
            }
            if (!string.IsNullOrEmpty(_cmd2))
            {
                Commands.Add(_cmd2);
            }
            if (!string.IsNullOrEmpty(_cmd3))
            {
                Commands.Add(_cmd3);
            }
            if (!string.IsNullOrEmpty(_cmd4))
            {
                Commands.Add(_cmd4);
            }
            if (!string.IsNullOrEmpty(_cmd5))
            {
                Commands.Add(_cmd5);
            }
            if (!string.IsNullOrEmpty(_cmd6))
            {
                Commands.Add(_cmd6);
            }
            if (!string.IsNullOrEmpty(_cmd7))
            {
                Commands.Add(_cmd7);
            }
            if (!string.IsNullOrEmpty(_cmd8))
            {
                Commands.Add(_cmd8);
            }
            if (!string.IsNullOrEmpty(_cmd9))
            {
                Commands.Add(_cmd9);
            }
            if (!string.IsNullOrEmpty(_cmd10))
            {
                Commands.Add(_cmd10);
            }
            if (Commands.Count != 0)
            {
                BackgroundWorker executeWorker = new BackgroundWorker();
                executeWorker.DoWork += executeWorker_DoWork;
                executeWorker.RunWorkerCompleted += executeWorker_RunWorkerCompleted;
                executeWorker.RunWorkerAsync();
            }
            else
            {
                ADBFlyout.Header = "Error";
                FlyoutText.Text = "You didn't enter any command";
                ShowADBFlyout();
            }

        }
        void executeWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }
        void executeWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.ExecuteMultipleCommands(Commands, createWindow, _target);
        }
        #endregion

        #region ListApps
        private string _appOutput;
        private void ListApps_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker appsList = new BackgroundWorker();
            appsList.DoWork += appsList_DoWork;
            appsList.RunWorkerCompleted += appsList_RunWorkerCompleted;
            appsList.RunWorkerAsync();
        }
        void appsList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            ADBFlyout2.Header = "ALL PACKAGES";
            FlyoutText2.Text = _appOutput;
            var flyout = (Flyout)Flyouts.Items[1];
            flyout.IsOpen = !flyout.IsOpen;
        }
        void appsList_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.ListApps(createWindow, _target);
            _appOutput = StringLinesRemover.ForgetLastLine(StringLinesRemover.RemoveLine(e.Result.ToString(), 4));

        }
        #endregion

        #region ADB
        private void StartADB_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker startADB = new BackgroundWorker();
            startADB.DoWork += startADB_DoWork;
            startADB.RunWorkerCompleted += startADB_RunWorkerCompleted;
            startADB.RunWorkerAsync();
        }

        void startADB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }

        void startADB_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.ExecuteSingleCommand("adb start-server", createWindow);
        }

        private void KillADB_Click(object sender, RoutedEventArgs e)
        {
            _adb.KillAdb();
        }
        private void RemoteADB_OnClick(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)Flyouts.Items[3];
            flyout.IsOpen = !flyout.IsOpen;
        }

        private void ApplyRemoteADB_Click(object sender, RoutedEventArgs e)
        {
            _targetDeviceIP = TargetDeviceIP.Text;
            _targetDevicePort = TargetDevicePort.Text;
            if (!string.IsNullOrEmpty(_targetDeviceIP) && !string.IsNullOrEmpty(_targetDevicePort))
            {
                _targetDeviceIP = TargetDeviceIP.Text;
                _targetDevicePort = TargetDevicePort.Text;
                _remoteADBWorker = new BackgroundWorker();
                _remoteADBWorker.DoWork += _remoteADBWorker_DoWork;
                _remoteADBWorker.RunWorkerCompleted += _remoteADBWorker_RunWorkerCompleted;
                _remoteADBWorker.RunWorkerAsync();
            }
            else
            {
                ADBFlyout.Header = "Error";
                FlyoutText.Text = "You must enter valid IP and PORT to set up \nREMOTE ADB";
                ShowADBFlyout();
            }


        }

        void _remoteADBWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }

        void _remoteADBWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.SetRemoteADB(_targetDevicePort, _targetDeviceIP, createWindow);
        }

        private BackgroundWorker _remoteADBWorker;

        private void KillRemoteADB_OnClick(object sender, RoutedEventArgs e)
        {
            _setUsbWorker = new BackgroundWorker();
            _setUsbWorker.DoWork += _setUsbWorker_DoWork;
            _setUsbWorker.RunWorkerCompleted += _setUsbWorker_RunWorkerCompleted;
            _setUsbWorker.RunWorkerAsync();
        }

        void _setUsbWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _setUsbWorker.Dispose();
        }

        void _setUsbWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.SetUSB(createWindow);
        }

        private BackgroundWorker _setUsbWorker;

        private async void ApplyMultipleADB_Click(object sender, RoutedEventArgs e)
        {
            if (TargetDeviceID.Text.Length != 0)
            {
                this._target = TargetDeviceID.Text;
                await this.ShowMessageAsync("SUCCESS", "Now, all commands are executed at device: " + _target);
            }
            else
            {
                await this.ShowMessageAsync("ERROR", "Please, enter device ID. You can find it at devices list.");
            }

        }

        private void OpenMultipleDevices(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)Flyouts.Items[4];
            flyout.IsOpen = !flyout.IsOpen;
        }
        private void UndoMultipleADB_Click(object sender, RoutedEventArgs e)
        {
            this._target = null;
        }

        #endregion

        #region DeviceInfo
        private string _deviceInfo;
        private BackgroundWorker deviceInfo;
        private void DeviceInfoButton_Click(object sender, RoutedEventArgs e)
        {
            deviceInfo = new BackgroundWorker();
            deviceInfo.DoWork += deviceInfo_DoWork;
            deviceInfo.RunWorkerCompleted += deviceInfo_RunWorkerCompleted;
            deviceInfo.RunWorkerAsync();
        }
        void deviceInfo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            ADBFlyout.Header = "build.prop";
            FlyoutText.Text = _deviceInfo;
            ShowADBFlyout();
            deviceInfo.Dispose();
        }
        void deviceInfo_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.DeviceInfo(createWindow, _target);
            _deviceInfo = StringLinesRemover.ForgetLastLine(StringLinesRemover.RemoveLine(e.Result.ToString(), 4));
        }


        private string _deviceName;
        private void DeviceName_Click(object sender, RoutedEventArgs e)
        {
            _deviceNameWorker = new BackgroundWorker();
            _deviceNameWorker.DoWork += _deviceNameWorker_DoWork;
            _deviceNameWorker.RunWorkerCompleted += _deviceNameWorker_RunWorkerCompleted;
            _deviceNameWorker.RunWorkerAsync();
        }
        void _deviceNameWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            ADBFlyout.Header = "DEVICE NAME";
            FlyoutText.Text = _deviceName;
            ShowADBFlyout();
            _deviceNameWorker.Dispose();
        }
        void _deviceNameWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.DeviceName(createWindow, _target);
            _deviceName = StringLinesRemover.ForgetLastLine(StringLinesRemover.RemoveLine(e.Result.ToString(), 4));
        }
        private BackgroundWorker _deviceNameWorker;


        private string _osVersion;
        private void AndroidVersion_Click(object sender, RoutedEventArgs e)
        {
            _osWorker = new BackgroundWorker();
            _osWorker.DoWork += _osWorker_DoWork;
            _osWorker.RunWorkerCompleted += _osWorker_RunWorkerCompleted;
            _osWorker.RunWorkerAsync();
        }
        void _osWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            ADBFlyout.Header = "OS Version";
            FlyoutText.Text = _osVersion;
            ShowADBFlyout();
            _osWorker.Dispose();
        }
        void _osWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.DeviceOsVersion(createWindow, _target);
            _osVersion = StringLinesRemover.ForgetLastLine(StringLinesRemover.RemoveLine(e.Result.ToString(), 4));
        }
        private BackgroundWorker _osWorker;


        private void Hardware_Click(object sender, RoutedEventArgs e)
        {
            _hardwareWorker = new BackgroundWorker();
            _hardwareWorker.DoWork += _hardwareWorker_DoWork;
            _hardwareWorker.RunWorkerCompleted += _hardwareWorker_RunWorkerCompleted;
            _hardwareWorker.RunWorkerAsync();
        }
        void _hardwareWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _hardwareWorker.Dispose();
        }
        void _hardwareWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = "Browser has been opened";
            Process.Start(string.Format("http://www.google.hr/#q={0}", _deviceName));
        }
        private BackgroundWorker _hardwareWorker;

        #endregion

        #region Flyouts
        private void ShowADBFlyout()
        {
            var flyout = Flyouts.Items[0] as Flyout;
            flyout.IsOpen = !flyout.IsOpen;
        }
        private void ShowADB2Flyout()
        {
            var flyout = Flyouts.Items[1] as Flyout;
            ADBFlyout2.Header = "Error";
            FlyoutText2.Text = "You select files you want to push and \n enter valid location";
            flyout.IsOpen = !flyout.IsOpen;
        }
        #endregion

        #region Theme

        private void LoadAccents()
        {
            Accents.Items.Add("Blue");
            Accents.Items.Add("Green");
            Accents.Items.Add("Red");
            Accents.Items.Add("Orange");
            Accents.Items.Add("Steel");
            Accents.Items.Add("Purple");
            Accents.Items.Add("Cyan");
            Accents.Items.Add("Emerald");
            Accents.Items.Add("Teal");
            Accents.Items.Add("Indigo");
            Accents.Items.Add("Yellow");
            Accents.Items.Add("Brown");
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var flyout = Flyouts.Items[2] as Flyout;
            flyout.IsOpen = !flyout.IsOpen;
        }

        private void Accents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _newAccentString = (string)Accents.SelectedItem;
        }
        protected string _newAccentString;

        private void ChangeAccent_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_newAccentString))
            {
                ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == _newAccentString), ThemeManager.DetectTheme(this).Item1);
            }
            else
            {

                ADBFlyout.Header = "Error";
                FlyoutText.Text = "You must select accent";
                ShowADBFlyout();
            }

        }
        #endregion

        #region Copy/Move/Delete
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            _copyFrom = CopyFrom.Text;
            _copyTo = CopyTo.Text;

            if (!string.IsNullOrEmpty(_copyFrom) && !string.IsNullOrEmpty(_copyTo))
            {
                _copyWorker = new BackgroundWorker();
                _copyWorker.DoWork += _copyWorker_DoWork;
                _copyWorker.RunWorkerCompleted += _copyWorker_RunWorkerCompleted;
                _copyWorker.RunWorkerAsync();
            }
            else
            {
                ADBFlyout.Header = "Error";
                FlyoutText.Text = "You must enter all copy paths";
                ShowADBFlyout();
            }
        }

        void _copyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();

            _copyWorker.Dispose();
        }

        void _copyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.Copy(_copyFrom, _copyTo, createWindow, _target);
        }

        private void Move_Click(object sender, RoutedEventArgs e)
        {
            _moveFrom = MoveFrom.Text;
            _moveTo = MoveTo.Text;
            if (!string.IsNullOrEmpty(_moveFrom) && !string.IsNullOrEmpty(_moveTo))
            {
                _moveWorker = new BackgroundWorker();
                _moveWorker.DoWork += _moveWorker_DoWork;
                _moveWorker.RunWorkerCompleted += _moveWorker_RunWorkerCompleted;
                _moveWorker.RunWorkerAsync();
            }
            else
            {
                ADBFlyout.Header = "Error";
                FlyoutText.Text = "You must enter all move paths";
                ShowADBFlyout();
            }
        }

        void _moveWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            MoveFrom.Text = string.Empty;
            MoveTo.Text = string.Empty;
            _moveWorker.Dispose();
        }

        void _moveWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.Move(_moveFrom, _moveTo, createWindow, _target);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            _deletePath = DeletePath.Text;
            if (!string.IsNullOrEmpty(_deletePath))
            {
                _deleteWorker = new BackgroundWorker();
                _deleteWorker.DoWork += _deleteWorker_DoWork;
                _deleteWorker.RunWorkerCompleted += _deleteWorker_RunWorkerCompleted;
                _deleteWorker.RunWorkerAsync();
            }
            else
            {
                ADBFlyout.Header = "Error";
                FlyoutText.Text = "You must enter delete path";
                ShowADBFlyout();
            }
        }

        void _deleteWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _deletePath = string.Empty;
            _deleteWorker.Dispose();
        }

        void _deleteWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adb.Delete(_deletePath, createWindow, _target);
        }

        private BackgroundWorker _copyWorker;
        private BackgroundWorker _moveWorker;
        private BackgroundWorker _deleteWorker;
        private string _copyFrom;
        private string _copyTo;

        private string _moveFrom;
        private string _moveTo;

        private string _deletePath;

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

        private string _saveDeviceIP;
        private string _saveDeviceName;
        private async void SaveIPButton_Click(object sender, RoutedEventArgs e)
        {
            _saveDeviceIP = SaveDeviceIP.Text;
            _saveDeviceName = SaveDeviceName.Text;
            if ((!string.IsNullOrEmpty(_saveDeviceIP) && (!string.IsNullOrEmpty(_saveDeviceName))))
            {
                await InsertIP(_saveDeviceIP, _saveDeviceName);
                LoadIPs();
            }
            else
            {
                await this.ShowMessageAsync("ERROR", "Please enter all IP and Name");
            }
        }

        #region Tasks
        private Task InsertIP(string ip, string name)
        {
            if (File.Exists("IP-s.xml"))
            {
                XDocument doc = XDocument.Load("IP-s.xml");
                XElement newEl = new XElement("IP");
                newEl.SetAttributeValue("IP", ip);
                newEl.SetAttributeValue("Name", name);
                doc.Root.Add(newEl);
                return Task.Factory.StartNew(() => doc.Save("IP-s.xml"));
            }
            return Task.Factory.StartNew(() => File.Create("IP-s.xml"));
        }

        private IEnumerable<object> RefreshIPs()
        {
            XDocument doc = XDocument.Load("IP-s.xml");
            foreach (var item in doc.Root.Descendants())
            {
                yield return new { DeviceIP = item.Attribute("IP").Value, DeviceName = item.Attribute("Name").Value };
            }
        }
        private void LoadIPs()
        {
            BackgroundWorker loader = new BackgroundWorker();
            loader.DoWork += delegate
            {
                this.IPListView.Dispatcher.InvokeAsync((Action)(async () =>
                    {
                        this.IPListView.ItemsSource = await Task.Factory.StartNew(() => RefreshIPs());
                    }));

            };
            loader.RunWorkerAsync();
        }
        private Task DeleteIPs()
        {
            XDocument doc = XDocument.Load("IP-s.xml");
            doc.Root.RemoveAll();
            return Task.Factory.StartNew(() => doc.Save("IP-s.xml"));
        }
        #endregion

    }
}






