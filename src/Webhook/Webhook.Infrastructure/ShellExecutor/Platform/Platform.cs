using System.Runtime.InteropServices;

namespace Webhook.Infrastructure.ShellExecutor.Platform
{
    public static class Platform
    {
        public static bool IsWin()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public static bool IsMac()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        public static bool IsGnu()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        public static OperatingSystem GetCurrent()
        {
            if (IsWin()) return OperatingSystem.Windows;
            if (IsMac()) return OperatingSystem.Mac;
            if (IsGnu()) return OperatingSystem.Linux;

            return OperatingSystem.Unknown;
        }
    }
}