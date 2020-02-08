using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace WhereTheFile
{
    static class PlatformQuirks
    {
        public static void OnAppInit()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsInterop.RtlSetProcessPlaceholderCompatibilityMode(WindowsInterop.PHCM_EXPOSE_PLACEHOLDERS);
            }
        }

        public static bool ShouldSkipDrive(string drive)
            => ShouldSkipDrive(drive, File.GetAttributes(drive));

        public static bool ShouldSkipDrive(ReadOnlySpan<char> drive, FileAttributes attributes)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // There's no point indexing devices.
                if (drive.Equals("/dev", StringComparison.Ordinal) || drive.StartsWith("/dev/"))
                {
                    return true;
                }

                // There's no point indexing process information.
                if (drive.Equals("/proc", StringComparison.Ordinal) || drive.StartsWith("/proc/"))
                {
                    return true;
                }

                // There's no point indexing kernel system information.
                // It's also a read-only filesystem, so we can't store temporary data.
                if (drive.Equals("/sys", StringComparison.Ordinal) || drive.StartsWith("/sys/"))
                {
                    return true;
                }

                // If individual files are mounted, such as through a container runtime, then these
                // also show up as logical drive roots. We can't really do anything with these, so skip them.
                // There are also highly unlikely to be user files that they want to find later.
                if (!attributes.HasFlag(FileAttributes.Directory))
                {
                    return true;
                }
            }
            
            return false;
        }

        public static StringComparison FileSystemStringComparison { get; } = GetFileSystemStringComparison();

        static StringComparison GetFileSystemStringComparison()
        {
            // In theory this is per-filesystem but let's just use some sensible defaults for now.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return StringComparison.OrdinalIgnoreCase;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return StringComparison.Ordinal;
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

    }
}