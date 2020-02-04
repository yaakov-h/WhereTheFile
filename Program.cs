using Standart.Hash.xxHash;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WhereTheFile
{
    class Program
    {
        //http://www.techmikael.com/2010/02/directory-search-with-multiple-filters.html
        // Works in .Net 4.0 - takes same patterns as old method, and executes in parallel
        public static IEnumerable<string> GetFiles(string path, string[] searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return searchPatterns.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(path, searchPattern, searchOption));
        }

        public static IEnumerable<string> GetFiles(string[] paths, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            paths = paths.Where(p => TryEnumerateFiles(p) == true).ToArray();
            //paths = paths.Where(p=> new DirectoryInfo(p).Attributes.HasFlag())
            return paths.AsParallel().SelectMany(path => Directory.EnumerateFiles(path, searchPattern, searchOption));
        }

        //public static IEnumerable<string> GetPaths(string startPath, string searchPattern, EnumerationOptions enumerationOptions)
        //{
        //    return path.SelectMany(path => Directory.EnumerateDirectories(path, searchPattern, enumerationOptions));
        //}

        public static bool TryEnumerateFiles(string path)
        {
            var di = new DirectoryInfo(path);
            try
            {
                di.GetFiles();
                return true;
            }

            catch (UnauthorizedAccessException e)
            {
                return false;
            }
        }
        static ulong ComputeHash(string path)
        {
            return xxHash64.ComputeHash(new FileStream(path, FileMode.Open, FileAccess.Read));
        }


        static void Main(string[] args)
        {
            var onedrive = Environment.GetEnvironmentVariable("ONEDRIVE");
            var paths = Directory.GetDirectories("C:\\","*",new EnumerationOptions() { RecurseSubdirectories = true });
            if (!string.IsNullOrEmpty(onedrive))
            {
                paths = paths.Where(p => !p.Contains(onedrive)).ToArray();
            }
            var files = GetFiles(paths.ToArray(), "*");
            var allFiles = files.ToArray();

            var hashes = allFiles.AsParallel().Select(file => ComputeHash(file)).ToArray();
        }


    }
}
    
