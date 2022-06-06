using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Mango.Web.Models;
using Mango.Web.Services.IServices;

using Newtonsoft.Json;

using static Mango.Web.SD;

namespace Mango.Web.Services
{
    public class BaseService : IBaseService
    {
        public ResponseDto responseModel { get; set; }

        private IHttpClientFactory httpClient;

        public BaseService(IHttpClientFactory httpClient)
        {
            this.httpClient = httpClient;
            responseModel = new ResponseDto();

        }

        public async Task<T> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                HttpClient client = httpClient.CreateClient("MangoApi");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                client.DefaultRequestHeaders.Clear();

                if (apiRequest.Data != null)
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");

                if (!string.IsNullOrEmpty(apiRequest.AccessToken))
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiRequest.AccessToken);

                message.Method = GetHttpMethod(apiRequest.ApiType);
                HttpResponseMessage responseMessage = new HttpResponseMessage();

                responseMessage = await client.SendAsync(message);
                var apiContent = await responseMessage.Content.ReadAsStringAsync();
                var apiResponseDto = JsonConvert.DeserializeObject<T>(apiContent);

                return apiResponseDto;
            }
            catch(Exception ex) {
                
                var dto = new ResponseDto
                {
                    DisplayMessage = "Error",
                    ErrorMessages = new List<string> { JsonConvert.SerializeObject(ex)},
                    IsSuccess  = true
                };

                var res = JsonConvert.SerializeObject(dto);
                var apiResponseDto = JsonConvert.DeserializeObject<T>(res);
                return apiResponseDto;
            }

        }

        public void Dispose()
        {
            GC.SuppressFinalize(true);
        }

        private HttpMethod GetHttpMethod(ApiType apiType)
        {

            switch (apiType)
            {
                case ApiType.POST:
                    return HttpMethod.Post;
                case ApiType.PUT:
                    return HttpMethod.Put;
                case ApiType.DELETE:
                    return HttpMethod.Delete;
                default:
                    return HttpMethod.Get;
            
            }


        }
    }
}
