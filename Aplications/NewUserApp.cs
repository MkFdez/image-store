using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dependencies;

namespace Aplications
{
    public class NewUserApp : INewUserApp
    {
        IEmailSender _emailsender;
        public IEmailModel Model { get; set; }
        public NewUserApp(IEmailSender emailsender)
        {
            _emailsender = emailsender;

        }

        public void Run()
        {
            _emailsender.SendEmail(Model);
        }
    }
}
