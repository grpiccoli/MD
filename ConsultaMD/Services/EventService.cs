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
        public async Task AddEvents(List<AgendaEvent> events)
        {
            if (events != null && events.Count != 0) {
                foreach (var e in events)
                {
                    await AddEvent(e).ConfigureAwait(false);
                }
            }
            return;
        }
        public async Task AddEvent(AgendaEvent e)
        {
            if(e != null)
            {
                await _context.AgendaEvents.AddAsync(e).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                var startEvent = e.StartDateTime.AddDays(-(double)e.StartDateTime.DayOfWeek);
                var maxEvent = e.EndDateTime;
                var daysEvent = (maxEvent.Date - startEvent.Date).Days;
                while (startEvent < maxEvent)
                {
                    foreach (var day in e.DaysOfWeek.Where(d => (int)d <= daysEvent))
                    {
                        var startAgenda = startEvent.AddDays((int)day);
                        if (startAgenda > maxEvent) break;
                        var agenda = new Agenda
                        {
                            AgendaEventId = e.Id,
                            StartTime = startAgenda,
                        };
                        await _context.Agenda.AddAsync(agenda).ConfigureAwait(false);
                        var weekdays = new EventDayWeek
                        {
                            AgendaEventId = e.Id,
                            DayOfWeek = day
                        };
                        await _context.EventDayWeeks.AddAsync(weekdays).ConfigureAwait(false);
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                        var startTime = startAgenda;
                        var slots = new List<TimeSlot>();
                        while (startTime.TimeOfDay < maxEvent.TimeOfDay)
                        {
                            slots.Add(new TimeSlot
                            {
                                AgendaId = agenda.Id,
                                StartTime = startTime,
                                EndTime = startTime.Add(e.Duration)
                            });
                            startTime = startTime.Add(e.Duration);
                        }
                        await _context.TimeSlots.AddRangeAsync(slots).ConfigureAwait(false);
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                    }
                    startEvent = startEvent.AddDays(7 * e.Frequency);
                }
            }
            return;
        }
        public async Task DeleteEvent(int id)
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
        public async Task DeleteEvents(List<int> ids)
        {
            if(ids != null)
            {
                foreach (int id in ids)
                {
                    await DeleteEvent(id).ConfigureAwait(false);
                }
            }
            return;
        }
        public async Task DisableEvent(int id)
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
