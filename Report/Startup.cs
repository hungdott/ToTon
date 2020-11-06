using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Report.Startup))]
namespace Report
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
