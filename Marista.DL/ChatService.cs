using AutoMapper;
using Marista.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.DL
{
    public class ChatService
    {
        private readonly MaristaEntities db = new MaristaEntities();
        private IMapper _map;

        public ChatService()
        {
            _map = VMMapper.Instance.Mapper;
        }

        public async Task<ChatVM> Get(int siteUserId)
        {
            // TODO: modify the condition to fetch a specific chat
            var c = await db.Chats
                .Include(x => x.SiteUser)
                .Include(x => x.ChatItems)
                .Include(x => x.ChatItems.Select(y => y.SiteUser))
                .SingleOrDefaultAsync(x => x.SiteUserId == siteUserId);
            if(c == null)
            {
                return null;
            }
            var cvm = _map.Map<ChatVM>(c);
            cvm.Items = _map.Map<ICollection<ChatItem>, ICollection<ChatItemVM>>(c.ChatItems);
            return cvm;
        }

        public async Task CreateChatItem(ChatItemVM item)
        {
            var chatItem = _map.Map<ChatItem>(item);
            db.ChatItems.Add(chatItem);
            await db.SaveChangesAsync();
        }
    }
}
