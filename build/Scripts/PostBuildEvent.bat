@echo off
echo "----------------------------------------------------------------------------"
set CONFIG=%1
set SOLUTIONDIR=%2

pushd %SOLUTIONDIR%

echo "CALL Scripts\CopyRedist.bat Application WorkTools .exe x86 Win32 %CONFIG%"
CALL Scripts\CopyRedist.bat Application WorkTools .exe x86 Win32 %CONFIG%

@pause
