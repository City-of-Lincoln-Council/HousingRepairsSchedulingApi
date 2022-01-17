namespace HousingRepairsSchedulingApi.Gateways
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using HACT.Dtos;

    public class DrsAppointmentGateway : IAppointmentsGateway
    {
        private readonly string drsUrl;

        public DrsAppointmentGateway(string drsUrl)
        {
            Guard.Against.NullOrWhiteSpace(drsUrl, nameof(drsUrl));

            this.drsUrl = drsUrl;
        }

        public Task<IEnumerable<Appointment>> GetAvailableAppointments(string sorCode, string locationId,
            DateTime? fromDate = null)
        {
            Guard.Against.NullOrWhiteSpace(sorCode, nameof(sorCode));
            Guard.Against.NullOrWhiteSpace(locationId, nameof(locationId));

            return Task.FromResult(Enumerable.Empty<Appointment>());
        }
    }
}
