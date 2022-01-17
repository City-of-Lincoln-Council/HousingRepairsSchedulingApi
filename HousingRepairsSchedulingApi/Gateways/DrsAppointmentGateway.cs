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
        private readonly IDrsService drsService;

        public DrsAppointmentGateway(IDrsService drsService)
        {
            Guard.Against.Null(drsService, nameof(drsService));
            this.drsService = drsService;
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
