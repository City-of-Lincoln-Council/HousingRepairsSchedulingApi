namespace HousingRepairsSchedulingApi.Tests.ServicesTests.Drs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Microsoft.Extensions.Options;
    using Moq;
    using Services.Drs;
    using Xunit;

    [SuppressMessage("Naming", "CA1707", MessageId = "Identifiers should not contain underscores")]
    public class DrsServiceTests
    {
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
    }
}
