using ConsultaMD.Data;
using ConsultaMD.Models.Entities;
using System;
using System.Collections.Generic;
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
        public Task AddEvents(List<AgendaEvent> events)
        {
            if (events != null && events.Count != 0) {
                foreach (var e in events)
                {
                    _context.AgendaEvents.Add(e);
                    _context.SaveChanges();
                    var start = e.StartDateTime;
                    var max = e.EndDateTime;
                    while (start < max)
                    {
                        foreach(var day in e.daysOfWeek)
                        {
                            var agenda = new Agenda
                            {
                                AgendaEventId = e.Id,
                                StartTime = start.AddDays((int)day),
                            };
                            _context.Agenda.Add(agenda);
                            var weekdays = new EventDayWeek
                            {
                                AgendaEventId = e.Id,
                                DayOfWeek = day
                            };
                            _context.EventDayWeeks.Add(weekdays);
                            _context.SaveChanges();
                            var startTime = start;
                            var end = startTime.Add(e.Duration);
                            var slots = new List<TimeSlot>();
                            while (startTime.TimeOfDay < max.TimeOfDay) {
                                slots.Add(new TimeSlot
                                {
                                    AgendaId = agenda.Id,
                                    StartTime = startTime,
                                    EndTime = end
                                });
                                startTime = startTime.Add(e.Duration);
                            }
                            _context.TimeSlots.AddRange(slots);
                            _context.SaveChanges();
                        }
                        start = start.AddDays(7 * e.Frequency);
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
