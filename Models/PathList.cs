using System.Collections.Generic;

namespace Sharer.Models
{
    public class SavedPath
    {
        public string FolderName { get; set; }
        public string FolderPath { get; set; }
    }

    public class PathList
    { 
        public List<SavedPath> SavedPaths { get; set; } = new();
    }
}