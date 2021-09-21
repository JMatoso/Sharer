using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private DirectoryService _directoryService;

        public SharingController(ILogger<SharingController> logger, 
            SharedFolders shared, IWebHostEnvironment web)
        {
            _logger = logger;
            _shared = shared;
            _web = web;
            _directoryService = new DirectoryService(_shared, _web);
        }

        [Route("/")]
        [Route("/sharing/")]
        [Route("/sharing/home")]
        [Route("/sharing/index")]
        public IActionResult Index()
        {
            var dirs = _directoryService
                .GetFilesInDirectory(Path.Combine(_web.WebRootPath, _shared.SharedFolder));

            return View(dirs);
        }

        [HttpGet]
        [Route("/sharing/folder")]
        public IActionResult Folder([FromQuery]string folderPath) 
        {
            string formattedUrl = folderPath.Replace("-", @"\");

            if(_directoryService.Exists(formattedUrl, true))
            {
                var dirs = _directoryService
                    .GetFilesInDirectory(formattedUrl);

                var splitted = formattedUrl.Split(@"\");
                ViewBag.Title = splitted[splitted.Length - 1] + " folder";

                return View(dirs);
            }

            return RedirectToAction(nameof(NotFoundAction));
        }

        [Route("/sharing/notfound")]
        public IActionResult NotFoundAction()
        {
            return View();
        }

        [Route("/sharing/error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
