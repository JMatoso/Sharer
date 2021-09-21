using ByteSizeLib;

namespace Sharer.Services
{
    public static class FileOperationService
    {
        public static void QualquerCoisaPorEnquanto()
        {
            // Image image = Image.FromFile(fileName);
            // Image thumb = image.GetThumbnailImage(120, 120, ()=>false, IntPtr.Zero);
            // thumb.Save(Path.ChangeExtension(fileName, "thumb"));
        }
        public static string ConvertFromBytes(long fileBytes) 
        {
           return ByteSize.FromBytes(fileBytes)
            .ToString();
        }
    }
}