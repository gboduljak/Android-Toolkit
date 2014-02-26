using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidToolkit.Helpers
{
    internal static class MetroMessageBoxHelper
    {
        public static void ShowBox(string title, string content, int width=0, int height=0)
        {
            var box = new MetroMessageBox(title, content);
            if(width>0)
            {
                box.Width = width;
            }
            if (height > 0)
            {
                box.Height = height;
            }
            box.ShowDialog();
            
        }
    }
}
