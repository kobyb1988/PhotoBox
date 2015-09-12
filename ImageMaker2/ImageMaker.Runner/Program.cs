using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.View;

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

                Console.ReadLine();
                process.Close();
            }
        }
    }
}
