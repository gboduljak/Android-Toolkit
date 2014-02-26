using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidToolkit.Infrastructure
{
    public class DeviceInfo
    {
        private readonly string _deviceName;
        private readonly string _deviceCodename;
        private readonly string _deviceManufacturer;
        private readonly string _os;
        private readonly string _osShort;
        private readonly bool isRooted;

        public DeviceInfo(string name, string manufacturer,string deviceCodename,string os, string osShort, bool isRooted)
        {
            this._deviceName = name;
            this._deviceCodename = deviceCodename;
            this._deviceManufacturer = manufacturer;
            this._os = os;
            this._osShort = osShort;
            this.isRooted = isRooted;
        }
        public string DeviceName
        {
            get
            {
                return _deviceName;
            }
        }
        public string DeviceCodename
        {
            get
            {
                return _deviceCodename;
            }
        }
        public string DeviceManufacturer
        {
            get
            {
                return _deviceManufacturer;
            }
        }
        public string OsVersion
        {
            get
            {
                return _os;
            }
        }
        public string OsVersionShort
        {
            get
            {
                return _osShort;
            }
        }
        public bool IsRooted
        {
            get
            {
                return isRooted;
            }
        }
    }
}
