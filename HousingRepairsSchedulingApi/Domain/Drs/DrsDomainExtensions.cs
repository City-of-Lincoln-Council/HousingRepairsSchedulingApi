namespace HousingRepairsSchedulingApi.Domain.Drs
{
    public static class DrsDomainExtensions
    {
        public static AppointmentSlot ToAppointmentSlot(this DrsAppointmentSlot drsAppointmentSlot) =>
            new()
            {
                StartTime = drsAppointmentSlot.StartTime,
                EndTime = drsAppointmentSlot.EndTime
            };
    }
}
