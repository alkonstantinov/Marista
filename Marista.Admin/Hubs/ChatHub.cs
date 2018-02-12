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

        //private readonly static ConcurrentDictionary<string, int> _connections = new ConcurrentDictionary<string, int>();

        public async override Task OnConnected()
        {


            //var userData = await GetUserData();
            //if (userData != null)
            //{
            //    var chat = await _cs.Get(userData.UserId, IsUp());

            //    var chatGroup = $"group-{chat.ChatId}";
            //    _connections.TryAdd(Context.ConnectionId, chat.ChatId);
            //    await Groups.Add(Context.ConnectionId, chatGroup);
            //}

            await base.OnConnected();
        }

        public void JoinGroup(int chatId)
        {
            this.Groups.Add(this.Context.ConnectionId, $"group-{chatId}");
        }

        public async Task SendMessage(string message, string chatId, string cacheId)
        {
            //var chatId = _connections[Context.ConnectionId];
            var userData = await GetUserData();
            if (userData != null)
            {
                ChatItemVM chatItem = null;
                if (cacheId != null)
                {
                    AttachmentVM att = HttpContext.Current.Cache[cacheId] as AttachmentVM;
                    chatItem = new ChatItemVM()
                    {
                        ChatId = int.Parse(chatId),
                        OnDate = DateTime.Now,
                        SiteUserId = userData.UserId,
                        FileName = System.IO.Path.GetFileName(att.Filename),
                        Said = message,
                        Attachment = att.Content
                    };
                    
                }
                else
                {
                    chatItem = new ChatItemVM()
                    {
                        ChatId = int.Parse(chatId),
                        OnDate = DateTime.Now,
                        SiteUserId = userData.UserId,
                        Said = message,
                        Attachment = null,
                        FileName = null
                    };
                    await _cs.CreateChatItem(chatItem);
                }
                Int32 id = await _cs.CreateChatItem(chatItem);
                chatItem.ChatItemId = id;
                Clients.Group($"group-{chatId}").SendMessage(userData.UserId, userData.Username, DateTime.Now.ToString(), chatItem);
            }
        }

        private async Task<UserData> GetUserData()
        {
            var cookie = Context.RequestCookies["ASP.NET_SessionId"];
            if (cookie == null) return null;

            return await _us.GetBySessionId(cookie.Value);
        }

        //private bool IsUp()
        //{
        //    var s = Context.Request.QueryString["up"];
        //    return !string.IsNullOrEmpty(s) && bool.Parse(s);
        //}
    }
}