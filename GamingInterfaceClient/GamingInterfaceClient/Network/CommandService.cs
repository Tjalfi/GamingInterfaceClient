using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;

namespace GamingInterfaceClient.Network
{
    using Models;

    class CommandService
    {
        private RESTClient client;

        public CommandService(string baseUrl)
        {
            client = new RESTClient(baseUrl);
        }

        public CommandResult<string> GetVersion()
        {
            return client.ProcessJSON<string>(HttpMethod.Get, "/api/Version");
        }

        public CommandResult<List<Result>> PostToggleCommand(string auth, Command command)
        {
            return client.ProcessJSON<List<Result>>(HttpMethod.Post, "/api/toggle", JsonConvert.SerializeObject(command), new Dictionary<string, string>() { { "Authentication", auth } });
        }

        public CommandResult<List<Result>> PostComplexCommand(string auth, Command command)
        {
            return client.ProcessJSON<List<Result>>(HttpMethod.Post, "/api/key", JsonConvert.SerializeObject(command), new Dictionary<string, string>() { { "Authentication", auth } });
        }
    }
}
