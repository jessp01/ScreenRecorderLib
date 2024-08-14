SETLOCAL ENABLEDELAYEDEXPANSION
set MP4_PATH="C:\Temp\zj"
set FFMPEG_BIN="c:\ffmpeg-7.0.2-full_build\bin\ffmpeg"
cd %MP4_PATH%
del screen.mp4 cam.mp4
(for %%i in (*screen.mp4) do @echo file '%%i') > screen_list.txt
%FFMPEG_BIN% -safe 0 -f concat -i screen_list.txt -c copy screen.mp4
(for %%i in (*cam.mp4) do @echo file '%%i') > cam_list.txt
%FFMPEG_BIN% -safe 0 -f concat -i cam_list.txt -c copy cam.mp4
cd %CD%