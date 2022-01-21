using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace HousingRepairsSchedulingApi.Tests.ControllersTests
{
    using System;
    using Controllers;
    using UseCases;

    public class AppointmentsControllerTests : ControllerTests
    {
        private const string SorCode = "SOR Code";
        private const string LocationId = "locationId";
        private AppointmentsController systemUndertest;
        private Mock<IRetrieveAvailableAppointmentsUseCase> availableAppointmentsUseCaseMock;

        public AppointmentsControllerTests()
        {
            availableAppointmentsUseCaseMock = new Mock<IRetrieveAvailableAppointmentsUseCase>();
            this.systemUndertest = new AppointmentsController(availableAppointmentsUseCaseMock.Object);
        }

        [Fact]
        public async Task TestAvailableAppointmentsEndpoint()
        {
            var result = await this.systemUndertest.AvailableAppointments(SorCode, LocationId);
            GetStatusCode(result).Should().Be(200);
            availableAppointmentsUseCaseMock.Verify(x => x.Execute(SorCode, LocationId, null), Times.Once);
        }

        [Fact]
        public async Task TestBookAppointmentEndpoint()
        {
            const string bookingReference = "bookingReference";
            var startDateTime = It.IsAny<DateTime>();
            var endDateTime = It.IsAny<DateTime>();

            var result = await this.systemUndertest.BookAppointment(bookingReference, SorCode, LocationId, startDateTime, endDateTime);
            GetStatusCode(result).Should().Be(200);
        }

        [Fact]
#pragma warning disable CA1707
        public async Task GivenAFromDate_WhenRequestingAvailableAppointment_ThenResultsAreReturned()
#pragma warning restore CA1707
        {
            // Arrange
            const string sorCode = "sorCode";
            const string locationId = "locationId";
            var fromDate = new DateTime(2021, 12, 15);

            // Act
            var result = await this.systemUndertest.AvailableAppointments(sorCode, locationId, fromDate);

            // Assert
            GetStatusCode(result).Should().Be(200);
            availableAppointmentsUseCaseMock.Verify(x => x.Execute(sorCode, locationId, fromDate), Times.Once);
        }
    }
}
