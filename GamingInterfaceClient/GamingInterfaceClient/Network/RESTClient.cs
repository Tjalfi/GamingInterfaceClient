using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GamingInterfaceClient.Network
{
    class RESTClient
    {
        private string baseUrl;
        private HttpClient client;

        public RESTClient(string baseUrl)
        {
            client = new HttpClient();
            this.baseUrl = baseUrl;
        }

        public async Task<CommandResult<T>> ProcessJSONasync<T>(HttpMethod method, string path, string content = "", Dictionary<string, string> headers = null, string contentType = "application/json")
        {
            CommandResult<T> result = new CommandResult<T>();
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(method, baseUrl + path);
                if (content != null && content != "")
                {
                    request.Content = new StringContent(content, Encoding.UTF8, contentType);
                }
                if (headers != null && headers.Count > 0)
                {
                    foreach (KeyValuePair<string, string> pair in headers)
                    {
                        request.Content.Headers.Add(pair.Key, pair.Value);
                    }
                }
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    result.response = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                    result.successful = true;
                }
            }
            catch (Exception e)
            {
                result.error = e.Message + "\n" + e.StackTrace;
                Debug.WriteLine(result.error);
            }
            return result;
        }

        public CommandResult<T> ProcessJSON<T>(HttpMethod method, string path, string content = "", Dictionary<string, string> headers = null, string contentType = "application/json")
        {
            return Task.Run(function: async () => await ProcessJSONasync<T>(method, path, content, headers, contentType)).Result;
        }
    }
}
