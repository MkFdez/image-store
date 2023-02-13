using Microsoft.Owin;
using Owin;
using System;
using System.Web;

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
