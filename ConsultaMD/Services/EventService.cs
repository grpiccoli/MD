using ConsultaMD.Data;
using ConsultaMD.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public class EventService : IEvent
    {
        private readonly ApplicationDbContext _context;
        public EventService(ApplicationDbContext context) 
        {
            _context = context;
        }
        public async Task AddRange(List<AgendaEvent> events)
        {
            if (events != null && events.Count != 0) {
                foreach (var e in events)
                {
                    await Add(e).ConfigureAwait(false);
                }
            }
            return;
        }
        public async Task Add(AgendaEvent e)
        {
            if(e != null)
            {
                //CREATE EVENT
                await _context.AgendaEvents.AddAsync(e).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                //CREATE EVENTDAYWEEKS
                var eventDays = e.DaysOfWeek.Select(d => new EventDayWeek
                {
                    AgendaEventId = e.Id,
                    DayOfWeek = d
                });
                await _context.EventDayWeeks.AddRangeAsync(eventDays).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                //AGENDAS
                var agendaStart = e.StartDateTime;
                var agendaLength = e.EndDateTime.TimeOfDay - e.StartDateTime.TimeOfDay;
                var max = e.EndDateTime;
                while (agendaStart.Date <= max.Date)
                {
                    if (e.DaysOfWeek.Contains(agendaStart.DayOfWeek))
                    {
                        var agenda = new Agenda
                        {
                            AgendaEventId = e.Id,
                            StartTime = agendaStart,
                            EndTime = agendaStart.Add(agendaLength)
                        };
                        await _context.Agenda.AddAsync(agenda).ConfigureAwait(false);
                        await _context.SaveChangesAsync().ConfigureAwait(false);

                        var slotStart = agendaStart;
                        var slots = new List<TimeSlot>();
                        while (slotStart.TimeOfDay.Add(e.Duration) <= max.TimeOfDay)
                        {
                            slots.Add(new TimeSlot
                            {
                                AgendaId = agenda.Id,
                                StartTime = slotStart,
                                EndTime = slotStart.Add(e.Duration)
                            });
                            slotStart = slotStart.Add(e.Duration);
                        }
                        await _context.TimeSlots.AddRangeAsync(slots).ConfigureAwait(false);
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                    }
                    agendaStart = agendaStart.AddDays(1);
                }
            }
            return;
        }
        public async Task Delete(int id)
        {
            var e = await _context.AgendaEvents
                .Include(a => a.EventDayWeeks)
                .Include(a => a.Agendas)
                    .ThenInclude(a => a.TimeSlots)
                        .ThenInclude(a => a.Reservation)
                .SingleOrDefaultAsync(a => a.Id == id)
                .ConfigureAwait(false);
            _context.EventDayWeeks.RemoveRange(e.EventDayWeeks);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            _context.Agenda.RemoveRange(e.Agendas);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            _context.TimeSlots
                .RemoveRange(e.Agendas
                .SelectMany(a => a.TimeSlots
                .Where(t => t.ReservationId.HasValue)));
            await _context.SaveChangesAsync().ConfigureAwait(false);
            _context.AgendaEvents.Remove(e);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return;
        }
        public async Task DeleteRange(List<int> ids)
        {
            if(ids != null)
            {
                foreach (int id in ids)
                {
                    await Delete(id).ConfigureAwait(false);
                }
            }
            return;
        }
        public async Task Disable(int id)
        {
            var e = await _context.AgendaEvents
                .Include(a => a.EventDayWeeks)
                .Include(a => a.Agendas)
                    .ThenInclude(a => a.TimeSlots)
                        .ThenInclude(a => a.Reservation)
                .SingleOrDefaultAsync(a => a.Id == id).ConfigureAwait(false);
            //_context.EventDayWeeks.RemoveRange(e.EventDayWeeks);
            //_context.TimeSlots.RemoveRange(e.Agendas.SelectMany(a => a.TimeSlots.Where(t => t.Reservation != null)));
            //_context.Agenda.RemoveRange(e.Agendas);
            //_context.AgendaEvents.Remove(e);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return;
        }
        public async Task Enable(int id)
        {
            var e = await _context.AgendaEvents
                .Include(a => a.EventDayWeeks)
                .Include(a => a.Agendas)
                    .ThenInclude(a => a.TimeSlots)
                        .ThenInclude(a => a.Reservation)
                .SingleOrDefaultAsync(a => a.Id == id).ConfigureAwait(false);
            //_context.EventDayWeeks.RemoveRange(e.EventDayWeeks);
            //_context.TimeSlots.RemoveRange(e.Agendas.SelectMany(a => a.TimeSlots.Where(t => t.Reservation != null)));
            //_context.Agenda.RemoveRange(e.Agendas);
            //_context.AgendaEvents.Remove(e);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return;
        }
    }
}
