namespace HousingRepairsSchedulingApi.UseCases
{
    using System;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;

    public class BookAppointmentUseCase : IBookAppointmentUseCase
    {
        public Task<string> Execute(string bookingReference, string sorCode, string locationId,
            DateTime startDateTime, DateTime endDateTime)
        {
            Guard.Against.NullOrWhiteSpace(bookingReference, nameof(bookingReference));
            Guard.Against.NullOrWhiteSpace(sorCode, nameof(sorCode));
            Guard.Against.NullOrWhiteSpace(locationId, nameof(locationId));
            Guard.Against.OutOfRange(endDateTime, nameof(endDateTime), startDateTime, DateTime.MaxValue);

            return Task.FromResult(bookingReference);
        }
    }
}
