using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortnerApi
{
    /// <summary>
    /// In production, we could make this scalable and consistent
    /// by using a service such as Apache Zookeeper 
    /// to assign distinct ranges of values across nodes as they are added
    /// https://zookeeper.apache.org/doc/current/index.html
    /// </summary>
    public static class Zookeeper
    {
        private static ulong _counter;

        public static ulong Counter
        {
            get { 
                _counter++;
                return _counter;
            }
            set {
                _counter = value;
            }
        }

    }
}
