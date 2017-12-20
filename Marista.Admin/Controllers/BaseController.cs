using Marista.Common.Models;
using Marista.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marista.Admin.Controllers
{
    public class BaseController : Controller
    {
        public UserData UserData
        {
            get
            {
                if (Session["UserData"] == null)
                    return null;
                else
                    return Session["UserData"] as UserData;
            }
            set
            {
                Session["UserData"] = value;
            }
        }

        private UserService _us;
        public UserService UserService
        {
            get
            {
                if (_us == null) _us = new UserService();
                return _us;
            }
        }
    }
}