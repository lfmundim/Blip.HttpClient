using Blip.HttpClient.Decorators;
using Lime.Protocol;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace Blip.HttpClient.TestService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            
            var factory = new BlipHttpClientFactory();
            var blipClient = factory.BuildBlipHttpClient("dGVzdGluZ2JvdHM6OU8zZEpWbHVaSWZNYmVnOWZaZzM=");
            var command = new Command()
            {
                Method = CommandMethod.Get,
                Uri = new LimeUri("/contacts")
            };
            var resp = await blipClient.ProcessCommandAsync(command, CancellationToken.None);
        }
    }
}
