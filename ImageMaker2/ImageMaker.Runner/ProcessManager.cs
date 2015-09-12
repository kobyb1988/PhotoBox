using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ImageMaker.Utils.Commands;

namespace ImageMaker.Runner
{
    public class ProcessManager
    {
        private const string CMain = @"ImageMaker.View.exe";
        private const string CAdmin = @"ImageMaker.AdminView.exe";

        private readonly List<Process> _processes = new List<Process>();

        public ProcessManager()
        {
        }

        public void Launch()
        {
            try
            {
                var process = Process.Start(new ProcessStartInfo(CMain)
                                            {
                                                RedirectStandardOutput = true,
                                                UseShellExecute = false,
                                                RedirectStandardError = true
                                            });

                if (process != null)
                {
                    process.EnableRaisingEvents = true;
                    process.OutputDataReceived += ProcOnOutputDataReceived;
                    process.ErrorDataReceived += ProcOnErrorDataReceived;
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();    

                    _processes.Add(process);
                }
            }
            catch (Exception)
            {
            }
        }

        private void SpawnProcess(AppCommand command)
        {
            Process process = _processes.FirstOrDefault(x => x.ProcessName == command.ProcessName);
        }

        private void ProcOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
        }

        private void ProcOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("error: {0}", e.Data);
        }
    }
}
