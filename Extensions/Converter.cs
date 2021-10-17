using System.IO;
using Sharer.Models;
using Sharer.Services;

namespace Sharer.Extensions
{
    public static class Converter
    {
        public static FolderInformation ToFolderInformation(DirectoryInfo model, string otherName = null)
        {
            return new FolderInformation()
            {
                Title = otherName ?? model.Name,
                Path = model.FullName,
                Size = string.Empty,
                IsReadOnly = false,
                Root = model.Root.FullName,
                CreationTime = model.CreationTime,
                LastAccessTime = model.LastAccessTime
            };
        }

        public static FileInformation ToFileInformation(FileInfo model, Types type, DocTypes docType, bool isPlayable, string sharedFolder, string uploads, string webRootPath)
        {
            return new FileInformation()
            {
                Title = model.Name.Replace(model.Extension, ""),
                Path = model.FullName,
                Type = type,
                DocType = docType,
                IsPlayable = isPlayable,
                Extension = model.Extension,
                Minutes = 0,
                Size = FileOperationService.ConvertFromBytes(model.Length),
                IsReadOnly = model.IsReadOnly,
                CreationTime = model.CreationTime,
                LastAccessTime = model.LastAccessTime
            };
        }
    }
}