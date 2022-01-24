using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NMVS.Common
{
    public class MonitoringService
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task SendErrorMessage(string mess)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            var stringTask =
                client.GetAsync("https://api.telegram.org/bot5050526038:AAFZ93LoEso8IwDkin50rYrej22oz9XvpMU/sendMessage?chat_id=607758592&text=[Aica exception Alert]%0A" + mess);

            var msg = await stringTask;
            Console.Write(msg);
        }
    }
}