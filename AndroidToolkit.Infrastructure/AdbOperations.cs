using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using AndroidToolkit.Infrastructure;
using AndroidToolkit.Infrastructure.Classes;
using System.Diagnostics;



namespace AndroidToolkit.Infrastructure
{
    public class AdbOperations
    {
        public AdbOperations()
        {
            _executor = new CommandExecutor();
            _remoteAdb = new RemoteADB();
        }

        private readonly CommandExecutor _executor;
        private readonly RemoteADB _remoteAdb;


        public void Prepare()
        {
            Command _command = new Command("adb start-server");
            _executor.ExecuteSingleCommandNoReturn(_command, false);
        }

        public void KillAdb()
        {
            Process[] processes = Process.GetProcesses();

            foreach (Process p in processes)
            {
                if (p.ProcessName.ToLower().Contains("adb"))
                {
                    p.Kill();
                    return;
                }
            }
        }

        public string SetRemoteADB(string port, string ip, bool createWindow)
        {
            return _remoteAdb.SetRemoteADB(port, ip, createWindow);
        }

        public string SetUSB(bool createWindow)
        {
            return _remoteAdb.SetUSB(createWindow);
        }

        public string ExecuteSingleCommand(string text, bool createWindow, string target = null)
        {
            Command _command = new Command(text);
            return _executor.ExecuteSingleCommandReturn(_command, createWindow);
        }

        public string ExecuteMultipleCommands(List<string> commands, bool createWindow, string target = null)
        {
            List<Command> Commands = new List<Command>();
            foreach (var cmd in commands)
            {
                Commands.Add(new Command(cmd));
            }
            return _executor.ExecuteMultipleCommandsReturn(Commands, createWindow);
        }

        #region Reboot

        public string Reboot(bool createWindow, string target = null)
        {
            Command _command;
            if (target.Length != 0)
            {
                _command = new Command(string.Format("adb -s {0} reboot", target));
                return _executor.ExecuteSingleCommandReturn(_command, createWindow);
            }
            _command = new Command("adb reboot");
            return _executor.ExecuteSingleCommandReturn(_command, createWindow);
        }

        public string RebootRecovery(bool createWindow, string target = null)
        {
            Command _command;
            if (target.Length != 0)
            {
                _command = new Command(string.Format("adb -s {0} reboot recovery", target));
                return _executor.ExecuteSingleCommandReturn(_command, createWindow);
            }
            _command = new Command("adb reboot recovery");
            return _executor.ExecuteSingleCommandReturn(_command, createWindow);
        }

        public string RebootBootloader(bool createWindow, string target = null)
        {
            Command _command;
            if (target.Length != 0)
            {
                _command = new Command(string.Format("adb -s {0} reboot-bootloader", target));
                return _executor.ExecuteSingleCommandReturn(_command, createWindow);
            }
            _command = new Command("adb reboot-bootloader");
            return _executor.ExecuteSingleCommandReturn(_command, createWindow);
        }
        #endregion

        public string ListAdbDevices(bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command("adb devices"), createWindow);
        }

        public string ListApps(bool createWindow, string target = null)
        {
            if (target.Length != 0)
            {
                return _executor.ExecuteSingleCommandReturn(new Command(string.Format("adb -s {0} shell pm list packages", target)), createWindow);
            }
            return _executor.ExecuteSingleCommandReturn(new Command("adb shell pm list packages"), createWindow);
        }

        #region Push and Pull

        public string Push(List<string> files, string location, bool createWindow, string target = null)
        {
            List<Command> Commands = new List<Command>();
            if (target.Length != 0)
            {
                foreach (var file in files)
                {
                    string filePath = string.Format(@"""{0}""", file);
                    Commands.Add(new Command(string.Format("adb -s {0} push {1} {2}", target, filePath, location)));
                }
                return _executor.ExecuteMultipleCommandsReturn(Commands, createWindow);
            }
            foreach (var file in files)
            {
                string filePath = string.Format(@"""{0}""", file);
                Commands.Add(new Command(string.Format("adb push {0} {1}", filePath, location)));
            }
            return _executor.ExecuteMultipleCommandsReturn(Commands, createWindow);
        }

        public string Pull(List<string> files, string location, bool createWindow, string target = null)
        {
            List<Command> Commands = new List<Command>();
            if (target.Length != 0)
            {
                foreach (var file in files)
                {
                    string locationPath = string.Format(@"""{0}""", location);
                    Commands.Add(new Command(string.Format("adb -s {0} pull {1} {2}", target, file, locationPath)));
                }
                return _executor.ExecuteMultipleCommandsReturn(Commands, createWindow);
            }
            foreach (var file in files)
            {
                string locationPath = string.Format(@"""{0}""", location);
                Commands.Add(new Command(string.Format("adb pull {0} {1}", file, locationPath)));
            }
            return _executor.ExecuteMultipleCommandsReturn(Commands, createWindow);
        }
        #endregion

