using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IEProject1.Startup))]
namespace IEProject1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
