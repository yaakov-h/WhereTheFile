# WhereTheFile
Hopefully a multi-machine file indexer for home users


# What
* This will be a tool that scans local filesystems, computes a fast hash of each file and dumps the info to a database

# Why
* I lost a backup file on a drive somewhere and want to find it again

# How
* For now, a .NET Core app that enumerates all files on (each?) drive, runs xxHash against them and then dumps it to a SQLite database

# Challenges

* OneDrive - from a C# console app, how do you figure out what is and isn't available on the local drive *without* triggering an automatic download? I'm just lucky I got "Cloud Provider not running" rather than a stack of downloads happening.
