using System.Net;
using System.Net.Http;
using Xunit;
using Moq;

namespace HousingRepairsSchedulingApi.Tests.GatewaysTests
{
    using Gateways;
    using RichardSzalay.MockHttp;

    public class AppointmentsGatewaysTests
    {
        private readonly IAppointmentsGateway appointmentsGateway;
        private readonly Mock<HttpClient> httpClientMock;
        private readonly MockHttpMessageHandler mockHttp;

        public AppointmentsGatewaysTests()
        {


        }
        [Fact]
        public void ItDoesSomething()
        {
// Arrange

        }
    }
}
