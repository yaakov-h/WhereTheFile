using System.ComponentModel.DataAnnotations;
using System.IO;

namespace WhereTheFile.Types
{
    public class HashedFile
    {
        [Key]
        public int Id { get; set; }
        public ulong Hash { get; set; }


        public SimpleFileInfo FileInfo { get; set; }
        
        //until I sort out a way past the "Entity type requires a primary key", we can't insert a FileSystemInfo
        //public FileSystemInfo FileInfo { get; set; }
    }
}