        #region Apks
        public string InstallApk(string apk, bool systemApp, bool createWindow, string target = null)
        {
            Command cmd;
            if (target.Length != 0)
            {
                if (systemApp == false)
                {
                    string appPath = string.Format(@"""{0}""", apk);
                    cmd = new Command(string.Format("adb -s {0} install {1}", target, appPath));
                    return _executor.ExecuteSingleCommandReturn(cmd, createWindow);
                }
                else
                {
                    string appPath = string.Format(@"""{0}""", apk);
                    string cmd2 = string.Format("adb -s {0} push {1} /system/app", target, appPath);
                    List<Command> cmds = new List<Command>();
                    cmds.Add(new Command(string.Format("adb -s {0} remount", target)));
                    cmds.Add(new Command(cmd2));
                    return _executor.ExecuteMultipleCommandsReturn(cmds, createWindow);
                }
            }
            if (systemApp == false)
            {
                string appPath = string.Format(@"""{0}""", apk);
                cmd = new Command(string.Format("adb install {0}", appPath));
                return _executor.ExecuteSingleCommandReturn(cmd, createWindow);
            }
            else
            {
                string appPath = string.Format(@"""{0}""", apk);
                string cmd2 = string.Format("adb push {0} /system/app", appPath);
                List<Command> cmds = new List<Command>();
                cmds.Add(new Command("adb remount"));
                cmds.Add(new Command(cmd2));
                return _executor.ExecuteMultipleCommandsReturn(cmds, createWindow);
            }

        }
        public string UninstallApp(string app, bool systemApp, bool createWindow, string target = null)
        {
            Command cmd;
            List<Command> cmds;
            if (target.Length != 0)
            {
                if (systemApp == false)
                {
                    cmd = new Command(string.Format("adb -s {0} uninstall {1}", target, app));
                    return _executor.ExecuteSingleCommandReturn(cmd, createWindow);
                }
                else
                {
                    cmds = new List<Command>();
                    cmds.Add(new Command(string.Format("adb -s {0} remount", target)));
                    cmds.Add(new Command(string.Format("adb -s {0} shell rm /system/app/{1}", target, app)));
                    return _executor.ExecuteMultipleCommandsReturn(cmds, createWindow);
                }
            }
            if (systemApp == false)
            {
                cmd = new Command(string.Format("adb uninstall {0}", app));
                return _executor.ExecuteSingleCommandReturn(cmd, createWindow);
            }
            else
            {
                cmds = new List<Command>();
                cmds.Add(new Command("adb remount"));
                cmds.Add(new Command(string.Format("adb shell rm /system/app/{0}", app)));
                return _executor.ExecuteMultipleCommandsReturn(cmds, createWindow);
            }


        }
        #endregion

        #region Copy/Delete/Move

        public string Copy(string from, string to, bool createWindow, string target = null)
        {
            if (target.Length != 0)
            {
                return _executor.ExecuteSingleCommandReturn(new Command(string.Format("adb -s {0} shell cat {1} > {2}", target, from, to)), createWindow);
            }
            return _executor.ExecuteSingleCommandReturn(new Command(string.Format("adb shell cp {0} {1}", from, to)), createWindow);
        }
        public string Move(string from, string to, bool createWindow, string target = null)
        {
            if (target.Length != 0)
            {
                return _executor.ExecuteSingleCommandReturn(new Command(string.Format("adb -s {0} shell mv {1} {2}", target, from, to)), createWindow);
            }
            return _executor.ExecuteSingleCommandReturn(new Command(string.Format("adb shell mv {0} {1}", from, to)), createWindow);
        }
        public string Delete(string file, bool createWindow, string target = null)
        {
            if (target.Length != 0)
            {
                return _executor.ExecuteSingleCommandReturn(new Command(string.Format("adb -s {0} shell rm {1}", target, file)), createWindow);
            }
            return _executor.ExecuteSingleCommandReturn(new Command(string.Format("adb shell rm {0}", file)), createWindow);
        }
        #endregion

