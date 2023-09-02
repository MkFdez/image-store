using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Store.Controllers;
using Dependencies;
using ApiTest;
using System.Web.Http;
using Services;
using Logger;

namespace Store
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();

            var builder = new ContainerBuilder();

            // Register your MVC controllers. (MvcApplication is the name of
            // the class in Global.asax.)
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();


            builder.RegisterType<EmailSender>().As<IEmailSender>().As<EmailSender>();
            builder.RegisterType<NewUserApp>().As<IApp>().As<NewUserApp>();
            builder.RegisterType<BuyApp>().As<IApp>().As<BuyApp>();
            builder.RegisterType<ServicePack>().As<IServicePack>();
            builder.RegisterType<FileLogger>().As<ILogger>();
            builder.Register(ctx => new SellController(ctx.Resolve<BuyApp>()));
            
            //builder.Register(ctx => new AccountController(ctx.Resolve<NewUserApp>()));
            //Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


            
        }
        void Session_Start(object sender, EventArgs e)
        {
            //to do
            
            string me = User.Identity.Name;
            //Response.Write("myself:" + myself + "<br>me:" + me);
        }
    }
}
