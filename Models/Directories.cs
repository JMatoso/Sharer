using System;
using System.Collections.Generic;

namespace Sharer.Models
{
    public class Directories
    {
        public List<FolderInformation> SharedFolders { get; set; } = new();
        public List<FolderInformation> Folders { get; set; } = new();
        public List<FileInformation> Files { get; set; } = new();
    }

    public class FolderInformation
    {
        public string Title { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }
        public string Root { get; set; }
        public bool IsReadOnly { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastAccessTime { get; set; }
    }

    public class FileInformation
    {
        public string Title { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public int? Minutes { get; set; }
        public string Size { get; set; }
        public Types Type { get; set; }
        public DocTypes? DocType { get; set; }
        public bool IsPlayable { get; set; }
        public bool IsReadOnly { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastAccessTime { get; set; }
    }
}