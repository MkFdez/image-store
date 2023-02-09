using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QandAProject.Startup))]
namespace QandAProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
