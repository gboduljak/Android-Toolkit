using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace AndroidToolkit
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Help : MetroWindow
    {
        private readonly AndroidToolkitDB _db;
        private readonly Style _textBlockStyle;
        private void ShowFlyout(int index)
        {
            var flyout = (Flyout)Flyouts.Items[index];
            if (flyout != null)
            {
                flyout.IsOpen = !flyout.IsOpen;
            }
        }

        public Help()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _db = new AndroidToolkitDB();
            _textBlockStyle = (Style)Resources["TileTitle"];

        }
        private async void Help_OnLoaded(object sender, RoutedEventArgs e)
        {
            await LoadFirst();
        }

        #region Rectangle
        private Rectangle DrawRectangle(string obj, int width, int height, SolidColorBrush brush=null)
        {
            var rect = new Rectangle {Fill = (SolidColorBrush) Resources["AccentColorBrush"], Width = width, Height = height};
            if (brush != null)
            {
                rect.Fill =brush;
            }
            rect.OpacityMask = new VisualBrush((Visual)Resources[obj]);
            rect.Visibility = Visibility.Visible;
            return rect;
        }
        #endregion

        #region Tasks

        private async Task LoadFirst()
        {
            this.ShowFlyout(0);
            var entity = await Task.Run(() => _db.HelpTexts.Where(h => h.Name == "TERMS").FirstOrDefaultAsync());
            this.ShowFlyout(0);
            if (entity != null)
            {
                this.HelpTextName.Text = entity.Name;
                var textblock = new TextBlock()
                                {
                                    Style = _textBlockStyle,
                                    TextWrapping = TextWrapping.Wrap
                                };
                HelpTextContent.Content = textblock;
                var strings = entity.Text.Split('#');
                foreach (string s in strings)
                {
                    textblock.Text = textblock.Text + "\n" + s;
                }
            }
            else
            {
                await this.ShowMessageAsync("Error", "Unknown error has occured :(");
            }


        }

        private async Task LoadHowItWorks()
        {
            this.ShowFlyout(0);
            var entity = await Task.Run(() => _db.HelpTexts.Where(h => h.Name == "HOW IT WORKS").FirstOrDefaultAsync());
            this.ShowFlyout(0);
            if (entity != null)
            {
                HelpTextName.Text = entity.Name;
                var textblock = new TextBlock()
                                {
                                    Style = _textBlockStyle,
                                    TextWrapping = TextWrapping.Wrap
                                };
                var strings = entity.Text.Split('#');
                foreach (var s in strings)
                {
                    textblock.Text = textblock.Text + "\n" + s;
                }
                HelpTextContent.Content = textblock;
            }
            else
            {
                await this.ShowMessageAsync("Error", "Unknown error has occured :(");
            }

        }

        private async Task LoadRemoteADB()
        {
            this.ShowFlyout(0);
            var entity = await Task.Run(() => _db.HelpTexts.Where(h => h.Name == "REMOTE ADB").FirstOrDefaultAsync());
            this.ShowFlyout(0);
            if (entity != null)
            {
                HelpTextName.Text = entity.Name;
                var textblock = new TextBlock()
                {
                    Style = _textBlockStyle,
                    TextWrapping = TextWrapping.Wrap
                };
                var strings = entity.Text.Split('#');
                foreach (var s in strings)
                {
                    textblock.Text = textblock.Text + "\n" + s;
                }
                HelpTextContent.Content = textblock;
            }
            else
            {
                await this.ShowMessageAsync("Error", "Unknown error has occured :(");
            }
        }

        private async Task<string> LoadChangelog()
        {
            string text = string.Empty;
            if (File.Exists("changelog.log"))
            {
                text = await Task.Run(() => File.ReadAllText("changelog.log"));
            }
            return text;
        }

        private async Task<IEnumerable<HelpText>> SearchTexts(string searchTerm)
        {
            var texts = new List<HelpText>();
            if (searchTerm.Length != 0)
            {
                texts.AddRange(_db.HelpTexts.Where(helpText => helpText.Text.Contains(searchTerm)));
            }
            else
            {
                await this.ShowMessageAsync("ERROR", "You must enter search term");
            }
            return texts;
        }

        #endregion

        private async void HowItWorks_Click(object sender, RoutedEventArgs e)
        {
            await LoadHowItWorks();
        }

        private async void Terms_Click(object sender, RoutedEventArgs e)
        {
            await LoadFirst();
            RectanglesContentControl.Content = null;
        }

        private async void RemoteAdb_OnClick(object sender, RoutedEventArgs e)
        {
            await LoadRemoteADB();
            RectanglesContentControl.Content = null;
        }

        private async void Changelog_OnClick(object sender, RoutedEventArgs e)
        {
            string text = await LoadChangelog();
            RectanglesContentControl.Content = null;
            await this.ShowMessageAsync("CHANGELOG", text);
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            var texts = await SearchTexts(SearchBox.Text);
            string names = "Results found in sections:"+"\n"+ texts.Aggregate(string.Empty, (current, helpText) => current + "\n" + helpText.Name);
            await this.ShowMessageAsync("SEARCH RESULT/S", names);
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
