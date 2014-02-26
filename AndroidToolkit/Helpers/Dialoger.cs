using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidToolkit.Helpers
{
    internal class Dialoger
    {
        public static string ShowSingleChooseDialog(string filter)
        {
            string selectedFile=null;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckPathExists = true;
            dlg.Multiselect = false;
            dlg.Filter = filter;
            Nullable<bool> result = dlg.ShowDialog();
            if(result==true)
            {
                selectedFile = dlg.FileName;
            }
            return selectedFile;
        }
        public static List<string> ShowMultipleChooseDialog(string filter)
        {
            List<string> Files = new List<string>();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckPathExists = true;
            dlg.Multiselect = true;
            dlg.Filter = filter;
            Nullable<bool> result = dlg.ShowDialog();
            if(result==true)
            {
                foreach (var file in dlg.FileNames)
                {
                    Files.Add(file);
                }
            }
            return Files;
        }

    }
}
