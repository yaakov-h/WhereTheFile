using Standart.Hash.xxHash;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WhereTheFile.Database;
using WhereTheFile.Types;
using DriveInfo = WhereTheFile.Types.DriveInfo;

namespace WhereTheFile
{
    class Program
    {

        private static string[] drives;
        private static List<DriveInfo> ScannedDrives;
        static void Main(string[] args)
        { 
            Menu();
        }

        static void Menu()
        {
            Console.WriteLine("i) Generate GUIDs for drives (requires Admin the first time around)");
            Console.WriteLine("s) Scan all drives");
            Console.WriteLine("ss) Scan specific drive only");
            Console.WriteLine("b) Backup scan database");
            Console.WriteLine("q) Exit");
            Console.WriteLine("Choice? ");
            var choice = Console.ReadLine().Trim();
            switch (choice)
            {
                case "i":
                    GetOrGenerateDriveGuids();
                    Menu();
                    break;
                case "s":
                    ScanAllDrives();
                    Menu();
                    break;
                case "ss":
                    DriveMenu();
                    Menu();
                    break;
                case "b":
                    BackupDatabase();
                    Menu();
                    break;
                case "q":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine();
                    Console.WriteLine("Invalid choice");
                    Menu();
                    break;
            }
        }

        private static void BackupDatabase()
        {
            Console.WriteLine();
            string databaseFile = "WTF_EF.db";
            if (!File.Exists(databaseFile))
            {
                Console.WriteLine("Database doesn't exist yet");
                Console.WriteLine();
                Menu();
            }

            File.Copy(databaseFile, $"{databaseFile}.bak");
            string fullPath = Path.GetFullPath($"{databaseFile}.bak");
            Console.WriteLine($"Backed up to {fullPath}");
            Menu();
        }

        static void DriveMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Scan a specific drive, or hit Enter to exit:");
            drives = System.IO.Directory.GetLogicalDrives();
            foreach (string drive in drives)
            {
                Console.WriteLine(drive);
            }

            Console.WriteLine("Choice? (enter letter only)");
            var choice = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(choice))
            {
                Menu();
            }

            var choiceDrive =
                drives.FirstOrDefault(d => d.StartsWith(choice, StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrEmpty(choiceDrive))
            {
                ScanFiles(choiceDrive);
            }

            else
            {
                Console.WriteLine($"Drive {choice} doesn't exist");
                DriveMenu();
            }

        }

        static void ScanAllDrives()
        {
            drives = System.IO.Directory.GetLogicalDrives();
            ScannedDrives = GetOrGenerateDriveGuids();

            foreach (string drive in drives)
            {
                Console.WriteLine($"Scanning {drive}");
                ScanFiles(drive);
            }
        }

        static void ScanFiles(string path)
        {
            DriveInfo drive = ScannedDrives.First(d => path.StartsWith(d.CurrentDriveLetter));
            WindowsInterop.RtlSetProcessPlaceholderCompatibilityMode(2);
            int baseBuffer = 8192;
            FileSystemEnumerable<ScannedFileInfo> fse =
                new FileSystemEnumerable<ScannedFileInfo>(path,
                    (ref FileSystemEntry entry) => new ScannedFileInfo() {FullPath = entry.ToFullPath(), Size = entry.Length, Drive = drive},
                    new EnumerationOptions() {RecurseSubdirectories = true});

            var context = new WTFContext();
            context.FilePaths.AddRange(fse);
            context.SaveChanges();
        }

        static List<DriveInfo> GetOrGenerateDriveGuids()
        {
            //This is far easier than trying to get the drive serial number, but I guess I'll have to do that eventually
            List<DriveInfo> scannedDrives = new List<DriveInfo>();
            foreach (string drive in drives)
            {
                string guid = String.Empty;
                string path = Path.Join(drive, ".wtf");
                if (File.Exists(path))
                {
                    guid = File.ReadAllText(path);
                }

                else
                {
                    guid = Guid.NewGuid().ToString();
                    File.WriteAllText(path,guid);
                }
                scannedDrives.Add(new DriveInfo() { CurrentDriveLetter = drive, GeneratedGuid = guid, HasBeenScanned = false});
            }

            return scannedDrives;
        }





    }
}


