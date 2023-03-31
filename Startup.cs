using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Insights.Startup))]
namespace Insights
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
