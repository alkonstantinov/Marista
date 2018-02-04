using AutoMapper;
using Marista.Common.Tools;
using Marista.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Marista.DL
{
    public class CheckoutService
    {
        private readonly MaristaEntities db = new MaristaEntities();
        private IMapper _map;

        public CheckoutService()
        {
            _map = VMMapper.Instance.Mapper;
        }

        

    }
}
