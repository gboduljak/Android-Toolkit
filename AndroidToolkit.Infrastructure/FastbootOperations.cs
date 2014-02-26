using System.Runtime.InteropServices;
using System.Security.Policy;
using AndroidToolkit.Infrastructure.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidToolkit.Infrastructure
{
    public partial class FastbootOperations 
    {
        public FastbootOperations()
        {
            _executor = new CommandExecutor();
        }
     
        private readonly CommandExecutor _executor;

        public void KillFastboot()
        {
            Process[] processes = Process.GetProcesses();

            foreach (Process p in processes)
            {
                if (p.ProcessName.ToLower().Contains("fastboot"))
                {
                    p.Kill();
                    return;
                }
            }
        }

        public string ListFastbootDevices(bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command ("fastboot devices"), createWindow);
        }

        #region Reboot
        public string Reboot(bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command("fastboot reboot"), createWindow);
        }
        public string RebootBootloader(bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command("fastboot reboot-bootloader"), createWindow);
        }
        public string RebootRecovery(bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command("fastboot reboot recovery"), createWindow);
        }
        #endregion

        #region Boot
        public string Boot(string file, bool createWindow)
        {
            string cmd = string.Format( @"fastboot boot ""{0}""",file);
            return _executor.ExecuteSingleCommandReturn(new Command(cmd), createWindow);
        }
        #endregion

        #region Execute
        public string ExecuteSingleCommand(string command, bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command(command), createWindow);
        }
        #endregion

        #region Bootloader

        public string Lock(bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command("fastboot oem lock"), createWindow);
        }
        public string Unlock(bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command("fastboot oem unlock"), createWindow);
        }
        public string GetIdentifierToken(bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command("fastboot oem get_identifier_token"), createWindow);
        }

        public string WriteCID(string cid,bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command(string.Format("fastboot oem writecid {0}",cid)), createWindow);
        }
        public string ReadCID(bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command("fastboot getvar cid"), createWindow);
        }
        #endregion

        #region Erase

        public string EraseSystem(bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command("fastboot erase system"), createWindow);
        }
        public string EraseRecovery(bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command("fastboot erase recovery"), createWindow);
        }
        public string EraseCache(bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command("fastboot erase cache"), createWindow);
        }
        public string EraseBoot(bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command("fastboot erase boot"), createWindow);
        }

        public string EraseUserdata(bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command("fastboot erase userdata"), createWindow);
        }
        #endregion

        #region ZipFlash
        public string FlashZips(bool createWindow, params string[] zips)
        {
            List<Command> Commands = new List<Command>();
            foreach (var zip in zips)
            {
                Commands.Add(new Command(string.Format("fastboot flash {0}", zip)));
            }
            return _executor.ExecuteMultipleCommandsReturn(Commands, createWindow);
        }
        #endregion

        #region Flash
        public string FlashSystem(string file, bool createwindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command(string.Format("fastboot flash system {0}", file)), createwindow);
        }
        public string FlashBoot(string file, bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command(string.Format("fastboot flash boot {0}", file)),createWindow);
        }
        public string FlashRecovery(string file, bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command(string.Format("fastboot flash recovery {0}", file)), createWindow);
        }
        public string FlashBootloader(string file, bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command(string.Format("fastboot flash bootloader {0}", file)), createWindow);
        }
        public string FlashRadio(string file, bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command(string.Format("fastboot flash radio {0}", file)), createWindow);
        }
        public string FlashUserdata(string file, bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command(string.Format("fastboot flash userdata {0}", file)), createWindow);
        }
        public string FlashUnlockToken(string file, bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command(string.Format("fastboot flash unlocktoken {0}", file)), createWindow);
        }
        #endregion



        public string ExecuteMultipleCommands(List<string> Commands, bool _createWindow)
        {
            List<Command> commands = new List<Command>();
            foreach (var cmd in Commands)
            {
                commands.Add(new Command(cmd));
            }
            return _executor.ExecuteMultipleCommandsReturn(commands, _createWindow);
        }
    }
}
