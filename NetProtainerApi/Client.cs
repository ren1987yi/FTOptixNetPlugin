using NetProtainerApi.Model;
using System.Text;
using System.Text.Json;

namespace NetProtainerApi
{
    public class Client
    {
        private string _baseAddress;
        private string _apiKey;

        private HttpClient _httpClient;

        public string ApiKey { get => _apiKey; set => _apiKey = value; }

        public Client(string baseAddress, string apiKey)
        {
            _baseAddress = baseAddress.Trim();
            if (!baseAddress.StartsWith('/'))
            {
                _baseAddress += "/";
            }

            _baseAddress += "api/";

            _apiKey = apiKey;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseAddress);

            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);

            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public void Initialization() 
        {
        


        }








        public async Task<long[]> GetEndPoints()
        {

            var url = "endpoints";
            var txt = await HttpGet(url);



            var data = JsonSerializer.Deserialize<EndPoint[]>(txt);
            if(data != null)
            {
                return data.Select(c => c.Id).ToArray();
            }

            return null;
        }



        public async Task<Container[]> ListAllContainers(long id)
        {
            var url = $"endpoints/{id}/docker/containers/json";
            var txt = await HttpGet(url);
            var data = JsonSerializer.Deserialize<Container[]>(txt);
            if (data != null)
            {
                return data.ToArray();
            }

            return null;
        }





        internal async Task<string> HttpGet(string url)
        {
            try
            {

                return await _httpClient.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                return null;
            }
        }



    }
}
