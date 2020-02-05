using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace WhereTheFile
{
    public class SimpleFileInfo
    {
        public int Id { get; set; }
        public FileAttributes Attributes { get; set; }
        public string FullName { get; set; }

        public long Size { get; set; }
        public SimpleFileInfo(FileSystemInfo info)
        {
            Attributes = info.Attributes;
            FullName = info.FullName;
            if (!Attributes.HasFlag(FileAttributes.Directory))
            {
                Size = ((FileInfo)info).Length;
            }
        }

        protected SimpleFileInfo() { }

    }
}

