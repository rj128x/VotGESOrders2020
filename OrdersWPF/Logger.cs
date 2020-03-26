using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotGESOrders.AdditService;

namespace VotGESOrders
{
    public class Logger
    {
        public static Logger Current { get; protected set; }
        protected AdditServiceClient client;
        static Logger()
        {
            Current = new Logger();
            Current.client = new AdditServiceClient();
        }
        public static void logMessage(String message)
        {
            Current.client.info(message);
        }
    }
}
