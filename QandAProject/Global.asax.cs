﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using QandAProject.Controllers;
using Dependencies;

namespace QandAProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

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
