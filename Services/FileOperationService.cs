using System;
using System.IO;
using ByteSizeLib;
using Newtonsoft.Json;

namespace Sharer.Services
{
    public static class FileOperationService
    {
        public static void QualquerCoisaPorEnquanto()
        {
            // Image image = System.Image.FromFile(fileName);
            // Image thumb = image.GetThumbnailImage(120, 120, ()=>false, IntPtr.Zero);
            // thumb.Save(Path.ChangeExtension(fileName, "thumb"));
        }

        public static string ConvertFromBytes(long fileBytes) 
        {
           return ByteSize.FromBytes(fileBytes)
            .ToString();
        }

        public static string ReadFile(string path)
        {
            if(System.IO.File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            return null;
        }

        public static void SaveFile(string content, string path)
        {
            File.WriteAllText(path, content);
        }
    }
}