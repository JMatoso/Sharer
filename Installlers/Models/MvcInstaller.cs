using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sharer.Installers;

namespace Sharer.Installlers.Models
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews(configure => {
                configure.RespectBrowserAcceptHeader = true;
                configure.ReturnHttpNotAcceptable = true;
                configure.CacheProfiles.Add("120SecondsDuration", new CacheProfile { Duration = 120 });
            }).AddRazorRuntimeCompilation();

            services.AddHttpContextAccessor();
            services.AddMvc(options => 
            { 
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(value => "Required field.");
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }
    }
}