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

namespace AndroidToolkit
{
    /// <summary>
    /// Interaction logic for FastbootWindow.xaml
    /// </summary>
    public partial class FastbootTools : MetroWindow
    {
        public FastbootTools()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _fastboot = new FastbootOperations();
            LoadAccents();
        }
        private readonly FastbootOperations _fastboot;

        private bool _createWindow = true;

        #region WindowOperations
        private void CreateWindowCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _createWindow = false;
        }
        private void CreateWindowCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _createWindow = true;
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
        #endregion

        #region Reboot
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
            e.Result = _fastboot.Reboot(_createWindow);
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
            e.Result = _fastboot.RebootBootloader(_createWindow);
        }

        private void RebootRecoveryButton_Click(object sender, RoutedEventArgs e)
        {
            _rebootRecoveryWorker = new BackgroundWorker();
            _rebootRecoveryWorker.DoWork += _rebootRecoveryWorker_DoWork;
            _rebootRecoveryWorker.RunWorkerCompleted += _rebootRecoveryWorker_RunWorkerCompleted;
            _rebootRecoveryWorker.RunWorkerAsync();
        }

        void _rebootRecoveryWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _rebootRecoveryWorker.Dispose();
        }

        void _rebootRecoveryWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.RebootRecovery(_createWindow);
        }
        private BackgroundWorker _rebootRecoveryWorker;
        #endregion

        #region FastbootOperations
        private void KillFastboot_Click(object sender, RoutedEventArgs e)
        {
            _fastboot.KillFastboot();
        }

        #endregion

        #region Boot
        private string _bootFile;
        private BackgroundWorker _bootWorker;
        private void BootChoose_Click(object sender, RoutedEventArgs e)
        {
            _bootFile = Dialoger.ShowSingleChooseDialog(filter: "Android Image File (.img)|*.img");
        }

        private void Boot_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_bootFile))
            {
                _bootWorker = new BackgroundWorker();
                _bootWorker.DoWork += _bootWorker_DoWork;
                _bootWorker.RunWorkerCompleted += _bootWorker_RunWorkerCompleted;
                _bootWorker.RunWorkerAsync();
            }

            else
            {
                this.ShowMessageAsync("ERROR", "You must select image to boot");
            }
        }

        void _bootWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _bootWorker.Dispose();
        }

        void _bootWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.Boot(_bootFile, _createWindow);
        }
        #endregion

        #region ExecuteSingleCommand
        private string _executeString = null;
        private BackgroundWorker _executeWorker;
        private void ExecuteOneCommand_OnClick(object sender, RoutedEventArgs e)
        {
            _executeString = ExecuteOneCommandText.Text;
            if (!string.IsNullOrEmpty(_executeString))
            {
                _executeWorker = new BackgroundWorker();
                _executeWorker.DoWork += _executeWorker_DoWork;
                _executeWorker.RunWorkerCompleted += _executeWorker_RunWorkerCompleted;
                _executeWorker.RunWorkerAsync();

            }
            else
            {
                this.ShowMessageAsync("ERROR", "You didn't enter command to execute");
            }

        }

        void _executeWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }

        void _executeWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.ExecuteSingleCommand(_executeString, _createWindow);
        }
        #endregion

        #region Bootloader
        private void GetIdentifierToken_Click(object sender, RoutedEventArgs e)
        {
            _tokenWorker = new BackgroundWorker();
            _tokenWorker.DoWork += _tokenWorker_DoWork;
            _tokenWorker.RunWorkerCompleted += _tokenWorker_RunWorkerCompleted;
            _tokenWorker.RunWorkerAsync();
        }
        void _tokenWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _tokenWorker.Dispose();
        }
        void _tokenWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var procSi = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            var proc = Process.Start(procSi);
            proc.StandardInput.WriteLine("fastboot oem get_identifier_token");
            e.Result = proc.StandardOutput.ReadToEnd();
        }
        private BackgroundWorker _tokenWorker;

        private BackgroundWorker _lockWorker;
        private BackgroundWorker _unlockWorker;
        private void Lock_Click(object sender, RoutedEventArgs e)
        {
            _lockWorker = new BackgroundWorker();
            _lockWorker.DoWork += _lockWorker_DoWork;
            _lockWorker.RunWorkerCompleted += _lockWorker_RunWorkerCompleted;
            _lockWorker.RunWorkerAsync();
        }
        void _lockWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }
        void _lockWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.Lock(_createWindow);
            _lockWorker.Dispose();
        }

        private void Unlock_Click(object sender, RoutedEventArgs e)
        {
            _unlockWorker = new BackgroundWorker();
            _unlockWorker.DoWork += _unlockWorker_DoWork;
            _unlockWorker.RunWorkerCompleted += _unlockWorker_RunWorkerCompleted;
            _unlockWorker.RunWorkerAsync();
        }
        void _unlockWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }
        void _unlockWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.Unlock(_createWindow);
        }

        private string _writeCID;
        private BackgroundWorker _cidWorker;
        private void WriteCid_Click(object sender, RoutedEventArgs e)
        {
            _writeCID = WriteCIDBlock.Text;
            if (!string.IsNullOrEmpty(_writeCID))
            {
                _writeCID = WriteCIDBlock.Text;
                _cidWorker = new BackgroundWorker();
                _cidWorker.DoWork += _cidWorker_DoWork;
                _cidWorker.RunWorkerCompleted += _cidWorker_RunWorkerCompleted;
                _cidWorker.RunWorkerAsync();
            }
            else
            {
                this.ShowMessageAsync("ERROR", "You must enter the CID number to write.");
            }

        }
        void _cidWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _cidWorker.Dispose();
        }
        void _cidWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.WriteCID(_writeCID, _createWindow);
        }
        #endregion

        #region Erase
        private void EraseSystem_Click(object sender, RoutedEventArgs e)
        {
            _eraseSystemWorker = new BackgroundWorker();
            _eraseSystemWorker.DoWork += _eraseSystemWorker_DoWork;
            _eraseSystemWorker.RunWorkerCompleted += _eraseSystemWorker_RunWorkerCompleted;
            _eraseSystemWorker.RunWorkerAsync();
        }
        void _eraseSystemWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }
        void _eraseSystemWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.EraseSystem(_createWindow);
        }
        private BackgroundWorker _eraseSystemWorker;

        private void EraseBoot_Click(object sender, RoutedEventArgs e)
        {
            _eraseBootWorker = new BackgroundWorker();
            _eraseBootWorker.DoWork += _eraseBootWorker_DoWork;
            _eraseBootWorker.RunWorkerCompleted += _eraseBootWorker_RunWorkerCompleted;
            _eraseBootWorker.RunWorkerAsync();
        }
        void _eraseBootWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }
        void _eraseBootWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.EraseBoot(_createWindow);
        }
        private BackgroundWorker _eraseBootWorker;

        private void EraseRecovery_Click(object sender, RoutedEventArgs e)
        {
            _eraseRecoveryWorker = new BackgroundWorker();
            _eraseRecoveryWorker.DoWork += _eraseRecoveryWorker_DoWork;
            _eraseRecoveryWorker.RunWorkerCompleted += _eraseRecoveryWorker_RunWorkerCompleted;
            _eraseRecoveryWorker.RunWorkerAsync();
        }
        void _eraseRecoveryWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _eraseRecoveryWorker.Dispose();
        }
        void _eraseRecoveryWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.EraseRecovery(_createWindow);
        }
        private BackgroundWorker _eraseRecoveryWorker;

        private void EraseCache_Click(object sender, RoutedEventArgs e)
        {
            _eraseCacheWorker = new BackgroundWorker();
            _eraseCacheWorker.DoWork += _eraseCacheWorker_DoWork;
            _eraseCacheWorker.RunWorkerCompleted += _eraseCacheWorker_RunWorkerCompleted;
            _eraseCacheWorker.RunWorkerAsync();
        }
        void _eraseCacheWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _eraseCacheWorker.Dispose();
        }
        void _eraseCacheWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.EraseCache(_createWindow);

        }
        private BackgroundWorker _eraseCacheWorker;

        private void EraseUserdata_Click(object sender, RoutedEventArgs e)
        {
            _eraseUserdataWorker = new BackgroundWorker();
            _eraseUserdataWorker.DoWork += _eraseUserdataWorker_DoWork;
            _eraseUserdataWorker.RunWorkerCompleted += _eraseUserdataWorker_RunWorkerCompleted;
            _eraseUserdataWorker.RunWorkerAsync();
        }
        void _eraseUserdataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _eraseUserdataWorker.Dispose();
        }
        void _eraseUserdataWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.EraseUserdata(_createWindow);
        }
        private BackgroundWorker _eraseUserdataWorker;
        #endregion

        #region Flash
        //SYSTEM
        private void FlashSystemChoose_Click(object sender, RoutedEventArgs e)
        {
            _flashSystem = Dialoger.ShowSingleChooseDialog(filter: "Android Image File (.img)|*.img");
        }
        private string _flashSystem;
        private void FlashSystem_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(_flashSystem))
            {
                _flashSystemWorker = new BackgroundWorker();
                _flashSystemWorker.DoWork += _flashSystemWorker_DoWork;
                _flashSystemWorker.RunWorkerCompleted += _flashSystemWorker_RunWorkerCompleted;
                _flashSystemWorker.RunWorkerAsync();
            }
            else
            {
                this.ShowMessageAsync("ERROR", "You select select system image to flash");

            }


        }
        void _flashSystemWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _flashSystemWorker.Dispose();
        }
        void _flashSystemWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.FlashSystem(_flashSystem, _createWindow);
        }
        private BackgroundWorker _flashSystemWorker;
        //END SYSTEM

        //BOOT
        private string _flashBoot;
        private void FlashBootChoose_Click(object sender, RoutedEventArgs e)
        {
            _flashBoot = Dialoger.ShowSingleChooseDialog(filter: "Android Image File (.img)|*.img");
        }
        private void FlashBoot_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_flashBoot))
            {
                _flashBootWorker = new BackgroundWorker();
                _flashBootWorker.DoWork += _flashBootWorker_DoWork;
                _flashBootWorker.RunWorkerCompleted += _flashBootWorker_RunWorkerCompleted;
                _flashBootWorker.RunWorkerAsync();

            }
            else
            {

                this.ShowMessageAsync("ERROR", "You select select boot image to flash");

            }

        }
        void _flashBootWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _flashBootWorker.Dispose();
        }
        void _flashBootWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.FlashBoot(_flashBoot, _createWindow);
        }
        private BackgroundWorker _flashBootWorker;
        //END BOOT

        //RECOVERY
        private string _flashRecovery;
        private void FlashRecoveryChoose_Click(object sender, RoutedEventArgs e)
        {
            _flashRecovery = Dialoger.ShowSingleChooseDialog(filter: "Android Image File (.img)|*.img");
        }
        private void FlashRecovery_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_flashRecovery))
            {
                _flashRecoveryWorker = new BackgroundWorker();
                _flashRecoveryWorker.DoWork += _flashRecoveryWorker_DoWork;
                _flashRecoveryWorker.RunWorkerCompleted += _flashRecoveryWorker_RunWorkerCompleted;
                _flashRecoveryWorker.RunWorkerAsync();
            }
            else
            {

                this.ShowMessageAsync("ERROR", "You select select recovery image to flash");
            }
        }
        void _flashRecoveryWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _flashRecoveryWorker.Dispose();
        }
        void _flashRecoveryWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.FlashRecovery(_flashRecovery, _createWindow);
        }
        private BackgroundWorker _flashRecoveryWorker;
        //END RECOVERY

        //BOOTLOADER
        private string _flashBootloader;
        private void FlashBootloaderChoose_Click(object sender, RoutedEventArgs e)
        {
            _flashBootloader = Dialoger.ShowSingleChooseDialog(filter: "Android Image File (.img)|*.img");
        }
        private void FlashBootloader_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_flashBootloader))
            {
                _flashBootloaderWorker = new BackgroundWorker();
                _flashBootloaderWorker.DoWork += _flashBootloaderWorker_DoWork;
                _flashBootloaderWorker.RunWorkerCompleted += _flashBootloaderWorker_RunWorkerCompleted;
                _flashBootloaderWorker.RunWorkerAsync();
            }
            else
            {
                this.ShowMessageAsync("ERROR", "You select select bootloader image to flash");
            }

        }
        void _flashBootloaderWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _flashBootloaderWorker.Dispose();
        }
        void _flashBootloaderWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.FlashBootloader(_flashBootloader, _createWindow);
        }
        private BackgroundWorker _flashBootloaderWorker;
        //END BOOTLOADER

        //RADIO
        private string _flashRadio;
        private void FlashRadioChoose_Click(object sender, RoutedEventArgs e)
        {
            _flashRadio = Dialoger.ShowSingleChooseDialog(filter: "Android Image File (.img)|*.img");
        }
        private void FlashRadio_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(_flashRadio))
            {
                _flashRadioWorker = new BackgroundWorker();
                _flashRadioWorker.DoWork += _flashRadioWorker_DoWork;
                _flashRadioWorker.RunWorkerCompleted += _flashRadioWorker_RunWorkerCompleted;
                _flashRadioWorker.RunWorkerAsync();
            }
            else
            {

                this.ShowMessageAsync("ERROR", "You select select radio image to flash");
            }

        }
        void _flashRadioWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _flashRadioWorker.Dispose();
        }
        void _flashRadioWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.FlashRadio(_flashRadio, _createWindow);
        }
        private BackgroundWorker _flashRadioWorker;
        //END RADIO

        //UNLOCKTOKEN
        private string _flashUnlockToken;
        private void FlashUnlockTokenChoose_Click(object sender, RoutedEventArgs e)
        {
            _flashUnlockToken = Dialoger.ShowSingleChooseDialog(filter: "Unlock token binaries (.bin)|*.bin");
        }
        private void FlashUnlockToken_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_flashUnlockToken))
            {
                _flashUnlockTokenWorker = new BackgroundWorker();
                _flashUnlockTokenWorker.DoWork += _flashUnlockTokenWorker_DoWork;
                _flashUnlockTokenWorker.RunWorkerCompleted += _flashUnlockTokenWorker_RunWorkerCompleted;
                _flashUnlockTokenWorker.RunWorkerAsync();

            }
            else
            {
                this.ShowMessageAsync("ERROR", "You select select unlock token to flash");
            }

        }
        void _flashUnlockTokenWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _flashUnlockTokenWorker.Dispose();
        }
        void _flashUnlockTokenWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            e.Result = _fastboot.FlashUnlockToken(_flashUnlockToken, _createWindow);
        }
        private BackgroundWorker _flashUnlockTokenWorker;
        //END UNLOCKTOKEN

        //USERDATA
        private string _flashUserdata;
        private void FlashUserdataChoose_Click(object sender, RoutedEventArgs e)
        {
            _flashUserdata = Dialoger.ShowSingleChooseDialog(filter: "Android Image File (.img)|*.img");
        }
        private void FlashUserdata_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_flashUserdata))
            {
                _flashUserdataWorker = new BackgroundWorker();
                _flashUserdataWorker.DoWork += _flashUserdataWorker_DoWork;
                _flashUserdataWorker.RunWorkerCompleted += _flashUserdataWorker_RunWorkerCompleted;
                _flashUserdataWorker.RunWorkerAsync();
            }
            else
            {
                this.ShowMessageAsync("ERROR", "You select select userdata image to flash");
            }
        }
        void _flashUserdataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _flashUserdataWorker.Dispose();
        }
        void _flashUserdataWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.FlashUserdata(_flashUserdata, _createWindow);
        }
        private BackgroundWorker _flashUserdataWorker;

        //END USERDATA


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
                this.ShowMessageAsync("ERROR", "You didn't enter any commads");
            }

        }
        void executeWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
        }
        void executeWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _fastboot.ExecuteMultipleCommands(Commands, _createWindow);
        }
        #endregion

        #region Device and CID

        private void DevicesButton_Click(object sender, RoutedEventArgs e)
        {
            RecoveryMaster win = new RecoveryMaster();
            win.Show();
        }
        private void OpenZipFlasher_Click(object sender, RoutedEventArgs e)
        {
            new ZipFlasher().Show();
        }
        private BackgroundWorker _readCIDWorker;
        private string _readCID;
        private void ReadCID_Click(object sender, RoutedEventArgs e)
        {
            _readCIDWorker = new BackgroundWorker();
            _readCIDWorker.DoWork += _readCIDWorker_DoWork;
            _readCIDWorker.RunWorkerCompleted += _readCIDWorker_RunWorkerCompleted;
            _readCIDWorker.RunWorkerAsync();
        }

        void _readCIDWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _readCIDWorker.Dispose();
        }

        void _readCIDWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var procSi = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            var proc = Process.Start(procSi);
            proc.StandardInput.WriteLine("fastboot getvar cid");
            e.Result = proc.StandardOutput.ReadToEnd();
        }

        private void OpenDeviceRestore_Click(object sender, RoutedEventArgs e)
        {
            var win = new DeviceRestore();
            win.Show();
        }

        #endregion

        #region Flyouts
        private void ShowFastbootFlyout()
        {
            var flyout = Flyouts.Items[0] as Flyout;
            flyout.IsOpen = flyout.IsOpen;
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
            var flyout = Flyouts.Items[1] as Flyout;
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

                FastbootFlyout.Header = "Error";
                FlyoutText.Text = "You must select accent";
                ShowFastbootFlyout();
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
