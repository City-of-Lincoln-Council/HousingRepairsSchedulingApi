namespace HousingRepairsSchedulingApi.Tests.GatewaysTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using FluentAssertions;
    using Gateways;
    using Moq;
    using Services.Drs;
    using Xunit;

    public class DrsAppointmentGatewayTests
    {
        private Mock<IDrsService> drsServiceMock = new();
        private DrsAppointmentGateway systemUnderTest;
        private const int RequiredNumberOfAppointmentDays = 5;
        private const int AppointmentSearchTimeSpanInDays = 14;
        private const int AppointmentLeadTimeInDays = 0;

        public DrsAppointmentGatewayTests()
        {
            systemUnderTest = new DrsAppointmentGateway(
                this.drsServiceMock.Object,
                RequiredNumberOfAppointmentDays,
                AppointmentSearchTimeSpanInDays,
                AppointmentLeadTimeInDays);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenNullDrsServiceParameter_WhenInstantiating_ThenArgumentNullExceptionIsThrown()
#pragma warning restore CA1707
        {
            // Arrange

            // Act
            Func<DrsAppointmentGateway> act = () => new DrsAppointmentGateway(
                null,
                default,
                default,
                default);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
#pragma warning disable CA1707
        public void GivenInvalidRequiredNumberOfAppointmentsParameter_WhenInstantiating_ThenArgumentExceptionIsThrown(int invalidRequiredNumberOfAppointments)
#pragma warning restore CA1707
        {
            // Arrange

            // Act
            Func<DrsAppointmentGateway> act = () => new DrsAppointmentGateway(
                this.drsServiceMock.Object,
                invalidRequiredNumberOfAppointments,
                default,
                default);

            // Assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenInvalidAppointmentLeadTimeInDaysParameter_WhenInstantiating_ThenArgumentExceptionIsThrown()
#pragma warning restore CA1707
        {
            // Arrange

            // Act
            Func<DrsAppointmentGateway> act = () => new DrsAppointmentGateway(
                this.drsServiceMock.Object,
                1,
                1,
                -1);

            // Assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
#pragma warning disable CA1707
        public void GivenInvalidAppointmentSearchTimeSpanInDaysParameter_WhenInstantiating_ThenArgumentExceptionIsThrown(int invalidAppointmentSearchTimeSpanInDays)
#pragma warning restore CA1707
        {
            // Arrange

            // Act
            Func<DrsAppointmentGateway> act = () => new DrsAppointmentGateway(
                this.drsServiceMock.Object,
                1,
                invalidAppointmentSearchTimeSpanInDays,
                default);

            // Assert
            act.Should().ThrowExactly<ArgumentException>();
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
#pragma warning disable CA1707
        public async void GivenInvalidSorCode_WhenGettingAvailableAppointments_ThenExceptionIsThrown<T>(T exception, string sorCode) where T : Exception
#pragma warning restore CA1707
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Func<Task> act = async () => await systemUnderTest.GetAvailableAppointments(sorCode, It.IsAny<string>());

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
#pragma warning disable CA1707
        public async void GivenInvalidLocationId_WhenGettingAvailableAppointments_ThenExceptionIsThrown<T>(T exception, string locationId) where T : Exception
#pragma warning restore CA1707
#pragma warning restore xUnit1026
        {
            // Arrange
            var sorCode = "sorCode";

            // Act
            Func<Task> act = async () => await systemUnderTest.GetAvailableAppointments(sorCode, locationId);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Fact]
        public async void
            GivenLocationIdWithTrailingWhitespace_WhenGettingAvailableAppointments_ThenSanitisedLocationIdIsUsedForRequests()
        {
            // Arrange
            var sorCode = "sorCode";
            var locationId = "12345    ";
            drsServiceMock.Setup(x => x.CheckAvailability(It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(new[] { new AppointmentSlot() });
            systemUnderTest = new DrsAppointmentGateway(
                this.drsServiceMock.Object,
                1,
                AppointmentSearchTimeSpanInDays,
                AppointmentLeadTimeInDays);

            // Act
            _ = await systemUnderTest.GetAvailableAppointments(sorCode, locationId);

            // Assert
            drsServiceMock.Verify(x => x.CheckAvailability(It.IsAny<string>(),
                locationId.TrimEnd(),
                It.IsAny<DateTime>()));
        }

        [Fact]
#pragma warning disable CA1707
        public async void GivenNullFromDate_WhenGettingAvailableAppointments_ThenNoExceptionIsThrown()
#pragma warning restore CA1707
        {
            // Arrange
            var sorCode = "sorCode";
            var locationId = "locationId";
            drsServiceMock.Setup(x => x.CheckAvailability(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>())).ReturnsAsync(CreateAppointmentsForSequentialDays(new DateTime(2022, 1, 17), 5));

            // Act
            Func<Task> act = async () => await systemUnderTest.GetAvailableAppointments(sorCode, locationId, null);

            // Assert
            await act.Should().NotThrowAsync<NullReferenceException>();
        }

        [Theory]
        [MemberData(nameof(DrsServiceHasFiveAvailableAppointmentsTestData))]
#pragma warning disable CA1707
        public async void GivenDrsServiceHasFiveDaysOfAvailableAppointments_WhenGettingAvailableAppointments_ThenFiveDaysOfAppointmentsAreReturned(IEnumerable<IEnumerable<AppointmentSlot>> appointmentReturnSequence)
#pragma warning restore CA1707
        {
            // Arrange
            var sorCode = "sorCode";
            var locationId = "locationId";

            var setupSequentialResult = drsServiceMock.SetupSequence(x => x.CheckAvailability(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>()));

            foreach (var appointments in appointmentReturnSequence)
            {
                setupSequentialResult = setupSequentialResult.ReturnsAsync(appointments);
            }

            // Act
            var actualAppointments = await systemUnderTest.GetAvailableAppointments(sorCode, locationId);

            // Assert
            Assert.Equal(RequiredNumberOfAppointmentDays, actualAppointments.Select(x => x.StartTime.Date).Distinct().Count());
        }

        public static IEnumerable<object[]> DrsServiceHasFiveAvailableAppointmentsTestData()
        {
            // single appointment per day
            yield return new object[] { new[]
            {
                CreateAppointmentsForDay(new DateTime(2022, 1, 18), true),
                CreateAppointmentsForDay(new DateTime(2022, 1, 19), true),
                CreateAppointmentsForDay(new DateTime(2022, 1, 20), true),
                CreateAppointmentsForDay(new DateTime(2022, 1, 21), true),
                CreateAppointmentsForDay(new DateTime(2022, 1, 22), true),
            }};
            yield return new object[] { new[]
            {
                CreateAppointmentsForDay(new DateTime(2022, 1, 18), include0800To1200: true)
                    .Concat(CreateAppointmentsForDay(new DateTime(2022, 1, 19), include0800To1200: true)),
                CreateAppointmentsForDay(new DateTime(2022, 1, 20), include0800To1200: true),
                CreateAppointmentsForDay(new DateTime(2022, 1, 21), include0800To1200: true),
                CreateAppointmentsForDay(new DateTime(2022, 1, 22), include0800To1200: true),
            }};
            yield return new object[] { new[]
            {
                CreateAppointmentsForDay(new DateTime(2022, 1, 18), include0800To1200: true)
                    .Concat(CreateAppointmentsForDay(new DateTime(2022, 1, 19), include0800To1200: true)),
                CreateAppointmentsForDay(new DateTime(2022, 1, 20), include0800To1200: true)
                    .Concat(CreateAppointmentsForDay(new DateTime(2022, 1, 21), include0800To1200: true)),
                CreateAppointmentsForDay(new DateTime(2022, 1, 22), include0800To1200: true),
            }};
            yield return new object[] { new[]
            {
                CreateAppointmentsForDay(new DateTime(2022, 1, 18), true)
                    .Concat(CreateAppointmentsForDay(new DateTime(2022, 1, 19), true)),
                Array.Empty<AppointmentSlot>(),
                CreateAppointmentsForDay(new DateTime(2022, 1, 20), true)
                    .Concat(CreateAppointmentsForDay(new DateTime(2022, 1, 21), true)),
                CreateAppointmentsForDay(new DateTime(2022, 1, 22), true),
            }};

            // multiple appointments per day
            yield return new object[] { new[]
            {
                CreateAppointmentsForDay(new DateTime(2022, 1, 18), true, true),
                CreateAppointmentsForDay(new DateTime(2022, 1, 19), true, true),
                CreateAppointmentsForDay(new DateTime(2022, 1, 20), true, true),
                CreateAppointmentsForDay(new DateTime(2022, 1, 21), true, true),
                CreateAppointmentsForDay(new DateTime(2022, 1, 22), true, true),
            }};
            yield return new object[] { new[]
            {
                CreateAppointmentsForDay(new DateTime(2022, 1, 18), true, true)
                    .Concat(CreateAppointmentsForDay(new DateTime(2022, 1, 19), true, true)),
                CreateAppointmentsForDay(new DateTime(2022, 1, 20), true, true),
                CreateAppointmentsForDay(new DateTime(2022, 1, 21), true, true),
                CreateAppointmentsForDay(new DateTime(2022, 1, 22), true, true),
            }};
        }

        private static IEnumerable<AppointmentSlot> CreateAppointmentsForDay(DateTime dateTime,
            bool include0800To1200 = false,
            bool include1200To1600 = false,
            bool include0930To1430 = false)
        {
            var result = new List<AppointmentSlot>();

            if (include0800To1200)
            {
                result.Add(
                    new()
                    {
                        StartTime = dateTime.AddHours(8),
                        EndTime = dateTime.AddHours(12),
                    }
                );
            }

            if (include0930To1430)
            {
                result.Add(
                    new()
                    {
                        StartTime = dateTime.AddHours(9).AddMinutes(30),
                        EndTime = dateTime.AddHours(14).AddMinutes(30),
                    }
                );
            }

            if (include1200To1600)
            {
                result.Add(
                    new()
                    {
                        StartTime = dateTime.AddHours(12),
                        EndTime = dateTime.AddHours(16),
                    }
                );
            }

            return result;
        }

        private static AppointmentSlot[] CreateAppointmentsForSequentialDays(DateTime firstDate, int numberOfDays)
        {
            var appointments = Enumerable.Range(0, numberOfDays).Select(x => CreateAppointmentForDay(firstDate.AddDays(x))).ToArray();

            return appointments;

            AppointmentSlot CreateAppointmentForDay(DateTime date)
            {
                return CreateAppointmentsForDay(date, true).Single();
            }
        }

        [Fact]
#pragma warning disable CA1707
        public async void GivenDrsServiceHasAvailableAppointmentsThatAreNotRequired_WhenGettingAvailableAppointments_ThenOnlyValidAppointmentsAreReturned()
#pragma warning restore CA1707
        {
            // Arrange
            var sorCode = "sorCode";
            var locationId = "locationId";

            systemUnderTest = new DrsAppointmentGateway(
                this.drsServiceMock.Object,
                1,
                AppointmentSearchTimeSpanInDays,
                AppointmentLeadTimeInDays);

            drsServiceMock.SetupSequence(x => x.CheckAvailability(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(CreateAppointmentsForDay(new DateTime(2022, 1, 17), true, true, true));

            var expected = new[]
                {
                    new AppointmentSlot
                    {
                        StartTime = new DateTime(2022, 1, 17, 8, 0, 0),
                        EndTime = new DateTime(2022, 1, 17, 12, 0, 0),
                    },
                    new AppointmentSlot
                    {
                        StartTime = new DateTime(2022, 1, 17, 12, 0, 0),
                        EndTime = new DateTime(2022, 1, 17, 16, 0, 0),
                    },
                };

            // Act
            var actualAppointments = await systemUnderTest.GetAvailableAppointments(sorCode, locationId);

            // Assert
            actualAppointments.Should().BeEquivalentTo(expected);
        }

        [Fact]
#pragma warning disable CA1707
        public async void GivenDrsServiceRequiresMultipleRequests_WhenGettingAvailableAppointments_ThenCorrectTimeSpanIncrementApplied()
#pragma warning restore CA1707
        {
            // Arrange
            var sorCode = "sorCode";
            var locationId = "locationId";

            drsServiceMock.Setup(x => x.CheckAvailability(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    new DateTime(2022, 1, 17)))
                .ReturnsAsync(CreateAppointmentsForSequentialDays(new DateTime(2022, 1, 17), RequiredNumberOfAppointmentDays - 2));
            drsServiceMock.Setup(x => x.CheckAvailability(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        new DateTime(2022, 1, 31)))
                .ReturnsAsync(CreateAppointmentsForSequentialDays(new DateTime(2022, 1, 31),
                    RequiredNumberOfAppointmentDays - (RequiredNumberOfAppointmentDays - 2)));

            // Act
            _ = await systemUnderTest.GetAvailableAppointments(sorCode, locationId, new DateTime(2022, 1, 17));

            // Assert
            drsServiceMock.VerifyAll();
        }
    }
}
