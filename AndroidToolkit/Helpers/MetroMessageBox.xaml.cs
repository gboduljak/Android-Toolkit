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

namespace AndroidToolkit.Helpers
{
    /// <summary>
    /// Interaction logic for MetroMessageBox.xaml
    /// </summary>
    public partial class MetroMessageBox : MetroWindow
    {
        private string _content;
        public MetroMessageBox(string title, string content)
        {
            Title = title;
            _content = content;
            InitializeComponent();
            MainContent.Text = content;
        }
    }
}
