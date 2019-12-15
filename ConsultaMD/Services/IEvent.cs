using ConsultaMD.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public interface IEvent
    {
        Task AddEvents(List<AgendaEvent> events);
        Task AddEvent(AgendaEvent e);
        Task DeleteEvents(List<int> eventIds);
        Task DeleteEvent(int id);
        Task DisableEvent(int id);
    }
}
