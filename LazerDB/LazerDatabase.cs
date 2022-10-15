﻿using BeatmapRemover.LazerDB.Schema;
using Realms;
using Realms.Exceptions;

namespace BeatmapRemover.LazerDB
{
    public class LazerDatabase
    {
        const int LazerSchemaVersion = 25;
        readonly string database;
        readonly string filesDirectory;

        private LazerDatabase(string database)
        {
            this.database = database;
            filesDirectory = Path.Combine(Path.GetDirectoryName(database)!, "files");
        }

        static string? GetDatabaseFile(string directory)
        {
            string path = Path.Combine(directory, "client.realm");
            return File.Exists(path) ? path : null;
        }

        public static LazerDatabase? Locate(string directory)
        {
            string? dbFile = GetDatabaseFile(directory);
            if (dbFile is null)
            {
                Console.Write("osu! song database not found. Please find and provide your osu! data folder.\nThe folder should contain a \"client.realm\" file and can be opened from in-game.\n\nFolder path: ");
                string? input = Console.ReadLine();
                if (input is not null)
                {
                    dbFile = GetDatabaseFile(input);
                }
            }
            return dbFile is not null ? new LazerDatabase(dbFile) : null;
        }

        public Realm? Open()
        {
            RealmConfiguration config = new(database)
            {
                // IsReadOnly = true,
                SchemaVersion = LazerSchemaVersion
            };
            config.Schema = new[] {
                typeof(Beatmap),
                typeof(BeatmapCollection),
                typeof(BeatmapDifficulty),
                typeof(BeatmapMetadata),
                typeof(BeatmapSet),
                typeof(RealmFile),
                typeof(RealmNamedFileUsage),
                typeof(BeatmapUserSettings),
                typeof(RealmUser),
                typeof(Ruleset),
                typeof(ModPreset)
            };

            try
            {
                return Realm.GetInstance(config);
            }
            catch (RealmException re)
            {
                Console.WriteLine($"\nError opening database: {re.Message}");
                if(re.Message.Contains("is less than last set version"))
                {
                    Console.WriteLine("The osu!lazer database structure has updated since the last BeatmapExporter update.");
                    Console.WriteLine("\nYou can check https://github.com/kabiiQ/BeatmapExporter/releases for a new release, or file an issue there to let me know it needs updating.");
                }
                return null;
            }
        }

        public string HashedFilePath(string hash) => Path.Combine(filesDirectory, hash[..1], hash[..2], hash);

        public FileStream? OpenHashedFile(string hash)
        {
            try
            {
                string path = HashedFilePath(hash);
                return File.Open(path, FileMode.Open);
            }
            catch (IOException ioe)
            {
                Console.WriteLine($"Unable to open file: {hash} :: {ioe.Message}");
                return null;
            }
        }
    }
}
