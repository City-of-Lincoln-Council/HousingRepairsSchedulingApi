namespace HousingRepairsSchedulingApi.Services.Drs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using Domain.Drs;
    using Microsoft.Extensions.Options;

    public class DrsService : IDrsService
    {
        private readonly SOAP drsSoapClient;
        private readonly IOptions<DrsOptions> drsOptions;

        public DrsService(SOAP drsSoapClient, IOptions<DrsOptions> drsOptions)
        {
            Guard.Against.Null(drsSoapClient, nameof(drsSoapClient));
            Guard.Against.Null(drsOptions, nameof(drsOptions));

            this.drsSoapClient = drsSoapClient;
            this.drsOptions = drsOptions;
        }

        public Task<IEnumerable<DrsAppointmentSlot>> CheckAvailability(string sorCode, string locationId, DateTime earliestDate) => throw new NotImplementedException();
    }
}
