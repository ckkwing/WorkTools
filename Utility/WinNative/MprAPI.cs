using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utility.WinNative
{
    public class MprAPI
    {
        public enum ErrorCodes
        {
            NO_ERROR = 0,
            ERROR_ACCESS_DENIED = 5,
            ERROR_BAD_NET_NAME = 67,
            ERROR_ALREADY_ASSIGNED = 85,
            ERROR_INVALID_PASSWORD = 86,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_MORE_DATA = 234,
            ERROR_NO_MORE_ITEMS = 259,
            ERROR_INVALID_ADDRESS = 487,
            ERROR_BAD_DEVICE = 1200,
            ERROR_NO_NET_OR_BAD_PATH = 1203,
            ERROR_BAD_PROVIDER = 1204,
            ERROR_CANNOT_OPEN_PROFILE = 1205,
            ERROR_BAD_PROFILE = 1206,
            ERROR_EXTENDED_ERROR = 1208,
            ERROR_SESSION_CREDENTIAL_CONFLICT = 1219,
            ERROR_NO_NETWORK = 1222,
            ERROR_CANCELLED = 1223,
            ERROR_NOT_CONNECTED = 2250,
            ERROR_OPEN_FILES = 2401,
            ERROR_DEVICE_IN_USE = 2404,
        };

        public enum ResourceScope
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET,
            RESOURCE_REMEMBERED,
            RESOURCE_RECENT,
            RESOURCE_CONTEXT
        };

        public enum ResourceType
        {
            RESOURCETYPE_ANY,
            RESOURCETYPE_DISK,
            RESOURCETYPE_PRINT,
            RESOURCETYPE_RESERVED
        };

        public enum ResourceDisplayType
        {
            RESOURCEDISPLAYTYPE_GENERIC,
            RESOURCEDISPLAYTYPE_DOMAIN,
            RESOURCEDISPLAYTYPE_SERVER,
            RESOURCEDISPLAYTYPE_SHARE,
            RESOURCEDISPLAYTYPE_FILE,
            RESOURCEDISPLAYTYPE_GROUP,
            RESOURCEDISPLAYTYPE_NETWORK,
            RESOURCEDISPLAYTYPE_ROOT,
            RESOURCEDISPLAYTYPE_SHAREADMIN,
            RESOURCEDISPLAYTYPE_DIRECTORY,
            RESOURCEDISPLAYTYPE_TREE,
            RESOURCEDISPLAYTYPE_NDSCONTAINER
        };

        [Flags]
        public enum Connect
        {
            CONNECT_UPDATE_PROFILE = 0x00000001,
            CONNECT_INTERACTIVE = 0x00000008,
            CONNECT_PROMPT = 0x00000010,
            CONNECT_REDIRECT = 0x00000080,
            CONNECT_LOCALDRIVE = 0x00000100,
            CONNECT_COMMANDLINE = 0x00000800,
            CONNECT_CMD_SAVECRED = 0x00001000,
        }

        [StructLayout(LayoutKind.Sequential)]
        public class NETRESOURCE
        {
            public ResourceScope dwScope = 0;
            public ResourceType dwType = 0;
            public ResourceDisplayType dwDisplayType = 0;
            public ResourceUsage dwUsage = 0;
            public string lpLocalName = null;
            public string lpRemoteName = null;
            public string lpComment = null;
            public string lpProvider = null;
        };

        [Flags]
        public enum ResourceUsage
        {
            RESOURCEUSAGE_CONNECTABLE = 0x00000001,
            RESOURCEUSAGE_CONTAINER = 0x00000002,
            RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
            RESOURCEUSAGE_SIBLING = 0x00000008,
            RESOURCEUSAGE_ATTACHED = 0x00000010,
            RESOURCEUSAGE_ALL = RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED,
        };

        [DllImport("Mpr.dll", EntryPoint = "WNetOpenEnumA", CallingConvention = CallingConvention.Winapi)]
        public static extern ErrorCodes WNetOpenEnum(ResourceScope dwScope, ResourceType dwType, ResourceUsage dwUsage, NETRESOURCE p, out IntPtr lphEnum);

        [DllImport("Mpr.dll", EntryPoint = "WNetCloseEnum", CallingConvention = CallingConvention.Winapi)]
        public static extern ErrorCodes WNetCloseEnum(IntPtr hEnum);

        [DllImport("Mpr.dll", EntryPoint = "WNetEnumResourceA", CallingConvention = CallingConvention.Winapi)]
        public static extern ErrorCodes WNetEnumResource(IntPtr hEnum, ref int lpcCount, IntPtr buffer, ref uint lpBufferSize);

        [DllImport("Mpr.dll")]
        public static extern int WNetUseConnection(
            IntPtr hwndOwner,
            NETRESOURCE lpNetResource,
            string lpPassword,
            string lpUserID,
            int dwFlags,
            string lpAccessName,
            string lpBufferSize,
            string lpResult
            );

        [DllImport("Mpr.dll")]
        public static extern int WNetCancelConnection2(
            string lpName,
            int dwFlags,
            bool fForce
            );

    }
}
