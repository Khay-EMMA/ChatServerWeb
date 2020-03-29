using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ChatServerWeb.Model.Entity;
using ChatServerWeb.Model.ViewModel;

namespace ChatServerWeb.Api.Model
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ChatMessage, ChatMessageViewModel>();
            CreateMap<ChatMessageViewModel, ChatMessage>();
        }
    }
}
