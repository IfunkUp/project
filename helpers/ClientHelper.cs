using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SyncWPF.helpers
{
    class ClientHelper
    {
        public static HttpClient GetClient(string username, string password)
        {
            var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
            var Client = new HttpClient() { DefaultRequestHeaders = { Authorization = authValue } };
            return Client;
        }
    }
}
