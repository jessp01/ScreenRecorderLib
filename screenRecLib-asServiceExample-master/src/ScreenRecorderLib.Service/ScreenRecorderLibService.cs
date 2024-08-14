using ScreenRecorderLib;
using System.IO;
using System;
using System.Timers;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;



namespace ScreenRecorderLib.Service
{
    public class ScreenRecorderLibService
    {

        private System.Timers.Timer _Timer;

        public ScreenRecorderLibService()
        {
            _Timer = null;
        }

        public void Start()
        {
            string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;



            //during init of your application bind to this event   
            SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);
            //SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
            SystemEvents.SessionSwitch += HandleSessionSwitch;
            
            void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e) 
            {     
                    string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    File.AppendAllText(Path.Combine("c:\\temp\\zj\\", "switch.log"), $"[{DateTime.Now:dd/mm/yyyy HH:mm:ss}] active user: {username}: Logoff handle; reason: {e.Reason}\n");
                    //ScreenRecorderLibHelper._Log($"[{DateTime.Now:dd/mm/yyyy HH:mm:ss}] active user: {username}: Logoff handle; reason: {e.Reason}\n");
                    //ScreenRecorderLibHelper._Log("Screen status: " + ScreenRecorderLibHelper.ScreenRecorderStatus());
                    //ScreenRecorderLibHelper._Log("Cam status: " + ScreenRecorderLibHelper.CamRecorderStatus());

                    ScreenRecorderLibHelper.StopRecording();
                    //System.Threading.Thread.Sleep(5000);

            }

            void RestartScreenRecording(){
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                int timestamp = (int)t.TotalSeconds;
                string videoPath = "c:\\temp\\zj\\" + timestamp +"_screen.mp4";
                string logPath = "c:\\temp\\zj\\" + timestamp +"_screen.log";

                ScreenRecorderLibHelper.StartScreenRecording(videoPath, logPath);
            }

            void RestartCamRecording(){
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                int timestamp = (int)t.TotalSeconds;
                string videoPath = "c:\\temp\\zj\\" + timestamp +"_cam.mp4";
                string logPath = "c:\\temp\\zj\\" + timestamp +"_cam.log";
                ScreenRecorderLibHelper.StartCamRecording(videoPath, logPath);
            }

            void HandleSessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
            {       //ScreenRecorderLibHelper.StopRecording();
                    File.AppendAllText(Path.Combine("c:\\temp\\zj\\", "switch.log"), $"[{DateTime.Now:dd/mm/yyyy HH:mm:ss}] active user: {username}: Switch handle; reason: {e.Reason}\n");
                    //ScreenRecorderLibHelper._Log($"[{DateTime.Now:dd/mm/yyyy HH:mm:ss}] active user: {username}: Switch handle; reason: {e.Reason}\n");
                    //ScreenRecorderLibHelper._Log("Screen status: " + ScreenRecorderLibHelper.ScreenRecorderStatus());
                    //ScreenRecorderLibHelper._Log("Cam status: " + ScreenRecorderLibHelper.CamRecorderStatus());
                    //if (e.Reason == SessionSwitchReason.ConsoleConnect || e.Reason == SessionSwitchReason.SessionLock || e.Reason == SessionSwitchReason.SessionUnlock){                   
                        

                    if (e.Reason == SessionSwitchReason.ConsoleDisconnect){
                        ScreenRecorderLibHelper.StopRecording();
                    }else{
                        if (ScreenRecorderLibHelper.ScreenRecorderStatus() != RecorderStatus.Recording){
                            RestartScreenRecording();
                        }

                        if (ScreenRecorderLibHelper.CamRecorderStatus() != RecorderStatus.Recording){
                            RestartCamRecording();
                        }
                    }
            }

            var exitEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) => {
                                  eventArgs.Cancel = true;
                                  exitEvent.Set();
                                  ScreenRecorderLibHelper.StopRecording();
                              };
            /*if (_Timer == null)
            {
                _Timer = new Timer(10000);
                _Timer.Elapsed += _OnTimedEvent;
                _Timer.Start();
            }*/
            //string rndFile = Path.GetRandomFileName();
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int timestamp = (int)t.TotalSeconds;
            string videoPath = "c:\\temp\\zj\\" + timestamp +"_screen.mp4";
            string logPath = "c:\\temp\\zj\\" + timestamp +"_screen.log";

            ScreenRecorderLibHelper.StartScreenRecording(videoPath, logPath);
            videoPath = "c:\\temp\\zj\\" + timestamp +"_cam.mp4";
            logPath = "c:\\temp\\zj\\" + timestamp +"_cam.log";
            ScreenRecorderLibHelper.StartCamRecording(videoPath, logPath);


            while (true)
            {
                ConsoleKeyInfo info = Console.ReadKey(true);
                if (info.Key == ConsoleKey.Escape)
                {
                    ScreenRecorderLibHelper.StopRecording();
                    break;
                }
            }
            exitEvent.WaitOne();
        }
        
        public void Stop()
        {
            if (_Timer != null)
            {
                _Timer.Stop();
                _Timer = null;
            }
        }

        private void _OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //string videoPath = $"{Path.GetTempPath()}{Path.GetRandomFileName()}.mp4";
            //string logPath = $"{Path.GetTempPath()}{Path.GetRandomFileName()}.txt";
            string rndFile = Path.GetRandomFileName();
            string videoPath = "c:\\temp\\zj\\" + rndFile +".mp4";
            string logPath = "c:\\temp\\zj\\" + rndFile +".log";

            ScreenRecorderLibHelper.StartScreenRecording(videoPath, logPath);

            System.Threading.Thread.Sleep(50000);

            ScreenRecorderLibHelper.StopRecording();
        }
    }
}
