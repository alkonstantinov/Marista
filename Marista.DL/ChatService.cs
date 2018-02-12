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

        public async Task<ChatVM> Get(int siteUserId, bool up)
        {
            // TODO: modify the condition to fetch a specific chat
            /*var c = await db.Chats
                .Include(x => x.SiteUser)
                .Include(x => x.ChatItems)
                .Include(x => x.ChatItems.Select(y => y.SiteUser))
                .SingleOrDefaultAsync(x => x.SiteUserId == siteUserId);*/

            int realUserId = siteUserId;
            if (up)
            {
                var p = db.Pyramids.First(pr => pr.SiteUserId == siteUserId);
                if (!p.PyramidParentId.HasValue)
                    return null;
                p = db.Pyramids.First(pr => pr.PyramidId == p.PyramidParentId);
                realUserId = p.SiteUserId;                
            }
            var c = await db.Chats
                .Include(x => x.SiteUser)
                .Include(x => x.ChatItems)
                .Include(x => x.ChatItems.Select(y => y.SiteUser))
                .SingleOrDefaultAsync(x => x.SiteUserId == realUserId);
            if (c == null)
            {
                c = new Chat()
                {
                    SiteUserId = realUserId
                };
                db.Chats.Add(c);
                db.SaveChanges();
            }
            var cvm = _map.Map<ChatVM>(c);
            cvm.Items = _map.Map<ICollection<ChatItem>, ICollection<ChatItemVM>>(c.ChatItems);
            return cvm;
        }

        public async Task<ChatVM> GetWholeChat(int chatId)
        {
            var c = await db.Chats
                .Include(x => x.SiteUser)
                .Include(x => x.ChatItems)
                .Include(x => x.ChatItems.Select(y => y.SiteUser))
                .SingleOrDefaultAsync(x => x.ChatId== chatId);
            var cvm = _map.Map<ChatVM>(c);
            cvm.Items = _map.Map<ICollection<ChatItem>, ICollection<ChatItemVM>>(c.ChatItems);
            return cvm;
        }


        public async Task<ChatItemVM> Get(int chatItemId)
        {
            // TODO: modify the condition to fetch a specific chat
            /*var c = await db.Chats
                .Include(x => x.SiteUser)
                .Include(x => x.ChatItems)
                .Include(x => x.ChatItems.Select(y => y.SiteUser))
                .SingleOrDefaultAsync(x => x.SiteUserId == siteUserId);*/

            var c = await db.ChatItems.FirstAsync(item => item.ChatItemId == chatItemId);
            return _map.Map<ChatItemVM>(c);
        }

        public async Task<Int32> CreateChatItem(ChatItemVM item)
        {
            var chatItem = _map.Map<ChatItem>(item);
            db.ChatItems.Add(chatItem);
            await db.SaveChangesAsync();
            return chatItem.ChatItemId;
        }
    }
}
