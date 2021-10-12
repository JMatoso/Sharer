using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sharer.Helpers;
using Sharer.Models;
using Sharer.Options;
using Sharer.Services;

namespace Sharer.Controllers
{
    public class SharingController : Controller
    {
        private readonly ILogger<SharingController> _logger;
        private readonly SharedFolders _shared;
        private readonly SystemFolders _sys;
        private readonly IWebHostEnvironment _web;
        private DirectoryService _directoryService;

        public SharingController(ILogger<SharingController> logger,
            SharedFolders shared, IWebHostEnvironment web, SystemFolders sys)
        {
            _logger = logger;
            _shared = shared;
            _web = web;
            _directoryService = new DirectoryService(_shared, _web);
            _sys = sys;
        }

        [Route("/")]
        [Route("/sharing/")]
        [Route("/sharing/home")]
        [Route("/sharing/index")]
        public IActionResult Index()
        {
            var dirs = _directoryService
                .GetFilesInDirectory(Path.Combine(_web.WebRootPath, _shared.SharedFolder));
            
            string pathsFolder = Path.Combine(_web.ContentRootPath, _sys.PathsData);
            if(_directoryService.Exists(pathsFolder))
            {
                var pathsSaved = new List<SavedPath>();
                var content = FileOperationService
                    .ReadFile(pathsFolder);

                if(content != null)
                {
                    pathsSaved = JsonConvert.DeserializeObject<List<SavedPath>>(content);

                    foreach (var item in pathsSaved)
                    {
                        var folder = new DirectoryInfo(item.FolderPath);

                        if(_directoryService.Exists(pathsFolder))
                        {
                            dirs.SharedFolders.Add(new FolderInformation()
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
                    }
                }
            }

            return View(dirs);
        }

        [HttpGet]
        [Route("/sharing/folder")]
        public IActionResult Folder([FromQuery]string folderPath) 
        {
            if(!string.IsNullOrEmpty(folderPath))
            {
                string formattedUrl = (new UrlParser()).Base64Decode(folderPath);

                if(_directoryService.Exists(formattedUrl, true))
                {
                    var dirs = _directoryService
                        .GetFilesInDirectory(formattedUrl);

                    var splitted = formattedUrl.Split(formattedUrl.Contains("\\") ? "\\" : "/");
                    ViewBag.Title = $"\"{splitted[splitted.Length - 1]}\" folder";

                    return View(dirs);
                }
            }

            return RedirectToAction(nameof(NotFoundAction));
        }

        [HttpPost]
        [Route("/sharing/uploads")]
        public async Task<IActionResult> Uploads(IFormFile fileInput) 
        {
            var upload = new UploadService(_web, _shared);

            if(fileInput != null)
            {
                ViewBag.FileName = await upload.Upload(fileInput);
                ViewBag.FileLocation = Path.Combine(_web.WebRootPath, _shared.Uploads.Replace("/", "\\"));
            }

            return View();
        }

        [HttpGet]
        [Route("/sharing/play")]
        public async Task<IActionResult> Play([FromQuery]string file)
        {
            if(!string.IsNullOrEmpty(file))
            {
                string formattedUrl = (new UrlParser()).Base64Decode(file);

                if(_directoryService.Exists(formattedUrl))
                {
                    var accessedFile = new FileInfo(formattedUrl);
                    var tmpFileName = 
                        Path.Combine(_web.WebRootPath, _shared.TempFolder, System.Guid.NewGuid() + accessedFile.Extension);

                    using(var fs = new FileStream(tmpFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        using(var ms = new MemoryStream())
                        {
                            await fs.CopyToAsync(ms);
                            var newFile = _directoryService.GetFileInformation(tmpFileName);
                            
                            newFile.Path = newFile.Path.Replace(_web.WebRootPath, string.Empty);
                            return View(newFile);
                        }
                    }
                }
            }

            return RedirectToAction(nameof(NotFoundAction));
        }

        [Route("/sharing/notfound")]
        public IActionResult NotFoundAction() => View();

        [Route("/sharing/error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
