using ByteSizeLib;

namespace Sharer.Services
{
    public static class FileOperationService
    {
        public static string ConvertFromBytes(long fileBytes) 
        {
           return ByteSize.FromBytes(fileBytes)
            .ToString();
        }
    }
}