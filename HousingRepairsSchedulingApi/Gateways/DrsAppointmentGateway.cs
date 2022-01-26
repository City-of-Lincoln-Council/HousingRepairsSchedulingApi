namespace HousingRepairsSchedulingApi.Gateways
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using Domain;
    using Services.Drs;

    public class DrsAppointmentGateway : IAppointmentsGateway
    {
        private readonly int requiredNumberOfAppointmentDays;
        private readonly int appointmentSearchTimeSpanInDays;
        private readonly int appointmentLeadTimeInDays;
        private readonly IDrsService drsService;

        public DrsAppointmentGateway(IDrsService drsService, int requiredNumberOfAppointmentDays, int appointmentSearchTimeSpanInDays, int appointmentLeadTimeInDays)
        {
            Guard.Against.Null(drsService, nameof(drsService));
            Guard.Against.NegativeOrZero(requiredNumberOfAppointmentDays, nameof(requiredNumberOfAppointmentDays));
            Guard.Against.NegativeOrZero(appointmentSearchTimeSpanInDays, nameof(appointmentSearchTimeSpanInDays));
            Guard.Against.Negative(appointmentLeadTimeInDays, nameof(appointmentLeadTimeInDays));

            this.drsService = drsService;
            this.requiredNumberOfAppointmentDays = requiredNumberOfAppointmentDays;
            this.appointmentSearchTimeSpanInDays = appointmentSearchTimeSpanInDays;
            this.appointmentLeadTimeInDays = appointmentLeadTimeInDays;
        }

        public async Task<IEnumerable<AppointmentSlot>> GetAvailableAppointments(string sorCode, string locationId,
            DateTime? fromDate = null)
        {
            Guard.Against.NullOrWhiteSpace(sorCode, nameof(sorCode));
            Guard.Against.NullOrWhiteSpace(locationId, nameof(locationId));

            var earliestDate = fromDate ?? DateTime.Today.AddDays(appointmentLeadTimeInDays);
            var appointmentSlots = Enumerable.Empty<AppointmentSlot>();

            while (appointmentSlots.Select(x => x.StartTime.Date).Distinct().Count() < requiredNumberOfAppointmentDays)
            {
                var appointments = await drsService.CheckAvailability(sorCode, locationId, earliestDate);
                appointments = appointments.Where(x =>
                    !(x.StartTime.Hour == 9 && x.EndTime.Minute == 30
                      && x.EndTime.Hour == 14 && x.EndTime.Minute == 30) &&
                    !(x.StartTime.Hour == 8 && x.EndTime.Minute == 0
                                            && x.EndTime.Hour == 16 && x.EndTime.Minute == 0)
                );
                appointmentSlots = appointmentSlots.Concat(appointments);
                earliestDate = earliestDate.AddDays(appointmentSearchTimeSpanInDays);
            }

            return appointmentSlots;
        }
    }
}
