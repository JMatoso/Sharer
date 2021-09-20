using System.Collections.Generic;

namespace Sharer.Models
{
    public class FileFormats
    {
        public List<string> Audio { get; } = new()
        {
            ".mp3", ".wav", ".m4a"
        };
        public List<string> Compressed { get; } = new()
        {
            ".zip", ".rar", ".tar", ".tgz", ".bz2", ".tar.xz", 
            ".7z", ".iso", ".xz", ".gz"
        };
        public List<string> Images { get; } = new()
        {
            ".jpg", ".png", ".bmp", ".jpeg", ".gif"
        };
        public List<string> Videos { get; } = new()
        {
            ".mp4", ".mkv", ".webp"
        };
        public List<string> Documents { get; } = new()
        {
            ".docx", ".pdf", ".odt", ".txt"
        };
        public List<string> Applications { get; } = new()
        {
            ".exe", ".msi", ".deb"
        };
    }

    public enum Types
    {
        Audio = 1,
        Compressed = 2,
        Document = 3,
        Image = 4,
        Video = 5,
        Application = 6,
        Other = 7
    }

    public enum DocTypes
    {
        txt = 1,
        docx = 2,
        pdf = 3
    }
}