using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using AndroidToolkit.Helpers;
using AndroidToolkit.Infrastructure;
using MahApps.Metro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Security;
using System.Security.Permissions;

namespace AndroidToolkit
{
    /// <summary>
    /// Interaction logic for AdvancedWindow.xaml
    /// </summary>
    public partial class AdvancedTools : MetroWindow
    {
        public AdvancedTools()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _advanced = new AdvancedOperations();
            LoadAccents();
            LoadBackupTypes();
            CheckDeviceConnection.Click += delegate
            {
                new DeviceWindow().Show();
            };
        }
        protected readonly AdvancedOperations _advanced;

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

        #region CreateWindow
        private bool _createWindow = true;
        private void CreateWindowBox_Checked(object sender, RoutedEventArgs e)
        {
            _createWindow = false;
        }
        private void CreateWindowBox_UnChecked(object sender, RoutedEventArgs e)
        {
            _createWindow = true;
        }
        #endregion

        #region Flyouts
        private void ShowAdvancedFlyout()
        {
            var flyout = Flyouts.Items[0] as Flyout;
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

                AdvancedFlyout.Header = "Error";
                FlyoutText.Text = "You must select accent";
                ShowAdvancedFlyout();
            }

        }
        #endregion

        #region Backup
        private void BackupTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentBackupButtonType = (Button)BackupTypeCombo.SelectedItem;
            _backupType = (string)_currentBackupButtonType.Content;
        }
        private string _backupPath;
        private string _backupFinalPath;
        private string _backupType;
        private Button _currentBackupButtonType;
        private void LoadBackupTypes()
        {
            BackupTypeCombo.Items.Add(new Button() { Content = "FULL BACKUP", IsEnabled = false });
            BackupTypeCombo.Items.Add(new Button() { Content = "APPS WITHOUT SYSTEM APPS", IsEnabled = false });
            BackupTypeCombo.Items.Add(new Button() { Content = "SYSTEM APPS - EXPERIMENTAL", IsEnabled = false });
            BackupTypeCombo.Items.Add(new Button() { Content = "SD BACKUP", IsEnabled = false });
        }

        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(BackupName.Text))
            {
                _backupFinalPath = null;
                _backupFinalPath = (string.Format(@"{0}\{1}.ab", _backupPath, BackupName.Text));
            }


            if (!string.IsNullOrEmpty(_backupType) && !string.IsNullOrEmpty(_backupPath))
            {
                _backupWorker = new BackgroundWorker();
                _backupWorker.DoWork += _backupWorker_DoWork;
                _backupWorker.RunWorkerCompleted += _backupWorker_RunWorkerCompleted;
                _backupWorker.RunWorkerAsync();
            }
            else
            {
                AdvancedFlyout.Header = "Error";
                FlyoutText.Text = "You must select backup type,\n enter backup name and choose location\n for saving backup";
                ShowAdvancedFlyout();
            }
        }

        void _backupWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _backupWorker.Dispose();
        }

        void _backupWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_backupType == "FULL BACKUP")
            {
                e.Result = _advanced.FullADBBackup(_backupFinalPath, _createWindow);
            }
            if (_backupType == "APPS WITHOUT SYSTEM APPS")
            {
                e.Result = _advanced.ADBNoSystemAppsBackup(_backupFinalPath, _createWindow);
            }
            if (_backupType == "SYSTEM APPS - EXPERIMENTAL")
            {
                e.Result = _advanced.ADBSystemAppsBackup(_backupFinalPath, _createWindow);
            }
            if (_backupType == "SD BACKUP")
            {
                e.Result = _advanced.ADBSDBackup(_backupPath, _createWindow);
            }

        }
        private BackgroundWorker _backupWorker;

        private void ChooseBackupLocation_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            _backupPath = dialog.SelectedPath;
        }
        #endregion

        #region Restore
        private void ChooseRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            _restoreName = Dialoger.ShowSingleChooseDialog(filter: "Android Backup (.ab)|*.ab");
        }
        private string _restoreName;

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_restoreName))
            {
                _restoreWorker = new BackgroundWorker();
                _restoreWorker.DoWork += _restoreWorker_DoWork;
                _restoreWorker.RunWorkerCompleted += _restoreWorker_RunWorkerCompleted;
                _restoreWorker.RunWorkerAsync();
            }
            else
            {
                FlyoutText.Text = "You must select backup file to restore";
                ShowAdvancedFlyout();
            }


        }

        void _restoreWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _restoreWorker.Dispose();
        }

        void _restoreWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _advanced.RestoreADBBackup(_restoreName, _createWindow);
        }

        private BackgroundWorker _restoreWorker;

        #endregion

        #region Root/Unroot

        private void Root_Click(object sender, RoutedEventArgs e)
        {
            _rootWorker = new BackgroundWorker();
            _rootWorker.DoWork += _rootWorker_DoWork;
            _rootWorker.RunWorkerCompleted += _rootWorker_RunWorkerCompleted;
            _rootWorker.RunWorkerAsync();
        }
        void _rootWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _rootWorker.Dispose();
        }
        void _rootWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _advanced.Root(_createWindow);
        }

        private void Unroot_OnClick(object sender, RoutedEventArgs e)
        {
            _unRootWorker = new BackgroundWorker();
            _unRootWorker.DoWork += _unRootWorker_DoWork;
            _unRootWorker.RunWorkerCompleted += _unRootWorker_RunWorkerCompleted;
            _unRootWorker.RunWorkerAsync();
        }
        void _unRootWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            _unRootWorker.Dispose();
        }
        void _unRootWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _advanced.Unroot(_createWindow);
        }

        private BackgroundWorker _unRootWorker;
        private BackgroundWorker _rootWorker;
        #endregion

        private void LogcatChoose_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            _logcatLocation = dialog.SelectedPath;
        }

        private string _logcat;
        private string _logcatLocation;

        private void Logcat_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_logcatLocation))
            {
                _logcatWorker = new BackgroundWorker();
                _logcatWorker.DoWork += _logcatWorker_DoWork;
                _logcatWorker.RunWorkerCompleted += _logcatWorker_RunWorkerCompleted;
                _logcatWorker.RunWorkerAsync();
            }
            else
            {
                var result = this.ShowMessageAsync("ERROR", "You must select logcat location");
            }
        }

        async void _logcatWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputBlock.Text = e.Result.ToString();
            LogcatText.Text = await Task.Run(() => File.ReadAllText(System.IO.Path.Combine(_logcatLocation, "logcat.txt")));
        }

        void _logcatWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _advanced.Logcat(string.Format(@"""{0}\logcat.txt""", _logcatLocation), _createWindow);
            _logcat = StringLinesRemover.ForgetLastLine(StringLinesRemover.RemoveLine(e.Result.ToString(), 4));
        }

        private BackgroundWorker _logcatWorker;

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
