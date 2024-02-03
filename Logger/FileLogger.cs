using Logger.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Logger
{
    public class FileLogger : ILogger
    {
        public void LogError(Exception e, LogModel info)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(HttpContext.Current.Server.MapPath("../Log"), "errorlog.txt"), true))
            {
                sw.WriteLine($"Date: {DateTime.Now} \n Controller: {info.Controller} \n Action: {info.Action} \n Error: {e.Message} \n ErrorType: {info.type.ToString()} \n");
            }
        }

        public void LogInfo(LogModel info)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(HttpContext.Current.Server.MapPath("../Log"), "infolog.txt"), true))
            {
                sw.WriteLine($"Date: {DateTime.Now} \n Controller: {info.Controller} \n Action: {info.Action} \n Message: {info.Message} \n ErrorType: {info.type.ToString()} \n");
            }
        }
    }
}
