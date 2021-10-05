using System.IO;

namespace Sharer.Helpers
{
    public static class Init
    {
        public static void CreateDefaultFolders(string[] paths)
        {
            foreach (var path in paths)
            {
                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }
    }
}