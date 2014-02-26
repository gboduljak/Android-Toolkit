using AndroidToolkit.Infrastructure.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidToolkit.Infrastructure
{
    public class AdvancedOperations
    {
        public AdvancedOperations()
        {
            _backup = new ADBBackup();
            _rootExploit=new RootExploit();
            _executor=new CommandExecutor();
        }

        #region Backup
        public string FullADBBackup(string path, bool createWindow)
        {
            return _backup.All(path, createWindow);
        }
        public string ADBNoSystemAppsBackup(string path, bool createWindow)
        {
            return _backup.NoSystemApps(path, createWindow);
        }
        public string ADBSystemAppsBackup(string path, bool createWindow)
        {
            return _backup.SystemApps(path, createWindow);
        }
        public string ADBSDBackup(string path, bool createWindow)
        {
            return _backup.SDBackup(path, createWindow);
        }
        #endregion

        #region Restore

        public string RestoreADBBackup(string name, bool createWindow)
        {
            return new ADBRestore(name, createWindow).Restore();
        }

        #endregion

        #region Logcat
        public string Logcat(string path, bool createWindow)
        {
            return _executor.ExecuteSingleCommandReturn(new Command(string.Format("adb logcat -d > {0}",path)),createWindow);
        }

        #endregion

        #region Root/Unroot
        public string Root(bool createWindow)
        {
            return _rootExploit.Root(createWindow);
        }
        public string Unroot(bool createWindow)
        {
            return _rootExploit.Unroot(createWindow);
        }

        #endregion

        private readonly CommandExecutor _executor;
        private readonly ADBBackup _backup;
        private ADBRestore _restore;
        private RootExploit _rootExploit;

        #region Classes
        private sealed class ADBBackup
        {
            public string All(string path, bool createWindow)
            {
                path = string.Format(@"""{0}""", path);
                return new CommandExecutor().ExecuteSingleCommandReturn(new Command(string.Format("adb backup -apk -shared -all -f {0}", path)), createWindow);
            }
            public string NoSystemApps(string path, bool createWindow)
            {
                path = string.Format(@"""{0}""", path);
                return new CommandExecutor().ExecuteSingleCommandReturn(new Command(string.Format("adb backup -apk -noshared - nosystem -f  {0}", path)), createWindow);
            }
            public string SystemApps(string path, bool createWindow)
            {
                path = string.Format(@"""{0}""", path);
                return new CommandExecutor().ExecuteSingleCommandReturn(new Command(string.Format("adb backup -apk -all -system -f {0}", path)), createWindow);
            }
            public string SDBackup(string path, bool createWindow)
            {
                path = string.Format(@"""{0}""", path);
                return new CommandExecutor().ExecuteSingleCommandReturn(new Command(string.Format("adb pull sdcard {0}", path)), createWindow);
            }
        }
        private sealed class ADBRestore
        {
            public ADBRestore(string backupName, bool createWindow)
            {
                _backupName = backupName;
                _createWindow = createWindow;
                Restore();
            }

            public string Restore()
            {
                return new CommandExecutor().ExecuteSingleCommandReturn(new Command(string.Format("adb restore {0}", _backupName)), _createWindow);
            }

            private string _backupName { get; set; }
            private bool _createWindow { get; set; }

        }
        private sealed class RootExploit
        {
            public RootExploit()
            {
                _executor = new CommandExecutor();

                _rootExploitCommands = new List<Command>
                                       {
                                           new Command("adb push Superuser.apk /system/app"),
                                           new Command("adb push su /system/xbin"),
                                           new Command("adb push su /system/bin"),
                                           new Command("adb reboot")
                                       };

                _unrootExploitCommands = new List<Command>
                                         {
                                             new Command("adb remount"),
                                             new Command("adb rm /system/app/Superuser.apk"),
                                             new Command("adb rm /system/xbin/su"),
                                             new Command("adb rm /system/bin/su"),
                                             new Command("adb reboot")
                                         };
            }
            private readonly List<Command> _rootExploitCommands;
            private readonly List<Command> _unrootExploitCommands;
            private readonly CommandExecutor _executor;
            public string Root(bool createWindow)
            {
                return _executor.ExecuteMultipleCommandsReturn(_rootExploitCommands, createWindow);
            }

            public string Unroot(bool createWindow)
            {
                return _executor.ExecuteMultipleCommandsReturn(_unrootExploitCommands, createWindow);
            }
        }
        #endregion
    }
}
