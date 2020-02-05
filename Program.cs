using Standart.Hash.xxHash;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WhereTheFile.Database;
using WhereTheFile.Types;

namespace WhereTheFile
{
    class Program
    {
      

        public static List<FailedFile> FailedFiles { get; private set; } = new List<FailedFile>();

        //http://www.techmikael.com/2010/02/directory-search-with-multiple-filters.html
        // Works in .Net 4.0 - takes same patterns as old method, and executes in parallel

        public static IEnumerable<FileSystemInfo> GetFileSystemInfos(string[] paths, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var context = new WTFContext();
            paths = paths.Where(p => TryEnumerateFiles(p, context) == true).ToArray();
            //paths = paths.Where(p=> new DirectoryInfo(p).Attributes.HasFlag())
            return paths.AsParallel().SelectMany(path => new DirectoryInfo(path).EnumerateFileSystemInfos(searchPattern));
        }

        //public static IEnumerable<string> GetPaths(string startPath, string searchPattern, EnumerationOptions enumerationOptions)
        //{
        //    return path.SelectMany(path => Directory.EnumerateDirectories(path, searchPattern, enumerationOptions));
        //}

        public static bool TryEnumerateFiles(string path, WTFContext context)
        {
            var di = new DirectoryInfo(path);
            try
            {
                var temp = di.GetFiles();
                return true;
            }

            catch (UnauthorizedAccessException e)
            {
                var currentFailure = new FailedFile() { FailedFileInfo = new SimpleFileInfo(di), FailureReason = e.Message };
                context.Add<FailedFile>(currentFailure);
                
                return false;
            }
        }
        static (IEnumerable<HashedFile>, IEnumerable<FailedFile>) ComputeHashes(IEnumerable<FileSystemInfo> fsi, int bufferSize = 8192)
        {
            List<HashedFile> hashedFiles = new List<HashedFile>();
            List<FailedFile> failedFiles = new List<FailedFile>();
            var start = DateTime.Now;
            foreach (var file in fsi)
            {
                if (file.Attributes.HasFlag(FileAttributes.Directory))
                {
                    continue;
                }

                if (file.Attributes.HasFlag(FileAttributes.Offline))
                {
                    //Console.WriteLine($"Skipping offline file: {fsi.FullName}");
                    failedFiles.Add(new FailedFile() { FailedFileInfo = new SimpleFileInfo(file), FailureReason = "Probably an offline file" });
                    continue;
                }
                                
                try
                {
                    var hash = xxHash64.ComputeHash(new FileStream(file.FullName, FileMode.Open, FileAccess.Read),bufferSize);
                    //_context.Add<HashedFile>(new HashedFile() { Hash = hash, FileInfo = fsi });
                    hashedFiles.Add(new HashedFile() { Hash = hash, FileInfo = new SimpleFileInfo(file) });
                    continue;

                }
                catch (Exception e)
                {
                    failedFiles.Add(new FailedFile() { FailedFileInfo = new SimpleFileInfo(file), FailureReason = e.Message });
                    continue;
                }
            }
            var end = DateTime.Now;

            var elapsed = end - start;
            var megabytes = hashedFiles.Sum(s => s.FileInfo.Size) / 1024 / 1024;
            var megabytesPerSecond = megabytes / elapsed.TotalSeconds;
            //Console.WriteLine($"Hashed {megabytes} megabytes in {elapsed} seconds, {megabytes} megabytes per second");
            return (hashedFiles, failedFiles);
        }



        static void Main(string[] args)
        {
            WindowsInterop.RtlSetProcessPlaceholderCompatibilityMode(2);
            int baseBuffer = 8192;
            
            HashAllFiles(1000, baseBuffer);
            //HashAllFiles(10000, baseBuffer * 10);

        }

        static void HashAllFiles(int partitionSize, int bufferSize)
        {

        WTFContext _context = new WTFContext();

        var paths = Directory.GetDirectories("C:\\", "*", new EnumerationOptions() { RecurseSubdirectories = true });

            var files = GetFileSystemInfos(paths.ToArray(), "*");

            int numberFailed = _context.SaveChanges();
            //Console.WriteLine($"Saved {numberFailed} failed paths");
            var allFiles = files.ToArray();
            //Console.WriteLine($"Scanned {allFiles.Length} files");

            var start = DateTime.Now;
            OrderablePartitioner<Tuple<int, int>> chunks = Partitioner.Create(0, allFiles.Length, partitionSize);

            int total = allFiles.Length;
            ConcurrentBag<HashedFile> hashedFilesBag = new ConcurrentBag<HashedFile>();
            ConcurrentBag<FailedFile> failedFilesBag = new ConcurrentBag<FailedFile>();

            Parallel.ForEach(chunks, chunk =>
            {
                int start = chunk.Item1;
                int end = chunk.Item2;

                (IEnumerable<HashedFile> hashedFiles, IEnumerable<FailedFile> failedFiles) = ComputeHashes(allFiles.Skip(start).Take(end - start));

                foreach (var hashedFile in hashedFiles)
                {
                    hashedFilesBag.Add(hashedFile);
                }

                foreach (var failedFile in failedFiles)
                {
                    failedFilesBag.Add(failedFile);
                }



            });

            var end = DateTime.Now;

            //lock (_lock)
            //{
            _context.FailedFiles.AddRange(failedFilesBag.ToArray());
            _context.Files.AddRange(hashedFilesBag.ToArray());
            _context.SaveChanges();
            _context = null;
                                                                                       
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"Using partition size {partitionSize} and buffer size {bufferSize}, hashing took ${end - start}");
            //ParallelEnumerable. (files,new ParallelOptions() {MaxDegreeOfParallelism = 4 }, (a, b) => { })
            //var hashes = allFiles.Select(file => ComputeHash(file)).ToArray();
        }


    }
}

