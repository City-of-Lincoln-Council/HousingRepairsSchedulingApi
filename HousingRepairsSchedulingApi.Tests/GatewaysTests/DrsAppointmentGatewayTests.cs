namespace HousingRepairsSchedulingApi.Tests.GatewaysTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Gateways;
    using Moq;
    using Services.Drs;
    using Xunit;

    public class DrsAppointmentGatewayTests
    {
        private Mock<IDrsService> drsServiceMock = new();

        [Fact]
#pragma warning disable xUnit1026
        public void GivenNullDrsServiceParameter_WhenInstantiating_ThenExceptionIsThrown()
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Func<DrsAppointmentGateway> act = () => new DrsAppointmentGateway(null);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        public static IEnumerable<object[]> InvalidArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
            yield return new object[] { new ArgumentException(), " " };
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenInvalidSorCode_WhenGettingAvailableAppointments_ThenExceptionIsThrown<T>(T exception, string sorCode) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange
            var systemUnderTest = new DrsAppointmentGateway(this.drsServiceMock.Object);

            // Act
            Func<Task> act = async () => await systemUnderTest.GetAvailableAppointments(sorCode, It.IsAny<string>());

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenInvalidLocationId_WhenGettingAvailableAppointments_ThenExceptionIsThrown<T>(T exception, string locationId) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange
            var sorCode = "sorCode";
            var systemUnderTest = new DrsAppointmentGateway(this.drsServiceMock.Object);


            // Act
            Func<Task> act = async () => await systemUnderTest.GetAvailableAppointments(sorCode, locationId);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Fact]
#pragma warning disable xUnit1026
        public async void GivenNullFromDate_WhenGettingAvailableAppointments_ThenNoExceptionIsThrown()
#pragma warning restore xUnit1026
        {
            // Arrange
            var sorCode = "sorCode";
            var locationId = "locationId";
            var systemUnderTest = new DrsAppointmentGateway(this.drsServiceMock.Object);

            // Act
            Func<Task> act = async () => await systemUnderTest.GetAvailableAppointments(sorCode, locationId, null);

            // Assert
            await act.Should().NotThrowAsync<NullReferenceException>();
        }
    }
}
