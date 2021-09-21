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

        public async Task<string> Upload(IFormFile image)
        {
            string uniqueFileName = null;
            string filePath = null;

            if(image != null)
            {
                string path = Path.Combine(_web.WebRootPath, _shared.Uploads);
                string genNewName = Guid.NewGuid().ToString().Replace("-", "");

                int dotPosition = image.FileName.IndexOf('.');
                int finalLength = image.FileName.Length - dotPosition;
                
                string fileExt = image.FileName.Substring(dotPosition, finalLength);

                uniqueFileName = $"{genNewName}{fileExt}";
                filePath = Path.Combine(path, uniqueFileName);

                using(var fs = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fs);
                }
            }

            return uniqueFileName;
        }
    }
}