using AutoMapper;
using Marista.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.DL
{
    internal class VMMapper
    {
        private readonly IMapper _map;
        public IMapper Mapper => _map;

        private static readonly Lazy<VMMapper> _self = new Lazy<VMMapper>(() => new VMMapper());

        public static VMMapper Instance => _self.Value;

        private VMMapper()
        {
            var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Product, ProductVM>()
                        .ReverseMap()
                        .PreserveReferences()
                        .ForMember(dest => dest.Picture,
                            y => y.Condition(src => src.Picture != null && src.Picture.Length > 0))
                        .ForMember(dest => dest.HCategory, y => y.Ignore())
                        .ForMember(dest => dest.VCategory, y => y.Ignore());

                    cfg.CreateMap<BonusSize, BonusSizeVM>()
                        .ReverseMap();

                    cfg.CreateMap<SOBonusSize, SOBonusSizeVM>()
                        .ReverseMap();

                    cfg.CreateMap<Constant, ConstantVM>()
                        .ReverseMap();
                }
            );


            _map = config.CreateMapper();
        }


    }
}
