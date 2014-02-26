using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidToolkit.Helpers.Devices
{
    internal sealed class DeviceManager
    {
        public DeviceManager()
        {
        }
        public bool CreateDevice(AndroidToolkitDB _db,string name, string cwm, string cwmTouch, string twrp, string deviceImg = null)
        {
            try
            {
                _device=new Device()
                {
                    Name=name,
                    CWMRecovery=cwm,
                    CWMTouchRecovery=cwmTouch,
                    TWRPRecovery=twrp,
                    DeviceImg=deviceImg
                };
                _db.Devices.Add(_device);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MetroMessageBoxHelper.ShowBox("Error", ex.ToString(), 400, 200);
               return false;
            }
          
        }
        public bool Edit(AndroidToolkitDB _db)
        {
            try
            {
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MetroMessageBoxHelper.ShowBox("Error", ex.ToString(), 400, 200);
                return false;
            }
        }
        public bool Delete(AndroidToolkitDB _db, Device device)
        {
            try
            {
                _db.Devices.Remove(device); 
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MetroMessageBoxHelper.ShowBox("Error", ex.ToString(), 400, 200);
                return false;
            }
        }

        private Device _device;
    }
}
