namespace HousingRepairsSchedulingApi.Services.Drs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using Domain;
    using Microsoft.Extensions.Options;

    public class DrsService : IDrsService
    {
        private const string DrsContract = "0";
        private const string DummyPrimaryOrderNumber = "HousingRepairsOnlineDummyPrimaryOrderNumber";
        private const string DummyUserId = "HousingRepairsOnlineUserId";
        private const string Priority = "Priority 20 Day";

        private readonly SOAP drsSoapClient;
        private readonly IOptions<DrsOptions> drsOptions;

        private string sessionId;

        public DrsService(SOAP drsSoapClient, IOptions<DrsOptions> drsOptions)
        {
            Guard.Against.Null(drsSoapClient, nameof(drsSoapClient));
            Guard.Against.Null(drsOptions, nameof(drsOptions));

            this.drsSoapClient = drsSoapClient;
            this.drsOptions = drsOptions;
        }

        public async Task<IEnumerable<AppointmentSlot>> CheckAvailability(string sorCode, string locationId, DateTime earliestDate)
        {
            await EnsureSessionOpened();

            var checkAvailability = new xmbCheckAvailability
            {
                sessionId = this.sessionId,
                periodBegin = earliestDate,
                periodBeginSpecified = true,
                periodEnd = earliestDate.AddDays(drsOptions.Value.SearchTimeSpanInDays - 1),
                periodEndSpecified = true,
                theOrder = new order
                {
                    userId = DummyUserId,
                    contract = DrsContract,
                    locationID = locationId,
                    primaryOrderNumber = DummyPrimaryOrderNumber,
                    priority = Priority,
                    theBookingCodes = new[]{
                        new bookingCode {
                            bookingCodeSORCode = sorCode,
                            itemNumberWithinBooking = "1",
                            primaryOrderNumber = DummyPrimaryOrderNumber,
                            quantity = "1",
                        }
                    }
                }
            };

            var checkAvailabilityResponse = await this.drsSoapClient.checkAvailabilityAsync(new checkAvailability(checkAvailability));

            var appointmentSlots = checkAvailabilityResponse.@return.theSlots
                .Where(x => x.slotsForDay != null)
                .SelectMany(x =>
                    x.slotsForDay.Where(y => y.available == availableValue.YES).Select(y =>
                        new AppointmentSlot
                        {
                            StartTime = y.beginDate,
                            EndTime = y.endDate,
                        }
                    )
            );

            return appointmentSlots;
        }

        private async Task OpenSession()
        {
            var xmbOpenSession = new xmbOpenSession
            {
                login = drsOptions.Value.Login,
                password = drsOptions.Value.Password
            };
            var response = await this.drsSoapClient.openSessionAsync(new openSession(xmbOpenSession));

            sessionId = response.@return.sessionId;
        }

        private async Task EnsureSessionOpened()
        {
            if (this.sessionId == null)
            {
                await OpenSession();
            }
        }
    }
}
