using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Dependencies;

namespace Aplications
{
    public static class BuyContainer
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();           
            builder.RegisterType<BuyEmailSender>().As<IEmailSender>();
            builder.RegisterType<BuyApp>().As<IApp>();

            return builder.Build();
            
        }
    }
}
