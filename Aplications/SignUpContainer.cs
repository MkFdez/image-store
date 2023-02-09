using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

using Dependencies;

namespace Aplications
{
    public static class SignUpContainer
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<SignUpEmailSender>().As<IEmailSender>();
            builder.RegisterType<SignUpEmailModel>().As<IEmailModel>();
            builder.RegisterType<Logger>().As<ILogger>();
            builder.RegisterType<NewUserApp>().As<IApp>();

            return builder.Build();
            
        }
    }
}
