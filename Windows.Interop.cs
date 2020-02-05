using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WhereTheFile
{
    public static class WindowsInterop
    {
        //From https://github.com/dotnet/runtime/issues/27976#issuecomment-445515585
        [DllImport("ntdll.dll")]
        static extern sbyte RtlQueryProcessPlaceholderCompatibilityMode();
        [DllImport("ntdll.dll")]
        public static extern sbyte RtlSetProcessPlaceholderCompatibilityMode(sbyte pcm);

        const sbyte PHCM_APPLICATION_DEFAULT = 0;
        const sbyte PHCM_DISGUISE_PLACEHOLDER = 1;
        const sbyte PHCM_EXPOSE_PLACEHOLDERS = 2;
        const sbyte PHCM_MAX = 2;
        const sbyte PHCM_ERROR_INVALID_PARAMETER = -1;
        const sbyte PHCM_ERROR_NO_TEB = -2;



        public class PlaceholderCompatibilityMode
        {
            public static string ToString(sbyte pcm)
            {
                switch (pcm)
                {
                    case 0: return "PHCM_APPLICATION_DEFAULT";
                    case 1: return "PHCM_DISGUISE_PLACEHOLDER";
                    case 2: return "PHCM_EXPOSE_PLACEHOLDERS";
                    case -1: return "PHCM_ERROR_INVALID_PARAMETER";
                    case -2: return "PHCM_ERROR_NO_TEB";
                    default: return String.Format("(??? unsupported PlaceholderCompatibilityMode value: {0} ???)", pcm);
                }
            }
        }

    }
}
