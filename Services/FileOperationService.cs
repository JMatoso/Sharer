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
            => ByteSize.FromBytes(fileBytes).ToString();

        public static string ReadFile(string path)
            => File.Exists(path) ? File.ReadAllText(path) : null;

        public static void SaveFile(object content, string path)
        {
            var data = JsonConvert.SerializeObject(content, Formatting.Indented);
            File.WriteAllText(path, data);
        }
    }
}