﻿// Original source file (modified by kabii) Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
using Realms;
using System.Text;

namespace BeatmapRemover.LazerDB.Schema
{
    public class BeatmapSet : RealmObject
    {
        [PrimaryKey]
        public Guid ID { get; set; } = Guid.NewGuid();
        [Indexed]
        public int OnlineID { get; set; } = -1;
        public DateTimeOffset DateAdded { get; set; }
        public DateTimeOffset? DateSubmitted { get; set; }
        public DateTimeOffset? DateRanked { get; set; }
        public IList<Beatmap> Beatmaps { get; } = null!;
        public IList<RealmNamedFileUsage> Files { get; } = null!;
        public int Status { get; set; }
        public bool DeletePending { get; set; }
        public string Hash { get; set; } = string.Empty;
        public bool Protected { get; set; }

        // Author kabii
        IList<Beatmap>? selected = null;

        [Ignored]
        public IList<Beatmap> SelectedBeatmaps
        {
            get
            {
                return selected switch
                {
                    not null => selected,
                    null => Beatmaps
                };
            }
            set { selected = value; }
        }

        public string Display()
        {
            BeatmapMetadata metadata = Beatmaps.First().Metadata;
            var difficulties = SelectedBeatmaps.Select(b => b.StarRating).OrderBy(r => r).Select(r => r.ToString("0.00"));
            string difficultySpread = string.Join(", ", difficulties);

            var output = new StringBuilder();
            output
                .Append(OnlineID)
                .Append(": ")
                .Append(metadata.Artist)
                .Append(" - ")
                .Append(metadata.Title)
                .Append(" (")
                .Append(metadata.Author.Username)
                .Append(" - ")
                .Append(difficultySpread)
                .Append(" stars)");
            return output.ToString();
        }
    }
}
