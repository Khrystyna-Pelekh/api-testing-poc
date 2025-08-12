using RestSharp;
using System.Net;

namespace TestProject1
{
    [TestFixture]
    public class Tests
    {

        private RestClient _client;
        private const string BASE_URL = "https://fakerestapi.azurewebsites.net/api/v1";
        private const int STATIC_ID = 1;

        [SetUp]
        public void Setup()
        {
            var options = new RestClientOptions(BASE_URL);
            _client = new RestClient(options);
        }

        [TearDown]
        public void Teardown()
        {
            _client?.Dispose();
        }

        [Test]
        [Order(1)]
        public async Task Post_CreatesNewActivity_ReturnsSuccess()
        {
            var uniqueTitle = $"Activity {Guid.NewGuid()}";

            var newActivity = new Activity
            {
                Title = uniqueTitle,
                Completed = false
            };

            var request = new RestRequest("Activities");

            request.AddJsonBody(newActivity);

            var response = await _client.ExecutePostAsync<Activity>(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), $"Expected status 200 OK, but received {response.StatusCode}");

            Assert.That(response.Data, Is.Not.Null, "Response can not be null");
            Assert.That(response.Data.Title, Is.EqualTo(uniqueTitle), "The title should match the one sent.");
        }

        [Test]
        [Order(2)]
        public async Task Get_ExistingActivity_ReturnsCorrectData()
        {
            var request = new RestRequest($"Activities/{STATIC_ID}");

            var response = await _client.ExecuteGetAsync<Activity>(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), $"Expected status 200 OK, but received {response.StatusCode}");

            Assert.That(response.Data, Is.Not.Null, "Response can not be null");
            Assert.That(response.Data.Id, Is.EqualTo(STATIC_ID), "The id should match the one requested.");
        }

        [Test]
        [Order(3)]
        public async Task Put_UpdatesExistingActivity_ReturnsSuccess()
        {
            var updatedTitle = $"Updated Activity {Guid.NewGuid()}";

            var updatePayload = new Activity
            {
                Id = STATIC_ID,
                Title = updatedTitle,
                Completed = true
            };

            var request = new RestRequest($"Activities/{STATIC_ID}");
            request.AddJsonBody(updatePayload);

            var response = await _client.ExecutePutAsync<Activity>(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), $"Expected status 200 OK, but received {response.StatusCode}");

            Assert.That(response.Data, Is.Not.Null, "Response can not be null");
            Assert.That(response.Data.Title, Is.EqualTo(updatedTitle), "The title should be updated.");
        }

        [Test]
        [Order(4)]
        public async Task Delete_ExistingActivity_ReturnsSuccess()
        {
            var request = new RestRequest($"Activities/{STATIC_ID}");

            var response = await _client.ExecuteDeleteAsync(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), $"Expected status 200 OK, but received {response.StatusCode}");
        }
    }
}
