using Application.DTOs.Notifications;
using AutoMapper;
using Core.Entities.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class NotificiationsProfile : Profile
    {
        public NotificiationsProfile()
        {
            CreateMap<NotificationsMarkAllDto , NotificationPersist>();
            CreateMap<NotificationsMarkDto, NotificationPersist>();
        }
    }
}
