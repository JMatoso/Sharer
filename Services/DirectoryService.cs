using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Sharer.Models;
using Sharer.Options;

namespace Sharer.Services
{
    public class DirectoryService
    {
        private readonly SharedFolders _shared;
        private readonly IWebHostEnvironment _web;
        public DirectoryService(SharedFolders shared, IWebHostEnvironment web)
        {
            _shared = shared;
            _web = web;
        }

        public bool Exists(string path, bool fileOrFolder = false)
        {
            if(fileOrFolder)
            {
                if(Directory.Exists(path))
                {
                    return true;
                }
            }  
            else
            {
                if(File.Exists(path))
                {
                    return true;
                }
            }

            return false;
        }

        public Directories GetDirectories(string path)
        {
            var foldersInDirectory = Directory.GetDirectories(Path.Combine(_web.WebRootPath, path));
            var dirs = new Directories();

            foreach (var item in foldersInDirectory)
            {
                var folder = new DirectoryInfo(item);

                dirs.Folders.Add(new FolderInformation()
                {
                    Title = folder.Name,
                    Path = folder.FullName,
                    Size = string.Empty,
                    IsReadOnly = false,
                    Root = folder.Root.FullName,
                    CreationTime = folder.CreationTime,
                    LastAccessTime = folder.LastAccessTime
                });
            }

            return dirs;
        }

        public Directories GetFilesInDirectory(string path)
        {
            var filesInTheFolder = Directory.GetFiles(path);
            var dirs = new Directories();

            dirs.Folders = GetDirectories(path).Folders;

            foreach (var item in filesInTheFolder)
            {
                var file = new FileInfo(item);
                var type = new Types();
                var docType = new DocTypes();
                var fileFormats = new FileFormats();
                bool isPlayable = true;

                if(fileFormats.Audio.Exists(x => x.Equals(file.Extension.ToLower())))
                {
                    type = Types.Audio;
                }
                else if(fileFormats.Compressed.Exists(x => x.Equals(file.Extension.ToLower())))
                {
                    type = Types.Compressed;
                    isPlayable = false;
                }
                else if(fileFormats.Documents.Exists(x => x.Equals(file.Extension.ToLower())))
                {
                    type = Types.Document;
                    docType = Enum.Parse<DocTypes>(file.Extension.Replace(".", ""));
                    isPlayable = false;

                    if(file.Extension.ToLower().Equals(".txt") || file.Extension.ToLower().Equals(".pdf"))
                    {
                        isPlayable = true;
                    }
                }
                else if(fileFormats.Images.Exists(x => x.Equals(file.Extension.ToLower())))
                {
                    type = Types.Image;
                }
                else if(fileFormats.Videos.Exists(x => x.Equals(file.Extension.ToLower())))
                {
                    type = Types.Video;
                }
                else if(fileFormats.Applications.Exists(x => x.Equals(file.Extension.ToLower())))
                {
                    type = Types.Application;
                    isPlayable = false;
                }
                else
                {
                    type = Types.Other;
                    isPlayable = false;
                }

                dirs.Files.Add(new()
                {
                    Title = file.Name.Replace(file.Extension, ""),
                    Path = item.Replace(_web.WebRootPath, ""),
                    Type = type,
                    DocType = docType,
                    IsPlayable = isPlayable,
                    Extension = file.Extension,
                    Minutes = 0,
                    Size = FileOperationService.ConvertFromBytes(file.Length),
                    IsReadOnly = file.IsReadOnly,
                    CreationTime = file.CreationTime,
                    LastAccessTime = file.LastAccessTime
                });
            }

            return dirs;
        }

        public Directories GetFilterDirectories(string path, Predicate<FileInformation> match)
        {
            var dirs = GetDirectories(path);

            dirs.Files
                .RemoveAll(match);

            return dirs;
        }
    }
}