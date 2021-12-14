using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HACT.Dtos;
namespace HousingRepairsSchedulingApi.Gateways
{
    public interface IAppointmentsGateway
    {
        Task<IEnumerable<Appointment>> GetAvailableAppointments(string sorCode, string locationId, DateTime? fromDate = null);
    }
}
