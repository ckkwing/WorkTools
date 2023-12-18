using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using Utility.WinNative;

namespace WorkTools.Theme.Dialog
{
    public class FolderBrowseDialog
    {
        public static string SelectFolder(string title, string currentPath)
        {
            string result = String.Empty;
            var windowHandle = new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle;
            NativeMethods.IFileOpenDialog fileOpenDialog = (NativeMethods.IFileOpenDialog)Activator.CreateInstance(Type.GetTypeFromCLSID(NativeMethods.Guids.FileOpenDialog));
            if (fileOpenDialog != null)
            {
                NativeMethods.IShellItem siCurrentPath = null;

                while (!string.IsNullOrEmpty(currentPath) && !Directory.Exists(currentPath) && (currentPath.Length > 3))
                {
                    // check if parent directory exists
                    currentPath = Path.GetDirectoryName(currentPath);
                }

                // we need an existing path - fall back to video directory
                if (string.IsNullOrEmpty(currentPath) || !Directory.Exists(currentPath))
                {
                    currentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                    try
                    {
                        Directory.CreateDirectory(currentPath);
                    }
                    catch (System.Exception)
                    {
                        currentPath = Environment.GetFolderPath(System.Environment.SpecialFolder.MyVideos, System.Environment.SpecialFolderOption.Create);
                    }
                }

                try
                {
                    NativeMethods.SHCreateItemFromParsingName(currentPath, IntPtr.Zero, NativeMethods.Guids.ShellItem, out siCurrentPath);
                    if (siCurrentPath != null)
                    {
                        fileOpenDialog.SetFolder(siCurrentPath);
                        fileOpenDialog.SetOptions(NativeMethods.FOS.FOS_PICKFOLDERS | NativeMethods.FOS.FOS_PATHMUSTEXIST);
                        fileOpenDialog.SetTitle(title);
                        if (NativeMethods.HRESULT.S_OK == (NativeMethods.HRESULT)fileOpenDialog.Show(windowHandle))
                        {
                            NativeMethods.IShellItem siResult = null;
                            fileOpenDialog.GetResult(out siResult);
                            if (siResult != null)
                            {
                                IntPtr ptr = IntPtr.Zero;
                                siResult.GetDisplayName(NativeMethods.SIGDN.FILESYSPATH, out ptr);
                                if (ptr != IntPtr.Zero)
                                {
                                    result = Marshal.PtrToStringUni(ptr);
                                    Marshal.FreeCoTaskMem(ptr);
                                }
                            }
                        }
                        Marshal.ReleaseComObject(siCurrentPath);
                    }
                }
                catch (Exception)
                { }
            }
            return result;
        }
    }
}
