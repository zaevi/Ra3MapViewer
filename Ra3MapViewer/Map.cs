using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace Ra3MapViewer
{
    [DebuggerDisplay("[{MaxPlayers}] {DisplayName}")]
    public class Map
    {
        public string Id { get; set; }

        public string FileId { get; set; }

        public string DisplayName { get; set; }

        public int MaxPlayers { get; set; }

        public string FolderPath { get; set; }

        public List<Point> PlayerPoints { get; set; } = new List<Point>();

        public Size Size { get; set; }

        public int BorderSize { get; set; }
    }
}
