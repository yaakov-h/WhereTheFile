using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace WhereTheFile.Types
{
    public class FailedFile
    {
        [Key]
        public int Id { get; set; }
        public SimpleFileInfo FailedFileInfo { get; set; }
        public string FailureReason { get; set; }

        public bool Retry { get; set; } = false;
    }
}
