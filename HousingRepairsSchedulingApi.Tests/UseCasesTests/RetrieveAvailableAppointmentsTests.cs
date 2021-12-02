using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsSchedulingApi.Gateways;
using HousingRepairsSchedulingApi.UseCases;
using Moq;
using Xunit;
namespace HousingRepairsSchedulingApi.Tests.UseCasesTests
{

    public class RetrieveAvailableAppointmentsTests
    {
        private readonly RetrieveAvailableAppointmentsUseCase sytemUndertest;
        private readonly Mock<IAppointmentsGateway> appointmentsGatewayMock;

        public RetrieveAvailableAppointmentsTests()
        {
            appointmentsGatewayMock = new Mock<IAppointmentsGateway>();
            sytemUndertest = new RetrieveAvailableAppointmentsUseCase(appointmentsGatewayMock.Object);
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidSorCode_WhenExecute_ThenExceptionIsThrown<T>(T exception, string sorCode) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange
            var systemUnderTest = new RetrieveAvailableAppointmentsUseCase(appointmentsGatewayMock.Object);

            // Act
            Func<Task> act = async () => await systemUnderTest.Execute(sorCode, "locationId");

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidLocationId_WhenExecute_ThenExceptionIsThrown<T>(T exception, string locationId) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange
            var systemUnderTest = new RetrieveAvailableAppointmentsUseCase(appointmentsGatewayMock.Object);

            // Act
            Func<Task> act = async () => await systemUnderTest.Execute("uprn", locationId);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        public static IEnumerable<object[]> InvalidArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
            yield return new object[] { new ArgumentException(), " " };
        }

        [Fact]
        public async void GivenParameters_WhenExecute_ThenGetAvailableAppointmentsGatewayIsCalled()
        {
            const string uprn = "uprn";
            const string locationId = "locationId";
            await sytemUndertest.Execute("uprn", "locationId");
            appointmentsGatewayMock.Verify(x => x.GetAvailableAppointments(uprn, locationId), Times.Once);
        }
    }
}
