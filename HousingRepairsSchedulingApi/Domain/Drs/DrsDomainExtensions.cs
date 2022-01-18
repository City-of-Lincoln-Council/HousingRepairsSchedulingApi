namespace HousingRepairsSchedulingApi.Domain.Drs
{
    using HACT.Dtos;

    public static class DrsDomainExtensions
    {
        public static Appointment ToHactAppointment(this DrsAppointmentSlot drsAppointmentSlot) =>
            new()
            {
                Date = drsAppointmentSlot.StartTime.Date,
                TimeOfDay = new TimeOfDay { EarliestArrivalTime = drsAppointmentSlot.StartTime, LatestArrivalTime = drsAppointmentSlot.EndTime }
            };
    }
}
