namespace HousingRepairsSchedulingApi.Gateways
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using Domain.Drs;
    using HACT.Dtos;
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

        public async Task<IEnumerable<Appointment>> GetAvailableAppointments(string sorCode, string locationId,
            DateTime? fromDate = null)
        {
            Guard.Against.NullOrWhiteSpace(sorCode, nameof(sorCode));
            Guard.Against.NullOrWhiteSpace(locationId, nameof(locationId));

            var earliestDate = fromDate ?? DateTime.Today.AddDays(appointmentLeadTimeInDays);
            var drsAppointmentSlots = Enumerable.Empty<DrsAppointmentSlot>();

            while (drsAppointmentSlots.Select(x => x.StartTime.Date).Distinct().Count() < requiredNumberOfAppointmentDays)
            {
                var appointments = await drsService.CheckAvailability(sorCode, locationId, earliestDate);
                appointments = appointments.Where(x =>
                    !(x.StartTime.Hour == 9 && x.EndTime.Minute == 30
                      && x.EndTime.Hour == 14 && x.EndTime.Minute == 30)
                );
                drsAppointmentSlots = drsAppointmentSlots.Concat(appointments);
                earliestDate = earliestDate.AddDays(appointmentSearchTimeSpanInDays);
            }

            var result = drsAppointmentSlots.Select(x => x.ToHactAppointment());

            return result;
        }
    }
}
