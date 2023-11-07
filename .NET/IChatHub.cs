using Sabio.Models.Domain.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Web.Hubs.Clients
{
    public interface IChatHub
    {
        Task SendMessage(string message);

        Task SendMessageToCaller(string message);
        
        Task ReceiveMessage(string message);
    }
}
