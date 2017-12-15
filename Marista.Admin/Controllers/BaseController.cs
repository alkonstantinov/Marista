using Marista.Common.Models;
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
    }
}