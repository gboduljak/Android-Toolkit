using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AndroidToolkit.Helpers
{
    internal static class ConnectionChecker
    {
        public static bool IsConnectedToInternet
        {
            get
            {
                try
                {
                    HttpWebRequest hwebRequest = (HttpWebRequest)WebRequest.Create("http://www.google.com");
                    hwebRequest.Timeout = 10000;
                    HttpWebResponse hWebResponse = (HttpWebResponse)hwebRequest.GetResponse();
                    if (hWebResponse.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else return false;
                }
                catch { return false; }
            }
        }
    }
}
