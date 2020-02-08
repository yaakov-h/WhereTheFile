using System.Runtime.InteropServices;

namespace WhereTheFile
{
    public static class WindowsInterop
    {
        //From https://github.com/dotnet/runtime/issues/27976#issuecomment-445515585

        [DllImport("ntdll.dll")]
        public static extern sbyte RtlSetProcessPlaceholderCompatibilityMode(sbyte pcm);

        public const sbyte PHCM_APPLICATION_DEFAULT = 0;
        public const sbyte PHCM_DISGUISE_PLACEHOLDER = 1;
        public const sbyte PHCM_EXPOSE_PLACEHOLDERS = 2;
        public const sbyte PHCM_MAX = 2;
        public const sbyte PHCM_ERROR_INVALID_PARAMETER = -1;
        public const sbyte PHCM_ERROR_NO_TEB = -2;

    }
}