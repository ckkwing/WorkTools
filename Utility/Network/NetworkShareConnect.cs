using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utility.WinNative.MprAPI;

namespace Utility.Network
{
    public class NetworkShareConnect
    {
        /// <summary>
        /// Connect to remote share
        /// </summary>
        /// <param name="remoteUNC">UNC path of remote shared</param>
        /// <param name="username">User name</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public static ErrorCodes ConnectToRemote(IntPtr hwndOwner, string localName, string remoteUNC, string username, string password)
        {
            ErrorCodes errorCodes = ErrorCodes.NO_ERROR;
            try
            {
                UseConnection(hwndOwner, localName, remoteUNC, username,password);
            }
            catch (Win32Exception e)
            {
                errorCodes = (ErrorCodes)e.NativeErrorCode;
            }
            return errorCodes;
        }

        /// <summary>
        /// Connect to remote share without username and password
        /// </summary>
        /// <param name="remoteUNC">UNC path of remote shared</param>
        /// <returns></returns>
        public static ErrorCodes ConnectToRemote(IntPtr hwndOwner, string localName, string remoteUNC, bool promptUser = false)
        {
            ErrorCodes errorCodes = ErrorCodes.NO_ERROR;
            try
            {
                UseConnection(hwndOwner, localName, remoteUNC, promptUser);
            }
            catch (Win32Exception e)
            {
                errorCodes = (ErrorCodes)e.NativeErrorCode;
            }
            return errorCodes;
        }

        /// <summary>
        /// Disconnet from remote share
        /// </summary>
        /// <param name="remoteUNC"></param>
        /// <returns></returns>
        public static ErrorCodes DisconnectRemote(string remoteUNC)
        {
            int ret = WNetCancelConnection2(remoteUNC, (int)Connect.CONNECT_UPDATE_PROFILE, false);
            return (ErrorCodes)ret;
        }

        private static void UseConnection(IntPtr hwndOwner, string localName, string remoteName, string username, string password)
        {
            NETRESOURCE nr = new NETRESOURCE()
            {
                dwType = ResourceType.RESOURCETYPE_DISK,
                lpLocalName = localName,
                lpRemoteName = remoteName,
            };

            IntPtr ownerPtr = null == hwndOwner ? IntPtr.Zero : hwndOwner;
            ThrowIfError(WNetUseConnection(ownerPtr, nr, password, username, 0, null, null, null));
        }

        private static void UseConnection(IntPtr hwndOwner, string localName, string remoteName, bool promptUser = false)
        {
            NETRESOURCE nr = new NETRESOURCE()
            {
                dwType = ResourceType.RESOURCETYPE_DISK,
                lpLocalName = localName,
                lpRemoteName = remoteName,
            };

            IntPtr ownerPtr = null == hwndOwner ? IntPtr.Zero : hwndOwner;
            ThrowIfError(WNetUseConnection(ownerPtr, nr, null, null, promptUser ? (int)(Connect.CONNECT_INTERACTIVE | Connect.CONNECT_PROMPT) : 0, null, null, null));
        }


        [DebuggerStepThrough]
        private static void ThrowIfError(int win32ErrorCode)
        {
            if (win32ErrorCode != 0)
            {
                throw new Win32Exception(win32ErrorCode);
            }
        }

    }
}
