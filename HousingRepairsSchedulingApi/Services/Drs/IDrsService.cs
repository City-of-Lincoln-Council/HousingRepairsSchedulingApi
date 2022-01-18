namespace HousingRepairsSchedulingApi.Services.Drs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Drs;

    public interface IDrsService
    {
        Task<IEnumerable<DrsAppointmentSlot>> CheckAvailability(string sorCode, string locationId, DateTime earliestDate);
    }
}
