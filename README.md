# WhereTheFile
Hopefully a multi-machine file indexer for home users

# What
* This will be a tool that scans local filesystems, computes a fast hash of each file and dumps the info to a database

# Why
* I lost a backup file on a drive somewhere and want to find it again

# How
* For now, a .NET Core app that enumerates all files on (each?) drive, and then dumps them to a SQLite database. From there, files with the same filesize can be grouped and hashed if needed

# Challenges

* OneDrive - touching a file in the wrong way will cause a download when you probably don't want it

* Drive identification - getting the drive serial on Windows is a real pain, https://github.com/unknownv2/CloudKit/blob/master/CloudKit.SteamKit/Util/Win32Helpers.cs one day.