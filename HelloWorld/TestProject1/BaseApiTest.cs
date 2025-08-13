using RestSharp;

namespace TestProject1
{
    [TestFixture]
    public abstract class BaseApiTest
    {
        protected RestClient _client;
        protected const string BASE_URL = "https://fakerestapi.azurewebsites.net/api/v1";

        [SetUp]
        public void BaseSetup()
        {
            var options = new RestClientOptions(BASE_URL);
            _client = new RestClient(options);
        }

        [TearDown]
        public void BaseTeardown()
        {
            _client?.Dispose();
        }
    }
}
