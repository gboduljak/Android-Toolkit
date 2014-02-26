using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;

namespace AndroidToolkitWeb
{
    public class VisitCount : Hub
    {
        public static int _visits=0;

        public void Send()
        {
            string message = string.Empty;

            if (_visits< 2)
                message = string.Format("Currently {0} user is online.", _visits);
            else
                message = string.Format("Currently {0} users are online.", _visits);

            Clients.All.visits(message);
        }

        public override Task OnConnected()
        {
            _visits++;
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            _visits--;
            return base.OnDisconnected();
        }
    }
}