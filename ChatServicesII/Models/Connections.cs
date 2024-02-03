

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ChatServicesII.Models
{

    public static class Connections
    {
        public static ConcurrentDictionary<string, List<string>> ConnectedUsers = new ConcurrentDictionary<string, List<string>>();
    }
}