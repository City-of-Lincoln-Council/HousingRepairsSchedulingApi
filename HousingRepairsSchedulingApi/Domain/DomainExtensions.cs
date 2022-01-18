namespace HousingRepairsSchedulingApi.Domain
{
    using HACT.Dtos;

    public static class DomainExtensions
    {
        public static Appointment ToHactAppointment(this AppointmentSlot appointmentSlot) =>
            new()
            {
                Date = appointmentSlot.StartTime.Date,
                TimeOfDay = new TimeOfDay
                {
                    EarliestArrivalTime = appointmentSlot.StartTime,
                    LatestArrivalTime = appointmentSlot.EndTime,
                }
            };
    }
}
