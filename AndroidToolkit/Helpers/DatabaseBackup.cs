using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidToolkit.Helpers
{
    internal static class DatabaseBackup
    {
        public static bool Backup()
        {
            try
            {
                const string destinationPath = @"backup\";
                string destinationFileName = "AndroidToolkitDB-" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".sdf";
                const string sourceFile = @"AndroidToolkitDB.sdf";
                string destinationFile = Path.Combine(destinationPath, destinationFileName);
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }
                File.Copy(sourceFile, destinationFile, true);
                return true;
            }
            catch (Exception ex)
            {
                MetroMessageBoxHelper.ShowBox("ERROR",ex.ToString(),400,200);
                return false;
            }
        }
    }
}
