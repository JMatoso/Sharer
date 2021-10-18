using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sharer.Extensions;
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
        [Route("/home")]
        [Route("/index")]
        public IActionResult Index()
        {
            var dirs = _directoryService
                .GetFilesInDirectory(Path.Combine(_web.WebRootPath, _shared.SharedFolder));
            
            string pathsFolder = Path.Combine(_web.ContentRootPath, _sys.PathsData);
            if(_directoryService.Exists(pathsFolder))
            {
                var savedPaths = new List<SavedPath>();
                var content = FileOperationService.ReadFile(pathsFolder);

                if(content != null)
                {
                    savedPaths = JsonConvert.DeserializeObject<List<SavedPath>>(content);

                    foreach (var item in savedPaths)
                    {
                        var folder = new DirectoryInfo(item.FolderPath);

                        if(_directoryService.Exists(pathsFolder))
                            dirs.SharedFolders.Add(Converter.ToFolderInformation(folder, item.FolderName));
                    }
                }
            }

            return View(dirs);
        }

        [HttpGet]
        [Route("/folder")]
        public IActionResult Folder([FromQuery]string path) 
        {
            if(!string.IsNullOrEmpty(path))
            {
                string formattedUrl = (new UrlParser()).Base64Decode(path);

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
        [Route("/uploads")]
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
        [Route("/play")]
        public IActionResult Play([FromQuery]string path)
        {
            if(!string.IsNullOrEmpty(path))
            {
                string formattedUrl = (new UrlParser()).Base64Decode(path);

                if(_directoryService.Exists(formattedUrl))
                {
                    var file = _directoryService.GetFileInfo(formattedUrl);

                    file.Path = 
                        file.Path.Contains(_shared.SharedFolder.Replace('/', Path.DirectorySeparatorChar)) || 
                        file.Path.Contains(_shared.Uploads.Replace('/', Path.DirectorySeparatorChar)) ? 
                        file.Path.Replace(_web.WebRootPath, "") :
                        file.Path;
                        
                    return View(file);
                }
            }

            return RedirectToAction(nameof(NotFoundAction));
        }

        [Route("/notfound")]
        public IActionResult NotFoundAction() => View();

        [Route("/error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
