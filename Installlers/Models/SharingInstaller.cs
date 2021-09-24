using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sharer.Installers;
using Sharer.Options;

namespace Sharer.Installlers.Models
{
    public class SharingInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var sharedFolders = new SharedFolders();
            configuration.Bind(nameof(SharedFolders), sharedFolders);
            
            services.AddSingleton(sharedFolders);

            var systemFolders = new SystemFolders();
            configuration.Bind(nameof(SystemFolders), systemFolders);
            
            services.AddSingleton(systemFolders);
        }
    }
}