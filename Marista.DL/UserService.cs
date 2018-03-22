using Marista.Common.Models;
using Marista.Common.Tools;
using Marista.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.DL
{
    public class UserService
    {
        private readonly MaristaEntities db = new MaristaEntities();

        public UserService()
        {
        }

        public async Task<UserData> Login(LoginVM login)
        {
            string hashedPassword = MD5.ConvertToMD5(login.Password);
            var user = await db.SiteUsers.SingleOrDefaultAsync(u => u.Username == login.Username && u.Password == hashedPassword);
            if (user != null)
            {
                if (user.LevelId == 2)
                    if (!db.BPs.First(bp => bp.SiteUserId == user.SiteUserId).Active)
                        return null;
                return new UserData()
                {
                    UserId = user.SiteUserId,
                    Username = user.Username,
                    LevelId = user.LevelId
                };
            }
            else return null;
        }

        public async Task StoreSessionId(int userId, string sessionId)
        {
            var user = await db.SiteUsers.SingleOrDefaultAsync(x => x.SiteUserId == userId);
            user.CurrentSessionId = sessionId;
            await db.SaveChangesAsync();
        }

        public async Task<UserData> GetBySessionId(string sessionId)
        {
            var user = await db.SiteUsers.SingleOrDefaultAsync(x => x.CurrentSessionId == sessionId);
            if (user != null)
            {
                return new UserData()
                {
                    UserId = user.SiteUserId,
                    Username = user.Username,
                    LevelId = user.LevelId
                };
            }
            else return null;
        }
    }
}
