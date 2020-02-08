using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WhereTheFile.Types
{
    public class DriveInfo
    {
        public string CurrentDriveLetter { get; set; }
        public bool HasBeenScanned { get; set; }
        [Key]
        public string GeneratedGuid { get; set; }
    }
}
