using Logger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Logger
{
    public interface ILogger
    {
        void LogError(Exception e, LogModel info);
        void LogInfo(LogModel info);
       
    }
}
