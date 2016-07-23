using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ZTBusService.Startup))]
namespace ZTBusService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
