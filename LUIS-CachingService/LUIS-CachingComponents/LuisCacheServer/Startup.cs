using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(KsparkAPI.Startup))]

namespace KsparkAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            ConfigureMobileApp(app);
        }
    }
}