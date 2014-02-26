using System.Data.Entity;
using AndroidToolkit.Helpers;
using AndroidToolkit.Helpers.Admin;
using AndroidToolkit.Helpers.Devices;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;

namespace AndroidToolkit
{
    /// <summary>
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : MetroWindow
    {

        public Admin()
        {
            InitializeComponent();
            _db = new AndroidToolkitDB();
            _deviceManager = new DeviceManager();
            _adminManager = new AdminManager();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DevicesGrid.CanUserAddRows = false;
            DevicesGrid.CanUserDeleteRows = false;
            AdminsGrid.CanUserAddRows = false;
            AdminsGrid.CanUserDeleteRows = false;
            SQLBtn.Click += ((sender, args) =>
                {
                    var flyout = (Flyout)Flyouts.Items[9];
                    flyout.IsOpen = !flyout.IsOpen;
                });

            ExecuteSQLBtn.Click += ((sender, args) =>
                {
                    string cmd = SQLCMD.Text;
                    if (cmd.Length != 0)
                    {
                        BackgroundWorker executer = new BackgroundWorker();

                        executer.DoWork += (async(sender1, args1) =>
                        {
                            try
                            {
                               await _db.Database.ExecuteSqlCommandAsync(cmd);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString(),"ERROR");
                            }

                        });

                        executer.RunWorkerAsync();


                    }

                });
        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)Flyouts.Items[1];
            flyout.IsOpen = !flyout.IsOpen;
            Load();
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            if (File.Exists("CurrentAdmin.xml"))
            {
                File.Delete("CurrentAdmin.xml");
            }
        }

        private void BackupDB_Click(object sender, RoutedEventArgs e)
        {
            DatabaseBackup.Backup();
        }


        #region Load

        private void Load()
        {
            _loader = new BackgroundWorker();
            _loader.DoWork += _loader_DoWork;
            _loader.RunWorkerCompleted += _loader_RunWorkerCompleted;
            _loader.RunWorkerAsync();

        }

        async void _loader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DevicesGrid.ItemsSource = _devices;
            LoadAdmins();
            AdminsList.ItemsSource = _admins;
            AdminsGrid.ItemsSource = _admins;
            var flyout = (Flyout)Flyouts.Items[1];
            flyout.IsOpen = !flyout.IsOpen;
            var flyout2 = (Flyout)Flyouts.Items[2];
            flyout2.IsOpen = !flyout.IsOpen;
            for (int i = 4; i < AdminsGrid.Columns.Count; i++)
            {
                AdminsGrid.Columns[i].Visibility = Visibility.Collapsed;
            }
            HelpTextsGrid.ItemsSource = await LoadHelpTexts();
        }

        void _loader_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadDevices();
        }

        private void LoadDevices()
        {
            _devices = _db.Devices.ToList();
        }
        private void LoadAdmins()
        {
            _admins = _db.Admins.ToList();

        }
        private IEnumerable<Device> _devices;
        private IEnumerable<Admin> _admins;
        private readonly AndroidToolkitDB _db;
        private BackgroundWorker _loader;
        #endregion

        #region AdminOptions
        private void OpenAdminOptions_Click(object sender, RoutedEventArgs e)
        {
            SetAdminData();
            var flyout = (Flyout)Flyouts.Items[0];
            flyout.IsOpen = !flyout.IsOpen;
        }
        private void SetAdminData()
        {
            var data = new AdminManager().AdminData();
            AdminID.Text = string.Format("ID: {0}", data[0]);
            AdminName.Text = string.Format("Username: {0}", data[1]);
        }
        #endregion

        #region Devices

        private void OpenNewDevice_Click(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)Flyouts.Items[4];
            flyout.IsOpen = !flyout.IsOpen;
        }
        private void EditDevices_Click(object sender, RoutedEventArgs e)
        {
            Edit();
        }
        private void DeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            if (_device != null)
            {
                if (_deviceManager.Delete(_db, _device))
                {
                    ErrorText.Text = "DEVICE DELETED SUCCESSFULLY";
                    var flyout = (Flyout)Flyouts.Items[7];
                    flyout.IsOpen = !flyout.IsOpen;
                }
                else
                {
                    ErrorText.Text = "UNEXPECTED ERROR";
                    var flyout = (Flyout)Flyouts.Items[7];
                    flyout.IsOpen = !flyout.IsOpen;
                }

            }
            else
            {
                ErrorText.Text = "You must select device for delete";
                var flyout = (Flyout)Flyouts.Items[7];
                flyout.IsOpen = !flyout.IsOpen;
            }
        }

        private readonly DeviceManager _deviceManager;
        private Device _device = null;

        private void DevicesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _device = (Device)DevicesGrid.SelectedItem;
            }
            catch (Exception)
            {

            }

        }
        private void DevicesGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                _deviceManager.Edit(_db);
            }
            catch (Exception)
            {
            }

        }

        private void Edit()
        {
            if (_deviceManager.Edit(_db))
            {
                ErrorText.Text = "DEVICE EDITED SUCCESSFULLY";
                var flyout = (Flyout)Flyouts.Items[7];
                flyout.IsOpen = !flyout.IsOpen;
            }
            else
            {
                ErrorText.Text = "UNEXPECTED ERROR";
                var flyout = (Flyout)Flyouts.Items[7];
                flyout.IsOpen = !flyout.IsOpen;
            }
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)Flyouts.Items[1];
            flyout.IsOpen = !flyout.IsOpen;
            Load();
        }

        private void CreateDevice_Click(object sender, RoutedEventArgs e)
        {
            _newDeviceName = NewDeviceName.Text;
            _newDeviceCWM = NewDeviceCWM.Text;
            _newDeviceCWMTouch = NewDeviceCWMTouch.Text;
            _newDeviceTWRP = NewDeviceTWRP.Text;
            _newDeviceDeviceImg = NewDeviceImage.Text;
            if ((!string.IsNullOrEmpty(_newDeviceName)))
            {
                if (_deviceManager.CreateDevice(_db, _newDeviceName, _newDeviceCWM, _newDeviceCWMTouch, _newDeviceTWRP, _newDeviceDeviceImg))
                {
                    ErrorText.Text = "DEVICE CREATED SUCCESSFULLY";
                    var flyout = (Flyout)Flyouts.Items[7];
                    flyout.IsOpen = !flyout.IsOpen;
                    NewDeviceName.Text = null;
                    NewDeviceCWM.Text = null;
                    NewDeviceCWMTouch.Text = null;
                    NewDeviceTWRP.Text = null;
                    NewDeviceImage.Text = null;
                }

                else
                {
                    ErrorText.Text = "UNEXPECTED ERROR";
                    var flyout = (Flyout)Flyouts.Items[7];
                    flyout.IsOpen = !flyout.IsOpen;
                }
            }
            else
            {
                ErrorText.Text = "You must enter device name";
                var flyout = (Flyout)Flyouts.Items[7];
                flyout.IsOpen = !flyout.IsOpen;
            }
        }
        private string _newDeviceName;
        private string _newDeviceCWM;
        private string _newDeviceCWMTouch;
        private string _newDeviceTWRP;
        private string _newDeviceDeviceImg;
        #endregion

        #region Admins
        private void OpenCreateAdmin_Click(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)Flyouts.Items[5];
            flyout.IsOpen = !flyout.IsOpen;
        }
        private void OpenAdminManagment_Click(object sender, RoutedEventArgs e)
        {
            var data = _adminManager.AdminData();
            if (data[0] == _adminManager.MasterAdminID && data[1] == _adminManager.MasterAdminUsername)
            {
                AdminsGrid.ItemsSource = _admins;
                for (int i = 4; i < AdminsGrid.Columns.Count; i++)
                {
                    AdminsGrid.Columns[i].Visibility = Visibility.Collapsed;
                }
                var flyout = (Flyout)Flyouts.Items[6];
                flyout.IsOpen = !flyout.IsOpen;
            }
            else
            {
                ErrorText.Text = "PERMISSIONS DENIED";
                var flyout = (Flyout)Flyouts.Items[7];
                flyout.IsOpen = !flyout.IsOpen;
            }

        }
        private void CreateAdmin_Click(object sender, RoutedEventArgs e)
        {
            var data = new AdminManager().AdminData();
            if (data[0] == _adminManager.MasterAdminID && data[1] == _adminManager.MasterAdminUsername)
            {
                _newAdminName = NewAdminName.Text;
                _newAdminUsername = NewAdminUsername.Text;
                _newAdminPassword = NewAdminPassword.Password;
                if ((!string.IsNullOrEmpty(_newAdminName)) && (!string.IsNullOrEmpty(_newAdminUsername)) && (!string.IsNullOrEmpty(_newAdminPassword)))
                {
                    if (_adminManager.CreateAdmin(_db, _newAdminName, _newAdminUsername, _newAdminPassword))
                    {
                        NewAdminName.Text = null;
                        NewAdminUsername.Text = null;
                        NewAdminPassword.Password = null;
                        ErrorText.Text = "ADMIN CREATED SUCCESSFULLY";
                        var flyout = (Flyout)Flyouts.Items[7];
                        flyout.IsOpen = !flyout.IsOpen;
                    }
                    else
                    {
                        ErrorText.Text = "UNEXPECTED ERROR";
                        var flyout = (Flyout)Flyouts.Items[7];
                        flyout.IsOpen = !flyout.IsOpen;
                    }

                }
                else
                {
                    ErrorText.Text = "You must fill all field for new admin";
                    var flyout = (Flyout)Flyouts.Items[7];
                    flyout.IsOpen = !flyout.IsOpen;
                }
            }
            else
            {
                ErrorText.Text = "PERMISSIONS DENIED";
                var flyout = (Flyout)Flyouts.Items[7];
                flyout.IsOpen = !flyout.IsOpen;
            }

        }
        private string _newAdminName;
        private string _newAdminUsername;
        private string _newAdminPassword;

        private void AdminsGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                _admin = (Admin)DevicesGrid.SelectedItem;
            }
            catch (Exception)
            {

            }
        }

        private void AdminsGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _admin = (Admin)DevicesGrid.SelectedItem;
            }
            catch (Exception)
            {

            }
        }

        private Admin _admin = null;

        private void EditAdmin_OnClick(object sender, RoutedEventArgs e)
        {

            if (_adminManager.Edit(_db))
            {
                ErrorText.Text = "ADMIN EDITED SUCCESSFULLY";
                var flyout = (Flyout)Flyouts.Items[7];
                flyout.IsOpen = !flyout.IsOpen;
            }
            else
            {
                ErrorText.Text = "UNEXPECTED ERROR";
                var flyout = (Flyout)Flyouts.Items[7];
                flyout.IsOpen = !flyout.IsOpen;
            }

        }

        private void DeleteAdmin_OnClick(object sender, RoutedEventArgs e)
        {
            if (_admin != null)
            {
                if (_adminManager.Delete(_db, _admin))
                {
                    ErrorText.Text = "ADMIN DELETED SUCCESSFULLY";
                    var flyout = (Flyout)Flyouts.Items[7];
                    flyout.IsOpen = !flyout.IsOpen;
                }
                else
                {
                    ErrorText.Text = "UNEXPECTED ERROR";
                    var flyout = (Flyout)Flyouts.Items[7];
                    flyout.IsOpen = !flyout.IsOpen;
                }

            }
            else
            {
                ErrorText.Text = "You must select admin for delete";
                var flyout = (Flyout)Flyouts.Items[7];
                flyout.IsOpen = !flyout.IsOpen;
            }
        }

        private void AdminChangePassword_OnClick(object sender, RoutedEventArgs e)
        {
            var data = _adminManager.AdminData();
            int idC = Convert.ToInt32(data[0]);

            if (!string.IsNullOrEmpty(NewPassword.Password) && !string.IsNullOrEmpty(OldPassword.Password))
            {
                if (_adminManager.ChangePassword(_db, idC, NewPassword.Password, OldPassword.Password))
                {
                    this.ShowMessageAsync("SUCCESS!", "Password changed succesfully! :)",
                        MahApps.Metro.Controls.Dialogs.MessageDialogStyle.Affirmative);
                    AdminsGrid.ItemsSource = _admins;
                    for (int i = 4; i < AdminsGrid.Columns.Count; i++)
                    {
                        AdminsGrid.Columns[i].Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    this.ShowMessageAsync("ERROR!", "Password was not changed",
                       MahApps.Metro.Controls.Dialogs.MessageDialogStyle.Affirmative);
                }
            }
            else
            {
                this.ShowMessageAsync("ERROR", "Your password was not changed. You must enter valid old password and fill all fields. :(",
                      MahApps.Metro.Controls.Dialogs.MessageDialogStyle.Affirmative);
            }
        }
        private readonly AdminManager _adminManager;
        #endregion

        #region HelpTexts
        private async void OpenHelpTexts_Click(object sender, RoutedEventArgs e)
        {
            HelpTextsGrid.ItemsSource = await LoadHelpTexts();
            var flyout = (Flyout)Flyouts.Items[8];
            flyout.IsOpen = !flyout.IsOpen;
        }

        private async Task<IEnumerable<HelpText>> LoadHelpTexts()
        {
            return await Task.Run(() => _db.HelpTexts.ToListAsync());
        }

        private async Task<bool> DeleteHelpTextTask(HelpText text)
        {
            _db.HelpTexts.Remove(text);
            return Convert.ToBoolean(await _db.SaveChangesAsync());
        }

        private void HelpTextsGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _helpText = (HelpText)HelpTextsGrid.SelectedItem;
            }
            catch (Exception ex)
            {
                MetroMessageBoxHelper.ShowBox("ERROR", ex.ToString());
            }

        }

        private HelpText _helpText;

        private async void EditHelpTextsOnClick(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => _db.SaveChangesAsync());
            HelpTextsGrid.ItemsSource = await LoadHelpTexts();
        }

        private async void DeleteHelpText_OnClick(object sender, RoutedEventArgs e)
        {
            if (_helpText != null)
            {
                bool passed = await DeleteHelpTextTask(_helpText);
                if (passed)
                {
                    await this.ShowMessageAsync("SUCCESS", "Help Text removed");
                }
                else
                {
                    await this.ShowMessageAsync("ERROR", "Unknown error");
                }
            }
            else
            {
                await this.ShowMessageAsync("ERROR", "Select Help Text for remove");
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
