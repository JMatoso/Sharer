using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sharer.Models;
using Sharer.Options;
using Sharer.Services;

namespace Sharer.Controllers
{
    public class SharingController : Controller
    {
        private readonly ILogger<SharingController> _logger;
        private readonly SharedFolders _shared;
        private readonly IWebHostEnvironment _web;

        public SharingController(ILogger<SharingController> logger, 
            SharedFolders shared, IWebHostEnvironment web)
        {
            _logger = logger;
            _shared = shared;
            _web = web;
        }

        [Route("/")]
        [Route("/sharing/")]
        [Route("/sharing/home")]
        [Route("/sharing/index")]
        public IActionResult Index()
        {
            var dirs = GetDirectories(_shared.SharedFolder);
            return View(dirs);
        }

        [Route("/sharing/audio")]
        public IActionResult Audio()
        {
            var fileFormats = new FileFormats();
            var dirs = GetFilterDirectories(_shared.Audio, 
                f => !fileFormats
                    .Audio
                    .Exists(x => x.Equals(f.Extension)));

            return View(dirs);
        }

        [Route("/sharing/compressed")]
        public IActionResult Compressed()
        {
            var fileFormats = new FileFormats();
            var dirs = GetFilterDirectories(_shared.Compressed, 
                f => !fileFormats
                    .Compressed
                    .Exists(x => x.Equals(f.Extension)));

            return View(dirs);
        }

        [Route("/sharing/docs")]
        public IActionResult Documents()
        {
            var fileFormats = new FileFormats();
            var dirs = GetFilterDirectories(_shared.Documents, 
                f => !fileFormats
                    .Documents
                    .Exists(x => x.Equals(f.Extension)));

            return View(dirs);
        }

        [Route("/sharing/images")]
        public IActionResult Images()
        {
            var fileFormats = new FileFormats();
            var dirs = GetFilterDirectories(_shared.Images, 
                f => !fileFormats
                    .Images
                    .Exists(x => x.Equals(f.Extension)));

            return View(dirs);
        }

        [Route("/sharing/uploads")]
        public IActionResult Uploads()
        {
            var dirs = GetDirectories(_shared.SharedFolder);
            return View(dirs);
        }

        [Route("/sharing/videos")]
        public IActionResult Videos()
        {
            var fileFormats = new FileFormats();
            var dirs = GetFilterDirectories(_shared.Videos, 
                f => !fileFormats
                    .Videos
                    .Exists(x => x.Equals(f.Extension)));

            return View(dirs);
        }

        [HttpGet]
        [Route("/sharing/delete")]
        public IActionResult Delete(string path, string returnUrl)
        {
            if(!string.IsNullOrEmpty(path))
            {
                var file = new FileInfo(Path.Combine(_web.WebRootPath, path));
                if(file.Exists)
                {
                    file.Delete();
                    _logger.LogDebug($"File '{file}' has been deleted.");
                }
            }

            return Redirect(returnUrl);
        }

        private Directories GetDirectories(string path)
        {
            var filesInTheFolder = Directory.GetFiles(Path.Combine(_web.WebRootPath, path));
            var dirs = new Directories();

            foreach (var item in filesInTheFolder)
            {
                var file = new FileInfo(item);
                var type = new Types();
                var docType = new DocTypes();
                var fileFormats = new FileFormats();
                bool isPlayable = true;

                if(fileFormats.Audio.Exists(x => x.Equals(file.Extension)))
                {
                    type = Types.Audio;
                }
                else if(fileFormats.Compressed.Exists(x => x.Equals(file.Extension)))
                {
                    type = Types.Compressed;
                    isPlayable = false;
                }
                else if(fileFormats.Documents.Exists(x => x.Equals(file.Extension)))
                {
                    type = Types.Document;
                    docType = Enum.Parse<DocTypes>(file.Extension.Replace(".", ""));
                    isPlayable = false;

                    if(file.Extension.Equals(".txt") || file.Extension.Equals(".pdf"))
                    {
                        isPlayable = true;
                    }
                }
                else if(fileFormats.Images.Exists(x => x.Equals(file.Extension)))
                {
                    type = Types.Image;
                }
                else if(fileFormats.Videos.Exists(x => x.Equals(file.Extension)))
                {
                    type = Types.Video;
                }
                else if(fileFormats.Applications.Exists(x => x.Equals(file.Extension)))
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

        private Directories GetFilterDirectories(string path, Predicate<FileInformation> match)
        {
            var dirs = GetDirectories(path);

            dirs.Files
                .RemoveAll(match);

            return dirs;
        }

        [Route("/sharing/error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
