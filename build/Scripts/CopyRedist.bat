
:: How to call this bat file from the command prompt.
:: CopyRedist.bat Application BackItUp .exe x86 Win32 Debug
:: CopyRedist.bat Application BackItUp .exe x86 Win32 Release

@ECHO OFF

:: What should be done (Component/Application) manifest file
SET Action=%1
:: Name (NBBridge/NBEngine)
SET Name=%2
:: Extension (.ocx/.exe/.dll)
SET Ext=%3
:: x86/x64
SET Architecture=%4
:: Win32/Win64
SET Platform=%5
:: Release/Debug
SET Configuration=%6

:: Copy all the other dependent components to the directory where we want to place the application manifest file.

xcopy /Y /R /E .\AppRedist\* .\%Architecture%\%Configuration%\

