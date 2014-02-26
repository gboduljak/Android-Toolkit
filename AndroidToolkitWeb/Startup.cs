using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AndroidToolkitWeb.Startup))]
namespace AndroidToolkitWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
