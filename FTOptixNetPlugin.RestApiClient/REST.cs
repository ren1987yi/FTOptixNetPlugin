using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.RestApiClient
{
    public class REST
    {


        #region sync methods

        public static RestApiStringResult Get(string url,Dictionary<string,string?> headers = null)
        {
           
            var httpclient = new HttpClient();

            httpclient.BaseAddress = new Uri(url);

            if(headers != null)
            {
                foreach (var header in headers)
                {
                    httpclient.DefaultRequestHeaders.Add(header.Key,header.Value);
                }
            }

            var response = httpclient.GetAsync(new Uri(url)).GetAwaiter().GetResult();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return new RestApiStringResult(true,result);
            }
            else
            {
                return new RestApiStringResult(false, null);
            }
        }


        public static RestApiStringResult Post(string url, Dictionary<string, string?> headers = null,string content = null,string mediaType= "application/json")
        {
            var httpclient = new HttpClient();

            httpclient.BaseAddress = new Uri(url);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpclient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            StringContent body = new StringContent(content,System.Text.Encoding.UTF8,mediaType);


            var response = httpclient.PostAsync(new Uri(url),body).GetAwaiter().GetResult();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return new RestApiStringResult(true, result);
            }
            else
            {
                return new RestApiStringResult(false, null);
            }
        }

        public static RestApiStringResult Post<T>(string url, Dictionary<string, string?> headers = null, T content = default)
        {
            var httpclient = new HttpClient();

            httpclient.BaseAddress = new Uri(url);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpclient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var response = httpclient.PostAsJsonAsync<T>(new Uri(url), content).GetAwaiter().GetResult();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return new RestApiStringResult(true, result);
            }
            else
            {
                return new RestApiStringResult(false, null);
            }
        }


        #endregion


        #region async methods

        public static async Task<RestApiStringResult> GetAsync(string url, Dictionary<string, string?> headers = null)
        {

            var httpclient = new HttpClient();

            httpclient.BaseAddress = new Uri(url);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpclient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var response = await httpclient.GetAsync(new Uri(url));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return new RestApiStringResult(true, result);
            }
            else
            {
                return new RestApiStringResult(false, null);
            }
        }




        public static async Task<RestApiStringResult> PostAsync(string url, Dictionary<string, string?> headers = null, string content = null, string mediaType = "application/json")
        {
            var httpclient = new HttpClient();

            httpclient.BaseAddress = new Uri(url);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpclient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            StringContent body = new StringContent(content, System.Text.Encoding.UTF8, mediaType);


            var response = await httpclient.PostAsync(new Uri(url), body);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return new RestApiStringResult(true, result);
            }
            else
            {
                return new RestApiStringResult(false, null);
            }
        }

        public static async Task<RestApiStringResult> PostAsync<T>(string url, Dictionary<string, string?> headers = null, T content = default)
        {
            var httpclient = new HttpClient();

            httpclient.BaseAddress = new Uri(url);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpclient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var response = await httpclient.PostAsJsonAsync<T>(new Uri(url), content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return new RestApiStringResult(true, result);
            }
            else
            {
                return new RestApiStringResult(false, null);
            }
        }



        #endregion
    }
}
