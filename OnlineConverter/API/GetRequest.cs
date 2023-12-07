using System.Net;

namespace OnlineConverter.API
{
    public class GetRequest
    {
        private readonly string _address;

        private readonly HttpClient _httpClient;

        public string Response { get; private set; }

        public GetRequest(string addres)
        {
            _address = addres;
            _httpClient = new HttpClient();
        }

        public async Task RunAsync()
        {
            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(_address);

                if (response.IsSuccessStatusCode)
                {
                    Response = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
