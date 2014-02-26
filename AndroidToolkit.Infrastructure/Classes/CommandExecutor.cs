using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidToolkit.Infrastructure.Classes
{
    internal class CommandExecutor
    {
        public CommandExecutor()
        {

        }
        public void ExecuteSingleCommandNoReturn(Command command, bool createWindow)
        {
            ProcessStartInfo start = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                FileName = "cmd.exe"
            };
            if (createWindow == false)
            {
                start.CreateNoWindow = true;
            }
            else
            {
                start.CreateNoWindow = false;
            }
            Process process = Process.Start(start);
            process.Start();
            process.StandardInput.WriteLine(command.Text);
            string output = process.StandardOutput.ReadToEnd();
            process.Close();

        }
        public string ExecuteSingleCommandReturn(Command command, bool createWindow)
        {
            var processStartInfo = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                

            };
            if (createWindow == false)
            {
                processStartInfo.CreateNoWindow = true;
            }
            else
            {
                processStartInfo.CreateNoWindow = false;
            }
            string outputString = string.Empty;
            Process process = Process.Start(processStartInfo);
            process.StandardInput.WriteLine(command.Text);
            process.StandardInput.Close();
            outputString = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return outputString;
        }
        public void ExecuteMultipleCommandsNoReturn(List<Command> commands, bool createWindow)
        {
            var processStartInfo = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false

            };
            if (createWindow == false)
            {
                processStartInfo.CreateNoWindow = true;
            }
            else
            {
                processStartInfo.CreateNoWindow = false;
            }
            string outputString = "";
            Process process = Process.Start(processStartInfo);
            foreach (var command in commands)
            {
                process.StandardInput.WriteLine(command.Text);
            }
            process.StandardInput.Close();
            outputString = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
        public string ExecuteMultipleCommandsReturn(List<Command> commands, bool createWindow)
        {
            var processStartInfo = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false

            };
            if (createWindow == false)
            {
                processStartInfo.CreateNoWindow = true;
            }
            else
            {
                processStartInfo.CreateNoWindow = false;
            }
            string outputString = "";
            Process process = Process.Start(processStartInfo);
            foreach (var command in commands)
            {
                process.StandardInput.WriteLine(command.Text);
            }
            process.StandardInput.Close();
            outputString = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return outputString;
        }

        public virtual void ExecuteJavaApp(string appName, bool createWindow)
        {
            var process = new Process();
            process.StartInfo.FileName = "java.exe";
            process.StartInfo.Arguments = "-jar " + appName;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (createWindow == true)
            {
                process.StartInfo.CreateNoWindow = false;
            }
            else
            {
                process.StartInfo.CreateNoWindow = true;
            }
            process.Start();
            process.WaitForExit();
        }
    }
}
