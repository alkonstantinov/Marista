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

        public async Task<SiteUser> Login(LoginVM login)
        {
            string hashedPassword = MD5.ConvertToMD5(login.Password);
            return await db.SiteUsers.SingleOrDefaultAsync(u => u.Username == login.Username && u.Password == hashedPassword);
        }
    }
}
