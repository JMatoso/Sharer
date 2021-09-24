using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Sharer.Options;

namespace Sharer.Services
{
    public class UploadService 
    {
        private readonly IWebHostEnvironment _web;
        private readonly SharedFolders _shared;

        public UploadService(IWebHostEnvironment web, SharedFolders shared)
        {
            _web = web;
            _shared = shared;
        }

        public async Task<string> Upload(IFormFile formFile)
        {
            string uniqueFileName = null;
            string filePath = null;

            if(formFile != null)
            {
                string path = Path.Combine(_web.WebRootPath, _shared.Uploads);
                var file = new FileInfo(formFile.FileName);

                string genNewName = Guid.NewGuid().ToString().Replace("-", "");

                uniqueFileName = $"{genNewName}{file.Extension}";
                filePath = Path.Combine(path, uniqueFileName);

                using(var fs = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(fs);
                }
            }

            return uniqueFileName;
        }
    }
}