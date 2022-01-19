namespace HousingRepairsSchedulingApi.Tests.ServicesTests.Drs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Domain.Drs;
    using FluentAssertions;
    using Microsoft.Extensions.Options;
    using Moq;
    using Services.Drs;
    using Xunit;

    [SuppressMessage("Naming", "CA1707", MessageId = "Identifiers should not contain underscores")]
    public class DrsServiceTests
    {
        private const string SorCode = "SorCode";
        private const string LocationId = "LocationId";

        private Mock<SOAP> soapMock;

        private DrsService systemUnderTest;

        public DrsServiceTests()
        {
            var drsOptionsMock = new Mock<IOptions<DrsOptions>>();
            drsOptionsMock.Setup(x => x.Value)
                .Returns(new DrsOptions { Login = "login", Password = "password" });

            soapMock = new Mock<SOAP>();
            soapMock.Setup(x => x.openSessionAsync(It.IsAny<openSession>()))
                .ReturnsAsync(new openSessionResponse
                {
                    @return = new xmbOpenSessionResponse { sessionId = "sessionId" }
                });

            systemUnderTest = new DrsService(soapMock.Object, drsOptionsMock.Object);

        }

        [Fact]
        public void GivenNullDrsSoapClientParameter_WhenInstantiating_ThenArgumentNullExceptionIsThrown()
        {
            // Arrange

            // Act
            Func<DrsService> act = () => new DrsService(null, It.IsAny<IOptions<DrsOptions>>());

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("drsSoapClient");
        }

        [Fact]
        public void GivenNullDrsOptionsParameter_WhenInstantiating_ThenArgumentNullExceptionIsThrown()
        {
            // Arrange

            // Act
            Func<DrsService> act = () => new DrsService(new Mock<SOAP>().Object, null);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("drsOptions");
        }

        [Theory]
        [MemberData(nameof(UnavailableSlotsTestData))]
        public async void
            GivenDrsCheckAvailabilityResponseContainsUnavailableSlots_WhenCheckingAvailability_ThenOnlyAvailableSlotsAreReturned(DateTime searchDate, daySlotsInfo[] daySlots, IEnumerable<DrsAppointmentSlot> expected)
        {
            // Arrange

            soapMock.Setup(x => x.checkAvailabilityAsync(It.IsAny<checkAvailability>()))
                .ReturnsAsync(new checkAvailabilityResponse(
                    new xmbCheckAvailabilityResponse { theSlots = daySlots }));

            // Act
            var drsAppointmentSlots = await systemUnderTest.CheckAvailability(SorCode, LocationId, searchDate);

            // Assert
            drsAppointmentSlots.Should().BeEquivalentTo(expected);
        }

        public static IEnumerable<object[]> UnavailableSlotsTestData()
        {
            var date = new DateTime(2022, 1, 19);

            yield return new object[]
            {
                date,
                new[]
                {
                    new daySlotsInfo
                    {
                        day = date,
                        slotsForDay = new[]
                        {
                            new slotInfo
                            {
                                available = availableValue.NO,
                                beginDate = date.AddHours(8),
                                endDate = date.AddHours(12),
                            },
                        }
                    }
                },
                Enumerable.Empty<DrsAppointmentSlot>()
            };

            yield return new object[]
            {
                date,
                new[]
                {
                    new daySlotsInfo
                    {
                        day = date,
                        slotsForDay = new[]
                        {
                            new slotInfo
                            {
                                available = availableValue.YES,
                                beginDate = date.AddHours(8),
                                endDate = date.AddHours(12),
                            },
                        }
                    }
                },
                new []{new DrsAppointmentSlot{StartTime = date.AddHours(8), EndTime = date.AddHours(12)}}
            };

            yield return new object[]
            {
                date,
                new[]
                {
                    new daySlotsInfo
                    {
                        day = date,
                        slotsForDay = new[]
                        {
                            new slotInfo
                            {
                                available = availableValue.NO,
                                beginDate = date.AddHours(8),
                                endDate = date.AddHours(12),
                            },
                            new slotInfo
                            {
                                available = availableValue.YES,
                                beginDate = date.AddHours(12),
                                endDate = date.AddHours(16),
                            },
                        }
                    }
                },
                new []{new DrsAppointmentSlot{StartTime = date.AddHours(12), EndTime = date.AddHours(16)}}
            };
        }

        [Fact]
        public async void
            GivenDrsCheckAvailabilityResponseContainsDaysWithNoSlots_WhenCheckingAvailability_ThenArgumentNullExceptionIsNotThrown()
        {
            // Arrange
            var dateTime = new DateTime(2022, 1, 19);

            soapMock.Setup(x => x.checkAvailabilityAsync(It.IsAny<checkAvailability>()))
                .ReturnsAsync(new checkAvailabilityResponse(
                    new xmbCheckAvailabilityResponse { theSlots = new[] { new daySlotsInfo { day = dateTime } } }));

            // Act
            var drsAppointmentSlots = await systemUnderTest.CheckAvailability(SorCode, LocationId, dateTime);
            Func<IEnumerable<DrsAppointmentSlot>> act = () => drsAppointmentSlots.ToArray();

            // Assert
            act.Should().NotThrow<ArgumentNullException>();
        }
    }
}
