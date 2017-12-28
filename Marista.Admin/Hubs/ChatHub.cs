using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Marista.Common.ViewModels;
using Marista.DL;
using Microsoft.AspNet.SignalR;

namespace Marista.Admin.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatService _cs = new ChatService();
        private readonly static ConcurrentDictionary<string, int> _connections = new ConcurrentDictionary<string, int>();

        public async override Task OnConnected()
        {
            int siteUserId = 1;
            var chat = await _cs.Get(siteUserId);

            var chatGroup = $"group-{chat.ChatId}";
            _connections.TryAdd(Context.ConnectionId, chat.ChatId);
            await Groups.Add(Context.ConnectionId, chatGroup);

            await base.OnConnected();
        }

        public async Task SendMessage(string from, string message)
        {
            var chatId = _connections[Context.ConnectionId];
            Clients.Group($"group-{chatId}").SendMessage(from, message);
            var chatItem = new ChatItemVM()
            {
                ChatId = chatId,
                OnDate = DateTime.Now,
                SiteUserId = 1,
                Said = message,
                Attachment = null
            };
            await _cs.CreateChatItem(chatItem);
        }
    }
}