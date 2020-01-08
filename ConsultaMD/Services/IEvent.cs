﻿using ConsultaMD.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public interface IEvent
    {
        Task AddRange(List<AgendaEvent> events);
        Task Add(AgendaEvent e);
        Task DeleteRange(List<int> eventIds);
        Task Delete(int id);
        Task Enable(int id);
        Task Disable(int id);
    }
}
