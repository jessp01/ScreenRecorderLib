using System;
using Topshelf;
using System.IO;
using System.Timers;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;
using System.Linq;

namespace ScreenRecorderLib.Service
{
    public static class Program
    {
        static bool exitSystem = false;
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig) {
            Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown");

            //do your cleanup here
            Thread.Sleep(5000); //simulate some cleanup delay

            Console.WriteLine("Cleanup complete");
            File.AppendAllText(Path.Combine("c:\\temp\\zj\\", "cleanup.log"), $"[{DateTime.Now:dd/mm/yyyy HH:mm:ss}] running cleanup! {sig}");


            //allow main to run off
            exitSystem = true;

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }

        static void Main(string[] args)
        {
            // Some boilerplate to react to close window event, CTRL-C, kill, etc
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);
           /* _consoleCtrlHandler += s =>
            {
                //DoCustomShutdownStuff();
                File.AppendAllText(Path.Combine("c:\\temp\\zj\\", "cleanup.log"), $"[{DateTime.Now:dd/mm/yyyy HH:mm:ss}] running cleanup");
                return false;   
            };*/

            var ec = HostFactory.Run(hc =>
            {
                hc.Service<ScreenRecorderLibService>(hc =>
                {
                    hc.ConstructUsing(name => new ScreenRecorderLibService());
                    hc.WhenStarted(tc => tc.Start());
                    hc.WhenStopped(tc => tc.Stop());

                    hc.WhenPaused(tc => tc.Stop());
                    hc.WhenContinued(tc => tc.Start());

                    hc.WhenShutdown(tc => tc.Stop());
                });
                hc.RunAsLocalSystem();

                hc.SetDescription("Screen Recorder Library Service Example");
                hc.SetDisplayName("Screen Recorder Library Service");
                hc.SetServiceName("ScreenRecorderLibService");
                
                hc.StartAutomatically();

                hc.EnablePauseAndContinue();
                hc.EnableShutdown();
            });

            var exitCode = (int)Convert.ChangeType(ec, ec.GetTypeCode());
            Environment.ExitCode = exitCode;
            while (!exitSystem) {
                Thread.Sleep(500);
            }
        }
    }
}
