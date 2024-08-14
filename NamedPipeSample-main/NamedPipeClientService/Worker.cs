using ScreenRecorderLib;

using System.IO.Pipes;
using ScreenRecorderLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using Microsoft.Win32;
using System.Collections.Generic;

namespace NamedPipeClientService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private static bool _isRecording;
    private static Stopwatch _stopWatch;
    private static Recorder rec;
    private static Recorder _ScreenRecorder;
    private static Recorder _CamRecorder;
    static string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

            public static void StartScreenRecording(string videoPath, string logPath)
        {
            if (_ScreenRecorder == null)
            {
                try
                {
                    Console.WriteLine();
                    _Log("Configuring Record");
                    _Log($"Video Path: {videoPath}");
                    _Log($"Log Path: {logPath}");
                    var audioInputDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.InputDevices);
                    var audioOutputDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.OutputDevices);
                    foreach (var input in audioInputDevices){
                        _Log("dev:" + input.DeviceName);
                    }
                    string selectedAudioInputDevice = audioInputDevices.Count > 0 ? audioInputDevices.First().DeviceName : null;
                    string selectedAudioOutputDevice = audioOutputDevices.Count > 0 ? audioOutputDevices.First().DeviceName : null;
                    _Log($"selectedAudioInputDevice: {selectedAudioInputDevice}");
                    _Log($"selectedAudioOnputDevice: {selectedAudioOutputDevice}");
                    var sources = new List<RecordingSourceBase>();
                    //To get a list of recordable cameras and other video inputs on the system, you can use the static Recorder.GetSystemVideoCaptureDevices() function.
                    //var allRecordableCameras = Recorder.GetSystemVideoCaptureDevices();
	                //var cameraSource = allRecordableCameras.FirstOrDefault();
                    var monitor1 = new DisplayRecordingSource(DisplayRecordingSource.MainMonitor);
	                //sources.Add(cameraSource);
                    sources.Add(monitor1);
                    RecorderOptions options = new RecorderOptions
                    {
                        SourceOptions = new SourceOptions {
                            RecordingSources = sources, 
                        },
                        OutputOptions = new OutputOptions
                        {
                            RecorderMode = RecorderMode.Video,
                            Stretch = StretchMode.Uniform
                        },
                        
                        AudioOptions = new AudioOptions
                        {
                            Bitrate = AudioBitrate.bitrate_128kbps,
                            Channels = AudioChannels.Stereo,
                            IsAudioEnabled = true,
                            IsOutputDeviceEnabled = true,
                            IsInputDeviceEnabled = true,
                            AudioOutputDevice = selectedAudioOutputDevice,
                            AudioInputDevice = selectedAudioInputDevice
                        },
                        VideoEncoderOptions = new VideoEncoderOptions
                        {
                            Encoder = new H264VideoEncoder
                            {
                                BitrateMode = H264BitrateControlMode.Quality,
                                EncoderProfile = H264Profile.Main
                            }
                        },
                        LogOptions = new LogOptions
                        {
                            IsLogEnabled = true,
                            LogSeverityLevel = LogLevel.Trace,
                            LogFilePath = logPath
                        }
                    };

                    _Log("Creating Recorder");

                    _ScreenRecorder = Recorder.CreateRecorder(options);
                    _ScreenRecorder.OnStatusChanged += Rec_OnStatusChanged;
                    _ScreenRecorder.OnRecordingFailed += Rec_OnRecordingFailed;
                    _ScreenRecorder.OnRecordingComplete += Rec_OnRecordingComplete;

                    _Log($"Starting Record");

                    _ScreenRecorder.Record(videoPath);

                    while (_ScreenRecorder.Status != RecorderStatus.Recording)
                    {
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception exception)
                {
                    _Log($"Failed to Start Recording: {exception.Message}");
                    _Log(exception.ToString());
                }
            }
        }
        public static void StartCamRecording(string videoPath, string logPath)
        {
            if (_CamRecorder == null)
            {
                try
                {
                    Console.WriteLine();
                    _Log("Configuring Record");
                    _Log($"Video Path: {videoPath}");
                    _Log($"Log Path: {logPath}");
                    var audioInputDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.InputDevices);
                    var audioOutputDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.OutputDevices);
                    foreach (var input in audioInputDevices){
                        _Log("dev:" + input.DeviceName);
                    }
                    string selectedAudioInputDevice = audioInputDevices.Count > 0 ? audioInputDevices.First().DeviceName : null;
                    string selectedAudioOutputDevice = audioOutputDevices.Count > 0 ? audioOutputDevices.First().DeviceName : null;
                    _Log($"selectedAudioInputDevice: {selectedAudioInputDevice}");
                    _Log($"selectedAudioOnputDevice: {selectedAudioOutputDevice}");
                    var sources = new List<RecordingSourceBase>();
                    //To get a list of recordable cameras and other video inputs on the system, you can use the static Recorder.GetSystemVideoCaptureDevices() function.
                    var allRecordableCameras = Recorder.GetSystemVideoCaptureDevices();
	                var cameraSource = allRecordableCameras.FirstOrDefault();
                    //var monitor1 = new DisplayRecordingSource(DisplayRecordingSource.MainMonitor);
	                sources.Add(cameraSource);
                    //sources.Add(monitor1);
                    RecorderOptions options = new RecorderOptions
                    {
                        SourceOptions = new SourceOptions {
                            RecordingSources = sources, 
                        },
                        OutputOptions = new OutputOptions
                        {
                            RecorderMode = RecorderMode.Video,
                            Stretch = StretchMode.Uniform
                        },
                        
                        AudioOptions = new AudioOptions
                        {
                            Bitrate = AudioBitrate.bitrate_128kbps,
                            Channels = AudioChannels.Stereo,
                            IsAudioEnabled = true,
                            IsOutputDeviceEnabled = true,
                            IsInputDeviceEnabled = true,
                            AudioOutputDevice = selectedAudioOutputDevice,
                            AudioInputDevice = selectedAudioInputDevice
                        },
                        VideoEncoderOptions = new VideoEncoderOptions
                        {
                            Encoder = new H264VideoEncoder
                            {
                                BitrateMode = H264BitrateControlMode.Quality,
                                EncoderProfile = H264Profile.Main
                            }
                        },
                        LogOptions = new LogOptions
                        {
                            IsLogEnabled = true,
                            LogSeverityLevel = LogLevel.Trace,
                            LogFilePath = logPath
                        }
                    };

                    _Log("Creating Recorder");

                    _CamRecorder = Recorder.CreateRecorder(options);
                    _CamRecorder.OnStatusChanged += Rec_OnStatusChanged;
                    _CamRecorder.OnRecordingFailed += Rec_OnRecordingFailed;
                    _CamRecorder.OnRecordingComplete += Rec_OnRecordingComplete;

                    _Log($"Starting Cam Record");

                    _CamRecorder.Record(videoPath);

                    while (_CamRecorder.Status != RecorderStatus.Recording)
                    {
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception exception)
                {
                    _Log($"Failed to Start Recording: {exception.Message}");
                    _Log(exception.ToString());
                }
            }
        }
        public static void StopRecording()
        {
            if (_ScreenRecorder != null)
            {
                _Log("Finishing Record");

                _ScreenRecorder.Stop();

                while (_ScreenRecorder.Status != RecorderStatus.Idle)
                {
                    Thread.Sleep(1000);
                }

                _ScreenRecorder = null;
            }
            if (_CamRecorder != null)
            {
                _Log("Finishing Record");

                _CamRecorder.Stop();

                while (_CamRecorder is Recorder && _CamRecorder.Status != RecorderStatus.Idle)
                {
                    Thread.Sleep(1000);
                }

                _CamRecorder = null;
            }
        }
        public static void _Log(string message)
        {
            File.AppendAllText(Path.Combine("c:\\temp\\zj\\", "rec.log"), $"[{DateTime.Now:dd/mm/yyyy HH:mm:ss}] {message}\n");
            Console.WriteLine($"[{DateTime.Now:dd/mm/yyyy HH:mm:ss}] {message}");
            EventLog.WriteEntry("ScreenRecorderLibService", message);
        }
       private static void Rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            switch (e.Status)
            {
                case RecorderStatus.Idle:
                    //Console.WriteLine("Recorder is idle");
                    break;
                case RecorderStatus.Recording:
                    _stopWatch = new Stopwatch();
                    _stopWatch.Start();
                    _isRecording = true;
                    Console.WriteLine("Recording started");
                    Console.WriteLine("Press ESC to stop recording");
                    break;
                case RecorderStatus.Paused:
                    Console.WriteLine("Recording paused");
                    break;
                case RecorderStatus.Finishing:
                    Console.WriteLine("Finishing encoding");
                    break;
                default:
                    break;
            }
        }

        private static void Rec_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
        {
            Console.WriteLine("Recording completed");
            _isRecording = false;
            _stopWatch?.Stop();
            Console.WriteLine(String.Format("File: {0}", e.FilePath));
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
        }

        private static void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            Console.WriteLine("Recording failed with: " + e.Error);
            _isRecording = false;
            _stopWatch?.Stop();
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
        }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await using var pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.In);

            // Connect to the pipe or wait until the pipe is available.
            _logger.LogInformation("Attempting to connect to pipe...");
            pipeClient.Connect();

            _logger.LogInformation("Connected to pipe.");

            using var sr = new StreamReader(pipeClient);
            string? temp;
            while ((temp = sr.ReadLine()) != null)
            {
                _logger.LogInformation("Received from server: {0}", temp);
                if (temp == "record"){
                    TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                    int timestamp = (int)t.TotalSeconds;
                    string videoPath = "c:\\temp\\zj\\" + timestamp +"_screen.mp4";
                    string logPath = "c:\\temp\\zj\\" + timestamp +"_screen.log";

                    StartScreenRecording(videoPath, logPath);
                    videoPath = "c:\\temp\\zj\\" + timestamp +"_cam.mp4";
                    logPath = "c:\\temp\\zj\\" + timestamp +"_cam.log";
                    StartCamRecording(videoPath, logPath);
                }
            }
            
            await Task.Delay(10000, stoppingToken);
        }
    }
}