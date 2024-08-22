using EnergiOverblikApp.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace EnergiOverblikApp.Services
{
    public class ElOverblikService
    {
        private readonly HttpClient client = new HttpClient();
        private readonly string refreshToken = "InsertApiKeyHere"; // https://eloverblik.dk/customer/data-sharing

        public async Task<string> GetDataAccessTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.eloverblik.dk/customerapi/api/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);
            return tokenResponse.AccessToken;
        }

        public async Task<List<MeteringPoint>> GetMeteringPointsAsync(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.eloverblik.dk/customerapi/api/meteringpoints/meteringpoints");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var meteringPointResponse = JsonConvert.DeserializeObject<MeteringPointResponse>(content);

            return meteringPointResponse.Result;
        }

        public async Task<TimeSeriesResponse> GetTimeSeriesAsync(string accessToken, string meteringPointId, DateTime startDate, DateTime endDate, string period)
        {
            string url = $"https://api.eloverblik.dk/customerapi/api/meterdata/gettimeseries/{startDate.ToString("yyyy-MM-dd")}/{endDate.ToString("yyyy-MM-dd")}/{period}";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("api-version", "1.0");

            var body = new
            {
                meteringPoints = new
                {
                    meteringPoint = new[] { meteringPointId }
                }
            };

            var jsonBody = JsonConvert.SerializeObject(body);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var timeSeriesResponse = JsonConvert.DeserializeObject<TimeSeriesResponse>(content);
            return timeSeriesResponse;
        }
    }
}