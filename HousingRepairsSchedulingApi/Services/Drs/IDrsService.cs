namespace HousingRepairsSchedulingApi.Services.Drs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HACT.Dtos;

    public interface IDrsService
    {
        Task<IEnumerable<Appointment>> CheckAvailability(string sorCode, string locationId, DateTime earliestDate);
    }
}
