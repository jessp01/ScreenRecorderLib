using ScreenRecorderLib;
using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Threading;

namespace ScreenRecorderLib.Service
{
    public static class ScreenRecorderLibHelper
    {
        private static Recorder _ScreenRecorder;
        private static Recorder _CamRecorder;


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

                    _Log($"Starting Screen Record");

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

        public static RecorderStatus ScreenRecorderStatus()
        {
            return _ScreenRecorder.Status;
        }

        public static RecorderStatus CamRecorderStatus()
        {
            return _CamRecorder.Status;
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

        private static void Rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            _Log($"Recorder OnStatusChanged Event: {e.Status}");
        }

        private static void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            _Log($"Recorder OnRecordingFailed Event: {e.Error}");
            StopRecording();
            Thread.Sleep(3000);
            string rndFile = Path.GetRandomFileName();
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int timestamp = (int)t.TotalSeconds;
            string videoPath = "c:\\temp\\zj\\" + timestamp +"_screen.mp4";
            string logPath = "c:\\temp\\zj\\" + timestamp +"_screen.log";
            //StartScreenRecording(videoPath, logPath);
            videoPath = "c:\\temp\\zj\\" + timestamp +"_cam.mp4";
            logPath = "c:\\temp\\zj\\" + timestamp +"_cam.log";
            //StartCamRecording(videoPath, logPath);

        }

        private static void Rec_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
        {
            _Log("Recorder OnRecordingComplete Event");
        }

        public static void _Log(string message)
        {
            File.AppendAllText(Path.Combine("c:\\temp\\zj\\", "rec.log"), $"[{DateTime.Now:dd/mm/yyyy HH:mm:ss}] {message}\n");
            Console.WriteLine($"[{DateTime.Now:dd/mm/yyyy HH:mm:ss}] {message}");
            EventLog.WriteEntry("ScreenRecorderLibService", message);
        }
    }
}
