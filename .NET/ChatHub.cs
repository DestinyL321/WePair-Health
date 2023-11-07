using Sabio.Models.Domain.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Sabio.Web.Hubs.Clients;
using Sabio.Services.Interfaces;
using Sabio.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace Sabio.Web.Hubs
{
    public class ChatHub : Hub<IChatHub>
    {
        private IMessageService _messageService = null;
        private IAuthenticationService<int> _authService = null;

        public ChatHub(IMessageService service
            , IAuthenticationService<int> authService)
        {
            _messageService = service;
            _authService = authService;
        }

        public override async Task OnConnectedAsync()
        {
            int userId = _authService.GetCurrentUserId();
            await _messageService.UserConnected(userId, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.ReceiveMessage($"{Context.ConnectionId}: {message}");
        }

        public async Task SendMessageToCaller(string message)
        {
            await Clients.Caller.ReceiveMessage($"{Context.ConnectionId}: {message}");
        }

        //public async Task ReceiveMessage(string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", message);
        //}
    }
}
