using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sharer.Models;
using Sharer.Options;

namespace Sharer.Services
{
    public interface IAppHub
    {
        Task BadRequest(string message);
        Task Successful(string message, object additinalData);
    }

    public class AppHub : Hub<IAppHub>
    {
        private readonly ILogger<AppHub> _logger;
        private readonly SharedFolders _shared;
        private readonly SystemFolders _sys;
        private readonly IWebHostEnvironment _web;
        private DirectoryService _directoryService;

        public AppHub(ILogger<AppHub> logger,
            SharedFolders shared, IWebHostEnvironment web, SystemFolders sys)
        {
            _logger = logger;
            _shared = shared;
            _web = web;
            _directoryService = new DirectoryService(_shared, _web);
            _sys = sys;
        }

        public void SaveFolderAsync(string path, string name)
        {
            if(!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(name))
            {                
                _logger.LogInformation($"SaveFolder {name} - {path}");

                if(_directoryService.Exists(path, true))
                {    
                    string pathsFolder = Path.Combine(_web.ContentRootPath, _sys.PathsData);

                    var pathsSaved = new List<SavedPath>();
                    var content = FileOperationService
                        .ReadFile(pathsFolder);

                    if(content != null)
                        pathsSaved = JsonConvert.DeserializeObject<List<SavedPath>>(content);

                    pathsSaved.Add(new()
                    {
                        FolderName = name,
                        FolderPath = path
                    });

                    FileOperationService.SaveFile(pathsSaved, pathsFolder);

                    Clients.All.Successful("Folder has been added to Shared Folders.", new SavedPath
                    {
                        FolderName = name,
                        FolderPath = path
                    });
                }
                else
                {
                    Clients.All.BadRequest("Folder not found.");
                }
            }
            else
            {
                Clients.All.BadRequest("Fill all required fields.");
            }
        }

        public async Task UploadAsync(IFormFile data) 
        {
            if(data != null)
            {
                _logger.LogInformation($"Upload");

                var upload = new UploadService(_web, _shared);
                await Clients.All.Successful("File has been uploaded.", new SavedPath
                {
                    FolderName = await upload.Upload(data),
                    FolderPath = Path.Combine(_web.WebRootPath, _shared.Uploads.Replace("/", "\\"))
                });
            }

            await Clients.All.BadRequest("Fill all required fields.");
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SharerHub");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SharerHub");
            await base.OnDisconnectedAsync(exception);
        }
    }
}