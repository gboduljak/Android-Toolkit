using AndroidToolkit.Helpers;
using AndroidToolkit.Helpers.Admin;
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

namespace AndroidToolkit
{
    /// <summary>
    /// Interaction logic for AdminLoger.xaml
    /// </summary>
    public partial class AdminLoger : MetroWindow
    {
        public AdminLoger()
        {
            InitializeComponent();
            _adminManager = new AdminManager();
        }

        private async void LogIn_Click(object sender, RoutedEventArgs e)
        {
            _username = Username.Text;
            _password = Password.Password;

            if((!string.IsNullOrEmpty(_username)) &&(!string.IsNullOrEmpty(_password)))
            {
                if (await _adminManager.LogIn(_username, _password))
                {
                    MetroMessageBoxHelper.ShowBox("LOG IN SUCCESS", string.Format("Welcome {0}. You have logged yourself successfully.", _username));
                    new Admin().Show();
                    this.Close();
                }
                else
                {
                    ErrorText.Text = "Admin with specified creditials does not exist. :(";
                    ShowError();
                }
            }
            else
            {
                ErrorText.Text = "You must enter username and password";
                ShowError();
            }
           
        }

        private void ShowError()
        {
            var flyout = (Flyout)Flyouts.Items[0];
            flyout.IsOpen = !flyout.IsOpen;
        }
        private string _username;
        private string _password;
        private readonly AdminManager _adminManager;
    }
}
