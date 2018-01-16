using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Marista.Common.Models;
using Marista.Common.ViewModels;
using Marista.DL;
using Microsoft.AspNet.SignalR;
using System.Web.SessionState;

namespace Marista.Admin.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatService _cs = new ChatService();
        private readonly UserService _us = new UserService();

        private readonly static ConcurrentDictionary<string, int> _connections = new ConcurrentDictionary<string, int>();

        public async override Task OnConnected()
        {
            

            var userData = await GetUserData();
            if (userData != null)
            {
                var chat = await _cs.Get(userData.UserId);

                var chatGroup = $"group-{chat.ChatId}";
                _connections.TryAdd(Context.ConnectionId, chat.ChatId);
                await Groups.Add(Context.ConnectionId, chatGroup);
            }

            await base.OnConnected();
        }

        public async Task SendMessage(string message)
        {
            var chatId = _connections[Context.ConnectionId];
            var userData = await GetUserData();
            if (userData != null)
            {
                Clients.Group($"group-{chatId}").SendMessage(userData.UserId, userData.Username, DateTime.Now.ToString(), message);
                var chatItem = new ChatItemVM()
                {
                    ChatId = chatId,
                    OnDate = DateTime.Now,
                    SiteUserId = userData.UserId,
                    Said = message,
                    Attachment = null
                };
                await _cs.CreateChatItem(chatItem);
            }
        }

        private async Task<UserData> GetUserData()
        {
            var cookie = Context.RequestCookies["ASP.NET_SessionId"];
            if (cookie == null) return null;
            
            return await _us.GetBySessionId(cookie.Value);
        }
    }
}