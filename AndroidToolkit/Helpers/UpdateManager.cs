using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AndroidToolkit.Helpers
{
    public static class UpdateManager
    {
        public static async Task<XDocument> LoadVersionsXML()
        {
            return await Task.Factory.StartNew(() => XDocument.Load("http://www.android-toolkit.somee.com/Content/Versions.xml"));
        }
        public static async Task<bool> IsUpdateAvailable()
        {
            var doc = await LoadVersionsXML();
            var versions = doc.Root.Descendants("AndroidToolkitDBVersion").OrderByDescending(v => v.Attribute("Number").Value).ToList();
            var latestVersion = versions.ElementAt(0);
            int currentVersionNumber;
            int.TryParse(File.ReadAllText("dbversion.txt"), out currentVersionNumber);
            int latestVersionNumber;
            int.TryParse(latestVersion.Attribute("Number").Value, out latestVersionNumber);
            if (latestVersionNumber > currentVersionNumber)
            {
                return true;
            }
            return false;
        }
        public static async Task<bool> Update()
        {
            try
            {
                var doc = await LoadVersionsXML();
                var versions = doc.Root.Descendants("AndroidToolkitDBVersion").OrderByDescending(v => v.Attribute("Number").Value).ToList();
                var latestVersion = versions.ElementAt(0);
                var downloadLink = latestVersion.Attribute("Download").Value;
                int latestVersionNumber;
                int.TryParse(latestVersion.Attribute("Number").Value, out latestVersionNumber);
                File.Delete("AndroidToolkitDB.sdf");
                WebClient downloader = new WebClient();
                await Task.Factory.StartNew(() => downloader.DownloadFile(new Uri(downloadLink), "AndroidToolkitDB.sdf"));
                await Task.Run(() => File.WriteAllText("dbversion.txt", latestVersionNumber.ToString()));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
