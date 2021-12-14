namespace HousingRepairsSchedulingApi.Gateways
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HACT.Dtos;

    public class DummyAppointmentsGateway : IAppointmentsGateway
    {
        public async Task<IEnumerable<Appointment>> GetAvailableAppointments(string sorCode, string locationId, DateTime? fromDate = null)
        {
            var dateTime = (fromDate ?? DateTime.Today).Date;
            List<Appointment> unorderedAppointments = new List<Appointment>
            {
                new ()
                {
                    Date = dateTime.AddDays(16),
                    TimeOfDay = new TimeOfDay
                    {
                        EarliestArrivalTime = dateTime.AddDays(16).AddHours(8),
                        LatestArrivalTime = dateTime.AddDays(16).AddHours(12)
                    },
                },
                new()
                {
                    Date = DateTime.Today.AddDays(20).AddDays(2),
                    TimeOfDay = new TimeOfDay
                    {
                        EarliestArrivalTime = dateTime.AddDays(20).AddDays(2).AddHours(12),
                        LatestArrivalTime = dateTime.AddDays(20).AddDays(2).AddHours(16)
                    },
                },
                new()
                {
                    Date = DateTime.Today.AddDays(7).AddDays(7),
                    TimeOfDay = new TimeOfDay
                    {
                        EarliestArrivalTime = dateTime.AddDays(7).AddDays(7).AddHours(8),
                        LatestArrivalTime = dateTime.AddDays(7).AddDays(7).AddHours(12)
                    },
                },
                new ()
                {
                    Date = DateTime.Today.AddDays(1).AddDays(1),
                    TimeOfDay = new TimeOfDay
                    {
                        EarliestArrivalTime = dateTime.AddDays(1).AddDays(1).AddHours(8),
                        LatestArrivalTime = dateTime.AddDays(1).AddDays(1).AddHours(12)
                    },
                },
                new ()
                {
                    Date = DateTime.Today.AddDays(5).AddDays(5),
                    TimeOfDay = new TimeOfDay
                    {
                        EarliestArrivalTime = dateTime.AddDays(5).AddDays(5).AddHours(12),
                        LatestArrivalTime = dateTime.AddDays(5).AddDays(5).AddHours(16)
                    },
                }
            };
            var orderedAppointments = unorderedAppointments.OrderBy(x => x.Date);
            return orderedAppointments;
        }
    }
}
