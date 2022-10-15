using BeatmapRemover.LazerDB;
using BeatmapRemover.LazerDB.Schema;
using Realms;

LazerDatabase? database = LazerDatabase.Locate(@"C:\Game\osu! lazer");

Realm? realm = database?.Open();
int mapsetCount = realm!.All<BeatmapSet>().Count();
Console.WriteLine($"Found {mapsetCount} mapsets in the database.");

HashSet<string> inCollectionMaps = new();
foreach (var collection in realm.All<BeatmapCollection>())
{
    inCollectionMaps.UnionWith(collection.BeatmapMD5Hashes);
}

int progress = 0;
foreach (var mapset in realm.All<BeatmapSet>())
{
    if (mapset.Beatmaps.All(map => !inCollectionMaps.Contains(map.MD5Hash)))
    {
        realm.Write(() => mapset.DeletePending = true);
    }
    progress++;
    Console.WriteLine($"\r{progress}/{mapsetCount} {mapset.Beatmaps.First().Metadata.Title}");
}
