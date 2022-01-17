namespace HousingRepairsSchedulingApi.Gateways
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
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
            var result = Enumerable.Empty<Appointment>();

            while (result.Select(x => x.Date).Distinct().Count() < requiredNumberOfAppointmentDays)
            {
                var appointments = await drsService.CheckAvailability(sorCode, locationId, earliestDate);
                appointments = appointments.Where(x =>
                    !(x.TimeOfDay.EarliestArrivalTime.Hour == 9 && x.TimeOfDay.EarliestArrivalTime.Minute == 30
                      && x.TimeOfDay.LatestArrivalTime.Hour == 14 && x.TimeOfDay.LatestArrivalTime.Minute == 30)
                );
                result = result.Concat(appointments);
                earliestDate = earliestDate.AddDays(appointmentSearchTimeSpanInDays);
            }

            return result;
        }
    }
}
