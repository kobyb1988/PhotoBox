using System.Diagnostics;

namespace ImageMaker.Runner
{
    class Program
    {
        private const string CAdmin = @"ImageMaker.AdminView.exe";

        static void Main(string[] args)
        {
            using (var server = Server.Instance)
            {
                server.Launch();
                var process = Process.Start(new ProcessStartInfo(CAdmin)
                {
                });

                process.WaitForExit();
                //process.Exited += (sender, eventArgs) =>
                //{
                //    Console.
                //};

                //if (!process.HasExited)
                //    process.Close();
            }
        }
    }
}