        public string Sideload(string file, bool createWindow, string target = null)
        {
            file = string.Format(@"""{0}""", file);
            if (target.Length != 0)
            {
                return _executor.ExecuteSingleCommandReturn(new Command(string.Format("adb -s {0} sideload {1}", target, file)), createWindow);
            }
            return _executor.ExecuteSingleCommandReturn(new Command(string.Format("adb sideload {0}", file)), createWindow);
        }
        public string DeviceInfo(bool createWindow, string target = null)
        {
            if (target.Length != 0)
            {
                string cmd = string.Format(@"adb -s {0} shell cat /system/build.prop", target);
                return _executor.ExecuteSingleCommandReturn(new Command(cmd), createWindow);
            }

            string cmd2 = @"adb shell cat /system/build.prop";
            return _executor.ExecuteSingleCommandReturn(new Command(cmd2), createWindow);

        }
        public string DeviceName(bool createWindow, string target = null)
        {
            if (target.Length != 0)
            {
                string cmd = string.Format(@"adb -s {0} shell getprop ro.product.model", target);
                return _executor.ExecuteSingleCommandReturn(new Command(cmd), createWindow);
            }
            string cmd2 = @"adb shell getprop ro.product.model";
            return _executor.ExecuteSingleCommandReturn(new Command(cmd2), createWindow);

        }
        public string DeviceOsVersion(bool createWindow, string target = null)
        {
            if (target.Length != 0)
            {
                string cmd = string.Format(@"adb -s {0} shell getprop ro.build.version.release", target);
                return _executor.ExecuteSingleCommandReturn(new Command(cmd), createWindow);
            }
            string cmd2 = @"adb shell getprop ro.build.version.release";
            return _executor.ExecuteSingleCommandReturn(new Command(cmd2), createWindow);
        }
        public string DeviceManufacturer(bool createWindow, string target = null)
        {
            if (target.Length != 0)
            {
                string cmd = string.Format(@"adb -s {0} shell getprop ro.product.manufacturer", target);
                return _executor.ExecuteSingleCommandReturn(new Command(cmd), createWindow);
            }
            string cmd2 = @"adb shell getprop ro.product.manufacturer";
            return _executor.ExecuteSingleCommandReturn(new Command(cmd2), createWindow);
        }
        public string DeviceCodename(bool createWindow, string target = null)
        {
            if (target.Length != 0)
            {
                string cmd = string.Format(@"adb -s {0} shell getprop ro.product.name", target);
                return _executor.ExecuteSingleCommandReturn(new Command(cmd), createWindow);
            }
            string cmd2 = @"adb shell getprop ro.product.name";
            return _executor.ExecuteSingleCommandReturn(new Command(cmd2), createWindow);
        }
        public DeviceInfo DeviceDetailedInfo(bool createWindow, string target = null)
        {
            string name =
                StringLinesRemover.ForgetLastLine(StringLinesRemover.RemoveLine(
                    DeviceName(createWindow, target), 4));

            string codename =
              StringLinesRemover.ForgetLastLine(StringLinesRemover.RemoveLine(
                  DeviceCodename(createWindow, target), 4));

            string manufacturer =
                StringLinesRemover.ForgetLastLine(StringLinesRemover.RemoveLine(
                    DeviceManufacturer(createWindow, target), 4));

            string os = StringLinesRemover.ForgetLastLine(StringLinesRemover.RemoveLine(DeviceOsVersion(createWindow, target), 4));
            string osDetails = string.Empty; ;
            string root = _executor.ExecuteSingleCommandReturn(new Command("adb remount"), createWindow);
            bool isRoot = false;
            if (os.Contains("1.5"))
            {
                osDetails = "CUPCAKE";
            }
            if (os.Contains("1.6"))
            {
                osDetails = "DONUT";
            }
            if (os.Contains("2"))
            {
                osDetails = "ECLAIR";
            }
            if (os.Contains("2.2") || os.Contains("2.2.3"))
            {
                osDetails = "FROYO";
            }
            if (os.Contains("2.3"))
            {
                osDetails = "GINGERBREAD";
            }
            if (os.Contains("3.0") || os.Contains("3.1") || os.Contains("3.2"))
            {
                osDetails = "HONEYCOMB";
            }
            if (os.Contains("4.0"))
            {
                osDetails = "ICE CREAM SANDWICH";
            }
            if (os.Contains("4.1") || os.Contains("4.2") || os.Contains("4.3"))
            {
                osDetails = "JELLY BEAN";
            }
            if (os.Contains("4.4"))
            {
                osDetails = "KIT KAT";
            }
            if (root.Contains("remount s"))
            {
                isRoot = true;
            }
            return new DeviceInfo(name, manufacturer, codename, os, osDetails, isRoot);
        }


        #region Classes

        private sealed class RemoteADB
        {
            public RemoteADB()
            {
                _executor = new CommandExecutor();
                _commands = new List<Command>();
            }
            public string SetRemoteADB(string port, string ip, bool createWindow)
            {
                _finalAddress = string.Format("{0}:{1}", ip, port);
                _commands.Add(new Command(string.Format("adb tcpip {0}", port)));
                _commands.Add(new Command(string.Format("adb connect {0}", _finalAddress)));
                return _executor.ExecuteMultipleCommandsReturn(_commands, createWindow);
            }
            public string SetUSB(bool createWindow)
            {
                return _executor.ExecuteSingleCommandReturn(new Command("adb usb"), createWindow);
            }
            private readonly List<Command> _commands;
            private readonly CommandExecutor _executor;
            private string _finalAddress;
        }

        #endregion
    }
}